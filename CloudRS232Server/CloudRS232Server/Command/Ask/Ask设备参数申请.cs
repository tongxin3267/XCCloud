using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CloudRS232Server.Command.Ask
{
    public class Ask设备参数申请
    {
        public byte 机头地址 { get; set; }
        public byte 设备类型 { get; set; }
        public byte 马达配置 { get; set; }
        /// <summary>
        /// 1 TM1639 4位*2行 2 TM1629 6位*2行
        /// </summary>
        public byte 数码管类型 { get; set; }
        public byte 马达1比例 { get; set; }
        public byte 马达2比例 { get; set; }
        public UInt16 存币箱最大存币数 { get; set; }
        public byte 是否允许打印 { get; set; }
        public byte SSR电平 { get; set; }
        public string 本店卡校验密码 { get; set; }

        public bool isSuccess = false;
        public Ask设备参数申请(Info.HeadInfo.机头绑定信息 Bind)
        {
            try
            {
                Info.HeadInfo.机头信息 机头 = Info.HeadInfo.GetHeadInfoByShort(Bind);
                DataAccess ac = new DataAccess();

                DataTable dt = ac.ExecuteQueryReturnTable(string.Format("select * from Base_DeviceInfo where SN='{0}'", 机头.常规.机头长地址));
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    机头地址 = Convert.ToByte(Bind.短地址, 16);

                    switch (row["type"].ToString())
                    {
                        case "售币机":
                            设备类型 = 0x01;
                            break;
                        case "存币机":
                            设备类型 = 0x02;
                            break;
                        case "提币机":
                        case "自助提币机":
                            设备类型 = 0x03;
                            break;
                        case "碎票机":
                            设备类型 = 0x04;
                            break;
                        case "投币机":
                            设备类型 = 0x05;
                            break;
                    }
                    string v = "000000";
                    v += row["motor2"].ToString();
                    v += row["motor1"].ToString();
                    马达配置 = Convert.ToByte(v, 2);
                    数码管类型 = Convert.ToByte(row["nixie_tube_type"].ToString());
                    马达1比例 = Convert.ToByte(row["motor1_coin"].ToString());
                    马达2比例 = Convert.ToByte(row["motor2_coin"].ToString());
                    存币箱最大存币数 = Convert.ToUInt16(row["alert_value"].ToString());
                    本店卡校验密码 = 机头.店密码;
                    是否允许打印 = Convert.ToByte(row["AllowPrint"].ToString());
                    try
                    {
                        SSR电平 = Convert.ToByte(row["SSR"].ToString());
                    }
                    catch
                    {
                        SSR电平 = 0x00;
                    }
                    isSuccess = true;
                }
            }
            catch { throw; }
        }
    }
}
