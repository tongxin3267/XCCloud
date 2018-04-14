using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XXCloudService.Utility.Info
{
    public class RouterInfo
    {
        public RouterInfo()
        {
            this.DeviceInfo = new DeviceInfo();
            this.MerchInfo = new MerchInfo();
        }

        public int RouterId { get; set; }

        public string RouterToken { get; set; }

        public virtual DeviceInfo DeviceInfo { get; set; }

        public virtual MerchInfo MerchInfo { get; set; }

        public string IP { get; set; }

        public int Port { get; set; }

        public string Online { get; set; }
    }

    public class DeviceInfo
    {
        public string Token { get; set; }

        public string DeviceName { get; set; }

        public int DeviceType { get; set; }

        public string SN { get; set; }

        public int Status { get; set; }
    }

    public class MerchInfo
    {
        public string MerchName { get; set; }

        public string Mobile { get; set; }

        public int State { get; set; }
    }

    public class DeviceModel
    {
        public string RouterToken { get; set; }

        public int DeviceId { get; set; }

        public string DeviceToken { get; set; }

        public string DeviceName { get; set; }

        public string DeviceType { get; set; }

        public string State { get; set; }

        public string SN { get; set; }

        public string HeadAddress { get; set; }

        public int GroupId { get; set; }
    }

    public class GroupInfo
    {
        public GroupInfo()
        {
            this.GroupName = string.Empty;
            this.GroupType = string.Empty;
        }

        public string RouterToken { get; set; }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string GroupType { get; set; }
    }
}