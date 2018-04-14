using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common;
using XCCloudService.Model.WeiXin.Message;
using XCCloudService.WeiXin.Common;

namespace XCCloudService.WeiXin.Message
{
    public class MessagePushFactory
    {
        public static string CreateMsgJson<TConfig, TData>(string openId, TConfig configModel, TData dataModel)
        {
            string SAppPagePath = Utils.GetPropertyValue(configModel, "SAppPagePath").ToString();
            string TemplateId = Utils.GetPropertyValue(configModel, "TemplateId").ToString();
            string DetailsUrl = Utils.GetPropertyValue(configModel, "DetailsUrl").ToString();
            string Params = string.Empty;

            if (!string.IsNullOrEmpty(SAppPagePath))
            {
                var miniprogram = new
                {
                    appid = WeiXinConfig.WXSmallAppId,
                    pagepath = SAppPagePath
                };
                
                var msgData = new
                {
                    touser = openId,
                    template_id = TemplateId,
                    data = GetMsgData<TConfig, TData>(configModel, dataModel, out Params),
                    url = DetailsUrl + (!string.IsNullOrEmpty(Params) ? ("?" + Params) : string.Empty),
                    miniprogram = miniprogram                
                };
                return Utils.SerializeObject(msgData).ToString();
            }
            else
            {
                var msgData = new
                {
                    touser = openId,
                    template_id = TemplateId,
                    data = GetMsgData<TConfig, TData>(configModel, dataModel, out Params),
                    url = DetailsUrl + (!string.IsNullOrEmpty(Params) ? ("?" + Params) : string.Empty)
                };
                return Utils.SerializeObject(msgData).ToString();
            }
        }


        private static object GetMsgData<TConfig, TData>(TConfig configModel, TData dataModel, out string uriParams)
        {
            uriParams = string.Empty;
            if (typeof(TConfig) == typeof(BuySuccessNotifyConfigModel))
            {
                return GetBuySuccessNotifyData(configModel, dataModel);
            }
            else if (typeof(TConfig) == typeof(UserRegisterRemindConfigModel))
            {
                return GetUserRegisterRemindData(configModel, dataModel, out uriParams);
            }
            else if (typeof(TConfig) == typeof(OrderPaySuccessConfigModel))
            {
                return GetOrderPaySuccessData(configModel, dataModel);
            }
            else if (typeof(TConfig) == typeof(OrderFailSuccessConfigModel))
            {
                return GetOrderFailData(configModel, dataModel);
            }
            else if (typeof(TConfig) == typeof(MerchNewPasswordConfigModel))
            {
                return GetMerchNewPasswordData(configModel, dataModel);
            }
            else if (typeof(TConfig) == typeof(MerchResetPasswordConfigModel))
            {
                return GetMerchResetPasswordData(configModel, dataModel);
            }
            else if (typeof(TConfig) == typeof(StoreRegisterRemindConfigModel))
            {
                return GetStoreRegisterRemindData(configModel, dataModel);
            }
            else if (typeof(TConfig) == typeof(XcUserNewPasswordConfigModel))
            {
                return GetXcUserNewPasswordData(configModel, dataModel);
            }
            else if (typeof(TConfig) == typeof(XcUserResetPasswordConfigModel))
            {
                return GetXcUserResetPasswordData(configModel, dataModel);
            }
            else if (typeof(TConfig) == typeof(XcGameGetCoinSuccessConfigModel))
            {
                return GetXcGameGetCoinSuccessData(configModel, dataModel);
            }
            return null;
        }


        private static object GetBuySuccessNotifyData<TConfig, TData>(TConfig configModel, TData dataModel)
        {
            BuySuccessNotifyConfigModel config = Utils.GetCopy<BuySuccessNotifyConfigModel>(configModel);
            BuySuccessNotifyDataModel data = Utils.GetCopy<BuySuccessNotifyDataModel>(dataModel);
            var msgData = new
            {
                first = new { value = config.Title, color = config.FirstColor },
                keynote1 = new { value = data.ProductName, color = config.Keynote1Color },
                keynote2 = new { value = data.BuyPrice, color = config.Keynote2Color },
                keynote3 = new { value = data.BuyDate, color = config.Keynote3Color },
                remark = new { value = config.Remark, color = config.RemarkColor }
            };

            return msgData;
        }

        private static object GetUserRegisterRemindData<TConfig, TData>(TConfig configModel, TData dataModel, out string uriParams)
        {
            uriParams = string.Empty;
            UserRegisterRemindConfigModel config = Utils.GetCopy<UserRegisterRemindConfigModel>(configModel);
            UserRegisterRemindDataModel data = Utils.GetCopy<UserRegisterRemindDataModel>(dataModel);
            var msgData = new
            {
                first = new { value = config.Title, color = config.FirstColor },
                keyword1 = new { value = data.UserName, color = config.Keynote1Color },
                keyword2 = new { value = data.RegisterTime, color = config.Keynote2Color },
                remark = new { value = "工单号：" + data.WorkId + "\n" + data.Message + "\n" + config.Remark, color = config.RemarkColor }
            };
            uriParams = string.Format("workId={0}&userName={1}&message={2}", Utils.UrlEncode(data.WorkId), Utils.UrlEncode(data.UserName), Utils.UrlEncode(data.Message));            
            return msgData;
        }

