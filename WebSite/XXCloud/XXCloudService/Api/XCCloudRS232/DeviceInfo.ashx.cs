using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.Business.Common;
using XCCloudService.Business.XCCloudRS232;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Common.Extensions;
using XCCloudService.Model.CustomModel.XCCloudRS232;
using XCCloudService.Model.XCCloudRS232;


namespace XXCloudService.Api.XCCloudRS232
{
    /// <summary>
    /// DeviceInfo 的摘要说明
    /// </summary>
    public class DeviceInfo : ApiBase
    {
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getDeviceInfo(Dictionary<string, object> dicParas)
        {
            try
            {
//                string MobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;//获取手机令牌
//                if (MobileToken == "")
//                {
//                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机令牌不能为空");
//                }
//                string mobile = string.Empty;
//                if (!MobileTokenBusiness.ExistToken(MobileToken, out mobile))
//                {
//                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机token无效");
//                }
//                mobile = mobile.Replace("RS232", "");
//                string sql = "";
//                sql = @"select b.SN,b.Token,b.DeviceName,b.Status from Base_DeviceInfo as b
// left join Base_MerchInfo as a on a.ID=b.MerchID  where a.State='1'  and DeviceType='0' and a.Mobile='" + mobile + "'";
//                DataSet ds = XCCloudRS232BLL.ExecuteQuerySentence(sql, null);
//                DataTable dt = ds.Tables[0];
//                if (dt.Rows.Count > 0)
//                {
//                    var basedeviceinfolist = Utils.GetModelList<BaseDeviceInfoModel>(dt).ToList();
//                    BaseDeviceInfoModelList baseDeviceInfoModelList = new BaseDeviceInfoModelList();
//                    baseDeviceInfoModelList.Lists = basedeviceinfolist;
//                    return ResponseModelFactory<BaseDeviceInfoModelList>.CreateModel(isSignKeyReturn, baseDeviceInfoModelList);
//                }
                string mobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;
                Base_MerchInfo merch = MerchBusiness.GetMerchModel(mobileToken);
                if (merch.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户令牌无效");
                }

                var list = DeviceBusiness.GetDeviceList().Where(t => t.MerchID == merch.ID && (DeviceTypeEnum)(int)t.DeviceType == DeviceTypeEnum.Router && t.Status == 1).ToList();
                if (list.IsNull() || list.Count == 0)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "无数据");
                }

                BaseDeviceInfoModelList baseDeviceInfoModelList = new BaseDeviceInfoModelList();
                baseDeviceInfoModelList.Lists = new List<BaseDeviceInfoModel>();
                foreach (var item in list)
                {
                    BaseDeviceInfoModel model = new BaseDeviceInfoModel();
                    model.DeviceName = item.DeviceName ?? item.SN;
                    model.SN = item.SN;
                    model.Token = item.Token;
                    model.Status = DeviceStatusBusiness.GetDeviceState(item.Token);

                    baseDeviceInfoModelList.Lists.Add(model);
                }

                return ResponseModelFactory<BaseDeviceInfoModelList>.CreateModel(isSignKeyReturn, baseDeviceInfoModelList);

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #region 获取控制器中的外设及分组列表
        /// <summary>
        /// 获取控制器中的外设及分组列表
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getRouterChildList(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string mobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;
                string routerToken = dicParas.ContainsKey("routerToken") ? dicParas["routerToken"].ToString() : string.Empty;

                Base_MerchInfo merch = MerchBusiness.GetMerchModel(mobileToken);
                if (merch.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户token无效");
                }

                Base_DeviceInfo router = DeviceBusiness.GetDeviceModel(routerToken);
                if (router.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "控制器token无效");
                }

                //分组集合
                var groupList = GameBusiness.GetGameList().Where(t => t.DeviceID == router.ID).ToList();

                //外设集合
                var peripheralList = MerchDeviceBusiness.GetListByParentId(router.ID).ToList();

                RouterModel model = new RouterModel();
                model.routerName = string.IsNullOrWhiteSpace(router.DeviceName) ? router.SN : router.DeviceName;
                model.routerToken = router.Token;
                model.routerStatus = DeviceStatusBusiness.GetDeviceState(router.Token);
                model.routerSN = router.SN;
                model.Groups = groupList.Select(t => new Group
                {
                    groupId = t.GroupID,
                    groupName = t.GroupName,
                    groupType = ((GroupTypeEnum)t.GroupType).ToDescription()
                }).ToList();

                List<Peripheral> Peripherals = new List<Peripheral>();
                foreach (var item in peripheralList)
                {
                    Peripheral p = new Peripheral();
                    Base_DeviceInfo cDevice = DeviceBusiness.GetDeviceModelById((int)item.DeviceID);
                    p.peripheralId = (int)item.DeviceID;
                    p.peripheralName = cDevice.DeviceName;
                    p.peripheralToken = cDevice.Token;
                    p.sn = cDevice.SN;
                    p.deviceType = ((DeviceTypeEnum)cDevice.DeviceType).ToDescription();
                    p.state = DeviceStatusBusiness.GetDeviceState(cDevice.Token);
                    p.headAddress = item.HeadAddress;

                    Peripherals.Add(p);
                }
                model.Peripherals = Peripherals;

                return ResponseModelFactory<RouterModel>.CreateModel(isSignKeyReturn, model);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 获取分组中的终端列表
        /// <summary>
        /// 获取分组中的终端列表
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getSegmentList(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string mobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;
                string routerToken = dicParas.ContainsKey("routerToken") ? dicParas["routerToken"].ToString() : string.Empty;
                string groupId = dicParas.ContainsKey("groupId") ? dicParas["groupId"].ToString() : string.Empty;

                Base_MerchInfo merch = MerchBusiness.GetMerchModel(mobileToken);
                if (merch.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户令牌无效");
                }

                Base_DeviceInfo router = DeviceBusiness.GetDeviceModel(routerToken);
                if (router.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "控制器令牌无效");
                }

                Data_GameInfo group = GameBusiness.GetGameInfoModel(Convert.ToInt32(groupId));
                if (group.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "分组参数无效");
                }

                //分组集合
                var groupList = MerchSegmentBusiness.GetMerchSegmentList().Where(t => t.ParentID == router.ID && t.GroupID == group.GroupID).ToList();

                GroupInfoModel model = new GroupInfoModel();
                model.groupId = group.GroupID;
                model.groupName = group.GroupName;

                List<Terminal> Terminals = new List<Terminal>();
                foreach (var item in groupList)
                {
                    Terminal t = new Terminal();
                    Base_DeviceInfo cDevice = DeviceBusiness.GetDeviceModelById((int)item.DeviceID);
                    t.terminalName = cDevice.DeviceName ?? cDevice.SN;
                    t.terminalToken = cDevice.Token;
                    t.headAddress = item.HeadAddress;
                    t.sn = cDevice.SN;
                    t.deviceType = ((DeviceTypeEnum)cDevice.DeviceType).ToDescription();
                    t.status = DeviceStatusBusiness.GetDeviceState(cDevice.Token);

                    Terminals.Add(t);
                }
                model.Terminals = Terminals;

                return ResponseModelFactory<GroupInfoModel>.CreateModel(isSignKeyReturn, model);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion
    }
}