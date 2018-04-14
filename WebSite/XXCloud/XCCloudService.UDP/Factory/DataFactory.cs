using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Base;
using XCCloudService.Business.Common;
using XCCloudService.Business.XCGame;
using XCCloudService.Business.XCGameMana;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.CustomModel.Common;
using XCCloudService.Model.CustomModel.XCGame;
using XCCloudService.Model.CustomModel.XCGameManager;
using XCCloudService.Model.Socket.UDP;
using XCCloudService.SocketService.TCP.HubService;
using XCCloudService.SocketService.UDP.Common;
using XCCloudService.SocketService.UDP.Security;


namespace XCCloudService.SocketService.UDP.Factory
{
    public class DataFactory
    {
        private static System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();

        #region "公开方法"

            /// <summary>
            /// 获取发送字节数据
            /// </summary>
            /// <param name="transmiteEnum"></param>
            /// <param name="dataModel"></param>
            /// <returns></returns>
        public static byte[] CreateRequesProtocolData(TransmiteEnum requestTransmiteEnum, object dataModel)
            {
                byte[] data = JsonHelper.DataContractJsonSerializerToByteArray(dataModel);
                return CreateRequestProtocolData(requestTransmiteEnum, data);
            }


            /// <summary>
            /// 是否是满足协议的数据
            /// </summary>
            /// <param name="packages">数据包</param>
            /// <returns></returns>
            public static bool IsProtocolData(byte[] packages)
            {
                if (packages.Length < 11 || packages.Length > 8192)
                {
                    return false;
                }

                //验证引导
                if (!(packages[0] == 0xfe && packages[1] == 0xfe && packages[packages.Length - 1] == 0xfe && packages[packages.Length - 2] == 0xfe))
                {
                    return false;
                }

                return true;
            }

            /// <summary>
            /// 读取协议数据
            /// </summary>
            /// <param name="packages">请求的数据包</param>
            /// <param name="transmiteEnumValue">指令枚举对应整数</param>
            /// <param name="data">请求的数据内容</param>
            /// <returns></returns>
            public static void GetProtocolData(byte[] packages, out int transmiteEnumValue, out string data,out int packId,out int packNum)
            {
                //获取数据内容
                int dataLength = BitConverter.ToInt16(packages, 3);
                data = System.Text.Encoding.UTF8.GetString(packages, 8, dataLength);

                transmiteEnumValue = (int)packages[7];
                packId = (int)packages[5];
                packNum = (int)packages[6];
            }


                
            /// <summary>
            /// 创建响应协议数据
            /// </summary>
            /// <param name="requestTransmiteEnum">请求指令</param>
            /// <param name="requestData">请求的数据</param>
            /// <param name="responsePackages">响应数据包</param>
            /// <param name="clientId"></param>
            /// <param name="cType"></param>
            public static void CreateResponseProtocolData(TransmiteEnum requestTransmiteEnum, string requestData, ref object requestObj,ref object outObj,int packId = 0)
            {
                //clientId = string.Empty;
                //cType = ClientType.服务中心;
                switch (requestTransmiteEnum)
                {
                    case TransmiteEnum.雷达注册授权: CreateRadarResponseProtocolData(TransmiteEnum.雷达注册授权响应, requestData, ref outObj); break;
                    case TransmiteEnum.设备状态变更通知: CreateDeviceStateResponseProtocolData(TransmiteEnum.设备状态变更通知响应, requestData, ref requestObj,ref outObj); break;
                    case TransmiteEnum.雷达心跳: CreateRadarHeartbeatResponseProtocolData(TransmiteEnum.雷达心跳响应, requestData, ref requestObj, ref outObj); break;
                    case TransmiteEnum.雷达通知指令: CreateRadarNotifyResponseProtocolData(TransmiteEnum.雷达通知指令响应, requestData,ref requestObj, ref outObj); break;
                    case TransmiteEnum.远程设备控制指令: CreateDeviceControlResponseProtocolData(TransmiteEnum.远程设备控制指令响应, requestData, ref requestObj, ref outObj); break;
                    case TransmiteEnum.远程门店账目查询指令: CreateStoreQueryResponseProtocolData(TransmiteEnum.远程门店账目查询指令响应, requestData, ref requestObj, ref outObj); break;
                    case TransmiteEnum.远程门店账目应答通知指令: CreateStoreQueryResultNotifyResponseProtocolData(TransmiteEnum.远程门店账目应答通知指令响应,requestData,packId, ref requestObj, ref outObj); break;
                    //case TransmiteEnum.远程门店会员卡数据请求响应: CreateMemberQueryResultNotifyResponseProtocolData(TransmiteEnum.远程门店会员卡数据请求响应, requestData, packId, ref requestObj, ref outObj); break;
                    default: break;
                }
            }

        #endregion


        #region "协议数据"

            /// <summary>
            /// 创建请求和响应的通讯协议数据
            /// </summary>
            /// <param name="transmiteEnum">指令类型</param>
            /// <param name="dataModel">创建请求和响应的数据模式</param>
            /// <returns></returns>
            private static byte[] CreateRequestProtocolData(TransmiteEnum transmiteEnum, byte[] data)
            {
                List<byte> dataBUF = new List<byte>();

                //2个引导字节
                dataBUF.Add(0xfe);
                dataBUF.Add(0xfe);
                //帧头
                dataBUF.Add(0x68);
                //数据包Json长度
                dataBUF.AddRange(BitConverter.GetBytes((UInt16)data.Length));
                //帧序号按(data)
                dataBUF.Add(0x01);
                //帧总数按(data)
                dataBUF.Add(0x01);
                //指令字
                dataBUF.Add((byte)transmiteEnum);
                //数据
                dataBUF.AddRange(data);
                //帧尾
                dataBUF.Add(0x16);
                //引导
                dataBUF.Add(0xfe);
                dataBUF.Add(0xfe);
                return dataBUF.ToArray();

            }



            /// <summary>
            /// 创建请求和响应的通讯协议数据
            /// </summary>
            /// <param name="transmiteEnum">指令类型</param>
            /// <param name="dataModel">创建请求和响应的数据模式</param>
            /// <returns></returns>
            private static byte[] CreateResponseProtocolData(TransmiteEnum responseTransmiteEnum, byte[] data)
            {
                List<byte> dataBUF = new List<byte>();

                //2个引导字节
                dataBUF.Add(0xfe);
                dataBUF.Add(0xfe);
                //帧头
                dataBUF.Add(0x68);
                //数据包Json长度
                if (data != null)
                {
                    dataBUF.AddRange(BitConverter.GetBytes((UInt16)data.Length));
                }
                else
                {
                    dataBUF.AddRange(BitConverter.GetBytes((UInt16)0));
                }
                
                //帧序号按(data)
                dataBUF.Add(0x01);
                //帧总数按(data)
                dataBUF.Add(0x01);
                //指令字
                dataBUF.Add((byte)responseTransmiteEnum);
                //数据
                if (data != null)
                {
                    dataBUF.AddRange(data);
                }
                else
                {
                    byte b = (byte)0;
                    dataBUF.Add(b);
                }
                //帧尾
                dataBUF.Add(0x16);
                //引导
                dataBUF.Add(0xfe);
                dataBUF.Add(0xfe);
                return dataBUF.ToArray();
            }

