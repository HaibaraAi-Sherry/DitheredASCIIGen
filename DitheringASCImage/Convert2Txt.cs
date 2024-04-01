using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DitheringASCImage {
    public struct Setting(string characters, Size outSize) {
        public string characters = characters;
        /// <summary>
        /// 设置输出的尺寸，宽度为字符数，高度为行数，高度为-1时表示自动计算
        /// </summary>
        public Size outSize = outSize;
        public Font font = new("Lucida Console", 8);
        public bool dither = true;
    }


    public class Convert2Txt : IDisposable {
        public Bitmap CurrentPicture { get; set; }

        private Setting _setting;

        /// <summary>
        /// 灰度矩阵，未抖动处理，需要手动调用GetGrayMatrix()生成
        /// </summary>
        private byte[,]? _matrix;

        /// <summary>
        /// 按照字符集抖动处理后的灰度矩阵，若不抖动则不采用该矩阵。
        /// </summary>
        private byte[,]? _ditheredMatrix;

        /// <summary>
        /// 字符尺寸，自动计算
        /// </summary>
        private Size _textSize;

        /// <summary>
        /// 字符的高宽比，（一般大于1）自动计算
        /// </summary>
        private double _textRatio;

        /// <summary>
        /// 自动缩放后的图片
        /// </summary>
        private Bitmap? _scaledPic;

        /// <summary>
        /// 每个字符对应的灰度值，自动计算
        /// </summary>
        private List<CharPoint>? _textPoints;

        /// <summary>
        /// 灰度值对应的CharPoint结构体的索引值，自动计算
        /// </summary>
        private int[]? _nearestCharPoint;

        /// <summary>
        /// 用于绑定字符和灰度值的结构体，自动计算
        /// </summary>
        private readonly record struct CharPoint(char c, byte gray) {
            readonly public char c = c;
            readonly public byte gray = gray;
        }

        /// <summary>
        /// 输出设置，包括字符集和输出尺寸，尺寸的高度为-1时表示自动计算
        /// 若要在实例化对象后进行更新，请调用其他Change方法
        /// </summary>
        public Setting OutputSetting {
            get {
                if (_scaledPic is null) {
                    throw new InvalidOperationException("实例还没有初始化完毕");
                }
                return this._setting with { outSize = new(_scaledPic.Width, _scaledPic.Height) };
            }
            // 初始化时，自动计算相应属性
            init {
                this._setting = value;
                // 计算字符的宽高比和文本尺寸
                UpdateRatio();
                UpdatePoints();
                UpdateNearestChar();
                // 计算图像相关
                UpdateScaledBitmap();
                UpdateGrayMatrix();
                UpdateDitheredMatrix();
            }
        }


        #region ChangeMethods
        public void ChangePicture(Bitmap pic) {
            this.CurrentPicture.Dispose();
            this.CurrentPicture = pic;
            UpdateScaledBitmap();
            UpdateGrayMatrix();
            UpdateDitheredMatrix();
        }

        /// <summary>
        /// 更新字符集，注意字符集的种类不能小于2，并且有可能改变字符宽高比
        /// </summary>
        /// <param name="characters">字符集</param>
        public void ChangeCharacter(string characters) {
            if (characters.ToHashSet().Count <= 1) {
                throw new ArgumentException("字符种类过少");
            }
            this._setting.characters = characters;
            var res = UpdateRatio();
            UpdatePoints();
            UpdateNearestChar();
            if (res) {
                UpdateScaledBitmap();
            }
            UpdateDitheredMatrix();
        }

        public void ChangeSize(Size size) {
            this._setting.outSize = size;
            if (size.Width == 0) {
                return;
            }
            if (UpdateHeight().Height == 0) {
                return;
            }
            UpdateScaledBitmap();
            UpdateGrayMatrix();
            UpdateDitheredMatrix();
        }

        public void ChangeFont(Font font) {
            this._setting.font = font;
            var res = UpdateRatio();
            UpdatePoints();
            UpdateNearestChar();
            if (res) {
                UpdateScaledBitmap();
                UpdateGrayMatrix();
            }
            UpdateDitheredMatrix();
        }

        public void ChangeIsDither(bool isDither) {
            this._setting.dither = isDither;
            UpdateDitheredMatrix();
        }
        #endregion

        private static byte ToGray(Color color) =>
            (byte)((color.R * 76 + color.G * 150 + color.B * 30) >> 8);

        #region UpdateMethods
        /// <summary>
        /// 在输出尺寸改变时，自动更新缩放后的图片
        /// </summary>
        private void UpdateScaledBitmap() {
            var res = UpdateHeight();
            this._scaledPic?.Dispose();
            this._scaledPic = new Bitmap(CurrentPicture, res);
        }

        /// <summary>
        /// 在更换字体，或者更换字符集后，自动更新字符尺寸和字符的高宽比
        /// </summary>
        /// <returns>返回字符比例是否发生了变化</returns>
        private bool UpdateRatio() {
            ArgumentNullException.ThrowIfNull(_setting.font);
            using Bitmap bitmap = new(100, 100);
            using Graphics gr = Graphics.FromImage(bitmap);
            // 不能用任意字符，要防空格
            var size = gr.MeasureString((
                _setting.characters[0] == ' ' ? _setting.characters[1] : _setting.characters[0]).ToString()
                , _setting.font);
            this._textSize = new((int)(size.Width + 0.5), (int)(size.Height + 0.5));
            var curRatio = size.Height / size.Width;
            if (_textRatio == curRatio) {
                return false;
            }
            this._textRatio = size.Height / size.Width + 0.4;
            return true;
        }


        /// <summary>
        /// 在字符集改变时，自动更新字符对应的灰度值
        /// </summary>
        private void UpdatePoints() {
            if (string.IsNullOrEmpty(_setting.characters)
                || this._setting.characters.Length < 2) {
                throw new ArgumentException("字符集长度不能小于2");
            }
            this._textPoints = [];
            Font font = this._setting.font;
            using Bitmap bg = new(_textSize.Width, _textSize.Height);
            using Graphics g = Graphics.FromImage(bg);
            foreach (var item in this._setting.characters) {
                g.Clear(Color.White);
                g.DrawString(item.ToString(), font, Brushes.Black, 0, 0);
                int whitePixel = 0;
                for (int i = 0; i < bg.Width; i++) {
                    for (int j = 0; j < bg.Height; j++) {
                        if (ToGray(bg.GetPixel(i, j)) == 255) {
                            whitePixel++;
                        }
                    }
                }
                //bg.Save("H:\\a.png");
                this._textPoints.Add(new CharPoint(item, (byte)(whitePixel * 255 / (bg.Width * bg.Height))));
            }

            // 线性变换，使得灰度值更加均匀
            // _textPoints[0] -> 0, _textPoints[^1] -> 255
            _textPoints.Sort((a, b) => a.gray.CompareTo(b.gray));
            byte min = _textPoints[0].gray;
            byte max = _textPoints[^1].gray;
            for (int i = 0; i < _textPoints.Count; i++) {
                _textPoints[i] = new CharPoint(_textPoints[i].c, (byte)((_textPoints[i].gray - min) * 255 / (max - min)));
            }
        }

        private Size UpdateHeight() {
            Size size = _setting.outSize;
            int height = size.Height;
            int width = size.Width;
            // 说明需要自动计算高度
            if (height == -1) {
                double h = CurrentPicture.Height * width * 1.0 / CurrentPicture.Width;
                h /= this._textRatio;

                size.Height = (int)h;
                return size;
            }

            return size;
        }

        /// <summary>
        /// 在缩放图片后更新灰度矩阵
        /// </summary>
        /// <exception cref="InvalidOperationException">当没有缩放的图片时，引发异常</exception>
        private void UpdateGrayMatrix() {
            ArgumentNullException.ThrowIfNull(_scaledPic);

            _matrix = new byte[_scaledPic.Height, _scaledPic.Width];

            BitmapData srcData = _scaledPic.LockBits(new(0, 0, _scaledPic.Width, _scaledPic.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            // 数据存放格式：BGRA，各占一个字节
            unsafe {
                byte* p = (byte*)srcData.Scan0;
                for (int i = 0; i < srcData.Height; i++) {
                    for (int j = 0; j < srcData.Width; j++) {
                        _matrix[i, j] = ToGray(Color.FromArgb(p[2], p[1], p[0]));
                        p += 4;
                    }
                }
            }
            _scaledPic.UnlockBits(srcData);

        }

        /// <summary>
        /// 在字符集更改时，更新字符对应的灰度值
        /// </summary>
        private void UpdateNearestChar() {
            ArgumentNullException.ThrowIfNull(_textPoints);

            _nearestCharPoint = new int[256];
            _textPoints!.Sort((a, b) => a.gray - b.gray);
            // 填充已经确定的灰度值
            for (int i = 0; i < _textPoints.Count; i++) {
                _nearestCharPoint[_textPoints[i].gray] = i;
            }

            // 先填充除去开头和末尾的灰度值
            int idx1 = 0;
            int idx2 = 1;

            while (true) {
                int m = (_textPoints[idx1].gray + _textPoints[idx2].gray) >> 1;
                Array.Fill(_nearestCharPoint, idx1, _textPoints[idx1].gray, m - _textPoints[idx1].gray + 1);
                Array.Fill(_nearestCharPoint, idx2, m + 1, _textPoints[idx2].gray - (m + 1) + 1);

                idx1 += 1;
                idx2 += 1;
                if (idx2 >= _textPoints.Count) {
                    break;
                }
            }

            // 填充开头和末尾的灰度值
            Array.Fill(_nearestCharPoint, 0, 0, _textPoints[0].gray);
            Array.Fill(_nearestCharPoint, _textPoints.Count - 1, _textPoints[^1].gray, 256 - _textPoints[^1].gray);
        }

        /// <summary>
        /// 根据缩放后的图片，更新抖动处理后的灰度矩阵。<br />
        /// 需要保证_matrix不为空
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private void UpdateDitheredMatrix() {
            if (_matrix is null) {
                throw new InvalidOperationException();
            }
            if (!_setting.dither) {
                return;
            }
            _ditheredMatrix = new byte[_matrix.GetLength(0), _matrix.GetLength(1)];
            for (int i = 0; i < _matrix.GetLength(0); i++) {
                for (int j = 0; j < _matrix.GetLength(1); j++) {
                    // 近似的灰度值：_nearestChars[_matrix[i, j]]
                    _ditheredMatrix[i, j] =
                        ColorClipping(_matrix[i, j] + _ditheredMatrix[i, j]);

                    byte choice = _textPoints[_nearestCharPoint[_ditheredMatrix[i, j]]].gray;
                    int error = _ditheredMatrix[i, j] - choice;

                    _ditheredMatrix[i, j] = choice;
                    error >>= 2;
                    if (j + 1 < _matrix.GetLength(1)) {
                        _ditheredMatrix[i, j + 1] = ColorClipping(_ditheredMatrix[i, j + 1] + error);
                    }
                    if (i + 1 < _matrix.GetLength(0)) {
                        _ditheredMatrix[i + 1, j] = ColorClipping(_ditheredMatrix[i + 1, j] + error * 2);
                    }
                    if (j + 2 < _matrix.GetLength(1)) {
                        _ditheredMatrix[i, j + 2] = ColorClipping(_ditheredMatrix[i, j + 2] + error);
                    }
                }
            }

        }
        #endregion

        private static byte ColorClipping(int color) {
            if ((color & 255) == color) {
                return (byte)color;
            }
            if (color < 0) {
                return 0;
            }
            return 255;
        }
        public Convert2Txt(Bitmap pic, Setting setting) {
            this.CurrentPicture = pic;
            this.OutputSetting = setting;
        }

        public Convert2Txt(Bitmap pic) {
            this.CurrentPicture = pic;
            this.OutputSetting = new Setting("M@WB08Za2SX7r;i:;. ", new Size(200, -1));
        }

        /// <summary>
        /// 在输出尺寸改变时，自动更新计算尺寸，高度为-1时自动计算
        /// </summary>


        public string Convert() {
            //_ditheredMatrix = _matrix;
            return GenerateTxt();
        }

        private string GenerateTxt() {
            ArgumentNullException.ThrowIfNull(_nearestCharPoint);
            ArgumentNullException.ThrowIfNull(_textPoints);
            if (_scaledPic is null) {
                return string.Empty;
            }
            // 根据需要采用不同的矩阵
            byte[,] desiredMatrix =
                _setting.dither ? _ditheredMatrix! : _matrix!;

            StringBuilder sb = new();
            for (int i = 0; i < desiredMatrix!.GetLength(0); i++) {
                for (int j = 0; j < desiredMatrix.GetLength(1); j++) {
                    sb.Append((_textPoints[_nearestCharPoint[desiredMatrix[i, j]]].c));
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }


        public void Dispose() {
            _scaledPic?.Dispose();
            CurrentPicture.Dispose();
        }
    }
}
