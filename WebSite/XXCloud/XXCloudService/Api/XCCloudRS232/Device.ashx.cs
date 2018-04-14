using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.Business.XCCloudRS232;
using XCCloudService.CacheService;
using XCCloudService.Model.XCCloudRS232;
using XCCloudService.Common.Extensions;
using XCCloudService.Model.CustomModel.XCCloudRS232;
using XCCloudService.Common.Enum;
using System.Transactions;
using XCCloudService.Business.Common;
using XCCloudService.Model.Socket.UDP;
using XCCloudService.SocketService.UDP.Factory;
using System.Data.SqlClient;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.BLL.IBLL.XCCloudRS232;
using XCCloudService.BLL.Container;
using XCCloudService.Business.XCGameMana;
using XXCloudService.RadarServer;
using System.Data;
using XCCloudService.Common;
using XCCloudService.Utility;

namespace XXCloudService.Api.XCCloudRS232
{
    public class Device : ApiBase
    {
        #region 扫码获取设备信息
        /// <summary>
        /// 扫码获取设备信息
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getDeviceInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string mobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;
                string deviceToken = dicParas.ContainsKey("deviceToken") ? dicParas["deviceToken"].ToString() : string.Empty;
                string routerToken = dicParas.ContainsKey("routerToken") ? dicParas["routerToken"].ToString().Trim().ToLower() : string.Empty;

                Base_MerchInfo merch = MerchBusiness.GetMerchModel(mobileToken);
                if (merch.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户令牌无效");
                }

                Base_DeviceInfo device = DeviceBusiness.GetDeviceModel(deviceToken);
                if (device.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备令牌无效");
                }

                string status = DeviceStatusBusiness.GetDeviceState(device.Token);

                //当前设备类型
                DeviceTypeEnum currDeviceType = (DeviceTypeEnum)device.DeviceType;

                DeviceInfoModel model = new DeviceInfoModel();
                model.Router = new Router();
                model.Group = new Group();
                model.deviceName = device.DeviceName ?? device.SN;
                model.deviceToken = device.Token;
                model.deviceType = currDeviceType.ToDescription();
                model.deviceSN = device.SN;
                model.status = DeviceStatusBusiness.GetDeviceState(device.Token);


                //设备未绑定
                if (device.MerchID == 0 || device.MerchID.IsNull())
                {
                    model.BindState = (int)DeviceBoundStateEnum.NotBound;
                    return ResponseModelFactory<DeviceInfoModel>.CreateModel(isSignKeyReturn, model);
                }

                //判断设备绑定的商户是否与当前商户一致
                if (device.MerchID != merch.ID)
                {
                    //与当前商户不一致
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "没有权限查看该设备信息");
                }

                //Base_DeviceInfo currRouter = null;
                //if (!string.IsNullOrWhiteSpace(routerToken))
                //{
                //    currRouter = DeviceBusiness.GetDeviceModel(routerToken);
                //}