            #endregion


        #region "雷达注册授权"

            /// <summary>
            /// 创建雷达注册请求响应协议数据模式
            /// </summary>
            /// <param name="responseTransmiteEnum">指令类型枚举</param>
            /// <param name="data">请求的Json数据内容</param>
            /// <param name="responsePackages">输出参数模式</param>
            private static void CreateRadarResponseProtocolData(TransmiteEnum responseTransmiteEnum, string data, ref object outObj)
            {
                string storeId = string.Empty;
                string segment = string.Empty;
                string radarToken = string.Empty;
                string password = string.Empty;
                //创建雷达注册请求相应数据模式
                object responseModel = CreateRadarRegisterResponseDataModel(data, out radarToken, out storeId, out segment, out password);
                SignKeyHelper.SetSignKey(responseModel, password);
                //对象序列化为字节数组
                byte[] dataByteArr = JsonHelper.DataContractJsonSerializerToByteArray(responseModel);
                byte[] responsePackages = CreateResponseProtocolData(responseTransmiteEnum, dataByteArr);
                string responseJson = JsonHelper.DataContractJsonSerializer(responseModel);
                outObj = new RadarRegisterOutParamsModel(storeId, segment,radarToken, responsePackages, responseJson);
            }

            /// <summary>
            /// 创建雷达注册请求响应数据模式
            /// </summary>
            /// <param name="data">请求的数据</param>
            /// <param name="radarToken">雷达token</param>
            /// <param name="storeId">店ID</param>
            /// <param name="segment">路由地址</param>
            /// <param name="password">店密码</param>
            /// <returns></returns>
            private static object CreateRadarRegisterResponseDataModel(string data, out string radarToken, out string storeId, out string segment, out string password)
            {
                string dbName = string.Empty;
                password = string.Empty;
                string errMsg = string.Empty;
                radarToken = string.Empty;
                storeId = string.Empty;
                segment = string.Empty;
                //验证雷达注册的请求数据
                RadarRegisterRequestDataModel requestDataModel = JsonHelper.DataContractJsonDeserializer<RadarRegisterRequestDataModel>(data);
                //验证店Id
                StoreBusiness storeBusiness = new StoreBusiness();
                if (!storeBusiness.IsEffectiveStore(requestDataModel.StoreId, out dbName, out password, out errMsg))
                {
                    return new ComonErrorResponseModel(Result_Code.F, errMsg, "");
                }

                //验证MD5
                if (!SignKeyHelper.CheckSignKey(requestDataModel, password))
                { 
                    return new ComonErrorResponseModel(Result_Code.F, "签名不正确", "");
                }

                //验证segment
                //if (!new DeviceBusiness().IsEffectiveStoreSegment(dbName, requestDataModel.Segment, out errMsg))
                //{
                //    return new ComonErrorResponseModel(Result_Code.F, errMsg, "");
                //}

                //设置路由设备token
                radarToken = XCGameRadarDeviceTokenBusiness.SetRadarDeviceToken(requestDataModel.StoreId, requestDataModel.Segment);
                storeId = requestDataModel.StoreId;
                segment = requestDataModel.Segment;
                return new RadarRegisterResponseModel(string.IsNullOrEmpty(radarToken) ? "" : radarToken, "");
            }

        #endregion


        #region "设备状态变更通知"

            /// <summary>
            /// 创建设备状态变更通知响应协议数据模式
            /// </summary>
            /// <param name="responseTransmiteEnum">指令类型枚举</param>
            /// <param name="data">请求的Json数据内容</param>
            /// <param name="obj">输出参数模式</param>
            private static void CreateDeviceStateResponseProtocolData(TransmiteEnum responseTransmiteEnum, string data, ref object requestDataObj, ref object obj)
            {
                //创建设备状态变更通知响应数据模式
                DeviceStateRequestDataModel requestDataModel = null;
                object responseModel = CreateDeviceStateResponseDataModel(data, ref requestDataModel);
                //变更设备响应内容为空
                byte[] responsePackages = CreateResponseProtocolData(responseTransmiteEnum, null);
                string responseJson = JsonHelper.DataContractJsonSerializer(responseModel);
                obj = new DeviceStateOutParamsModel(responsePackages, responseModel, responseJson);
                requestDataObj = ((DeviceStateRequestDataModel)(requestDataModel));
            }

            /// <summary>
            /// 创建设备状态变更通知响应数据模式
            /// </summary>
            /// <param name="data">请求的数据</param>
            private static object CreateDeviceStateResponseDataModel(string data, ref DeviceStateRequestDataModel requestDataModel)
            {
                string dbName = string.Empty;
                string password = string.Empty;
                string errMsg = string.Empty;
                //验证雷达注册的请求数据
                requestDataModel = JsonHelper.DataContractJsonDeserializer<DeviceStateRequestDataModel>(data);
                requestDataModel.StoreId = "invalidstore";
                //验证mcuId
                XCCloudService.Model.XCGameManager.t_device manadeviceModel = null;
                if (!DeviceManaBusiness.CheckDevice(requestDataModel.MCUId, ref manadeviceModel))
                {
                    return new ComonErrorResponseModel(Result_Code.F, "mcuId无效", "");
                }
                //验证token
                XCGameRadarDeviceTokenModel deviceTokenModel = XCGameRadarDeviceTokenBusiness.GetRadarDeviceTokenModel(requestDataModel.Token);
                if (deviceTokenModel == null)
                {
                    return new ComonErrorResponseModel(Result_Code.F, "token无效", "");
                }
                //验证门店是否一致
                if (!manadeviceModel.StoreId.Equals(deviceTokenModel.StoreId))
                {
                    return new ComonErrorResponseModel(Result_Code.F, "设备ID门店和雷达门店不一致", "");
                }
                //验证店Id
                StoreBusiness storeBusiness = new StoreBusiness();
                if (!storeBusiness.IsEffectiveStore(deviceTokenModel.StoreId, out dbName, out password, out errMsg))
                {
                    return new ComonErrorResponseModel(Result_Code.F, errMsg, "");
                }
                requestDataModel.StoreId = deviceTokenModel.StoreId;
                //验证MD5
                if (!SignKeyHelper.CheckSignKey(requestDataModel, password))
                {
                    return new ComonErrorResponseModel(Result_Code.F, "签名不正确", "");
                }

