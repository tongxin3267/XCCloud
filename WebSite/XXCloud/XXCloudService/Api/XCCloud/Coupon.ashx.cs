using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.Common;
using XCCloudService.DBService.BLL;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.XCCloud;

namespace XXCloudService.Api.XCCloud
{
    [Authorize(Roles = "MerchUser")]
    /// <summary>
    /// Coupon 的摘要说明
    /// </summary>
    public class Coupon : ApiBase
    {

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object QueryCouponInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string merchId = userTokenKeyModel.DataModel.MerchID;
                string errMsg = string.Empty;

                object[] conditions = dicParas.ContainsKey("conditions") ? (object[])dicParas["conditions"] : null;

                SqlParameter[] parameters = new SqlParameter[1];
                string sqlWhere = string.Empty;
                parameters[0] = new SqlParameter("@MerchId", merchId);

                if (conditions != null && conditions.Length > 0)
                {
                    if (!QueryBLL.GenDynamicSql(conditions, "a.", ref sqlWhere, ref parameters, out errMsg))
                    {
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                }

                string sql = @"select a.ID, a.CouponName, b.DictKey as CouponTypeStr, a.PublishCount, (a.PublishCount - isnull(c.UseCount, 0) - isnull(d.UseCount, 0)) as LeftCount, " +
                    " (case when a.StartTime is null or a.StartTime='' then '' else convert(varchar,a.StartTime,23) end) as StartTime, (case when a.EndTime is null or a.EndTime='' then '' else convert(varchar,a.EndTime,23) end) as EndTime, " +
                    " a.Context from Data_CouponInfo a" +
                    " left join (select b.* from Dict_System a inner join Dict_System b on a.ID=b.PID where a.DictKey='优惠券类别' and a.PID=0) b on convert(varchar, a.CouponType)=b.DictValue " +
                    " left join (select a.ID as CouponID, count(c.ID) as UseCount from Data_CouponInfo a inner join Data_CouponList b on a.ID=b.CouponID inner join Flw_CouponUse c on b.ID=c.CouponCode) c on a.ID=c.CouponID " +
                    " left join (select a.ID as CouponID, count(b.ID) as UseCount from Data_CouponInfo a inner join Flw_CouponUse b on a.ID=b.CouponID) d on a.ID=d.CouponID " + 
                    " where a.MerchID=@MerchId";
                sql = sql + sqlWhere;

                IData_CouponInfoService data_CouponInfoService = BLLContainer.Resolve<IData_CouponInfoService>();
                var data_CouponInfo = data_CouponInfoService.SqlQuery<Data_CouponInfoModel>(sql, parameters).ToList();

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, data_CouponInfo);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }        

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetCouponInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? (dicParas["id"] + "") : string.Empty;
                if (string.IsNullOrEmpty(id))
                {
                    errMsg = "规则编号不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iId = Convert.ToInt32(id);
                IData_CouponInfoService data_CouponInfoService = BLLContainer.Resolve<IData_CouponInfoService>();
                var data_CouponInfo = data_CouponInfoService.GetModels(p => p.ID == iId).FirstOrDefault();
                if (data_CouponInfo == null)
                {
                    errMsg = "该优惠券不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, data_CouponInfo);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        //[ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        //public object SaveCouponInfo(Dictionary<string, object> dicParas)
        //{
        //    try
        //    {
        //        XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
        //        string merchId = userTokenKeyModel.DataModel.MerchID;

        //        string errMsg = string.Empty;
        //        string id = dicParas.ContainsKey("id") ? (dicParas["id"] + "") : string.Empty;
        //        string couponName = dicParas.ContainsKey("couponName") ? (dicParas["couponName"] + "") : string.Empty;
        //        string couponType = dicParas.ContainsKey("couponType") ? (dicParas["couponType"] + "") : string.Empty;
        //        string entryCouponFlag = dicParas.ContainsKey("entryCouponFlag") ? (dicParas["entryCouponFlag"] + "") : string.Empty;
        //        string authorFlag = dicParas.ContainsKey("authorFlag") ? (dicParas["authorFlag"] + "") : string.Empty;
        //        string overUseCount = dicParas.ContainsKey("overUseCount") ? (dicParas["overUseCount"] + "") : string.Empty;
        //        string publishCount = dicParas.ContainsKey("publishCount") ? (dicParas["publishCount"] + "") : string.Empty;
        //        string couponValue = dicParas.ContainsKey("couponValue") ? (dicParas["couponValue"] + "") : string.Empty;
        //        string couponDiscount = dicParas.ContainsKey("couponDiscount") ? (dicParas["couponDiscount"] + "") : string.Empty;
        //        string couponThreshold = dicParas.ContainsKey("couponThreshold") ? (dicParas["couponThreshold"] + "") : string.Empty;
        //        string startTime = dicParas.ContainsKey("startTime") ? (dicParas["startTime"] + "") : string.Empty;
        //        string endTime = dicParas.ContainsKey("endTime") ? (dicParas["endTime"] + "") : string.Empty;
        //        string sendType = dicParas.ContainsKey("sendType") ? (dicParas["sendType"] + "") : string.Empty;
        //        string overMoney = dicParas.ContainsKey("overMoney") ? (dicParas["overMoney"] + "") : string.Empty;
        //        string freeCouponCount = dicParas.ContainsKey("freeCouponCount") ? (dicParas["freeCouponCount"] + "") : string.Empty;
        //        string jackpotCount = dicParas.ContainsKey("jackpotCount") ? (dicParas["jackpotCount"] + "") : string.Empty;
        //        string jackpotId = dicParas.ContainsKey("jackpotId") ? (dicParas["jackpotId"] + "") : string.Empty;
        //        string chargeType = dicParas.ContainsKey("chargeType") ? (dicParas["chargeType"] + "") : string.Empty;
        //        string chargeCount = dicParas.ContainsKey("chargeCount") ? (dicParas["chargeCount"] + "") : string.Empty;
        //        string goodId = dicParas.ContainsKey("goodId") ? (dicParas["goodId"] + "") : string.Empty;
        //        string projectId = dicParas.ContainsKey("projectId") ? (dicParas["projectId"] + "") : string.Empty;
        //        string context = dicParas.ContainsKey("context") ? (dicParas["context"] + "") : string.Empty;
        //        int iId = 0;
        //        int.TryParse(id, out iId);
                                
        //        #region 验证参数

        //        if (!string.IsNullOrEmpty(id) && !Utils.isNumber(id))
        //        {
        //            errMsg = "规则ID格式不正确";
        //            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
        //        }

        //        if (string.IsNullOrEmpty(couponName))
        //        {
        //            errMsg = "优惠券名称不能为空";
        //            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
        //        }

        //        if (string.IsNullOrEmpty(couponValue))
        //        {
        //            errMsg = "优惠券价值不能为空";
        //            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
        //        }

        //        if (!Utils.IsDecimal(couponValue))
        //        {
        //            errMsg = "优惠券价值格式不正确";
        //            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
        //        }

        //        if (string.IsNullOrEmpty(couponDiscount))
        //        {
        //            errMsg = "优惠券折扣不能为空";
        //            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
        //        }

        //        if (!Utils.IsDecimal(couponDiscount))
        //        {
        //            errMsg = "优惠券折扣格式不正确";
        //            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
        //        }

        //        if (string.IsNullOrEmpty(couponThreshold))
        //        {
        //            errMsg = "优惠券阈值不能为空";
        //            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
        //        }

        //        if (!Utils.IsDecimal(couponThreshold))
        //        {
        //            errMsg = "优惠券阈值格式不正确";
        //            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
        //        }

        //        if (string.IsNullOrEmpty(couponType))
        //        {
        //            errMsg = "使用类别不能为空";
        //            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
        //        }

        //        if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
        //        {
        //            errMsg = "使用期限不能为空";
        //            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
        //        }
                
        //        #endregion

        //        IData_CouponInfoService data_CouponInfoService = BLLContainer.Resolve<IData_CouponInfoService>();
        //        var data_CouponInfo = new Data_CouponInfo();
        //        data_CouponInfo.ID = iId;
        //        data_CouponInfo.CouponName = couponName;
        //        data_CouponInfo.CouponValue = Convert.ToDecimal(couponValue);
        //        data_CouponInfo.CouponDiscount = Convert.ToDecimal(couponDiscount);
        //        data_CouponInfo.CouponThreshold = Convert.ToDecimal(couponThreshold);
        //        data_CouponInfo.CouponType = Convert.ToInt32(couponType);
        //        data_CouponInfo.OpUserID = Convert.ToInt32(userTokenKeyModel.LogId);
        //        data_CouponInfo.Context = context;
        //        data_CouponInfo.MerchID = merchId;
        //        if (!data_CouponInfoService.Any(a => a.ID == iId))
        //        {
        //            //新增
        //            if (!data_CouponInfoService.Add(data_CouponInfo))
        //            {
        //                errMsg = "添加电子优惠券信息失败";
        //                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
        //            }
        //        }
        //        else
        //        {
        //            //修改
        //            if (!data_CouponInfoService.Update(data_CouponInfo))
        //            {
        //                errMsg = "修改电子优惠券信息失败";
        //                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
        //            }
        //        }

        //        return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
        //    }
        //    catch (Exception e)
        //    {
        //        return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
        //    }
        //}

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object DelCouponInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? (dicParas["id"] + "") : string.Empty;
                if (string.IsNullOrEmpty(id))
                {
                    errMsg = "规则ID不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iId = Convert.ToInt32(id);
                IData_CouponListService data_CouponListService = BLLContainer.Resolve<IData_CouponListService>();
                IData_CouponInfoService data_CouponInfoService = BLLContainer.Resolve<IData_CouponInfoService>();
                IFlw_CouponUseService flw_CouponUseService = BLLContainer.Resolve<IFlw_CouponUseService>();
                if (flw_CouponUseService.Any(a => a.CouponCode == iId))
                {
                    errMsg = "该优惠券规则已使用不能删除";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!data_CouponInfoService.Any(a => a.ID == iId))
                {
                    errMsg = "该优惠券规则信息不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                //开启EF事务
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        var data_CouponInfo = data_CouponInfoService.GetModels(p => p.ID == iId).FirstOrDefault();
                        data_CouponInfoService.DeleteModel(data_CouponInfo);

                        var data_CouponList = data_CouponListService.GetModels(p => p.CouponID == iId);
                        foreach (var model in data_CouponList)
                        {
                            data_CouponListService.DeleteModel(model);
                        }

                        if (!data_CouponInfoService.SaveChanges())
                        {
                            errMsg = "删除优惠券规则信息失败";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        ts.Complete();
                    }
                    catch (Exception ex)
                    {
                        return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, ex.Message);
                    }
                }

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }        
    }
}