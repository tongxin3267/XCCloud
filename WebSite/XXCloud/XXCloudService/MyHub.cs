using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using XXCloudService.RadarServer;
using Newtonsoft.Json;
using System.Net;
using XCCloudService.Business.XCCloudRS232;
using System.Data;
using XCCloudService.Utility;
using XXCloudService.RadarServer.Info;
using XXCloudService.Utility.Info;
using XCCloudService.Common.Enum;
using XCCloudService.Common.Extensions;

namespace XXCloudService
{
    [HubName("MyHub")]
    public class MyHub : Hub
    {
        //private readonly static List<Connect> _connections = new List<Connect>();

        public static List<XXCloudService.MyHub.DataLength> SendLengthList = new List<XXCloudService.MyHub.DataLength>();
        public static List<XXCloudService.MyHub.DataLength> RecvLengthList = new List<XXCloudService.MyHub.DataLength>();

        #region 推送指令数和控制器集合
        /// <summary>
        /// 推送指令
        /// </summary>
        public void PushRoutersAndInst(string clientId, int level = 1, string token = "", string routerName = "", int groupId = 0, string groupName = "", int pageIndex = 1, string commands = "")
        {
            //Connect connect = _connections.FirstOrDefault(c => c.ClientId == clientId);
            //if (connect == null)
            //{
            //    if (_connections.Count > 10)
            //    {
            //        _connections.RemoveAt(0);
            //    }
            //    connect = new Connect() { ClientId = clientId, ConnectionId = Context.ConnectionId };
            //    _connections.Add(connect);
            //}

            PushRouterInstModel model = new PushRouterInstModel();
            try
            {
                InstructionsModel inst = new InstructionsModel();

                inst.Total = PubLib.当前总指令数;
                inst.Querys = PubLib.当前查询指令数;
                inst.Coins = PubLib.当前币业务指令数;
                inst.ICCardCoinRepeats = PubLib.当前IC卡进出币指令重复数;
                inst.ICCardQueryRepeats = PubLib.当前IC卡查询重复指令数;
                inst.Receipts = PubLib.当前小票指令数;
                inst.Errors = PubLib.当前错误指令数;
                inst.Returns = PubLib.当前返还指令数;
                model.Instructions = inst;

                model.CurrVar = new CurrVar() { Level = level, CurrRouterToken = token, CurrRouterName = routerName, CurrGroupId = groupId, CurrGroupName = groupName };

                GetDeviceInfo(model, level, token, groupId);

                List<string> msgList = UDPServer.当前消息列表;
                model.Messages = msgList.Select(t => new InstMessage { MsgContent = t }).ToList();
                UDPServer.MessageStore("", false);

                string date = DateTime.Now.ToString("HH:mm:ss");
                model.SendLength = new DataLength() { y = PubLib.SendDataLength, name = date };
                model.RecvLength = new DataLength() { y = PubLib.RecvDataLength, name = date };

                if (SendLengthList.Count >= 50)
                {
                    SendLengthList.RemoveAt(0);
                    RecvLengthList.RemoveAt(0);
                }
                SendLengthList.Add(model.SendLength);
                RecvLengthList.Add(model.RecvLength);

                model.SendLengthList = SendLengthList;
                model.RecvLengthList = RecvLengthList;

                model.PerMinuteCount = UDPServer.PerMinuteCount;

                PubLib.SendDataLength = 0;
                PubLib.RecvDataLength = 0;

                //Clients.Client(connect.ConnectionId).PullInst(model);
                Clients.All.PullInst(model);
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.msg = ex.Message;
                Clients.All.PullInst(model);
            }
        } 
        #endregion

        #region 远程事件
        /// <summary>
        /// 锁定/解锁
        /// </summary>
        /// <param name="routerToken"></param>
        /// <param name="headAddress"></param>
        /// <param name="isLock"></param>
        public void LockDevice(string routerToken, string headAddress, bool isLock)
        {
            ResultModel model = new ResultModel();
            UDPServer.server.机头锁定解锁指令(routerToken, headAddress, isLock);

            string jsonData = JsonConvert.SerializeObject(model);
            Clients.All.HubCall(model);

        }

