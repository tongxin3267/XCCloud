using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace CloudRS232Server.Info
{
    public class HeadInfo
    {
        public class IC卡进出币控制信号结构
        {
            public bool 保留7 { get; set; }
            public bool 保留6 { get; set; }
            public bool 保留5是否正在使用限时送分优惠券 { get; set; }
            public bool 保留4是否将退币上回游戏机 { get; set; }
            public bool 保留3超出当日机头最大净退币上线 { get; set; }
            public bool 保留2是否启用卡片专卡专用功能 { get; set; }
            public bool 保留1当前卡是否允许退分 { get; set; }
            public bool 保留0当前卡是否允许上分 { get; set; }
        }

        public class IC卡模式进出币应答结构
        {
            public bool 机头能上分 { get; set; }
            public bool 机头能打票 { get; set; }
            public bool 锁机头 { get; set; }
            public int 发脉冲数 { get; set; }
            public int 币余额 { get; set; }
            public bool 是否启用卡片专卡专用 { get; set; }
            public bool 超出当日机头最大净退币上线 { get; set; }
            public bool 是否将退币上回游戏机 { get; set; }
            public bool 是否正在使用限时送分优惠券 { get; set; }
        }

        public enum 设备类型
        {
            控制器,
            提_售币机,
            存币机,
            出票器,
            卡头,
        }

        public class 存售提币机碎票机参数
        {
            public bool 马达1启用标识;
            public bool 马达2启用标识;
            public int 数码管类型;
            public byte 设备数币计数;
            public byte 卡上增加币数;
            public int 最小数币数;
            public int 存币箱最大存币报警阀值;
            public bool 是否允许打印;
        }

        public class 机头常规属性
        {
            public string 机头编号 = "";
            public string 游戏机编号 = "";
            public string 游戏机名 = "";
            public string 路由器编号 = "";
            public string 机头地址 = "";
            public string 机头长地址 = "";
            public string 当前卡片号 = "";
            public string 管理卡号 = "";
            public string 设备名称 = "";
            public int 当前会员卡级别 = 0;
            public bool 是否为首次投币 = false;
            public int 退币信号超时时间 = 0;
            public int 退币信号超时退币个数 = 0;
            public bool 退币保护启用标志 = false;
        }

        public class 机头干扰开关
        {
            public bool 长地址登记标示 { get; set; }
            public bool bit6 { get; set; }
            public bool 存币箱满或限时优惠锁定 { get; set; }
            public bool CO2信号异常 { get; set; }
            public bool CO信号异常 { get; set; }
            public bool SSR信号异常 { get; set; }
            public bool 高压干扰报警 { get; set; }
            public bool 高频干扰报警 { get; set; }
        }

        public class 投币信息
        {
            public int 投币数 = 0;
            public int 退币数 = 0;
            public int 盈利数 { get { return 投币数 - 退币数; } }
            public int 单次退币上限 = 0;
            public int 每天净退币上限 = 0;
            public int 最小退币数 = 0;
        }
        /// <summary>
        /// 由路由器报告取值
        /// </summary>
        public class 状态值
        {
            public bool 锁定机头 = false;
            public bool 允许打票 = false;
            public bool 机头能上分 = false;
            public bool 读币器故障 = false;
            public bool 非法币或卡报警 = false;
            public bool 打印设置错误 = false;
            public bool 打印机故障 = false;
            public bool 在线状态 = false;
            public bool 是否正在使用限时送分优惠 = false;
            public bool 存币箱是否满 = false;
            public bool 是否忽略超分报警 = false;
            public bool 超级解锁卡标识 = false;
            public bool 出币机或存币机正在数币 = false;
        }
        public class 参数属性
        {
            public int 单次退币限额 = 0;
            public int 投币时扣卡上币数 = 0;
            public int 投币时给游戏机信号数 = 0;
            public int 退币时给游戏机脉冲数比例因子 = 0;
            public int 退币时卡上增加币数比例因子 = 0;
            public int 退币按钮脉宽 = 0;
            public int 异常SSR退币检测次数 = 0;
            public int 异常SSR退币检测速度 = 0;
            public int 首次投币启动间隔 = 0;
            public int 退币速度 = 0;
            public int 退币脉宽 = 0;
            public int 投币速度 = 0;
            public int 投币脉宽 = 0;
            public int 第二路上分线投币时扣卡上币数 = 0;
            public int 第二路上分线投币时给游戏机信号数 = 0;
            public int 第二路上分线上分速度 = 0;
            public int 第二路上分线上分脉宽 = 0;
            public int 第二路上分线首次上分启动间隔 = 0;
            public int 打印灰度 = 0;
        }
        public class 开关信号
        {
            public bool 硬件投币控制 = false;
            public bool 异常SSR退币检测或异常打票控制 = false;
            public bool 保留字 = false;
            public bool 退币超限标志 = false;
            public bool 允许实物退币 = false;
            public bool 允许电子投币 = false;
            public bool 允许十倍投币 = false;
            public bool 允许电子退币或允许打票 = false;
            public bool 第二路上分线上分电平 = false;
            public bool 启用第二路上分信号 = false;
            public bool 转发实物投币 = false;
            public bool SSR退币驱动脉冲电平 = false;
            public bool 数币脉冲电平 = false;
            public bool 投币脉冲电平 = false;
            public bool 增强防止转卡 = false;
            public bool 启用专卡专用 = false;
            public bool 启用异常退币检测 = false;
            public bool 是否启用最小退币功能 = false;
            public bool BO按钮是否维持 = false;
            public bool 退分锁定标志 = false;
            public bool 启用即中即退模式 = false;
            public bool 启用防霸位功能 = false;
            public bool 是否启用卡片专卡专用 = false;
            public bool 启用外部报警检测 = false;
            public bool 启用回路报警检测 = false;
            public bool 启动刷卡即扣 = false;
            public bool 启用刷卡版彩票 = false;
            public bool 只退实物彩票 = false;
            public bool 小票是否打印二维码 = false;
        }
        public class 机头报警结构
        {
            public bool CO2信号异常 { get; set; }
            public bool CO信号异常 { get; set; }
            public string 路由器令牌 { get; set; }
            public string 设备短地址 { get; set; }
            public bool SSR信号异常 { get; set; }
            public bool 存币箱满报警 { get; set; }
            public bool 打印机故障 { get; set; }
            public bool 打印设置错误 { get; set; }
            public bool 读币器故障 { get; set; }
            public bool 非法币或卡报警 { get; set; }
            public bool 高频干扰报警 { get; set; }
            public bool 高压干扰报警 { get; set; }
        }

        public class 机头信息
        {
            public 机头常规属性 常规 = new 机头常规属性();
            public 投币信息 投币 = new 投币信息();
            public 状态值 状态 = new 状态值();
            public 参数属性 参数 = new 参数属性();
            public 开关信号 开关 = new 开关信号();
            public 机头干扰开关 报警 = new 机头干扰开关();
            public 设备类型 类型;
            public bool 是否从雷达获取到状态 = false;
            public 机头报警结构 报警回调数据 = new 机头报警结构();
            public 存售提币机碎票机参数 存币机 = new 存售提币机碎票机参数();
            public int 临时错误计数;
            public int 不在线检测计数;
            public bool 彩票模式 = false;
            public int 打印小票有效天数 = 0;
            public string 订单编号 = "";
            public string 令牌 = "";
            public string 二维码 = "";
            public string 店密码 = "";
            public string 商户号 = "";
            int coinSN = 0;
            public int 远程投币上分流水号
            {
                get
                {
                    coinSN++;
                    if (coinSN == 65535)
                        coinSN = 1;
                    else if (coinSN == 0)
                        coinSN++;
                    return coinSN;
                }
            }
        }

        public class 机头绑定信息
        {
            public string 控制器令牌 = "";
            public string 短地址 = "";
            public string 长地址 = "";
        }
        //按照控制器令牌创建文件，按机头短地址序号写入16位ASCII码卡号
        static Dictionary<string, byte[]> SaveFileBUF = new Dictionary<string, byte[]>();

        /// <summary>
        /// 按长地址记录机头信息
        /// </summary>
        static Dictionary<string, 机头信息> HeadListMCUID = new Dictionary<string, 机头信息>();
        /// <summary>
        /// 按机头绑定信息记录设备长地址
        /// </summary>
        static HashSet<机头绑定信息> HeadListAddress = new HashSet<机头绑定信息>();

        public static List<机头信息> GetAllHead()
        {
            return new List<机头信息>(HeadListMCUID.Values.ToArray());
        }

        public static 机头信息 GetHeadInfoByShort(机头绑定信息 ShortAddress)
        {
            foreach (机头绑定信息 bind in HeadListAddress)
            {
                if (bind.短地址 == ShortAddress.短地址 && bind.控制器令牌 == ShortAddress.控制器令牌)
                {
                    return GetHeadInfoByFull(bind.长地址);
                }
            }
            return null;
        }

        public static 机头信息 GetHeadInfoByFull(string MCUID)
        {
            if (HeadListMCUID.ContainsKey(MCUID))
            {
                return HeadListMCUID[MCUID];
            }
            else
            {
                机头信息 head = CreateHeadByFull(MCUID);
                HeadListMCUID.Add(MCUID, head);
                return head;
            }
        }

        /// <summary>
        /// 根据长地址创建机头信息
        /// 终端设备在申请短地址时才允许调用
        /// </summary>
        /// <param name="MCUID"></param>
        /// <returns></returns>
        static 机头信息 CreateHeadByFull(string MCUID)
        {
            string sql = string.Format("select * from Base_DeviceInfo where SN='{0}'", MCUID);
            DataAccess ac = new DataAccess();
            DataTable dt = ac.ExecuteQueryReturnTable(sql);
            if (dt.Rows.Count == 0) return null;
            DataRow row = dt.Rows[0];

            机头信息 head = new 机头信息();
            head.类型 = (设备类型)Convert.ToInt32(row["DeviceType"]);
            head.令牌 = row["token"].ToString();
            head.二维码 = row["qrurl"].ToString();
            if (head.类型 == 设备类型.出票器 || head.类型 == 设备类型.卡头)
                sql = string.Format("select m.HeadAddress,m.GroupID,d.Token,d.ID,d.SN,d.MerchID from Data_MerchSegment m,Base_DeviceInfo d where m.ParentID=d.ID and m.DeviceID='{0}'", row["ID"].ToString());
            else
                sql = string.Format("select m.HeadAddress,d.Token,d.ID,d.SN,d.MerchID from Data_MerchDevice m,Base_DeviceInfo d where m.ParentID=d.ID and m.DeviceID='{0}'", row["ID"].ToString());
            DataTable tp = ac.ExecuteQueryReturnTable(sql);
            if (tp.Rows.Count == 0) return null;
            DataRow rp = tp.Rows[0];
            head.常规.路由器编号 = rp["Token"].ToString();
            head.常规.机头长地址 = MCUID;
            head.常规.机头地址 = rp["HeadAddress"].ToString();
            head.常规.机头编号 = rp["ID"].ToString();
            head.商户号 = rp["MerchID"].ToString();
            if (head.类型 == 设备类型.出票器 || head.类型 == 设备类型.卡头)
                head.常规.游戏机编号 = rp["GroupID"].ToString();
            机头绑定信息 bind = new 机头绑定信息();
            bind.控制器令牌 = head.常规.路由器编号;
            bind.短地址 = head.常规.机头地址;
            bind.长地址 = MCUID;
            HeadListAddress.Add(bind);

            sql = string.Format("select m.StorePassword from Base_DeviceInfo d,Base_MerchInfo m where d.MerchID=m.ID and d.SN='{0}'", MCUID);
            dt = ac.ExecuteQueryReturnTable(sql);
            if (dt.Rows.Count > 0)
            {
                head.店密码 = dt.Rows[0]["StorePassword"].ToString();
            }
            return head;
        }

        /// <summary>
        /// 判断机头地址是否合法
        /// </summary>
        /// <param name="Code">路由器令牌</param>
        /// <param name="HeadAddress">机头地址</param>
        /// <returns></returns>
        public bool ExistsHeadInfo(string Code, string HeadAddress)
        {
            DataAccess ac = new DataAccess();
            DataTable dt = ac.ExecuteQueryReturnTable("select d.ID from Base_DeviceInfo d,Data_MerchSegment m where d.ID=m.ParentID and m.HeadAddress='" + HeadAddress + "' and d.Token='" + Code + "'");
            return dt.Rows.Count > 0;
        }

        /// <summary>
        /// 初始化设备列表
        /// </summary>
        public static void InitDeviceInfo()
        {
            string sql = string.Format("select * from Base_DeviceInfo");
            DataAccess ac = new DataAccess();
            DataTable dt = ac.ExecuteQueryReturnTable(sql);
            if (dt.Rows.Count == 0) return;

            foreach (DataRow row in dt.Rows)
            {
                string MCUID = row["SN"].ToString();
                机头信息 head = CreateHeadByFull(MCUID);
                if (head != null)
                {
                    HeadListMCUID.Add(MCUID, head);

                    机头绑定信息 bind = new 机头绑定信息();
                    bind.控制器令牌 = head.常规.路由器编号;
                    bind.短地址 = head.常规.机头地址;
                    bind.长地址 = MCUID;
                    HeadListAddress.Add(bind);
                }
            }
        }

        #region 会员卡机头缓存
        /// <summary>
        /// 将卡号写入缓存文件
        /// </summary>
        /// <param name="hAddress">机头地址</param>
        /// <param name="ICCardID">卡号</param>
        public static void WriteBufHead(string Code, string hAddress, string ICCardID)
        {
            if (!SaveFileBUF.ContainsKey(Code))
            {
                byte[] data = new byte[255 * 16];
                SaveFileBUF.Add(Code, data);
            }

            int i = Convert.ToInt32(hAddress, 16);
            byte[] card = Encoding.ASCII.GetBytes(ICCardID);
            Array.Copy(card, 0, SaveFileBUF[Code], i * 16, 16);
        }
        /// <summary>
        /// 将缓存写入文件
        /// </summary>
        public static void WriteBUFFile()
        {
            foreach (string code in SaveFileBUF.Keys)
            {
                //检查缓存目录
                if (!Directory.Exists("/BUF"))
                    Directory.CreateDirectory("/BUF");
                //写入缓存
                StreamWriter sw = new StreamWriter("/BUF/" + code + ".dat");
                sw.Write(SaveFileBUF[code]);
                sw.Flush();
                sw.Close();
                sw.Dispose();
                sw = null;
            }
        }

        public static void ReadBUFFile()
        {
            if (!Directory.Exists("/BUF")) return;
            //读取缓存文件
            string[] filelist = Directory.GetFiles("/BUF");
            //遍历缓存文件
            foreach (string file in filelist)
            {
                //获取文件名
                string filename = file.ToLower().Replace(".dat", "");
                byte[] data = new byte[255 * 16];
                //读取文件内容
                StreamReader sr = new StreamReader("/BUF/" + file);
                int i = 0;
                while (!sr.EndOfStream)
                {
                    data[i++] = (byte)sr.Read();
                }
                sr.Close();
                sr.Dispose();
                sr = null;
                //写入缓存
                if (!SaveFileBUF.ContainsKey(filename))
                {
                    SaveFileBUF.Add(filename, data);
                }
            }

            foreach (string MCUID in HeadListMCUID.Keys)
            {
                //遍历机头信息
                string code = HeadListMCUID[MCUID].常规.路由器编号;
                int i = Convert.ToInt32(HeadListMCUID[MCUID].常规.机头地址, 16);

                if (SaveFileBUF.ContainsKey(code))
                {
                    byte[] tmp = new byte[16];
                    Array.Copy(SaveFileBUF[code], i * 8, tmp, 0, 16);   //读取卡号信息
                    string cardNum = Encoding.ASCII.GetString(tmp).Replace("\0", "");   //还原卡号
                    HeadListMCUID[MCUID].常规.当前卡片号 = cardNum;    //缓存卡号
                }
            }
        }
        #endregion
    }
}
