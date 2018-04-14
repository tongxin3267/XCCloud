using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;
using XCCloudService.Business.Common;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.Socket.UDP;
using XCCloudService.SocketService.TCP.Business;
using XCCloudService.SocketService.TCP.Client;
using XCCloudService.SocketService.TCP.Common;
using XCCloudService.SocketService.TCP.Model;
using XCCloudService.SocketService.UDP;
using XCCloudService.SocketService.UDP.Factory;
using XCCloudService.SocketService.UDP.Security;



namespace XCCloudService.Test
{
    /// <summary>
    /// SocketMsg 的摘要说明
    /// </summary>
    public class SocketTest : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //string str = "fewfewli测试数据";
            //byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            ////XCCloudWebSocketService.Server.Send("192.168.1.110", 12882, bytes);

            string acton = context.Request["method"];
            switch (acton)
            {
                case "routeRegister": routeRegister(context); break;
                case "routeRegisterUDP": routeRegisterUDP(context); break;
                case "webClientRegister": webClientRegister(context); break;
                case "deviceState": deviceState(context); break;
                case "radarHeartbeat": radarHeartbeat(context); break;
                case "deviceControl": deviceControl(context); break;
                case "getRadarToken": getRadarToken(context); break;
                case "serverReceiveRadarResponse": serverReceiveRadarResponse(context); break;
                case "radarNotify": radarNotify(context); break;
                case "getMemberInfo": getMemberInfo(context); break;
            }
        }

        private void getMemberInfo(HttpContext context)
        {
            MemberQueryResultModel resultModel = new MemberQueryResultModel();
            resultModel.StoreId = "100000";
            resultModel.StoreName = "武汉莘宸电玩";
            resultModel.ICCardID = "90000011";
            resultModel.MemberLevelName = "李俊杰";
            resultModel.Gender = "1";
            resultModel.Birthday = "2017-09-08";
            resultModel.CertificalID = "420300197610102511";
            resultModel.Mobile = "15618920029";
            resultModel.Balance = 10;
            resultModel.Point = 100;
            resultModel.Deposit = 100;
            resultModel.Lottery = 100;
            resultModel.MemberState = "1";
            resultModel.Note = "100";
            resultModel.MemberLevelName = "普通会员";
            resultModel.EndDate = "2027-09-09";

            MemberQueryResultNotifyRequestModel requestModel = new MemberQueryResultNotifyRequestModel();
            requestModel.Result_Code = "1";
            requestModel.Result_Msg = "";
            requestModel.SignKey = "";
            requestModel.Result_Data = resultModel;

            ClientService service = new ClientService();
            service.Connection();

            byte[] data = DataFactory.CreateRequesProtocolData(TransmiteEnum.远程门店会员卡数据请求响应, requestModel);
            service.Send(data);
        }

        private void radarNotify(HttpContext context)
        {
            string errMsg = string.Empty;
            string token = context.Request["token"];
            string action = context.Request["action"];
            string result = context.Request["result"];
            string orderid = context.Request["orderid"];
            string sn = context.Request["sn"];
            string signkey = context.Request["signkey"];

            ClientService service = new ClientService();
            service.Connection();
            RadarNotifyRequestModel dataModel = new RadarNotifyRequestModel();
            dataModel.Token = token;
            dataModel.SN = sn;
            dataModel.Result = result;
            dataModel.OrderId = orderid;
            dataModel.SignKey = "";
            dataModel.Action = "1";
            dataModel.Coins = "5";
            byte[] data = DataFactory.CreateRequesProtocolData(TransmiteEnum.雷达通知指令, dataModel);
            service.Send(data);

            RadarSendObject sendObj = new RadarSendObject("100010", "0001");
            //手机号码构建TCP接受对象
            MemberReceiveObject receiveObj = new MemberReceiveObject("15618920033");
            RadarAnswerObj radarAnswerObj = new RadarAnswerObj("1", ((int)(TCPAnswerMessageType.出币)).ToString(), "成功出币5");

            SocketDataModel<RadarSendObject, MemberReceiveObject, RadarAnswerObj> socketDataModel = new SocketDataModel<RadarSendObject, MemberReceiveObject, RadarAnswerObj>("3", "0", sendObj, receiveObj, radarAnswerObj);
            //string str = Utils.DataContractJsonSerializer(socketDataModel);

            var dataObj = new {
                answerMsgType = "1",
                answerOjb = new { 
                    result_code = "1",
                    answerMsg = "成功出币10个" 
                }
            };

            TCPServiceBusiness.Send("15618920033", dataObj);
    
        }

        private void getRadarToken(HttpContext context)
        { 
            //string radarToken = context.Request["radarToken"];
            //string mcuid = "20171030100001";
            //string action = "1";
            //int count = 50;
            //string sn = "10000000100101001";
            //string ip = string.Empty;
            //string orderId = "N000000001";
            //int port = 0;
            //if (DataFactory.GetRadarClient(radarToken, out ip, out port))
            //{
            //    DeviceControlRequestDataModel dataModel = new DeviceControlRequestDataModel(storeId,segment, mcuid, action, count, sn, orderId, "");
                
            //    byte[] data = DataFactory.CreateRequesProtocolData(RequestTransmiteEnum.远程设备控制指令, dataModel);
            //}
        }

        private void routeRegister(HttpContext context)
        {
            string errMsg = string.Empty;
            string dbName = string.Empty;
            string password = string.Empty;
            string storeId = context.Request["storeId"];
            string segment = context.Request["segment"];
            string token = context.Request["token"];

            //设置路由设备token
            string routeDeivceToken = XCGameRadarDeviceTokenBusiness.SetRadarDeviceToken(storeId, segment);
            context.Response.Write(routeDeivceToken);
        }

        public void routeRegisterUDP(HttpContext context)
        {
            string storeId = context.Request["storeId"];
            string segment = context.Request["segment"];
            string signkey = context.Request["signkey"];

            ClientService service = new ClientService();
            service.Connection();
            RadarRegisterRequestDataModel dataModel = new RadarRegisterRequestDataModel(storeId, segment);
            byte[] data = DataFactory.CreateRequesProtocolData(TransmiteEnum.雷达注册授权, dataModel);
            service.Send(data);
        }

        public void radarHeartbeat(HttpContext context)
        {
            string token = context.Request["token"];
            ClientService service = new ClientService();
            service.Connection();
            RadarHeartbeatRequestDataModel dataModel = new RadarHeartbeatRequestDataModel(token,"");
            byte[] data = DataFactory.CreateRequesProtocolData(TransmiteEnum.雷达心跳, dataModel);
            service.Send(data);


        }

        public void serverReceiveRadarResponse(HttpContext context)
        {
            string result_code = context.Request["result_code"];
            string result_msg = context.Request["result_msg"];
            string sn = context.Request["sn"];
            string signkey = context.Request["signkey"];

            ClientService service = new ClientService();
            service.Connection();
            DeviceControlResponseModel dataModel = new DeviceControlResponseModel(result_code, result_msg, sn, signkey);
            byte[] data = DataFactory.CreateRequesProtocolData(TransmiteEnum.远程设备控制指令响应, dataModel);
            service.Send(data);
        }

        public void deviceControl(HttpContext context)
        {
            string token = context.Request["token"];
            string mcuid = context.Request["mcuid"];
            string action = context.Request["action"];
            string count = context.Request["count"];
            string sn = context.Request["sn"];
            string orderId = context.Request["orderId"];
            string signkey = context.Request["signkey"];

            //ClientService service = new ClientService();
            //service.Connection();
            //DeviceControlRequestDataModel dataModel = new DeviceControlRequestDataModel(token, mcuid, action, int.Parse(count), sn,orderId, signkey);
            //byte[] data = DataFactory.CreateRequesProtocolData(RequestTransmiteEnum.远程设备控制指令, dataModel);
            //service.Send(data);
        }

        private void webClientRegister(HttpContext context)
        {
            string storeId = context.Request["storeId"] ?? "";
            string mobile = context.Request["mobile"] ?? "";
            ClientSerivce socketService = new ClientSerivce();
            if (socketService.ConnectSocket())
            {
                //客户端注册
                //WebClientRegisterModel webClientRegisterModel = new WebClientRegisterModel(storeId, mobile);
                //var registerObj = SocketFactory<WebClientRegisterModel>.GetRetisterObject("sysId", "versionNo", SocketClientType.WebClient,SocketServiceType.ClientRegister,webClientRegisterModel);
                //socketService.Register(JsonHelper.DataContractJsonSerializer(registerObj));
            }
        }

        public void deviceState(HttpContext context)
        {
            string token = context.Request["token"] ?? "";
            string mcuid = context.Request["mcuid"] ?? "";
            string status = context.Request["status"] ?? "";
            string signkey = context.Request["signkey"] ?? "";

            ClientService service = new ClientService();
            service.Connection();
            DeviceStateRequestDataModel dataModel = new DeviceStateRequestDataModel(token,mcuid,status,"",signkey);
            byte[] data = DataFactory.CreateRequesProtocolData(TransmiteEnum.设备状态变更通知, dataModel);
            service.Send(data);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}