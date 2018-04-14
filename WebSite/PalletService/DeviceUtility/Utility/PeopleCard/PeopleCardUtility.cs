using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;

namespace PalletService.Utility.PeopleCard
{
    public class PeopleCardUtility
    {
        private IntPtr formHandle = IntPtr.Zero;
        const int MSG_CARD_READY = 0x0400 + 6;

        [DllImport("termb.dll")]
        public extern static int InitComm(int Port);
        [DllImport("termb.dll")]
        public extern static int InitCommExt();
        [DllImport("termb.dll")]
        public extern static int Authenticate();
        [DllImport("termb.dll")]
        public extern static int Read_Content(int Active);
        [DllImport("termb.dll")]
        unsafe public extern static int Read_Content_Path(string strPath, int Active);
        [DllImport("termb.dll")]
        unsafe public extern static int CloseComm();
        [DllImport("termb.dll")]
        unsafe public extern static int GetBmpPhoto(string Wlt_File);
        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);


        /// <summary>
        /// 自动初始化设备
        /// 非0则表示初始化成功，0则初始化失败
        /// </summary>
        /// <returns></returns>
        bool OpenDevice()
        {
            int nPort = InitCommExt();
            return nPort > 0;
        }
        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <returns></returns>
        bool CloseDevice()
        {
            int res = CloseComm();
            return res == 1;
        }


