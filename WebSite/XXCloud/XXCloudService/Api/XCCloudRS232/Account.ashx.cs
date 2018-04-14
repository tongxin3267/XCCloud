using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.Business.XCCloudRS232;
using XCCloudService.Model.XCCloudRS232;
using XCCloudService.Common.Extensions;
using System.Data;
using XCCloudService.Model.CustomModel.XCCloudRS232;
using XCCloudService.Common;
using XCCloudService.Common.Enum;

namespace XXCloudService.Api.XCCloudRS232
{
    /// <summary>
    /// Account 的摘要说明
    /// </summary>
    public class Account : ApiBase
    {
        #region 统计商户营收总览
        /// <summary>
        /// 统计商户营收总览
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getFoodSaleRevenue(Dictionary<string, object> dicParas)
        {
            try
            {
                string mobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;
                string routerToken = dicParas.ContainsKey("routerToken") ? dicParas["routerToken"].ToString() : string.Empty;
                string sDate = dicParas.ContainsKey("sDate") ? dicParas["sDate"].ToString() : string.Empty;
                string eDate = dicParas.ContainsKey("eDate") ? dicParas["eDate"].ToString() : string.Empty;

                if (mobileToken == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机令牌不能为空");
                }
                Base_MerchInfo merch = MerchBusiness.GetMerchModel(mobileToken);
                if (merch.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户token无效");
                }

                int? routerId = null;
                Base_DeviceInfo router = DeviceBusiness.GetDeviceModel(routerToken);
                if (!router.IsNull())
                {
                    routerId = router.ID;
                }

                DataSet ds = AccountBusiness.GetMerchRevenue(merch.ID, routerId, sDate, eDate);

                MerchRevenueModel model = new MerchRevenueModel();
                model.MerchName = merch.MerchName;

                if (ds.Tables.Count == 0)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "无数据");
                }

                model.Revenues = new List<RevenueDataModel>();
                model.Payment = new List<RevenueDataModel>();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    model.Amount = ds.Tables[0].Rows[0]["Amount"].ToString();

                    model.Revenues.Add(new RevenueDataModel() { Title = "充值总额", Value = ds.Tables[0].Rows[0]["RechargeAmount"].ToString() });
                    model.Revenues.Add(new RevenueDataModel() { Title = "售币总额", Value = ds.Tables[0].Rows[0]["SaleAmount"].ToString() });

                    model.Payment.Add(new RevenueDataModel() { Title = "现金", Value = ds.Tables[0].Rows[0]["CashPayAmount"].ToString() });
                    model.Payment.Add(new RevenueDataModel() { Title = "微信", Value = ds.Tables[0].Rows[0]["WechatPayAmount"].ToString() });
                    model.Payment.Add(new RevenueDataModel() { Title = "支付宝", Value = ds.Tables[0].Rows[0]["AliPayAmount"].ToString() });
                    model.Payment.Add(new RevenueDataModel() { Title = "银联", Value = ds.Tables[0].Rows[0]["UnionPayAmount"].ToString() });
                }
                else
                {
                    model.Revenues.Add(new RevenueDataModel() { Title = "充值总额", Value = "0.00" });
                    model.Revenues.Add(new RevenueDataModel() { Title = "售币总额", Value = "0.00" });

                    model.Payment.Add(new RevenueDataModel() { Title = "现金", Value = "0.00" });
                    model.Payment.Add(new RevenueDataModel() { Title = "微信", Value = "0.00" });
                    model.Payment.Add(new RevenueDataModel() { Title = "支付宝", Value = "0.00" });
                    model.Payment.Add(new RevenueDataModel() { Title = "银联", Value = "0.00" });
                }

                model.Coins = new List<RevenueDataModel>();
                if (ds.Tables[1].Rows.Count > 0)
                {
                    model.Coins.Add(new RevenueDataModel() { Title = "存币数", Value = ds.Tables[1].Rows[0]["SaveCoins"].ToString() });
                }

                if (ds.Tables[2].Rows.Count > 0)
                {
                    model.Coins.Add(new RevenueDataModel() { Title = "售币数", Value = ds.Tables[2].Rows[0]["SaleCoins"].ToString() });
                }
                
