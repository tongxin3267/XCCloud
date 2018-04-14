using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudWebSocketService
{
    [DataContract]
    public class CommonObject
    {
        [DataMember(Name = "key", Order = 1)]
        public string Key { set; get; }

        [DataMember(Name = "clientType", Order = 2)]
        public int ClientType { set; get; }

        [DataMember(Name = "sendStoreID", Order = 3)]
        public string SendStoreID { set; get; }

        [DataMember(Name = "sendUserID", Order = 4)]
        public string SendUserID { set; get; }

        [DataMember(Name = "receiveStoreID", Order = 5)]
        public string ReceiveStoreID { set; get; }

        [DataMember(Name = "receiveUserID", Order = 6)]
        public string ReceiveUserID { set; get; }

        [DataMember(Name = "content", Order = 7)]
        public string Content { set; get; }

    }
}