                //修改设备状态
                DeviceStateBusiness.SetDeviceState(deviceTokenModel.StoreId, requestDataModel.MCUId, requestDataModel.Status);
                return new ComonSuccessResponseModel(Result_Code.T,"","");
            }

        #endregion


        #region "雷达心跳"

            /// <summary>
            /// 创建雷达心跳响应协议数据模式
            /// </summary>
            /// <param name="responseTransmiteEnum">指令类型枚举</param>
            /// <param name="data">请求的Json数据内容</param>
            /// <param name="outObj">输出参数模式</param>
            private static void CreateRadarHeartbeatResponseProtocolData(TransmiteEnum responseTransmiteEnum, string data,ref object requestObj,ref object outObj)
            {
                string routeDeivceToken = string.Empty;
                string password = string.Empty;
                string storeId = string.Empty;
                //创建设备状态变更通知响应数据模式
                RadarHeartbeatRequestDataModel requestDataModel = null;
                object responseModel = CreateRadarHeartbeatResponseDataModel(data, out password, ref requestDataModel);
                SignKeyHelper.SetSignKey(responseModel, password);
                requestObj = requestDataModel;
                //对象序列化为字节数组
                byte[] dataByteArr = JsonHelper.DataContractJsonSerializerToByteArray(responseModel);
                string responseJson = JsonHelper.DataContractJsonSerializer(responseModel);
                byte[] responsePackages = CreateResponseProtocolData(responseTransmiteEnum, dataByteArr);
                outObj = new RadarHeartbeatOutParamsModel(responsePackages, responseModel, responseJson);
            }

            /// <summary>
            /// 创建雷达心跳响应数据模式
            /// </summary>
            /// <param name="data">请求的Json数据内容</param>
            /// <param name="password">店密码</param>
            private static object CreateRadarHeartbeatResponseDataModel(string data, out string password, ref RadarHeartbeatRequestDataModel requestDataModel)
            {
                string dbName = string.Empty;
                password = string.Empty;
                string errMsg = string.Empty;
                //验证雷达注册的请求数据
                requestDataModel = JsonHelper.DataContractJsonDeserializer<RadarHeartbeatRequestDataModel>(data);
                requestDataModel.StoreId = Constant.InvalidStore;
                //验证token
                XCGameRadarDeviceTokenModel deviceTokenModel = XCGameRadarDeviceTokenBusiness.GetRadarDeviceTokenModel(requestDataModel.Token);
                if (deviceTokenModel == null)
                {
                    return new ComonErrorResponseModel(Result_Code.F, "token无效", "");
                }
                //验证店Id
                StoreBusiness storeBusiness = new StoreBusiness();
                if (!storeBusiness.IsEffectiveStore(deviceTokenModel.StoreId, out dbName, out password, out errMsg))
                {
                    return new ComonErrorResponseModel(Result_Code.F, errMsg, "");
                }
                requestDataModel.StoreId = deviceTokenModel.StoreId;
                requestDataModel.Segment = deviceTokenModel.Segment;
                //验证MD5
                if (!SignKeyHelper.CheckSignKey(requestDataModel, password))
                {
                    return new ComonErrorResponseModel(Result_Code.F, "签名不正确", "");
                }

                return new ComonSuccessResponseModel(Result_Code.T, "", "");
            }

            #endregion


        #region "远程设备控制指令"

            /// <summary>
            /// 创建远程设备控制指令响应协议数据模式
            /// </summary>
            /// <param name="responseTransmiteEnum">指令类型枚举</param>
            /// <param name="data">请求的Json数据内容</param>
            /// <param name="data">输出参数模式</param>
            private static void CreateDeviceControlResponseProtocolData(TransmiteEnum responseTransmiteEnum, string data,ref object requestObj, ref object outObj)
            {
                string routeDeivceToken = string.Empty;
                string password = string.Empty;
                //创建设备控制响应数据模式
                DeviceControlAnswerRequestDataModel requestDataModel = null;
                object responseModel = CreateDeviceControlResponseDataModel(data, out password,ref requestDataModel);
                SignKeyHelper.SetSignKey(responseModel, password);
                requestObj = requestDataModel;
                //对象序列化为字节数组
                byte[] dataByteArr = JsonHelper.DataContractJsonSerializerToByteArray(responseModel);
                string responseJson = JsonHelper.DataContractJsonSerializer(responseModel);
                byte[] responsePackages = CreateResponseProtocolData(responseTransmiteEnum, dataByteArr);
                outObj = new DeviceControlOutParmasModel(responsePackages, responseModel, responseJson);
            }

            

            /// <summary>
            /// 创建设备状态变更通知响应数据模式
            /// </summary>
            /// <param name="data">请求的数据</param>
            /// <param name="password">店密码</param>
            private static object CreateDeviceControlResponseDataModel(string data, out string password, ref DeviceControlAnswerRequestDataModel requestDataModel)
            {
                string dbName = string.Empty;
                password = string.Empty;
                string errMsg = string.Empty;
                //验证雷达注册的请求数据
                requestDataModel = JsonHelper.DataContractJsonDeserializer<DeviceControlAnswerRequestDataModel>(data);
                requestDataModel.StoreId = "invalidestore";

                //验证雷达请求数据结果代码
                if (requestDataModel.Result_Code != "1")
                {
                    errMsg = "雷达发送数据结果代码不正确";
                    return new ComonErrorResponseModel(Result_Code.F, errMsg, "");
                }
                //验证SN序号
                if (!UDPSocketAnswerBusiness.ExistSN(requestDataModel.SN))
                {
                    errMsg = "SN序号无效";
                    return new ComonErrorResponseModel(Result_Code.F, errMsg, "");
                }
                //SN序号获取发送给雷达的缓存数据
                UDPSocketAnswerModel answerModel = UDPSocketAnswerBusiness.GetAnswerModel(requestDataModel.SN);
                requestDataModel.StoreId = answerModel.StoreId;
                requestDataModel.OrderId = answerModel.OrderId;
                //验证店Id
                StoreBusiness storeBusiness = new StoreBusiness();
                if (!storeBusiness.IsEffectiveStore(requestDataModel.StoreId, out dbName, out password, out errMsg))
                {
                    return new ComonErrorResponseModel(Result_Code.F, errMsg, "");
                }
                //验证MD5
                if (!SignKeyHelper.CheckSignKey(requestDataModel, password))
                {
                    return new ComonErrorResponseModel(Result_Code.F, "签名不正确", "");
                }

                return new DeviceControlResponseModel(Result_Code.T, "", requestDataModel.SN, "");
            }

        #endregion


        #region"雷达出币通知回复"


