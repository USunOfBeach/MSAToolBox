using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;
using System.Runtime.Serialization;

namespace LegacyUpdateServices
{
    [ServiceContract]
    public interface IUpdateService 
    {
        [OperationContract]
        List<UpdateInfo> GetUpdateInfo();

        [OperationContract]
        List<UpdatePatchFileInfo> GetLatestPatchFiles();

        [OperationContract]
        Stream DownloadFile(UpdatePatchFileInfo fileInfo);
    }

    [DataContract]
    public class UpdateInfo
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public int Build { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember]
        public string Date { get; set; }
    }
    [DataContract]
    public class UpdatePatchFileInfo
    {
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string FilePath { get; set; }
        [DataMember]
        public int Build { get; set; }
        [DataMember]
        public long FileLength { get; set; }
    }
}
