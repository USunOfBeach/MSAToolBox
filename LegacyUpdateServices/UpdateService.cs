using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading;
using LegacyUpdateServices;
using MySql.Data.MySqlClient;

namespace LegacyUpdateServices
{
    public class UpdateService : IUpdateService
    {
        public static List<UpdatePatchFileInfo> patchList = new List<UpdatePatchFileInfo>();
        public static string PatchDirectory = "";

        private MySqlConnection db_connection;
        public UpdateService()
        {
            db_connection = new MySqlConnection(String.Format("Server={0};Database={1};Uid={2};Pwd={3};Charset={4};", "localhost", "legacyupdateinfo", "root", "", "utf8"));
            db_connection.Open();
        }
        public Stream DownloadFile(UpdatePatchFileInfo fileInfo)
        {
            string filePath = PatchDirectory + "\\" + (fileInfo.FilePath == "" ? fileInfo.FileName : fileInfo.FilePath + "\\" + fileInfo.FileName);
            if (!File.Exists(filePath))
                return null;
            else
            {
                var incomingRequest = WebOperationContext.Current.IncomingRequest;
                var outgoingResponse = WebOperationContext.Current.OutgoingResponse;
                long offset = 0, count = fileInfo.FileLength;
                outgoingResponse.ContentType = "application/force-download";
                outgoingResponse.ContentLength = count;

                LegacyStreamReader fs = new LegacyStreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read), offset, count);
                fs.Reading += (t) =>
                    {
                        Thread.Sleep(300);
                    };
                return fs;
            }
        }
        public List<UpdateInfo> GetUpdateInfo()
        {
            List<UpdateInfo> list = new List<UpdateInfo>();
            MySqlCommand cmd = db_connection.CreateCommand();
            cmd.CommandText = String.Format("SELECT `version`, `build`, `title`, `content`, `date` FROM updateinfo ORDER BY id DESC LIMIT 10");
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                UpdateInfo update = new UpdateInfo();
                update.Version = reader.GetString(0);
                update.Build = reader.GetInt32(1);
                update.Title = reader.GetString(2);
                update.Content = reader.GetString(3);
                update.Date = reader.GetString(4);
                list.Add(update);
            }
            reader.Close();
            return list;
        }
        public List<UpdatePatchFileInfo> GetLatestPatchFiles()
        {
            int maxbuild = 0;
            foreach (UpdatePatchFileInfo info in patchList)
                maxbuild = info.Build > maxbuild ? info.Build : maxbuild;

            List<UpdatePatchFileInfo> latestPatches = new List<UpdatePatchFileInfo>();
            foreach (UpdatePatchFileInfo info in patchList)
            {
                if (info.Build == maxbuild)
                    latestPatches.Add(info);
            }
            return latestPatches;
        }
    }
}
