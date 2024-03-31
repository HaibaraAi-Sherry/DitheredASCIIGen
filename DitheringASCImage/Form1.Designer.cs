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
            tB_Width = new TextBox();
            label1 = new Label();
            label2 = new Label();
            tB_Height = new TextBox();
            chkB_isLock = new CheckBox();
            SuspendLayout();
            // 
            // tB
            // 
            tB.AllowDrop = true;
            tB.BackColor = Color.White;
            tB.Font = new Font("Lucida Console", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tB.Location = new Point(12, 66);
            tB.Multiline = true;
            tB.Name = "tB";
            tB.ReadOnly = true;
            tB.ScrollBars = ScrollBars.Both;
            tB.Size = new Size(1159, 697);
            tB.TabIndex = 0;
            tB.DragDrop += textBox_DragDrop;
            tB.DragEnter += textBox_DragEnter;
            // 
            // tB_Width
            // 
            tB_Width.Location = new Point(122, 22);
            tB_Width.Name = "tB_Width";
            tB_Width.Size = new Size(65, 30);
            tB_Width.TabIndex = 1;
            tB_Width.TextChanged += textBox_Width_TextChanged;
            tB_Width.KeyPress += textBox_Width_KeyPress;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(16, 25);
            label1.Name = "label1";
            label1.Size = new Size(100, 24);
            label1.TabIndex = 2;
            label1.Text = "输出尺寸：";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(193, 25);
            label2.Name = "label2";
            label2.Size = new Size(19, 24);
            label2.TabIndex = 3;
            label2.Text = "x";
            // 
            // tB_Height
            // 
            tB_Height.Location = new Point(218, 22);
            tB_Height.Name = "tB_Height";
            tB_Height.Size = new Size(65, 30);
            tB_Height.TabIndex = 1;
            tB_Height.TextChanged += textBox_Height_TextChanged;
            tB_Height.KeyPress += textBox_Width_KeyPress;
            // 
            // chkB_isLock
            // 
            chkB_isLock.AutoSize = true;
            chkB_isLock.Location = new Point(289, 24);
            chkB_isLock.Name = "chkB_isLock";
            chkB_isLock.Size = new Size(84, 28);
            chkB_isLock.TabIndex = 5;
            chkB_isLock.Text = "Lock?";
            chkB_isLock.UseVisualStyleBackColor = true;
            chkB_isLock.CheckedChanged += chkBox_lock_CheckedChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1183, 775);
            Controls.Add(chkB_isLock);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(tB_Height);
            Controls.Add(tB_Width);
            Controls.Add(tB);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            SizeChanged += Form1_SizeChanged;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tB;
        private TextBox tB_Width;
        private Label label1;
        private Label label2;
        private TextBox tB_Height;
        private CheckBox chkB_isLock;
    }
}
