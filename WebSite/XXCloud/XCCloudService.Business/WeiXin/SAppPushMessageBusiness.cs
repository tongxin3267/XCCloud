using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common.Enum;

namespace XCCloudService.Business.WeiXin
{

    

    public class SAppPushMessageBusiness
    {
        private static List<SAppPushMessageCacheModel> list = new List<SAppPushMessageCacheModel>();

        public static void Add(SAppPushMessageCacheModel model)
        {
            var tmpModel = list.Where<SAppPushMessageCacheModel>(p => p.Mobile.Equals(model.Mobile)).FirstOrDefault<SAppPushMessageCacheModel>();
            if (tmpModel == null)
            {
                list.Add(model);
            }
            else
            {
                tmpModel.FormId = model.FormId;
                tmpModel.OpenId = model.OpenId;
            }
        }


        public static bool GetModel(string orderId,ref SAppPushMessageCacheModel model)
        {
            var tmpModel = list.Where<SAppPushMessageCacheModel>(p => p.Mobile.Equals(orderId)).FirstOrDefault<SAppPushMessageCacheModel>();
            if (tmpModel != null)
            {
                model = tmpModel;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void Remove(string mobile)
        {
            var tmpModel = list.Where<SAppPushMessageCacheModel>(p => p.Mobile.Equals(mobile)).FirstOrDefault<SAppPushMessageCacheModel>();
            if (tmpModel != null)
            {
                list.Remove(tmpModel);
            }
        }
    }

    public class SAppPushMessageCacheModel
    {
        public SAppPushMessageCacheModel(string orderId,string mobile, string openId, string formId, string xmlFilePath, string sAppAccessToken,SAppMessageType sAppMessageType,object dataObj)
        {
            this.OrderId = orderId;
            this.Mobile = mobile;
            this.OpenId = openId;
            this.FormId = formId;
            this.XmlFilePath = xmlFilePath;
            this.SAppAccessToken = sAppAccessToken;
            this.DataObj = dataObj;
            this.SAppMessageType = sAppMessageType;
            this.CreateTime = System.DateTime.Now;
        }

        public string OrderId { set; get; }

        public string Mobile { set; get; }

        public string OpenId { set; get; }

        public string FormId { set; get; }

        public string XmlFilePath { set; get; }

        public string SAppAccessToken { set; get; }

        public object DataObj { set; get; }

        public SAppMessageType SAppMessageType { set; get; }

        public DateTime CreateTime { set; get; }
    }
}
