using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.Business.Common;
using XCCloudService.Business.XCGameMana;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.Common;
using XCCloudService.Model.CustomModel.XCGameManager;
using XCCloudService.Model.Socket.UDP;
using XCCloudService.Model.XCGame;
using XCCloudService.SocketService.UDP;
using XCCloudService.SocketService.UDP.Factory;

namespace XXCloudService.Api.XCGame
{
    /// <summary>
    /// Lottery 的摘要说明
    /// </summary>
    public class Lottery : ApiBase
    {
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameManaUserToken)]
        public object getLotteryInfo(Dictionary<string, object> dicParas)
        {
            string gameName = string.Empty;
            string headInfo = string.Empty;
            string barCode = dicParas.ContainsKey("barCode") ? dicParas["barCode"].ToString() : string.Empty;

            if (string.IsNullOrEmpty(barCode))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入条码编号");
            }

            XCCloudManaUserTokenModel userTokenModel = (XCCloudManaUserTokenModel)(dicParas[Constant.XCGameManaUserToken]);
            StoreBusiness store = new StoreBusiness();
            string errMsg = string.Empty;
            StoreCacheModel storeModel = null;
            StoreBusiness storeBusiness = new StoreBusiness();
            if (!storeBusiness.IsEffectiveStore(userTokenModel.StoreId, ref storeModel, out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
            }

            if (storeModel.StoreDBDeployType == 0)
            { 
                XCCloudService.BLL.IBLL.XCGame.IFlwLotteryService lotteryService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IFlwLotteryService>(storeModel.StoreDBName);
                XCCloudService.BLL.IBLL.XCGame.IHeadService headService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IHeadService>(storeModel.StoreDBName);
                XCCloudService.BLL.IBLL.XCGame.IGameService gameService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IGameService>(storeModel.StoreDBName);

                var lotteryModel = lotteryService.GetModels(p => p.Barcode.Equals(barCode)).FirstOrDefault<flw_lottery>();
                if (lotteryModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "彩票信息不存在");
                }

                if (!string.IsNullOrEmpty(lotteryModel.HeadID))
                {
                    //var headModel = headService.GetModels(p => p.HeadAddress.Equals(lotteryModel.HeadID)).FirstOrDefault<t_head>();
                    headInfo = lotteryModel.HeadID;
                }
                else
                {
                    headInfo = lotteryModel.WorkStation;
                }

                if (!string.IsNullOrEmpty(lotteryModel.GameID))
                {
                    var gameModel = gameService.GetModels(p => p.GameID.Equals(lotteryModel.GameID)).FirstOrDefault<t_game>();
                    if (gameModel != null)
                    {
                        gameName = gameModel.GameName;
                    }
                    else
                    {
                        gameName = string.Empty;
                    }
                }
                else
                {
                    gameName = string.Empty;
                }

                XCCloudService.BLL.IBLL.XCGame.IParametersService parametersService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IParametersService>(storeModel.StoreDBName);
                var paramDateValidityModel = parametersService.GetModels(p => p.System.Equals("rbnBackDateValidity", StringComparison.OrdinalIgnoreCase)).FirstOrDefault<t_parameters>();
                if (paramDateValidityModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "返分卡有效期参数未设置，不能兑换");
                }

                int state = Convert.ToInt32(lotteryModel.State);
                string stateName = lotteryModel.State == 1 ? "已兑" : "未兑";
                if (state != 1)
                {
                    DateTime dateTime = Convert.ToDateTime(lotteryModel.PrintTime).AddDays(Convert.ToDouble(paramDateValidityModel.ParameterValue));
                    if (System.DateTime.Now > dateTime)
                    {
                        state = 2;
                        stateName = "已过期";
                    }                
                }

                string printTime = Convert.ToDateTime(lotteryModel.PrintTime).ToString("yyyy-MM-dd HH:mm:ss");
                string realTime = Convert.ToDateTime(lotteryModel.RealTime).ToString("yyyy-MM-dd HH:mm:ss");

                var obj = new {
                    id = lotteryModel.id,					
                    lottery = lotteryModel.LotteryCount,//彩票数
                    gameName = gameName,		        //游戏机名
                    headInfo = headInfo,		        //出票位置
                    state = stateName,			        //小票状态
                    makeTime = printTime.Substring(0, 10) == "0001-01-01" ? "" : printTime, 	//出票时间                    
                };

                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, obj);                
            }
            else if (storeModel.StoreDBDeployType == 1)
            {
                string sn = System.Guid.NewGuid().ToString().Replace("-","");
                UDPSocketCommonQueryAnswerModel answerModel = null;
                string radarToken = string.Empty;
                //string storeId, string storePassword, string barCode,string icCardId,string mobileName, string phone, string operate, out string errMsg
                if (DataFactory.SendDataLotteryQuery(sn,storeModel.StoreID.ToString(), storeModel.StorePassword, barCode,out radarToken, out errMsg))
                {

                }
                else
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                answerModel = null;
                int whileCount = 0;
                while (answerModel == null && whileCount <= 25)
                {
                    //获取应答缓存数据
                    whileCount++;
                    System.Threading.Thread.Sleep(1000);
                    answerModel = UDPSocketCommonQueryAnswerBusiness.GetAnswerModel(sn, 1);
                }

                if (answerModel != null)
                {
                    LotteryQueryResultNotifyRequestModel model = (LotteryQueryResultNotifyRequestModel)(answerModel.Result);
                    //移除应答缓存数据
                    UDPSocketCommonQueryAnswerBusiness.Remove(sn);

                    if (model.Result_Code == "1")
                    {
                        var obj = new
                        {
                            id = model.Result_Data.Id,
                            lottery = model.Result_Data.Lottery,//彩票数
                            gameName = model.Result_Data.GameName,//游戏机名
                            headInfo = model.Result_Data.HeadInfo,//出票位置
                            state = model.Result_Data.State,	//小票状态
                            makeTime = model.Result_Data.PrintDate 	//出票时间                    
                        };

                        return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, obj);
                    }
                    else
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, model.Result_Msg);
                    }
                }
                else
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "系统没有响应");
                }
            }
            else
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店配置无效");
            }
        }


        #region "提取彩票"

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameManaUserToken)]
        public object exchangeLottery(Dictionary<string, object> dicParas)
        {
            XCCloudManaUserTokenModel userTokenModel = (XCCloudManaUserTokenModel)(dicParas[Constant.XCGameManaUserToken]);
            string barCode = dicParas.ContainsKey("barCode") ? dicParas["barCode"].ToString() : string.Empty;
            string icCardId = dicParas.ContainsKey("icCardId") ? dicParas["icCardId"].ToString() : string.Empty;
            string mobileName = dicParas.ContainsKey("mobileName") ? dicParas["mobileName"].ToString() : string.Empty;

            if (string.IsNullOrEmpty(barCode))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入条码编号");
            }

            StoreBusiness store = new StoreBusiness();
            string errMsg = string.Empty;
            StoreCacheModel storeModel = null;
            StoreBusiness storeBusiness = new StoreBusiness();
            if (!storeBusiness.IsEffectiveStore(userTokenModel.StoreId, ref storeModel, out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
            }

            if (storeModel.StoreDBDeployType == 0)
            {
                XCCloudService.BLL.IBLL.XCGame.IFlwLotteryService lotteryService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IFlwLotteryService>(storeModel.StoreDBName);
                XCCloudService.BLL.IBLL.XCGame.IMemberService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IMemberService>(storeModel.StoreDBName);
                XCCloudService.BLL.IBLL.XCGame.IScheduleService scheduleService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IScheduleService>(storeModel.StoreDBName);

                System.DateTime startTime = System.DateTime.Parse(System.DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00");
                System.DateTime endTime = System.DateTime.Parse(System.DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59");

                var lotteryModel = lotteryService.GetModels(p => p.Barcode.Equals(barCode)).FirstOrDefault<flw_lottery>();
                var memberModel = memberService.GetModels(p => p.ICCardID.ToString().Equals(icCardId)).FirstOrDefault<t_member>();
                var scheduleModel = scheduleService.GetModels(p => (p.OpenTime >= startTime && p.OpenTime <= endTime && p.UserID == userTokenModel.XCGameUserId && p.State.Equals("0"))).FirstOrDefault<flw_schedule>();

                if (scheduleModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户班次信息不存在");
                }

                if (lotteryModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "彩票信息不存在");
                }

                if (lotteryModel.State == 1)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "当前彩票已使用");
                }

                if (memberModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "会员信息不存在");
                }

                XCCloudService.BLL.IBLL.XCGame.IParametersService parametersService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IParametersService>(storeModel.StoreDBName);
                var paramDateValidityModel = parametersService.GetModels(p => p.System.Equals("rbnBackDateValidity", StringComparison.OrdinalIgnoreCase)).FirstOrDefault<t_parameters>();
                if (paramDateValidityModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "返分卡有效期参数未设置，不能兑换");
                }

                DateTime dateTime = Convert.ToDateTime(lotteryModel.PrintTime).AddDays(Convert.ToInt32(paramDateValidityModel.ParameterValue));
                if (System.DateTime.Now > dateTime)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "已过期，不能兑换");
                }

                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        lotteryModel.State = 1;
                        lotteryModel.RealTime = System.DateTime.Now;
                        lotteryModel.ICCardID = int.Parse(icCardId);
                        lotteryModel.UserID = userTokenModel.XCGameUserId;
                        lotteryModel.ScheduleID = scheduleModel.ID;
                        lotteryModel.WorkStation = userTokenModel.Mobile;

                        memberModel.Lottery = Convert.ToInt32(memberModel.Lottery) + lotteryModel.LotteryCount;
                        memberModel.ModTime = System.DateTime.Now;
 
                        lotteryService.Update(lotteryModel);
                        memberService.Update(memberModel);
                        ts.Complete();

                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            else if (storeModel.StoreDBDeployType == 1)
            {
                string sn = System.Guid.NewGuid().ToString().Replace("-", "");
                UDPSocketCommonQueryAnswerModel answerModel = null;
                string radarToken = string.Empty;
                //发送彩票操作
                if (DataFactory.SendDataLotteryOperate(sn, storeModel.StoreID.ToString(), storeModel.StorePassword, barCode, icCardId, "0",mobileName,userTokenModel.Mobile,out radarToken,out errMsg))
                {

                }
                else
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                answerModel = null;
                int whileCount = 0;
                while (answerModel == null && whileCount <= 25)
                {
                    //获取应答缓存数据
                    whileCount++;
                    System.Threading.Thread.Sleep(1000);
                    answerModel = UDPSocketCommonQueryAnswerBusiness.GetAnswerModel(sn, 1);
                }

                if (answerModel != null)
                {
                    LotteryOperateResultNotifyRequestModel model = (LotteryOperateResultNotifyRequestModel)(answerModel.Result);
                    //移除应答缓存数据
                    UDPSocketCommonQueryAnswerBusiness.Remove(sn);

                    if (model.Result_Code == "1")
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
                    }
                    else
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, model.Result_Msg);
                    }
                }
                else
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "系统没有响应");
                }
            }
            else
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.F, "", Result_Code.T, "门店配置无效");
            }

        }

        #endregion
    }
}