using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.Business.XCCloud;
using XCCloudService.Business.XCGameMana;
using XCCloudService.CacheService.XCCloud;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.DAL;
using XCCloudService.DBService.BLL;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.CustomModel.XCCloud.Order;
using XCCloudService.Model.XCCloud;

namespace XXCloudService.Api.XCCloud
{
    [Authorize(Roles = "StoreUser")]
    /// <summary>
    /// Order 的摘要说明
    /// </summary>
    public class Order : ApiBase
    {
        /// <summary>
        /// 获取订单流水号
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
       
        public object getOrder(Dictionary<string, object> dicParas)
        {
            try
            {
                string StoreID = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
                string UserToken = dicParas.ContainsKey("userToken") ? dicParas["userToken"].ToString() : string.Empty;
                if (string.IsNullOrEmpty(UserToken))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户token无效");
                }

                var list = XCCloudUserTokenBusiness.GetUserTokenModel(UserToken);
                if (list == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户token无效");
                }
                if (string.IsNullOrEmpty(StoreID))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店号无效");
                }
                string OrderNum = string.Empty;
                IFlw_Order_SerialNumberService flw_Order_SerialNumberService = BLLContainer.Resolve<IFlw_Order_SerialNumberService>();
                var orderlist = flw_Order_SerialNumberService.GetModels(x => x.StoreiD == StoreID).ToList().FirstOrDefault(x => Convert.ToDateTime(x.CreateDate).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"));


                IBase_StoreInfoService base_StoreInfoService = BLLContainer.Resolve<IBase_StoreInfoService>();
                var menlist = base_StoreInfoService.GetModels(x => x.StoreID == StoreID).FirstOrDefault<Base_StoreInfo>();
                if (menlist == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "未查询到商户号");
                }
                int num = 0;
                if (orderlist == null)
                {
                    num = +1;
                    Flw_Order_SerialNumber flw_Order_SerialNumber = new Flw_Order_SerialNumber();
                    flw_Order_SerialNumber.StoreiD = StoreID;
                    flw_Order_SerialNumber.CreateDate = DateTime.Now;
                    flw_Order_SerialNumber.CurNumber = num;
                    flw_Order_SerialNumberService.Add(flw_Order_SerialNumber);
                }
                else
                {
                    num = Convert.ToInt32(orderlist.CurNumber + 1);
                    orderlist.CurNumber = num;
                    flw_Order_SerialNumberService.Update(orderlist);
                }
                OrderNum = DateTime.Now.ToString("yyyyMMddHHmm") + menlist.MerchID + num.ToString();
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, OrderNum, Result_Code.T, "");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 购物车结算
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [Authorize(Roles = "XcUser,XcAdmin")]
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object addOrder(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            XCCloudUserTokenModel userTokenModel = (XCCloudUserTokenModel)(dicParas[Constant.XCCloudUserTokenModel]);
            StoreIDDataModel userTokenDataModel = (StoreIDDataModel)(userTokenModel.DataModel);
            if (!CheckAddOrderParams(dicParas, out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
            }

            string buyDetailsJson = dicParas.ContainsKey("buyDetails") ? dicParas["buyDetails"].ToString() : string.Empty;
            List<OrderBuyDetailModel> buyDetailList = Utils.DataContractJsonDeserializer<List<OrderBuyDetailModel>>(buyDetailsJson);
            string customerType = dicParas.ContainsKey("customerType") ? dicParas["customerType"].ToString() : string.Empty;
            string memberLevelId = dicParas.ContainsKey("memberLevelId") ? dicParas["memberLevelId"].ToString() : string.Empty;
            //string foodCount = dicParas.ContainsKey("foodCount") ? dicParas["foodCount"].ToString() : string.Empty;
            //string goodCount = dicParas.ContainsKey("goodCount") ? dicParas["goodCount"].ToString() : string.Empty;
            string icCardId = dicParas.ContainsKey("icCardId") ? dicParas["icCardId"].ToString() : string.Empty;
            string payCount = dicParas.ContainsKey("payCount") ? dicParas["payCount"].ToString() : string.Empty;
            string freePay = dicParas.ContainsKey("freePay") ? dicParas["freePay"].ToString() : string.Empty;
            string realPay = dicParas.ContainsKey("realPay") ? dicParas["realPay"].ToString() : string.Empty;
            string scheduleId = dicParas.ContainsKey("scheduleId") ? dicParas["scheduleId"].ToString() : string.Empty;
            string workStation = dicParas.ContainsKey("workStation") ? dicParas["workStation"].ToString() : string.Empty;
            string authorId = dicParas.ContainsKey("authorId") ? dicParas["authorId"].ToString() : string.Empty;
            string note = dicParas.ContainsKey("note") ? dicParas["note"].ToString() : string.Empty;
            string orderSource = dicParas.ContainsKey("orderSource") ? dicParas["orderSource"].ToString() : string.Empty;
            string saleCoinType = dicParas.ContainsKey("saleCoinType") ? dicParas["saleCoinType"].ToString() : string.Empty;
            string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;

            string storedProcedure = "CreateOrder";
            String[] Ary = new String[] { "数据0", "数据1", "数据2", "数据3"};

            List<SqlDataRecord> listSqlDataRecord = new List<SqlDataRecord>();
            SqlMetaData[] MetaDataArr = new SqlMetaData[] { 
                    new SqlMetaData("foodId", SqlDbType.Int), 
                    new SqlMetaData("foodCount", SqlDbType.Int),
                    new SqlMetaData("payType", SqlDbType.Int),
                    new SqlMetaData("payNum", SqlDbType.Decimal)
            };

            
            for (int i = 0; i < buyDetailList.Count; i++)
            {
                List<object> listParas = new List<object>();
                listParas.Add(buyDetailList[i].FoodId);
                listParas.Add(buyDetailList[i].FoodCount);
                listParas.Add(buyDetailList[i].PayType);
                listParas.Add(buyDetailList[i].PayNum);

                var record = new SqlDataRecord(MetaDataArr);
                for (int j = 0; j < Ary.Length; j++)
                {
                    record.SetValue(j, listParas[j]);
                }
                listSqlDataRecord.Add(record);
            }

           

            SqlParameter[] sqlParameter = new SqlParameter[18];
            sqlParameter[0] = new SqlParameter("@FoodDetail", SqlDbType.Structured);
            sqlParameter[0].Value = listSqlDataRecord;

            sqlParameter[1] = new SqlParameter("@StoreID", SqlDbType.VarChar);
            sqlParameter[1].Value = userTokenDataModel.StoreId;

            sqlParameter[2] = new SqlParameter("@ICCardID", SqlDbType.Int);
            sqlParameter[2].Value = icCardId;

            sqlParameter[3] = new SqlParameter("@PayCount", SqlDbType.Decimal);
            sqlParameter[3].Value = payCount;

            sqlParameter[4] = new SqlParameter("@FreePay", SqlDbType.Decimal);
            sqlParameter[4].Value = freePay;

            sqlParameter[5] = new SqlParameter("@RealPay", SqlDbType.Decimal);
            sqlParameter[5].Value = realPay;

            sqlParameter[6] = new SqlParameter("@UserID", SqlDbType.Int);
            sqlParameter[6].Value = userTokenModel.LogId;

            sqlParameter[7] = new SqlParameter("@MemberLevelId", SqlDbType.Int);
            sqlParameter[7].Value = memberLevelId;

            sqlParameter[8] = new SqlParameter("@WorkStation", SqlDbType.VarChar);
            sqlParameter[8].Value = workStation;

            sqlParameter[9] = new SqlParameter("@AuthorID", SqlDbType.Int);
            sqlParameter[9].Value = authorId;

            sqlParameter[10] = new SqlParameter("@Note", SqlDbType.VarChar);
            sqlParameter[10].Value = note;

            sqlParameter[11] = new SqlParameter("@OrderSource", SqlDbType.Int);
            sqlParameter[11].Value = orderSource;

            sqlParameter[12] = new SqlParameter("@SaleCoinType", SqlDbType.Int);
            sqlParameter[12].Value = saleCoinType;

            sqlParameter[13] = new SqlParameter("@CustomerType", SqlDbType.Int);
            sqlParameter[13].Value = customerType;

            sqlParameter[14] = new SqlParameter("@Mobile", SqlDbType.VarChar);
            sqlParameter[14].Value = mobile;

            sqlParameter[15] = new SqlParameter("@ErrMsg", SqlDbType.VarChar,200);
            sqlParameter[15].Direction = ParameterDirection.Output;

            sqlParameter[16] = new SqlParameter("@OrderFlwID", SqlDbType.Int);
            sqlParameter[16].Direction = ParameterDirection.Output;

            sqlParameter[17] = new SqlParameter("@Return", SqlDbType.Int);
            sqlParameter[17].Direction = ParameterDirection.ReturnValue;

            XCCloudBLL.ExecuteStoredProcedureSentence(storedProcedure, sqlParameter);
            if (sqlParameter[17].Value.ToString() == "1")
            {
                var obj = new {
                    orderFlwId = Convert.ToInt32(sqlParameter[16].Value)
                };
                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, obj);
            }
            else
            {
                return new ResponseModel(Return_Code.T, "", Result_Code.F, sqlParameter[15].Value.ToString());
            }
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [Authorize(Roles = "XcUser,XcAdmin")]
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object getOrderContain(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            string orderFlwId = dicParas.ContainsKey("orderFlwId") ? dicParas["orderFlwId"].ToString() : string.Empty;
            XCCloudUserTokenModel userTokenModel = (XCCloudUserTokenModel)(dicParas[Constant.XCCloudUserTokenModel]);
            StoreIDDataModel userTokenDataModel = (StoreIDDataModel)(userTokenModel.DataModel);

            if (string.IsNullOrEmpty(orderFlwId))
            {
                return new ResponseModel(Return_Code.T, "", Result_Code.F, "订单Id参数无效");
            }

            string storedProcedure = "GetOrderContainById";
            SqlParameter[] sqlParameter = new SqlParameter[2];
            sqlParameter[0] = new SqlParameter("@StoreId", SqlDbType.VarChar);
            sqlParameter[0].Value = userTokenDataModel.StoreId;
            sqlParameter[1] = new SqlParameter("@OrderFlwId", SqlDbType.Int);
            sqlParameter[1].Value = orderFlwId;

            System.Data.DataSet ds = XCCloudBLL.GetStoredProcedureSentence(storedProcedure, sqlParameter);
            if (ds != null && ds.Tables.Count == 2 && ds.Tables[0].Rows.Count >0)
            {
                OrderMainModel main = Utils.GetModelList<OrderMainModel>(ds.Tables[0]).ToList()[0];
                List<OrderDetailModel> detail = Utils.GetModelList<OrderDetailModel>(ds.Tables[1]).ToList();
                OrderInfoModel model = new OrderInfoModel(main, detail);
                return ResponseModelFactory.CreateSuccessModel<OrderInfoModel>(isSignKeyReturn, model);
            }
            else
            {
                return new ResponseModel(Return_Code.T, "", Result_Code.F, "订单信息不存在");
            }
        }

        /// <summary>
        /// 完成订单支付
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [Authorize(Roles = "XcUser,XcAdmin")]
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object payOrder(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            string orderFlwId = dicParas.ContainsKey("orderFlwId") ? dicParas["orderFlwId"].ToString() : string.Empty;
            string openICCardId = dicParas.ContainsKey("openICCardId") ? dicParas["openICCardId"].ToString() : string.Empty;
            string workStation = dicParas.ContainsKey("workStation") ? dicParas["workStation"].ToString() : string.Empty;
            string authorId = dicParas.ContainsKey("authorId") ? dicParas["authorId"].ToString() : string.Empty;
            string realPay = dicParas.ContainsKey("realPay") ? dicParas["realPay"].ToString() : string.Empty;

            XCCloudUserTokenModel userTokenModel = (XCCloudUserTokenModel)(dicParas[Constant.XCCloudUserTokenModel]);
            StoreIDDataModel userTokenDataModel = (StoreIDDataModel)(userTokenModel.DataModel);

            if (string.IsNullOrEmpty(orderFlwId))
            {
                return new ResponseModel(Return_Code.T, "", Result_Code.F, "订单Id参数无效");
            }

            if (!Utils.IsNumeric(openICCardId) || int.Parse(openICCardId) <0)
            {
                return new ResponseModel(Return_Code.T, "", Result_Code.F, "开通卡号参数无效");
            }

            if (!Utils.IsNumeric(authorId) || int.Parse(authorId) < 0)
            {
                return new ResponseModel(Return_Code.T, "", Result_Code.F, "授权Id无效");
            }

            if (string.IsNullOrEmpty(workStation))
            {
                return new ResponseModel(Return_Code.T, "", Result_Code.F, "工作站参数无效");
            }

            if (!Utils.IsNumeric(realPay) && decimal.Parse(realPay) <= 0)
            {
                return new ResponseModel(Return_Code.T, "", Result_Code.F, "实付金额无效");
            }

            string storedProcedure = "FinishOrderPayment";
            SqlParameter[] sqlParameter = new SqlParameter[9];
            sqlParameter[0] = new SqlParameter("@StoreID", SqlDbType.VarChar);
            sqlParameter[0].Value = userTokenDataModel.StoreId;

            sqlParameter[1] = new SqlParameter("@OrderFlwId", SqlDbType.Int);
            sqlParameter[1].Value = orderFlwId;

            sqlParameter[2] = new SqlParameter("@OpenICCardId", SqlDbType.Int);
            sqlParameter[2].Value = openICCardId;

            sqlParameter[3] = new SqlParameter("@RealPay", SqlDbType.Decimal);
            sqlParameter[3].Value = realPay;

            sqlParameter[4] = new SqlParameter("@UserID", SqlDbType.Int);
            sqlParameter[4].Value = userTokenModel.LogId;

            sqlParameter[5] = new SqlParameter("@WorkStation", SqlDbType.VarChar);
            sqlParameter[5].Value = workStation;

            sqlParameter[6] = new SqlParameter("@AuthorID", SqlDbType.Int);
            sqlParameter[6].Value = authorId;

            sqlParameter[7] = new SqlParameter("@ErrMsg", SqlDbType.VarChar,200);
            sqlParameter[7].Direction = ParameterDirection.Output;

            sqlParameter[8] = new SqlParameter("@Return", SqlDbType.Int);
            sqlParameter[8].Direction = ParameterDirection.ReturnValue;

            XCCloudBLL.ExecuteStoredProcedureSentence(storedProcedure, sqlParameter);
            if (sqlParameter[8].Value.ToString() == "1")
            {
                return new ResponseModel(Return_Code.T, "", Result_Code.T, "");
            }
            else
            {
                return new ResponseModel(Return_Code.T, "", Result_Code.F, sqlParameter[7].Value.ToString());
            }
        }

        private bool CheckAddOrderParams(Dictionary<string, object> dicParas,out string errMsg)
        {
            errMsg = string.Empty;
            string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
            string icCardId = dicParas.ContainsKey("icCardId") ? dicParas["icCardId"].ToString() : string.Empty;
            string payCount = dicParas.ContainsKey("payCount") ? dicParas["payCount"].ToString() : string.Empty;
            string freePay = dicParas.ContainsKey("freePay") ? dicParas["freePay"].ToString() : string.Empty;
            string realPay = dicParas.ContainsKey("realPay") ? dicParas["realPay"].ToString() : string.Empty;
            string scheduleId = dicParas.ContainsKey("scheduleId") ? dicParas["scheduleId"].ToString() : string.Empty;
            string workStation = dicParas.ContainsKey("workStation") ? dicParas["workStation"].ToString() : string.Empty;
            string authorId = dicParas.ContainsKey("authorId") ? dicParas["authorId"].ToString() : string.Empty;
            string note = dicParas.ContainsKey("note") ? dicParas["note"].ToString() : string.Empty;
            string orderSource = dicParas.ContainsKey("orderSource") ? dicParas["orderSource"].ToString() : string.Empty;
            string saleCoinType = dicParas.ContainsKey("saleCoinType") ? dicParas["saleCoinType"].ToString() : string.Empty;

            if (!XCCloudStoreBusiness.IsEffectiveStore(storeId))
            {
                errMsg = "门店无效";
                return false;
            }

            if (!Utils.IsDecimal(payCount))
            {
                errMsg = "应付金额无效";
                return false;
            }

            if (!Utils.IsDecimal(realPay))
            {
                errMsg = "实付金额无效";
                return false;
            }

            if (!Utils.IsDecimal(freePay))
            {
                errMsg = "减免金额无效";
                return false;
            }

            if (!Utils.isNumber(scheduleId))
            {
                errMsg = "班次Id不正确";
                return false;
            }

            if (string.IsNullOrEmpty(workStation))
            {
                errMsg = "工作站无效";
                return false;
            }

            if (!Utils.IsNumeric(authorId))
            {
                errMsg = "授权员工Id无效";
                return false;
            }

            if (string.IsNullOrEmpty(orderSource))
            {
                errMsg = "订单来源无效";
                return false;
            }

            string orderSourceDefineStr = "0,1,2,3,4";
            string[] orderSourceArr = orderSource.Split(',');
            for (int i = 0; i < orderSourceArr.Length; i++)
            {
                if (!Utils.IsNumeric(orderSourceArr[i]))
                {
                    errMsg = "订单来源无效";
                    return false;
                }

                if (!orderSourceDefineStr.Contains(orderSourceArr[i]))
                {
                    errMsg = "订单来源无效";
                    return false;
                }
            }

            //:1电子币、2实物币手工、3、实物币售币机
            string saleCoinTypeStr = "1,2,3";
            string[] saleCoinTypeArr = saleCoinTypeStr.Split(',');
            for (int i = 0; i < saleCoinTypeArr.Length; i++)
            {
                if (!Utils.IsNumeric(saleCoinTypeArr[i]))
                {
                    errMsg = "售币类型无效";
                    return false;
                }

                if (!saleCoinTypeStr.Contains(saleCoinTypeArr[i]))
                {
                    errMsg = "售币类型无效";
                    return false;
                }
            }

            return true;
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetOrders(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                object[] conditions = dicParas.ContainsKey("conditions") ? (object[])dicParas["conditions"] : null;
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string merchId = userTokenKeyModel.DataModel.MerchID;

                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@merchId", merchId);
                string sqlWhere = string.Empty;
                
                if (conditions != null && conditions.Length > 0)
                {
                    if (!QueryBLL.GenDynamicSql(conditions, "b.", ref sqlWhere, ref parameters, out errMsg))
                    {
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                }

                string sql = @"select b.ID, a.StoreID, a.StoreName, b.OrderID, b.FoodCount, b.OrderSource, b.CreateTime, b.PayType, b.OrderStatus," +
                    " c.DictKey as OrderSourceStr, d.DictKey as PayTypeStr, e.DictKey as OrderStatusStr, f.FoodName from Base_StoreInfo a " +
                    " inner join Flw_Order b on a.StoreID=b.StoreID " +
                    " left join (select b.* from Dict_System a inner join Dict_System b on a.ID=b.PID where a.DictKey='订单来源' and a.PID=0) c on convert(varchar, b.OrderSource)=c.DictValue " +
                    " left join (select b.* from Dict_System a inner join Dict_System b on a.ID=b.PID where a.DictKey='支付通道' and a.PID=0) d on convert(varchar, b.PayType)=d.DictValue " +
                    " left join (select b.* from Dict_System a inner join Dict_System b on a.ID=b.PID where a.DictKey='订单状态' and a.PID=0) e on convert(varchar, b.OrderStatus)=e.DictValue " +
                    " left join (select top 1 a.ID as OrderFlwID, d.FoodName from Flw_Order a inner join Flw_Order_Detail b on a.ID=b.OrderFlwID " +
                    " inner join Flw_Food_Sale c on b.FoodFlwID=c.ID " +
                    " inner join Data_FoodInfo d on c.FoodID=d.FoodID) f on b.ID=f.OrderFlwID " +
                    " where a.MerchID=@merchId ";
                sql = sql + sqlWhere;
                var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Flw_Order).Namespace);
                var flw_Orders = dbContext.Database.SqlQuery<Flw_OrdersModel>(sql, parameters).ToList();

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, flw_Orders);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetOrdersDetails(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                int orderFlwId = (dicParas.ContainsKey("id") && Utils.isNumber(dicParas["id"])) ? Convert.ToInt32(dicParas["id"]) : 0;

                IFlw_OrderService flw_OrderService = BLLContainer.Resolve<IFlw_OrderService>();
                if (!flw_OrderService.Any(p => p.ID == orderFlwId))
                {
                    errMsg = "该订单不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@id", orderFlwId);

                string sql = @"select d.*, e.StoreName, f.DictKey as FoodTypeStr, g.DictKey as RechargeTypeStr, h.DictKey as FoodStateStr from Flw_Order a" +
                    " inner join Flw_Order_Detail b on a.ID=b.OrderFlwID " +
                    " inner join Flw_Food_Sale c on b.FoodFlwID=c.ID " +
                    " inner join Data_FoodInfo d on c.FoodID=d.FoodID " +
                    " left join Base_StoreInfo e on d.StoreID=e.StoreID " +
                    " left join (select b.* from Dict_System a inner join Dict_System b on a.ID=b.PID where a.DictKey='套餐类别' and a.PID=0) f on convert(varchar, d.FoodType)=f.DictValue " +
                    " left join (select b.* from Dict_System a inner join Dict_System b on a.ID=b.PID where a.DictKey='充值方式' and a.PID=0) g on convert(varchar, d.RechargeType)=g.DictValue " +
                    " left join (select b.* from Dict_System a inner join Dict_System b on a.ID=b.PID where a.DictKey='套餐状态' and a.PID=0) h on convert(varchar, d.FoodState)=h.DictValue " +
                    " where a.ID=@id ";
                var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Data_FoodInfo).Namespace);
                var data_FoodInfo = dbContext.Database.SqlQuery<Data_FoodInfoModel>(sql, parameters).ToList();

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, data_FoodInfo);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetOrdersChart(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string merchId = userTokenKeyModel.DataModel.MerchID;

                DateTime now = DateTime.Now;
                DateTime start = new DateTime(now.Year, now.Month, 1);
                DateTime end = start.AddMonths(1).AddDays(-1);
                TimeSpan ts = end.Subtract(start);
                int num = ts.Days;

                List<Store_CheckDateChartModel> store_CheckDateChart = new List<Store_CheckDateChartModel>();
                var base_StoreInfo = BLLContainer.Resolve<IBase_StoreInfoService>().GetModels(p => p.MerchID.Equals(merchId, StringComparison.OrdinalIgnoreCase)).Select(o => o.StoreID);
                var store_CheckDate = BLLContainer.Resolve<IStore_CheckDateService>().GetModels(p => base_StoreInfo.Contains(p.StoreID) && System.Data.Entity.DbFunctions.DiffMonths(p.CheckDate, DateTime.Now) == 0);
                for (int i = 0; i <= num; i++)
                {
                    DateTime da = start.AddDays(i).Date;
                    var query = store_CheckDate.Where(p => System.Data.Entity.DbFunctions.DiffDays(p.CheckDate, da) == 0);
                    Store_CheckDateChartModel store_CheckDateModel = new Store_CheckDateChartModel();
                    store_CheckDateModel.CheckDate = da;
                    store_CheckDateModel.AliPay = (decimal)query.Sum(s => s.AliPay).GetValueOrDefault();
                    store_CheckDateModel.Wechat = (decimal)query.Sum(s => s.Wechat).GetValueOrDefault();
                    store_CheckDateChart.Add(store_CheckDateModel);
                }

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, store_CheckDateChart);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetOrdersCheck(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string checkDate = dicParas.ContainsKey("checkDate") ? dicParas["checkDate"].ToString() : string.Empty;
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string merchId = userTokenKeyModel.DataModel.MerchID;

                string sql = " exec  GetOrdersCheck @CheckDate,@MerchId ";
                var parameters = new SqlParameter[2];
                parameters[0] = new SqlParameter("@CheckDate", checkDate);
                parameters[1] = new SqlParameter("@MerchId", merchId);

                var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Flw_CheckDate).Namespace);
                var flw_OrderCheck = dbContext.Database.SqlQuery<Flw_OrderCheckModel>(sql, parameters).ToList();
                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, flw_OrderCheck);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object CheckOrders(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                int id = (dicParas.ContainsKey("id") && Utils.isNumber(dicParas["id"])) ? Convert.ToInt32(dicParas["id"]) : 0;
                string checkDate = dicParas.ContainsKey("checkDate") ? dicParas["checkDate"].ToString() : string.Empty;
                string merchId = string.Empty;
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                merchId = userTokenKeyModel.DataModel.MerchID;

                if (string.IsNullOrEmpty(checkDate))
                {
                    errMsg = "营业日期不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }
                
                string sql = " exec  CheckOrders @CheckDate,@MerchId,@ID,@Return output ";
                var parameters = new SqlParameter[4];
                parameters[0] = new SqlParameter("@CheckDate", checkDate);
                parameters[1] = new SqlParameter("@MerchId", merchId);
                parameters[2] = new SqlParameter("@ID", id);
                parameters[3] = new SqlParameter("@Return", 0);
                parameters[3].Direction = System.Data.ParameterDirection.Output;
                
                XCCloudBLL.ExecuteQuerySentence(sql, parameters);
                if (parameters[3].Value.ToString() == "1")
                {
                    return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
                }
                else
                {
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, "账目审核失败");
                }                
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        /// <summary>
        /// 获取订单支付状态
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getOrderPayState(Dictionary<string, object> dicParas)
        {
            try
            {
                string OrderId = dicParas.ContainsKey("orderId") ? dicParas["orderId"].ToString().Trim() : string.Empty;
                string UserToken = dicParas.ContainsKey("userToken") ? dicParas["userToken"].ToString() : string.Empty;
                if (string.IsNullOrEmpty(UserToken))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户token无效");
                }

                var list = XCCloudUserTokenBusiness.GetUserTokenModel(UserToken);
                if (list == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户token无效");
                }

                OrderPayCacheModel model = new OrderPayCacheModel();
                if(OrderPayCache.IsExist(OrderId))
                {
                    model = OrderPayCache.GetValue(OrderId) as OrderPayCacheModel;
                }
                else
                {
                    Flw_Order order = Flw_OrderBusiness.GetOrderModel(OrderId);
                    if(order != null)
                    {
                        model.OrderId = OrderId;
                        model.PayAmount = order.RealPay == null ? 0 : order.RealPay.Value;
                        model.PayTime = order.PayTime == null ? "" : order.PayTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                        model.PayState = (OrderState)order.OrderStatus.Value;
                    }
                }

                return ResponseModelFactory<OrderPayCacheModel>.CreateModel(isSignKeyReturn, model);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}