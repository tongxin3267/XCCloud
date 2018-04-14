using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PalletService.Model.Device
{
    public class DeviceTagModel
    {
        public DeviceTagModel(string type,string model,object memuItem)
        {
            this.Type = type;
            this.Model = model;
            this.ParentMenuItem = memuItem;
        }

        public string Type { set; get; }

        public string Model { set; get; }

        public object ParentMenuItem { set; get; }
    }
}
