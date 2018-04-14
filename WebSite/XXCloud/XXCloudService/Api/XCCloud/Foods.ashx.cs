using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.Business.XCCloud;
using XCCloudService.Business.XCGameMana;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCCloud;

namespace XXCloudService.Api.XCCloud
{
    /// <summary>
    /// Foods 的摘要说明
    /// </summary>
    [Authorize(Roles = "StoreUser")]
    public class Foods : ApiBase
    {
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object getMemberOpenCardFoodInfo(Dictionary<string, object> dicParas)
        {
            XCCloudUserTokenModel userTokenModel = (XCCloudUserTokenModel)(dicParas[Constant.XCCloudUserTokenModel]);
            StoreIDDataModel userTokenDataModel = (StoreIDDataModel)(userTokenModel.DataModel);
            string MemberLevelId = dicParas.ContainsKey("memberLevelId") ? dicParas["memberLevelId"].ToString() : string.Empty;

            string sql = "exec GetMemberOpenCardFoodInfo @StoreId,@MemberLevelId";
            SqlParameter[] parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("@StoreId", userTokenDataModel.StoreId);
            parameters[1] = new SqlParameter("@MemberLevelId", MemberLevelId);
            System.Data.DataSet ds = XCCloudBLL.ExecuteQuerySentence(sql, parameters);
            DataTable dt = ds.Tables[0];
            
            if (dt.Rows.Count > 0)
            {
                List<OpenCardFoodInfoModel> list1 = Utils.GetModelList<OpenCardFoodInfoModel>(ds.Tables[0]).ToList();
                //for (int i = 0; i < list1.Count; i++)
                //{
                //    List<FoodInfoPriceModel> listFoodInfoPriceModel = new List<FoodInfoPriceModel>();
                //    FoodInfoPriceModel foodInfoModel = new FoodInfoPriceModel(0, list1[i].FoodPrice);
                //    listFoodInfoPriceModel.Add(foodInfoModel);

                //    if (list1[i].AllowCoin == 1)
                //    {
                //        foodInfoModel = new FoodInfoPriceModel(1, list1[i].Coins);
                //        listFoodInfoPriceModel.Add(foodInfoModel);
                //    }

                //    if (list1[i].AllowPoint == 1)
                //    {
                //        foodInfoModel = new FoodInfoPriceModel(2, list1[i].Points);
                //        listFoodInfoPriceModel.Add(foodInfoModel);
                //    }

                //    if (list1[i].AllowLottery == 1)
                //    {
                //        foodInfoModel = new FoodInfoPriceModel(3, list1[i].Lottery);
                //        listFoodInfoPriceModel.Add(foodInfoModel);
                //    }

                //    list1[i].priceListModel = listFoodInfoPriceModel;
                //}
                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, list1);
            }

            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "无数据");
        }

        /// <summary>
        /// 获取套餐列表
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object getFoodList(Dictionary<string, object> dicParas)
        {
            XCCloudUserTokenModel userTokenModel = (XCCloudUserTokenModel)(dicParas[Constant.XCCloudUserTokenModel]);
            StoreIDDataModel userTokenDataModel = (StoreIDDataModel)(userTokenModel.DataModel);

            string customerType = dicParas.ContainsKey("customerType") ? dicParas["customerType"].ToString() : string.Empty;
            string memberLevelId = dicParas.ContainsKey("memberLevelId") ? dicParas["memberLevelId"].ToString() : string.Empty;
            string foodTypeStr = dicParas.ContainsKey("foodTypeStr") ? dicParas["foodTypeStr"].ToString() : string.Empty;

            if (string.IsNullOrEmpty(memberLevelId))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "会员等级无效");
            }

            string sql = "exec GetFoodListInfo @StoreId,@CustomerType,@MemberLevelId,@FoodTypeStr ";
            SqlParameter[] parameters = new SqlParameter[4];
            parameters[0] = new SqlParameter("@StoreId", userTokenDataModel.StoreId);
            parameters[1] = new SqlParameter("@CustomerType", customerType);
            parameters[2] = new SqlParameter("@MemberLevelId", memberLevelId);
            parameters[3] = new SqlParameter("@FoodTypeStr", foodTypeStr);
            System.Data.DataSet ds = XCCloudBLL.ExecuteQuerySentence(sql, parameters);
            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                List<FoodInfoModel> list1 = Utils.GetModelList<FoodInfoModel>(ds.Tables[0]).ToList();
                for (int i = 0; i < list1.Count; i++)
                {
                    List<FoodInfoPriceModel> listFoodInfoPriceModel = new List<FoodInfoPriceModel>();
                    FoodInfoPriceModel foodInfoModel = new FoodInfoPriceModel(0, list1[i].FoodPrice);
                    listFoodInfoPriceModel.Add(foodInfoModel);

                    if (list1[i].AllowCoin == 1)
                    {
                        foodInfoModel = new FoodInfoPriceModel(1, list1[i].Coins);
                        listFoodInfoPriceModel.Add(foodInfoModel);
                    }

                    if (list1[i].AllowPoint == 1)
                    {
                        foodInfoModel = new FoodInfoPriceModel(2, list1[i].Points);
                        listFoodInfoPriceModel.Add(foodInfoModel);
                    }

                    if (list1[i].AllowLottery == 1)
                    {
                        foodInfoModel = new FoodInfoPriceModel(3, list1[i].Lottery);
                        listFoodInfoPriceModel.Add(foodInfoModel);
                    }

                    list1[i].priceListModel = listFoodInfoPriceModel;
                }
                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, list1);
            }

            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "无数据");
        }

        /// <summary>
        /// 获取套餐明细
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object getFoodDetail(Dictionary<string, object> dicParas)
        {
            XCCloudUserTokenModel userTokenModel = (XCCloudUserTokenModel)(dicParas[Constant.XCCloudUserTokenModel]);
            StoreIDDataModel userTokenDataModel = (StoreIDDataModel)(userTokenModel.DataModel);

            string foodId = dicParas.ContainsKey("foodId") ? dicParas["foodId"].ToString() : string.Empty;

            if (string.IsNullOrEmpty(foodId))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "套餐名不能为空");
            }

            string sql = "exec GetFoodDetail @StoreId,@FoodId ";
            SqlParameter[] parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("@StoreId", userTokenDataModel.StoreId);
            parameters[1] = new SqlParameter("@FoodId", foodId);
            System.Data.DataSet ds = XCCloudBLL.ExecuteQuerySentence(sql, parameters);
            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                List<FoodDetailModel> list1 = Utils.GetModelList<FoodDetailModel>(ds.Tables[0]).ToList();
                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, list1);
            }
            else
            { 
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "无数据");
            }   
        }
    }
}