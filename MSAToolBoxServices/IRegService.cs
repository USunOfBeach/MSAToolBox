using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MSAToolBoxServices
{
    [ServiceContract]
    public interface IRegService
    {
        [OperationContract]
        RegResult TryRegister(byte[] data);
        [OperationContract]
        ChangePwResult TryChangePw(byte[] data);
    }
}
