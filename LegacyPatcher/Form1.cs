using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LegacyPatcher
{
    public partial class Form1 : Form
    {
        private string PatchCategory = "";
        private List<PatchFileInfo> patchFiles;

        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MFileManager.Test();
        }

        private void selectFileDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            if (d.ShowDialog() == DialogResult.OK || d.ShowDialog() == DialogResult.Yes)
            {
                PatchCategory = d.SelectedPath;
                labelFileDir.Text = PatchCategory;

                patchFileList.Clear();
                patchFileList.Columns.Add("FileName", 220);
                patchFileList.Columns.Add("FileDir", 220);
                patchFileList.Columns.Add("FileSize", 220);
                patchFiles = new List<PatchFileInfo>();
                patchFiles = GetAllPatchFiles(new DirectoryInfo(PatchCategory));
                foreach (PatchFileInfo file in patchFiles)
                {
                    ListViewItem viewItem = new ListViewItem(file.FileName);
                    //viewItem.SubItems.Add(file.FileName);
                    viewItem.SubItems.Add(file.FilePath);
                    Stream f = File.OpenRead(PatchCategory + "\\" + (file.FilePath == "" ? file.FileName : file.FilePath + "\\" + file.FileName));
                    viewItem.SubItems.Add(Convert.ToString(f.Length));
                    f.Close();
                    patchFileList.Items.AddRange(new ListViewItem[] { viewItem });
                }
            }
        }
        private List<PatchFileInfo> GetAllPatchFiles(DirectoryInfo dir)
        {
            FileInfo[] allFiles = dir.GetFiles();
            foreach (FileInfo fi in allFiles)
            {
                PatchFileInfo file = new PatchFileInfo();
                file.FileName = fi.Name;
                file.FilePath = fi.DirectoryName.Replace(PatchCategory, "");
                patchFiles.Add(file);
            }

            DirectoryInfo[] allDir = dir.GetDirectories();

            foreach (DirectoryInfo d in allDir)
                GetAllPatchFiles(d);

            return patchFiles; 
        }

        private void packPatch_Click(object sender, EventArgs e)
        {
            MFileManager.PackPatch(MFileType.FILETYPE_LEGACY_CLIENT_PATCH, patchFiles, Convert.ToUInt32(tbGameBuild.Text), PatchCategory, patchName.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }
    }
    class PatchFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
