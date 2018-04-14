using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common;
using XCCloudService.Model.WeiXin.SAppMessage;
using XCCloudService.WeiXin.Common;

namespace XCCloudService.WeiXin.Message
{
    public class SAppMessagePushFactory
    {
        public static string CreateMsgJson<TConfig, TData>(string openId, string form_id, TConfig configModel, TData dataModel)
        {
            string TemplateId = Utils.GetPropertyValue(configModel, "TemplateId").ToString();
            string DetailsUrl = Utils.GetPropertyValue(configModel, "DetailsUrl").ToString();
            string Params = string.Empty;

            if (!string.IsNullOrEmpty(DetailsUrl))
            {        
                var msgData = new
                {
                    touser = openId,
                    template_id = TemplateId,
                    page = DetailsUrl,
                    form_id = form_id,
                    data = GetMsgData<TConfig, TData>(configModel, dataModel, out Params)       
                };
                return Utils.SerializeObject(msgData).ToString();
            }
            else
            {
                var msgData = new
                {
                    touser = openId,
                    template_id = TemplateId,
                    form_id = form_id,
                    data = GetMsgData<TConfig, TData>(configModel, dataModel, out Params),
                };
                return Utils.SerializeObject(msgData).ToString();
            }
        }


        private static object GetMsgData<TConfig, TData>(TConfig configModel, TData dataModel, out string uriParams)
        {
            uriParams = string.Empty;
            if (typeof(TConfig) == typeof(MemberFoodSaleConfigModel))
            {
                return GetMemberFoodSaleData(configModel, dataModel);
            }
            else if (typeof(TConfig) == typeof(MemberCoinsConfigModel))
            {
                return GetMemberCoinsData(configModel, dataModel);
            }
            return null;
        }


        private static object GetMemberFoodSaleData<TConfig, TData>(TConfig configModel, TData dataModel)
        {
            MemberFoodSaleConfigModel config = Utils.GetCopy<MemberFoodSaleConfigModel>(configModel);
            MemberFoodSaleDataModel data = Utils.GetCopy<MemberFoodSaleDataModel>(dataModel);
            var msgData = new
            {
                keyword1 = new { value = data.StoreName},
                keyword2 = new { value = data.BuyDate},
                keyword3 = new { value = data.OrderId },
                keyword4 = new { value = data.FoodName },
                keyword5 = new { value = data.FoodNum.ToString() },
                keyword6 = new { value = data.BuyMobile},
                keyword7 = new { value = data.BuyAmmount.ToString("0.00") },
                keyword8 = new { value = data.Remark}
            };
            return msgData;
        }

        private static object GetMemberCoinsData<TConfig, TData>(TConfig configModel, TData dataModel)
        {
            MemberCoinsConfigModel config = Utils.GetCopy<MemberCoinsConfigModel>(configModel);
            MemberCoinsDataModel data = Utils.GetCopy<MemberCoinsDataModel>(dataModel);
            var msgData = new
            {
                keyword1 = new { value = data.StoreName },
                keyword2 = new { value = data.Type },
                keyword3 = new { value = 0 },
                keyword4 = new { value = data.Date },
                keyword5 = new { value = data.Coins },
                keyword6 = new { value = data.Balance+"币" },
                keyword7 = new { value = data.Mobile },
                keyword8 = new { value = data.Remark }
            };

            return msgData;
        }

    }
}
