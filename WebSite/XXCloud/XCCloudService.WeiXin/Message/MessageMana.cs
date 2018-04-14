using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.WeiXin.Message;
using XCCloudService.WeiXin.Common;
using XCCloudService.WeiXin.Config;
using XCCloudService.WeiXin.WeixinOAuth;

namespace XCCloudService.WeiXin.Message
{
    public class MessageMana
    {
        public static bool PushMessage(WeiXinMesageType messageType,string openId,object dataModel,out string errMsg)
        {
            errMsg = "推送类型无效";
            switch (messageType)
            {
                case WeiXinMesageType.BuySuccessNotify: return Push<BuySuccessNotifyConfigModel, BuySuccessNotifyDataModel>(openId, (BuySuccessNotifyDataModel)dataModel, out errMsg);
                case WeiXinMesageType.OrderPaySuccess: return Push<OrderPaySuccessConfigModel, OrderPaySuccessDataModel>(openId, (OrderPaySuccessDataModel)dataModel, out errMsg);
                case WeiXinMesageType.OrderFailSuccess: return Push<OrderFailSuccessConfigModel, OrderFailSuccessDataModel>(openId, (OrderFailSuccessDataModel)dataModel, out errMsg);
                case WeiXinMesageType.UserRegisterRemind: return Push<UserRegisterRemindConfigModel, UserRegisterRemindDataModel>(openId, (UserRegisterRemindDataModel)dataModel, out errMsg);
                case WeiXinMesageType.MerchNewPassword: return Push<MerchNewPasswordConfigModel, MerchNewPasswordDataModel>(openId, (MerchNewPasswordDataModel)dataModel, out errMsg);
                case WeiXinMesageType.StoreRegisterRemind: return Push<StoreRegisterRemindConfigModel, StoreRegisterRemindDataModel>(openId, (StoreRegisterRemindDataModel)dataModel, out errMsg);
                case WeiXinMesageType.MerchResetPassword: return Push<MerchResetPasswordConfigModel, MerchResetPasswordDataModel>(openId, (MerchResetPasswordDataModel)dataModel, out errMsg);
                case WeiXinMesageType.XcUserNewPassword: return Push<XcUserNewPasswordConfigModel, XcUserNewPasswordDataModel>(openId, (XcUserNewPasswordDataModel)dataModel, out errMsg);
                case WeiXinMesageType.XcUserResetPassword: return Push<XcUserResetPasswordConfigModel, XcUserResetPasswordDataModel>(openId, (XcUserResetPasswordDataModel)dataModel, out errMsg);
                case WeiXinMesageType.XCGameGetCoinSuccess: return Push<XcGameGetCoinSuccessConfigModel, XcGameGetCoinSuccessDataModel>(openId, (XcGameGetCoinSuccessDataModel)dataModel, out errMsg);
                default: return false;
            }
        }

        public static bool Push<TConfig,TData>(string openId,TData dataModel,out string errMsg)
        {
            errMsg = string.Empty;
            TConfig configModel = default(TConfig);
            if (!WeiXinMsgConfig.GetMsgTemplateConfig<TConfig>(ref configModel))
            {
                errMsg = "读取微信消息配置文件出错";
                return false;
            }

            string accessToken = string.Empty;
            if (TokenMana.GetAccessToken(out accessToken))
            {
                string json = MessagePushFactory.CreateMsgJson<TConfig, TData>(openId, configModel, dataModel);
                string url = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}";
                url = string.Format(url, accessToken);
                string str = Utils.HttpPost(url, json);
                return true;
            }
            else
            {
                errMsg = "获取微信令牌出错";
                return false;
            }
        }



        //public String show(string OpenID)
        //{
        //    string color = "#173177";
        //    var First = new { value = "恭喜你购买成功!", color = color };
        //    var Remark = new { value = "欢迎再次购买！", color = color };
        //    var AppId = "";
        //    var pagepath = "";
        //    var Data = new
        //    {
        //        name = First,
        //        remark = Remark
        //    };
        //    var miniprogram = new
        //    {
        //        appid = AppId,
        //        pagepath = pagepath
        //    };
        //    string accessToken = string.Empty;
        //    if (TokenMana.GetAccessToken(out accessToken))
        //    {
        //        //string lis = "https://api.weixin.qq.com/cgi-bin/template/get_all_private_template?access_token={0}";
        //        //lis = string.Format(lis, accessToken);
        //        //string aa = Utils.WebClientDownloadString(lis);
        //        LogHeler.SaveLog("template_list:" + aa);
        //        string Urls = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}";
        //        Urls = string.Format(Urls, accessToken);
        //        var msgData = new
        //        {
        //            touser = OpenID,
        //            template_id = "18k5i1CRs9Z9mHDBYjvDjucj1oC9NTHV7GjlmBUTOWo",
        //            url = "http://weixin.qq.com/download",
        //            miniprogram = miniprogram,
        //            data = Data
        //        };
        //        string data1 = Utils.SerializeObject(msgData).ToString();
        //        string str = Utils.HttpPost(Urls, data1);
        //        LogHeler.SaveLog("str请求:" + str);
              
        //    }
        //    return "";
        //}

        //public String checkshow(string fromOpenId, string toOpenId, string username, string regtime, string workId)
        //{
        //    string color = "#173177";            
        //    var AppId = "";
        //    var pagepath = "";
        //    var First = new { value = "", color = color };
        //    var Keynote1 = new { value = username, color = color };
        //    var Keynote2 = new { value = regtime, color = color };
        //    var Keynote3 = new { value = workId, color = color };
        //    var Remark = new { value = "", color = color };
        //    var Data = new
        //    {
        //        first = First,
        //        keynote1 = Keynote1,
        //        keynote2 = Keynote2,
        //        keynote3 = Keynote3,
        //        remark = Remark
        //    };
        //    var miniprogram = new
        //    {
        //        appid = AppId,
        //        pagepath = pagepath
        //    };
        //    string accessToken = string.Empty;
        //    if (TokenMana.GetAccessToken(out accessToken))
        //    {                
        //        string Urls = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}";
        //        Urls = string.Format(Urls, accessToken);
        //        var msgData = new
        //        {
        //            touser = toOpenId,
        //            template_id = "FXplFqR4Mo55im--hfYC0Hg-U_8IIwns_bc5RR0__gs",
        //            url = string.Format("{0}/WeiXin/Api/Register?action=checkUser&workId={1}", WeiXinConfig.WeiXinHttpsHostUrl, workId),
        //            miniprogram = miniprogram,
        //            data = Data
        //        };
        //        string param = Utils.SerializeObject(msgData).ToString();
        //        string str = Utils.HttpPost(Urls, param);
        //        LogHeler.SaveLog("审批请求返回:" + str);

        //    }
        //    return "";
        //}

    }

}
