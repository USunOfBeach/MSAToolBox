namespace LegacyUpdateServer
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
            this.updateFileList = new System.Windows.Forms.ListView();
            this.version = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.build = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.category = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.filename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.filesize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.date = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // updateFileList
            // 
            this.updateFileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.version,
            this.build,
            this.category,
            this.filename,
            this.filesize,
            this.date});
            this.updateFileList.Location = new System.Drawing.Point(14, 41);
            this.updateFileList.Name = "updateFileList";
            this.updateFileList.Size = new System.Drawing.Size(760, 288);
            this.updateFileList.TabIndex = 0;
            this.updateFileList.UseCompatibleStateImageBehavior = false;
            this.updateFileList.View = System.Windows.Forms.View.Details;
            // 
            // version
            // 
            this.version.Text = "版本";
            this.version.Width = 121;
            // 
            // build
            // 
            this.build.Text = "Build";
            this.build.Width = 105;
            // 
            // category
            // 
            this.category.Text = "目录";
            this.category.Width = 103;
            // 
            // filename
            // 
            this.filename.Text = "文件名";
            this.filename.Width = 171;
            // 
            // filesize
            // 
            this.filesize.Text = "文件大小";
            this.filesize.Width = 115;
            // 
            // date
            // 
            this.date.Text = "日期";
            this.date.Width = 140;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 376);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "启动更新服务";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 332);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "服务状态信息";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 347);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(106, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "停止更新服务";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(697, 376);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "测试";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(124, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "更新目录路径";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(14, 12);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(104, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "指定更新目录";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(784, 411);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.updateFileList);
            this.Name = "Form1";
            this.Text = "LegacyUpdateServer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView updateFileList;
        private System.Windows.Forms.ColumnHeader version;
        private System.Windows.Forms.ColumnHeader build;
        private System.Windows.Forms.ColumnHeader category;
        private System.Windows.Forms.ColumnHeader filename;
        private System.Windows.Forms.ColumnHeader date;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ColumnHeader filesize;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button4;
    }
}

