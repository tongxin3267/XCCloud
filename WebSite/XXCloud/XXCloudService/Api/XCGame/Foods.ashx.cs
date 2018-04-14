using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.CacheService;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.Business.XCGameMana;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCGame;
using XCCloudService.Common.Enum;
using System.Data.SqlClient;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.Model.XCGameManager;
using XCCloudService.Business.Common;
using XCCloudService.Model.Socket.UDP;
using XCCloudService.SocketService.UDP.Factory;
using XCCloudService.Model.CustomModel.XCGameManager;
using System.Transactions;
using XXCloudService.Utility;
using XCCloudService.Model.WeiXin.SAppMessage;
using XCCloudService.WeiXin.Message;
using Aop.Api;
using XCCloudService.Pay.Alipay;
using Aop.Api.Request;
using Aop.Api.Response;

namespace XCCloudService.Api.XCGame
{
    /// <summary>
    /// Foods 的摘要说明
    /// </summary>
    public class Foods : ApiBase
    {

        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameMemberToken)]
        public object getfoods(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                TokenType tokenType = TokenType.Member;
                StoreCacheModel storeModel = null;
                List<XCCloudService.Model.XCGame.t_foods> xcGameFoodsList = null;
                List<XCCloudService.Model.XCCloudRS232.t_foods> rs232FoodsList = null;

                //获取token模式
                MobileTokenModel mobileTokenModel = null;
                XCGameMemberTokenModel memberTokenModel = null;
                if (!XCCloudService.Business.XCGame.MeberAndMobileTokenBusiness.GetTokenData(dicParas, out tokenType, ref mobileTokenModel, ref memberTokenModel, out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                //验证门店
                StoreBusiness store = new StoreBusiness();
                if (!store.IsEffectiveStore(memberTokenModel.StoreId, ref storeModel, out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }
                if (storeModel.StoreType == 0)
                {
                    XCCloudService.BLL.IBLL.XCGame.IFoodsService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IFoodsService>(storeModel.StoreDBName);
                    xcGameFoodsList = memberService.GetModels(p => p.FoodState.Equals("1") && p.FoodType.Equals("0") && p.ForeAuthorize.ToString().Equals("0")).ToList<XCCloudService.Model.XCGame.t_foods>();
                }
                else if (storeModel.StoreType == 1)
                {
                    XCCloudService.BLL.IBLL.XCCloudRS232.IFoodsService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCCloudRS232.IFoodsService>();
                    rs232FoodsList = memberService.GetModels(p => p.FoodState == 1).ToList<XCCloudService.Model.XCCloudRS232.t_foods>();
                }

                //根据查询结果输出
                if (xcGameFoodsList == null && rs232FoodsList == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "套餐明细信息不存在");
                }
                else
                {
                    if (xcGameFoodsList != null)
                    {
                        List<FoodsResponseModel> foodsResponseModel = Utils.GetCopyList<FoodsResponseModel, XCCloudService.Model.XCGame.t_foods>(xcGameFoodsList);
                        SetFoodsResponseModelList(foodsResponseModel);
                        return ResponseModelFactory<List<FoodsResponseModel>>.CreateModel(isSignKeyReturn, foodsResponseModel);
                    }
                    else
                    {
                        List<FoodsResponseModel> foodsResponseModel = Utils.GetCopyList<FoodsResponseModel, XCCloudService.Model.XCCloudRS232.t_foods>(rs232FoodsList);
                        SetFoodsResponseModelList(foodsResponseModel);
                        return ResponseModelFactory<List<FoodsResponseModel>>.CreateModel(isSignKeyReturn, foodsResponseModel); 
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        private void SetFoodsResponseModelList(List<FoodsResponseModel> foodsResponseModel)
        {
            var foodsModel = foodsResponseModel.Where<FoodsResponseModel>(p => p.IsQuickFood == 1).FirstOrDefault<FoodsResponseModel>();
            List<FoodsResponseModel> list = foodsResponseModel.Where<FoodsResponseModel>(p => p.IsQuickFood != 1).ToList<FoodsResponseModel>();
            for (int i = 0;i < list.Count;i++)
            {
                if (foodsModel != null)
                {
                    list[i].SendCoinQuantity = Convert.ToInt32(Math.Round(foodsModel.CoinQuantity / foodsModel.FoodPrice / 10 * list[i].FoodPrice));
                } 
            }
        }



        #region "foodsale方法"

        /// <summary>
        /// 验证手机令牌
        /// </summary>
        /// <param name="mobileToken">手机令牌</param>
        /// <param name="mobile">手机号码</param>
        /// <returns></returns>
        private bool checkMobileToken(string mobileToken, out string mobile)
        {
            mobile = string.Empty;
            if (string.IsNullOrEmpty(mobileToken) || !MobileTokenBusiness.ExistToken(mobileToken, out mobile))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 会员令牌
        /// </summary>
        /// <param name="memberToken">会员令牌</param>
        /// <param name="mobile">手机号码</param>
        /// <returns></returns>
        private bool checkMemberToken(string memberToken, out string mobile)
        {
            mobile = string.Empty;
            var memberTokenModel = XCCloudService.Business.XCGame.MemberTokenBusiness.GetMemberTokenModel(memberToken);
            if (memberTokenModel == null)
            {
                mobile = memberTokenModel.Mobile;
                return true;
            }
            else
            {
                return false;
            }
        }



        //购币
        private bool BuyCoin(XCGameManaDeviceStoreType deviceStoreType, string xcGameDBName, string storeId,int icCardId, int memberLevelId, int foodId, string orderId, string money, int coins, int balance, string paymentype, string deviceId,int deviceIdentityId, out string errMsg)
        {
            errMsg = string.Empty;
            if (deviceStoreType == XCGameManaDeviceStoreType.Store)
            {
                //验证套餐信息
                XCCloudService.BLL.IBLL.XCGame.IFoodsService foodservice = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IFoodsService>(xcGameDBName);
                var foodlist = foodservice.GetModels(p => p.FoodID == foodId).FirstOrDefault<XCCloudService.Model.XCGame.t_foods>();
                if (foodlist == null)
                {
                    errMsg = "套餐不存在";
                    return false;
                }
                //验证班次信息
                XCCloudService.BLL.IBLL.XCGame.IScheduleService scheduleService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IScheduleService>(xcGameDBName);
                var scheduleModel = scheduleService.GetModels(p => p.State.Equals("0", StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XCCloudService.Model.XCGame.flw_schedule>();
                if (scheduleModel == null)
                {
                    errMsg = "相关班次不存在";
                    return false;
                }

                int userID = Convert.ToInt32(scheduleModel.UserID);
                string scheduleId = scheduleModel.ID.ToString();
                string workStation = scheduleModel.WorkStation;
                string foodName = foodlist.FoodName;
                //获取套餐名称
                ///向数据库子表的food_sale插入数据
                string sql = "exec InsertFood @Balance,@ICCardID,@FoodID,@CoinQuantity,@Point,@MemberLevelID,@UserID,@ScheduleID,@WorkStation,@MacAddress,@OrderID,@FoodName,@Money,@Paymentype,@Return output ";
                SqlParameter[] parameters = new SqlParameter[15];
                parameters[0] = new SqlParameter("@Balance", balance);
                parameters[1] = new SqlParameter("@ICCardID", icCardId);
                parameters[2] = new SqlParameter("@FoodID", foodId);
                parameters[3] = new SqlParameter("@CoinQuantity", coins);
                parameters[4] = new SqlParameter("@Point", "0");
                parameters[5] = new SqlParameter("@MemberLevelID", memberLevelId);
                parameters[6] = new SqlParameter("@UserID", userID);
                parameters[7] = new SqlParameter("@ScheduleID", scheduleId);
                parameters[8] = new SqlParameter("@WorkStation", workStation);
                parameters[9] = new SqlParameter("@MacAddress", deviceId);
                parameters[10] = new SqlParameter("@OrderID", orderId);
                parameters[11] = new SqlParameter("@FoodName", foodName);
                parameters[12] = new SqlParameter("@Money", money);
                parameters[13] = new SqlParameter("@Paymentype", paymentype);
                parameters[14] = new SqlParameter("@Return", 0);
                parameters[14].Direction = System.Data.ParameterDirection.Output;
                XCCloudService.BLL.IBLL.XCGame.IFoodsaleService foodsale = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IFoodsaleService>(xcGameDBName);
                XCCloudService.Model.XCGame.flw_food_sale member = foodsale.SqlQuery(sql, parameters).FirstOrDefault<XCCloudService.Model.XCGame.flw_food_sale>();
                return true;
            }
            else if (deviceStoreType == XCGameManaDeviceStoreType.Merch)
            {
                XCCloudService.BLL.IBLL.XCCloudRS232.IFoodSaleService foodsale = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCCloudRS232.IFoodSaleService>();
                XCCloudService.Model.XCCloudRS232.flw_food_sale flwFood = new Model.XCCloudRS232.flw_food_sale();
                flwFood.OrderID = orderId;
                flwFood.MerchID = int.Parse(storeId);
                flwFood.ICCardID = icCardId;
                flwFood.DeviceID = deviceIdentityId;
                flwFood.FlowType = 1;
                flwFood.CoinQuantity = coins;
                flwFood.TotalMoney = decimal.Parse(money);
                flwFood.Point = 0;
                flwFood.Balance = balance;
                flwFood.Note = string.Empty;
                flwFood.PayType = PayBusiness.GetPaymentTypeId(paymentype);
                flwFood.PayTime = System.DateTime.Now;
                flwFood.PayState = 1;
                flwFood.PayTotal = 0;
                foodsale.Add(flwFood);
                return true;
            }
            else
            {
                errMsg = "门店类型不正确";
                return false;
            }
        }



        private bool Recharge(XCGameManaDeviceStoreType deviceStoreType, string mobile,string xcGameDBName, string storeId, int icCardId, int memberLevelId, int foodId, string orderId, string money, int coins, int balance, string paymentype, string deviceId,int deviceIdentityId,out string foodName,out string errMsg)
        {
            foodName = string.Empty;
            errMsg = string.Empty;
            balance += coins;
            if (deviceStoreType == XCGameManaDeviceStoreType.Store)
            {
                //验证套餐信息
                
                XCCloudService.BLL.IBLL.XCGame.IFoodsService foodservice = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IFoodsService>(xcGameDBName);
                var foodModel = foodservice.GetModels(p => p.FoodID == foodId).FirstOrDefault<XCCloudService.Model.XCGame.t_foods>();
                if (foodModel == null)
                {
                    errMsg = "套餐明细不存在";
                    return false;
                }
                foodName = foodModel.FoodName;
                //验证班次信息
                XCCloudService.BLL.IBLL.XCGame.IScheduleService scheduleService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IScheduleService>(xcGameDBName);
                var schedulelist = scheduleService.GetModels(p => p.State.Equals("0", StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XCCloudService.Model.XCGame.flw_schedule>();
                if (schedulelist == null)
                {
                    errMsg = "相关班次不存在";
                    return false;
                }

                string sql = "exec RechargeFood @Balance,@ICCardID,@FoodID,@CoinQuantity,@Point,@MemberLevelID,@UserID,@ScheduleID,@WorkStation,@MacAddress,@OrderID,@FoodName,@Money,@Paymentype,@Return output ";
                SqlParameter[] parameters = new SqlParameter[15];
                parameters[0] = new SqlParameter("@Balance", balance);
                parameters[1] = new SqlParameter("@ICCardID", icCardId);
                parameters[2] = new SqlParameter("@FoodID", foodId);
                parameters[3] = new SqlParameter("@CoinQuantity", coins);
                parameters[4] = new SqlParameter("@Point", "0");
                parameters[5] = new SqlParameter("@MemberLevelID", memberLevelId);
                parameters[6] = new SqlParameter("@UserID", Convert.ToInt32(schedulelist.UserID));
                parameters[7] = new SqlParameter("@ScheduleID", schedulelist.ID.ToString());
                parameters[8] = new SqlParameter("@WorkStation", schedulelist.WorkStation);
                parameters[9] = new SqlParameter("@MacAddress", "");
                parameters[10] = new SqlParameter("@OrderID", orderId);
                parameters[11] = new SqlParameter("@FoodName", foodModel.FoodName);
                parameters[12] = new SqlParameter("@Money", money);
                parameters[13] = new SqlParameter("@Paymentype", paymentype);
                parameters[14] = new SqlParameter("@Return", 0);
                parameters[14].Direction = System.Data.ParameterDirection.Output;
                XCCloudService.BLL.IBLL.XCGame.IFoodsaleService foodsale = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IFoodsaleService>(xcGameDBName);
                XCCloudService.Model.XCGame.flw_food_sale member = foodsale.SqlQuery(sql, parameters).FirstOrDefault<XCCloudService.Model.XCGame.flw_food_sale>();
            }
            else if (deviceStoreType == XCGameManaDeviceStoreType.Merch)
            {
                XCCloudService.BLL.IBLL.XCCloudRS232.IFoodSaleService foodsale = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCCloudRS232.IFoodSaleService>();
                XCCloudService.Model.XCCloudRS232.flw_food_sale flwFood = new Model.XCCloudRS232.flw_food_sale();
                flwFood.OrderID = orderId;
                flwFood.MerchID = int.Parse(storeId);
                flwFood.ICCardID = icCardId;
                flwFood.DeviceID = deviceIdentityId;
                flwFood.FlowType = 1;
                flwFood.CoinQuantity = coins;
                flwFood.TotalMoney = decimal.Parse(money);
                flwFood.Point = 0;
                flwFood.Balance = balance;
                flwFood.Note = string.Empty;
                flwFood.PayType = PayBusiness.GetPaymentTypeId(paymentype);
                flwFood.PayTime = System.DateTime.Now;
                flwFood.PayState = 1;
                flwFood.PayTotal = 0;

                XCCloudService.BLL.IBLL.XCCloudRS232.IMemberService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCCloudRS232.IMemberService>();
                var memberModel = memberService.GetModels(p => p.Mobile.Equals(mobile, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XCCloudService.Model.XCCloudRS232.t_member>();
                memberModel.Balance = Convert.ToInt32(memberModel.Balance) + coins;

                using (var transactionScope = new System.Transactions.TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    foodsale.Add(flwFood);
                    memberService.Update(memberModel);
                    transactionScope.Complete();
                }

                return true;
            }
            else
            {
                errMsg = "门店类型不正确";
            }
            return true;
        }


        #endregion

      
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameMemberOrMobileToken)]
        public object foodsale(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string storeName = string.Empty;
                string foodName = string.Empty;
                string storePassword = string.Empty;
                int icCardId = 0;//会员
                int balance = 0;//币余额
                int memberLevelId = 0;//会员级别
                string state = string.Empty;
                string stateName = string.Empty;
                string mobile = string.Empty;
                string segment = string.Empty;
                string mcuId = string.Empty;
                string storeId = string.Empty;
                string deviceId = string.Empty;
                int foodId = 0;
                int deviceIdentityId = 0;
                string terminalNo = dicParas.ContainsKey("deviceToken") ? dicParas["deviceToken"].ToString() : string.Empty;
                string type = dicParas.ContainsKey("type") ? dicParas["type"].ToString() : string.Empty;
                string paymentype = dicParas.ContainsKey("Paymentype") ? dicParas["Paymentype"].ToString() : string.Empty;//获取支付方式
                string foodIdStr = dicParas.ContainsKey("foodid") ? dicParas["foodid"].ToString() : string.Empty;//获取支付方式
                string money = dicParas.ContainsKey("money") ? dicParas["money"].ToString() : string.Empty;
                string orderId = dicParas.ContainsKey("OrderID") ? dicParas["OrderID"].ToString() : string.Empty;
                int coins = int.Parse(dicParas.ContainsKey("Coins") ? dicParas["Coins"].ToString() : string.Empty);
                int foodNum = int.Parse(dicParas.ContainsKey("foodNum") ? dicParas["foodNum"].ToString() : "1");
                int.TryParse(foodIdStr, out foodId);

                XCGameMemberTokenModel memberTokenModel = (XCGameMemberTokenModel)(dicParas[Constant.XCGameMemberTokenModel]);
                mobile = memberTokenModel.Mobile;

                if (string.IsNullOrEmpty(orderId))
                {
                    orderId = System.Guid.NewGuid().ToString("N");
                }

                if (type == "购币")
                {
                    string xcGameDBName = string.Empty;
                    //根据终端号查询终端号是否存在
                    XCGameManaDeviceStoreType deviceStoreType;
                    if (!ExtendBusiness.checkXCGameManaDeviceInfo(terminalNo, out deviceStoreType, out storeId, out deviceId))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "终端号不存在");
                    }
                    //验证会员令牌的门店号和设备门店号
                    if (!memberTokenModel.StoreId.Equals(storeId))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "会员令牌不能再此设备上操作");
                    }
                    //验证门店信息和设备状态是否为启用状态
                    if (!ExtendBusiness.checkStoreDeviceInfo(deviceStoreType, storeId, deviceId, out segment, out mcuId, out xcGameDBName, out deviceIdentityId,out storePassword,out storeName, out errMsg))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                    }
                    //验证雷达设备缓存状态
                    if (!ExtendBusiness.checkRadarDeviceState(deviceStoreType, storeId, deviceId, out errMsg))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                    }
                    //获取会员信息
                    if (!ExtendBusiness.GetMemberInfo(deviceStoreType, mobile, xcGameDBName, out balance, out icCardId, out memberLevelId, out errMsg))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                    }
                    //购币
                    if (!BuyCoin(deviceStoreType, xcGameDBName,storeId,icCardId, memberLevelId, foodId, orderId, money, coins, balance, paymentype, deviceId,deviceIdentityId, out errMsg))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                    }
                    //请求雷达处理出币
                    if (!IConUtiltiy.DeviceOutputCoin(deviceStoreType, DevieControlTypeEnum.出币, storeId, mobile, icCardId, orderId, segment, mcuId, storePassword, foodId, coins, string.Empty, out errMsg))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                    }
                    //设置推送消息的缓存结构
                    string form_id = dicParas.ContainsKey("form_id") ? dicParas["form_id"].ToString() : string.Empty;
                    MemberFoodSaleNotifyDataModel dataModel = new MemberFoodSaleNotifyDataModel("购币", storeName, mobile, foodName, foodNum, icCardId, decimal.Parse(money), coins);
                    SAppMessageMana.SetMemberCoinsMsgCacheData( SAppMessageType.MemberFoodSaleNotify,orderId, form_id, mobile, dataModel, out errMsg);
                }
                else if (type == "充值")
                {
                    StoreCacheModel storeModel = null;
                    XCGameManaDeviceStoreType deviceStoreType;
                    //验证门店
                    StoreBusiness store = new StoreBusiness();
                    if (!store.IsEffectiveStore(memberTokenModel.StoreId,out deviceStoreType, ref storeModel, out errMsg))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                    }
                    //获取会员信息
                    if (!ExtendBusiness.GetMemberInfo(deviceStoreType, mobile, storeModel.StoreDBName, out balance, out icCardId, out memberLevelId, out errMsg))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                    }
                    //充值
                    LogHelper.SaveLog(TxtLogType.Api, TxtLogContentType.Debug, TxtLogFileType.Day, "Recharge:" + errMsg);
                    if (!Recharge(deviceStoreType, mobile, storeModel.StoreDBName, storeId, icCardId, memberLevelId, foodId, orderId, money, coins, balance, paymentype, deviceId, deviceIdentityId,out foodName, out errMsg))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                    }
                    //推送消息
                    string form_id = dicParas.ContainsKey("form_id") ? dicParas["form_id"].ToString() : string.Empty;
                    SAppMessageMana.PushMemberFoodSaleMsg("","","充值",storeModel.StoreName, mobile, orderId, foodName, foodNum, icCardId, decimal.Parse(money),coins, form_id, out errMsg);
                }
                else
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "类型无效");
                }


                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }
            catch (Exception e)
            {
                throw e;
            }
       }
        

    }
}