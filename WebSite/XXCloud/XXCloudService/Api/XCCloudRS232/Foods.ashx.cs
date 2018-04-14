using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloudRS232;
using XCCloudService.Business.Common;
using XCCloudService.Business.XCCloudRS232;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCCloudRS232;
using XCCloudService.Model.XCCloudRS232;

namespace XXCloudService.Api.XCCloudRS232
{
    /// <summary>
    /// Foods 的摘要说明
    /// </summary>
    public class Foods : ApiBase
    {
        /// <summary>
        /// 套餐新增
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object AddFoods(Dictionary<string, object> dicParas)
        {
            try
            {
                string MobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;//获取手机令牌
                string DeviceToken = dicParas.ContainsKey("deviceToken") ? dicParas["deviceToken"].ToString() : string.Empty;//获取设备token
                string FoodName = dicParas.ContainsKey("foodname") ? dicParas["foodname"].ToString() : string.Empty;//获取套餐名称
                string FoodPrice = dicParas.ContainsKey("foodprice") ? dicParas["foodprice"].ToString() : string.Empty;//获取套餐价格
                string CoinQuantity = dicParas.ContainsKey("coinquantity") ? dicParas["coinquantity"].ToString() : string.Empty;//获取币数量
                string IsQuickFood = dicParas.ContainsKey("isquickfood") ? dicParas["isquickfood"].ToString() : string.Empty;//获取是否允许散客购买0不允许，1允许
                string FoodState = dicParas.ContainsKey("foodstate") ? dicParas["foodstate"].ToString() : string.Empty;//获取是否有效 1有效
                IMerchService merchService = BLLContainer.Resolve<IMerchService>("XCCloudRS232");
                var merchlist = merchService.GetModels(x => x.Token == MobileToken && x.State == 1).FirstOrDefault<Base_MerchInfo>();
                if (merchlist == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机令牌无效");
                }
                var devicelist = DeviceBusiness.GetDeviceModel(DeviceToken);
                if (devicelist == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备令牌无效");
                }
                t_foods foods = new t_foods();
                IFoodsService foodsService = BLLContainer.Resolve<IFoodsService>("XCCloudRS232");
                foods.MerchID = merchlist.ID;
                foods.DeviceID = devicelist.ID;
                foods.FoodName = FoodName;
                foods.FoodPrice =Convert.ToDecimal(FoodPrice);
                foods.IsQuickFood = int.Parse(IsQuickFood);
                foods.FoodState = int.Parse(FoodState);
                foods.CoinQuantity = int.Parse(CoinQuantity);
                foodsService.Add(foods);
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 套餐修改
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]

        public object UpdateFoods(Dictionary<string, object> dicParas)
        {
            try
            {
                string MobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;//获取手机令牌
                string FoodName = dicParas.ContainsKey("foodname") ? dicParas["foodname"].ToString() : string.Empty;//获取套餐名称
                string FoodPrice = dicParas.ContainsKey("foodprice") ? dicParas["foodprice"].ToString() : string.Empty;//获取套餐价格
                string CoinQuantity = dicParas.ContainsKey("coinquantity") ? dicParas["coinquantity"].ToString() : string.Empty;//获取币数量
                string IsQuickFood = dicParas.ContainsKey("isquickfood") ? dicParas["isquickfood"].ToString() : string.Empty;//获取是否允许散客购买0不允许，1允许
                string FoodState = dicParas.ContainsKey("foodstate") ? dicParas["foodstate"].ToString() : string.Empty;//获取是否有效 1有效
                string FoodID = dicParas.ContainsKey("foodid") ? dicParas["foodid"].ToString() : string.Empty;//获取套餐ID
                IMerchService merchService = BLLContainer.Resolve<IMerchService>("XCCloudRS232");
                var merchlist = merchService.GetModels(x => x.Token == MobileToken && x.State == 1).FirstOrDefault<Base_MerchInfo>();
                if (merchlist == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机令牌无效");
                }            
                int ID = int.Parse(FoodID);
                IFoodsService foodsService = BLLContainer.Resolve<IFoodsService>("XCCloudRS232");
                var foods = foodsService.GetModels(x => x.FoodID == ID).FirstOrDefault<t_foods>();
                if (foods == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "该套餐不存在");
                }
                foods.MerchID = merchlist.ID;
                foods.FoodName = FoodName;
                foods.FoodPrice = Convert.ToDecimal(FoodPrice);
                foods.IsQuickFood = int.Parse(IsQuickFood);
                foods.FoodState = int.Parse(FoodState);
                foods.CoinQuantity = int.Parse(CoinQuantity);
                foodsService.Update(foods);
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        /// <summary>
        /// 获取套餐信息
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object GetFoods(Dictionary<string, object> dicParas)
        {
            try
            {
                string MobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;//获取手机令牌
                string mobile = string.Empty;
                IMerchService merchService = BLLContainer.Resolve<IMerchService>("XCCloudRS232");
                var merchlist = merchService.GetModels(x => x.Token == MobileToken && x.State == 1).FirstOrDefault<Base_MerchInfo>();
                if (merchlist == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机令牌无效");
                }
                string FoodID = dicParas.ContainsKey("foodid") ? dicParas["foodid"].ToString() : string.Empty;//获取套餐ID
                string DeviceToken = dicParas.ContainsKey("deviceToken") ? dicParas["deviceToken"].ToString() : string.Empty;//获取设备token
                var devicelist = DeviceBusiness.GetDeviceModel(DeviceToken);
                if (devicelist == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备令牌无效");
                }
                string sql = "select * from (";
                sql += "select a.FoodID,FoodName,a.FoodPrice,a.CoinQuantity,a.IsQuickFood,a.FoodState,a.DeviceID,b.*from (select * from t_foods) a, (select id,DeviceName from Base_DeviceInfo where Status='1')  as b where a.DeviceID=b.ID ";
                sql += ")b";
                sql += " where FoodState='1'and DeviceID='"+devicelist.ID+"' ";
                if (FoodID != "")
                {

                    int ID = int.Parse(FoodID);
                    sql += " and FoodID='"+ID+"' ";
                }
                sql += " order by ID ";
                DataSet ds1 = XCCloudRS232BLL.ExecuteQuerySentence(sql, null);
                DataTable dt1 = ds1.Tables[0];
                if (dt1.Rows.Count > 0)
                {
                    var StoreNamelist = Utils.GetModelList<FoodsModel>(dt1).ToList();
                    FoodsModellist foodsModellist = new FoodsModellist();
                    foodsModellist.Lists = StoreNamelist;
                    return ResponseModelFactory<FoodsModellist>.CreateModel(isSignKeyReturn, foodsModellist);
                }
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "未查询到套餐信息");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 删除套餐信息
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object DeleteFoods(Dictionary<string, object> dicParas)
        {
            try
            {
                string MobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;//获取手机令牌
                string mobile = string.Empty;
                if (!MobileTokenBusiness.ExistToken(MobileToken, out mobile))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机令牌无效");
                }
                string FoodID = dicParas.ContainsKey("foodid") ? dicParas["foodid"].ToString() : string.Empty;//获取套餐ID
                int ID = int.Parse(FoodID);
                IFoodsService foodsService = BLLContainer.Resolve<IFoodsService>("XCCloudRS232");
                var foods = foodsService.GetModels(x => x.FoodID == ID).FirstOrDefault<t_foods>();
                if (foods == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "未查询到套餐信息");
                }
                foods.FoodState = 0;
                foodsService.Update(foods);
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        

    }
}