            /// <summary>
            /// 创建雷达出币通知应答数据模式
            /// </summary>
            /// <param name="responseTransmiteEnum">指令类型枚举</param>
            /// <param name="data">请求的Json数据内容</param>
            /// <param name="outObj">输出参数模式</param>
            private static void CreateRadarNotifyResponseProtocolData(TransmiteEnum responseTransmiteEnum, string data, ref object requestModel,ref object outObj)
            {
                string routeDeivceToken = string.Empty;
                string password = string.Empty;
                //创建设备状态变更通知响应数据模式
                RadarNotifyRequestModel requestDataModel = null;
                object responseModel = CreateRadarNotifyResponseDataModel(data, out password, ref requestDataModel);
                SignKeyHelper.SetSignKey(responseModel, password);
                requestModel = requestDataModel;
                //对象序列化为字节数组
                byte[] dataByteArr = JsonHelper.DataContractJsonSerializerToByteArray(responseModel);
                byte[] responsePackages = CreateResponseProtocolData(responseTransmiteEnum, dataByteArr);
                string responseJson = JsonHelper.DataContractJsonSerializer(responseModel);
                outObj = new RadarNotifyOutParamsModel(responsePackages,responseModel,responseJson);
            }

            /// <summary>
            /// 创建设备状态变更通知响应数据模式
            /// </summary>
            /// <param name="data">请求的数据</param>
            private static object CreateRadarNotifyResponseDataModel(string data, out string password, ref RadarNotifyRequestModel requestDataModel)
            {
                string dbName = string.Empty;
                password = string.Empty;
                string errMsg = string.Empty;
                //验证雷达注册的请求数据
                requestDataModel = JsonHelper.DataContractJsonDeserializer<RadarNotifyRequestModel>(data);
                requestDataModel.StoreId = Constant.InvalidStore;
                //验证token
                XCGameRadarDeviceTokenModel deviceTokenModel = XCGameRadarDeviceTokenBusiness.GetRadarDeviceTokenModel(requestDataModel.Token);
                if (deviceTokenModel == null)
                {
                    return new ComonErrorResponseModel(Result_Code.F, "token无效(" + requestDataModel.Token + ")", "");
                }
                //验证店Id
                StoreBusiness storeBusiness = new StoreBusiness();
                if (!storeBusiness.IsEffectiveStore(deviceTokenModel.StoreId, out dbName, out password, out errMsg))
                {
                    return new ComonErrorResponseModel(Result_Code.F, errMsg, "");
                }
                requestDataModel.StoreId = deviceTokenModel.StoreId;
                //验证MD5
                if (!SignKeyHelper.CheckSignKey(requestDataModel, password))
                {
                    return new ComonErrorResponseModel(Result_Code.F, "签名不正确", "");
                }

                //验证订单号是否存在
                return new RadarNotifyResponseModel(requestDataModel.SN,"");
            }

            #endregion


        #region "向雷达发送出币数据包"

