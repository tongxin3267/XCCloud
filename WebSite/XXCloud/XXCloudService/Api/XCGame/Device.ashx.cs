using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGame;
using XCCloudService.Business.Common;
using XCCloudService.Business.XCGame;
using XCCloudService.Business.XCGameMana;
using XCCloudService.Common;
using XCCloudService.Model;
using XCCloudService.Model.XCGame;
using XCCloudService.ResponseModels;

namespace XCCloudService.Api.XCGame
{
    /// <summary>
    /// Member 的摘要说明
    /// </summary>
    public class Device : ApiBase
    {

        /// <summary>
        /// 维护设备状态，给雷达调用
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object updateDeviceState(Dictionary<string, object> dicParas)
        {
            string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
            string deviceId = dicParas.ContainsKey("deviceId") ? dicParas["deviceId"].ToString() : string.Empty;
            string token = dicParas.ContainsKey("token") ? dicParas["token"].ToString() : string.Empty;
            string state = dicParas.ContainsKey("state") ? dicParas["state"].ToString() : string.Empty;

            string dbName = string.Empty;
            string password = string.Empty;
            string errMsg = string.Empty;
            StoreBusiness storeBusiness = new StoreBusiness();
            if (!storeBusiness.IsEffectiveStore(storeId, out dbName, out password, out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店不存在");
            }

            DeviceBusiness deviceBusiness = new DeviceBusiness();
            if (!deviceBusiness.IsEffectiveStoreDevice(dbName,deviceId,out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店不存在");
            }

            //验证MD5
            string md5token = Utils.MD5(storeId + deviceId + password);
            if (!token.Equals(md5token))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "token不正确");
            }

            //设置设备状态
            DeviceStateBusiness.SetDeviceState(storeId, deviceId, state);

            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
        }
    }
}