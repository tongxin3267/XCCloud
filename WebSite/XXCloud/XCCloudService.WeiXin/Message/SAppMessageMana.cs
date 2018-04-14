using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Business.Common;
using XCCloudService.Business.WeiXin;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.WeiXin.SAppMessage;
using XCCloudService.WeiXin.Common;
using XCCloudService.WeiXin.Config;
using XCCloudService.WeiXin.WeixinOAuth;

namespace XCCloudService.WeiXin.Message
{
    public class SAppMessageMana
    {
        private static bool Push<TConfig, TData>(string openId, string accessToken,string form_id, TData dataModel, out string errMsg)
        {
            errMsg = string.Empty;
            TConfig configModel = default(TConfig);
            if (!SAppMsgConfig.GetMsgTemplateConfig<TConfig>(ref configModel, CommonConfig.SAppMessagePushXmlFilePath))
            {
                errMsg = "读取微信消息配置文件出错";
                return false;
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                if (!SAppTokenMana.GetAccessToken(out accessToken, out errMsg))
                {
                    errMsg = "获取微信令牌出错";
                    return false;
                }
            }

            string json = SAppMessagePushFactory.CreateMsgJson<TConfig, TData>(openId, form_id, configModel, dataModel);
            string url = "https://api.weixin.qq.com/cgi-bin/message/wxopen/template/send?access_token={0}";
            url = string.Format(url, accessToken);
            string str = Utils.HttpPost(url, json);

            Dictionary<string, object> dict = new Dictionary<string, object>();
            if (WeiXinJsonHelper.GetResponseJsonResult(str, ref dict))
            {
                return true;
            }
            else
            {
                errMsg = dict["errmsg"].ToString();
                return false;
            }
        }

        public static bool SetMemberCoinsMsgCacheData(SAppMessageType sAppMessageType, string orderId, string form_id, string mobile, object dataObj, out string errMsg)
        {
            string openId = string.Empty;
            string accessToken = string.Empty;
            errMsg = string.Empty;
            try
            {
                if (MobileTokenBusiness.GetOpenId(mobile, out openId, out errMsg) && SAppTokenMana.GetAccessToken(out accessToken, out errMsg))
                {
                    SAppPushMessageCacheModel msgModel = new SAppPushMessageCacheModel(orderId, mobile, openId, form_id, CommonConfig.SAppMessagePushXmlFilePath, accessToken, sAppMessageType, dataObj);
                    SAppPushMessageBusiness.Add(msgModel);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool PushMemberFoodSaleMsg(string openId, string accessToken, string buyType, string storeName, string mobile, string orderId, string foodName, int foodNum, int icCardId, decimal money, int coins, string form_id, out string errMsg)
        {
            errMsg = string.Empty;
            try
            {
                MemberFoodSaleDataModel dataModel = new MemberFoodSaleDataModel();
                dataModel.StoreName = storeName;
                dataModel.BuyDate = System.DateTime.Now.ToString("yyyy年MM月dd日");
                dataModel.OrderId = orderId;
                dataModel.FoodName = foodName;
                dataModel.FoodNum = foodNum;
                dataModel.BuyMobile = mobile;
                dataModel.BuyAmmount = money;
                dataModel.Remark = buyType;
                return SAppMessageMana.Push<MemberFoodSaleConfigModel, MemberFoodSaleDataModel>(openId, accessToken,form_id, dataModel, out errMsg);    
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return false;
            }
        }

        public static bool PushMemberCoinsMsg(string openId,string accessToken,string coinsOperationType, string storeName, string mobile, int icCardId, int coins, int balance, string form_id, string remark, out string errMsg)
        {
            errMsg = string.Empty;
            try
            {
                MemberCoinsDataModel dataModel = new MemberCoinsDataModel();
                dataModel.StoreName = storeName;
                dataModel.Date = System.DateTime.Now.ToString("yyyy年MM月dd日");
                dataModel.Coins = coins;
                dataModel.Type = coinsOperationType;
                dataModel.Mobile = mobile;
                dataModel.Balance = balance;
                dataModel.Remark = remark;
                return SAppMessageMana.Push<MemberCoinsConfigModel, MemberCoinsDataModel>(openId,accessToken,form_id,dataModel,out errMsg);
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return false;
            }
        }    
    }
}
