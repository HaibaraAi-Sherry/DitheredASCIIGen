using System.Windows.Forms.VisualStyles;

namespace DitheringASCImage {
    public partial class Form1 : Form {
        Convert2Txt? c2t = null;

        public Form1() {
            InitializeComponent();
        }

        private void textBox_DragDrop(object sender, DragEventArgs e) {
            try {
                Bitmap pic = new(((string[])e.Data!.GetData(DataFormats.FileDrop)!)[0]);
                if (c2t != null) {
                    c2t.ChangePicture(pic);
                } else {
                    c2t = new(pic);
                }
                tB.Text = c2t.Convert();
                tB_Width.Text = c2t.OutputSetting.outSize.Width.ToString();
                tB_Height.Text = c2t.OutputSetting.outSize.Height.ToString();
            } catch (ArgumentException) {
                MessageBox.Show("不支持的文件格式", "错误：", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox_DragEnter(object sender, DragEventArgs e) {
            if (e.Data!.GetDataPresent(DataFormats.FileDrop)) {
                string[] tmp = (string[])e.Data.GetData(DataFormats.FileDrop)!;
                if (tmp.Length > 1) {
                    e.Effect = DragDropEffects.None;
                    return;
                }
                e.Effect = DragDropEffects.Copy;
            } else {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e) {
            tB.Size = new Size(ClientSize.Width - 20, ClientSize.Height - tB.Top);
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
        }

        private void textBox_Height_TextChanged(object sender, EventArgs e) {
            if (c2t is null) {
                return;
            }
            if (chkB_isLock.Checked) {
                // 说明是宽度改变导致的高度改变，不需要改变图片
                return;
            }
            Size size = new();
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
    }
}
