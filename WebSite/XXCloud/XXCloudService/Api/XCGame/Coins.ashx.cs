using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.CacheService;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGame;
using XCCloudService.Business.XCGameMana;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCGame;
using XCCloudService.Model.XCGame;
using XCCloudService.Business.XCGame;
using XCCloudService.Common.Enum;
using System.Data.SqlClient;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.Model.XCGameManager;
using XCCloudService.Business.Common;
using XCCloudService.Model.Socket.UDP;
using XCCloudService.SocketService.UDP.Factory;
using XCCloudService.Business.WeiXin;
using XCCloudService.WeiXin.WeixinOAuth;
using XXCloudService.Utility;
using XCCloudService.WeiXin.Message;

namespace XCCloudService.Api.XCGame
{
    /// <summary>
    /// Coins 的摘要说明
    /// </summary>
    public class Coins : ApiBase
    {
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameMemberOrMobileToken)]
        public object saveCoins(Dictionary<string, object> dicParas)
        {
            try
            {
                string state = string.Empty;
                string stateName = string.Empty;
                string xcGameDBName = string.Empty;
                string errMsg = string.Empty;
                string storePassword = string.Empty;
                string terminalNo = dicParas.ContainsKey("deviceToken") ? dicParas["deviceToken"].ToString() : string.Empty;

                XCGameMemberTokenModel memberTokenModel = (XCGameMemberTokenModel)(dicParas[Constant.XCGameMemberTokenModel]);
                MobileTokenModel mobileTokenModel = (MobileTokenModel)(dicParas[Constant.MobileTokenModel]);
         
                //根据终端号查询终端号是否存在
                XCCloudService.BLL.IBLL.XCGameManager.IDeviceService deviceService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGameManager.IDeviceService>();
                var deviceModel = deviceService.GetModels(p => p.TerminalNo.Equals(terminalNo, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XCCloudService.Model.XCGameManager.t_device>();
                if (deviceModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "终端号不存在");
                }
                StoreBusiness store = new StoreBusiness();
                if (!store.IsEffectiveStore(deviceModel.StoreId, out xcGameDBName, out storePassword, out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                if (!deviceModel.StoreId.Equals(memberTokenModel.StoreId))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "会员令牌对应门店和设备不一致");
                }

                //判断设备状态是否为启用状态
                XCCloudService.BLL.IBLL.XCGame.IDeviceService ids = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IDeviceService>(xcGameDBName);
                var menlist = ids.GetModels(p => p.MCUID.Equals(deviceModel.DeviceId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XCCloudService.Model.XCGame.t_device>();
                if (menlist == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备不存在");
                }
            
                if (menlist.state != "启用")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备未启用");
                }

                //验证缓存设备状态
                if (DeviceStateBusiness.ExistDeviceState(deviceModel.StoreId, deviceModel.DeviceId))
                {
                    state = DeviceStateBusiness.GetDeviceState(deviceModel.StoreId, deviceModel.DeviceId);
                }
                if (state != "1")
                {
                    stateName = DeviceStateBusiness.GetStateName(state);
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, stateName);
                }
            
                //请求雷达处理存币
                string action = ((int)(DevieControlTypeEnum.存币)).ToString();
                string sn = UDPSocketAnswerBusiness.GetSN();
                string orderId = System.Guid.NewGuid().ToString("N");
                DeviceControlRequestDataModel deviceControlModel = new DeviceControlRequestDataModel(deviceModel.StoreId, mobileTokenModel.Mobile, memberTokenModel.ICCardId, menlist.segment, menlist.MCUID, action, 0, sn, orderId, storePassword, 0,"");
                MPOrderBusiness.AddTCPAnswerOrder(orderId, mobileTokenModel.Mobile, 0, action, memberTokenModel.ICCardId, deviceModel.StoreId);
                IconOutLockBusiness.AddByNoTimeLimit(mobileTokenModel.Mobile);
                if (!DataFactory.SendDataToRadar(deviceControlModel, out errMsg))
                {
                    ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.F, "", Result_Code.T, errMsg);
                }

                //设置推送消息的缓存结构
                string form_id = dicParas.ContainsKey("form_id") ? dicParas["form_id"].ToString() : string.Empty;
                SAppMessageMana.SetMemberCoinsMsgCacheData(SAppMessageType.MemberCoinsOperationNotify,orderId, form_id, mobileTokenModel.Mobile,  null, out errMsg);

                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}