        /// <summary>
        /// 读取身份证信息
        /// </summary>
        /// <param name="card">正确读卡时，返回身份证信息</param>
        /// <param name="msg">出错时，返回错误信息</param>
        /// <returns></returns>
        public bool ReadIDCard(out PeopleCardModel card, out string msg)
        {
            msg = "";
            card = new PeopleCardModel();
            try
            {
                if (!OpenDevice())
                {
                    msg = "读卡器未连接";
                    return false;
                }
                int n = Authenticate();
                int res = Read_Content(2);
                if (res != 1)
                {
                    msg = "读卡错误";
                    return false;
                }
                ReadWZFile(ref card);
                CloseDevice();
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return false;
        }

        void ReadWZFile(ref PeopleCardModel card)
        {
            if (File.Exists("wz.txt"))
            {
                byte[] data = null;
                using (FileStream fs = new FileStream("wz.txt", FileMode.Open, FileAccess.Read))
                {
                    data = new byte[fs.Length];
                    fs.Read(data, 0, (int)fs.Length);
                    fs.Close();
                }
                card.姓名 = Encoding.Unicode.GetString(data, 0, 30).Trim();
                switch (Encoding.ASCII.GetString(data, 30, 2).Replace("\0", ""))
                {
                    case "0":
                        card.性别 = "未知";
                        break;
                    case "1":
                        card.性别 = "男";
                        break;
                    case "2":
                        card.性别 = "女";
                        break;
                    default:
                        card.性别 = "未说明";
                        break;
                }
                switch (Encoding.ASCII.GetString(data, 32, 4).Replace("\0", ""))
                {
                    case "01":
                        card.民族 = "汉族";
                        break;
                    case "02":
                        card.民族 = "蒙古族";
                        break;
                    case "03":
                        card.民族 = "回族";
                        break;
                    case "04":
                        card.民族 = "藏族";
                        break;
                    case "05":
                        card.民族 = "维吾尔族";
                        break;
                    case "06":
                        card.民族 = "苗族";
                        break;
                    case "07":
                        card.民族 = "彝族";
                        break;
                    case "08":
                        card.民族 = "壮族";
                        break;
                    case "09":
                        card.民族 = "布依族";
                        break;
                    case "10":
                        card.民族 = "朝鲜族";
                        break;
                    case "11":
                        card.民族 = "满族";
                        break;
                    case "12":
                        card.民族 = "侗族";
                        break;
                    case "13":
                        card.民族 = "瑶族";
                        break;
                    case "14":
                        card.民族 = "白族";
                        break;
                    case "15":
                        card.民族 = "土家族";
                        break;
                    case "16":
                        card.民族 = "哈尼族";
                        break;
                    case "17":
                        card.民族 = "哈萨克族";
                        break;
                    case "18":
                        card.民族 = "傣族";
                        break;
                    case "19":
                        card.民族 = "黎族";
                        break;
                    case "20":
                        card.民族 = "傈僳族";
                        break;
                    case "21":
                        card.民族 = "佤族";
                        break;
                    case "22":
                        card.民族 = "畲族";
                        break;
                    case "23":
                        card.民族 = "高山族";
                        break;
                    case "24":
                        card.民族 = "拉祜族";
                        break;
                    case "25":
                        card.民族 = "水族";
                        break;
                    case "26":
                        card.民族 = "东乡族";
                        break;
                    case "27":
                        card.民族 = "纳西族";
                        break;
                    case "28":
                        card.民族 = "景颇族";
                        break;
                    case "29":
                        card.民族 = "柯尔克孜族";
                        break;
                    case "30":
                        card.民族 = "土族";
                        break;
                    case "31":
                        card.民族 = "达斡尔族";
                        break;
                    case "32":
                        card.民族 = "仫佬族";
                        break;
                    case "33":
                        card.民族 = "羌族";
                        break;
                    case "34":
                        card.民族 = "布朗族";
                        break;
                    case "35":
                        card.民族 = "撒拉族";
                        break;
                    case "36":
                        card.民族 = "毛南族";
                        break;
                    case "37":
                        card.民族 = "仡佬族";
                        break;
                    case "38":
                        card.民族 = "锡伯族";
                        break;
                    case "39":
                        card.民族 = "阿昌族";
                        break;
                    case "40":
                        card.民族 = "普米族";
                        break;
                    case "41":
                        card.民族 = "塔吉克族";
                        break;
                    case "42":
                        card.民族 = "怒族";
                        break;
                    case "43":
                        card.民族 = "乌孜别克族";
                        break;
                    case "44":
                        card.民族 = "俄罗斯族";
                        break;
                    case "45":
                        card.民族 = "鄂温克族";
                        break;
                    case "46":
                        card.民族 = "德昂族";
                        break;
                    case "47":
                        card.民族 = "保安族";
                        break;
                    case "48":
                        card.民族 = "裕固族";
                        break;
                    case "49":
                        card.民族 = "京族";
                        break;
                    case "50":
                        card.民族 = "塔塔尔族";
                        break;
                    case "51":
                        card.民族 = "独龙族";
                        break;
                    case "52":
                        card.民族 = "鄂伦春族";
                        break;
                    case "53":
                        card.民族 = "赫哲族";
                        break;
                    case "54":
                        card.民族 = "门巴族";
                        break;
                    case "55":
                        card.民族 = "珞巴族";
                        break;
                    case "56":
                        card.民族 = "基诺族";
                        break;
                    default:
                        card.民族 = "未知";
                        break;
                }
                string date = Encoding.ASCII.GetString(data, 36, 16).Replace("\0", "");
                card.生日 = date.Substring(0, 4) + "-" + date.Substring(4, 2) + "-" + date.Substring(6, 2);
                card.住址 = Encoding.Unicode.GetString(data, 52, 70).Trim();
                card.身份证号 = Encoding.ASCII.GetString(data, 122, 36).Replace("\0", "");
                card.签发机关 = Encoding.Unicode.GetString(data, 158, 30).Trim();
                date = Encoding.ASCII.GetString(data, 188, 16).Replace("\0", "");
                card.发证日期 = date.Substring(0, 4) + "-" + date.Substring(4, 2) + "-" + date.Substring(6, 2);
                date = Encoding.ASCII.GetString(data, 204, 16).Replace("\0", "");
                card.有效期限 = date.Substring(0, 4) + "-" + date.Substring(4, 2) + "-" + date.Substring(6, 2);
                int res = GetBmpPhoto("xp.wlt");
                if (res == 1)
                {
                    FileStream fs = new FileStream("zp.bmp", FileMode.Open, FileAccess.Read);
                    byte[] b = new byte[fs.Length];
                    fs.Read(b, 0, b.Length);
                    MemoryStream ms = new MemoryStream(b, false);
                    fs.Close();
                    card.照片 = new Bitmap(ms);
                }
                File.Delete("wz.txt");
                File.Delete("xp.wlt");
                File.Delete("zp.bmp");
            }
        }
    }
}
