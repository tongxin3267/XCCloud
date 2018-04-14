using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common.Enum;
using XCCloudService.Common.Extensions;

namespace XCCloudService.Model.CustomModel.XCCloudRS232
{
    [DataContract]
    public class DeviceModel
    {
        public DeviceModel(int bindState, string deviceName, string deviceToken, int deviceType, string deviceStatus)
        {
            this.BindState = bindState;
            this.DeviceName = deviceName;
            this.DeviceToken = deviceToken;
            this.DeviceType = ((DeviceTypeEnum)deviceType).ToDescription();
            this.DeviceStatus = deviceStatus;
        }

        [DataMember(Name = "BindState", Order = 0)]
        public int BindState { get; set; }

        [DataMember(Name = "DeviceName", Order = 1)]
        public string DeviceName { get; set; }

        [DataMember(Name = "DeviceToken", Order = 2)]
        public string DeviceToken { get; set; }

        [DataMember(Name = "DeviceType", Order = 3)]
        public string DeviceType { get; set; }

        [DataMember(Name = "DeviceStatus", Order = 4)]
        public string DeviceStatus { get; set; }
    }

    [DataContract]
    public class RouterModel
    {
        [DataMember(Name = "routerName", Order = 0)]
        public string routerName { get; set; }

        [DataMember(Name = "routerToken", Order = 1)]
        public string routerToken { get; set; }

        [DataMember(Name = "routerStatus", Order = 2)]
        public string routerStatus { get; set; }

        [DataMember(Name = "routerSN", Order = 3)]
        public string routerSN { get; set; }

        [DataMember(Name = "Groups", Order = 4)]
        public List<Group> Groups { get; set; }

        [DataMember(Name = "Peripherals", Order = 5)]
        public List<Peripheral> Peripherals { get; set; }
    }

    [DataContract]
    public class Peripheral 
    {
        [DataMember(Name = "peripheralId", Order = 0)]
        public int peripheralId { get; set; }

        [DataMember(Name = "peripheralName", Order = 1)]
        public string peripheralName { get; set; }

        [DataMember(Name = "deviceType", Order = 2)]
        public string deviceType { get; set; }

        [DataMember(Name = "state", Order = 3)]
        public string state { get; set; }

        [DataMember(Name = "sn", Order = 4)]
        public string sn { get; set; }

        [DataMember(Name = "peripheralToken", Order = 5)]
        public string peripheralToken { get; set; }

        [DataMember(Name = "headAddress", Order = 6)]
        public string headAddress { get; set; }
    }

    [DataContract]
    public class Group
    {
        public Group()
        {
            this.groupName = string.Empty;
            this.groupType = string.Empty;
        }

        [DataMember(Name = "groupId", Order = 0)]
        public int groupId { get; set; }

        [DataMember(Name = "groupName", Order = 1)]
        public string groupName { get; set; }

        [DataMember(Name = "groupType", Order = 2)]
        public string groupType { get; set; }
    }

    [DataContract]
    public class DeviceInfoModel
    {
        public DeviceInfoModel()
        {
            this.headAddress = "";
        }

        [DataMember(Name = "BindState", Order = 0)]
        public int BindState { get; set; }

        [DataMember(Name = "deviceToken", Order = 1)]
        public string deviceToken { get; set; }

        [DataMember(Name = "deviceName", Order = 2)]
        public string deviceName { get; set; }

        [DataMember(Name = "deviceType", Order = 3)]
        public string deviceType { get; set; }

        [DataMember(Name = "deviceSN", Order = 4)]
        public string deviceSN { get; set; }

        [DataMember(Name = "status", Order = 5)]
        public string status { get; set; }

        [DataMember(Name = "headAddress", Order = 6)]
        public string headAddress { get; set; }

        [DataMember(Name = "Router", Order = 7)]
        public Router Router { get; set; }

        [DataMember(Name = "Group", Order = 8)]
        public Group Group { get; set; }
    }

    [DataContract]
    public class Router
    {
        public Router()
        {
            this.routerToken = string.Empty;
            this.routerName = string.Empty;
            this.sn = string.Empty;
        }

        [DataMember(Name = "routerToken", Order = 0)]
        public string routerToken { get; set; }

        [DataMember(Name = "routerName", Order = 1)]
        public string routerName { get; set; }

        [DataMember(Name = "sn", Order = 2)]
        public string sn { get; set; }
    }

    [DataContract]
    public class WarnLogModel
    {
        [DataMember(Name = "ID", Order = 0)]
        public int ID { get; set; }

        [DataMember(Name = "ICCardID", Order = 1)]
        public int ICCardID { get; set; }

        [DataMember(Name = "MerchID", Order = 2)]
        public int MerchID { get; set; }

        [DataMember(Name = "DeviceID", Order = 3)]
        public int DeviceID { get; set; }

        [DataMember(Name = "DeviceName", Order = 4)]
        public string DeviceName { get; set; }

        [DataMember(Name = "DeviceType", Order = 4)]
        public string DeviceType { get; set; }

        [DataMember(Name = "SN", Order = 4)]
        public string SN { get; set; }

        [DataMember(Name = "AlertType", Order = 4)]
        public string AlertType { get; set; }

        [DataMember(Name = "HappenTime", Order = 5)]
        public string HappenTime { get; set; }

        [DataMember(Name = "EndTime", Order = 6)]
        public string EndTime { get; set; }

        [DataMember(Name = "State", Order = 7)]
        public int State { get; set; }

        [DataMember(Name = "LockGame", Order = 8)]
        public int LockGame { get; set; }

        [DataMember(Name = "LockMember", Order = 9)]
        public int LockMember { get; set; }

        [DataMember(Name = "AlertContent", Order = 10)]
        public string AlertContent { get; set; }

        [DataMember(Name = "HeadAddress", Order = 11)]
        public string HeadAddress { get; set; }

        [DataMember(Name = "RouterToken", Order = 12)]
        public string RouterToken { get; set; }
    }
}
