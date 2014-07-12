using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegacyUpdateServices;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace MSAToolBoxClient
{
    public static class DataManager
    {
        static IUpdateService channel;
        static string baseAddress = "net.tcp://localhost:5780/";

        public static string BaseAddress
        {
            get { return baseAddress; }
            set
            {
                channel = null;
                baseAddress = value;
            }
        }

        public static IUpdateService Channel
        {
            get
            {
                if (channel == null)
                {
                    NetTcpBinding binding = new NetTcpBinding()
                    {
                        TransferMode = TransferMode.Streamed,
                        MaxReceivedMessageSize = 2147483647 
                    };
                    EndpointAddress address = new EndpointAddress(new Uri(baseAddress));
                    var channelFactory = new ChannelFactory<IUpdateService>(binding, address);
                    channel = channelFactory.CreateChannel();
                }
                return channel;
            }
        }
    }
}
