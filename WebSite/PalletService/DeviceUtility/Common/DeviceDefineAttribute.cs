using System;
using System.Collections.Generic;
using System.Text;

namespace PalletService.DeviceUtility.Common
{
    public class DeviceDefineAttribute:Attribute
    {
        public string ChineseName { set; get; }
    }
}
