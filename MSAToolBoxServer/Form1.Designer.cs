namespace MSAToolBoxServer
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStartRegService = new System.Windows.Forms.Button();
            this.status = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStartRegService
            // 
            this.btnStartRegService.Location = new System.Drawing.Point(72, 164);
            this.btnStartRegService.Name = "btnStartRegService";
            this.btnStartRegService.Size = new System.Drawing.Size(143, 23);
            this.btnStartRegService.TabIndex = 0;
            this.btnStartRegService.Text = "Start Reg Service";
            this.btnStartRegService.UseVisualStyleBackColor = true;
            this.btnStartRegService.Click += new System.EventHandler(this.btnStartRegService_Click);
            // 
            // status
            // 
            this.status.AutoSize = true;
            this.status.Location = new System.Drawing.Point(12, 115);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(41, 12);
            this.status.TabIndex = 1;
            this.status.Text = "label1";
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(72, 193);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(143, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Stop Reg Service";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.status);
            this.Controls.Add(this.btnStartRegService);
            this.Name = "Form1";
            this.Text = "MSAToolBox Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStartRegService;
        private System.Windows.Forms.Label status;
        private System.Windows.Forms.Button button1;
    }
}