        /// <summary>
        /// 远程投币
        /// </summary>
        /// <param name="routerToken"></param>
        /// <param name="headAddress"></param>
        /// <param name="icCard"></param>
        /// <param name="coins"></param>
        public void InCoins(string routerToken, string headAddress, string icCard, int coins)
        {
            ResultModel model = new ResultModel();
            UDPServer.server.远程投币上分(routerToken, headAddress, icCard, coins);

            string jsonData = JsonConvert.SerializeObject(model);
            Clients.All.HubCall(model);

        }

        /// <summary>
        /// 退币
        /// </summary>
        /// <param name="routerToken"></param>
        /// <param name="headAddress"></param>
        public void OutCoins(string routerToken, string headAddress)
        {
            ResultModel model = new ResultModel();
            UDPServer.server.远程退币(routerToken, headAddress);

            string jsonData = JsonConvert.SerializeObject(model);
            Clients.All.HubCall(model);

        }

        /// <summary>
        /// 设置机头长地址
        /// </summary>
        /// <param name="routerToken"></param>
        /// <param name="mcuid"></param>
        public void SetDeviceSN(string routerToken, string mcuid)
        {
            ResultModel model = new ResultModel();
            UDPServer.server.设置机头长地址(routerToken, mcuid);

            string jsonData = JsonConvert.SerializeObject(model);
            Clients.All.HubCall(model);

        }

        /// <summary>
        /// 指定路由器复位
        /// </summary>
        /// <param name="routerToken"></param>
        /// <param name="mcuid"></param>
        public void ResetRouter(string routerToken)
        {
            ResultModel model = new ResultModel();
            UDPServer.server.指定路由器复位(routerToken);

            string jsonData = JsonConvert.SerializeObject(model);
            Clients.All.InitDeviceCall(model);

        } 

        public void InitDevice()
        {
            HeadInfo.InitDeviceInfo();
            UDPServer.InitRouterDevices();
            ResultModel model = new ResultModel();
            string jsonData = JsonConvert.SerializeObject(model);
            Clients.All.InitDeviceCall(model);
        }

        public void SetMessageCommandType(int type, bool check)
        {
            RadarServer.CommandType commandType = (RadarServer.CommandType)type;
            if(check)
            {
                UDPServer.server.CommandTypeList.Add(commandType);
            }
            else
            {
                UDPServer.server.CommandTypeList.Remove(commandType);
            }
        }

        public void ClearMessageCommand()
        {
            UDPServer.server.CommandTypeList.Clear();
            UDPServer.server.ListenRouterToken = string.Empty;
            UDPServer.server.ListenDeviceAddress = string.Empty;
        }

        public void SetListenRouter(string routerToken, bool isListen)
        {
            if (isListen)
            {
                UDPServer.server.ListenRouterToken = routerToken;
            }
            else
            {
                UDPServer.server.ListenRouterToken = string.Empty;
            }
        }

        public void SetListenDevice(string routerToken, string deiveceToken, bool isListen)
        {
            if (isListen)
            {
                UDPServer.server.ListenRouterToken = routerToken;
                UDPServer.server.ListenDeviceAddress = deiveceToken;
            }
            else
            {
                UDPServer.server.ListenDeviceAddress = string.Empty;
            }
        }
        #endregion

        #region ResultModel
        public class ResultModel
        {
            public ResultModel(int _status = 200, string _msg = "")
            {
                this.status = _status;
                this.msg = _msg;
            }
            public int status { get; set; }

            public string msg { get; set; }
        } 
        #endregion

        #region 获取设备列表
        public void GetDeviceInfo(PushRouterInstModel model, int level, string routerToken = "", int groupId = 0, int pageSize = 10, int pageIndex = 1)
        {
            switch (level)
            {
                case 1:
                    Dictionary<string, UDPServerHelper.RouteInfo> dic = UDPServerHelper.RouteList;
                    model.RouterCount = dic.Count;
                    if (dic.Count > 0)
                    {
                        var routers = dic.Skip((pageIndex - 1) * pageSize).Take(pageSize * pageIndex);
                        foreach (var item in routers)
                        {
                            RouterInfo router = UDPServer.RouterList.FirstOrDefault(r => r.RouterToken == item.Key);
                            if (router != null)
                            {
                                router.RouterToken = item.Key;
                                router.Online = item.Value.IsOnline ? "在线" : "离线";
                                IPEndPoint remotePoint = (IPEndPoint)item.Value.RemotePoint;
                                router.IP = remotePoint.Address.ToString();
                                router.Port = remotePoint.Port;

                                model.RouterList.Add(router);
                            }
                        }
                    }
                    break;
                case 2:
                    model.RouterDevices = GetDeviceList(UDPServer.CoinDeviceList.Where(t => t.RouterToken == routerToken).ToList());
                    model.RouterGroups = UDPServer.GroupList.Where(g => g.RouterToken == routerToken).ToList();
                    break;
                case 3:
                    model.RouterDevices = GetDeviceList(UDPServer.TerminalList.Where(t => t.GroupId == groupId).ToList());
                    break;
            }
        } 