            /// <summary>
            /// 向雷达发送出币数据
            /// </summary>
            /// <param name="controlModel">出币请求模式</param>
            /// <param name="errMsg">错误信息</param>
            /// <returns></returns>
            public static bool SendDataToRadar(DeviceControlRequestDataModel controlModel, out string errMsg)
            {
                errMsg = string.Empty;
                //验证雷达是否上线(获取雷达token)
                string radarToken = string.Empty;
                if (!XCGameRadarDeviceTokenBusiness.GetRouteDeviceToken(controlModel.StoreId, controlModel.Segment, out radarToken))
                {
                    errMsg = "雷达未上线";
                    return false;
                }
                string ip = string.Empty;
                int port = 0;
                if (!DataFactory.GetRadarClient(radarToken, out ip, out port))
                {
                    errMsg = "未能获取雷达端地址";
                    return false;
                }
                //向雷达发送数据
                RadarRequestDataModel radarModel =
                        new RadarRequestDataModel(radarToken, controlModel.MCUId, controlModel.Action, controlModel.Coins, controlModel.SN, controlModel.OrderId, controlModel.IcCardId, "","");
                SignKeyHelper.SetSignKey(radarModel, controlModel.StorePassword);
                //对象序列化为字节数组
                byte[] dataByteArr = JsonHelper.DataContractJsonSerializerToByteArray(radarModel);
                //生成发送数据包
                byte[] requestPackages = CreateResponseProtocolData(TransmiteEnum.远程设备控制指令, dataByteArr);
                //按sn序号，保存发送数据包
                UDPSocketAnswerModel answerModel = new UDPSocketAnswerModel(ip, port, requestPackages, controlModel.OrderId,System.DateTime.Now, controlModel.Mobile, controlModel.StoreId, controlModel.Segment, controlModel.MCUId, controlModel.Coins, controlModel.SN);
                UDPSocketAnswerBusiness.SetAnswer(answerModel);
                //服务端发送数据
                XCCloudService.SocketService.UDP.Server.Send(ip, port, requestPackages);
                //记录日志
                string requestJson = JsonHelper.DataContractJsonSerializer(radarModel);
                UDPLogHelper.SaveUDPSendDeviceControlLog(controlModel.StoreId, controlModel.Mobile, controlModel.MCUId, controlModel.OrderId, controlModel.Segment, controlModel.SN, controlModel.Coins, int.Parse(controlModel.Action), requestJson);
                SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程设备控制指令), "远程设备控制指令",radarModel.Token, requestJson);
                return true;
            }

            /// <summary>
            /// 通过雷达token获取雷达客户端信息
            /// </summary>
            /// <returns></returns>
            public static bool GetRadarClient(string radarToken,out string ip,out int port)
            {
                ip = string.Empty;
                port = 0;
                UDPClientItemBusiness.ClientItem clientItem = Server.GetClientItem(radarToken);
                if (clientItem == null)
                {
                    return false;
                }
                else
                {
                    string remotePoint = clientItem.remotePoint.ToString();
                    string[] strArr = remotePoint.Split(':');
                    ip = strArr[0];
                    port = int.Parse(strArr[1]);
                    return true;
                }
            }

        #endregion


        #region "向雷达发送门店账目查询指令"

            public static bool SendDataStoreQuery(string storeId, string date, string sn, string searchType, string icCardId, string storePassword, out string radarToken, out string errMsg)
            {
                errMsg = string.Empty;
                //验证雷达是否上线(获取雷达token)
                if (!XCGameRadarDeviceTokenBusiness.GetRouteDeviceToken(storeId, out radarToken))
                {
                    errMsg = "雷达未上线";
                    return false;
                }
                string ip = string.Empty;
                int port = 0;
                if (!DataFactory.GetRadarClient(radarToken, out ip, out port))
                {
                    errMsg = "未能获取雷达端地址";
                    return false;
                }

                UDPSocketStoreQueryAnswerBusiness.AddAnswer(sn, storeId, radarToken);

                //向雷达发送数据
                //RadarRequestDataModel radarModel =
                //        new RadarRequestDataModel(radarToken, controlModel.MCUId, controlModel.Action, controlModel.Coins, controlModel.SN, controlModel.OrderId, controlModel.IcCardId, "", "");

                StoreQueryRequestDataModel dataModel = new StoreQueryRequestDataModel(storeId, date, sn, searchType, icCardId);
                SignKeyHelper.SetSignKey(dataModel, storePassword);

                //对象序列化为字节数组
                byte[] dataByteArr = JsonHelper.DataContractJsonSerializerToByteArray(dataModel);
                //生成发送数据包
                byte[] requestPackages = CreateResponseProtocolData(TransmiteEnum.远程门店账目查询指令, dataByteArr);
                ////按sn序号，保存发送数据包
                //UDPSocketAnswerModel answerModel = new UDPSocketAnswerModel(ip, port, requestPackages, controlModel.OrderId, System.DateTime.Now, controlModel.Mobile, controlModel.StoreId, controlModel.Segment, controlModel.MCUId, controlModel.Coins, controlModel.SN);
                //UDPSocketAnswerBusiness.SetAnswer(answerModel);
                //服务端发送数据
                XCCloudService.SocketService.UDP.Server.Send(ip, port, requestPackages);
                string requestJson = JsonHelper.DataContractJsonSerializer(dataModel);
                SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店账目查询指令), "远程门店账目查询指令", radarToken, requestJson);
                return true;
            }


        #endregion

        #region ""

            private static void CreateStoreQueryResponseProtocolData(TransmiteEnum responseTransmiteEnum, string data, ref object requestObj, ref object outObj)
            {
                string routeDeivceToken = string.Empty;
                string password = string.Empty;

                StoreQueryResponseModel requestDataModel = null;
                object responseModel = CreateStoreQueryResponseDataModel(data, out password, ref requestDataModel);
                SignKeyHelper.SetSignKey(responseModel, password);
                requestObj = requestDataModel;
                //对象序列化为字节数组
                byte[] dataByteArr = JsonHelper.DataContractJsonSerializerToByteArray(responseModel);
                string responseJson = JsonHelper.DataContractJsonSerializer(responseModel);
                byte[] responsePackages = CreateResponseProtocolData(responseTransmiteEnum, dataByteArr);
                outObj = new StoreQueryOutParamsModel(responsePackages, responseModel, responseJson);
            }

            private static object CreateStoreQueryResponseDataModel(string data, out string password, ref StoreQueryResponseModel requestDataModel)
            {
                string dbName = string.Empty;
                password = string.Empty;
                string errMsg = string.Empty;
                //验证雷达注册的请求数据
                requestDataModel = JsonHelper.DataContractJsonDeserializer<StoreQueryResponseModel>(data);
                requestDataModel.StoreId = "invalidestore";

                ////验证雷达请求数据结果代码
                //if (requestDataModel.Result_Code != "1")
                //{
                //    errMsg = "雷达发送数据结果代码不正确";
                //    return new ComonErrorResponseModel(Result_Code.F, errMsg, "");
                //}
                //验证SN序号
                if (!UDPSocketStoreQueryAnswerBusiness.ExistSN(requestDataModel.SN))
                {
                    errMsg = "SN序号无效";
                    return new ComonErrorResponseModel(Result_Code.F, errMsg, "");
                }
                //SN序号获取发送给雷达的缓存数据
                UDPSocketStoreQueryAnswerModel answerModel = UDPSocketStoreQueryAnswerBusiness.GetAnswerModel(requestDataModel.SN);
                requestDataModel.StoreId = answerModel.StoreId;
                //验证店Id
                StoreBusiness storeBusiness = new StoreBusiness();
                if (!storeBusiness.IsEffectiveStore(requestDataModel.StoreId, out dbName, out password, out errMsg))
                {
                    return new ComonErrorResponseModel(Result_Code.F, errMsg, "");
                }
                //验证MD5
                if (!SignKeyHelper.CheckSignKey(requestDataModel, password))
                {
                    return new ComonErrorResponseModel(Result_Code.F, "签名不正确", "");
                }

                return new StoreQueryResponseModel(Result_Code.T, "", requestDataModel.SN, "");
            }



        #endregion

        #region"雷达出币通知回复"


            /// <summary>
            /// 
            /// </summary>
            /// <param name="responseTransmiteEnum">指令类型枚举</param>
            /// <param name="data">请求的Json数据内容</param>
            /// <param name="outObj">输出参数模式</param>
            private static void CreateStoreQueryResultNotifyResponseProtocolData(TransmiteEnum responseTransmiteEnum, string data,int packId, ref object requestModel, ref object outObj)
            {
                string routeDeivceToken = string.Empty;
                string password = string.Empty;
                //创建设备状态变更通知响应数据模式
                StoreQueryResultNotifyRequestModel requestDataModel = null;
                object responseModel = CreateStoreQueryResultNotifyResponseDataModel(data, out password, ref requestDataModel, packId);
                SignKeyHelper.SetSignKey(responseModel, password);
                requestModel = requestDataModel;
                //对象序列化为字节数组
                byte[] dataByteArr = JsonHelper.DataContractJsonSerializerToByteArray(responseModel);
                byte[] responsePackages = CreateResponseProtocolData(responseTransmiteEnum, dataByteArr);
                string responseJson = JsonHelper.DataContractJsonSerializer(responseModel);
                outObj = new StoreQueryNotifyOutParamsModel(responsePackages, responseModel, responseJson);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="data">请求的数据</param>
            private static object CreateStoreQueryResultNotifyResponseDataModel(string data, out string password, ref StoreQueryResultNotifyRequestModel requestDataModel,int packId)
            {
                string dbName = string.Empty;
                password = string.Empty;
                string errMsg = string.Empty;
                //验证雷达注册的请求数据
                object obj = Utils.DeserializeObject(data);
                string token = Utils.GetJsonObjectValue(obj, "token").ToString();
                object tabledataObj = Utils.GetJsonObjectValue(obj, "tabledata");
                object[] tabledataArr = (object[])tabledataObj;
                List<StoreQueryResultNotifyTableData> list = new List<StoreQueryResultNotifyTableData>();
                for (int i = 0; i < tabledataArr.Length; i++)
                {
                    string key = Utils.GetJsonObjectValue(tabledataArr[i], "itemKey").ToString();
                    string value = Utils.GetJsonObjectValue(tabledataArr[i], "itemValue").ToString();
                    list.Add(new StoreQueryResultNotifyTableData(key,value));
                }

                string sn = Utils.GetJsonObjectValue(obj, "sn").ToString();
                string signkey = Utils.GetJsonObjectValue(obj, "signkey").ToString();

                requestDataModel = new StoreQueryResultNotifyRequestModel(token, signkey, sn, list);
                requestDataModel.StoreId = Constant.InvalidStore;
                //验证token
                XCGameRadarDeviceTokenModel deviceTokenModel = XCGameRadarDeviceTokenBusiness.GetRadarDeviceTokenModel(requestDataModel.Token);
                if (deviceTokenModel == null)
                {
                    return new StoreQueryResultNotifyResponseModel("0", "token无效(" + requestDataModel.Token + ")", "");
                }
                //验证店Id
                StoreBusiness storeBusiness = new StoreBusiness();
                if (!storeBusiness.IsEffectiveStore(deviceTokenModel.StoreId, out dbName, out password, out errMsg))
                {
                    return new StoreQueryResultNotifyResponseModel("0", errMsg, "");
                }
                requestDataModel.StoreId = deviceTokenModel.StoreId;
                //验证MD5
                //if (!SignKeyHelper.CheckSignKey(requestDataModel, password))
                //{
                //    return new StoreQueryResultNotifyResponseModel("0", "签名不正确","");
                //}

                //验证订单号是否存在
                return new StoreQueryResultNotifyResponseModel("1", "", packId.ToString());
            }

        #endregion

        #region "向雷达发送会员查询指令"

            public static bool SendDataMemberQuery(string sn, string storeId, string storePassword, string icCardId, out string radarToken, out string errMsg)
            {
                errMsg = string.Empty;
                //验证雷达是否上线(获取雷达token)
                if (!XCGameRadarDeviceTokenBusiness.GetRouteDeviceToken(storeId, out radarToken))
                {
                    errMsg = "雷达未上线";
                    return false;
                }
                string ip = string.Empty;
                int port = 0;
                if (!DataFactory.GetRadarClient(radarToken, out ip, out port))
                {
                    errMsg = "未能获取雷达端地址";
                    return false;
                }

                UDPSocketCommonQueryAnswerModel answerModel = new UDPSocketCommonQueryAnswerModel(sn, storeId, storePassword, 0, null, radarToken);
                UDPSocketCommonQueryAnswerBusiness.AddAnswer(sn, answerModel);

                MemberQueryRequestDataModel dataModel = new MemberQueryRequestDataModel(sn,icCardId);
                SignKeyHelper.SetSignKey(dataModel, storePassword);

                //对象序列化为字节数组
                byte[] dataByteArr = JsonHelper.DataContractJsonSerializerToByteArray(dataModel);
                //生成发送数据包
                byte[] requestPackages = CreateResponseProtocolData(TransmiteEnum.远程门店会员卡数据请求, dataByteArr);

                //服务端发送数据
                XCCloudService.SocketService.UDP.Server.Send(ip, port, requestPackages);
                string requestJson = JsonHelper.DataContractJsonSerializer(dataModel);
                SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店会员卡数据请求), "远程门店会员卡数据请求",radarToken,requestJson);
                return true;
            }

        #endregion

        #region "向雷达发送门票查询指令"

            public static bool SendDataTicketQuery(string sn, string storeId, string storePassword, string barCode, out string radarToken, out string errMsg)
            {
                errMsg = string.Empty;
                //验证雷达是否上线(获取雷达token)
                if (!XCGameRadarDeviceTokenBusiness.GetRouteDeviceToken(storeId, out radarToken))
                {
                    errMsg = "雷达未上线";
                    return false;
                }
                string ip = string.Empty;
                int port = 0;
                if (!DataFactory.GetRadarClient(radarToken, out ip, out port))
                {
                    errMsg = "未能获取雷达端地址";
                    return false;
                }

                UDPSocketCommonQueryAnswerModel answerModel = new UDPSocketCommonQueryAnswerModel(sn, storeId, storePassword, 0, null, radarToken);
                UDPSocketCommonQueryAnswerBusiness.AddAnswer(sn, answerModel);

                TicketQueryRequestDataModel dataModel = new TicketQueryRequestDataModel(sn,barCode);
                SignKeyHelper.SetSignKey(dataModel, storePassword);

                //对象序列化为字节数组
                byte[] dataByteArr = JsonHelper.DataContractJsonSerializerToByteArray(dataModel);
                //生成发送数据包
                byte[] requestPackages = CreateResponseProtocolData(TransmiteEnum.远程门店门票数据请求, dataByteArr);

                //服务端发送数据
                XCCloudService.SocketService.UDP.Server.Send(ip, port, requestPackages);
                string requestJson = JsonHelper.DataContractJsonSerializer(dataModel);
                SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店门票数据请求), "远程门店门票数据请求",radarToken,requestJson);
                return true;
            }

        #endregion

        #region "远程门店门票数据操作指令"

            public static bool SendDataTicketOperate(string sn,string storeId,string storePassword, string barCode, string operate,out string radarToken,out string errMsg)
            {
                errMsg = string.Empty;
                //验证雷达是否上线(获取雷达token)
                if (!XCGameRadarDeviceTokenBusiness.GetRouteDeviceToken(storeId, out radarToken))
                {
                    errMsg = "雷达未上线";
                    return false;
                }
                string ip = string.Empty;
                int port = 0;
                if (!DataFactory.GetRadarClient(radarToken, out ip, out port))
                {
                    errMsg = "未能获取雷达端地址";
                    return false;
                }

                UDPSocketCommonQueryAnswerModel answerModel = new UDPSocketCommonQueryAnswerModel(sn, storeId, storePassword, 0, null, radarToken);
                UDPSocketCommonQueryAnswerBusiness.AddAnswer(sn, answerModel);

                TicketOperateRequestDataModel dataModel = new TicketOperateRequestDataModel(sn,barCode, operate);
                SignKeyHelper.SetSignKey(dataModel, storePassword);

                //对象序列化为字节数组
                byte[] dataByteArr = JsonHelper.DataContractJsonSerializerToByteArray(dataModel);
                //生成发送数据包
                byte[] requestPackages = CreateResponseProtocolData(TransmiteEnum.远程门店门票操作请求, dataByteArr);

                //服务端发送数据
                XCCloudService.SocketService.UDP.Server.Send(ip, port, requestPackages);
                string requestJson = JsonHelper.DataContractJsonSerializer(dataModel);
                SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店门票操作请求), "远程门店门票操作请求",radarToken, requestJson);
                return true;
            }

        #endregion

        #region "向雷达发送彩票查询指令"

            public static bool SendDataLotteryQuery(string sn, string storeId, string storePassword, string barCode, out string radarToken, out string errMsg)
            {
                errMsg = string.Empty;
                //验证雷达是否上线(获取雷达token)
                if (!XCGameRadarDeviceTokenBusiness.GetRouteDeviceToken(storeId, out radarToken))
                {
                    errMsg = "雷达未上线";
                    return false;
                }
                string ip = string.Empty;
                int port = 0;
                if (!DataFactory.GetRadarClient(radarToken, out ip, out port))
                {
                    errMsg = "未能获取雷达端地址";
                    return false;
                }

                UDPSocketCommonQueryAnswerModel answerModel = new UDPSocketCommonQueryAnswerModel(sn, storeId, storePassword, 0, null, radarToken);
                UDPSocketCommonQueryAnswerBusiness.AddAnswer(sn, answerModel);

                LotteryQueryRequestDataModel dataModel = new LotteryQueryRequestDataModel(sn,barCode);
                SignKeyHelper.SetSignKey(dataModel, storePassword);

                //对象序列化为字节数组
                byte[] dataByteArr = JsonHelper.DataContractJsonSerializerToByteArray(dataModel);
                //生成发送数据包
                byte[] requestPackages = CreateResponseProtocolData(TransmiteEnum.远程门店彩票数据请求, dataByteArr);

                //服务端发送数据
                XCCloudService.SocketService.UDP.Server.Send(ip, port, requestPackages);
                string requestJson = JsonHelper.DataContractJsonSerializer(dataModel);
                SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店彩票数据请求), "远程门店彩票数据请求", radarToken, requestJson);
                return true;
            }

        #endregion

        #region "向雷达发送彩票操作指令"

            public static bool SendDataLotteryOperate(string sn, string storeId, string storePassword, string barCode, string icCardId, string operate, string mobileName, string phone, out string radarToken, out string errMsg)
            {
                errMsg = string.Empty;
                //验证雷达是否上线(获取雷达token)
                if (!XCGameRadarDeviceTokenBusiness.GetRouteDeviceToken(storeId, out radarToken))
                {
                    errMsg = "雷达未上线";
                    return false;
                }
                string ip = string.Empty;
                int port = 0;
                if (!DataFactory.GetRadarClient(radarToken, out ip, out port))
                {
                    errMsg = "未能获取雷达端地址";
                    return false;
                }

                UDPSocketCommonQueryAnswerModel answerModel = new UDPSocketCommonQueryAnswerModel(sn, storeId, storePassword, 0, null, radarToken);
                UDPSocketCommonQueryAnswerBusiness.AddAnswer(sn, answerModel);

                LotteryOperateRequestDataModel dataModel = new LotteryOperateRequestDataModel(sn,barCode,icCardId,operate,mobileName,phone);
                SignKeyHelper.SetSignKey(dataModel, storePassword);

                //对象序列化为字节数组
                byte[] dataByteArr = JsonHelper.DataContractJsonSerializerToByteArray(dataModel);
                //生成发送数据包
                byte[] requestPackages = CreateResponseProtocolData(TransmiteEnum.远程门店彩票操作请求, dataByteArr);

                //服务端发送数据
                XCCloudService.SocketService.UDP.Server.Send(ip, port, requestPackages);
                string requestJson = JsonHelper.DataContractJsonSerializer(dataModel);
                SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店彩票操作请求), "远程门店彩票操作请求", radarToken, requestJson);
                return true;
            }

        #endregion

        #region "远程门店出票条码数据请求"

            public static bool SendDataOutTicketQuery(string sn, string storeId, string storePassword, string barCode, out string radarToken, out string errMsg)
            {
                errMsg = string.Empty;
                //验证雷达是否上线(获取雷达token)
                if (!XCGameRadarDeviceTokenBusiness.GetRouteDeviceToken(storeId, out radarToken))
                {
                    errMsg = "雷达未上线";
                    return false;
                }
                string ip = string.Empty;
                int port = 0;
                if (!DataFactory.GetRadarClient(radarToken, out ip, out port))
                {
                    errMsg = "未能获取雷达端地址";
                    return false;
                }

                UDPSocketCommonQueryAnswerModel answerModel = new UDPSocketCommonQueryAnswerModel(sn, storeId, storePassword, 0, null, radarToken);
                UDPSocketCommonQueryAnswerBusiness.AddAnswer(sn, answerModel);

                OutTicketQueryRequestDataModel dataModel = new OutTicketQueryRequestDataModel(sn,barCode);
                SignKeyHelper.SetSignKey(dataModel, storePassword);

                //对象序列化为字节数组
                byte[] dataByteArr = JsonHelper.DataContractJsonSerializerToByteArray(dataModel);
                //生成发送数据包
                byte[] requestPackages = CreateResponseProtocolData(TransmiteEnum.远程门店出票条码数据请求, dataByteArr);

                //服务端发送数据
                XCCloudService.SocketService.UDP.Server.Send(ip, port, requestPackages);
                string requestJson = JsonHelper.DataContractJsonSerializer(dataModel);
                SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店出票条码数据请求), "远程门店出票条码数据请求", radarToken, requestJson);
                return true;
            }

        #endregion

        #region "远程门店门票操作请求"

            public static bool SendDataOutTicketOperate(string sn,string storeId,string storePassword,string barCode,string icCardId,string mobileName,string phone,decimal money,string operate,out string radarToken,out string errMsg)
            {
                errMsg = string.Empty;
                //验证雷达是否上线(获取雷达token)
                if (!XCGameRadarDeviceTokenBusiness.GetRouteDeviceToken(storeId, out radarToken))
                {
                    errMsg = "雷达未上线";
                    return false;
                }
                string ip = string.Empty;
                int port = 0;
                if (!DataFactory.GetRadarClient(radarToken, out ip, out port))
                {
                    errMsg = "未能获取雷达端地址";
                    return false;
                }

                UDPSocketCommonQueryAnswerModel answerModel = new UDPSocketCommonQueryAnswerModel(sn, storeId, storePassword, 0, null, radarToken);
                UDPSocketCommonQueryAnswerBusiness.AddAnswer(sn, answerModel);

                OutTicketOperateRequestDataModel dataModel = new OutTicketOperateRequestDataModel(sn,barCode, icCardId, mobileName, phone, money, operate);
                SignKeyHelper.SetSignKey(dataModel, storePassword);

                //对象序列化为字节数组
                byte[] dataByteArr = JsonHelper.DataContractJsonSerializerToByteArray(dataModel);
                //生成发送数据包
                byte[] requestPackages = CreateResponseProtocolData(TransmiteEnum.远程门店出票条码操作请求, dataByteArr);

                //服务端发送数据
                XCCloudService.SocketService.UDP.Server.Send(ip, port, requestPackages);
                string requestJson = JsonHelper.DataContractJsonSerializer(dataModel);
                SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店出票条码操作请求), "远程门店出票条码操作请求", radarToken, requestJson);
                return true;
            }

        #endregion

        #region "远程门店运行参数请求"

            public static bool SendDataParamQuery(string sn, string storeId, string storePassword, string requestType, out string radarToken, out string errMsg)
            {
                errMsg = string.Empty;
                //验证雷达是否上线(获取雷达token)
                if (!XCGameRadarDeviceTokenBusiness.GetRouteDeviceToken(storeId, out radarToken))
                {
                    errMsg = "雷达未上线";
                    return false;
                }
                string ip = string.Empty;
                int port = 0;
                if (!DataFactory.GetRadarClient(radarToken, out ip, out port))
                {
                    errMsg = "未能获取雷达端地址";
                    return false;
                }

                UDPSocketCommonQueryAnswerModel answerModel = new UDPSocketCommonQueryAnswerModel(sn, storeId, storePassword, 0, null, radarToken);
                UDPSocketCommonQueryAnswerBusiness.AddAnswer(sn, answerModel);

                ParamQueryRequestDataModel dataModel = new ParamQueryRequestDataModel(sn,requestType);
                SignKeyHelper.SetSignKey(dataModel, storePassword);

                //对象序列化为字节数组
                byte[] dataByteArr = JsonHelper.DataContractJsonSerializerToByteArray(dataModel);
                //生成发送数据包
                byte[] requestPackages = CreateResponseProtocolData(TransmiteEnum.远程门店运行参数数据请求, dataByteArr);

                //服务端发送数据
                XCCloudService.SocketService.UDP.Server.Send(ip, port, requestPackages);
                string requestJson = JsonHelper.DataContractJsonSerializer(dataModel);
                SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店运行参数数据请求),"远程门店运行参数数据请求",radarToken, requestJson);
                return true;
            }

        #endregion

        #region "远程门店会员转账操作请求"

            public static bool SendMemberTransOperate(string sn, string storeId, string storePassword, string mobilename, string phone, string iniccardid, string outiccardid, int coins, out string radarToken, out string errMsg)
            {
                errMsg = string.Empty;
                //验证雷达是否上线(获取雷达token)
                if (!XCGameRadarDeviceTokenBusiness.GetRouteDeviceToken(storeId, out radarToken))
                {
                    errMsg = "雷达未上线";
                    return false;
                }
                string ip = string.Empty;
                int port = 0;
                if (!DataFactory.GetRadarClient(radarToken, out ip, out port))
                {
                    errMsg = "未能获取雷达端地址";
                    return false;
                }

                UDPSocketCommonQueryAnswerModel answerModel = new UDPSocketCommonQueryAnswerModel(sn, storeId, storePassword, 0, null, radarToken);
                UDPSocketCommonQueryAnswerBusiness.AddAnswer(sn, answerModel);

                MemberTransOperateRequestDataModel dataModel = new MemberTransOperateRequestDataModel(sn,mobilename, phone, iniccardid,outiccardid, coins);
                SignKeyHelper.SetSignKey(dataModel, storePassword);

                //对象序列化为字节数组
                byte[] dataByteArr = JsonHelper.DataContractJsonSerializerToByteArray(dataModel);
                //生成发送数据包
                byte[] requestPackages = CreateResponseProtocolData(TransmiteEnum.远程门店会员转账操作请求, dataByteArr);

                //服务端发送数据
                XCCloudService.SocketService.UDP.Server.Send(ip, port, requestPackages);
                string requestJson = JsonHelper.DataContractJsonSerializer(dataModel);
                SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店会员转账操作请求), "远程门店会员转账操作请求",radarToken, requestJson);
                return true;
            }

        #endregion

        #region "远程门店员工手机号校验指令"

            public static bool SendDataUserPhoneQuery(string sn, string storeId, string storePassword, string mobile, out string radarToken, out string errMsg)
            {
                errMsg = string.Empty;
                //验证雷达是否上线(获取雷达token)
                if (!XCGameRadarDeviceTokenBusiness.GetRouteDeviceToken(storeId, out radarToken))
                {
                    errMsg = "雷达未上线";
                    return false;
                }
                string ip = string.Empty;
                int port = 0;
                if (!DataFactory.GetRadarClient(radarToken, out ip, out port))
                {
                    errMsg = "未能获取雷达端地址";
                    return false;
                }

                UDPSocketCommonQueryAnswerModel answerModel = new UDPSocketCommonQueryAnswerModel(sn, storeId, storePassword, 0, null, radarToken);
                UDPSocketCommonQueryAnswerBusiness.AddAnswer(sn, answerModel);

                UserPhoneQueryRequestDataModel dataModel = new UserPhoneQueryRequestDataModel(sn, mobile);
                SignKeyHelper.SetSignKey(dataModel, storePassword);

                //对象序列化为字节数组
                byte[] dataByteArr = JsonHelper.DataContractJsonSerializerToByteArray(dataModel);
                //生成发送数据包
                byte[] requestPackages = CreateResponseProtocolData(TransmiteEnum.远程门店员工手机号校验请求, dataByteArr);

                //服务端发送数据
                XCCloudService.SocketService.UDP.Server.Send(ip, port, requestPackages);
                string requestJson = JsonHelper.DataContractJsonSerializer(dataModel);
                SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.远程门店员工手机号校验请求), "远程门店员工手机号校验请求", radarToken, requestJson);
                return true;
            }

            #endregion

        #region "向雷达发送黄牛卡信息查询指令"

            public static bool SendDataCattleMemberCardQuery(string sn, string storeId, string storePassword, string requestType, out string radarToken, out string errMsg)
            {
                errMsg = string.Empty;
                //验证雷达是否上线(获取雷达token)
                if (!XCGameRadarDeviceTokenBusiness.GetRouteDeviceToken(storeId, out radarToken))
                {
                    errMsg = "雷达未上线";
                    return false;
                }
                string ip = string.Empty;
                int port = 0;
                if (!DataFactory.GetRadarClient(radarToken, out ip, out port))
                {
                    errMsg = "未能获取雷达端地址";
                    return false;
                }

                UDPSocketCommonQueryAnswerModel answerModel = new UDPSocketCommonQueryAnswerModel(sn, storeId, storePassword, 0, null, radarToken);
                UDPSocketCommonQueryAnswerBusiness.AddAnswer(sn, answerModel);

                CattleMemberCardQueryRequestDataModel dataModel = new CattleMemberCardQueryRequestDataModel(sn,requestType);
                SignKeyHelper.SetSignKey(dataModel, storePassword);

                //对象序列化为字节数组
                byte[] dataByteArr = JsonHelper.DataContractJsonSerializerToByteArray(dataModel);
                //生成发送数据包
                byte[] requestPackages = CreateResponseProtocolData(TransmiteEnum.黄牛卡信息查询请求, dataByteArr);

                //服务端发送数据
                XCCloudService.SocketService.UDP.Server.Send(ip, port, requestPackages);
                string requestJson = JsonHelper.DataContractJsonSerializer(dataModel);
                SignalrServerToClient.BroadcastMessage(Convert.ToInt32(TransmiteEnum.黄牛卡信息查询请求), "黄牛卡信息查询请求", radarToken, requestJson);
                return true;
            }

        #endregion
    }
}
