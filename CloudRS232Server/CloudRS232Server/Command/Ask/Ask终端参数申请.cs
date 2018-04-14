using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CloudRS232Server.Command.Ask
{
    [Serializable]
    public class 游戏机维修开关信号1
    {
        public bool bit15保留 { get; set; }
        public bool bit14保留 { get; set; }
        public bool bit13保留 { get; set; }
        public bool bit12保留 { get; set; }
        public bool bit11保留 { get; set; }
        public bool bit10启用第二路上分信号 { get; set; }
        public bool bit9数币脉冲电平 { get; set; }
        public bool bit8SSR退币驱动脉冲电平 { get; set; }
        public bool bit7第二路上分线上分电平 { get; set; }
        public bool bit6投币脉冲电平 { get; set; }
        public bool bit5硬件投币控制 { get; set; }
        public bool bit4转发实物投币 { get; set; }
        public bool bit3允许实物退币 { get; set; }
        public bool bit2允许十倍投币 { get; set; }
        public bool bit1允许电子投币 { get; set; }
        public bool bit0允许电子退币或允许打票 { get; set; }
    }
    [Serializable]
    public class 游戏机维修开关信号2
    {
        public bool bit15保留 { get; set; }
        public bool bit14小票是否打印二维码 { get; set; }
        public bool bit13启用彩票模式 { get; set; }
        public bool bit12只退实物彩票 { get; set; }
        public bool bit11启用防霸位功能 { get; set; }
        public bool bit10启用刷卡版彩票功能 { get; set; }
        public bool bit9启用刷卡即扣 { get; set; }
        public bool bit8启用增强防止转卡 { get; set; }
        public bool bit7启用回路报警检测 { get; set; }
        public bool bit6启用外部报警检测 { get; set; }
        public bool bit5启用即中即退模式 { get; set; }
        public bool bit4启用异常退币检测 { get; set; }
        public bool bit3退分锁定标志 { get; set; }
        public bool bit2BO按钮是否维持 { get; set; }
        public bool bit1启用专卡专用 { get; set; }
        public bool bit0退币超限标志 { get; set; }
    }
    [Serializable]
    public class Ask终端参数申请
    {
        public byte 机头地址 { get; set; }
        public UInt16 单次退币限额 { get; set; }
        public byte 扣卡里币基数 { get; set; }
        public byte 退币时给游戏机脉冲数比例因子 { get; set; }
        public byte 退币时卡上增加币数比例因子 { get; set; }
        public UInt16 退币按钮脉宽 { get; set; }
        public string 本店卡校验密码 { get; set; }
        public UInt16 开关1 { get; set; }
        public UInt16 开关2 { get; set; }
        public byte 首次投币启动间隔 { get; set; }
        public UInt16 投币速度 { get; set; }
        public byte 投币脉宽 { get; set; }
        public byte 第二路上分线首次上分启动间隔 { get; set; }
        public UInt16 第二路上分线上分速度 { get; set; }
        public byte 第二路上分线上分脉宽 { get; set; }
        public UInt16 退币速度 { get; set; }
        public byte 退币脉宽 { get; set; }
        public string 游戏机机头编号 { get; set; }
        public byte 异常检测时间 { get; set; }
        public byte 异常检测次数 { get; set; }
        public byte 测试间隔时间 { get; set; }
        public byte 有效期天数 { get; set; }

        public Ask终端参数申请(Info.HeadInfo.机头绑定信息 Bind)
        {
            try
            {
                游戏机机头编号 = "0000A";

                Info.HeadInfo.机头信息 机头 = Info.HeadInfo.GetHeadInfoByShort(Bind);
                机头地址 = Convert.ToByte(Bind.短地址, 16);
                DataAccess ac = new DataAccess();
                DataTable dt = ac.ExecuteQueryReturnTable(string.Format("select g.* from Data_MerchSegment s,Base_DeviceInfo d,Data_GameInfo g where d.ID=s.DeviceID and g.GroupID=s.GroupID and d.SN='{0}'", 机头.常规.机头长地址));
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    有效期天数 = (byte)机头.打印小票有效天数;
                    机头.彩票模式 = (row["LotteryMode"].ToString() == "1");
                    单次退币限额 = Convert.ToUInt16(row["OnceOutLimit"]);
                    机头.参数.单次退币限额 = 单次退币限额;
                    机头.参数.投币时扣卡上币数 = Convert.ToByte(row["PushReduceFromCard"]);
                    //扣卡里币基数 = (byte)机头.参数.投币时扣卡上币数;
                    机头.参数.投币时给游戏机信号数 = Convert.ToUInt16(row["PushAddToGame"]);
                    机头.参数.第二路上分线投币时扣卡上币数 = Convert.ToInt32(row["SecondReduceFromCard"]);
                    机头.参数.第二路上分线投币时给游戏机信号数 = Convert.ToInt32(row["SecondAddToGame"]);
                    退币时给游戏机脉冲数比例因子 = Convert.ToByte(row["OutReduceFromGame"]);
                    机头.参数.退币时给游戏机脉冲数比例因子 = 退币时给游戏机脉冲数比例因子;
                    退币时卡上增加币数比例因子 = Convert.ToByte(row["OutAddToCard"]);
                    机头.参数.退币时卡上增加币数比例因子 = 退币时卡上增加币数比例因子;
                    退币按钮脉宽 = Convert.ToUInt16(row["BOPulse"]);
                    机头.参数.退币按钮脉宽 = 退币按钮脉宽;
                    游戏机维修开关信号1 开关信号1 = new 游戏机维修开关信号1();
                    开关信号1.bit0允许电子退币或允许打票 = Convert.ToBoolean(row["AllowElecOut"]);
                    机头.开关.允许电子退币或允许打票 = 开关信号1.bit0允许电子退币或允许打票;
                    开关信号1.bit1允许电子投币 = Convert.ToBoolean(row["AllowElecPush"]);
                    机头.开关.允许电子投币 = 开关信号1.bit1允许电子投币;
                    开关信号1.bit2允许十倍投币 = Convert.ToBoolean(row["AllowDecuplePush"]);
                    机头.开关.允许十倍投币 = 开关信号1.bit2允许十倍投币;
                    开关信号1.bit3允许实物退币 = Convert.ToBoolean(row["AllowRealOut"]);
                    机头.开关.允许实物退币 = 开关信号1.bit3允许实物退币;
                    开关信号1.bit4转发实物投币 = Convert.ToBoolean(row["AllowRealPush"]);
                    机头.开关.转发实物投币 = 开关信号1.bit4转发实物投币;
                    //开关信号1.bit5硬件投币控制 = Convert.ToBoolean(row["PushControl"]);
                    //机头.开关.硬件投币控制 = 开关信号1.bit5硬件投币控制;
                    开关信号1.bit6投币脉冲电平 = Convert.ToBoolean(row["PushLevel"]);
                    机头.开关.投币脉冲电平 = 开关信号1.bit6投币脉冲电平;
                    开关信号1.bit7第二路上分线上分电平 = Convert.ToBoolean(row["SecondLevel"]);
                    机头.开关.第二路上分线上分电平 = 开关信号1.bit7第二路上分线上分电平;
                    开关信号1.bit8SSR退币驱动脉冲电平 = Convert.ToBoolean(row["OutLevel"]);
                    机头.开关.SSR退币驱动脉冲电平 = 开关信号1.bit8SSR退币驱动脉冲电平;
                    开关信号1.bit9数币脉冲电平 = Convert.ToBoolean(row["CountLevel"]);
                    机头.开关.数币脉冲电平 = 开关信号1.bit9数币脉冲电平;
                    开关信号1.bit10启用第二路上分信号 = Convert.ToBoolean(row["UseSecondPush"]);
                    机头.开关.启用第二路上分信号 = 开关信号1.bit10启用第二路上分信号;
                    开关1 = PubLib.GetBit16ByObject(开关信号1);
                    游戏机维修开关信号2 开关信号2 = new 游戏机维修开关信号2();
                    开关信号2.bit0退币超限标志 = ((机头.投币.盈利数 > 机头.投币.每天净退币上限) && !机头.状态.是否忽略超分报警);
                    机头.开关.退币超限标志 = 开关信号2.bit0退币超限标志;
                    开关信号2.bit1启用专卡专用 = Convert.ToBoolean(row["GuardConvertCard"]);
                    机头.开关.启用专卡专用 = 开关信号2.bit1启用专卡专用;
                    开关信号2.bit2BO按钮是否维持 = Convert.ToBoolean(row["BOKeep"]);
                    机头.开关.BO按钮是否维持 = 开关信号2.bit2BO按钮是否维持;
                    开关信号2.bit3退分锁定标志 = Convert.ToBoolean(row["BOLock"]);
                    机头.开关.退分锁定标志 = 开关信号2.bit3退分锁定标志;
                    开关信号2.bit4启用异常退币检测 = Convert.ToBoolean(row["ExceptOutTest"]);
                    机头.开关.启用异常退币检测 = 开关信号2.bit4启用异常退币检测;
                    开关信号2.bit5启用即中即退模式 = Convert.ToBoolean(row["NowExit"]);
                    机头.开关.启用即中即退模式 = 开关信号2.bit5启用即中即退模式;
                    开关信号2.bit6启用外部报警检测 = Convert.ToBoolean(row["OutsideAlertCheck"]);
                    机头.开关.启用外部报警检测 = 开关信号2.bit6启用外部报警检测;
                    开关信号2.bit7启用回路报警检测 = Convert.ToBoolean(row["ReturnCheck"]);
                    机头.开关.启用回路报警检测 = 开关信号2.bit7启用回路报警检测;
                    开关信号2.bit8启用增强防止转卡 = Convert.ToBoolean(row["StrongGuardConvertCard"]);
                    机头.开关.增强防止转卡 = 开关信号2.bit8启用增强防止转卡;
                    开关信号2.bit9启用刷卡即扣 = Convert.ToBoolean(row["ReadCat"]);
                    机头.开关.启动刷卡即扣 = 开关信号2.bit9启用刷卡即扣;
                    开关信号2.bit10启用刷卡版彩票功能 = Convert.ToBoolean(row["ICTicketOperation"]);
                    机头.开关.启用刷卡版彩票 = 开关信号2.bit10启用刷卡版彩票功能;
                    开关信号2.bit11启用防霸位功能 = Convert.ToBoolean(row["BanOccupy"]);
                    机头.开关.启用防霸位功能 = 开关信号2.bit11启用防霸位功能;
                    开关信号2.bit12只退实物彩票 = Convert.ToBoolean(row["OnlyExitLottery"]);
                    机头.开关.只退实物彩票 = 开关信号2.bit12只退实物彩票;
                    开关信号2.bit13启用彩票模式 = 机头.彩票模式;
                    开关信号2.bit14小票是否打印二维码 = 机头.开关.小票是否打印二维码;
                    开关2 = PubLib.GetBit16ByObject(开关信号2);
                    首次投币启动间隔 = Convert.ToByte(row["PushStartInterval"]);
                    机头.参数.首次投币启动间隔 = 首次投币启动间隔;
                    投币速度 = Convert.ToUInt16(row["PushSpeed"]);
                    机头.参数.投币速度 = 投币速度;
                    投币脉宽 = Convert.ToByte(row["PushPulse"]);
                    机头.参数.投币脉宽 = 投币脉宽;
                    第二路上分线首次上分启动间隔 = Convert.ToByte(row["SecondStartInterval"]);
                    机头.参数.第二路上分线首次上分启动间隔 = 第二路上分线首次上分启动间隔;
                    第二路上分线上分速度 = Convert.ToUInt16(row["SecondSpeed"]);
                    机头.参数.第二路上分线上分速度 = 第二路上分线上分速度;
                    第二路上分线上分脉宽 = Convert.ToByte(row["SecondPulse"]);
                    机头.参数.第二路上分线上分脉宽 = 第二路上分线上分脉宽;
                    退币速度 = Convert.ToUInt16(row["OutSpeed"]);
                    机头.参数.退币速度 = 退币速度;
                    退币脉宽 = Convert.ToByte(row["OutPulse"]);
                    机头.参数.退币脉宽 = 退币脉宽;
                    机头.投币.单次退币上限 = 单次退币限额;
                    机头.投币.每天净退币上限 = Convert.ToInt32(row["OneDayPureOutLimit"]);
                    //游戏机机头编号 = 机头.常规.机头编号;
                    异常检测时间 = Convert.ToByte(row["ExceptOutSpeed"]);
                    机头.参数.异常SSR退币检测速度 = 异常检测时间;
                    异常检测次数 = Convert.ToByte(row["Frequency"]);
                    机头.参数.异常SSR退币检测次数 = 异常检测次数;
                    if (机头.开关.启用第二路上分信号)
                    {
                        扣卡里币基数 = (byte)机头.参数.第二路上分线投币时扣卡上币数;
                    }
                    else
                    {
                        扣卡里币基数 = (byte)机头.参数.投币时扣卡上币数;
                    }
                    本店卡校验密码 = 机头.店密码;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
