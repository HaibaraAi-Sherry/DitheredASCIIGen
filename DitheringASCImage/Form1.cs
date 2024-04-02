using System.Windows.Forms.VisualStyles;

namespace DitheringASCImage {
    public partial class Form1 : Form {
        Convert2Txt? c2t = null;

        public Form1() {
            InitializeComponent();
        }

        private bool SaveFile() {
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = "txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    File.WriteAllText(saveFileDialog.FileName, tB.Text);
                } catch (Exception e) {
                    MessageBox.Show($"保存失败：{e.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                return true;
            }
            return false;
        }

        private bool AskToSave() {
            var res = MessageBox.Show("结果已更改，是否保存当前文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return res == DialogResult.OK;
        }

        private void OpenAndConvert(string path) {
            try {
                Bitmap pic = new(path);
                if (c2t != null) {
                    c2t.ChangePicture(pic);
                } else {
                    c2t = new(pic);
                    c2t.CreateFileMonitor(SaveFile, AskToSave);
                }
                tB.Text = c2t.Convert();
                tB_Width.Text = c2t.OutputSetting.outSize.Width.ToString();
                tB_Height.Text = c2t.OutputSetting.outSize.Height.ToString();
            } catch (ArgumentException) {
                MessageBox.Show("不支持的文件格式", "错误：", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void textBox_DragDrop(object sender, DragEventArgs e) {
            string[]? res = (string[])e!.Data!.GetData(DataFormats.FileDrop);
            OpenAndConvert(res[0]);
        }

        private void textBox_DragEnter(object sender, DragEventArgs e) {
            if (e.Data!.GetDataPresent(DataFormats.FileDrop)) {
                string[] tmp = (string[])e.Data.GetData(DataFormats.FileDrop)!;
                if (tmp.Length > 1) {
                    e.Effect = DragDropEffects.None;
                } else {
                    e.Effect = DragDropEffects.Copy;
                }
            } else {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e) {
            tB.Top = pnl_allOption.Top + pnl_allOption.Height + 4;

            pnl_allOption.Top = menu.Height;
            pnl_size.Left = 0;
            pnl_char.Left = pnl_size.Left + pnl_size.Width + 16;

            tB.Left = pnl_allOption.Left;
            tB.Size = new Size(ClientSize.Width - 2 * tB.Left, ClientSize.Height - tB.Top - tB.Left);

            btn_changeFont.Height = pnl_char.Height;
            btn_changeFont.Left = pnl_char.Left + pnl_char.Width + 16;

        }

        private void textBox_Width_KeyPress(object sender, KeyPressEventArgs e) {
            if (char.IsDigit(e.KeyChar) || e.KeyChar == '\b') {

            } else {
                e.Handled = true;
            }
        }

        private void chkBox_lock_CheckedChanged(object sender, EventArgs e) {
            tB_Height.Enabled = !chkB_isLock.Checked;

            if (chkB_isLock.Checked) {
                tB_Height.Text = c2t?.OutputSetting.outSize.Height.ToString();
                textBox_Width_TextChanged(tB_Width, new());
            }
        }

        private void textBox_Width_TextChanged(object sender, EventArgs e) {
            if (c2t is null) {
                return;
            }

            if (int.TryParse(tB_Height.Text, out int height)
                && int.TryParse(tB_Width.Text, out int width)) {
                Size cur = new();
                if (chkB_isLock.Checked) {
                    cur.Height = -1;
                } else {
                    cur.Height = int.Parse(tB_Height.Text);
                }
                if (height <= 0 || width <= 0) {
                    return;
                }

                cur.Width = width;
                c2t.ChangeSize(cur);
                tB.Text = c2t.Convert();
            }

            tB_Height.Text = c2t.OutputSetting.outSize.Height.ToString();
        }

        private void Form1_Load(object sender, EventArgs e) {
            chkB_isLock.Checked = true;
            //textBox_Width.Text = "200";
            Form1_SizeChanged(this, new());
        }

        private void textBox_Height_TextChanged(object sender, EventArgs e) {
            if (c2t is null) {
                return;
            }
            if (chkB_isLock.Checked) {
                // 说明是宽度改变导致的高度改变，不需要改变图片
                return;
            }
            Size size;
            if (int.TryParse(tB_Height.Text, out int height)
                && int.TryParse(tB_Width.Text, out int width)) {
                if (height > 0 && width > 0) {
                    size = new(width, height);
                    c2t.ChangeSize(size);
                }
            }
            tB.Text = c2t.Convert();
        }

        private void cbB_char_TextChanged(object sender, EventArgs e) {
            if (c2t is null || cbB_char.Text.Length < 2) {
                return;
            }
            if (cbB_char.Text.ToHashSet().Count < 2) {
                return;
            }

            c2t.ChangeCharacter(cbB_char.Text);
            tB.Text = c2t.Convert();
            if (chkB_isLock.Checked) {
                tB_Height.Text = c2t.OutputSetting.outSize.Height.ToString();
            }
        }

        private void btn_changeFont_Click(object sender, EventArgs e) {
            if (c2t is null) {
                return;
            }
            fontDialog.Font = this.tB.Font;
            if (fontDialog.ShowDialog() == DialogResult.OK) {
                c2t.ChangeFont(fontDialog.Font);
                tB.Text = c2t.Convert();
                tB.Font = fontDialog.Font;

                if (chkB_isLock.Checked) {
                    tB_Height.Text = c2t.OutputSetting.outSize.Height.ToString();
                }
            }
        }

        private void chB_isDither_CheckedChanged(object sender, EventArgs e) {
            if (c2t is null) {
                return;
            }

            c2t.ChangeIsDither(chB_isDither.Checked);
            tB.Text = c2t.Convert();
        }

        private void MenuItem_Save_Click(object sender, EventArgs e) {
            c2t!.SaveCurrentASCImage();
        }

        private void MenuItem_Close_Click(object sender, EventArgs e) {
            c2t?.ClosePic();
            c2t = null;
        }

        private void MenuItem_Open_Click(object sender, EventArgs e) {
            openFileDialog.Filter = "图片文件|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                OpenAndConvert(openFileDialog.FileName);
            }
        }

        private void MenuItem_Quit_Click(object sender, EventArgs e) {
            c2t?.ClosePic();
            this.Close();
        }

        private void MenuItem_Dither_CheckedChanged(object sender, EventArgs e) {
            chB_isDither.Checked = !chB_isDither.Checked;
        }

        private void MenuItem_Dither_Click(object sender, EventArgs e) {
            MenuItem_Dither.Checked = !MenuItem_Dither.Checked;
        }
    }
}
