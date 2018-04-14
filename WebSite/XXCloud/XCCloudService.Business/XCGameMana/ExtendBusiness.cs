using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.Container;
using XCCloudService.Business.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.CustomModel.XCGameManager;
using XCCloudService.Model.Socket.UDP;

namespace XCCloudService.Business.XCGameMana
{
    public class ExtendBusiness
    {
        /// <summary>
        /// 验证总库设备信息
        /// </summary>
        /// <param name="deviceToken"></param>
        /// <param name="deviceStoreType"></param>
        /// <param name="storeId"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static bool checkXCGameManaDeviceInfo(string deviceToken, out XCGameManaDeviceStoreType deviceStoreType, out string storeId, out string deviceId)
        {
            storeId = string.Empty;
            deviceId = string.Empty;
            deviceStoreType = XCGameManaDeviceStoreType.Store;
            XCCloudService.BLL.IBLL.XCGameManager.IDeviceService deviceService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGameManager.IDeviceService>();
            var deviceModel = deviceService.GetModels(p => p.TerminalNo.Equals(deviceToken, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XCCloudService.Model.XCGameManager.t_device>();
            if (deviceModel == null)
            {
                return false;
            }
            else
            {
                deviceStoreType = (XCGameManaDeviceStoreType)(deviceModel.StoreType);
                if (deviceStoreType == XCGameManaDeviceStoreType.Store)
                {
                    storeId = deviceModel.StoreId;
                }
                else
                {
                    storeId = deviceModel.StoreId;
                }
                deviceId = deviceModel.DeviceId;
                return true;
            }
        }

        /// <summary>
        /// 验证门店设备信息
        /// </summary>
        /// <param name="deviceStoreType"></param>
        /// <param name="storeId"></param>
        /// <param name="deviceId"></param>
        /// <param name="segment"></param>
        /// <param name="mcuId"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool checkStoreDeviceInfo(XCGameManaDeviceStoreType deviceStoreType, string storeId, string deviceId, out string segment, out string mcuId, out string xcGameDBName, out int deviceIdentityId,out string storePassword,out string storeName,out string errMsg)
        {
            errMsg = string.Empty;
            segment = string.Empty;
            storeName = string.Empty;
            mcuId = string.Empty;
            xcGameDBName = string.Empty;
            deviceIdentityId = 0;
            storePassword = string.Empty;
            if (deviceStoreType == XCGameManaDeviceStoreType.Store)
            {
                StoreBusiness store = new StoreBusiness();
                StoreCacheModel storeModel = null;
                if (!store.IsEffectiveStore(storeId, ref storeModel, out errMsg))
                {
                    errMsg = "门店不存在";
                    return false;
                }
                storePassword = storeModel.StorePassword;
                xcGameDBName = storeModel.StoreDBName;
                storeName = storeModel.StoreName;

                XCCloudService.BLL.IBLL.XCGame.IDeviceService xcGameDeviceService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IDeviceService>(xcGameDBName);
                var xcGameDeviceModel = xcGameDeviceService.GetModels(p => p.MCUID.Equals(deviceId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XCCloudService.Model.XCGame.t_device>();
                if (xcGameDeviceModel == null)
                {
                    errMsg = "设备不存在";
                    return false;
                }
                else
                {
                    if (xcGameDeviceModel.state != "启用")
                    {
                        errMsg = "设备未启用";
                        return false;
                    }
                    else
                    {
                        segment = xcGameDeviceModel.segment;
                        mcuId = xcGameDeviceModel.MCUID;
                        deviceIdentityId = (int)(xcGameDeviceModel.id);
                        return true;
                    }
                }
            }
            else if (deviceStoreType == XCGameManaDeviceStoreType.Merch)
            {
                XCCloudService.BLL.IBLL.XCCloudRS232.IBase_DeviceInfoService deviceService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCCloudRS232.IBase_DeviceInfoService>();
                var deviceModel = deviceService.GetModels(p => p.SN.Equals(deviceId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XCCloudService.Model.XCCloudRS232.Base_DeviceInfo>();
                if (deviceModel == null)
                {
                    errMsg = "设备不存在";
                    return false;
                }
                else
                {
                    if (deviceModel.Status != 1)
                    {
                        errMsg = "设备未启用";
                        return false;
                    }
                    else
                    {
                        mcuId = deviceModel.SN;
                        deviceIdentityId = deviceModel.ID;
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 验证门店设备信息
        /// </summary>
        /// <param name="deviceStoreType"></param>
        /// <param name="storeId"></param>
        /// <param name="deviceId"></param>
        /// <param name="segment"></param>
        /// <param name="mcuId"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool checkStoreGameDeviceInfo(XCGameManaDeviceStoreType deviceStoreType, string storeId, string deviceId, out string segment, out string mcuId,out string headId,out string gameId,out string xcGameDBName,out string storePassword, out string storeName, out string errMsg)
        {
            errMsg = string.Empty;
            segment = string.Empty;
            storeName = string.Empty;
            mcuId = string.Empty;
            headId = string.Empty;
            gameId = string.Empty;
            xcGameDBName = string.Empty;
            storePassword = string.Empty;

            if (deviceStoreType == XCGameManaDeviceStoreType.Store)
            {
                StoreBusiness store = new StoreBusiness();
                StoreCacheModel storeModel = null;
                if (!store.IsEffectiveStore(storeId, ref storeModel, out errMsg))
                {
                    errMsg = "门店不存在";
                    return false;
                }
                storePassword = storeModel.StorePassword;
                xcGameDBName = storeModel.StoreDBName;
                storeName = storeModel.StoreName;

                //验证头信息
                XCCloudService.BLL.IBLL.XCGame.IHeadService headService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IHeadService>(xcGameDBName);
                var headModel = headService.GetModels(p => p.MCUID.Equals(deviceId) && p.State.Equals("1")).FirstOrDefault<XCCloudService.Model.XCGame.t_head>();
                if (headModel == null)
                {
                    errMsg = "设备门店头部信息不存在";
                    return false;
                }
                //验证游戏机信息
                XCCloudService.BLL.IBLL.XCGame.IGameService gameService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IGameService>(xcGameDBName);
                var gameModel = gameService.GetModels(p => p.GameID.Equals(headModel.GameID) && p.State.Equals("1")).FirstOrDefault<XCCloudService.Model.XCGame.t_game>();
                if (gameModel == null)
                {
                    errMsg = "设备门店头部信息不存在";
                    return false;
                }

                segment = headModel.Segment;
                mcuId = headModel.MCUID;
                headId = headModel.HeadID;
                gameId = headModel.GameID;
                return true;
            }
            else if (deviceStoreType == XCGameManaDeviceStoreType.Merch)
            {
                XCCloudService.BLL.IBLL.XCCloudRS232.IBase_DeviceInfoService deviceService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCCloudRS232.IBase_DeviceInfoService>();
                var deviceModel = deviceService.GetModels(p => p.SN.Equals(deviceId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XCCloudService.Model.XCCloudRS232.Base_DeviceInfo>();
                if (deviceModel == null)
                {
                    errMsg = "设备不存在";
                    return false;
                }
                else
                {
                    if (deviceModel.Status != 1)
                    {
                        errMsg = "设备未启用";
                        return false;
                    }
                    else
                    {
                        mcuId = deviceModel.SN;
                        //deviceIdentityId = deviceModel.ID;
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }



        /// <summary>
        /// 验证雷达设备状态
        /// </summary>
        /// <param name="deviceStoreType">设备门店类型</param>
        /// <param name="storeId">门店Id</param>
        /// <param name="deviceId">设备Id</param>
        /// <param name="errMsg">错误消息</param>
        /// <returns></returns>
        public static bool checkRadarDeviceState(XCGameManaDeviceStoreType deviceStoreType, string storeId, string deviceId, out string errMsg)
        {
            string state = string.Empty;
            string stateName = string.Empty;
            errMsg = string.Empty;
            if (deviceStoreType == XCGameManaDeviceStoreType.Store)
            {
                if (DeviceStateBusiness.ExistDeviceState(storeId, deviceId))
                {
                    state = DeviceStateBusiness.GetDeviceState(storeId, deviceId);
                }
                if (state != "1")
                {
                    stateName = DeviceStateBusiness.GetStateName(state);
                    errMsg = stateName;
                    return false;
                }
                return true;
            }
            else if (deviceStoreType == XCGameManaDeviceStoreType.Merch)
            {
                if (DeviceStateBusiness.ExistDeviceState(storeId, deviceId))
                {
                    state = DeviceStateBusiness.GetDeviceState(storeId, deviceId);
                }
                if (state != "1")
                {
                    stateName = DeviceStateBusiness.GetStateName(state);
                    errMsg = stateName;
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }


        public static bool checkRadarDeviceState(XCGameManaDeviceStoreType deviceStoreType, string storeId, string deviceId, out string state,out string stateName,out string errMsg)
        {
            errMsg = string.Empty;
            state = string.Empty;
            stateName = string.Empty;
            if (deviceStoreType == XCGameManaDeviceStoreType.Store)
            {
                if (DeviceStateBusiness.ExistDeviceState(storeId, deviceId))
                {
                    state = DeviceStateBusiness.GetDeviceState(storeId, deviceId);
                    stateName = DeviceStateBusiness.GetStateName(state);
                    return true;
                }
                else
                {
                    errMsg = "设备不存在";
                    return false;
                }  
            }
            else if (deviceStoreType == XCGameManaDeviceStoreType.Merch)
            {
                if (DeviceStateBusiness.ExistDeviceState(storeId, deviceId))
                {
                    state = DeviceStateBusiness.GetDeviceState(storeId, deviceId);
                    stateName = DeviceStateBusiness.GetStateName(state);
                    return true;
                }
                else
                {
                    errMsg = "设备不存在";
                    return false;
                }    
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 读取会员信息
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <param name="xcGameDBName">XCGame数据库名</param>
        /// <param name="balance">会员卡余额</param>
        /// <param name="icCardId">会员卡号</param>
        /// <param name="memberLevelId">会员</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns></returns>
        public static bool GetMemberInfo(XCGameManaDeviceStoreType deviceStoreType, string mobile, string xcGameDBName, out int balance, out int icCardId, out int memberLevelId, out string errMsg)
        {
            balance = 0;
            icCardId = 0;
            memberLevelId = 0;
            errMsg = string.Empty;
            if (deviceStoreType == XCGameManaDeviceStoreType.Store)
            {
                XCCloudService.Model.XCGame.t_member memberModel = null;
                if (XCCloudService.Business.XCGame.MemberBusiness.IsEffectiveStore(mobile, xcGameDBName, ref memberModel, out errMsg))
                {
                    balance = Convert.ToInt32(memberModel.Balance);
                    icCardId = memberModel.ICCardID;
                    memberLevelId = Convert.ToInt32(memberModel.MemberLevelID);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (deviceStoreType == XCGameManaDeviceStoreType.Merch)
            {
                XCCloudService.Model.XCCloudRS232.t_member memberModel = null;
                if (XCCloudService.Business.XCCloud.MemberBusiness.IsEffectiveStore(mobile, ref memberModel, out errMsg))
                {
                    balance = Convert.ToInt32(memberModel.Balance);
                    icCardId = (int)(memberModel.ICCardID);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


   }
}
