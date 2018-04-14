using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.Business.Common;
using XCCloudService.Business.XCGameMana;
using XCCloudService.Model.CustomModel.Common;
using XCCloudService.Model.CustomModel.XCGame;
using XCCloudService.Model.CustomModel.XCGameManager;
using XCCloudService.Model.Socket.UDP;
using XCCloudService.SocketService.UDP;
using XCCloudService.SocketService.UDP.Factory;

namespace XXCloudService.Utility
{
    public class UDPApiService
    {
        public static bool GetParam(string storeId, string requestType, ref ParamQueryResultModel paramQueryResultModel, out string errMsg)
        {
            StoreBusiness storeBusiness = new StoreBusiness();
            StoreCacheModel storeCacheModel = null;
            if (!storeBusiness.IsEffectiveStore(storeId, ref storeCacheModel, out errMsg))
            {
                return false;
            }

            string sn = System.Guid.NewGuid().ToString().Replace("-", "");
            UDPSocketCommonQueryAnswerModel answerModel = null;
            string radarToken = string.Empty;
            if (DataFactory.SendDataParamQuery(sn,storeId, storeCacheModel.StorePassword, requestType,out radarToken,out errMsg))
            {

            }
            else
            {
                return false;
            }

            answerModel = null;
            while (answerModel == null)
            {
                System.Threading.Thread.Sleep(1000);
                answerModel = UDPSocketCommonQueryAnswerBusiness.GetAnswerModel(sn, 1);
            }

            if (answerModel != null)
            {
                ParamQueryResultNotifyRequestModel model = (ParamQueryResultNotifyRequestModel)(answerModel.Result);
                //移除应答缓存数据
                UDPSocketCommonQueryAnswerBusiness.Remove(sn);

                if (model.Result_Code == "1")
                {
                    paramQueryResultModel = model.Result_Data;
                    return true;
                }
                else
                {
                    errMsg = model.Result_Msg;
                    return false;
                }
            }
            else
            {
                errMsg = "系统没有响应";
                return false;
            }
        }
    }
}