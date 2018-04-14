using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.XCCloud;
using XXCloudService.Api.XCCloud.Common;

namespace XXCloudService.Api.XCCloud
{
    [Authorize(Roles = "StoreUser")]
    /// <summary>
    /// GameInfo 的摘要说明
    /// </summary>
    public class GameInfo : ApiBase
    {
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetGameInfoList(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;

                IDict_SystemService dict_SystemService = BLLContainer.Resolve<IDict_SystemService>(resolveNew: true);
                int GameTypeId = dict_SystemService.GetModels(p => p.DictKey.Equals("游戏机类型")).FirstOrDefault().ID;

                IData_GameInfoService data_GameInfoService = BLLContainer.Resolve<IData_GameInfoService>(resolveNew: true);
                var data_GameInfo = from a in data_GameInfoService.GetModels(p => p.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase))
                                    join b in dict_SystemService.GetModels(p => p.PID == GameTypeId) on a.GameType equals b.DictValue into b1
                                    from b in b1.DefaultIfEmpty()
                                    orderby a.GameID
                                    select new
                                    {
                                        ID = a.ID,
                                        GameID = a.GameID,
                                        GameName = a.GameName,
                                        GameTypeStr = b != null ? b.DictKey : string.Empty,
                                        PushReduceFromCard = a.PushReduceFromCard,
                                        AllowElecPushStr = a.AllowElecPush != null ? (a.AllowElecPush == 1 ? "启用" : "禁用") : "",
                                        LotteryModeStr = a.LotteryMode != null ? (a.LotteryMode == 1 ? "启用" : "禁用") : "",
                                        ReadCatStr = a.ReadCat != null ? (a.ReadCat == 1 ? "启用" : "禁用") : "",
                                        StateStr = !string.IsNullOrEmpty(a.State) ? (a.State == "1" ? "启用" : "禁用") : ""
                                    };


                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, data_GameInfo);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetStoreGameInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;
                IData_GameInfoService data_GameInfoService = BLLContainer.Resolve<IData_GameInfoService>();
                Dictionary<int, string> gameInfo = data_GameInfoService.GetModels(p => p.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase)).Select(o => new
                {
                    ID = o.ID,
                    GameName = o.GameName
                }).Distinct().ToDictionary(d => d.ID, d => d.GameName);

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, gameInfo);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetGameInfo(Dictionary<string, object> dicParas)
        {
            try
            {                
                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? (dicParas["id"] + "") : string.Empty;
                if (string.IsNullOrEmpty(id))
                {
                    errMsg = "游戏机流水号不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iId = Convert.ToInt32(id);

                IData_GameInfoService data_GameInfoService = BLLContainer.Resolve<IData_GameInfoService>(resolveNew: true);
                if (!data_GameInfoService.Any(a => a.ID == iId))
                {
                    errMsg = "该游戏机信息不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                IDict_SystemService dict_SystemService = BLLContainer.Resolve<IDict_SystemService>(resolveNew: true);
                int GameInfoId = dict_SystemService.GetModels(p => p.DictKey.Equals("游戏机档案维护")).FirstOrDefault().ID;
                var result = from a in data_GameInfoService.GetModels(p => p.ID == iId).FirstOrDefault().AsDictionary()
                             join b in dict_SystemService.GetModels(p => p.PID == GameInfoId) on a.Key equals b.DictKey into b1
                             from b in b1.DefaultIfEmpty()
                             select new { 
                                 name = a.Key,
                                 value = a.Value,
                                 comment = b != null ? b.Comment : string.Empty
                             };

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, result);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object SaveGameInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;

                string errMsg = string.Empty;
                string gameId = dicParas.ContainsKey("GameID") ? (dicParas["GameID"] + "") : string.Empty;
                string id = dicParas.ContainsKey("ID") ? (dicParas["ID"] + "") : string.Empty;
                string gameName = dicParas.ContainsKey("GameName") ? (dicParas["GameName"] + "") : string.Empty;

                if (string.IsNullOrEmpty(gameId))
                {
                    errMsg = "游戏机编号GameID不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }
                if (gameId.Length > 4)
                {
                    errMsg = "游戏机编号GameID不能超过4个字符";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }
                if (string.IsNullOrEmpty(gameName))
                {
                    errMsg = "游戏机名称GameName不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }
                if (!string.IsNullOrEmpty(id) && !Utils.isNumber(id))
                {
                    errMsg = "游戏机参数ID格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                IData_GameInfoService data_GameInfoService = BLLContainer.Resolve<IData_GameInfoService>();
                var data_GameInfo = new Data_GameInfo();                
                if (string.IsNullOrEmpty(id))
                {                    
                    if (data_GameInfoService.Any(a => a.GameID.Equals(gameId, StringComparison.OrdinalIgnoreCase)))
                    {
                        errMsg = "该游戏机编号已使用";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    Utils.GetModel(dicParas, ref data_GameInfo);
                    data_GameInfo.StoreID = storeId;
                    if (!data_GameInfoService.Add(data_GameInfo))
                    {
                        errMsg = "新增游戏机信息失败";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                }
                else
                {
                    int iId = Convert.ToInt32(id);
                    if (!data_GameInfoService.Any(a => a.ID == iId))
                    {
                        errMsg = "该游戏机不存在";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                    if (data_GameInfoService.Any(a => a.ID != iId && a.GameID.Equals(gameId, StringComparison.OrdinalIgnoreCase)))
                    {
                        errMsg = "该游戏机编号已使用";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    data_GameInfo = data_GameInfoService.GetModels(p => p.ID == iId).FirstOrDefault();
                    Utils.GetModel(dicParas, ref data_GameInfo);
                    data_GameInfo.StoreID = storeId;
                    if (!data_GameInfoService.Update(data_GameInfo))
                    {
                        errMsg = "修改游戏机信息失败";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                }
                
                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
            }
            catch (DbEntityValidationException e)
            {
                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, e.EntityValidationErrors.ToErrors());
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object DelGameInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string id = dicParas.ContainsKey("id") ? (dicParas["id"] + "") : string.Empty;
                if (string.IsNullOrEmpty(id))
                {
                    errMsg = "游戏机流水号不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iId = Convert.ToInt32(id);
                IData_GameInfoService data_GameInfoService = BLLContainer.Resolve<IData_GameInfoService>();
                if (!data_GameInfoService.Any(a => a.ID == iId))
                {
                    errMsg = "该游戏机信息不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                var data_GameInfo = data_GameInfoService.GetModels(p => p.ID == iId).FirstOrDefault();
                if (!data_GameInfoService.Delete(data_GameInfo))
                {
                    errMsg = "删除游戏机信息失败";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
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