        private List<DeviceModel> GetDeviceList(List<DeviceModel> currList)
        {
            List<DeviceModel> list = new List<DeviceModel>();
            List<HeadInfo.机头信息> headList = HeadInfo.GetAllHead();
            foreach (var item in currList)
            {
                HeadInfo.机头信息 head = headList.FirstOrDefault(t => t.令牌 == item.DeviceToken);
                DeviceModel info = new DeviceModel();
                info.RouterToken = item.RouterToken;
                info.DeviceId = item.DeviceId;
                info.DeviceToken = item.DeviceToken;
                info.DeviceName = item.DeviceName;
                info.DeviceType = item.DeviceType;
                info.State = UDPServer.GetDeviceState(head);
                info.SN = item.SN;
                info.HeadAddress = item.HeadAddress;
                list.Add(info);
            }
            return list;
        }
        #endregion

        #region 控制器及指令数据
        public class PushRouterInstModel : ResultModel
        {
            public PushRouterInstModel()
            {
                this.Instructions = new InstructionsModel();
                this.RouterList = new List<RouterInfo>();
            }

            /// <summary>
            /// 发送长度
            /// </summary>
            public DataLength SendLength { get; set; }

            /// <summary>
            /// 接收长度
            /// </summary>
            public DataLength RecvLength { get; set; }

            public List<DataLength> SendLengthList { get; set; }

            public List<DataLength> RecvLengthList { get; set; }

            /// <summary>
            /// 每分钟指令数
            /// </summary>
            public int PerMinuteCount { get; set; }

            public List<InstMessage> Messages { get; set; }

            public InstructionsModel Instructions { get; set; }

            public CurrVar CurrVar { get; set; }

            public List<RouterInfo> RouterList { get; set; }

            public int RouterCount { get; set; }

            public List<DeviceModel> RouterDevices { get; set; }

            public List<GroupInfo> RouterGroups { get; set; }
        }

        public class CurrVar
        {
            public int Level { get; set; }

            public string CurrRouterToken { get; set; }

            public string CurrRouterName { get; set; }

            public int CurrGroupId { get; set; }

            public string CurrGroupName { get; set; }
        }

        public class DataLength
        {
            public long y { get; set; }

            public string name { get; set; }
        }

        public class InstMessage
        {
            public string MsgContent { get; set; }
        }
        #endregion

        #region 指令
        public class InstructionsModel
        {
            public InstructionsModel()
            {
                Total = Querys = Coins = ICCardCoinRepeats = ICCardQueryRepeats = Receipts = Errors = Returns = 0;
            }
            /// <summary>
            /// 总指令数
            /// </summary>
            public int Total { get; set; }

            /// <summary>
            /// 查询指令数
            /// </summary>
            public int Querys { get; set; }

            /// <summary>
            /// 币业务指令数
            /// </summary>
            public int Coins { get; set; }

            /// <summary>
            /// IC卡查询重复指令数
            /// </summary>
            public int ICCardQueryRepeats { get; set; }

            /// <summary>
            /// IC卡进出币指令重复数
            /// </summary>
            public int ICCardCoinRepeats { get; set; }

            /// <summary>
            /// 小票指令数
            /// </summary>
            public int Receipts { get; set; }

            /// <summary>
            /// 错误指令数
            /// </summary>
            public int Errors { get; set; }

            /// <summary>
            /// 返还指令数
            /// </summary>
            public int Returns { get; set; }
        } 
        #endregion

        public class Connect
        {
            public string ClientId { get; set; }

            public string ConnectionId { get; set; }
        }
    }
}