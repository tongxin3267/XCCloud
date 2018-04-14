using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Messaging;

namespace CloudRS232Server.Command.Recv
{
    public class Recv机头网络状态报告
    {
        public FrameData RecvData;
        public byte[] SendData;
        public DateTime SendDate;
        public int 登记用户数 = 0;
        public delegate void DelegRun(FrameData data);
        public void Run(FrameData data)
        {
            try
            {
                登记用户数 = data.commandData[0];
                for (int i = 1; i < data.commandLength; i++)
                {
                    bool changed = false;   //状态是否改变
                    bool isAlert = false;
                    bool isLock = false;
                    int gInCoin, gOutCoin, headCount;
                    int alertCount = 0;
                    string 机头地址 = PubLib.Hex2String(data.commandData[i]);

                    Info.HeadInfo.机头绑定信息 bind = new Info.HeadInfo.机头绑定信息();
                    bind.控制器令牌 = data.Code;
                    bind.短地址 = 机头地址;
                    Info.HeadInfo.机头信息 机头 = Info.HeadInfo.GetHeadInfoByShort(bind);
                    if (机头 != null)
                    {
                        机头.是否从雷达获取到状态 = true;
                        Info.HeadInfo.机头干扰开关 报警 = new Info.HeadInfo.机头干扰开关();

                        i++;
                        if (i < data.commandLength)
                        {
                            string status = PubLib.Hex2BitString(data.commandData[i]);
                            bool OnlineFlag = (status.Substring(7, 1) == "1");
                            changed = (机头.状态.在线状态 != OnlineFlag);
                            if (OnlineFlag)
                            {
                                机头.不在线检测计数 = 0;
                                机头.状态.在线状态 = true;
                            }
                            else
                            {
                                机头.不在线检测计数++;
                                if (机头.不在线检测计数 > 2)
                                {
                                    机头.状态.在线状态 = false;
                                }
                            }
                            if (机头.状态.在线状态)
                            {
                                机头.状态.打印机故障 = (status.Substring(6, 1) == "1");
                                if (机头.报警回调数据.打印机故障 != 机头.状态.打印机故障)
                                {
                                    机头.报警回调数据.打印机故障 = 机头.状态.打印机故障;
                                    UpdateAlertDB(机头地址, 机头.状态.打印机故障, "打印机故障", 0, bind.控制器令牌);
                                    alertCount++;
                                    isAlert = true;
                                }
                                机头.状态.打印设置错误 = (status.Substring(5, 1) == "1");
                                if (机头.报警回调数据.打印设置错误 != 机头.状态.打印设置错误)
                                {
                                    机头.报警回调数据.打印设置错误 = 机头.状态.打印设置错误;
                                    UpdateAlertDB(机头地址, 机头.状态.打印设置错误, "打印设置错误", 0, bind.控制器令牌);
                                    alertCount++;
                                    isAlert = true;
                                }
                                机头.状态.读币器故障 = (status.Substring(3, 1) == "1");
                                if (机头.报警回调数据.读币器故障 != 机头.状态.读币器故障)
                                {
                                    机头.报警回调数据.读币器故障 = 机头.状态.读币器故障;
                                    UpdateAlertDB(机头地址, 机头.状态.读币器故障, "读币器故障", 0, bind.控制器令牌);
                                    alertCount++;
                                    isAlert = true;
                                }
                                机头.状态.锁定机头 = (status.Substring(0, 1) == "1");
                                isLock = 机头.状态.锁定机头;
                            }
                        }
                        i++;
                        if (i < data.commandLength)
                        {
                            if (机头.状态.在线状态)
                            {
                                string status = PubLib.Hex2BitString(data.commandData[i]);
                                报警.高频干扰报警 = (status.Substring(7, 1) == "1");
                                if (机头.报警回调数据.高频干扰报警 != 报警.高频干扰报警)
                                {
                                    机头.报警回调数据.高频干扰报警 = 报警.高频干扰报警;
                                    if (报警.高频干扰报警)
                                    {
                                        UpdateAlertDB(机头地址, 报警.高频干扰报警, "高频干扰报警", 1, bind.控制器令牌);
                                        alertCount++;
                                    }
                                    isAlert = true;
                                    isLock = true;
                                }
                                报警.高压干扰报警 = (status.Substring(6, 1) == "1");
                                if (机头.报警回调数据.高压干扰报警 != 报警.高压干扰报警)
                                {
                                    机头.报警回调数据.高压干扰报警 = 报警.高压干扰报警;
                                    if (报警.高压干扰报警 && alertCount < 4)
                                    {
                                        UpdateAlertDB(机头地址, 报警.高压干扰报警, "高压干扰报警", 1, bind.控制器令牌);
                                        alertCount++;
                                    }
                                    isAlert = true;
                                    isLock = true;
                                }
                                报警.SSR信号异常 = (status.Substring(5, 1) == "1");
                                if (机头.报警回调数据.SSR信号异常 != 报警.SSR信号异常)
                                {
                                    机头.报警回调数据.SSR信号异常 = 报警.SSR信号异常;
                                    if (报警.SSR信号异常 && alertCount < 4)
                                    {
                                        UpdateAlertDB(机头地址, 报警.SSR信号异常, "SSR信号异常", 1, bind.控制器令牌);
                                        alertCount++;
                                    }
                                    isAlert = true;
                                    isLock = true;
                                }
                                报警.CO信号异常 = (status.Substring(4, 1) == "1");
                                if (机头.报警回调数据.CO信号异常 != 报警.CO信号异常)
                                {
                                    机头.报警回调数据.CO信号异常 = 报警.CO信号异常;
                                    if (报警.CO信号异常 && alertCount < 4)
                                    {
                                        UpdateAlertDB(机头地址, 报警.CO信号异常, "CO信号异常", 1, bind.控制器令牌);
                                        alertCount++;
                                    }
                                    isAlert = true;
                                    isLock = true;
                                }
                                报警.CO2信号异常 = (status.Substring(3, 1) == "1");
                                if (机头.报警回调数据.CO2信号异常 != 报警.CO2信号异常)
                                {
                                    机头.报警回调数据.CO2信号异常 = 报警.CO2信号异常;
                                    if (报警.CO2信号异常 && alertCount < 4)
                                    {
                                        UpdateAlertDB(机头地址, 报警.CO2信号异常, "CO2信号异常", 1, bind.控制器令牌);
                                        alertCount++;
                                    }
                                    isAlert = true;
                                    isLock = true;
                                }
                                if (机头.类型 == Info.HeadInfo.设备类型.存币机)
                                {
                                    机头.状态.存币箱是否满 = (status.Substring(2, 1) == "1");
                                    if (机头.报警回调数据.存币箱满报警 != 机头.状态.存币箱是否满)
                                    {
                                        机头.报警回调数据.存币箱满报警 = 机头.状态.存币箱是否满;
                                        if (alertCount < 4)
                                        {
                                            UpdateAlertDB(机头地址, 机头.状态.存币箱是否满, "存币箱满报警", 0, bind.控制器令牌);
                                            alertCount++;
                                        }
                                        isAlert = true;
                                    }
                                }
                                else
                                {
                                    机头.状态.是否正在使用限时送分优惠 = (status.Substring(2, 1) == "1");
                                }
                                机头.状态.锁定机头 = isLock;
                            }
                        }

                        //if (!changed && alertCount > 0)
                        //changed = true;

                        //if (changed)
                        //{
                        //    if (状态.在线状态)
                        //    {
                        //        if (isAlert)
                        //            FrmMain.GetInterface.ChangeDeviceStatus(机头.常规.机头长地址, XCSocketService.DeviceStatusEnum.故障);
                        //        else
                        //        {
                        //            if (状态.锁定机头)
                        //                FrmMain.GetInterface.ChangeDeviceStatus(机头.常规.机头长地址, XCSocketService.DeviceStatusEnum.锁定);
                        //            else
                        //            {
                        //                if (状态.出币机或存币机正在数币)
                        //                    FrmMain.GetInterface.ChangeDeviceStatus(机头.常规.机头长地址, XCSocketService.DeviceStatusEnum.出币中);
                        //                else
                        //                    FrmMain.GetInterface.ChangeDeviceStatus(机头.常规.机头长地址, XCSocketService.DeviceStatusEnum.在线);
                        //            }
                        //        }
                        //    }
                        //    else
                        //        FrmMain.GetInterface.ChangeDeviceStatus(机头.常规.机头长地址, XCSocketService.DeviceStatusEnum.离线);
                        //}

                        //Info.HeadInfo.GetCoinByGame(机头.常规.游戏机编号, out gInCoin, out gOutCoin, out headCount);

                        //h.hAddress = 机头地址;
                        //h.InCoins = 机头.投币.投币数;
                        //h.OutCoins = 机头.投币.退币数;
                        //h.WinCoins = 机头.投币.盈利数;
                        //h.IsInCoin = false;
                        //h.IsOnline = 状态.在线状态;
                        //h.IsOutCoin = false;
                        //h.IsLock = isLock;
                        //h.IsOver = (0 - 机头.投币.盈利数 > 机头.投币.每天净退币上限);
                        //h.GameInCoins = gInCoin;
                        //h.GameOutCoins = gOutCoin;
                        //h.GameWinCoins = gInCoin - gOutCoin;
                        //if (机头.状态.锁定机头)
                        //{
                        //    h.IsLock = true;
                        //}
                        //else
                        //{
                        //    if (机头.开关.退币超限标志 && !机头.状态.是否忽略超分报警)
                        //    {
                        //        h.IsOver = true;
                        //    }
                        //}
                        //h.rAddress = PubLib.路由器段号;
                        //listStatus.Add(h);

                        //Info.HeadInfo.Update(PubLib.路由器段号, 机头地址, 状态, 报警);
                    }
                }
                //ServiceDll.ClientCall.机头状态汇报(listStatus.ToArray());
            }
            catch
            {
                throw;
            }
        }

