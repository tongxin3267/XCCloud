using System;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.Business.XCGameMana;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCGameManager;
using XCCloudService.Model.XCGame;
using XCCloudService.SocketService.UDP;
using XCCloudService.SocketService.UDP.Factory;
using XCCloudService.Model.CustomModel.Common;
using XCCloudService.Business.Common;
using XCCloudService.Model.Socket.UDP;
using XXCloudService.Utility;

namespace XXCloudService.Api.XCGame
{
    /// <summary>
    /// Ticket 的摘要说明
    /// </summary>
    public class Ticket : ApiBase
    {
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameManaUserToken)]
        public object getTicketInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                XCCloudManaUserTokenModel userTokenModel = (XCCloudManaUserTokenModel)(dicParas[Constant.XCGameManaUserToken]);
                string barCode = dicParas.ContainsKey("barCode") ? dicParas["barCode"].ToString() : string.Empty;

                if (string.IsNullOrEmpty(barCode))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入条码编号");
                }

                //验证是否有效门店
                StoreCacheModel storeModel = null;
                StoreBusiness storeBusiness = new StoreBusiness();
                if (!storeBusiness.IsEffectiveStore(userTokenModel.StoreId,ref storeModel,out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                if (storeModel.StoreDBDeployType == 0)
                {
                    //验证门票是否有效
                    XCCloudService.BLL.IBLL.XCGame.IProject_buy_codelistService codeListService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IProject_buy_codelistService>(storeModel.StoreDBName);
                    var codeListModel = codeListService.GetModels(p => p.Barcode.Equals(barCode)).FirstOrDefault<flw_project_buy_codelist>();
                    if (codeListModel == null)
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票不存在");
                    }

                    //验证门票项目是否有效
                    XCCloudService.BLL.IBLL.XCGame.IProjectService projectService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IProjectService>(storeModel.StoreDBName);
                    var projectModel = projectService.GetModels(p => p.id == codeListModel.ProjectID).FirstOrDefault<t_project>();
                    if (projectModel == null)
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票项目不存在");
                    }

                    //验证购买记录
                    XCCloudService.BLL.IBLL.XCGame.IProject_buyService projectBuyService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IProject_buyService>(storeModel.StoreDBName);
                    var projectBuyModel = projectBuyService.GetModels(p => p.ID == codeListModel.BuyID).FirstOrDefault<flw_project_buy>();
                    if (projectBuyModel == null)
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票购买记录不存在");
                    }

                    string icCardId = projectBuyModel.ICCardID;
                    string projectName = projectModel.ProjectName;
                    string stateName = getTicketStuatsName(Convert.ToInt32(codeListModel.State));
                    string projectTypeName = getConsumptionType(Convert.ToInt32(codeListModel.ProjectType));

                    //消费类型：0 次数 1有效期
                    if (Convert.ToInt32(codeListModel.ProjectType) == 0)
                    {
                        var obj = new
                        {
                            id = codeListModel.ID,
                            projectName = projectName,
                            state = stateName,
                            projectType = projectTypeName,
                            remainCount = codeListModel.RemainCount,
                            endTime = Convert.ToDateTime(codeListModel.EndTime).ToString("yyyy-MM-dd")
                        };
                        return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, obj);
                    }
                    else if (Convert.ToInt32(codeListModel.ProjectType) == 1)
                    {
                        var obj = new
                        {
                            id = codeListModel.ID,
                            projectName = projectName,
                            state = stateName,
                            projectType = projectTypeName,
                            remainCount = 0,
                            endTime = Convert.ToDateTime(codeListModel.EndTime).ToString("yyyy-MM-dd")
                        };
                        return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, obj);
                    }
                    else
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "");
                    }
                }
                else if (storeModel.StoreDBDeployType == 1)
                {
                    string sn = System.Guid.NewGuid().ToString().Replace("-","");
                    UDPSocketCommonQueryAnswerModel answerModel = null;
                    string radarToken = string.Empty;
                    if (DataFactory.SendDataTicketQuery(sn, storeModel.StoreID.ToString(), storeModel.StorePassword, barCode,out radarToken, out errMsg))
                    {
                        answerModel = new UDPSocketCommonQueryAnswerModel(sn, storeModel.StoreID.ToString(), storeModel.StorePassword, 0, null, radarToken);
                        UDPSocketCommonQueryAnswerBusiness.AddAnswer(sn, answerModel);
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
                        TicketQueryResultNotifyRequestModel model = (TicketQueryResultNotifyRequestModel)(answerModel.Result);
                        //移除应答缓存数据
                        UDPSocketCommonQueryAnswerBusiness.Remove(sn);

                        if (model.Result_Code == "1")
                        {
                            var obj = new
                            {
                                id = model.Result_Data.Id,
                                projectName = model.Result_Data.ProjectName,
                                state = model.Result_Data.State,
                                projectType = model.Result_Data.ProjectType,
                                remainCount = model.Result_Data.RemainCount,
                                endTime = model.Result_Data.endtime
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
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店设置错误");
                }
            }
            catch(Exception e)
            {
                throw e;   
            }
            
        }

        private string getTicketStuatsName(int state)
        {
            switch (state)
            {
                case 0: return "未使用";
                case 1: return "已使用";
                case 2: return "被锁定";
            }
            return "";
        }

        private string getConsumptionType(int projectType)
        { 
            switch (projectType)
            {
                case 0: return "次数";
                case 1: return "有效期";
            }
            return "";
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameManaUserToken)]
        public object addTicket(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                XCCloudManaUserTokenModel userTokenModel = (XCCloudManaUserTokenModel)(dicParas[Constant.XCGameManaUserToken]);
                string barCode = dicParas.ContainsKey("barCode") ? dicParas["barCode"].ToString() : string.Empty;

                if (string.IsNullOrEmpty(barCode))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入条码编号");
                }

                //验证是否有效门店
                //验证是否有效门店
                StoreCacheModel storeModel = null;
                StoreBusiness storeBusiness = new StoreBusiness();
                if (!storeBusiness.IsEffectiveStore(userTokenModel.StoreId,ref storeModel,out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                if (storeModel.StoreDBDeployType == 0)
                {
                    //验证门票是否有效
                    XCCloudService.BLL.IBLL.XCGame.IProject_buy_codelistService codeListService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IProject_buy_codelistService>(storeModel.StoreDBName);
                    var codeListModel = codeListService.GetModels(p => p.Barcode.Equals(barCode)).FirstOrDefault<flw_project_buy_codelist>();
                    if (codeListModel == null)
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票不存在");
                    }

                    if (codeListModel.State == 1)
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票已使用");
                    }

                    //验证门票项目是否有效
                    XCCloudService.BLL.IBLL.XCGame.IProjectService projectService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IProjectService>(storeModel.StoreDBName);
                    var projectModel = projectService.GetModels(p => p.id == codeListModel.ProjectID).FirstOrDefault<t_project>();
                    if (projectModel == null)
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票项目不存在");
                    }

                    //验证购买记录
                    XCCloudService.BLL.IBLL.XCGame.IProject_buyService projectBuyService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IProject_buyService>(storeModel.StoreDBName);
                    var projectBuyModel = projectBuyService.GetModels(p => p.ID == codeListModel.BuyID).FirstOrDefault<flw_project_buy>();
                    if (projectBuyModel == null)
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票购买记录不存在");
                    }

                    string icCardId = projectBuyModel.ICCardID;

                    //消费类型：0 次数 1有效期
                    if (Convert.ToInt32(codeListModel.ProjectType) == 0)
                    {
                        long currentDate = Utils.ConvertDateTimeToLong(System.DateTime.Now);
                        long startDate = Utils.ConvertDateTimeToLong(Convert.ToDateTime(codeListModel.StartTime));
                        long endDate = Utils.ConvertDateTimeToLong(Convert.ToDateTime(codeListModel.EndTime));
                        if (currentDate < startDate)
                        {
                            return ResponseModelFactory.CreateAnonymousFailModel(isSignKeyReturn, "门票未到使用日期");
                        }
                        else if (currentDate > endDate)
                        {
                            return ResponseModelFactory.CreateAnonymousFailModel(isSignKeyReturn, "门票已超过使用日期");
                        }
                        else if (codeListModel.BuyCount <= codeListModel.RemainCount)
                        {
                            return ResponseModelFactory.CreateAnonymousFailModel(isSignKeyReturn, "门票次数已用尽");
                        }

                        using (TransactionScope ts = new TransactionScope())
                        {
                            try
                            {
                                codeListModel.RemainCount -= 1;
                                codeListService.Update(codeListModel);

                                XCCloudService.BLL.IBLL.XCGame.IProjectPlayService projectPlayService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IProjectPlayService>(storeModel.StoreDBName);
                                flw_project_play play = new flw_project_play();
                                play.BarCode = barCode;
                                play.ICCardID = icCardId;
                                play.InTime = System.DateTime.Now;
                                play.CheckType = 2;
                                play.BuyID = codeListModel.BuyID;
                                projectPlayService.Add(play);

                                ts.Complete();

                                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                        }
                    }
                    else if (Convert.ToInt32(codeListModel.ProjectType) == 1)
                    {
                        long currentDate = Utils.ConvertDateTimeToLong(System.DateTime.Now);
                        long startDate = Utils.ConvertDateTimeToLong(Convert.ToDateTime(codeListModel.StartTime));
                        long endDate = Utils.ConvertDateTimeToLong(Convert.ToDateTime(codeListModel.EndTime));
                        if (currentDate < startDate)
                        {
                            return ResponseModelFactory.CreateAnonymousFailModel(isSignKeyReturn, "门票未到使用日期");
                        }
                        else if (currentDate > endDate)
                        {
                            return ResponseModelFactory.CreateAnonymousFailModel(isSignKeyReturn, "门票已超过使用日期");
                        }
                        else
                        {
                            XCCloudService.BLL.IBLL.XCGame.IProjectPlayService projectPlayService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IProjectPlayService>(storeModel.StoreDBName);
                            flw_project_play play = new flw_project_play();
                            play.BarCode = barCode;
                            play.ICCardID = icCardId;
                            play.InTime = System.DateTime.Now;
                            play.CheckType = 2;
                            play.BuyID = codeListModel.BuyID;
                            projectPlayService.Add(play);
                            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
                        }
                    }
                    else
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "");
                    }
                }
                else if (storeModel.StoreDBDeployType == 1)
                {
                    string sn = System.Guid.NewGuid().ToString().Replace("-","");
                    UDPSocketCommonQueryAnswerModel answerModel = null;
                    string radarToken = string.Empty;
                    if (DataFactory.SendDataTicketOperate(sn,storeModel.StoreID.ToString(), storeModel.StorePassword, barCode, "0", out radarToken,out errMsg))
                    {
                        answerModel = new UDPSocketCommonQueryAnswerModel(sn, storeModel.StoreID.ToString(), storeModel.StorePassword, 0, null, radarToken);
                        UDPSocketCommonQueryAnswerBusiness.AddAnswer(sn, answerModel);
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
                        TicketOperateResultNotifyRequestModel model = (TicketOperateResultNotifyRequestModel)(answerModel.Result);
                        //移除应答缓存数据
                        UDPSocketCommonQueryAnswerBusiness.Remove(sn);

                        if (model.Result_Code == "1" && model.Result_Data == "1")
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
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店设置错误");
                }
            }
            catch(Exception e)
            {
                throw e;
            }

        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameManaUserToken)]
        public object lockTicket(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            XCCloudManaUserTokenModel userTokenModel = (XCCloudManaUserTokenModel)(dicParas[Constant.XCGameManaUserToken]);
            string barCode = dicParas.ContainsKey("barCode") ? dicParas["barCode"].ToString() : string.Empty;

            if (string.IsNullOrEmpty(barCode))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入条码编号");
            }

            //验证是否有效门店
            StoreCacheModel storeModel = null;
            StoreBusiness storeBusiness = new StoreBusiness();
            if (!storeBusiness.IsEffectiveStore(userTokenModel.StoreId, ref storeModel, out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
            }

            if (storeModel.StoreDBDeployType == 0)
            {
                //验证门票是否有效
                XCCloudService.BLL.IBLL.XCGame.IProject_buy_codelistService codeListService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IProject_buy_codelistService>(storeModel.StoreDBName);
                var codeListModel = codeListService.GetModels(p => p.Barcode.Equals(barCode)).FirstOrDefault<flw_project_buy_codelist>();
                if (codeListModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票不存在");
                }

                if (codeListModel.State == 1)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票已使用不能锁定");
                }

                if (codeListModel.State == 2)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票已锁定");
                }

                //验证门票项目是否有效
                XCCloudService.BLL.IBLL.XCGame.IProjectService projectService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IProjectService>(storeModel.StoreDBName);
                var projectModel = projectService.GetModels(p => p.id == codeListModel.ProjectID).FirstOrDefault<t_project>();
                if (projectModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票项目不存在");
                }

                //验证购买记录
                XCCloudService.BLL.IBLL.XCGame.IProject_buyService projectBuyService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IProject_buyService>(storeModel.StoreDBName);
                var projectBuyModel = projectBuyService.GetModels(p => p.ID == codeListModel.BuyID).FirstOrDefault<flw_project_buy>();
                if (projectBuyModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票购买记录不存在");
                }

                //消费类型：0 次数 1有效期
                long currentDate = Utils.ConvertDateTimeToLong(System.DateTime.Now);
                long startDate = Utils.ConvertDateTimeToLong(Convert.ToDateTime(codeListModel.StartTime));
                long endDate = Utils.ConvertDateTimeToLong(Convert.ToDateTime(codeListModel.EndTime));
                if (Convert.ToInt32(codeListModel.ProjectType) == 0)
                {
                    if (currentDate < startDate)
                    {
                        return ResponseModelFactory.CreateAnonymousFailModel(isSignKeyReturn, "门票未到使用日期");
                    }
                    else if (currentDate > endDate)
                    {
                        return ResponseModelFactory.CreateAnonymousFailModel(isSignKeyReturn, "门票已超过使用日期");
                    }
                    else if (codeListModel.BuyCount <= codeListModel.RemainCount)
                    {
                        return ResponseModelFactory.CreateAnonymousFailModel(isSignKeyReturn, "门票次数已用尽");
                    }
                }
                else if (Convert.ToInt32(codeListModel.ProjectType) == 1)
                {
                    if (currentDate < startDate)
                    {
                        return ResponseModelFactory.CreateAnonymousFailModel(isSignKeyReturn, "门票未到使用日期");
                    }
                    else if (currentDate > endDate)
                    {
                        return ResponseModelFactory.CreateAnonymousFailModel(isSignKeyReturn, "门票已超过使用日期");
                    }
                    else
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "");
                    }
                }
                else
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "");
                }

                //锁定门票
                codeListModel.State = 2;
                if (codeListService.Update(codeListModel))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
                }
                else
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票锁定出错");
                }
            }
            else if (storeModel.StoreDBDeployType == 1)
            {
                string sn = System.Guid.NewGuid().ToString().Replace("-", "");
                UDPSocketCommonQueryAnswerModel answerModel = null;
                string radarToken = string.Empty;
                if (DataFactory.SendDataTicketOperate(sn, storeModel.StoreID.ToString(), storeModel.StorePassword, barCode, "2", out radarToken,out errMsg))
                {
                    answerModel = new UDPSocketCommonQueryAnswerModel(sn, storeModel.StoreID.ToString(), storeModel.StorePassword, 0, null, radarToken);
                    UDPSocketCommonQueryAnswerBusiness.AddAnswer(sn, answerModel);
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
                    TicketOperateResultNotifyRequestModel model = (TicketOperateResultNotifyRequestModel)(answerModel.Result);
                    //移除应答缓存数据
                    UDPSocketCommonQueryAnswerBusiness.Remove(sn);

                    if (model.Result_Code == "1" && model.Result_Data == "1")
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
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "系统没有响应");
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameManaUserToken)]
        public object unlockTicket(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                XCCloudManaUserTokenModel userTokenModel = (XCCloudManaUserTokenModel)(dicParas[Constant.XCGameManaUserToken]);
                string barCode = dicParas.ContainsKey("barCode") ? dicParas["barCode"].ToString() : string.Empty;

                if (string.IsNullOrEmpty(barCode))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入条码编号");
                }

                //验证是否有效门店
                StoreCacheModel storeModel = null;
                StoreBusiness storeBusiness = new StoreBusiness();
                if (!storeBusiness.IsEffectiveStore(userTokenModel.StoreId, ref storeModel, out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                if (storeModel.StoreDBDeployType == 0)
                { 
                    //验证门票是否有效
                    XCCloudService.BLL.IBLL.XCGame.IProject_buy_codelistService codeListService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IProject_buy_codelistService>(storeModel.StoreDBName);
                    var codeListModel = codeListService.GetModels(p => p.Barcode.Equals(barCode)).FirstOrDefault<flw_project_buy_codelist>();
                    if (codeListModel == null)
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票不存在");
                    }

                    if (codeListModel.State == 1)
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票已使用不能锁定");
                    }

                    if (codeListModel.State == 0)
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票未锁定");
                    }

                    //验证门票项目是否有效
                    XCCloudService.BLL.IBLL.XCGame.IProjectService projectService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IProjectService>(storeModel.StoreDBName);
                    var projectModel = projectService.GetModels(p => p.id == codeListModel.ProjectID).FirstOrDefault<t_project>();
                    if (projectModel == null)
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票项目不存在");
                    }

                    //验证购买记录
                    XCCloudService.BLL.IBLL.XCGame.IProject_buyService projectBuyService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IProject_buyService>(storeModel.StoreDBName);
                    var projectBuyModel = projectBuyService.GetModels(p => p.ID == codeListModel.BuyID).FirstOrDefault<flw_project_buy>();
                    if (projectBuyModel == null)
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票购买记录不存在");
                    }

                    //锁定门票
                    codeListModel.State = 2;
                    long currentDate = Utils.ConvertDateTimeToLong(System.DateTime.Now);
                    long startDate = Utils.ConvertDateTimeToLong(Convert.ToDateTime(codeListModel.StartTime));
                    long endDate = Utils.ConvertDateTimeToLong(Convert.ToDateTime(codeListModel.EndTime));
                    if (currentDate > endDate)
                    {
                        codeListModel.State = 1;
                    }
                    else if (codeListModel.BuyCount <= codeListModel.RemainCount)
                    {
                        codeListModel.State = 1;
                    }

                    if (codeListService.Update(codeListModel))
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
                    }
                    else
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门票锁定出错");
                    }                    
                }
                else if (storeModel.StoreDBDeployType == 1)
                {
                    string sn = System.Guid.NewGuid().ToString().Replace("-", "");
                    UDPSocketCommonQueryAnswerModel answerModel = null;
                    string radarToken = string.Empty;
                    if (DataFactory.SendDataTicketOperate(sn,storeModel.StoreID.ToString(), storeModel.StorePassword, barCode, "1",out radarToken, out errMsg))
                    {
                        answerModel = new UDPSocketCommonQueryAnswerModel(sn, storeModel.StoreID.ToString(), storeModel.StorePassword, 0, null, radarToken);
                        UDPSocketCommonQueryAnswerBusiness.AddAnswer(sn, answerModel);
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
                        TicketOperateResultNotifyRequestModel model = (TicketOperateResultNotifyRequestModel)(answerModel.Result);
                        //移除应答缓存数据
                        UDPSocketCommonQueryAnswerBusiness.Remove(sn);

                        if (model.Result_Code == "1" && model.Result_Data == "1")
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
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店设置错误");
                }
            }
            catch(Exception e)
            {
                throw e;   
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameManaUserToken)]
        public object exchangeOutTicket(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            decimal money = 0;
            XCCloudManaUserTokenModel userTokenModel = (XCCloudManaUserTokenModel)(dicParas[Constant.XCGameManaUserToken]);
            string barCode = dicParas.ContainsKey("barCode") ? dicParas["barCode"].ToString() : string.Empty;
            string icCardId = dicParas.ContainsKey("icCardId") ? dicParas["icCardId"].ToString() : string.Empty;
            string mobileName = dicParas.ContainsKey("mobileName") ? dicParas["mobileName"].ToString() : string.Empty;
            string moneyStr = dicParas.ContainsKey("money") ? dicParas["money"].ToString() : string.Empty;

            if (string.IsNullOrEmpty(barCode))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入条码编号");
            }

            if (string.IsNullOrEmpty(icCardId))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入会员卡号");
            }

            if (!String.IsNullOrEmpty(moneyStr) && !Utils.IsDecimal(moneyStr))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "兑币金额无效");
            }

            StoreCacheModel storeModel = null;
            StoreBusiness storeBusiness = new StoreBusiness();
            if (!storeBusiness.IsEffectiveStore(userTokenModel.StoreId, ref storeModel, out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
            }

            if (storeModel.StoreDBDeployType == 0)
            {
                XCCloudService.BLL.IBLL.XCGame.IFlwTicketExitService flwTicketExitService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IFlwTicketExitService>(storeModel.StoreDBName);
                XCCloudService.BLL.IBLL.XCGame.IMemberService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IMemberService>(storeModel.StoreDBName);
                XCCloudService.BLL.IBLL.XCGame.IScheduleService scheduleService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IScheduleService>(storeModel.StoreDBName);
                XCCloudService.BLL.IBLL.XCGame.IParametersService parametersService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IParametersService>(storeModel.StoreDBName);

                var memberModel = memberService.GetModels(p => p.ICCardID.ToString().Equals(icCardId)).FirstOrDefault<t_member>();
                if (memberModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "会员信息不存在");
                }

                System.DateTime startTime = System.DateTime.Parse(System.DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00");
                System.DateTime endTime = System.DateTime.Parse(System.DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59");
                var scheduleModel = scheduleService.GetModels(p => (p.OpenTime >= startTime && p.OpenTime <= endTime && p.UserID == userTokenModel.XCGameUserId && p.State.Equals("0"))).FirstOrDefault<flw_schedule>();
                if (scheduleModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "用户班次信息不存在");
                }

                var flwTicketExitModel = flwTicketExitService.GetModels(p => p.Barcode.Equals(barCode)).FirstOrDefault<flw_ticket_exit>();
                if (flwTicketExitModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "出票信息不存在");
                }

                if (flwTicketExitModel.State == 1)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "票已兑换");
                }

                if (flwTicketExitModel.isNoAllow == 1)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "票禁止兑换");
                }

                var paramCoinPriceModel = parametersService.GetModels(p => p.System.Equals("txtCoinPrice", StringComparison.OrdinalIgnoreCase)).FirstOrDefault<t_parameters>();
                var paramDateValidityModel = parametersService.GetModels(p => p.System.Equals("rbnBackDateValidity", StringComparison.OrdinalIgnoreCase)).FirstOrDefault<t_parameters>();

                if (paramCoinPriceModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "兑币单价参数未设置，不能兑换");
                }

                if (paramDateValidityModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "返分卡有效期参数未设置，不能兑换");
                }

                if (flwTicketExitModel.Coins <= 0)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "票兑换金额无效，不能兑换");
                }

                decimal exCoinMoney = Convert.ToDecimal(flwTicketExitModel.Coins) * decimal.Parse(paramCoinPriceModel.ParameterValue);

                DateTime dateTime = Convert.ToDateTime(flwTicketExitModel.RealTime).AddDays(Convert.ToDouble(paramDateValidityModel.ParameterValue));
                if (System.DateTime.Now > dateTime)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "已过期，不能兑换");
                }

                using (TransactionScope ts = new TransactionScope())
                {
                    memberModel.Balance += flwTicketExitModel.Coins;
                    memberModel.ModTime = System.DateTime.Now;
                    memberService.Update(memberModel);

                    flwTicketExitModel.ICCardID = int.Parse(icCardId);
                    flwTicketExitModel.ChargeTime = System.DateTime.Now;
                    flwTicketExitModel.WorkStation = userTokenModel.Mobile;
                    flwTicketExitModel.MacAddress = userTokenModel.Mobile;
                    flwTicketExitModel.DiskID = userTokenModel.Mobile;
                    flwTicketExitModel.Note = "小程序兑换";
                    flwTicketExitModel.CoinMoney = exCoinMoney;
                    flwTicketExitModel.UserID = userTokenModel.XCGameUserId.ToString();
                    flwTicketExitModel.State = 1;
                    flwTicketExitModel.ScheduleID = scheduleModel.ID;
                    flwTicketExitService.Update(flwTicketExitModel);
                    ts.Complete();
                }
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }
            else if (storeModel.StoreDBDeployType == 1)
            {
                string sn = System.Guid.NewGuid().ToString().Replace("-", "");
                UDPSocketCommonQueryAnswerModel answerModel = null;
                string radarToken = string.Empty;
                if (DataFactory.SendDataOutTicketOperate(sn, storeModel.StoreID.ToString(), storeModel.StorePassword, barCode, icCardId, mobileName, userTokenModel.Mobile, money, "0", out radarToken, out errMsg))
                {
                    answerModel = new UDPSocketCommonQueryAnswerModel(sn, storeModel.StoreID.ToString(), storeModel.StorePassword, 0, null, radarToken);
                    UDPSocketCommonQueryAnswerBusiness.AddAnswer(sn, answerModel);
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
                    OutTicketOperateResultNotifyRequestModel model = (OutTicketOperateResultNotifyRequestModel)(answerModel.Result);
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
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "门店设置错误");
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameManaUserToken)]
        public object getOutTicket(Dictionary<string, object> dicParas)
        {
            XCCloudManaUserTokenModel userTokenModel = (XCCloudManaUserTokenModel)(dicParas[Constant.XCGameManaUserToken]);
            string barCode = dicParas.ContainsKey("barCode") ? dicParas["barCode"].ToString() : string.Empty;

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

            if (string.IsNullOrEmpty(barCode))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "请输入票编号");
            }

            if (storeModel.StoreDBDeployType == 0)
            {
                XCCloudService.BLL.IBLL.XCGame.IParametersService parametersService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IParametersService>(storeModel.StoreDBName);
                var paramDateValidityModel = parametersService.GetModels(p => p.System.Equals("rbnBackDateValidity", StringComparison.OrdinalIgnoreCase)).FirstOrDefault<t_parameters>();
                if (paramDateValidityModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "返分卡有效期参数未设置，不能兑换");
                }

                var paramCoinPriceModel = parametersService.GetModels(p => p.System.Equals("txtCoinPrice", StringComparison.OrdinalIgnoreCase)).FirstOrDefault<t_parameters>();
                if (paramCoinPriceModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "兑币单价参数未设置，不能兑换");
                }
                decimal exchangeCoinPrice = decimal.Parse(paramCoinPriceModel.ParameterValue);

                XCCloudService.BLL.IBLL.XCGame.IFlwTicketExitService flwTicketExitService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IFlwTicketExitService>(storeModel.StoreDBName);
                var flwTicketExitModel = flwTicketExitService.GetModels(p => p.Barcode.Equals(barCode,StringComparison.OrdinalIgnoreCase)).FirstOrDefault<flw_ticket_exit>();
                if (flwTicketExitModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "出票信息不存在");
                }

                XCCloudService.BLL.IBLL.XCGame.IHeadService headService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IHeadService>(storeModel.StoreDBName);
                var headModel = headService.GetModels(p => p.HeadAddress.Equals(flwTicketExitModel.HeadAddress) && p.Segment.Equals(flwTicketExitModel.Segment)).FirstOrDefault<t_head>();
                if (headModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "彩票出票设备信息不存在");
                }

                XCCloudService.BLL.IBLL.XCGame.IGameService gameService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IGameService>(storeModel.StoreDBName);
                var gameModel = gameService.GetModels(p => p.GameID.Equals(headModel.GameID)).FirstOrDefault<t_game>();
                if (gameModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "彩票对应游戏信息不存在");
                }

                int state = Convert.ToInt32(flwTicketExitModel.State);
                string stateName = flwTicketExitModel.State == 1 ? "已兑" : "未兑";
                if (state != 1)
                {
                    DateTime dateTime = Convert.ToDateTime(flwTicketExitModel.RealTime).AddDays(Convert.ToDouble(paramDateValidityModel.ParameterValue));
                    if (System.DateTime.Now > dateTime)
                    {
                        state = 2;
                        stateName = "已过期";
                    }

                    if (flwTicketExitModel.isNoAllow == 1)
                    {
                        state = 3;
                        stateName = "被锁定";
                    }
                }

                var obj = new {
                    id = flwTicketExitModel.ID,
                    coin = flwTicketExitModel.Coins,
                    gameName = gameModel.GameName,
                    headInfo = headModel.HeadID,
                    state = stateName,
                    makeTime = Convert.ToDateTime(flwTicketExitModel.RealTime).ToString("yyyy-MM-dd HH:mm:ss"),
                    exchangeCoinPrice = exchangeCoinPrice
                };

                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, obj);
            }
            else if (storeModel.StoreDBDeployType == 1)
            {
                string txtCoinPrice = string.Empty;
                string txtTicketDate = string.Empty;
                ParamQueryResultModel paramQueryResultModel = null;
                if (UDPApiService.GetParam(userTokenModel.StoreId, "0", ref paramQueryResultModel, out errMsg))
                {
                    txtCoinPrice = paramQueryResultModel.TxtCoinPrice;
                    txtTicketDate = paramQueryResultModel.TxtTicketDate;
                }
                else
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, errMsg); 
                }

                string sn = System.Guid.NewGuid().ToString().Replace("-", "");
                UDPSocketCommonQueryAnswerModel answerModel = null;
                string radarToken = string.Empty;
                if (DataFactory.SendDataOutTicketQuery(sn,storeModel.StoreID.ToString(), storeModel.StorePassword, barCode,out radarToken,out errMsg))
                {
                    answerModel = new UDPSocketCommonQueryAnswerModel(sn, storeModel.StoreID.ToString(), storeModel.StorePassword, 0, null, radarToken);
                    UDPSocketCommonQueryAnswerBusiness.AddAnswer(sn, answerModel);
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
                    OutTicketQueryResultNotifyRequestModel model = (OutTicketQueryResultNotifyRequestModel)(answerModel.Result);
                    //移除应答缓存数据
                    UDPSocketCommonQueryAnswerBusiness.Remove(sn);

                    if (model.Result_Code == "1")
                    {
                        var obj = new
                        {
                            id = model.Result_Data.Id,
                            coin = model.Result_Data.Coins,
                            gameName = model.Result_Data.GameName,
                            headInfo = model.Result_Data.HeadInfo,
                            state = model.Result_Data.State,
                            makeTime = model.Result_Data.PrintDate,
                            exchangeCoinPrice = txtCoinPrice
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
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店设置错误");
            }
        }
    }
}