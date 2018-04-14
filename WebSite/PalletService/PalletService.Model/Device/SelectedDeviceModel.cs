using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PalletService.Model.Device
{
    public class SelectedDeviceModel
    {
        public SelectedDeviceModel(string type, string model)
        {
            this.Type = type;
            this.Model = model;
        }

        public string Type { set; get; }

        public string Model { set; get; }
    }
}
