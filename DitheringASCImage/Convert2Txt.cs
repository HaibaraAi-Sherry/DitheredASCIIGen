using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Common;
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
        public Font font = new("Lucida Console", 10);
    }


    public class Convert2Txt : IDisposable {
        public Bitmap CurrentPicture { get; set; }

        private Setting _setting;

        /// <summary>
        /// 灰度矩阵，未抖动处理，需要手动调用GetGrayMatrix()生成
        /// </summary>
        private byte[,]? _matrix;

        /// <summary>
        /// 按照字符集抖动处理后的灰度矩阵
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

        private char[] _nearestChars = new char[256];

        /// <summary>
        /// 用于绑定字符和灰度值的结构体，自动计算
        /// </summary>
        private readonly record struct CharPoint(char c, int gray) {
            readonly public char c = c;
            readonly public int gray = gray;
        }

        /// <summary>
        /// 输出设置，包括字符集和输出尺寸，尺寸的高度为-1时表示自动计算
        /// 若要在实例化对象后进行更新，请调用其他Change方法
        /// </summary>
        public Setting OutputSetting {
            get {
                return this._setting with { outSize = new(_scaledPic.Width, _scaledPic.Height) };
            }
            // 初始化时，自动计算相应属性
            init {
                if (value.characters.Length < 2) {
                    throw new ArgumentException("字符集长度至少为2");
                }
                if (value.font is null) {
                    throw new ArgumentNullException(nameof(value.font));
                }
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

        public void ChangeCharacter(string characters) {
            this._setting.characters = characters;
            UpdatePoints();
            UpdateNearestChar();
            UpdateDitheredMatrix();
        }

        public void ChangeSize(Size size) {
            this._setting.outSize = size;
            if (size.Width == 0) {
                return;
            }
            if(UpdateHeight().Height == 0) {
                return;
            }
            UpdateScaledBitmap();
            UpdateGrayMatrix();
            UpdateDitheredMatrix();
        }

        #endregion

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
        /// 在更换字体后，自动更新字符尺寸和字符的高宽比
        /// </summary>
        private void UpdateRatio() {
            using Graphics gr = Graphics.FromImage(new Bitmap(1, 1));
            this._textSize = gr.MeasureString(_setting.characters[0].ToString(), _setting.font).ToSize();

            this._textRatio = (_textSize.Height + 8) * 1.0 / _textSize.Width;
        }


        /// <summary>
        /// 在字符集改变时，自动更新字符对应的灰度值
        /// </summary>
        private void UpdatePoints() {
            this._textPoints = new();
            Font font = this._setting.font;
            using Bitmap bg = new(_textSize.Width + 2, _textSize.Height + 2);
            using Graphics g = Graphics.FromImage(bg);
            foreach (var item in this._setting.characters) {
                g.Clear(Color.White);
                g.DrawString(item.ToString(), font, Brushes.Black, 0, 0);
                int blackPixel = 0;
                for (int i = 0; i < bg.Width; i++) {
                    for (int j = 0; j < bg.Height; j++) {
                        if (bg.GetPixel(i, j).R < 80) {
                            blackPixel++;
                        }
                    }
                }
                this._textPoints.Add(new CharPoint(item, blackPixel * 255 / (bg.Width * bg.Height)));
            }

            // 线性变换，使得灰度值更加均匀
            // _textPoints[0] -> 0, _textPoints[^1] -> 255
            _textPoints.Sort((a, b) => a.gray.CompareTo(b.gray));
            int min = _textPoints[0].gray;
            int max = _textPoints[^1].gray;
            for (int i = 0; i < _textPoints.Count; i++) {
                _textPoints[i] = new CharPoint(_textPoints[i].c, 255 - (_textPoints[i].gray - min) * 255 / (max - min));
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

                return size with { Height = (int)h };
            }

            return size;
        }

        /// <summary>
        /// 在缩放图片后更新灰度矩阵
        /// </summary>
        /// <exception cref="InvalidOperationException">当没有缩放的图片时，引发异常</exception>
        private void UpdateGrayMatrix() {
            if (_scaledPic is null) {
                throw new InvalidOperationException();
            }
            _matrix = new byte[_scaledPic.Height, _scaledPic.Width];
            for (int i = 0; i < _scaledPic.Height; i++) {
                for (int j = 0; j < _scaledPic.Width; j++) {
                    var pixel = _scaledPic.GetPixel(j, i);
                    var gray = (pixel.R * 299 + pixel.G * 587 + pixel.B * 114 + 500) / 1000;
                    gray = ColorClipping(gray);
                    _matrix[i, j] = (byte)gray;
                }
            }
        }

        /// <summary>
        /// 在字符集更改时，更新字符对应的灰度值
        /// </summary>
        private void UpdateNearestChar() {
            if (_textPoints is null) {
                UpdatePoints();
            }

            _nearestChars = new char[256];
            _textPoints!.Sort((a, b) => a.gray - b.gray);
            // 填充已经确定的灰度值
            for (int i = 0; i < _textPoints.Count; i++) {
                _nearestChars[_textPoints[i].gray] = _textPoints[i].c;
            }

            // 先填充除去开头和末尾的灰度值
            int idx1 = 0;
            int idx2 = 1;

            while (true) {
                int m = (_textPoints[idx1].gray + _textPoints[idx2].gray) >> 1;
                Array.Fill(_nearestChars, _textPoints[idx1].c, _textPoints[idx1].gray, m - _textPoints[idx1].gray + 1);
                Array.Fill(_nearestChars, _textPoints[idx2].c, m + 1, _textPoints[idx2].gray - (m + 1) + 1);

                idx1 += 1;
                idx2 += 1;
                if (idx2 >= _textPoints.Count) {
                    break;
                }
            }

            // 填充开头和末尾的灰度值
            Array.Fill(_nearestChars, _textPoints[0].c, 0, _textPoints[0].gray);
            Array.Fill(_nearestChars, _textPoints[^1].c, _textPoints[^1].gray, 256 - _textPoints[^1].gray);
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
            _ditheredMatrix = new byte[_matrix.GetLength(0), _matrix.GetLength(1)];
            for (int i = 0; i < _matrix.GetLength(0); i++) {
                for (int j = 0; j < _matrix.GetLength(1); j++) {
                    // 近似的灰度值：_nearestChars[_matrix[i, j]]
                    _ditheredMatrix[i, j] += _matrix[i, j];
                    _ditheredMatrix[i, j] = ColorClipping(_ditheredMatrix[i, j]);

                    int error = _matrix[i, j] -
                        _textPoints!.Find((a) => a.c == _nearestChars[_ditheredMatrix[i, j]]).gray;
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
            if (color < 0) {
                return 0;
            } else if (color > 255) {
                return 255;
            }
            return (byte)color;
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
            if (_scaledPic is null) {
                return string.Empty;
            }

            StringBuilder sb = new();
            for (int i = 0; i < _ditheredMatrix!.GetLength(0); i++) {
                for (int j = 0; j < _ditheredMatrix.GetLength(1); j++) {
                    sb.Append(_nearestChars[_ditheredMatrix[i, j]]);
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