        void UpdateAlertDB(string headAddress, bool AlertValue, string AlertType, int lockGame,string Code)
        {
            string sql = "";
            if (AlertValue)
            {
                sql = string.Format("if not exists(select * from flw_game_alarm where AlertContent like '{2}' and State='0' and Segment='{0}' and HeadAddress='{1}') begin INSERT INTO flw_game_alarm (ICCardID,Segment,HeadAddress,AlertType,HappenTime,State,LockGame,LockMember,AlertContent) VALUES (0,'{0}','{1}','{2}',GETDATE(),0,{3},0,'{2}') end",
                    Code, headAddress, AlertType, lockGame);
            }
            else
            {
                sql = string.Format("UPDATE flw_game_alarm SET EndTime=GETDATE(),State=1 where Segment='{0}' and HeadAddress='{1}' and AlertType='{2}' and State=0",
                    Code, headAddress, AlertType);
            }
            DataAccess ac = new DataAccess();
            ac.Execute(sql);
        }

        //public void RunCallBack(IAsyncResult ar)
        //{
        //    AsyncResult result = (AsyncResult)ar;
        //    DelegRun d = (DelegRun)result.AsyncDelegate;
        //    d.EndInvoke(ar);
        //}

        public Recv机头网络状态报告(FrameData data)
        {
            //RecvData = data;
            //DelegRun a = Run;
            //a.BeginInvoke(data, new AsyncCallback(RunCallBack), null);
            Run(data);
            SendData = PubLib.GetFrameDataBytes(data, null, CommandType.机头网络状态报告应答);
            SendDate = DateTime.Now;
        }
    }
}
