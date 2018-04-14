using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.Business;
using XCCloudService.Business.Common;
using XCCloudService.Business.XCGame;
using XCCloudService.Business.XCGameMana;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCGame;
using XCCloudService.Model.CustomModel.XCGameManager;
using XCCloudService.Model.XCGameManager;
using XCCloudService.Pay;
using XCCloudService.WeiXin.Common;

namespace XXCloudService.Api.XCGameMana
{
    /// <summary>
    /// User 的摘要说明
    /// </summary>
    public class WSOrder : ApiBase
    {
        
        /// <summary>
        /// 查询手机号码发送短信验证码
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object payRequest(Dictionary<string, object> dicParas)
        {
            string deviceToken = dicParas.ContainsKey("deviceToken") ? dicParas["deviceToken"].ToString() : string.Empty;
            string memberToken = dicParas.ContainsKey("memberToken") ? dicParas["memberToken"].ToString() : string.Empty;
            string orderTip = dicParas.ContainsKey("orderTip") ? dicParas["orderTip"].ToString() : string.Empty;
            string orderAmountStr = dicParas.ContainsKey("orderAmount") ? dicParas["orderAmount"].ToString() : string.Empty;
            decimal orderAmount = 0;
            string storeId = string.Empty;
            if (!string.IsNullOrEmpty(deviceToken))
            {
                if (!DeviceManaBusiness.ExistDevice(deviceToken, out storeId))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备不存在");
                }
            }
            else if (!string.IsNullOrEmpty(memberToken))
            {
                XCGameMemberTokenModel model = MemberTokenBusiness.GetMemberTokenModel(memberToken);
                if (model == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "会员令牌无效");
                }
                storeId = model.StoreId;
            }
            else
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "会员令牌或设备令牌无效");
            }

            if (string.IsNullOrEmpty(orderTip))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "订单标题无效");
            }

            if (!decimal.TryParse(orderAmountStr, out orderAmount))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "订单金额无效");
            }

            string orderNo = "";
            string timeStamp = Utils.ConvertDateTimeToLong(System.DateTime.Now, 0).ToString();
            string nonceStr = Utils.GetGuid();
            string prepay_id = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            string package = "prepay_id=" + prepay_id;
            string signType = "MD5";
            string paySignStr = string.Format("appId={0}&nonceStr={1}&package={2}&signType={3}&timeStamp={4}&key={5}",
                WeiXinConfig.WXSmallAppId, nonceStr, package, signType, timeStamp, WeiXinConfig.WXSmallAppSecret);
            //paySign = MD5(appId=wxd678efh567hg6787&nonceStr=5K8264ILTKCH16CQ2502SI8ZNMTM67VS&package=prepay_id=wx2017033010242291fcfe0db70013231072&signType=MD5&timeStamp=1490840662&key=qazwsxedcrfvtgbyhnujmikolp111111) = 22D9B4E54AB1950F51E0649E8810ACD6
            string paySign = Utils.MD5(paySignStr);
            var data = new
            {
                orderNo = orderNo,
                timeStamp = timeStamp,
                nonceStr = nonceStr,
                package = package,
                signType = signType,
                paySign = paySign
            };
            var resObj = new
            {
                reutrn_code = "1",
                return_msg = "",
                result_code = "1",
                result_msg = "",
                data = data
            };
            return resObj;
        }

    }
}