        private static object GetStoreRegisterRemindData<TConfig, TData>(TConfig configModel, TData dataModel)
        {
            StoreRegisterRemindConfigModel config = Utils.GetCopy<StoreRegisterRemindConfigModel>(configModel);
            StoreRegisterRemindDataModel data = Utils.GetCopy<StoreRegisterRemindDataModel>(dataModel);
            var msgData = new
            {
                first = new { value = config.Title, color = config.FirstColor },
                keyword1 = new { value = data.MerchAccount, color = config.Keynote1Color },
                keyword2 = new { value = data.RegisterTime, color = config.Keynote2Color },
                remark = new { value = "门店名称：" + data.StoreName + "\n工单号：" + data.WorkId + "\n" + config.Remark, color = config.RemarkColor }
            };

            return msgData;
        }

        private static object GetMerchNewPasswordData<TConfig, TData>(TConfig configModel, TData dataModel)
        {
            MerchNewPasswordConfigModel config = Utils.GetCopy<MerchNewPasswordConfigModel>(configModel);
            MerchNewPasswordDataModel data = Utils.GetCopy<MerchNewPasswordDataModel>(dataModel);
            var msgData = new
            {
                first = new { value = config.Title, color = config.FirstColor },
                keyword1 = new { value = data.UserName, color = config.Keynote1Color },
                keyword2 = new { value = data.Password, color = config.Keynote2Color },
                remark = new { value = config.Remark, color = config.RemarkColor }
            };
            return msgData;
        }

        private static object GetMerchResetPasswordData<TConfig, TData>(TConfig configModel, TData dataModel)
        {
            MerchResetPasswordConfigModel config = Utils.GetCopy<MerchResetPasswordConfigModel>(configModel);
            MerchResetPasswordDataModel data = Utils.GetCopy<MerchResetPasswordDataModel>(dataModel);
            var msgData = new
            {
                first = new { value = config.Title, color = config.FirstColor },
                keyword1 = new { value = data.UserName, color = config.Keynote1Color },
                keyword2 = new { value = data.Password, color = config.Keynote2Color },
                remark = new { value = config.Remark, color = config.RemarkColor }
            };
            return msgData;
        }

        private static object GetXcUserNewPasswordData<TConfig, TData>(TConfig configModel, TData dataModel)
        {
            XcUserNewPasswordConfigModel config = Utils.GetCopy<XcUserNewPasswordConfigModel>(configModel);
            XcUserNewPasswordDataModel data = Utils.GetCopy<XcUserNewPasswordDataModel>(dataModel);
            var msgData = new
            {
                first = new { value = config.Title, color = config.FirstColor },
                keyword1 = new { value = data.UserName, color = config.Keynote1Color },
                keyword2 = new { value = data.Password, color = config.Keynote2Color },
                remark = new { value = config.Remark, color = config.RemarkColor }
            };
            return msgData;
        }

        private static object GetXcUserResetPasswordData<TConfig, TData>(TConfig configModel, TData dataModel)
        {
            XcUserResetPasswordConfigModel config = Utils.GetCopy<XcUserResetPasswordConfigModel>(configModel);
            XcUserResetPasswordDataModel data = Utils.GetCopy<XcUserResetPasswordDataModel>(dataModel);
            var msgData = new
            {
                first = new { value = config.Title, color = config.FirstColor },
                keyword1 = new { value = data.UserName, color = config.Keynote1Color },
                keyword2 = new { value = data.Password, color = config.Keynote2Color },
                remark = new { value = config.Remark, color = config.RemarkColor }
            };
            return msgData;
        }

        private static object GetOrderPaySuccessData<TConfig, TData>(TConfig configModel, TData dataModel)
        {
            OrderPaySuccessConfigModel config = Utils.GetCopy<OrderPaySuccessConfigModel>(configModel);
            OrderPaySuccessDataModel data = Utils.GetCopy<OrderPaySuccessDataModel>(dataModel);
            var msgData = new
            {
                first = new { value = config.Title, color = config.FirstColor },
                keyword1 = new { value = data.ProductName, color = config.Keynote1Color },
                keyword2 = new { value = data.BuyPrice, color = config.Keynote2Color },
                keyword3 = new { value = data.BuyDate, color = config.Keynote3Color },
                keyword4 = new { value = data.Createtime, color = config.Keynote3Color },
                keyword5 = new { value = data.OrderNumber, color = config.Keynote3Color },
                remark = new { value = config.Remark, color = config.RemarkColor }
            };

            return msgData;
        }

        private static object GetOrderFailData<TConfig, TData>(TConfig configModel, TData dataModel)
        {
            OrderFailSuccessConfigModel config = Utils.GetCopy<OrderFailSuccessConfigModel>(configModel);
            OrderFailSuccessDataModel data = Utils.GetCopy<OrderFailSuccessDataModel>(dataModel);
            var msgData = new
            {
                first = new { value = config.Title, color = config.FirstColor },
                keyword1 = new { value = data.ProductName, color = config.Keynote1Color },
                keyword2 = new { value = data.BuyPrice, color = config.Keynote2Color },
                keyword3 = new { value = data.BuyDate, color = config.Keynote3Color },
                remark = new { value = config.Remark, color = config.RemarkColor }
            };

            return msgData;
        }

        private static object GetXcGameGetCoinSuccessData<TConfig, TData>(TConfig configModel, TData dataModel)
        {
            XcGameGetCoinSuccessConfigModel config = Utils.GetCopy<XcGameGetCoinSuccessConfigModel>(configModel);
            XcGameGetCoinSuccessDataModel data = Utils.GetCopy<XcGameGetCoinSuccessDataModel>(dataModel);
            var msgData = new
            {
                first = new { value = config.Title, color = config.FirstColor },
                keyword1 = new { value = string.Format("成功提币{0}个", data.Coins), color = config.Keynote1Color },
                keyword2 = new { value = data.OperationDate, color = config.Keynote2Color },
                remark = new { value = config.Remark, color = config.RemarkColor }
            };

            return msgData;
        }
    }
}
