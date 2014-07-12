using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using MSAToolBoxClient.RegServiceReference;
using System.Threading;
using System.IO;
using LegacyUpdateServices;
using System.Collections.Generic;
using System.Windows.Forms;
using MSAToolBoxClient.UpdateServiceReference;
using LegacyPatcher;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;

namespace MSAToolBoxClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Regex UserNameRegEx = new Regex("^\\w+$");
        Regex EmailRegEx = new Regex("^(\\w)+(\\.\\w+)*@(\\w)+((\\.\\w+)+)$");

        UpdateInfo[] updateInfoList;
        UpdatePatchFileInfo[] patchFileList;

        private string ClientPath = "";
        private string LegacyUpdateDirectory = "LegacyUpdates";
        private string confFile = "MSAToolBox.xml";
        private string ServerHost = "127.0.0.1";
        private bool IsClientPathValid = false;
        private ushort ClientBuild = 0;
        private bool IsClientValid = false;
        private bool IsUpdateNeeded = true;
        private bool ConfSaveUpdateFile = false;
        private bool ClearCache = true;
        private bool StartGameAfterUpdate = false;
        private bool SkipClientCheck = false;

        public class LegacyFilesData
        {
            public string File { get; set; }
            public long Size { get; set; }
            public string Description { get; set; }
            public bool Critical { get; set; }
            public long SizeV2 { get; set; }
            public string MismatchReason { get; set; }
            public LegacyFilesData(string file, long size, string description, long sizeV2 = 0, bool critical = true, string mismatchReason = "")
            {
                File = file;
                Size = size;
                Description = description;
                SizeV2 = sizeV2;
                Critical = critical;
                MismatchReason = mismatchReason;
            }
        }

        private List<LegacyFilesData> ClientFilesData = new List<LegacyFilesData>()
        {
            new LegacyFilesData("Data\\common.MPQ", 3031465069, "核心游戏数据"),
            new LegacyFilesData("Data\\common-2.MPQ", 1835535666, "核心游戏数据"),
            new LegacyFilesData("Data\\expansion.MPQ", 1908643839, "核心游戏数据"),
            new LegacyFilesData("Data\\lichking.MPQ", 2551828006, "核心游戏数据"),
            new LegacyFilesData("Data\\patch.MPQ", 3550348556, "核心游戏数据"),
            new LegacyFilesData("Data\\patch-2.MPQ", 1536602393, "核心游戏数据"),
            new LegacyFilesData("Data\\patch-3.MPQ", 378115554, "核心游戏数据"),
            new LegacyFilesData("Data\\patch-4.MPQ", 895334673, "世界场景增量更新数据"),
            new LegacyFilesData("Data\\patch-5.MPQ", 851390454, "生物增量更新数据"),
            new LegacyFilesData("Data\\patch-6.MPQ", 691942923, "法术增量更新数据"),
            new LegacyFilesData("Data\\patch-7.MPQ", 710828623, "物品增量更新数据"),
            new LegacyFilesData("Data\\patch-8.MPQ", 136505434, "V3角色模型微调数据", 0, false),
            new LegacyFilesData("Data\\zhCN\\speech-zhCN.MPQ", 494915603, "核心本地化语音数据"),
            new LegacyFilesData("Data\\zhCN\\expansion-locale-zhCN.MPQ", 3073765, "核心本地化游戏数据"),
            new LegacyFilesData("Data\\zhCN\\expansion-speech-zhCN.MPQ", 252982372, "核心本地化游戏数据"),
            new LegacyFilesData("Data\\zhCN\\lichking-locale-zhCN.MPQ", 11993723, "核心本地化游戏数据"),
            new LegacyFilesData("Data\\zhCN\\lichking-speech-zhCN.MPQ", 274739660, "核心本地化游戏数据"),
            new LegacyFilesData("Data\\zhCN\\patch-zhCN.MPQ", 215949912, "核心本地化更新数据", 619109376),
            new LegacyFilesData("Data\\zhCN\\patch-zhCN-2.MPQ", 23029, "核心本地化更新数据", 91350016),
        };

        private void ValidateRegInfo()
        {
            bool isValid = true;
            if (regUsername.Text.Length == 0 || regPassword.Password.Length == 0
                || regConfirmPassword.Password.Length == 0 || regEmail.Text.Length == 0)
                isValid = false;

            if (regUsername.Text.Length < 6 || regUsername.Text.Length > 32)
            {
                regUsername.Foreground = new SolidColorBrush(Colors.Red);
                isValid = false;
            }
            else
            {
                if (!UserNameRegEx.IsMatch(regUsername.Text))
                {
                    regUsername.Foreground = new SolidColorBrush(Colors.Red);
                    isValid = false;
                }
                else
                    regUsername.Foreground = new SolidColorBrush(Colors.Green);
            }

            if (regPassword.Password.Length < 6 || regPassword.Password.Length > 32)
            {
                regPassword.Foreground = new SolidColorBrush(Colors.Red);
                isValid = false;
            }
            else
            {
                if (!UserNameRegEx.IsMatch(regPassword.Password) || regUsername.Text == regPassword.Password)
                {
                    regPassword.Foreground = new SolidColorBrush(Colors.Red);
                    isValid = false;
                }
                else
                    regPassword.Foreground = new SolidColorBrush(Colors.Green);
            }

            if (regPassword.Password != regConfirmPassword.Password)
            {
                regConfirmPassword.Foreground = new SolidColorBrush(Colors.Red);
                isValid = false;
            }
            else
                regConfirmPassword.Foreground = new SolidColorBrush(Colors.Green);

            if (!EmailRegEx.IsMatch(regEmail.Text))
            {
                regEmail.Foreground = new SolidColorBrush(Colors.Red);
                isValid = false;
            }
            else
                regEmail.Foreground = new SolidColorBrush(Colors.Green);

            regButton.IsEnabled = isValid;
        }
        private void ValidateChangePwInfo()
        {
            bool isValid = true;
            if (changePwUsername.Text.Length == 0 || changePwOriginalPw.Password.Length == 0
                || changePwEmail.Text.Length == 0 || changePwNewPw.Password.Length == 0 || changePwConfirmPw.Password.Length == 0)
                isValid = false;

            if (changePwUsername.Text.Length < 6 || changePwUsername.Text.Length > 32)
            {
                changePwUsername.Foreground = new SolidColorBrush(Colors.Red);
                isValid = false;
            }
            else
            {
                if (!UserNameRegEx.IsMatch(changePwUsername.Text))
                {
                    changePwUsername.Foreground = new SolidColorBrush(Colors.Red);
                    isValid = false;
                }
                else
                    changePwUsername.Foreground = new SolidColorBrush(Colors.Green);
            }

            if (changePwOriginalPw.Password == changePwUsername.Text) // no regex check.
            {
                changePwOriginalPw.Foreground = new SolidColorBrush(Colors.Red);
                isValid = false;
            }
            else
                changePwOriginalPw.Foreground = new SolidColorBrush(Colors.Green);

            if (!UserNameRegEx.IsMatch(changePwNewPw.Password) || changePwNewPw.Password == changePwUsername.Text
                || changePwNewPw.Password.Length < 6 || changePwNewPw.Password.Length > 32)
            {
                changePwNewPw.Foreground = new SolidColorBrush(Colors.Red);
                isValid = false;
            }
            else
                changePwNewPw.Foreground = new SolidColorBrush(Colors.Green);

            if (changePwNewPw.Password != changePwConfirmPw.Password)
            {
                changePwConfirmPw.Foreground = new SolidColorBrush(Colors.Red);
                isValid = false;
            }
            else
                changePwConfirmPw.Foreground = new SolidColorBrush(Colors.Green);

            if (!EmailRegEx.IsMatch(changePwEmail.Text))
            {
                changePwEmail.Foreground = new SolidColorBrush(Colors.Red);
                isValid = false;
            }
            else
                changePwEmail.Foreground = new SolidColorBrush(Colors.Green);

            changePwBtn.IsEnabled = isValid;
        }
        private void regPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidateRegInfo();
        }
        private void regEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateRegInfo();
        }
        private void regUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateRegInfo();
        }
        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            this.regButton.Content = "正在注册...";
            this.regButton.IsEnabled = false;
            this.regUsername.IsEnabled = false;
            this.regPassword.IsEnabled = false;
            this.regConfirmPassword.IsEnabled = false;
            Thread regThread = new Thread(TryRegister);
            regThread.Start();
        }
        private void changePwBtn_Click(object sender, RoutedEventArgs e)
        {
            changePwUsername.IsEnabled = false;
            changePwOriginalPw.IsEnabled = false;
            changePwNewPw.IsEnabled = false;
            changePwEmail.IsEnabled = false;
            changePwConfirmPw.IsEnabled = false;
            changePwBtn.IsEnabled = false;
            changePwBtn.Content = "正在处理...";
            Thread changePwThread = new Thread(TryChangePw);
            changePwThread.Start();
        }
        private void TryRegister()
        {
            RegResult result = RegResult.RESULT_FAILED_WITHOUT_REASON;
            try
            {
                RegServiceClient client = new RegServiceClient();
                string userString = this.Dispatcher.Invoke(new GetRegUserData(GetRegData)).ToString();
                result = client.TryRegister(EncryptString(userString, "MSAToolBoxRegistration"));
                client.Close();
            }
            catch (System.Exception ex)
            {
            	this.Dispatcher.Invoke((Action)(() => { 
                        tbRegResult.Text = "注册失败啦，真是杯具呀。";
                        regUsername.Text = "";
                        regPassword.Password = "";
                        regConfirmPassword.Password = "";
                        regEmail.Text = "";
                        regUsername.IsEnabled = true;
                        regPassword.IsEnabled = true;
                        regConfirmPassword.IsEnabled = true;
                        regEmail.IsEnabled = true;
                        regButton.IsEnabled = true;
                        DialogManager.ShowMessageAsync(this, "发生了一个神秘的致命错误！", "把这些信息发送给MSA：\n" + ex.ToString());
                }));
                return;
            }

            switch (result)
            {
                case RegResult.RESULT_FAILED_CANT_CONNECT_DB:
                    this.Dispatcher.Invoke((Action)(() => { 
                        tbRegResult.Text = "虽然可以连接注册服务器，但是数据库连接好像挂了。通知MSA来处理一下！"; 
                    }));
                    break;
                case RegResult.RESULT_FAILED_EMAIL_TAKEN:
                    this.Dispatcher.Invoke((Action)(() => { 
                        tbRegResult.Text = "邮箱地址已经被别人用掉了。请换一个把。";
                        regEmail.Foreground = new SolidColorBrush(Colors.Red);
                    }));
                    break;
                case RegResult.RESULT_FAILED_INVALID_REGINFO:
                    this.Dispatcher.Invoke((Action)(() => { 
                        tbRegResult.Text = "注册信息无法被服务器所识别。一般来说这是因为你没有正确使用注册模组，嗯，但如果你不是闲得蛋疼来RE这个小东西，那就不知道为什么会出问题了。再试一遍吧。";
                    }));
                    break;
                case RegResult.RESULT_FAILED_USERNAME_TAKEN:
                    this.Dispatcher.Invoke((Action)(() => { 
                        tbRegResult.Text = "这个用户名已经被别人用掉了。请换一个吧。";
                        regUsername.Foreground = new SolidColorBrush(Colors.Red);
                    }));
                    break;
                case RegResult.RESULT_SUCCESS:
                    this.Dispatcher.Invoke((Action)(() => { 
                        tbRegResult.Text = "你成功注册了一个账号！这简直是个奇迹！祝你在3秒内忘掉它！";
                        regUsername.Text = "";
                        regPassword.Password = "";
                        regConfirmPassword.Password = "";
                        regEmail.Text = "";
                    }));
                    break;
                default:
                    this.Dispatcher.Invoke((Action)(() => { tbRegResult.Text = "好像出现了一个莫名其妙的错误，就连MSA都不知道这个错误是怎么出现的。"; 
                    }));
                    break;
            }
            this.Dispatcher.Invoke((Action)(() =>
            {
                regButton.Content = "注册！";
                regUsername.IsEnabled = true;
                regPassword.IsEnabled = true;
                regConfirmPassword.IsEnabled = true;
                regEmail.IsEnabled = true;
                ValidateRegInfo();
            }));
        }
        private void TryChangePw()
        {
            ChangePwResult result = ChangePwResult.RESULT_FAILED_WITHOUT_REASON;
            try
            {
                RegServiceClient client = new RegServiceClient();
                string userString = this.Dispatcher.Invoke(new GetChangePwUserData(GetChangePwData)).ToString();
                result = client.TryChangePw(EncryptString(userString, "MSAToolBoxChangePassword"));
                client.Close();
            }
            catch (System.Exception ex)
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    tbChangePwResult.Text = "好像出了一点问题……好像没有连上服务器，总之好像是这样吧。";
                    changePwUsername.Text = "";
                    changePwOriginalPw.Password = "";
                    changePwNewPw.Password = "";
                    changePwConfirmPw.Password = "";
                    changePwEmail.Text = "";
                    changePwBtn.Content = "改！";
                    changePwUsername.IsEnabled = true;
                    changePwOriginalPw.IsEnabled = true;
                    changePwNewPw.IsEnabled = true;
                    changePwConfirmPw.IsEnabled = true;
                    changePwEmail.IsEnabled = true;
                    changePwBtn.IsEnabled = true;
                    DialogManager.ShowMessageAsync(this, "又发生了一个神秘的致命错误！", "把这些信息发送给MSA：\n" + ex.ToString());
                }));
            }
            
            switch (result)
            {
                case ChangePwResult.RESULT_FAILED_CANT_CONNECT_DB:
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        tbChangePwResult.Text = "虽然可以连接到服务器，但是数据库连接好像挂了。通知MSA来处理一下！";
                    }));
                    break;
                case ChangePwResult.RESULT_FAILED_INFO_NOT_MATCH:
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        tbChangePwResult.Text = "你输入的信息和数据库记载的信息不符耶。这样你是无法改掉密码的啦！";
                    }));
                    break;
                case ChangePwResult.RESULT_SUCCESS:
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        tbChangePwResult.Text = "祝贺你！你成功把你至今都没有忘掉的密码改成了你在接下去2.5秒内一定会忘掉的新密码！";
                    }));
                    break;
            }
            this.Dispatcher.Invoke((Action)(() =>
            {
                changePwUsername.Text = "";
                changePwOriginalPw.Password = "";
                changePwNewPw.Password = "";
                changePwEmail.Text = "";
                changePwConfirmPw.Password = "";
                changePwBtn.Content = "改！";

                changePwUsername.IsEnabled = true;
                changePwOriginalPw.IsEnabled = true;
                changePwNewPw.IsEnabled = true;
                changePwEmail.IsEnabled = true;
                changePwConfirmPw.IsEnabled = true;
                changePwBtn.IsEnabled = false;
            }));
        }
        private delegate string GetRegUserData();
        private string GetRegData()
        {
            // USERNAME:PASSHASH:EMAIL:REGCODE
            return regUsername.Text.ToUpper() + ":" +
                ComputePasswordHash(regUsername.Text.ToUpper() + ":" + regPassword.Password.ToUpper()) + ":" + regEmail.Text + ":MSAToolBoxRegistration";
        }
        private delegate string GetChangePwUserData();
        private string GetChangePwData()
        {
            // USERNAME:OLDPASSHASH:EMAIL:NEWPASSHASH:CHANGEPWCODE
            return changePwUsername.Text.ToUpper() + ":" + ComputePasswordHash(changePwUsername.Text.ToUpper() + ":" + changePwOriginalPw.Password.ToUpper()) + ":" + changePwEmail.Text + ":" + ComputePasswordHash(changePwUsername.Text.ToUpper() + ":" + changePwNewPw.Password.ToUpper()) + ":" + "MSAToolBoxChangePassword";
        }
        private string ComputePasswordHash(string password)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] passByteIn = Encoding.ASCII.GetBytes(password);
            byte[] passByteHash = sha1.ComputeHash(passByteIn);
            string s = BitConverter.ToString(passByteHash).Replace("-", "");
            return s;
        }
        private byte[] EncryptString(string s, string k)
        {
            byte[] b = Encoding.ASCII.GetBytes(s);
            byte[] key = Encoding.ASCII.GetBytes(k);
            for (int i = 0; i < b.Length; i += 2)
            {
                for (int j = 0; j < key.Length; j += 2)
                {
                    b[i] = Convert.ToByte(b[i] ^ key[j]);
                }
            }
            return b;
        }
        private void changePwUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateChangePwInfo();
        }
        private void changePwOriginalPw_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidateChangePwInfo();
        }
        private void changePwEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateChangePwInfo();
        }
        private void changePwNewPw_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidateChangePwInfo();
        }
        private void changePwConfirmPw_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidateChangePwInfo();
        }
        private async void GetUpdateInfo()
        {
            var c = await this.ShowProgressAsync("获取更新信息", "正在获取最近的更新说明文档。");
            Exception e = null;
            try
            {
                UpdateServiceClient client = new UpdateServiceClient();
                updateInfoList = client.GetUpdateInfo();
                updateDataGrid.ItemsSource = updateInfoList;
                client.Close();
            }
            catch (System.Exception ex)
            {
                e = ex;
            }
            await Task.Delay(1000);
            await c.CloseAsync();
            if (e != null)
                await this.ShowMessageAsync("发生了一个错误", e.ToString());
        }
        private void GetLatestPatches()
        {
            SetText(updateStatusLabelA, "准备建立远程连接...");
            UpdateServiceClient client = new UpdateServiceClient();
            Thread.Sleep(500);
            SetText(updateStatusLabelA, "连接成功。");
            SetText(updateStatusLabelB, "正在查询最新更新资源...");
            patchFileList = client.GetLatestPatchFiles();
            if (patchFileList.Length == 0)
            {
                SetText(updateStatusLabelB, "未检测到可用更新。即将跳转至启动程序。");
                IsUpdateNeeded = false;
                Thread.Sleep(1000);
            }
            else if (patchFileList[0].Build == ClientBuild)
            {
                SetText(updateStatusLabelB, "客户端已是最新。即将跳转至启动程序。");
                IsUpdateNeeded = false;
                Thread.Sleep(1000);
            }
            else
            {
                SetText(updateStatusLabelB, "检测到可用更新。即将跳转至更新程序。");
                IsUpdateNeeded = true;
                Thread.Sleep(1000);
            }
            ClientInfoUpdateSequence();
            Thread.Sleep(500);
        }
        private void GetClientBuild()
        {
            if (!File.Exists(ClientPath + "\\Wow.exe"))
            {
                ClientBuild = 0;
                return;
            }

            using (FileStream fs = File.OpenRead(ClientPath + "\\Wow.exe"))
            {
                byte[] reversedBuildBytes = new byte[2];
                fs.Seek(5020144, SeekOrigin.Begin);
                fs.Read(reversedBuildBytes, 0, 2);
                ushort a = (ushort)reversedBuildBytes[1];
                ushort b = (ushort)reversedBuildBytes[0];
                string buildString = String.Format("{0:X2}{1:X2}", a, b);
                ClientBuild = Convert.ToUInt16(buildString, 16);
                this.Dispatcher.Invoke(new Action(() =>
                    { buildLabelB.Content = String.Format("Build {0}", ClientBuild); }));
            }
        }
        private void DownloadPatch()
        {
            string fullPath = ClientPath + "\\" + LegacyUpdateDirectory + "\\";
            int build = 0;
            foreach (UpdatePatchFileInfo info in patchFileList)
            {
                build = info.Build > build ? info.Build : build;
                UpdateServiceClient client = new UpdateServiceClient();
                Stream patchStream = client.DownloadFile(info);
                if (patchStream != null)
                {
                    if (patchStream.CanRead)
                    {
                        if (!Directory.Exists(fullPath))
                            Directory.CreateDirectory(fullPath);

                        using (FileStream fs = new FileStream(fullPath + info.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            const int bufferLength = 4096;
                            byte[] buffer = new byte[bufferLength];
                            int count;
                            long downloaded = 0;
                            while ((count = patchStream.Read(buffer, 0, bufferLength)) > 0)
                            {
                                downloaded += count;
                                double pct = (double)downloaded / (double)info.FileLength * 100;
                                fs.Write(buffer, 0, count);
                                SetText(updateStatusLabelA, String.Format("{0} {1:F1}%", info.FileName, pct));
                                SetText(updateStatusLabelB, String.Format("{0:N0}B/{1:N0}B", downloaded, info.FileLength));
                                GivePercentage(updateProgressbar, (long)pct);
                            }
                            fs.Close();
                            // set left text
                            patchStream.Close();
                        }
                    }
                }
                client.Close();
                SetText(updateStatusLabelA, "成功获取更新文档。");
                Thread.Sleep(1000);
                SetText(updateStatusLabelB, "等待处理...");
                Thread.Sleep(1000);
                SetText(updateStatusLabelB, "正在解析文件，可能需要一点时间...");
                MFileManager.UnpackPatch(ClientPath, fullPath + "\\" + info.FileName);
                SetText(updateStatusLabelB, info.FileName + "已成功安装。");
                Thread.Sleep(1000);
                if (!ConfSaveUpdateFile)
                {
                    if (File.Exists(fullPath + info.FileName))
                        File.Delete(fullPath + info.FileName);
                }
            }
            ModifyGameVersion(build.ToString(), 1, 0, 1);
            SetText(updateStatusLabelA, "");
            SetText(updateStatusLabelB, "更新完成。");
            IsUpdateNeeded = false;
            ClientInfoUpdateSequence();
        }
        private void ValidateStartButtonUsability()
        {
            this.Dispatcher.Invoke(new Action(() =>
                {
                    if (!IsClientPathValid)
                    {
                        updateButton.IsEnabled = false;
                        updateStatusLabelB.Content = "请在设置栏页指定游戏客户端位置。";
                    }
                    else if (!IsClientValid)
                    {
                        updateButton.IsEnabled = false;
                        updateStatusLabelB.Content = "未识别客户端基础版本，无法更新。";
                    }
                    else if (IsUpdateNeeded)
                    {
                        updateButton.Content = "检测更新";
                        updateButton.IsEnabled = true;
                        updateStatusLabelB.Content = "数据就绪，准备检测更新。";
                    }
                    else if (!IsUpdateNeeded)
                    {
                        updateButton.Content = "启动游戏";
                        updateButton.IsEnabled = true;
                        updateStatusLabelB.Content = "一切就绪。";
                    }
                }));
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // always disable this button after click
            updateButton.IsEnabled = false;
            // update case
            if (IsUpdateNeeded)
            {
                Thread downloadThread = new Thread(DownloadSequence);
                downloadThread.Start();
            }
            else
                // start game case
                StartGame();
        }
        private void StartGame()
        {
            CloseGame();

            if (ClearCache)
            {
                if (Directory.Exists(ClientPath + "\\Cache"))
                    Directory.Delete(ClientPath + "\\Cache", true);
            }
            if (File.Exists(ClientPath + "realmlist.wtf"))
                File.Delete(ClientPath + "\\realmlist.wtf");

            using (FileStream fs = File.Create(ClientPath + "\\realmlist.wtf"))
            {
                byte[] ipBytes = Encoding.ASCII.GetBytes("set realmlist " + ServerHost);
                fs.Write(ipBytes, 0, ipBytes.Length);
            }

            Process.Start(ClientPath + "\\Wow.exe");
            Environment.Exit(0);
        }
        private void DownloadSequence()
        {
            GetLatestPatches();
            ClientInfoUpdateSequence();
            if (IsUpdateNeeded)
                DownloadPatch();
            if (StartGameAfterUpdate)
                StartGame();
        }
        private void ClientInfoUpdateSequence()
        {
            GetClientBuild();
            ValidateStartButtonUsability();
        }
        private void updateDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = updateDataGrid.SelectedIndex;
            if (index >= 0 && index < updateInfoList.Length)
            {
                updateInfoTitle.Content = updateInfoList[index].Title;
                updateInfoContent.Text = updateInfoList[index].Content;
            }
        }
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ClientPath = d.SelectedPath;
                clientDirLabel.Content = ClientPath;

                // need full check here
                if (File.Exists(ClientPath + "\\" + "Wow.exe"))
                {
                    clientDirLabel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB3FFA7"));
                    SaveConfig("msatoolbox/legacy/update/conf", "clientPath", ClientPath);
                    IsClientPathValid = true;
                    if (!SkipClientCheck)
                    {
                        var c = await this.ShowProgressAsync("客户端完整性检查", "正在进行客户端完整性检测准备工作。如果你不希望工具箱检测你的客户端，请在设置里更改相应选项。");
                        await Task.Delay(3000);
                        await c.CloseAsync();
                        LaunchLegacyClientBasicValidation();
                    }
                }
                else
                {
                    clientDirLabel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFA7A7"));
                    IsClientPathValid = false;
                }

                ClientInfoUpdateSequence();
            }
        }
        private void SetText(System.Windows.Controls.Label label, string text = null)
        {
            if (text == null)
                label.Dispatcher.Invoke(new Action(() => { label.Content = ""; }));
            else
                label.Dispatcher.Invoke(new Action(() => { label.Content = text; }));
        }
        private void GivePercentage(System.Windows.Controls.ProgressBar bar, long value, long max = 100)
        {
            bar.Dispatcher.Invoke(new Action(() => { bar.Value = value; bar.Maximum = max; }));
        }
        private async void LaunchLegacyClientBasicValidation()
        {
            var c = await this.ShowProgressAsync("游戏数据检测", "正在准备游戏数据完整性，请稍候...");

            await Task.Delay(3000);

            CloseGame();

            IsClientValid = true;
            
            string checkResult = "";
            int fileIndex = 0;
            foreach (LegacyFilesData data in ClientFilesData)
            {
                fileIndex++;
                // open & check it.
                if (!File.Exists(ClientPath + "\\" + data.File))
                {
                    data.MismatchReason = "文件不存在";
                    if (data.Critical)
                        IsClientValid = false;
                }
                else
                {
                    using (FileStream fs = File.OpenRead(ClientPath + "\\" + data.File))
                    {
                        if (data.SizeV2 != 0)
                        {
                            if (IsSizeCloseEnough(fs.Length, data.Size, 0.05d)
                                || IsSizeCloseEnough(fs.Length, data.SizeV2, 0.05d))
                                data.MismatchReason = "检测通过";
                            else
                            {
                                data.MismatchReason = "数据不符";
                                if (data.Critical)
                                    IsClientValid = false;
                            }
                        }
                        else
                        {
                            if (IsSizeCloseEnough(fs.Length, data.Size, 0.05d))
                            {
                                data.MismatchReason = "检测通过";
                            }
                            else
                            {
                                data.MismatchReason = "数据不符";
                                if (data.Critical)
                                    IsClientValid = false;
                            }
                        }
                    }
                }

                checkResult = String.Format("{0} [{1}] - {2}", data.File, data.Description, data.MismatchReason);
                
                c.SetProgress((double)(fileIndex / ClientFilesData.Count));
                c.SetMessage(checkResult);
                await Task.Delay(500);
            }

            checkResult = "检测完成。";

            await Task.Delay(1000);

            if (!IsClientValid)
                checkResult = "客户端数据存在问题，更新服务无法正常运作。请查阅稍候列出的检测信息以修复它们。即将跳转。";
            else
                checkResult = "检测通过。你可以使用更新服务来更新你的客户端。即将跳转。";

            c.SetMessage(checkResult);

            await Task.Delay(3000);

            await c.CloseAsync();

            if (!IsClientValid)
            {
                string fullResult = "缺失的文件：" + Environment.NewLine;
                bool found = false;
                foreach (LegacyFilesData info in ClientFilesData)
                {
                    if (info.MismatchReason == "文件不存在")
                    {
                        found = true;
                        fullResult += info.File + " " + info.Description + Environment.NewLine;
                    }
                }
                if (!found)
                    fullResult += "无" + Environment.NewLine;

                fullResult += Environment.NewLine + "未通过验证的文件：" + Environment.NewLine;

                found = false;
                foreach (LegacyFilesData info in ClientFilesData)
                {
                    if (info.MismatchReason == "数据不符")
                    {
                        found = true;
                        fullResult += info.File + " " + info.Description + Environment.NewLine;
                    }
                }
                if (!found)
                    fullResult += "无" + Environment.NewLine;

                await this.ShowMessageAsync("检测结果(未通过)", fullResult, MessageDialogStyle.Affirmative);
            }
            else
            {
                IsClientValid = true;
                confSkipClientCheck.IsChecked = true;
                SaveConfig("msatoolbox/legacy/update/conf", "skipClientCheck", "true");
                await this.ShowMessageAsync("检测结果(通过)", "此客户端可以正常更新。检测结果已记录在配置文件当中，未来更新时将不再检测客户端。如果你希望再次检测，请在设置栏位更改选项。", MessageDialogStyle.Affirmative);
            }
            ClientInfoUpdateSequence();
        }
        private bool IsSizeCloseEnough(long x, long y, double factor)
        {
            if ((double)(x / y) > (1d - factor) || (double)(x / y) < (1d + factor))
                return true;
            else
                return false;
        }
        private void CloseGame()
        {
            Process[] ps = Process.GetProcesses();
            foreach (Process p in ps)
            {
                if (p.ProcessName == "Wow.exe")
                    p.Kill();
            }
        }
        private void ModifyGameVersion(string build, byte highV, byte middleV, byte lowV)
        {
            CloseGame();

            if (!File.Exists(ClientPath + "\\Wow.exe"))
                return;

            using (FileStream fs = File.Open(ClientPath + "\\Wow.exe", FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                // plain version text 5 chars - x.x.x
                fs.Seek(6240776, SeekOrigin.Begin);
                fs.Write(Encoding.ASCII.GetBytes(String.Format("{0}.{1}.{2}", highV, middleV, lowV)), 0, 5);

                // build - 5 chars - xxxxx
                fs.Seek(6165041, SeekOrigin.Begin);
                fs.Write(Encoding.ASCII.GetBytes(build), 0, 5);
                fs.Seek(6240768, SeekOrigin.Begin);
                fs.Write(Encoding.ASCII.GetBytes(build), 0, 5);
                fs.Seek(6484972, SeekOrigin.Begin);
                fs.Write(Encoding.ASCII.GetBytes(build), 0, 5);
                fs.Seek(6516568, SeekOrigin.Begin);
                fs.Write(Encoding.ASCII.GetBytes(build), 0, 5);
                fs.Seek(6549899, SeekOrigin.Begin);
                fs.Write(Encoding.ASCII.GetBytes(build), 0, 5);
                fs.Seek(7530139, SeekOrigin.Begin);
                fs.Write(Encoding.ASCII.GetBytes(build), 0, 5);

                // date 11 chars (Mon Da Year)
                // dont need this.

                // version
                fs.Seek(5020131, SeekOrigin.Begin);
                fs.WriteByte(highV);
                fs.Seek(5020135, SeekOrigin.Begin);
                fs.WriteByte(middleV);
                fs.Seek(5020139, SeekOrigin.Begin);
                fs.WriteByte(lowV);

                // reserved build - 2bytes
                fs.Seek(5020144, SeekOrigin.Begin);
                fs.Write(GetReversedBuild(build), 0, 2);
            }
        }
        private byte[] GetReversedBuild(string build)
        {
            ushort buildNumber = Convert.ToUInt16(build);
            // eg. 40712
            // build hex
            // 9F08
            string buildHex = String.Format("{0:X}", buildNumber);
            // reverse chars
            // 089F
            char[] hexChars = buildHex.ToCharArray();
            char[] reversedChars = new char[4];
            reversedChars[0] = hexChars[2];
            reversedChars[1] = hexChars[3];
            reversedChars[2] = hexChars[0];
            reversedChars[3] = hexChars[1];
            // get new number
            string reversedString = new string(reversedChars);
            ushort reversedBuild = Convert.ToUInt16(reversedString, 16);
            byte[] temp = BitConverter.GetBytes(reversedBuild);
            byte[] buildBytes = new byte[2];
            buildBytes[0] = temp[1];
            buildBytes[1] = temp[0];
            return buildBytes;
        }
        private async void LoadConfigration()
        {
            if (!File.Exists(confFile))
            {
                var c = await this.ShowProgressAsync("创建配置文档", "未找到配置文档，正在创建...");
                BuildConfFile(confFile);
                await Task.Delay(1000);
                await c.CloseAsync();
            }
            else
            {
                var c = await this.ShowProgressAsync("读取配置", "正在载入配置...");

                XmlDocument x = new XmlDocument();
                x.Load(confFile);
                XmlNode node = x.SelectSingleNode("/msatoolbox/legacy/update/conf");
                XmlElement e = (XmlElement)node;

                string s = e.GetAttribute("clientPath");
                if (s == null)
                    BuildConfFile(confFile);
                else
                {
                    ClientPath = s;
                    clientDirLabel.Content = ClientPath;
                    if (File.Exists(ClientPath + "\\" + "Wow.exe"))
                    {
                        clientDirLabel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB3FFA7"));
                        IsClientPathValid = true;
                    }
                    else
                    {
                        clientDirLabel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFA7A7"));
                        IsClientPathValid = false;
                    }
                }

                s = e.GetAttribute("saveUpdateFile");
                if (s == null)
                    BuildConfFile(confFile);
                else if (s == "true")
                {
                    ConfSaveUpdateFile = true;
                    confSaveUpdateFiles.IsChecked = true;
                }
                else if (s == "false")
                {
                    ConfSaveUpdateFile = false;
                    confSaveUpdateFiles.IsChecked = false;
                }
                else
                    BuildConfFile(confFile);

                s = e.GetAttribute("updateFilePath");
                if (s == null)
                    BuildConfFile(confFile);
                else
                {
                    LegacyUpdateDirectory = s;
                    confUpdateFilePath.Text = s;
                }

                s = e.GetAttribute("clearCache");
                if (s == null)
                    BuildConfFile(confFile);
                else if (s == "true")
                {
                    ClearCache = true;
                    confClearCache.IsChecked = true;
                }
                else if (s == "false")
                {
                    ClearCache = false;
                    confClearCache.IsChecked = false;
                }
                else
                    BuildConfFile(confFile);

                s = e.GetAttribute("startGameAfterUpdate");
                if (s == null)
                    BuildConfFile(confFile);
                else if (s == "false")
                {
                    StartGameAfterUpdate = false;
                    confAutoStartGame.IsChecked = false;
                }
                else if (s == "true")
                {
                    StartGameAfterUpdate = true;
                    confAutoStartGame.IsChecked = true;
                }
                else
                    BuildConfFile(confFile);

                s = e.GetAttribute("skipClientCheck");
                if (s == null)
                    BuildConfFile(confFile);
                else if (s == "true")
                {
                    SkipClientCheck = true;
                    // also skip client validation on update check
                    IsClientValid = true;
                    confSkipClientCheck.IsChecked = true;
                }
                else if (s == "false")
                {
                    SkipClientCheck = false;
                    confSkipClientCheck.IsChecked = false;
                }
                else
                    BuildConfFile(confFile);

                await Task.Delay(1000);
                await c.CloseAsync();
            }
        }
        private void BuildConfFile(string fileName)
        {
            File.Delete(fileName);
            XmlDocument confXml = new XmlDocument();
            XmlDeclaration dec = confXml.CreateXmlDeclaration("1.0", "utf-8", null);
            confXml.AppendChild(dec);
            XmlElement root = confXml.CreateElement("msatoolbox");
            XmlElement legacy = confXml.CreateElement("legacy");
            XmlElement update = confXml.CreateElement("update");
            XmlElement conf = confXml.CreateElement("conf");
            conf.SetAttribute("clientPath", "");
            conf.SetAttribute("saveUpdateFile", "true");
            conf.SetAttribute("updateFilePath", "LegacyUpdate");
            conf.SetAttribute("clearCache", "true");
            conf.SetAttribute("startGameAfterUpdate", "false");
            conf.SetAttribute("skipClientCheck", "false");
            update.AppendChild(conf);
            legacy.AppendChild(update);
            root.AppendChild(legacy);
            confXml.AppendChild(root);
            confXml.Save(fileName);
            confSaveUpdateFiles.IsChecked = true;
            confClearCache.IsChecked = true;
        }
        private void SaveConfig(string xmlPath, string attrName, string attrValue)
        {
            XmlDocument d = new XmlDocument();
            try { d.Load(confFile); }
            catch { BuildConfFile(confFile); d.Load(confFile); }
            XmlNode n = d.SelectSingleNode(xmlPath);
            XmlElement e = (XmlElement)n;
            e.SetAttribute(attrName, attrValue);
            d.Save(confFile);
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (confSaveUpdateFiles.IsChecked == true)
            {
                ConfSaveUpdateFile = true;
                SaveConfig("msatoolbox/legacy/update/conf", "saveUpdateFile", "true");
            }
            else
            {
                ConfSaveUpdateFile = false;
                SaveConfig("msatoolbox/legacy/update/conf", "saveUpdateFile", "false");
            }
        }
        private void confClearCache_Checked(object sender, RoutedEventArgs e)
        {
            if (confClearCache.IsChecked == true)
            {
                ClearCache = true;
                SaveConfig("msatoolbox/legacy/update/conf", "clearCache", "true");
            }
            else
            {
                ClearCache = false;
                SaveConfig("msatoolbox/legacy/update/conf", "clearCache", "false");
            }
        }
        private void confSkipClientCheck_Checked(object sender, RoutedEventArgs e)
        {
            if (confSkipClientCheck.IsChecked == true)
            {
                SkipClientCheck = true;
                SaveConfig("msatoolbox/legacy/update/conf", "skipClientCheck", "true");
            }
            else
            {
                SkipClientCheck = false;
                SaveConfig("msatoolbox/legacy/update/conf", "skipClientCheck", "false");
            }
        }
        private void confAutoStartGame_Checked(object sender, RoutedEventArgs e)
        {
            if (confAutoStartGame.IsChecked == true)
            {
                StartGameAfterUpdate = true;
                SaveConfig("msatoolbox/legacy/update/conf", "startGameAfterUpdate", "true");
            }
            else
            {
                StartGameAfterUpdate = false;
                SaveConfig("msatoolbox/legacy/update/conf", "startGameAfterUpdate", "false");
            }
        }
        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfigration();
            await Task.Delay(1000);
            GetClientBuild();
            await Task.Delay(1000);
            GetUpdateInfo();
            await Task.Delay(1000);
            ValidateStartButtonUsability();
            await Task.Delay(1000);
        }
    }
}
