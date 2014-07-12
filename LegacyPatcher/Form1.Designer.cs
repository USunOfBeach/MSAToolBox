namespace LegacyPatcher
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
            this.patchFileList = new System.Windows.Forms.ListView();
            this.selectFileDir = new System.Windows.Forms.Button();
            this.rootDir = new System.Windows.Forms.TextBox();
            this.labelFileDir = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.packPatch = new System.Windows.Forms.Button();
            this.patchProgress = new System.Windows.Forms.ProgressBar();
            this.tbGameBuild = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.patchName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // patchFileList
            // 
            this.patchFileList.AllowColumnReorder = true;
            this.patchFileList.Location = new System.Drawing.Point(12, 44);
            this.patchFileList.Name = "patchFileList";
            this.patchFileList.Size = new System.Drawing.Size(660, 266);
            this.patchFileList.TabIndex = 0;
            this.patchFileList.UseCompatibleStateImageBehavior = false;
            this.patchFileList.View = System.Windows.Forms.View.Details;
            // 
            // selectFileDir
            // 
            this.selectFileDir.Location = new System.Drawing.Point(13, 13);
            this.selectFileDir.Name = "selectFileDir";
            this.selectFileDir.Size = new System.Drawing.Size(97, 23);
            this.selectFileDir.TabIndex = 1;
            this.selectFileDir.Text = "选择文件目录";
            this.selectFileDir.UseVisualStyleBackColor = true;
            this.selectFileDir.Click += new System.EventHandler(this.selectFileDir_Click);
            // 
            // rootDir
            // 
            this.rootDir.Location = new System.Drawing.Point(350, 12);
            this.rootDir.Name = "rootDir";
            this.rootDir.Size = new System.Drawing.Size(109, 21);
            this.rootDir.TabIndex = 2;
            this.rootDir.Text = "%ClientDir%";
            // 
            // labelFileDir
            // 
            this.labelFileDir.AutoSize = true;
            this.labelFileDir.Location = new System.Drawing.Point(116, 18);
            this.labelFileDir.Name = "labelFileDir";
            this.labelFileDir.Size = new System.Drawing.Size(59, 12);
            this.labelFileDir.TabIndex = 3;
            this.labelFileDir.Text = "地址Label";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(303, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "根目录";
            // 
            // packPatch
            // 
            this.packPatch.Location = new System.Drawing.Point(582, 326);
            this.packPatch.Name = "packPatch";
            this.packPatch.Size = new System.Drawing.Size(90, 23);
            this.packPatch.TabIndex = 5;
            this.packPatch.Text = "生成更新文件";
            this.packPatch.UseVisualStyleBackColor = true;
            this.packPatch.Click += new System.EventHandler(this.packPatch_Click);
            // 
            // patchProgress
            // 
            this.patchProgress.Location = new System.Drawing.Point(13, 326);
            this.patchProgress.Name = "patchProgress";
            this.patchProgress.Size = new System.Drawing.Size(446, 23);
            this.patchProgress.TabIndex = 6;
            // 
            // tbGameBuild
            // 
            this.tbGameBuild.Location = new System.Drawing.Point(506, 328);
            this.tbGameBuild.Name = "tbGameBuild";
            this.tbGameBuild.Size = new System.Drawing.Size(70, 21);
            this.tbGameBuild.TabIndex = 7;
            this.tbGameBuild.Text = "10000";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(465, 331);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "Build";
            // 
            // patchName
            // 
            this.patchName.Location = new System.Drawing.Point(533, 12);
            this.patchName.Name = "patchName";
            this.patchName.Size = new System.Drawing.Size(139, 21);
            this.patchName.TabIndex = 9;
            this.patchName.Text = "LEGACYPATCH407091.M";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(468, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "PatchName";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(200, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "Unpack";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 361);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.patchName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbGameBuild);
            this.Controls.Add(this.patchProgress);
            this.Controls.Add(this.packPatch);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelFileDir);
            this.Controls.Add(this.rootDir);
            this.Controls.Add(this.selectFileDir);
            this.Controls.Add(this.patchFileList);
            this.Name = "Form1";
            this.Text = "LegacyPatcher";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView patchFileList;
        private System.Windows.Forms.Button selectFileDir;
        private System.Windows.Forms.TextBox rootDir;
        private System.Windows.Forms.Label labelFileDir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button packPatch;
        private System.Windows.Forms.ProgressBar patchProgress;
        private System.Windows.Forms.TextBox tbGameBuild;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox patchName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
    }
}