                //1.如果设备绑定的商户与当前商户一致
                //2.判断设备是否已与当前商户建立绑定关系
                switch (currDeviceType)
                {
                    case DeviceTypeEnum.Router:
                        model.BindState = (int)DeviceBoundStateEnum.Bound;
                        break;
                    case DeviceTypeEnum.SlotMachines:
                    case DeviceTypeEnum.DepositMachine:
                        {
                            //获取外设绑定关系实体
                            Data_MerchDevice md = MerchDeviceBusiness.GetMerchDeviceModel(device.ID);
                            if (md.IsNull())
                            {
                                model.BindState = (int)DeviceBoundStateEnum.NotBound;
                            }
                            else
                            {

                                //获取控制器实体
                                Base_DeviceInfo router = DeviceBusiness.GetDeviceModelById((int)md.ParentID);

                                if (router.Token.Trim().ToLower() != routerToken)
                                {
                                    //设备所属控制器与当前控制器不一致
                                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "该设备不属于当前控制器");
                                }

                                model.BindState = (int)DeviceBoundStateEnum.Bound;
                                model.headAddress = md.HeadAddress;
                                //控制器信息
                                model.Router.routerName = router.DeviceName ?? router.SN;
                                model.Router.routerToken = router.Token;
                                model.Router.sn = router.SN;
                            }
                        }
                        break;
                    case DeviceTypeEnum.Clerk:
                    case DeviceTypeEnum.Terminal:
                        {
                            //获取终端绑定关系实体
                            Data_MerchSegment ms = MerchSegmentBusiness.GetMerchSegmentModel(device.ID);
                            if (ms.IsNull())
                            {
                                model.BindState = (int)DeviceBoundStateEnum.NotBound;
                            }
                            else
                            {
                                //获取控制器实体
                                Base_DeviceInfo router = DeviceBusiness.GetDeviceModelById((int)ms.ParentID);

                                if (router.Token.Trim().ToLower() != routerToken)
                                {
                                    //设备所属控制器与当前控制器不一致
                                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "该设备不属于当前控制器");
                                }

                                model.BindState = (int)DeviceBoundStateEnum.Bound;
                                model.headAddress = ms.HeadAddress;

                                //控制器信息
                                model.Router.routerName = router.DeviceName ?? router.SN;
                                model.Router.routerToken = router.Token;
                                model.Router.sn = router.SN;

                                //获取分组实体
                                Data_GameInfo group = GameBusiness.GetGameInfoModel(Convert.ToInt32(ms.GroupID));
                                model.Group.groupId = group.GroupID;
                                model.Group.groupName = group.GroupName;
                                model.Group.groupType = ((GroupTypeEnum)(int)group.GroupType).ToDescription();
                            }
                        }
                        break;
                }

                //返回设备信息
                return ResponseModelFactory<DeviceInfoModel>.CreateModel(isSignKeyReturn, model);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 商户绑定设备
        /// <summary>
        /// 商户绑定设备
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object bindDevice(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string mobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;
                string deviceToken = dicParas.ContainsKey("deviceToken") ? dicParas["deviceToken"].ToString() : string.Empty;
                string deviceName = dicParas.ContainsKey("deviceName") ? dicParas["deviceName"].ToString() : string.Empty;
                string level = dicParas.ContainsKey("level") ? dicParas["level"].ToString() : string.Empty;
                string routerToken = dicParas.ContainsKey("routerToken") ? dicParas["routerToken"].ToString() : string.Empty;
                string groupId = dicParas.ContainsKey("groupId") ? dicParas["groupId"].ToString() : string.Empty; 

                Base_MerchInfo merch = MerchBusiness.GetMerchModel(mobileToken);
                if (merch.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户令牌无效");
                }

                if (merch.State == 0)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "当前用户已被禁用");
                }

                Base_DeviceInfo device = DeviceBusiness.GetDeviceModel(deviceToken);
                if (device.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备令牌无效");
                }

                //设备已被绑定
                if (!device.MerchID.IsNull() && device.MerchID != 0 && device.MerchID != merch.ID)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "该设备已被其他商户绑定");
                }

                //设备未绑定，开始绑定
                device.MerchID = merch.ID;
                device.DeviceName = deviceName;
                device.Status = (int)DeviceStatusEnum.启用;

                if (string.IsNullOrWhiteSpace(level))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "参数错误");
                }

                DeviceTypeEnum currDeviceType = (DeviceTypeEnum)device.DeviceType;

                switch (level)
                {
                    case "1":
                        {
                            if (currDeviceType == DeviceTypeEnum.Router)
                            {
                                DeviceBusiness.UpdateDevice(device);
                                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
                            }
                            else if (currDeviceType == DeviceTypeEnum.SlotMachines || currDeviceType == DeviceTypeEnum.DepositMachine)
                            {
                                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请选定控制器后绑定该设备");
                            }
                            else
                            {
                                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请在分组中绑定该设备");
                            }
                        }
                    case "2":
                        {
                            if (currDeviceType == DeviceTypeEnum.Router)
                            {
                                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请在返回上级绑定");
                            }
                            else if (currDeviceType == DeviceTypeEnum.SlotMachines || currDeviceType == DeviceTypeEnum.DepositMachine)
                            {
                                if (string.IsNullOrWhiteSpace(routerToken))
                                {
                                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "获取控制器参数错误");
                                }
                                //获取控制器实体
                                Base_DeviceInfo router = DeviceBusiness.GetDeviceModel(routerToken);
                                if (router.IsNull())
                                {
                                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "获取控制器参数错误");
                                }

                                //获取当前控制器中的外设列表
                                var list = MerchDeviceBusiness.GetListByParentId(router.ID).OrderBy(m => m.HeadAddress).ToList();

                                if (list.Count >= 11)
                                {
                                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "超出最大绑定数量");
                                }

                                Data_MerchDevice md = list.FirstOrDefault(m => m.DeviceID == device.ID);
                                if (md.IsNull())
                                {
                                    md = new Data_MerchDevice();
                                    md.ParentID = router.ID;
                                    md.DeviceID = device.ID;

                                    int index = 0;
                                    foreach (var item in list)
                                    {
                                        string currAddress = string.Format("A{0}", index);
                                        //int currIndex = int.Parse(item.HeadAddress, System.Globalization.NumberStyles.AllowHexSpecifier);
                                        if (currAddress != item.HeadAddress)
                                        {
                                            break;
                                        }
                                        index++;
                                        continue;
                                    }
                                    //md.HeadAddress = Convert.ToString(index, 16).PadLeft(2, '0').ToUpper();
                                    md.HeadAddress = string.Format("A{0}", index);
                                }

                                using (var transactionScope = new System.Transactions.TransactionScope(
                                  TransactionScopeOption.RequiresNew))
                                {
                                    DeviceBusiness.UpdateDevice(device);
                                    MerchDeviceBusiness.AddMerchDevice(md);

                                    transactionScope.Complete();
                                }                                

                                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
                            }
                            else
                            {
                                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请在分组中绑定该设备");
                            }
                        }
                    case "3":
                        {
                            if (currDeviceType != DeviceTypeEnum.Clerk && currDeviceType != DeviceTypeEnum.Terminal)
                            {
                                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请在分组中绑定该设备");
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(groupId))
                                {
                                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "获取分组参数错误");
                                }

                                //获取分组实体
                                Data_GameInfo group = GameBusiness.GetGameInfoModel(Convert.ToInt32(groupId));
                                if (group.IsNull())
                                {
                                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "获取分组参数错误");
                                }

                                //根据分组获取当前分组的控制器
                                Base_DeviceInfo router = DeviceBusiness.GetDeviceModelById((int)group.DeviceID);
                                if (router.IsNull())
                                {
                                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "获取控制器参数错误");
                                }

                                //判断当前分组是否属于当前商户
                                if (router.MerchID != merch.ID)
                                {
                                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "分组信息与当前商户不匹配");
                                }

                                //获取当前分组中的终端列表
                                var list = MerchSegmentBusiness.GetListByGroupId(group.GroupID).OrderBy(m => m.HeadAddress).ToList();

                                if (list.Count >= 99)
                                {
                                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "超出最大绑定数量");
                                }

                                Data_MerchSegment ms = list.FirstOrDefault(m => m.ParentID == group.DeviceID && m.DeviceID == device.ID);
                                if (ms.IsNull())
                                {
                                    ms = new Data_MerchSegment();
                                    ms.ParentID = group.DeviceID;
                                    ms.GroupID = group.GroupID;
                                    ms.DeviceID = device.ID;

                                    int index = 1;
                                    foreach (var item in list)
                                    {
                                        int currIndex = int.Parse(item.HeadAddress, System.Globalization.NumberStyles.AllowHexSpecifier);
                                        if (currIndex != index)
                                        {
                                            break;
                                        }
                                        index++;
                                        continue;
                                    }
                                    ms.HeadAddress = Convert.ToString(index, 16).PadLeft(2, '0').ToUpper();
                                }

                                using (var transactionScope = new System.Transactions.TransactionScope(
                                  TransactionScopeOption.RequiresNew))
                                {
                                    DeviceBusiness.UpdateDevice(device);
                                    MerchSegmentBusiness.AddMerchSegment(ms);

                                    transactionScope.Complete();
                                }

                                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
                            }
                        }
                }

                DeviceStatusBusiness.SetDeviceState(device.Token, DeviceStatusEnum.离线.ToDescription());

                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 设备解除绑定
        /// <summary>
        /// 设备解除绑定
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object removeDevice(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string mobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;
                string deviceToken = dicParas.ContainsKey("deviceToken") ? dicParas["deviceToken"].ToString() : string.Empty;

                Base_MerchInfo merch = MerchBusiness.GetMerchModel(mobileToken);
                if (merch.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户令牌无效");
                }

                Base_DeviceInfo device = DeviceBusiness.GetDeviceModel(deviceToken);
                if (device.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备令牌无效");
                }

                //设备所属商户不是当前商户
                if (device.MerchID != merch.ID)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "该设备已被其他商户绑定，没有权限操作该设备");
                }

                //开始解除绑定
                device.MerchID = 0;
                device.Status = (int)DeviceStatusEnum.未激活;

                DeviceTypeEnum currDeviceType = (DeviceTypeEnum)device.DeviceType;
                bool ret = false;

                switch (currDeviceType)
                {
                    case DeviceTypeEnum.Router:
                        {
                            string sql = string.Format("exec RemoveMerchRouter {0}", device.ID);
                            XCCloudRS232BLL.ExecuteSql(sql);

                            //ret = DeviceBusiness.UpdateDevice(device);
                        }
                        break;
                    case DeviceTypeEnum.SlotMachines:
                    case DeviceTypeEnum.DepositMachine:
                        {
                            //获取外设绑定关系实体
                            Data_MerchDevice md = MerchDeviceBusiness.GetMerchDeviceModel(device.ID);

                            using (var transactionScope = new System.Transactions.TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                DeviceBusiness.UpdateDevice(device);
                                ret =MerchDeviceBusiness.DeleteMerchDevice(md);

                                transactionScope.Complete();
                            }
                        }
                        break;
                    case DeviceTypeEnum.Clerk:
                    case DeviceTypeEnum.Terminal:
                        {
                            //获取终端绑定关系实体
                            Data_MerchSegment ms = MerchSegmentBusiness.GetMerchSegmentModel(device.ID);

                            using (var transactionScope = new System.Transactions.TransactionScope(TransactionScopeOption.RequiresNew))
                            {
                                DeviceBusiness.UpdateDevice(device);
                                ret = MerchSegmentBusiness.DeleteMerchSegment(ms);

                                transactionScope.Complete();
                            }
                        }
                        break;
                }

                if (ret)
                {
                    DeviceStatusBusiness.SetDeviceState(device.Token, DeviceStatusEnum.未激活.ToDescription());
                }

                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 查询设备信息
        /// <summary>
        /// 查询设备信息
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getMerchDeviceInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string mobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;
                string deviceToken = dicParas.ContainsKey("deviceToken") ? dicParas["deviceToken"].ToString() : string.Empty;

                Base_MerchInfo merch = MerchBusiness.GetMerchModel(mobileToken);
                if (merch.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户令牌无效");
                }

                Base_DeviceInfo device = DeviceBusiness.GetDeviceModel(deviceToken);
                if (device.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备令牌无效");
                }

                //设备所属商户不是当前商户
                if (device.MerchID != merch.ID)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "该设备属于其他商户，您没有权限查看该设备");
                }

                DeviceTypeEnum currDeviceType = (DeviceTypeEnum)device.DeviceType;

                DeviceInfoModel model = new DeviceInfoModel();
                model.Router = new Router();
                model.Group = new Group();
                model.deviceName = device.DeviceName ?? device.SN;
                model.deviceToken = device.Token;
                model.deviceType = currDeviceType.ToDescription();
                model.deviceSN = device.SN;
                model.status = DeviceStatusBusiness.GetDeviceState(device.Token);

                switch (currDeviceType)
                {
                    case DeviceTypeEnum.SlotMachines:
                    case DeviceTypeEnum.DepositMachine:
                        {
                            //获取外设绑定关系实体
                            Data_MerchDevice md = MerchDeviceBusiness.GetMerchDeviceModel(device.ID);
                            model.headAddress = md.HeadAddress;

                            //获取控制器实体
                            Base_DeviceInfo router = DeviceBusiness.GetDeviceModelById((int)md.ParentID);
                            model.Router.routerName = router.DeviceName ?? router.SN;
                            model.Router.routerToken = router.Token;
                            model.Router.sn = router.SN;
                        }
                        break;
                    case DeviceTypeEnum.Clerk:
                    case DeviceTypeEnum.Terminal:
                        {
                            //获取终端绑定关系实体
                            Data_MerchSegment ms = MerchSegmentBusiness.GetMerchSegmentModel(device.ID);
                            model.headAddress = ms.HeadAddress;

                            //获取分组实体
                            Data_GameInfo group = GameBusiness.GetGameInfoModel(Convert.ToInt32(ms.GroupID));
                            model.Group.groupId = group.GroupID;
                            model.Group.groupName = group.GroupName;
                            model.Group.groupType = ((GroupTypeEnum)(int)group.GroupType).ToDescription();
                        }
                        break;
                }
                return ResponseModelFactory<DeviceInfoModel>.CreateModel(isSignKeyReturn, model);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 修改设备状态
        /// <summary>
        /// 修改设备状态
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object updateDeviceState(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string mobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;
                string deviceToken = dicParas.ContainsKey("deviceToken") ? dicParas["deviceToken"].ToString() : string.Empty;
                string status = dicParas.ContainsKey("status") ? dicParas["status"].ToString() : string.Empty;

                Base_MerchInfo merch = MerchBusiness.GetMerchModel(mobileToken);
                if (merch.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户令牌无效");
                }

                Base_DeviceInfo device = DeviceBusiness.GetDeviceModel(deviceToken);
                if (device.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备令牌无效");
                }

                //设备所属商户不是当前商户
                if (device.MerchID != merch.ID)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "该设备属于其他商户，您没有权限查看该设备");
                }

                if (string.IsNullOrWhiteSpace(status))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "参数错误");
                }

                int routerId = 0;
                string strHeadAddress = string.Empty;

                //当前设备类型
                DeviceTypeEnum currDeviceType = (DeviceTypeEnum)device.DeviceType;
                switch (currDeviceType)
                {
                    case DeviceTypeEnum.SlotMachines:
                    case DeviceTypeEnum.DepositMachine:
                        {
                            //获取外设绑定关系实体
                            Data_MerchDevice md = MerchDeviceBusiness.GetMerchDeviceModel(device.ID);
                            if (md.IsNull())
                            {
                                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备未绑定");
                            }

                            //获取控制器ID
                            routerId = (int)md.ParentID;
                            strHeadAddress = md.HeadAddress;
                        }
                        break;
                    case DeviceTypeEnum.Clerk:
                    case DeviceTypeEnum.Terminal:
                        {
                            //获取终端绑定关系实体
                            Data_MerchSegment ms = MerchSegmentBusiness.GetMerchSegmentModel(device.ID);
                            if (ms.IsNull())
                            {
                                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备未绑定");
                            }

                            //获取控制器ID
                            routerId = (int)ms.ParentID;
                            strHeadAddress = ms.HeadAddress;
                        }
                        break;
                }

                Base_DeviceInfo router = null;

                if (routerId != 0)
                {
                    //获取当前设备所属的控制器实体
                    router = DeviceBusiness.GetDeviceModelById(routerId);
                }

                if (router == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "获取控制器信息失败");
                }

                if (string.IsNullOrWhiteSpace(strHeadAddress))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "获取设备地址失败");
                }

                //雷达处理指令
                switch (status.Trim())
                { 
                    //启用
                    case "1":
                        device.DeviceType = (int)DeviceStatusEnum.启用;
                        DeviceBusiness.UpdateDevice(device);
                        DeviceStatusBusiness.SetDeviceState(device.Token, DeviceStatusEnum.离线.ToDescription());
                        break;
                    //停用
                    case "2":
                        device.DeviceType = (int)DeviceStatusEnum.停用;
                        DeviceBusiness.UpdateDevice(device);
                        DeviceStatusBusiness.SetDeviceState(device.Token, DeviceStatusEnum.停用.ToDescription());
                        break;
                    //锁定
                    case "3":
                        UDPServer.server.机头锁定解锁指令(router.Token, strHeadAddress, true);
                        DeviceStatusBusiness.SetDeviceState(device.Token, DeviceStatusEnum.锁定.ToDescription());
                        break;
                    //解除锁定
                    case "4":
                        UDPServer.server.机头锁定解锁指令(router.Token, strHeadAddress, false);
                        DeviceStatusBusiness.SetDeviceState(device.Token, DeviceStatusEnum.在线.ToDescription());
                        break;
                    //解除报警
                    case "5": break;
                    ////投币
                    //case "6":
                    //    UDPServer.server.远程投币上分(router.Token, strHeadAddress, "10000010", 5);
                    //    break;
                    ////退币
                    //case "7":
                    //    UDPServer.server.远程退币(router.Token, strHeadAddress);
                    //    break;
                }

                //if (state == DeviceStatusEnum.停用 || state == DeviceStatusEnum.启用)
                //{
                //    device.Status = (int)state;
                //    DeviceBusiness.UpdateDevice(device);
                //}

                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 修改设备名称
        /// <summary>
        /// 修改设备名称
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object editDeviceName(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string mobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;
                string deviceToken = dicParas.ContainsKey("deviceToken") ? dicParas["deviceToken"].ToString() : string.Empty;
                string deviceName = dicParas.ContainsKey("deviceName") ? dicParas["deviceName"].ToString() : string.Empty;

                Base_MerchInfo merch = MerchBusiness.GetMerchModel(mobileToken);
                if (merch.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户令牌无效");
                }

                Base_DeviceInfo device = DeviceBusiness.GetDeviceModel(deviceToken);
                if (device.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备令牌无效");
                }

                //设备未绑定
                if (device.MerchID.IsNull() || device.MerchID == 0)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "该设备尚未绑定");
                }

                //设备所属商户不是当前商户
                if (device.MerchID.IsNull() || device.MerchID == 0 || device.MerchID != merch.ID)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "该设备属于其他商户，您没有权限查看该设备");
                }

                device.DeviceName = deviceName;
                DeviceBusiness.UpdateDevice(device);

                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 替换设备
        /// <summary>
        /// 替换设备
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object replaceDevice(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string mobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;
                string oldDeviceToken = dicParas.ContainsKey("oldDeviceToken") ? dicParas["oldDeviceToken"].ToString() : string.Empty;
                string newDeviceToken = dicParas.ContainsKey("newDeviceToken") ? dicParas["newDeviceToken"].ToString() : string.Empty;

                Base_MerchInfo merch = MerchBusiness.GetMerchModel(mobileToken);
                if (merch.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户令牌无效");
                }

                Base_DeviceInfo oldDevice = DeviceBusiness.GetDeviceModel(oldDeviceToken);
                if (oldDevice.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "您要替换的设备令牌无效");
                }

                //设备所属商户不是当前商户
                if (oldDevice.MerchID != merch.ID)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "该设备不属于当前商户，没有权限操作该设备");
                }

                //新设备实体
                Base_DeviceInfo newDevice = DeviceBusiness.GetDeviceModel(newDeviceToken);
                if (newDevice.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "当前扫面的设备令牌无效");
                }

                //判断设备类别是否相同
                if (oldDevice.DeviceType != newDevice.DeviceType)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备类型不同，不能替换");
                }

                DeviceTypeEnum currDeviceType = (DeviceTypeEnum)oldDevice.DeviceType;
                int category = 0;

                switch (currDeviceType)
                {
                    case DeviceTypeEnum.Router:
                        category = 1;
                        break;
                    case DeviceTypeEnum.SlotMachines:
                    case DeviceTypeEnum.DepositMachine:
                        category = 2;
                        break;
                    case DeviceTypeEnum.Clerk:
                    case DeviceTypeEnum.Terminal:
                        category = 3;
                        break;
                }

                if (category != 0)
                {
                    string sql = "exec ReplaceDevice @oldDeviceId,@newDeviceId,@merchId,@category";
                    SqlParameter[] parameters = new SqlParameter[4];
                    parameters[0] = new SqlParameter("@oldDeviceId", oldDevice.ID);
                    parameters[1] = new SqlParameter("@newDeviceId", newDevice.ID);
                    parameters[2] = new SqlParameter("@merchId", merch.ID);
                    parameters[3] = new SqlParameter("@category", category);
                    int ret = XCCloudRS232BLL.ExecuteSql(sql, parameters);
                    if (ret == 0)
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备替换失败");
                    }
                }

                //修改设备缓存状态
                DeviceStatusBusiness.SetDeviceState(oldDevice.Token, DeviceStatusEnum.未激活.ToDescription());
                DeviceStatusBusiness.SetDeviceState(newDevice.Token, DeviceStatusEnum.离线.ToDescription());

                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region "注册设备"
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object register(Dictionary<string, object> dicParas)
        {
            //MerchID,DeviceName,DeviceType,SN,Token,QRURL,Status,motor1,motor2,nixie_tube_type,motor2_coin,motor1_coin,FromDevice,ToCard,SSR,alert_value
            string deviceName = dicParas.ContainsKey("deviceName") ? dicParas["deviceName"].ToString() : string.Empty;
            string deviceType = dicParas.ContainsKey("deviceType") ? dicParas["deviceType"].ToString() : string.Empty;
            string sn = dicParas.ContainsKey("sn") ? dicParas["sn"].ToString() : string.Empty;
            string qrUrl = dicParas.ContainsKey("qrUrl") ? dicParas["qrUrl"].ToString() : string.Empty;
            string motor1 = dicParas.ContainsKey("motor1") ? dicParas["motor1"].ToString() : string.Empty;
            string motor2 = dicParas.ContainsKey("motor2") ? dicParas["motor2"].ToString() : string.Empty;
            string nixie_tube_type = dicParas.ContainsKey("nixie_tube_type") ? dicParas["nixie_tube_type"].ToString() : string.Empty;
            string motor2_coin = dicParas.ContainsKey("motor2_coin") ? dicParas["motor2_coin"].ToString() : string.Empty;
            string motor1_coin = dicParas.ContainsKey("motor1_coin") ? dicParas["motor1_coin"].ToString() : string.Empty;
            string fromDevice = dicParas.ContainsKey("fromDevice") ? dicParas["fromDevice"].ToString() : string.Empty;
            string toCard = dicParas.ContainsKey("toCard") ? dicParas["toCard"].ToString() : string.Empty;
            string ssr = dicParas.ContainsKey("ssr") ? dicParas["ssr"].ToString() : string.Empty;
            string alert_value = dicParas.ContainsKey("alert_value") ? dicParas["alert_value"].ToString() : string.Empty;

            if (int.Parse(deviceType) < 0 || int.Parse(deviceType) > 4)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备类型不正确");
            }

            XCCloudService.BLL.IBLL.XCCloudRS232.IDeviceService xcCloudRS232DeviceService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCCloudRS232.IDeviceService>();
            XCCloudService.BLL.IBLL.XCGameManager.IDeviceService xcGameManaDeviceService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGameManager.IDeviceService>();
            var xcCloudRS232DeviceModel = xcCloudRS232DeviceService.GetModels(p => p.SN.Equals(sn)).FirstOrDefault<Base_DeviceInfo>();
            if (xcCloudRS232DeviceModel == null)
            {
                string deviceToken = DeviceManaBusiness.GetDeviceToken();
                bool isExist = false;
                while (isExist == false)
                {
                    if (xcGameManaDeviceService.GetCount(p => p.TerminalNo.Equals(deviceToken)) == 0 && xcCloudRS232DeviceService.GetCount(p => p.SN.Equals(deviceToken)) == 0)
                    {
                        isExist = true;
                    }
                    else
                    {
                        deviceToken = DeviceManaBusiness.GetDeviceToken();
                    }
                    System.Threading.Thread.Sleep(100);
                }

                Base_DeviceInfo deviceModel = new Base_DeviceInfo();
                deviceModel.MerchID = 0;
                deviceModel.DeviceName = deviceName;
                deviceModel.DeviceType = int.Parse(deviceType);
                deviceModel.SN = sn;
                deviceModel.Token = deviceToken;
                deviceModel.QRURL = qrUrl;
                deviceModel.Status = 1;
                deviceModel.motor1 = int.Parse(motor1);
                deviceModel.motor2 = int.Parse(motor2);
                deviceModel.nixie_tube_type = int.Parse(nixie_tube_type);
                deviceModel.motor1_coin = int.Parse(motor1_coin);
                deviceModel.motor2_coin = int.Parse(motor2_coin);
                deviceModel.FromDevice = int.Parse(fromDevice);
                deviceModel.ToCard = int.Parse(toCard);
                deviceModel.SSR = int.Parse(ssr);
                deviceModel.alert_value = int.Parse(alert_value);
                xcCloudRS232DeviceService.Add(deviceModel);
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }
            else
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备信息已存在");
            } 
        }

        #endregion

        #region 查询控制器报警日志
        /// <summary>
        /// 查询控制器报警日志
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getRouterWarnLog(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string routerToken = dicParas.ContainsKey("routerToken") ? dicParas["routerToken"].ToString() : string.Empty;
                string deviceSN = dicParas.ContainsKey("deviceSN") ? dicParas["deviceSN"].ToString() : string.Empty;
                string headAddress = dicParas.ContainsKey("headAddress") ? dicParas["headAddress"].ToString() : string.Empty;
                string alertType = dicParas.ContainsKey("alertType") ? dicParas["alertType"].ToString() : string.Empty;
                string sDate = dicParas.ContainsKey("sDate") ? dicParas["sDate"].ToString() : string.Empty;
                string eDate = dicParas.ContainsKey("eDate") ? dicParas["eDate"].ToString() : string.Empty;
                string strPageIndex = dicParas.ContainsKey("pageIndex") ? dicParas["pageIndex"].ToString() : string.Empty;
                string strpageSize = dicParas.ContainsKey("pageSize") ? dicParas["pageSize"].ToString() : string.Empty;

                if (routerToken == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "控制器令牌不能为空");
                }

                int pageIndex = 1, pageSize = 10;

                if (!string.IsNullOrWhiteSpace(strPageIndex) && strPageIndex.IsInt())
                {
                    pageIndex = Convert.ToInt32(strPageIndex);
                }

                if (!string.IsNullOrWhiteSpace(strpageSize) && strpageSize.IsInt())
                {
                    pageSize = Convert.ToInt32(strpageSize);
                }

                DataTable table = DeviceBusiness.GetRouterWarnLog(routerToken, deviceSN, headAddress, alertType, sDate, eDate, pageIndex, pageSize);

                if (table.Rows.Count == 0)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "无数据");
                }

                var list = Utils.GetModelList<WarnLogModel>(table).ToList();
                return ResponseModelFactory<List<WarnLogModel>>.CreateModel(isSignKeyReturn, list);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion
    }
}