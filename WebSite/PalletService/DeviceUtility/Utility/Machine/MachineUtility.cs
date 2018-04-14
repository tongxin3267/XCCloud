using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PalletService.DeviceUtility.Utility.Machine
{
    public class MachineUtility
    {
        public static bool GetMachineName(out string computerName)
        {
            computerName = string.Empty;
            try
            {
                computerName = System.Environment.GetEnvironmentVariable("ComputerName");
                return true;
            }
            catch
            {
                return false;
            } 
        }
    }
}
