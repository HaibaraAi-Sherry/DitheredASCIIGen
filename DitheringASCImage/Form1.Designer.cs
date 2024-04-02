namespace DitheringASCImage {
    partial class Form1 {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            tB = new TextBox();
            pnl_size = new Panel();
            chkB_isLock = new CheckBox();
            tB_Width = new TextBox();
            label2 = new Label();
            tB_Height = new TextBox();
            label1 = new Label();
            pnl_char = new Panel();
            cbB_char = new ComboBox();
            label3 = new Label();
            btn_changeFont = new Button();
            fontDialog = new FontDialog();
            chB_isDither = new CheckBox();
            menu = new MenuStrip();
            你好ToolStripMenuItem = new ToolStripMenuItem();
            MenuItem_Save = new ToolStripMenuItem();
            MenuItem_Open = new ToolStripMenuItem();
            MenuItem_Close = new ToolStripMenuItem();
            MenuItem_Quit = new ToolStripMenuItem();
            图像ToolStripMenuItem = new ToolStripMenuItem();
            MenuItem_Dither = new ToolStripMenuItem();
            pnl_allOption = new Panel();
            saveFileDialog = new SaveFileDialog();
            openFileDialog = new OpenFileDialog();
            锐化ToolStripMenuItem = new ToolStripMenuItem();
            pnl_size.SuspendLayout();
            pnl_char.SuspendLayout();
            menu.SuspendLayout();
            pnl_allOption.SuspendLayout();
            SuspendLayout();
            // 
            // tB
            // 
            tB.AllowDrop = true;
            tB.BackColor = Color.White;
            tB.Font = new Font("Lucida Console", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tB.Location = new Point(12, 123);
            tB.Multiline = true;
            tB.Name = "tB";
            tB.ReadOnly = true;
            tB.ScrollBars = ScrollBars.Both;
            tB.Size = new Size(1159, 697);
            tB.TabIndex = 0;
            tB.WordWrap = false;
            tB.DragDrop += textBox_DragDrop;
            tB.DragEnter += textBox_DragEnter;
            // 
            // pnl_size
            // 
            pnl_size.BorderStyle = BorderStyle.Fixed3D;
            pnl_size.Controls.Add(chkB_isLock);
            pnl_size.Controls.Add(tB_Width);
            pnl_size.Controls.Add(label2);
            pnl_size.Controls.Add(tB_Height);
            pnl_size.Controls.Add(label1);
            pnl_size.Location = new Point(16, 11);
            pnl_size.Name = "pnl_size";
            pnl_size.Size = new Size(391, 44);
            pnl_size.TabIndex = 6;
            // 
            // chkB_isLock
            // 
            chkB_isLock.AutoSize = true;
            chkB_isLock.Location = new Point(291, 9);
            chkB_isLock.Name = "chkB_isLock";
            chkB_isLock.Size = new Size(84, 28);
            chkB_isLock.TabIndex = 5;
            chkB_isLock.Text = "Lock?";
            chkB_isLock.UseVisualStyleBackColor = true;
            chkB_isLock.CheckedChanged += chkBox_lock_CheckedChanged;
            // 
            // tB_Width
            // 
            tB_Width.Location = new Point(124, 7);
            tB_Width.Name = "tB_Width";
            tB_Width.Size = new Size(65, 30);
            tB_Width.TabIndex = 1;
            tB_Width.TextChanged += textBox_Width_TextChanged;
            tB_Width.KeyPress += textBox_Width_KeyPress;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(195, 10);
            label2.Name = "label2";
            label2.Size = new Size(19, 24);
            label2.TabIndex = 3;
            label2.Text = "x";
            // 
            // tB_Height
            // 
            tB_Height.Location = new Point(220, 7);
            tB_Height.Name = "tB_Height";
            tB_Height.Size = new Size(65, 30);
            tB_Height.TabIndex = 1;
            tB_Height.TextChanged += textBox_Height_TextChanged;
            tB_Height.KeyPress += textBox_Width_KeyPress;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 10);
            label1.Name = "label1";
            label1.Size = new Size(100, 24);
            label1.TabIndex = 2;
            label1.Text = "输出尺寸：";
            // 
            // pnl_char
            // 
            pnl_char.BorderStyle = BorderStyle.Fixed3D;
            pnl_char.Controls.Add(cbB_char);
            pnl_char.Controls.Add(label3);
            pnl_char.Location = new Point(426, 11);
            pnl_char.Name = "pnl_char";
            pnl_char.Size = new Size(322, 44);
            pnl_char.TabIndex = 7;
            // 
            // cbB_char
            // 
            cbB_char.FormattingEnabled = true;
            cbB_char.Items.AddRange(new object[] { " #,.0123456789:;@ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz$", " ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", " 1234567890", "M@WB08Za2SX7r;i:;. ", "@#MBHAGh93X25Sisr;:, ", "█▓▒░ " });
            cbB_char.Location = new Point(108, 5);
            cbB_char.Name = "cbB_char";
            cbB_char.Size = new Size(205, 32);
            cbB_char.TabIndex = 1;
            cbB_char.Text = "M@WB08Za2SX7r;i:;. ";
            cbB_char.TextChanged += cbB_char_TextChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(20, 10);
            label3.Name = "label3";
            label3.Size = new Size(82, 24);
            label3.TabIndex = 0;
            label3.Text = "字符集：";
            // 
            // btn_changeFont
            // 
            btn_changeFont.Location = new Point(765, 11);
            btn_changeFont.Name = "btn_changeFont";
            btn_changeFont.Size = new Size(112, 44);
            btn_changeFont.TabIndex = 8;
            btn_changeFont.Text = "更换字体";
            btn_changeFont.UseVisualStyleBackColor = true;
            btn_changeFont.Click += btn_changeFont_Click;
            // 
            // chB_isDither
            // 
            chB_isDither.AutoSize = true;
            chB_isDither.Checked = true;
            chB_isDither.CheckState = CheckState.Checked;
            chB_isDither.Location = new Point(883, 20);
            chB_isDither.Name = "chB_isDither";
            chB_isDither.Size = new Size(126, 28);
            chB_isDither.TabIndex = 9;
            chB_isDither.Text = "是否Dither";
            chB_isDither.UseVisualStyleBackColor = true;
            chB_isDither.CheckedChanged += chB_isDither_CheckedChanged;
            // 
            // menu
            // 
            menu.ImageScalingSize = new Size(24, 24);
            menu.Items.AddRange(new ToolStripItem[] { 你好ToolStripMenuItem, 图像ToolStripMenuItem });
            menu.Location = new Point(0, 0);
            menu.Name = "menu";
            menu.Size = new Size(1183, 32);
            menu.TabIndex = 10;
            menu.Text = "menuStrip1";
            // 
            // 你好ToolStripMenuItem
            // 
            你好ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { MenuItem_Save, MenuItem_Open, MenuItem_Close, MenuItem_Quit });
            你好ToolStripMenuItem.Name = "你好ToolStripMenuItem";
            你好ToolStripMenuItem.Size = new Size(62, 28);
            你好ToolStripMenuItem.Text = "文件";
            // 
            // MenuItem_Save
            // 
            MenuItem_Save.Name = "MenuItem_Save";
            MenuItem_Save.Size = new Size(182, 34);
            MenuItem_Save.Text = "保存";
            MenuItem_Save.Click += MenuItem_Save_Click;
            // 
            // MenuItem_Open
            // 
            MenuItem_Open.Name = "MenuItem_Open";
            MenuItem_Open.Size = new Size(182, 34);
            MenuItem_Open.Text = "打开图片";
            MenuItem_Open.Click += MenuItem_Open_Click;
            // 
            // MenuItem_Close
            // 
            MenuItem_Close.Name = "MenuItem_Close";
            MenuItem_Close.Size = new Size(182, 34);
            MenuItem_Close.Text = "关闭图片";
            MenuItem_Close.Click += MenuItem_Close_Click;
            // 
            // MenuItem_Quit
            // 
            MenuItem_Quit.Name = "MenuItem_Quit";
            MenuItem_Quit.Size = new Size(182, 34);
            MenuItem_Quit.Text = "退出程序";
            MenuItem_Quit.Click += MenuItem_Quit_Click;
            // 
            // 图像ToolStripMenuItem
            // 
            图像ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { MenuItem_Dither, 锐化ToolStripMenuItem });
            图像ToolStripMenuItem.Name = "图像ToolStripMenuItem";
            图像ToolStripMenuItem.Size = new Size(62, 28);
            图像ToolStripMenuItem.Text = "图像";
            // 
            // MenuItem_Dither
            // 
            MenuItem_Dither.Checked = true;
            MenuItem_Dither.CheckState = CheckState.Checked;
            MenuItem_Dither.Name = "MenuItem_Dither";
            MenuItem_Dither.Size = new Size(270, 34);
            MenuItem_Dither.Text = "Dither";
            MenuItem_Dither.CheckedChanged += MenuItem_Dither_CheckedChanged;
            MenuItem_Dither.Click += MenuItem_Dither_Click;
            // 
            // pnl_allOption
            // 
            pnl_allOption.Controls.Add(pnl_size);
            pnl_allOption.Controls.Add(chB_isDither);
            pnl_allOption.Controls.Add(pnl_char);
            pnl_allOption.Controls.Add(btn_changeFont);
            pnl_allOption.Location = new Point(12, 50);
            pnl_allOption.Name = "pnl_allOption";
            pnl_allOption.Size = new Size(1159, 67);
            pnl_allOption.TabIndex = 11;
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "openFileDialog1";
            // 
            // 锐化ToolStripMenuItem
            // 
            锐化ToolStripMenuItem.Name = "锐化ToolStripMenuItem";
            锐化ToolStripMenuItem.Size = new Size(270, 34);
            锐化ToolStripMenuItem.Text = "锐化";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1183, 832);
            Controls.Add(pnl_allOption);
            Controls.Add(tB);
            Controls.Add(menu);
            MainMenuStrip = menu;
            Name = "Form1";
            RightToLeft = RightToLeft.No;
            Text = "不要用不等宽字体";
            Load += Form1_Load;
            SizeChanged += Form1_SizeChanged;
            pnl_size.ResumeLayout(false);
            pnl_size.PerformLayout();
            pnl_char.ResumeLayout(false);
            pnl_char.PerformLayout();
            menu.ResumeLayout(false);
            menu.PerformLayout();
            pnl_allOption.ResumeLayout(false);
            pnl_allOption.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tB;
        private Panel pnl_size;
        private CheckBox chkB_isLock;
        private TextBox tB_Width;
        private Label label2;
        private TextBox tB_Height;
        private Label label1;
        private Panel pnl_char;
        private Label label3;
        private ComboBox cbB_char;
        private Button btn_changeFont;
        private FontDialog fontDialog;
        private CheckBox chB_isDither;
        private MenuStrip menu;
        private ToolStripMenuItem 你好ToolStripMenuItem;
        private ToolStripMenuItem MenuItem_Save;
        private ToolStripMenuItem MenuItem_Open;
        private ToolStripMenuItem MenuItem_Close;
        private ToolStripMenuItem MenuItem_Quit;
        private Panel pnl_allOption;
        private SaveFileDialog saveFileDialog;
        private OpenFileDialog openFileDialog;
        private ToolStripMenuItem 图像ToolStripMenuItem;
        private ToolStripMenuItem MenuItem_Dither;
        private ToolStripMenuItem 锐化ToolStripMenuItem;
    }
}