                return ResponseModelFactory<MerchRevenueModel>.CreateModel(isSignKeyReturn, model);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 按控制器查询外设游戏币营收
        /// <summary>
        /// 按控制器查询外设游戏币营收
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getRoutersRevenue(Dictionary<string, object> dicParas)
        {
            try
            {
                string mobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;
                string routerToken = dicParas.ContainsKey("routerToken") ? dicParas["routerToken"].ToString() : string.Empty;
                string sDate = dicParas.ContainsKey("sDate") ? dicParas["sDate"].ToString() : string.Empty;
                string eDate = dicParas.ContainsKey("eDate") ? dicParas["eDate"].ToString() : string.Empty;

                if (mobileToken == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机令牌不能为空");
                }
                Base_MerchInfo merch = MerchBusiness.GetMerchModel(mobileToken);
                if (merch.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户令牌无效");
                }

                Base_DeviceInfo router = DeviceBusiness.GetDeviceModel(routerToken);
                if (router.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "控制器令牌无效");
                }

                DataSet ds = AccountBusiness.GetRoutersAmount(merch.ID, router.ID, sDate, eDate);

                MerchRoutersRevenueModel model = new MerchRoutersRevenueModel();
                model.RouterName = router.DeviceName;
                model.Token = router.Token;

                if (ds.Tables.Count == 0)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "无数据");
                }

                model.SaveCoins = new CoinRevenueModel() { Coins = new List<RevenueDataModel>() };
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Int64 coinTotal = 0;
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        string currCoin = row["SaveCoins"].IsNull() ? "0" : row["SaveCoins"].ToString();
                        model.SaveCoins.Coins.Add(new RevenueDataModel() { Title = row["DeviceName"].ToString(), Value = currCoin });

                        coinTotal += Convert.ToInt64(currCoin);
                    }
                    model.SaveCoins.CoinsTotal = coinTotal.ToString();
                }

                model.SaleCoins = new CoinRevenueModel() { Coins = new List<RevenueDataModel>() };
                if (ds.Tables[1].Rows.Count > 0)
                {
                    Int64 coinTotal = 0;
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        string currCoin = row["SaleCoins"].IsNull() ? "0" : row["SaleCoins"].ToString();
                        model.SaleCoins.Coins.Add(new RevenueDataModel() { Title = row["DeviceName"].ToString(), Value = currCoin });

                        coinTotal += Convert.ToInt64(currCoin);
                    }

                    model.SaleCoins.CoinsTotal = coinTotal.ToString();
                }

                return ResponseModelFactory<MerchRoutersRevenueModel>.CreateModel(isSignKeyReturn, model);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 统计外设营收
        /// <summary>
        /// 统计外设营收
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getPeripheralRevenue(Dictionary<string, object> dicParas)
        {
            try
            {
                string mobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;
                string deviceToken = dicParas.ContainsKey("deviceToken") ? dicParas["deviceToken"].ToString() : string.Empty;
                string sDate = dicParas.ContainsKey("sDate") ? dicParas["sDate"].ToString() : string.Empty;
                string eDate = dicParas.ContainsKey("eDate") ? dicParas["eDate"].ToString() : string.Empty;
                string strPageIndex = dicParas.ContainsKey("pageIndex") ? dicParas["pageIndex"].ToString() : string.Empty;
                string strpageSize = dicParas.ContainsKey("pageSize") ? dicParas["pageSize"].ToString() : string.Empty;

                int pageIndex = 1, pageSize = 10;

                if (!string.IsNullOrWhiteSpace(strPageIndex) && strPageIndex.IsInt())
                {
                    pageIndex = Convert.ToInt32(strPageIndex);
                }

                if (!string.IsNullOrWhiteSpace(strpageSize) && strpageSize.IsInt())
                {
                    pageSize = Convert.ToInt32(strpageSize);
                }

                if (mobileToken == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机令牌不能为空");
                }
                Base_MerchInfo merch = MerchBusiness.GetMerchModel(mobileToken);
                if (merch.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户令牌无效");
                }

                Base_DeviceInfo device = DeviceBusiness.GetDeviceModel(deviceToken);
                if (device.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备令牌无效");
                }

                PeripheralRevenueModel model = new PeripheralRevenueModel();
                model.DeviceName = device.DeviceName;
                model.SN = device.SN;

                //当前设备类型
                DeviceTypeEnum currDeviceType = (DeviceTypeEnum)device.DeviceType;
                if (currDeviceType == DeviceTypeEnum.DepositMachine)
                {
                    DataSet ds = AccountBusiness.GetSaveCoinMachineRevenue(merch.ID, device.ID, sDate, eDate, pageIndex, pageSize);
                    if(ds.Tables.Count > 0)
                    {
                        if(ds.Tables[0].Rows.Count>0)
                        {
                            RevenueDataModel m = new RevenueDataModel();
                            m.Title = "存币数";
                            m.Value = ds.Tables[0].Rows[0]["SaveCoins"].ToString();
                            model.Coins.Add(m);
                        }
                        if (ds.Tables[1].Rows.Count > 0)
                        {
                            model.CoinSerials = Utils.GetModelList<CoinSerialModel>(ds.Tables[1]).ToList();
                        }
                    }
                }
                else if (currDeviceType == DeviceTypeEnum.SlotMachines)
                {
                    DataSet ds = AccountBusiness.GetSaleCoinMachineRevenue(merch.ID, device.ID, sDate, eDate, pageIndex, pageSize);
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            RevenueDataModel m = new RevenueDataModel();
                            m.Title = "提币数";
                            m.Value = ds.Tables[0].Rows[0]["PullCoins"].ToString();
                            model.Coins.Add(m);

                            m = new RevenueDataModel();
                            m.Title = "售币数";
                            m.Value = ds.Tables[0].Rows[0]["SaleCoins"].ToString();
                            model.Coins.Add(m);

                            m = new RevenueDataModel();
                            m.Title = "售币金额";
                            m.Value = ds.Tables[0].Rows[0]["SaleCoinAmount"].ToString();
                            model.Coins.Add(m);
                        }
                        if (ds.Tables[1].Rows.Count > 0)
                        {
                            model.CoinSerials = Utils.GetModelList<CoinSerialModel>(ds.Tables[1]).ToList();
                        }
                    }
                }
                else
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "无数据");
                }

                return ResponseModelFactory<PeripheralRevenueModel>.CreateModel(isSignKeyReturn, model);
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
        }
        #endregion

        #region 会员数据统计
        /// <summary>
        /// 会员数据统计
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getMemberSummary(Dictionary<string, object> dicParas)
        {
            try
            {
                string mobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;
                string icCardId = dicParas.ContainsKey("icCardId") ? dicParas["icCardId"].ToString() : string.Empty;
                string sDate = dicParas.ContainsKey("sDate") ? dicParas["sDate"].ToString() : string.Empty;
                string eDate = dicParas.ContainsKey("eDate") ? dicParas["eDate"].ToString() : string.Empty;
                string strPageIndex = dicParas.ContainsKey("pageIndex") ? dicParas["pageIndex"].ToString() : string.Empty;
                string strpageSize = dicParas.ContainsKey("pageSize") ? dicParas["pageSize"].ToString() : string.Empty;

                int pageIndex = 1, pageSize = 10;

                if (!string.IsNullOrWhiteSpace(strPageIndex) && strPageIndex.IsInt())
                {
                    pageIndex = Convert.ToInt32(strPageIndex);
                }

                if (!string.IsNullOrWhiteSpace(strpageSize) && strpageSize.IsInt())
                {
                    pageSize = Convert.ToInt32(strpageSize);
                }

                if (mobileToken == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机令牌不能为空");
                }
                Base_MerchInfo merch = MerchBusiness.GetMerchModel(mobileToken);
                if (merch.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户令牌无效");
                }

                DataSet ds = AccountBusiness.GetMemberSummary(merch.ID, icCardId, sDate, eDate, pageIndex, pageSize);

                MemberViewModel model = new MemberViewModel();

                if (ds.Tables.Count == 0)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "无数据");
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    model.MemberTotal = Convert.ToInt32(row["MemberTotal"]);
                    model.BalanceTotal = Convert.ToInt32(row["BalanceTotal"]);
                    model.TodayJoinMembers = Convert.ToInt32(row["TodayJoinMembers"]);
                    model.TodayGameMembers = Convert.ToInt32(row["TodayGameMembers"]);
                }

                model.MemberList = new List<MemberListModel>();
                if (ds.Tables[1].Rows.Count > 0)
                {
                    model.MemberList = Utils.GetModelList<MemberListModel>(ds.Tables[1]).ToList();
                }

                return ResponseModelFactory<MemberViewModel>.CreateModel(isSignKeyReturn, model);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 会员详情
        /// <summary>
        /// 会员详情
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getMemberDetail(Dictionary<string, object> dicParas)
        {
            try
            {
                string mobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;
                string memberId = dicParas.ContainsKey("memberId") ? dicParas["memberId"].ToString() : string.Empty;

                if (mobileToken == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机令牌不能为空");
                }
                Base_MerchInfo merch = MerchBusiness.GetMerchModel(mobileToken);
                if (merch.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户令牌无效");
                }

                if (string.IsNullOrWhiteSpace(memberId))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "参数错误");
                }

                MemberDetailModel model = new MemberDetailModel();

                DataTable table = AccountBusiness.GetMemberDetail(memberId);

                if (table.Rows.Count == 0)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "无数据");
                }

                DataRow row = table.Rows[0];
                model.ICCardID = row["ICCardID"].ToString();
                model.MemberName = row["MemberName"].ToString();
                model.Mobile = row["Mobile"].ToString();
                model.Birthday = row["Birthday"].ToString();
                model.CertificalID = row["CertificalID"].ToString();
                model.Balance = string.IsNullOrWhiteSpace(row["Balance"].ToString()) ? "0" : row["Balance"].ToString();
                model.Lottery = string.IsNullOrWhiteSpace(row["Lottery"].ToString()) ? "0" : row["Lottery"].ToString();
                model.Point = string.IsNullOrWhiteSpace(row["Point"].ToString()) ? "0" : row["Point"].ToString();
                switch (row["Type"].ToString())
                {
                    case "0": model.Type = "会员卡"; break;
                    case "1": model.Type = "数字币"; break;
                    case "2": model.Type = "临时卡"; break;
                    case "3": model.Type = "返分卡"; break;
                    default: model.Type = "会员卡"; break;
                }
                model.JoinTime = row["JoinTime"].IsNull() ? "" : Convert.ToDateTime(row["JoinTime"]).ToString("yyyy-MM-dd HH:mm:ss");
                model.EndDate = string.IsNullOrWhiteSpace(row["EndDate"].ToString()) ? "" : Convert.ToDateTime(row["EndDate"]).ToString("yyyy-MM-dd");
                model.MemberPassword = row["MemberPassword"].ToString();
                model.Note = row["Note"].ToString();
                model.Lock = string.IsNullOrWhiteSpace(row["EndDate"].ToString()) || row["EndDate"].ToString() == "0" ? "未锁定" : "已锁定";
                model.LockDate = "";
                if (model.Lock == "已锁定")
                {
                    model.LockDate = string.IsNullOrWhiteSpace(row["LockDate"].ToString()) ? "" : Convert.ToDateTime(row["EndDate"]).ToString("yyyy-MM-dd");
                }

                return ResponseModelFactory<MemberDetailModel>.CreateModel(isSignKeyReturn, model);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 会员消费流水
        /// <summary>
        /// 会员消费流水
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getMemberExpenseDetail(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string mobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;
                string icCardId = dicParas.ContainsKey("icCardId") ? dicParas["icCardId"].ToString() : string.Empty;
                string flowType = dicParas.ContainsKey("flowType") ? dicParas["flowType"].ToString() : string.Empty;
                string sDate = dicParas.ContainsKey("sDate") ? dicParas["sDate"].ToString() : string.Empty;
                string eDate = dicParas.ContainsKey("eDate") ? dicParas["eDate"].ToString() : string.Empty;
                string strPageIndex = dicParas.ContainsKey("pageIndex") ? dicParas["pageIndex"].ToString() : string.Empty;
                string strpageSize = dicParas.ContainsKey("pageSize") ? dicParas["pageSize"].ToString() : string.Empty;

                int pageIndex = 1, pageSize = 10;

                if (!string.IsNullOrWhiteSpace(strPageIndex) && strPageIndex.IsInt())
                {
                    pageIndex = Convert.ToInt32(strPageIndex);
                }

                if (!string.IsNullOrWhiteSpace(strpageSize) && strpageSize.IsInt())
                {
                    pageSize = Convert.ToInt32(strpageSize);
                }

                if (mobileToken == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机令牌不能为空");
                }
                Base_MerchInfo merch = MerchBusiness.GetMerchModel(mobileToken);
                if (merch.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户token无效");
                }

                t_member member = MemberBusiness.GetMerchModel(icCardId);
                if (member.IsNull())
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "会员卡号错误");
                }

                DataTable table = AccountBusiness.GetMemberExpenseDetail(merch.ID, icCardId, flowType, sDate, eDate, pageIndex, pageSize);

                if (table.Rows.Count == 0)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "无数据");
                }

                var list = Utils.GetModelList<MemberExpenseDetailModel>(table).ToList();
                return ResponseModelFactory<List<MemberExpenseDetailModel>>.CreateModel(isSignKeyReturn, list);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion
    }
}