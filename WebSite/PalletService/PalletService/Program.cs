
using PalletService.Init;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PalletService
{
    static class Program
    {
        public static Guid MyGuid = Guid.NewGuid();
        public static string MainVersion = "2.0.0";
        public static string StoreID = "100010";
        public static string ServerIP = "192.168.1.145";
        public static string CurDownload = "";
        public static string ProcessName = "莘宸自助机";

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationInit.Init();   
            Application.Run(new MainForm());
        }
    }
}
