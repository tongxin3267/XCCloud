using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGame;
using XCCloudService.Business.Common;
using XCCloudService.Business.XCGameMana;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCGameManager;
using XCCloudService.Model.Socket.UDP;
using XCCloudService.Model.XCGame;
using XCCloudService.SocketService.UDP;
using XCCloudService.SocketService.UDP.Factory;
using XCCloudService.SocketService.UDP.Security;

namespace XXCloudService.Api.Service
{
    /// <summary>
    /// UDPService 的摘要说明
    /// </summary>
    public class UDPService : ApiBase
    {
        /// <summary>
        /// 雷达注册测试接口
        /// </summary>

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameManamAdminUserToken, SysIdAndVersionNo = false)]
        public object radarRegister(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
            string segment = dicParas.ContainsKey("segment") ? dicParas["segment"].ToString() : string.Empty;

            XCGameManaAdminUserTokenModel tokenModel = (XCGameManaAdminUserTokenModel)(dicParas[Constant.XCGameManamAdminUserToken]);
            if (tokenModel == null)
            {
                errMsg = "用户没有授权";
                return false;
            }
            
            StoreBusiness storeBusiness = new StoreBusiness();
            StoreCacheModel storeCacheModel = null;
            if (!storeBusiness.IsEffectiveStore(storeId, ref storeCacheModel,out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店不存在");
            }

            ClientService service = new ClientService();
            service.Connection();
            RadarRegisterRequestDataModel dataModel = new RadarRegisterRequestDataModel(storeId, segment);
            SignKeyHelper.SetSignKey(dataModel, storeCacheModel.StorePassword);
            byte[] data = DataFactory.CreateRequesProtocolData(TransmiteEnum.雷达注册授权, dataModel);
            service.Send(data);

            System.Threading.Thread.Sleep(5000);

            int count = 0;
            UDPClientItemBusiness.ClientItem item = XCCloudService.SocketService.UDP.ClientList.ClientListObj.Where<UDPClientItemBusiness.ClientItem>(p => p.StoreID.Equals(storeId) && p.Segment.Equals(segment)).FirstOrDefault<UDPClientItemBusiness.ClientItem>();
            while (item == null && count < 10)
            {
                item = XCCloudService.SocketService.UDP.ClientList.ClientListObj.Where<UDPClientItemBusiness.ClientItem>(p => p.StoreID.Equals(storeId) && p.Segment.Equals(segment)).FirstOrDefault<UDPClientItemBusiness.ClientItem>();
                System.Threading.Thread.Sleep(1000);
                count++;
            }

            if (item == null)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "注册失败");
            }

            var obj = new { token = item.gID };
            return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, obj);
        }


        /// <summary>
        /// 雷达心跳测试接口
        /// </summary>

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameManamAdminUserToken, SysIdAndVersionNo = false)]
        public object radarHeat(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            string token = dicParas.ContainsKey("token") ? dicParas["token"].ToString() : string.Empty;

            UDPClientItemBusiness.ClientItem item = XCCloudService.SocketService.UDP.ClientList.ClientListObj.Where<UDPClientItemBusiness.ClientItem>(p => p.gID.Equals(token)).FirstOrDefault<UDPClientItemBusiness.ClientItem>();
            if (item == null)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "雷达token不存在");
            }

            StoreBusiness storeBusiness = new StoreBusiness();
            StoreCacheModel storeCacheModel = null;
            if (!storeBusiness.IsEffectiveStore(item.StoreID, ref storeCacheModel, out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店不存在");
            }

            ClientService service = new ClientService();
            service.Connection();
            RadarHeartbeatRequestDataModel dataModel = new RadarHeartbeatRequestDataModel(token, "");
            byte[] data = DataFactory.CreateRequesProtocolData(TransmiteEnum.雷达心跳, dataModel);
            service.Send(data);

            var obj = new { token = token };
            return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, obj);
        }


        /// <summary>
        /// 设备变更
        /// </summary>

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameManamAdminUserToken, SysIdAndVersionNo = false)]
        public object deviceChange(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            string token = dicParas.ContainsKey("token") ? dicParas["token"].ToString() : string.Empty;
            string mcuid = dicParas.ContainsKey("mcuid") ? dicParas["mcuid"].ToString() : string.Empty;
            string status = dicParas.ContainsKey("status") ? dicParas["status"].ToString() : string.Empty;

            UDPClientItemBusiness.ClientItem item = XCCloudService.SocketService.UDP.ClientList.ClientListObj.Where<UDPClientItemBusiness.ClientItem>(p => p.gID.Equals(token)).FirstOrDefault<UDPClientItemBusiness.ClientItem>();
            if (item == null)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "雷达token不存在");
            }

            StoreBusiness storeBusiness = new StoreBusiness();
            StoreCacheModel storeCacheModel = null;
            if (!storeBusiness.IsEffectiveStore(item.StoreID, ref storeCacheModel, out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店不存在");
            }

            ClientService service = new ClientService();
            service.Connection();
            DeviceStateRequestDataModel dataModel = new DeviceStateRequestDataModel(token, mcuid, status, "", "");
            SignKeyHelper.SetSignKey(dataModel, storeCacheModel.StorePassword);
            byte[] data = DataFactory.CreateRequesProtocolData(TransmiteEnum.设备状态变更通知, dataModel);
            service.Send(data);

            var obj = new { token = token };
            return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, obj);
        }


        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameManamAdminUserToken, SysIdAndVersionNo = false)]
        public object deviceControl(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            string token = dicParas.ContainsKey("token") ? dicParas["token"].ToString() : string.Empty;
            string mcuid = dicParas.ContainsKey("mcuid") ? dicParas["mcuid"].ToString() : string.Empty;
            string icCardId = dicParas.ContainsKey("icCardId") ? dicParas["icCardId"].ToString() : string.Empty;
            string action = dicParas.ContainsKey("controlAction") ? dicParas["controlAction"].ToString() : string.Empty;
            string coins = dicParas.ContainsKey("coins") ? dicParas["coins"].ToString() : string.Empty;

            UDPClientItemBusiness.ClientItem item = XCCloudService.SocketService.UDP.ClientList.ClientListObj.Where<UDPClientItemBusiness.ClientItem>(p => p.gID.Equals(token)).FirstOrDefault<UDPClientItemBusiness.ClientItem>();
            if (item == null)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "雷达token不存在");
            }

            StoreBusiness storeBusiness = new StoreBusiness();
            StoreCacheModel storeCacheModel = null;
            if (!storeBusiness.IsEffectiveStore(item.StoreID, ref storeCacheModel, out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店不存在");
            }

            IMemberService memberService = BLLContainer.Resolve<IMemberService>(storeCacheModel.StoreDBName);
            var memberlist = memberService.GetModels(x => x.ICCardID.ToString() == icCardId).FirstOrDefault<t_member>();
            if (memberlist == null)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机号码无效");
            }

            //判断设备状态是否为启用状态
            XCCloudService.BLL.IBLL.XCGame.IDeviceService ids = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IDeviceService>(storeCacheModel.StoreDBName);
            var menlist = ids.GetModels(p => p.MCUID.Equals(mcuid, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XCCloudService.Model.XCGame.t_device>();
            if (menlist == null)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备不存在");
            }

            if (menlist.state != "启用")
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备未启用");
            }

            string sn = UDPSocketAnswerBusiness.GetSN();
            string orderId = System.Guid.NewGuid().ToString("N");
            DeviceControlRequestDataModel deviceControlModel = new DeviceControlRequestDataModel(item.StoreID, memberlist.Mobile, icCardId, item.Segment, mcuid, action, int.Parse(coins), sn, orderId, storeCacheModel.StorePassword, 0, "");
            MPOrderBusiness.AddTCPAnswerOrder(orderId, memberlist.Mobile, int.Parse(coins), action, icCardId, item.StoreID);

            if (!DataFactory.SendDataToRadar(deviceControlModel, out errMsg))
            {
                ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.F, "", Result_Code.T, errMsg);
            }

            var obj = new { orderId = orderId,sn = sn };
            return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, obj);
        }


        /// <summary>
        /// 获取已注册雷达缓存
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameManamAdminUserToken, SysIdAndVersionNo = false)]
        public object getRegisterRadar(Dictionary<string, object> dicParas)
        {
            try
            {
                string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
                string errMsg = string.Empty;
                string storeName = string.Empty;
                StoreBusiness storeBusiness = new StoreBusiness();
                List<UDPClientItemBusiness.ClientItem> clientList = XCCloudService.SocketService.UDP.ClientList.ClientListObj;
                List<XCGameManaRadarMonitor> monitorList = new List<XCGameManaRadarMonitor>();

                if (!string.IsNullOrEmpty(storeId))
                {
                    clientList = clientList.Where<UDPClientItemBusiness.ClientItem>(p => p.StoreID.Equals(storeId)).ToList<UDPClientItemBusiness.ClientItem>();
                }

                for (int i = 0; i < clientList.Count; i++)
                { 
                    XCGameManaRadarMonitor monitor = new XCGameManaRadarMonitor();
                    monitor.StoreId = clientList[i].StoreID;
                    if (storeBusiness.GetStoreName(monitor.StoreId, out storeName, out errMsg))
                    {
                        monitor.StoreName = storeName;
                    }
                    else
                    {
                        monitor.StoreName = "门店不存在";
                    }
                    monitor.Segment = clientList[i].Segment;
                    monitor.Token = clientList[i].gID;
                    monitor.RegisterTime = clientList[i].curTime.ToString("yyyy-MM-dd HH:mm:ss");
                    monitor.HeatTime = clientList[i].HeatTime.ToString("yyyy-MM-dd HH:mm:ss");
                    monitor.Address = ((IPEndPoint)(clientList[i].remotePoint)).Address.ToString();
                    monitor.Port = ((IPEndPoint)(clientList[i].remotePoint)).Port;
                    monitor.StateName = getStateName(clientList[i].HeatTime);
                    monitorList.Add(monitor);
                }
                return ResponseModelFactory.CreateSuccessModel<List<XCGameManaRadarMonitor>>(isSignKeyReturn, monitorList);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public string getStateName(DateTime heatTime)
        {
            if (heatTime < DateTime.Now.AddSeconds(0 - XCCloudService.Common.CommonConfig.RadarOffLineTimeLong))
            {
                return "离线";
            }
            else
            {
                return "在线";
            }
        }
    }
}