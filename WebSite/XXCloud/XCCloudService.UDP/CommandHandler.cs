using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Business.Common;
using XCCloudService.Business.WeiXin;
using XCCloudService.Business.XCGame;
using XCCloudService.Business.XCGameMana;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.CustomModel.Common;
using XCCloudService.Model.CustomModel.XCGame;
using XCCloudService.Model.Socket.UDP;
using XCCloudService.Model.WeiXin.SAppMessage;
using XCCloudService.SocketService.TCP.Business;
using XCCloudService.SocketService.TCP.HubService;
using XCCloudService.SocketService.UDP.Common;
using XCCloudService.SocketService.UDP.Factory;
using XCCloudService.SocketService.UDP.Security;
using XCCloudService.WeiXin.Message;

namespace XCCloudService.SocketService.UDP
{
    public class CommandHandler
    {
        public static void RadarRegister(string requestDataJson, UDPClientItemBusiness.ClientItem item)
        {
            //通知服务器上线 
            object outModel = null;
            object requestModel = null;
            bool bRegister = false;

            DataFactory.CreateResponseProtocolData(TransmiteEnum.雷达注册授权, requestDataJson, ref requestModel, ref outModel);
            RadarRegisterOutParamsModel parmasModel = (RadarRegisterOutParamsModel)outModel;
            if (!string.IsNullOrEmpty(parmasModel.Token))
            {
                bRegister = true;
                item.gID = parmasModel.Token;
                item.StoreID = parmasModel.StoreId;
                item.Segment = parmasModel.Segment;
                ClientList.UpdateClient(item);                
            }
            Send(((IPEndPoint)item.remotePoint).Address.ToString(), ((IPEndPoint)item.remotePoint).Port, parmasModel.ResponsePackages);
            string logTxt = "[接收：" + requestDataJson + "]" + "[响应：" + Utils.SerializeObject(parmasModel) + "]" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            UDPLogHelper.SaveRadarRegisterLog(((IPEndPoint)item.remotePoint).Address.ToString(), ((IPEndPoint)item.remotePoint).Port, parmasModel.StoreId, parmasModel.Segment, parmasModel.Token, bRegister, requestDataJson, parmasModel.ResponseJson, logTxt);
            string message = "[接收：" + requestDataJson + "]" + "[响应：" + parmasModel.ResponseJson + "]";
            SignalrServerToClient.BroadcastMessageByRadarRegister("雷达注册授权", item.StoreID,item.Segment, message,System.DateTime.Now);
        }

