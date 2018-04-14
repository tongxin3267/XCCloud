using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCGameManager;

namespace XXCloudService.Api.XCGameMana
{
    /// <summary>
    /// Promotion 的摘要说明
    /// </summary>
    public class Promotion : ApiBase
    {
        
        /// <summary>
        /// 获取新闻
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getPromotion(Dictionary<string, object> dicParas)
        {
            try
            {
                string sql = "select ID,StoreName,StoreID,CONVERT(varchar(100), [Time], 23) as [Time],CONVERT(varchar(100), [ReleaseTime], 23) as [ReleaseTime],PicturePath,Title,PagePath,Publisher,PublishType ,PromotionType  from t_promotion where State='1'";
                System.Data.DataSet ds = XCGameManabll.ExecuteQuerySentence(sql, null);
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count == 0)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "无数据");
                }
                var list = Utils.GetModelList<PromotionModel>(ds.Tables[0]).ToList();
                return ResponseModelFactory<List<PromotionModel>>.CreateModel(isSignKeyReturn, list);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}