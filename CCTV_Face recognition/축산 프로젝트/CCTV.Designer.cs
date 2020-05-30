namespace 축산_프로젝트
{
    partial class CCTV
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CCTV));
            this.picboxCow = new OpenCvSharp.UserInterface.PictureBoxIpl();
            this.btnstart = new System.Windows.Forms.Button();
            this.picplCSi = new OpenCvSharp.UserInterface.PictureBoxIpl();
            this.label15 = new System.Windows.Forms.Label();
            this.lbf = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picboxCow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picplCSi)).BeginInit();
            this.SuspendLayout();
            // 
            // picboxCow
            // 
            this.picboxCow.BackColor = System.Drawing.Color.Transparent;
            this.picboxCow.Location = new System.Drawing.Point(12, 83);
            this.picboxCow.Name = "picboxCow";
            this.picboxCow.Size = new System.Drawing.Size(817, 462);
            this.picboxCow.TabIndex = 4;
            this.picboxCow.TabStop = false;
            // 
            // btnstart
            // 
            this.btnstart.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.btnstart.Location = new System.Drawing.Point(500, 6);
            this.btnstart.Name = "btnstart";
            this.btnstart.Size = new System.Drawing.Size(76, 59);
            this.btnstart.TabIndex = 6;
            this.btnstart.Text = "시작";
            this.btnstart.UseVisualStyleBackColor = false;
            this.btnstart.Click += new System.EventHandler(this.button1_Click);
            // 
            // picplCSi
            // 
            this.picplCSi.BackColor = System.Drawing.Color.Transparent;
            this.picplCSi.Location = new System.Drawing.Point(909, 156);
            this.picplCSi.Name = "picplCSi";
            this.picplCSi.Size = new System.Drawing.Size(198, 283);
            this.picplCSi.TabIndex = 10;
            this.picplCSi.TabStop = false;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.Color.Transparent;
            this.label15.Font = new System.Drawing.Font("휴먼모음T", 30F, System.Drawing.FontStyle.Bold);
            this.label15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(219)))), ((int)(((byte)(228)))));
            this.label15.Location = new System.Drawing.Point(34, 9);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(460, 56);
            this.label15.TabIndex = 11;
            this.label15.Text = "녹화본 영상 분석하기";
            // 
            // lbf
            // 
            this.lbf.AutoSize = true;
            this.lbf.BackColor = System.Drawing.Color.Transparent;
            this.lbf.Font = new System.Drawing.Font("휴먼모음T", 30F, System.Drawing.FontStyle.Bold);
            this.lbf.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(219)))), ((int)(((byte)(228)))));
            this.lbf.Location = new System.Drawing.Point(863, 83);
            this.lbf.Name = "lbf";
            this.lbf.Size = new System.Drawing.Size(192, 56);
            this.lbf.TabIndex = 12;
            this.lbf.Text = "사람찾기";
            this.lbf.Visible = false;
            // 
            // CCTV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(30, 30);
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1182, 636);
            this.Controls.Add(this.lbf);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.picplCSi);
            this.Controls.Add(this.btnstart);
            this.Controls.Add(this.picboxCow);
            this.Name = "CCTV";
            this.Text = "CCTV";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CCTV_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.picboxCow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picplCSi)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private OpenCvSharp.UserInterface.PictureBoxIpl picboxCow;
        private System.Windows.Forms.Button btnstart;
        private OpenCvSharp.UserInterface.PictureBoxIpl picplCSi;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label lbf;
    }
}