        public static void DeviceStateChange(string requestDataJson, UDPClientItemBusiness.ClientItem item)
        {
            object outModel = null;
            object requestModel = null;
            DataFactory.CreateResponseProtocolData(TransmiteEnum.设备状态变更通知, requestDataJson,ref requestModel, ref outModel);
            DeviceStateOutParamsModel responseOutModel = (DeviceStateOutParamsModel)outModel;
            Send(((IPEndPoint)item.remotePoint).Address.ToString(), ((IPEndPoint)item.remotePoint).Port, ((DeviceStateOutParamsModel)outModel).ResponsePackages);
            DeviceStateRequestDataModel requestDataModel = (DeviceStateRequestDataModel)requestModel;

            //记录心跳日志
            bool bChangeSuccess = false;
            if (responseOutModel.ResponseModel.GetType().Name.Equals("ComonErrorResponseModel"))
            {
                ComonErrorResponseModel model = (ComonErrorResponseModel)(responseOutModel.ResponseModel);
                bChangeSuccess = model.Result_Code == "1" ? true : false;
            }
            else if (responseOutModel.ResponseModel.GetType().Name.Equals("ComonSuccessResponseModel"))
            {
                ComonSuccessResponseModel model = (ComonSuccessResponseModel)(responseOutModel.ResponseModel);
                bChangeSuccess = model.Result_Code == "1" ? true : false;
            }
            string logTxt = "[接收：" + requestDataJson + "]" + "[响应：" + Utils.SerializeObject(outModel) + "]" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            UDPLogHelper.SaveDeviceStateChangeLog(((IPEndPoint)item.remotePoint).Address.ToString(), ((IPEndPoint)item.remotePoint).Port, requestDataModel.StoreId, requestDataModel.Token, requestDataModel.MCUId, requestDataModel.Status, bChangeSuccess, requestDataJson, responseOutModel.ResponseJson, logTxt);
            string message = "[接收：" + requestDataJson + "]" + "[响应：" + responseOutModel.ResponseJson + "]";
            SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.设备状态变更通知), "设备状态变更通知", requestDataModel.Token, message,System.DateTime.Now);
        }

        public static void RadarHeat(string requestDataJson, UDPClientItemBusiness.ClientItem item)
        {
            object outModel = null;
            object requestModel = null;
            DataFactory.CreateResponseProtocolData(TransmiteEnum.雷达心跳, requestDataJson,ref requestModel ,ref outModel);
            RadarHeartbeatOutParamsModel outDataModel = (RadarHeartbeatOutParamsModel)outModel;
            Send(((IPEndPoint)item.remotePoint).Address.ToString(), ((IPEndPoint)item.remotePoint).Port, ((RadarHeartbeatOutParamsModel)outModel).ResponsePackages);
            RadarHeartbeatRequestDataModel requestDataModel = (RadarHeartbeatRequestDataModel)requestModel;
            
            //记录心跳日志
            bool bHeadSuccess = false;
            if(outDataModel.ResponseModel.GetType().Name.Equals("ComonErrorResponseModel"))
            {
                ComonErrorResponseModel model = (ComonErrorResponseModel)(outDataModel.ResponseModel);
                bHeadSuccess = model.Result_Code == "1" ? true : false;
            }
            else if (outDataModel.ResponseModel.GetType().Name.Equals("ComonSuccessResponseModel"))
            { 
                ComonSuccessResponseModel model = (ComonSuccessResponseModel)(outDataModel.ResponseModel);
                bHeadSuccess = model.Result_Code == "1" ? true : false;
            }
            string logTxt = "[接收：" + requestDataJson + "]" + "[响应：" + Utils.SerializeObject(outModel) + "]" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            UDPLogHelper.SaveRadarHeatLog(((IPEndPoint)item.remotePoint).Address.ToString(), ((IPEndPoint)item.remotePoint).Port, requestDataModel.StoreId, requestDataModel.Segment, requestDataModel.Token, bHeadSuccess, requestDataJson, outDataModel.ResponseJson, logTxt);
            ClientList.UpdateClientHeatTime(requestDataModel.Token);
            string message = "[接收：" + requestDataJson + "]" + "[响应：" + outDataModel.ResponseJson + "]";
            SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.雷达心跳), "雷达心跳", requestDataModel.Token, message, System.DateTime.Now);
        }

        public static void DeviceControl(string requestDataJson, UDPClientItemBusiness.ClientItem item)
        {
            object outModel = null;
            object requestModel = null;
            DataFactory.CreateResponseProtocolData(TransmiteEnum.远程设备控制指令, requestDataJson, ref requestModel,ref outModel);
            DeviceControlAnswerRequestDataModel requestDataModel = (DeviceControlAnswerRequestDataModel)requestModel;
            DeviceControlOutParmasModel responseDataModel = (DeviceControlOutParmasModel)outModel;
            UDPSocketAnswerModel answerModel = null;
            string orderId = string.Empty;
            bool bSuccess = GetResponseModelResultCode(responseDataModel.ResponseModel);
            answerModel = UDPSocketAnswerBusiness.GetAnswerModel(requestDataModel.SN);
            if (requestDataModel.Result_Code == "1" && bSuccess && UDPSocketAnswerBusiness.ExistSN(requestDataModel.SN))
            {
                //如果应答结果正确，清除应答缓存,添加对手机号的接口调用锁定
                UDPSocketAnswerBusiness.Remove(requestDataModel.SN);
            }
            string logTxt = "[接收：" + requestDataJson + "]" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string radarToken = string.Empty;
            if (answerModel != null && XCGameRadarDeviceTokenBusiness.GetRouteDeviceToken(requestDataModel.StoreId, answerModel.Segment, out radarToken))
            { 
                
            }
            UDPLogHelper.SaveUDPDeviceControlLog(requestDataModel.StoreId, requestDataModel.OrderId, ((IPEndPoint)item.remotePoint).Address.ToString(), ((IPEndPoint)item.remotePoint).Port, requestDataModel.SN, requestDataJson, responseDataModel.ResponseJson, bSuccess, logTxt);
            string message = "[接收：" + requestDataJson + "]" + "[响应：" + responseDataModel.ResponseJson + "]";
            SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.雷达心跳), "远程设备控制指令响应", radarToken, message,System.DateTime.Now);
        }

        public static void RadarNotify(string requestDataJson, UDPClientItemBusiness.ClientItem item)
        {
            object outModel = null;
            object requestModel = null;

            DataFactory.CreateResponseProtocolData(TransmiteEnum.雷达通知指令, requestDataJson, ref requestModel,ref outModel);
            RadarNotifyRequestModel rnrModel = Utils.DataContractJsonDeserializer<RadarNotifyRequestModel>(requestDataJson);
            RadarNotifyOutParamsModel responseOutModel = (RadarNotifyOutParamsModel)outModel;
            XCGameRadarDeviceTokenModel radarTokenModel = XCGameRadarDeviceTokenBusiness.GetRadarDeviceTokenModel(rnrModel.Token);
            Send(((IPEndPoint)item.remotePoint).Address.ToString(), ((IPEndPoint)item.remotePoint).Port, ((RadarNotifyOutParamsModel)outModel).ResponsePackages);
            RadarNotifyRequestModel requestDataModel = (RadarNotifyRequestModel)requestModel;
            
            //验证相应模式
            bool bCoinSuccess = false;
            if (responseOutModel.ResponseModel.GetType().Name.Equals("ComonErrorResponseModel"))
            {
                ComonErrorResponseModel model = (ComonErrorResponseModel)(responseOutModel.ResponseModel);
                bCoinSuccess = model.Result_Code == "1" ? true : false;
            }
            else if (responseOutModel.ResponseModel.GetType().Name.Equals("ComonSuccessResponseModel"))
            {
                ComonSuccessResponseModel model = (ComonSuccessResponseModel)(responseOutModel.ResponseModel);
                bCoinSuccess = model.Result_Code == "1" ? true : false;
            }

            //记录日志
            string logTxt = "[接收：" + requestDataJson + "]" + "[响应：" + responseOutModel.ResponseJson + "]" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            UDPLogHelper.SaveUDPRadarNotifyLog(requestDataModel.StoreId, requestDataModel.OrderId, requestDataModel.Token, requestDataModel.SN, int.Parse(requestDataModel.Coins), int.Parse(requestDataModel.Action), requestDataModel.Result, ((IPEndPoint)item.remotePoint).Address.ToString(), ((IPEndPoint)item.remotePoint).Port, requestDataJson, responseOutModel.ResponseJson, bCoinSuccess, logTxt);
            string message = "[接收：" + requestDataJson + "]" + "[响应：" + responseOutModel.ResponseJson + "]";
            SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.雷达通知指令), "雷达通知指令", requestDataModel.Token, message, System.DateTime.Now);
            if (rnrModel.Result == "成功" && bCoinSuccess)
            {
                //出币后向客户端发送成功信息
                try
                {
                    TCPAnswerOrderModel taoModel =null;
                    //如果订单缓存信息存在
                    if (MPOrderBusiness.ExistTCPAnswerOrder(rnrModel.OrderId, ref taoModel))
                    {
                        string coinType = CoinBusiess.GetCoinOpetionName(taoModel.Action);
                        string notifyMsg = string.Empty;
                        string answerMsgType = string.Empty;
                        string errMsg = string.Empty;
                        SAppPushMessageCacheModel msgModel = null;
   
                        if (taoModel.Action == "1")
                        {
                            notifyMsg = "成功出币" + taoModel.Coins.ToString() + "个";
                            answerMsgType = ((int)(TCPAnswerMessageType.出币)).ToString();
                            if (SAppPushMessageBusiness.GetModel(rnrModel.OrderId, ref msgModel))
                            {
                                if (msgModel.SAppMessageType == SAppMessageType.MemberCoinsOperationNotify)
                                {
                                    MemberCoinsOperationNotifyDataModel dataModel = (MemberCoinsOperationNotifyDataModel)(msgModel.DataObj);
                                    SAppMessageMana.PushMemberCoinsMsg(msgModel.OpenId,msgModel.SAppAccessToken,"提币",dataModel.StoreName,dataModel.Mobile, dataModel.ICCardId, int.Parse(requestDataModel.Coins), dataModel.LastBalance, msgModel.FormId, "", out errMsg);
                                }
                                else if (msgModel.SAppMessageType == SAppMessageType.MemberFoodSaleNotify)
                                {
                                    MemberFoodSaleNotifyDataModel dataModel = (MemberFoodSaleNotifyDataModel)(msgModel.DataObj);
                                    SAppMessageMana.PushMemberFoodSaleMsg(msgModel.OpenId,msgModel.SAppAccessToken,"购币", dataModel.StoreName,msgModel.Mobile, msgModel.OrderId, dataModel.FoodName, dataModel.FoodNum, dataModel.ICCardId, dataModel.Money, dataModel.Coins, msgModel.FormId, out errMsg);
                                }
                            }
                        }
                        else if (taoModel.Action == "2")
                        {
                            notifyMsg = "成功存币" + requestDataModel.Coins.ToString() + "个";
                            answerMsgType = ((int)(TCPAnswerMessageType.存币)).ToString();
                            IconOutLockBusiness.RemoveByNoTimeList(taoModel.Mobile);
                            if (int.Parse(requestDataModel.Coins) > 0)
                            {
                                try
                                {
                                    int lastBalance = 0;
                                    string storeName = string.Empty;
                                    string mobile = string.Empty;
                                    if (MemberPreservationBusiness.PreservationBusiness(int.Parse(taoModel.ICCardId), int.Parse(taoModel.StoreId), int.Parse(requestDataModel.Coins), out lastBalance, out storeName, out mobile))
                                    {
                                        if (SAppPushMessageBusiness.GetModel(rnrModel.OrderId, ref msgModel))
                                        {
                                            MemberCoinsOperationNotifyDataModel dataModel = (MemberCoinsOperationNotifyDataModel)(msgModel.DataObj);
                                            SAppMessageMana.PushMemberCoinsMsg(msgModel.OpenId, msgModel.SAppAccessToken, "存币", dataModel.StoreName, dataModel.Mobile, dataModel.ICCardId, int.Parse(requestDataModel.Coins), dataModel.LastBalance, msgModel.FormId, "", out errMsg);
                                        }
                                    }
                                }
                                catch(Exception e)
                                {
                                    LogHelper.SaveLog(TxtLogType.Api, TxtLogContentType.Debug, TxtLogFileType.Day, "MemberPreservationBusiness.PreservationBusiness:" + e.Message);
                                }
                            }
                        }
                        else if (taoModel.Action == "6")//投币
                        {
                            MemberCoinsOperationNotifyDataModel dataModel = (MemberCoinsOperationNotifyDataModel)(msgModel.DataObj);
                            SAppMessageMana.PushMemberCoinsMsg(msgModel.OpenId, msgModel.SAppAccessToken, "投币", dataModel.StoreName, dataModel.Mobile, dataModel.ICCardId, int.Parse(requestDataModel.Coins), dataModel.LastBalance, msgModel.FormId, "", out errMsg);
                        }
                        else if (taoModel.Action == "7")//退币
                        {
                            MemberCoinsOperationNotifyDataModel dataModel = (MemberCoinsOperationNotifyDataModel)(msgModel.DataObj);
                            SAppMessageMana.PushMemberCoinsMsg(msgModel.OpenId, msgModel.SAppAccessToken, "退币", dataModel.StoreName, dataModel.Mobile, dataModel.ICCardId, int.Parse(requestDataModel.Coins), dataModel.LastBalance, msgModel.FormId, "", out errMsg);
                        }

                        var dataObj = new {
                            result_code = "1",
                            answerMsg = notifyMsg, 
                            answerMsgType = answerMsgType  
                        };
                        MPOrderBusiness.RemoveTCPAnswerOrder(rnrModel.OrderId);
                        TCPServiceBusiness.Send(taoModel.Mobile, dataObj);                                  
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.SaveLog(TxtLogType.UPDService, TxtLogContentType.Exception, TxtLogFileType.Day, "Exception:" + rnrModel.OrderId + Utils.GetException(ex) + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                }
            }
        }

        private static void Send(string IP, int Port, byte[] data)
        {
            try
            {
                IPEndPoint clients = new IPEndPoint(IPAddress.Parse(IP), Port);
                EndPoint epSender = (EndPoint)clients;
                Server.SocketServer.BeginSendTo(data, 0, data.Length, SocketFlags.None, epSender, new AsyncCallback(Server.SendCallBack), Server.SocketServer);
            }
            catch
            {
                throw;
            }
        }

        private static bool GetResponseModelResultCode(object responseModel)
        {
            bool bSuccess = false;
            if (responseModel.GetType().Name.Equals("ComonErrorResponseModel"))
            {
                ComonErrorResponseModel model = (ComonErrorResponseModel)(responseModel);
                bSuccess = model.Result_Code == "1" ? true : false;
            }
            else if (responseModel.GetType().Name.Equals("ComonSuccessResponseModel"))
            {
                ComonSuccessResponseModel model = (ComonSuccessResponseModel)(responseModel);
                bSuccess = model.Result_Code == "1" ? true : false;
            }
            return bSuccess;
        }


        public static void StoreQuery(string requestDataJson, UDPClientItemBusiness.ClientItem item)
        {
            object outModel = null;
            object requestModel = null;
            DataFactory.CreateResponseProtocolData(TransmiteEnum.远程门店账目查询指令, requestDataJson, ref requestModel, ref outModel);
            StoreQueryResponseModel requestDataModel = (StoreQueryResponseModel)requestModel;
            StoreQueryOutParamsModel responseOutModel = (StoreQueryOutParamsModel)outModel;
            
            //验证相应模式
            bool bSuccess = false;
            if (responseOutModel.ResponseModel.GetType().Name.Equals("ComonErrorResponseModel"))
            {
                ComonErrorResponseModel model = (ComonErrorResponseModel)(responseOutModel.ResponseModel);
                bSuccess = model.Result_Code == "1" ? true : false;
            }
            else if (responseOutModel.ResponseModel.GetType().Name.Equals("ComonSuccessResponseModel"))
            {
                ComonSuccessResponseModel model = (ComonSuccessResponseModel)(responseOutModel.ResponseModel);
                bSuccess = model.Result_Code == "1" ? true : false;
            }
            else
            {
                StoreQueryResponseModel responseDataModel = (StoreQueryResponseModel)(responseOutModel.ResponseModel);
                bSuccess = responseDataModel.Result_Code == "1" ? true : false;
            }

            UDPSocketStoreQueryAnswerModel answerModel = UDPSocketStoreQueryAnswerBusiness.GetAnswerModel(requestDataModel.SN);
            if (bSuccess && answerModel != null)
            {
                //如果应答结果正确，修改状态
                answerModel.Status = 1;
            }
            string message = "[接收：" + requestDataJson + "]" + "[响应：" + responseOutModel.ResponseJson + "]";
            SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店账目查询指令), "远程门店账目查询指令响应", answerModel.RadarToken, message);
        }


        public static void StoreQueryNotify(string requestDataJson, UDPClientItemBusiness.ClientItem item, int packId, int packNum)
        {
            object outModel = null;
            object requestModel = null;

            DataFactory.CreateResponseProtocolData(TransmiteEnum.远程门店账目应答通知指令, requestDataJson, ref requestModel, ref outModel,packId);
            StoreQueryResultNotifyRequestModel rnrModel = (StoreQueryResultNotifyRequestModel)(requestModel);
            StoreQueryNotifyOutParamsModel responseOutModel = (StoreQueryNotifyOutParamsModel)outModel;
            Send(((IPEndPoint)item.remotePoint).Address.ToString(), ((IPEndPoint)item.remotePoint).Port, ((StoreQueryNotifyOutParamsModel)outModel).ResponsePackages);
            StoreQueryResultNotifyResponseModel responseDataModel = (StoreQueryResultNotifyResponseModel)(responseOutModel.ResponseModel);

            UDPSocketStoreQueryAnswerModel answerModel = UDPSocketStoreQueryAnswerBusiness.GetAnswerModel(rnrModel.SN, 1);
            if (responseDataModel != null && responseDataModel.Result_Code == "1" && answerModel != null)
            {
                //如果应答结果正确，清除应答缓存
                //if (packId == packNum)
                //{
                answerModel.Status = 2;
                answerModel.Result = rnrModel.TDList;
                //} 
            }
            SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店账目应答通知指令), "远程门店账目应答通知指令", answerModel.RadarToken, requestDataJson);
        }

        public static void MemberQueryNotify(string requestDataJson, UDPClientItemBusiness.ClientItem item)
        {
            string routeDeivceToken = string.Empty;
            string password = string.Empty;
            string errMsg = string.Empty;
            //获取雷达通知请求数据模式
            MemberQueryResultNotifyRequestModel requestDataModel = JsonHelper.DataContractJsonDeserializer<MemberQueryResultNotifyRequestModel>(requestDataJson);
            UDPSocketCommonQueryAnswerModel asnwerModel = UDPSocketCommonQueryAnswerBusiness.GetAnswerModel(requestDataModel.SN);

            //验证MD5
            if (!SignKeyHelper.CheckSignKey(requestDataModel, asnwerModel.StorePassword))
            {
                errMsg = "签名不正确";
                return;
            }

            asnwerModel.Status = 1;
            asnwerModel.Result = requestDataModel;

            SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店会员卡数据请求响应), "远程门店会员卡数据请求响应", asnwerModel.RadarToken, requestDataJson);
        }

        public static void TicketQueryNotify(string requestDataJson, UDPClientItemBusiness.ClientItem item)
        {
            string routeDeivceToken = string.Empty;
            string password = string.Empty;
            string errMsg = string.Empty;

            //获取雷达通知请求数据模式
            TicketQueryResultNotifyRequestModel requestDataModel = JsonHelper.DataContractJsonDeserializer<TicketQueryResultNotifyRequestModel>(requestDataJson);
            UDPSocketCommonQueryAnswerModel asnwerModel = UDPSocketCommonQueryAnswerBusiness.GetAnswerModel(requestDataModel.SN);

            //验证MD5
            if (!SignKeyHelper.CheckSignKey(requestDataModel, asnwerModel.StorePassword))
            {
                errMsg = "签名不正确";
                return;
            }

            asnwerModel.Status = 1;
            asnwerModel.Result = requestDataModel;

            SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店门票数据请求响应), "远程门店门票数据请求响应", asnwerModel.RadarToken, requestDataJson);
        }

        public static void TicketOperateNotify(string requestDataJson, UDPClientItemBusiness.ClientItem item)
        {
            string routeDeivceToken = string.Empty;
            string password = string.Empty;
            string errMsg = string.Empty;

            //获取雷达通知请求数据模式
            TicketOperateResultNotifyRequestModel requestDataModel = JsonHelper.DataContractJsonDeserializer<TicketOperateResultNotifyRequestModel>(requestDataJson);
            UDPSocketCommonQueryAnswerModel asnwerModel = UDPSocketCommonQueryAnswerBusiness.GetAnswerModel(requestDataModel.SN);

            //验证MD5
            if (!SignKeyHelper.CheckSignKey(requestDataModel, asnwerModel.StorePassword))
            {
                errMsg = "签名不正确";
                return;
            }

            asnwerModel.Status = 1;
            asnwerModel.Result = requestDataModel;

            SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店门票操作请求响应), "远程门店门票操作请求响应", asnwerModel.RadarToken, requestDataJson);
        }

        public static void LotteryQueryNotify(string requestDataJson, UDPClientItemBusiness.ClientItem item)
        {
            string routeDeivceToken = string.Empty;
            string password = string.Empty;
            string errMsg = string.Empty;

            //获取雷达通知请求数据模式
            LotteryQueryResultNotifyRequestModel requestDataModel = JsonHelper.DataContractJsonDeserializer<LotteryQueryResultNotifyRequestModel>(requestDataJson);
            UDPSocketCommonQueryAnswerModel asnwerModel = UDPSocketCommonQueryAnswerBusiness.GetAnswerModel(requestDataModel.SN);

            //验证MD5
            if (!SignKeyHelper.CheckSignKey(requestDataModel, asnwerModel.StorePassword))
            {
                errMsg = "签名不正确";
                return;
            }

            asnwerModel.Status = 1;
            asnwerModel.Result = requestDataModel;

            SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店彩票数据请求响应), "远程门店彩票数据请求响应", asnwerModel.RadarToken, requestDataJson);
        }

        public static void LotteryOperateNotify(string requestDataJson, UDPClientItemBusiness.ClientItem item)
        {
            string routeDeivceToken = string.Empty;
            string password = string.Empty;
            string errMsg = string.Empty;

            //获取雷达通知请求数据模式
            LotteryOperateResultNotifyRequestModel requestDataModel = JsonHelper.DataContractJsonDeserializer<LotteryOperateResultNotifyRequestModel>(requestDataJson);
            UDPSocketCommonQueryAnswerModel asnwerModel = UDPSocketCommonQueryAnswerBusiness.GetAnswerModel(requestDataModel.SN);
            if (requestDataModel.Result_Data == null)
            {
                requestDataModel.Result_Data = "";
            }
            //验证MD5
            if (!SignKeyHelper.CheckSignKey(requestDataModel, asnwerModel.StorePassword))
            {
                errMsg = "签名不正确";
                return;
            }
   
            asnwerModel.Status = 1;
            asnwerModel.Result = requestDataModel;

            SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店彩票操作请求响应), "远程门店彩票操作请求响应", asnwerModel.RadarToken, requestDataJson);
        }

        public static void OutTicketQueryNotify(string requestDataJson, UDPClientItemBusiness.ClientItem item)
        {
            string routeDeivceToken = string.Empty;
            string password = string.Empty;
            string errMsg = string.Empty;

            //获取雷达通知请求数据模式
            OutTicketQueryResultNotifyRequestModel requestDataModel = JsonHelper.DataContractJsonDeserializer<OutTicketQueryResultNotifyRequestModel>(requestDataJson);
            UDPSocketCommonQueryAnswerModel asnwerModel = UDPSocketCommonQueryAnswerBusiness.GetAnswerModel(requestDataModel.SN);

            //验证MD5
            if (!SignKeyHelper.CheckSignKey(requestDataModel, asnwerModel.StorePassword))
            {
                errMsg = "签名不正确";
                return;
            }

            asnwerModel.Status = 1;
            asnwerModel.Result = requestDataModel;

            SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店出票条码数据请求响应), "远程门店出票条码数据请求响应", asnwerModel.RadarToken, requestDataJson);
        }

        public static void OutTicketOperateNotify(string requestDataJson, UDPClientItemBusiness.ClientItem item)
        {
            string errMsg = string.Empty;

            //获取雷达通知请求数据模式
            OutTicketOperateResultNotifyRequestModel requestDataModel = JsonHelper.DataContractJsonDeserializer<OutTicketOperateResultNotifyRequestModel>(requestDataJson);
            UDPSocketCommonQueryAnswerModel asnwerModel = UDPSocketCommonQueryAnswerBusiness.GetAnswerModel(requestDataModel.SN);
            if (requestDataModel.Result_Data == null)
            {
                requestDataModel.Result_Data = "";
            }
            //验证MD5
            if (!SignKeyHelper.CheckSignKey(requestDataModel, asnwerModel.StorePassword))
            {
                errMsg = "签名不正确";
                return;
            }

            asnwerModel.Status = 1;
            asnwerModel.Result = requestDataModel;

            SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店出票条码操作请求响应), "远程门店出票条码操作请求响应", asnwerModel.RadarToken, requestDataJson);
        }

        public static void ParamQueryNotify(string requestDataJson, UDPClientItemBusiness.ClientItem item)
        {
            string errMsg = string.Empty;

            //获取雷达通知请求数据模式
            ParamQueryResultNotifyRequestModel requestDataModel = JsonHelper.DataContractJsonDeserializer<ParamQueryResultNotifyRequestModel>(requestDataJson);
            UDPSocketCommonQueryAnswerModel asnwerModel = UDPSocketCommonQueryAnswerBusiness.GetAnswerModel(requestDataModel.SN);

            //验证MD5
            if (!SignKeyHelper.CheckSignKey(requestDataModel, asnwerModel.StorePassword))
            {
                errMsg = "签名不正确";
                return;
            }

            asnwerModel.Status = 1;
            asnwerModel.Result = requestDataModel;

            SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店运行参数数据请求响应), "远程门店运行参数数据请求响应", asnwerModel.RadarToken, requestDataJson);
        }

        public static void UserPhoneQueryNotify(string requestDataJson, UDPClientItemBusiness.ClientItem item)
        {
            string errMsg = string.Empty;

            //获取雷达通知请求数据模式
            UserPhoneQueryResultNotifyRequestModel requestDataModel = JsonHelper.DataContractJsonDeserializer<UserPhoneQueryResultNotifyRequestModel>(requestDataJson);
            UDPSocketCommonQueryAnswerModel asnwerModel = UDPSocketCommonQueryAnswerBusiness.GetAnswerModel(requestDataModel.SN);

            //验证MD5
            if (!SignKeyHelper.CheckSignKey(requestDataModel, asnwerModel.StorePassword))
            {
                errMsg = "签名不正确";
                return;
            }

            asnwerModel.Status = 1;
            asnwerModel.Result = requestDataModel;

            SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店运行参数数据请求响应), "远程门店运行参数数据请求响应", asnwerModel.RadarToken, requestDataJson);
        }


        public static void MemberTransOperateNotify(string requestDataJson, UDPClientItemBusiness.ClientItem item)
        {
            string errMsg = string.Empty;

            //获取雷达通知请求数据模式
            MemberTransOperateResultNotifyRequestModel requestDataModel = JsonHelper.DataContractJsonDeserializer<MemberTransOperateResultNotifyRequestModel>(requestDataJson);
            UDPSocketCommonQueryAnswerModel asnwerModel = UDPSocketCommonQueryAnswerBusiness.GetAnswerModel(requestDataModel.SN);

            //验证MD5
            if (!SignKeyHelper.CheckSignKey(requestDataModel, asnwerModel.StorePassword))
            {
                errMsg = "签名不正确";
                return;
            }

            asnwerModel.Status = 1;
            asnwerModel.Result = requestDataModel;

            SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店会员转账操作请求响应), "远程门店会员转账操作请求响应", asnwerModel.RadarToken, requestDataJson);
        }

        public static void CattleMemberCardQueryNotify(string requestDataJson, UDPClientItemBusiness.ClientItem item)
        {
            string errMsg = string.Empty;

            //获取雷达通知请求数据模式
            CattleMemberCardQueryResultNotifyRequestModel requestDataModel = JsonHelper.DataContractJsonDeserializer<CattleMemberCardQueryResultNotifyRequestModel>(requestDataJson);
            UDPSocketCommonQueryAnswerModel asnwerModel = UDPSocketCommonQueryAnswerBusiness.GetAnswerModel(requestDataModel.SN);

            //验证MD5
            if (!SignKeyHelper.CheckSignKey(requestDataModel, asnwerModel.StorePassword))
            {
                errMsg = "签名不正确";
                return;
            }

            asnwerModel.Status = 1;
            asnwerModel.Result = requestDataModel;

            SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.黄牛卡信息查询请求响应), "黄牛卡信息查询请求响应", asnwerModel.RadarToken, requestDataJson);
        }

    }
}
