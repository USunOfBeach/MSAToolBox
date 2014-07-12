using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using LegacyUpdateServices;
using System.Threading;
using System.IO;
using MSAToolBoxClient;

namespace LegacyUpdateServer
{

    public partial class Form1 : Form
    {
        private string PatchCategory = "";
        private ServiceHost serviceHost = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void StartUpdateService()
        {
            serviceHost = new ServiceHost(typeof(UpdateService));
            serviceHost.Open();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartUpdateService();

            label1.Text = "服务已启动。";
            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                serviceHost.Abort();
                serviceHost.Close();
                label1.Text = "服务已停止。";
                button1.Enabled = true;
                button2.Enabled = false;
            }
            catch { }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog d = new FolderBrowserDialog())
            {
                d.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
                if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PatchCategory = d.SelectedPath;
                    label2.Text = PatchCategory;
                    UpdateService.PatchDirectory = PatchCategory;

                    updateFileList.Items.Clear();
                    UpdateService.patchList = new List<UpdatePatchFileInfo>();
                    UpdateService.patchList = GetAllPatchFiles(new DirectoryInfo(PatchCategory));

                    foreach (UpdatePatchFileInfo file in UpdateService.patchList)
                    {
                        string fileFullPath = PatchCategory + "\\" + (file.FilePath == "" ? file.FileName : file.FilePath + "\\" + file.FileName);
                        if (Path.GetExtension(fileFullPath) != ".M")
                            continue;
                        // version - build - dir - name - size - date
                        // version
                        ListViewItem viewItem = new ListViewItem("Legacy");
                        // build
                        using (FileStream f = File.OpenRead(PatchCategory + "\\" + (file.FilePath == "" ? file.FileName : file.FilePath + "\\" + file.FileName)))
                        {
                            int build = MFileManager.ReadIntFromStream(f, MFileManager.M_BUILD_OFFSET);
                            viewItem.SubItems.Add(build.ToString());
                            file.Build = build;
                        }
                        // dir
                        viewItem.SubItems.Add("ROOT" + file.FilePath.Replace(PatchCategory, ""));
                        // name
                        viewItem.SubItems.Add(file.FileName);
                        // size
                        using (FileStream f = File.OpenRead(PatchCategory + "\\" + (file.FilePath == "" ? file.FileName : file.FilePath + "\\" + file.FileName)))
                        {
                            viewItem.SubItems.Add(f.Length.ToString());
                            file.FileLength = f.Length;
                        }
                        // date
                        viewItem.SubItems.Add(File.GetCreationTime(PatchCategory + "\\" + (file.FilePath == "" ? file.FileName : file.FilePath + "\\" + file.FileName)).ToString());

                        updateFileList.Items.AddRange(new ListViewItem[] { viewItem });
                    }
                }
            }
        }

        private List<UpdatePatchFileInfo> GetAllPatchFiles(DirectoryInfo dir)
        {
            FileInfo[] allFiles = dir.GetFiles();
            foreach (FileInfo fi in allFiles)
            {
                UpdatePatchFileInfo file = new UpdatePatchFileInfo();
                file.FileName = fi.Name;
                file.FilePath = fi.DirectoryName.Replace(PatchCategory, "");
                UpdateService.patchList.Add(file);
            }

            DirectoryInfo[] allDir = dir.GetDirectories();

            foreach (DirectoryInfo d in allDir)
                GetAllPatchFiles(d);

            return UpdateService.patchList;
        }
    }
}
