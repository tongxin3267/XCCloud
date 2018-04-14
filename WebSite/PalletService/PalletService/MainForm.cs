
using DeviceUtility.Utility.Coin;
using DeviceUtility.Utility.MemberCard;
using DeviceUtility.Utility.Print;
using ICSharpCode.SharpZipLib.Zip;
using PalletService.Business.Device;
using PalletService.Business.SysConfig;
using PalletService.Common;
using PalletService.DeviceUtility.Common;
using PalletService.Model.Device;
using PalletService.Utility.Dog;
using PalletService.Utility.MemberCard;
using PalletService.Utility.PeopleCard;
using ServiceDll;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PalletService
{
    public partial class MainForm : Form
    {
        int updateFlag = 0;
        bool isWait = false;
        int WaitDelay = 0;
        int WaitType = 0;
        int RequestCount = 0;
        int checkFlag = 0;
        int diskID = 0;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.SysConfigInit();
            this.ContextMenuInit();
            this.UpgradeInit();
        }

        private void UpgradeInit()
        {
            CallBackEvent.OnSoftVersionAnswer += new CallBackEvent.软件版本应答(CallBackEvent_OnSoftVersionAnswer);
            CallBackEvent.OnDownLoadComplite += new CallBackEvent.下载文件应答(CallBackEvent_OnDownLoadComplite);
            CallBackEvent.OnDownloadProcess += new CallBackEvent.下载进度(CallBackEvent_OnDownloadProcess);
            Client.Init(Program.ServerIP, 3456, Program.MyGuid, ClientType.后台客户端);
            //ClientCall.软件版本请求指令(Program.MyGuid, Program.StoreID, 1);
            //isWait = true;
            //WaitDelay = 0;
            //WaitType = 1;
            tmrUpdate.Enabled = true;
        }

        #region "升级"

            Rectangle rect;

            delegate void DelegShowprocess(int pIndex, int pCount);
            void Showprocess(int pIndex, int pCount)
            {
                if (this.InvokeRequired)
                {
                    DelegShowprocess a = new DelegShowprocess(Showprocess);
                    this.Invoke(a, new object[] { pIndex, pCount });
                }
                else
                {
                    rect = new Rectangle(0, 0, pIndex * this.Width / pCount, this.Height);
                    this.Refresh();
                }
            }

            void CallBackEvent_OnDownloadProcess(int pIndex, int pCount)
            {
                WaitDelay = 0;
                Console.WriteLine(pIndex + "  " + pCount);
                Showprocess(pIndex, pCount);
            }

            delegate void DelegComplite(Stream fileStream);
            void CallBackEvent_OnDownLoadComplite(Stream fileStream)
            {
                if (this.InvokeRequired)
                {
                    DelegComplite a = new DelegComplite(CallBackEvent_OnDownLoadComplite);
                    this.Invoke(a, fileStream);
                }
                else
                {
                    if (fileStream.Length > 0)
                    {
                        Showprocess(100, 100);

                        isWait = false;
                        WaitDelay = 0;

                        string updatePath = string.Format("{0}{1}{2}", Application.StartupPath, "\\update\\", Program.CurDownload);
                        using (FileStream sw = new FileStream(updatePath,FileMode.Create))
                        {
                            fileStream.CopyTo(sw);
                            sw.Flush();
                            sw.Close();

                            tmrCheck.Enabled = true;
                        }
                    }
                }
            }

            delegate void DelegVersionAnswer(TransmiteObject.软件版本号应答结构 soft);
            void CallBackEvent_OnSoftVersionAnswer(TransmiteObject.软件版本号应答结构 soft)
            {
                if (this.InvokeRequired)
                {
                    DelegVersionAnswer a = new DelegVersionAnswer(CallBackEvent_OnSoftVersionAnswer);
                    this.Invoke(a, soft);
                }
                else
                {
                    isWait = false;
                    WaitDelay = 0;

                    Console.WriteLine("检测到版本：" + soft.软件版本号);

                    string[] myver = Program.MainVersion.Split('.');
                    string[] curver = soft.软件版本号.Split('.');
                    bool isUpdate = false;
                    for (int i = 0; i < myver.Length; i++)
                    {
                        if (Convert.ToInt32(myver[i]) < Convert.ToInt32(curver[i]))
                        {
                            isUpdate = true;
                            break;
                        }
                    }
                    if (isUpdate)
                    {
                        //需要升级
                        //lblInfo.Text = "检测到新版本，正在下载...";
                        Program.CurDownload = soft.升级包名;
                        ClientCall.下载文件("update\\" + Program.CurDownload);
                        Console.WriteLine("下载文件：" + soft.升级包名);
                        WaitType = 2;
                        WaitDelay = 0;
                        isWait = true;
                        tmrUpdate.Enabled = false;
                    }
                    else
                    {
                        Application.Exit();
                    }
                }
            }

            /// <summary>
            /// 解压缩一个 zip 文件。
            /// </summary>
            /// <param name="zipedFile">The ziped file.</param>
            /// <param name="strDirectory">The STR directory.</param>
            /// <param name="password">zip 文件的密码。</param>
            /// <param name="overWrite">是否覆盖已存在的文件。</param>
            public bool UnZip(string zipedFile, string strDirectory, string password, bool overWrite)
            {
                if (strDirectory == "")
                    strDirectory = Directory.GetCurrentDirectory();
                if (!strDirectory.EndsWith("\\"))
                    strDirectory = strDirectory + "\\";

                try
                {
                    using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipedFile)))
                    {
                        s.Password = password;
                        ZipEntry theEntry;

                        while ((theEntry = s.GetNextEntry()) != null)
                        {
                            string directoryName = "";
                            string pathToZip = "";
                            pathToZip = theEntry.Name;

                            if (pathToZip != "")
                                directoryName = Path.GetDirectoryName(pathToZip) + "\\";

                            string fileName = Path.GetFileName(pathToZip);

                            Directory.CreateDirectory(strDirectory + directoryName);

                            if (fileName != "")
                            {
                                if ((File.Exists(strDirectory + directoryName + fileName) && overWrite) || (!File.Exists(strDirectory + directoryName + fileName)))
                                {
                                    using (FileStream streamWriter = File.Create(strDirectory + directoryName + fileName))
                                    {
                                        int size = 2048;
                                        byte[] data = new byte[2048];
                                        while (true)
                                        {
                                            size = s.Read(data, 0, data.Length);

                                            if (size > 0)
                                                streamWriter.Write(data, 0, size);
                                            else
                                                break;
                                        }
                                        streamWriter.Close();
                                    }
                                }
                            }
                        }
                        s.Close();
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            /// <summary>
            /// 删除目录，递归法
            /// </summary>
            /// <param name="path"></param>
            void DeletePath(string path)
            {
                foreach (string f in Directory.GetFiles(path))
                {
                    File.Delete(f);
                }
                foreach (string d in Directory.GetDirectories(path))
                {
                    DeletePath(d);
                    Directory.Delete(d);
                }
            }

            void CheckUpdate()
            {
                tmrUpdate.Enabled = false;
                switch (checkFlag)
                {
                    case 0: //从磁盘检测
                        if (diskID > 5)
                        {
                            //lblInfo.Text = "未能从磁盘检测到更新";
                            checkFlag++;
                        }
                        else
                        {
                            string diskName = Convert.ToChar(67 + diskID).ToString();
                            //lblInfo.Text = "正在检测" + diskName + "盘";
                            if (File.Exists(diskName + ":\\" + Program.ProcessName + ".zip"))
                            {
                                //找到相关压缩包
                                tmrUpdate.Enabled = false;
                                //lblInfo.Text = "正在解压缩...";
                                if (Directory.Exists(Application.StartupPath + "\\update\\unzip"))
                                    DeletePath(Application.StartupPath + "\\update\\unzip");
                                if (UnZip(diskName + ":\\" + Program.ProcessName + ".zip", Application.StartupPath + "\\update\\unzip", "", true))
                                {
                                    updateFlag++;
                                    tmrCheck.Enabled = true;
                                }
                                else
                                {
                                    //lblInfo.Text = "文件解压失败";

                                    Application.DoEvents();
                                    Thread.Sleep(2000);
                                    Application.Exit();
                                }
                            }
                            diskID++;
                        }
                        break;
                    case 1:
                        //lblInfo.Text = "从网络检查更新...";

                        ClientCall.软件版本请求指令(Program.MyGuid, Program.StoreID, 1);
                        isWait = true;
                        WaitDelay = 0;
                        WaitType = 1;
                        RequestCount++;
                        //if (RequestCount > 5)
                            //Application.Exit();
                        break;
                }
                tmrUpdate.Enabled = true;
            }

            void UpdateProcess()
            {
                tmrCheck.Enabled = false;
                switch (updateFlag)
                {
                    case 0: //解压文件
                        //lblInfo.Text = "正在解压缩...";
                        if (Directory.Exists(Application.StartupPath + "\\update\\unzip"))
                            DeletePath(Application.StartupPath + "\\update\\unzip");
                        if (UnZip(Application.StartupPath + "\\update\\" + Program.CurDownload, Application.StartupPath + "\\update\\unzip", "", true))
                            updateFlag++;
                        else
                        {
                            //lblInfo.Text = "文件解压失败";
                            Application.DoEvents();
                            File.Delete(Application.StartupPath + "\\update\\" + Program.CurDownload);
                            Thread.Sleep(2000);
                            Application.Exit();
                        }
                        break;
                    case 1://关闭进程
                        //lblInfo.Text = "关闭已打开的程序...";
                        foreach (Process p in Process.GetProcesses())
                        {
                            Console.WriteLine(p.ProcessName);
                            if (p.ProcessName.Contains(Program.ProcessName))
                            {
                                p.Kill();
                            }
                        }
                        updateFlag++;
                        break;
                    case 2: //更新文件
                        //lblInfo.Text = "正在更新文件...";
                        //删除广告文件
                        string[] l = Directory.GetFiles(@"images");
                        foreach (string f in l)
                        {
                            File.Delete(f);
                        }
                        CopyFolderTo(Application.StartupPath + "\\update\\unzip", Application.StartupPath);
                        if (Directory.Exists(Application.StartupPath + "\\update\\unzip\\images"))
                        {
                            CopyFolderTo(Application.StartupPath + "\\update\\unzip\\images", Application.StartupPath + "\\images");
                        }
                        if (Directory.Exists(Application.StartupPath + "\\update\\unzip\\files"))
                        {
                            CopyFolderTo(Application.StartupPath + "\\update\\unzip\\files", Application.StartupPath + "\\files");
                        }
                        if (Directory.Exists(Application.StartupPath + "\\update\\unzip\\music"))
                        {
                            CopyFolderTo(Application.StartupPath + "\\update\\unzip\\music", Application.StartupPath + "\\music");
                        }
                        updateFlag++;
                        break;
                    case 3: //启动文件
                        //lblInfo.Text = "正在启动程序...";
                        Process pStart = new Process();
                        pStart.StartInfo.WorkingDirectory = Application.StartupPath;    //要启动程序路径
                        pStart.StartInfo.FileName = Program.ProcessName + ".exe";//需要启动的程序名     
                        pStart.Start();//启动 
                        updateFlag++;

                        Application.Exit();
                        break;
                    default:
                        return;
                }
                tmrCheck.Enabled = true;
            }


            /// <summary>
            /// 从一个目录将其内容复制到另一目录
            /// </summary>
            /// <param name="directorySource">源目录</param>
            /// <param name="directoryTarget">目标目录</param>
            public void CopyFolderTo(string directorySource, string directoryTarget)
            {

                //先来复制文件  
                DirectoryInfo directoryInfo = new DirectoryInfo(directorySource);
                FileInfo[] files = directoryInfo.GetFiles();
                string s = Application.StartupPath;
                int v = 50 / files.Length;

                //复制所有文件  
                foreach (FileInfo file in files)
                {
                    try
                    {
                        file.CopyTo(Path.Combine(directoryTarget, file.Name), true);
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message);
                    }
                }
            }


            private void tmrCheck_Tick(object sender, EventArgs e)
            {
                int i = 0;
                i++;
                UpdateProcess();
            }

            private void tmrTimeOut_Tick(object sender, EventArgs e)
            {
                if (isWait)
                {
                    WaitDelay++;
                    if (WaitDelay > 2)
                    {
                        if (WaitType == 1)
                        {
                            ClientCall.软件版本请求指令(Program.MyGuid, Program.StoreID, 1);
                            isWait = true;
                            WaitDelay = 0;
                            WaitType = 1;
                        }
                        else if (WaitType == 2)
                        {
                            ClientCall.下载文件("update\\" + Program.CurDownload);
                            WaitType = 3;
                            WaitDelay = 0;
                            isWait = true;
                        }
                        else if (WaitType == 3)
                        {
                            Console.WriteLine("下载超时");
                            //WaitType = 2;
                            ClientCall.文件下载超时();
                            WaitDelay = 0;
                        }
                    }
                }
            }

            private void tmrUpdate_Tick(object sender, EventArgs e)
            {
                CheckUpdate();
            }

        #endregion

        #region "菜单"

        private void ContextMenuInit()
            {
                LoadDeviceMenu(this.toolStripMenuItem_MemberCard, DeviceDefine.ICCard);
                LoadDeviceMenu(this.toolStripMenuItem_PeopleCard, DeviceDefine.PeopleCard);
                LoadDeviceMenu(this.toolStripMenuItem_EncryptDog, DeviceDefine.EncryptDog);
                LoadDeviceMenu(this.toolStripMenuItem_PalmPrints, DeviceDefine.PalmPrints);
                LoadPrinterMenu(this.toolStripMenuItem_Printer);
                LoadDeviceMenu(this.toolStripMenuItem_Printer, DeviceDefine.Printer);
            }

            private void LoadPrinterMenu(ToolStripMenuItem parentMenu)
            {
                string selectedPrinter = string.Empty;
                List<DeviceModel> installList = new List<DeviceModel>();
                List<DeviceModel> list = DeviceBusiness.GetDevice(DeviceDefine.Printer);
                List<DeviceModel> listFilter = DeviceBusiness.GetDevice(DeviceDefine.PrinterFilter);
                DeviceModel deviceModel = list.Find(p => p.Selected == true);
                if (deviceModel != null)
                {
                    selectedPrinter = deviceModel.Name;
                }

                foreach (string printer in PrinterSettings.InstalledPrinters)
                {
                    deviceModel = new DeviceModel();
                    deviceModel.Name = printer;
                    deviceModel.Model = printer;

                    if (listFilter.Find(p => p.Model.Equals(printer)) == null)
                    {
                        if (selectedPrinter.Equals(printer))
                        {
                            deviceModel.Selected = true;
                        }
                        else
                        {
                            deviceModel.Selected = false;
                        }
                        installList.Add(deviceModel);
                    }
                }
                DeviceBusiness.ReplaceDevice(installList, DeviceDefine.Printer);
            }

            private void LoadDeviceMenu(ToolStripMenuItem parentMenu, string deviceType)
            {
                List<DeviceModel> listDeviceModel = DeviceBusiness.GetDevice(deviceType);
                foreach (var model in listDeviceModel)
                {
                    //IC卡设备菜单
                    ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
                    toolStripMenuItem.Name = model.Model;
                    toolStripMenuItem.Size = new System.Drawing.Size(152, 22);
                    toolStripMenuItem.Text = model.Name;
                    toolStripMenuItem.Tag = new DeviceTagModel(deviceType, model.Model, parentMenu);
                    toolStripMenuItem.Checked = model.Selected;
                    toolStripMenuItem.CheckOnClick = true;
                    toolStripMenuItem.Click += ToolStripMenuItem_Device_Click;
                    parentMenu.DropDownItems.Add(toolStripMenuItem);
                    if (model.Selected)
                    {
                        DeviceBusiness.SetSelectedDeviceList(deviceType, model.Model);
                    }
                }
            }

            private void ToolStripMenuItem_Device_Click(object sender, EventArgs e)
            {
                ToolStripMenuItem menuItem = ((ToolStripMenuItem)(sender));
                DeviceTagModel tagModel = (DeviceTagModel)(menuItem.Tag);
                ToolStripMenuItem parentMenuItem = (ToolStripMenuItem)(tagModel.ParentMenuItem);
                foreach (ToolStripMenuItem item in parentMenuItem.DropDownItems)
                {
                    DeviceTagModel itemTagModel = (DeviceTagModel)(item.Tag);
                    if (!tagModel.Model.Equals(itemTagModel.Model))
                    {
                        item.Checked = false;
                    }
                    else
                    {
                        item.Checked = true;
                        DeviceBusiness.SetSelectedDevice(itemTagModel.Type, itemTagModel.Model);
                        DeviceBusiness.SetSelectedDeviceList(itemTagModel.Type, itemTagModel.Model);
                    }
                }
            }

        #endregion

        #region "系统初始化"

            private void SysConfigInit()
            {
                this.txtPrinterName.Text = SysConfigBusiness.PrinterName;
            }

        #endregion

        #region "主窗体事件"

            /// <summary>
            /// 窗体尺寸改变
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void MainForm_SizeChanged(object sender, EventArgs e)
            {
                //判断是否选择的是最小化按钮
                if (WindowState == FormWindowState.Minimized)
                {
                    //隐藏任务栏区图标
                    this.ShowInTaskbar = false;
                    //图标显示在托盘区
                    notifyIcon1.Visible = true;
                }
            }

            private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
            {
                if (MessageBox.Show("是否确认退出程序？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    // 关闭所有的线程
                    this.Dispose();
                    this.Close();
                }
                else
                {
                    e.Cancel = true;
                }
            }

            /// <summary>
            /// 托盘图标双击
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    //还原窗体显示    
                    WindowState = FormWindowState.Normal;
                    //激活窗体并给予它焦点
                    this.Activate();
                    //任务栏区显示图标
                    this.ShowInTaskbar = true;
                    //托盘区图标隐藏
                    notifyIcon1.Visible = false;
                }
            }

            private void toolStripMenuItem_Exit_Click(object sender, EventArgs e)
            {
                if (MessageBox.Show("是否确认退出程序？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    // 关闭所有的线程
                    this.Dispose();
                    this.Close();
                }
            }

        #endregion

        #region "掌纹采集事件"

            private void btnWhoIs1_Click(object sender, EventArgs e)
            {
                this.axwhois_enroll_ocx1.start("", "");
                btnWhoIs1.Enabled = false;
            }

            private void btnWhoIs2_Click(object sender, EventArgs e)
            {
                this.axwhois_feature_extract1.start("", "");
            }

            private void axwhois_enroll_ocx1_EnrollFinish(object sender, Axwhois_enroll_ocxLib._Dwhois_enroll_ocxEvents_EnrollFinishEvent e)
            {
                btnWhoIs1.Enabled = true;
                Console.WriteLine("掌纹数据：" + e.feature_str);
                axwhois_enroll_ocx1.stop();
            }

            private void axwhois_feature_extract1_FeatureGot(object sender, Axwhois_feature_extractLib._Dwhois_feature_extractEvents_FeatureGotEvent e)
            {
                Console.WriteLine("掌纹数据：" + e.feature_str);
                axwhois_feature_extract1.stop();
            }

        #endregion 

    }
}
