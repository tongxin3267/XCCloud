using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.Business.Common;
using XCCloudService.Business.XCGameMana;
using XCCloudService.Model.CustomModel.Common;
using XCCloudService.Model.CustomModel.XCGameManager;
using XCCloudService.Model.Socket.UDP;
using XCCloudService.SocketService.UDP.Factory;

namespace XXCloudService.Api.XCGameMana
{
    /// <summary>
    /// Query 的摘要说明
    /// </summary>
    public class Query : ApiBase
    {
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object GetStoreSales(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
            string date = dicParas.ContainsKey("date") ? dicParas["date"].ToString() : string.Empty;
            string icCardId = dicParas.ContainsKey("icCardId") ? dicParas["icCardId"].ToString() : string.Empty;
            string searchType = dicParas.ContainsKey("searchType") ? dicParas["searchType"].ToString() : string.Empty;

            //验证门店信息
            StoreCacheModel storeModel = null;
            StoreBusiness store = new StoreBusiness();
            if (!store.IsEffectiveStore(storeId, ref storeModel, out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
            }

            string sn = UDPSocketStoreQueryAnswerBusiness.GetSN();
            string radarToken = string.Empty;
            if (!DataFactory.SendDataStoreQuery(storeId, date, sn, searchType, icCardId, storeModel.StorePassword,out radarToken, out errMsg))
            {
                UDPSocketStoreQueryAnswerBusiness.AddAnswer(sn, storeId, radarToken);
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
            }

            int whileCount = 0;
            UDPSocketStoreQueryAnswerModel answerModel = null;
            while (answerModel == null && whileCount <= 25)
            {
                whileCount++;
                System.Threading.Thread.Sleep(1000);
                answerModel = UDPSocketStoreQueryAnswerBusiness.GetAnswerModel(sn,2); 
            }

            if (answerModel == null)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "系统没有响应");
            }

            List<StoreQueryResultNotifyTableData> list = (List<StoreQueryResultNotifyTableData>)(answerModel.Result);

            if (list == null || list.Count == 0)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "营业数据不存在");
            }
            else if (list[0].Key.Equals("查询结果") && list[0].Value.Equals("无此营业日期"))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "营业数据不存在");
            }
            else
            {
                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, list);
            }           
        }
    }
}