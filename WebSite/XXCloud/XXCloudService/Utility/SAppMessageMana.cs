using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Business.Common;
using XCCloudService.Business.WeiXin;
using XCCloudService.Common.Enum;
using XCCloudService.Model.WeiXin.SAppMessage;
using XCCloudService.WeiXin.Message;
using XCCloudService.WeiXin.WeixinOAuth;

namespace XXCloudService.Utility
{
    public class SAppMessagePushMana
    {
        public static bool PushMemberFoodSaleMsg(string storeName, string mobile, string orderId, string foodName, int foodNum, int icCardId, decimal money, string buyType, int coins, string form_id, out string errMsg)
        {
            errMsg = string.Empty;
            try
            {
                string openId = string.Empty;
                if (MobileTokenBusiness.GetOpenId(mobile, out openId, out errMsg))
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
                    return SAppMessageMana.Push<MemberFoodSaleConfigModel, MemberFoodSaleDataModel>(openId, form_id, dataModel, out errMsg);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return false;
            }
        }


        public  static bool PushMemberCoinsMsg(string coinsType ,string storeName, string mobile, int icCardId, int coins,int balance, string form_id,string remark, out string errMsg)
        {
            errMsg = string.Empty;
            try
            {
                string openId = string.Empty;
                if (MobileTokenBusiness.GetOpenId(mobile, out openId, out errMsg))
                {
                    MemberCoinsDataModel dataModel = new MemberCoinsDataModel();
                    dataModel.StoreName = storeName;
                    dataModel.Date = System.DateTime.Now.ToString("yyyy年MM月dd日");
                    dataModel.Coins = coins;
                    dataModel.Type = coinsType;
                    dataModel.Mobile = mobile;
                    dataModel.Balance = balance;
                    dataModel.Remark = remark;
                    return SAppMessageMana.Push<MemberCoinsConfigModel, MemberCoinsDataModel>(openId, form_id, dataModel, out errMsg);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return false;
            }
        }


        public static bool SetMemberCoinsMsgCacheData(string orderId,string form_id,string mobile,SAppMessageType sAppMessageType,object dataObj,out string errMsg)
        {
            string openId = string.Empty;
            string accessToken = string.Empty;
            errMsg = string.Empty;
            try
            {
                if (MobileTokenBusiness.GetOpenId(mobile, out openId, out errMsg) && SAppTokenMana.GetAccessToken(out accessToken, out errMsg))
                {
                    string xmlFilePath = HttpContext.Current.Server.MapPath("/Config/SAppMessageTemplate.xml");
                    SAppPushMessageCacheModel msgModel = new SAppPushMessageCacheModel(orderId,mobile, openId, form_id, xmlFilePath, accessToken, sAppMessageType, dataObj);
                    SAppPushMessageBusiness.Add(msgModel);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}