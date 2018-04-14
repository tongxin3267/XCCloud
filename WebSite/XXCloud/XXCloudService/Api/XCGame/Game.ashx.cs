using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.Business.XCGame;
using XCCloudService.Business.XCGameMana;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCGame;

namespace XXCloudService.Api.XCGame
{
    /// <summary>
    /// Game 的摘要说明
    /// </summary>
    public class Game : ApiBase
    {
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameMemberToken)]
        public object GetGameInfo(Dictionary<string, object> dicParas)
        {
            //获取token模式
            XCGameMemberTokenModel memberTokenModel = (XCGameMemberTokenModel)(dicParas[Constant.XCGameMemberTokenModel]);
            string deviceToken = dicParas.ContainsKey("deviceToken") ? dicParas["deviceToken"].ToString() : string.Empty;
            StoreBusiness store = new StoreBusiness();
            string xcGameDBName = string.Empty;
            string password = string.Empty;
            string errMsg = string.Empty;
            string storeId = string.Empty;

            //验证门店信息
            if (!store.IsEffectiveStore(memberTokenModel.StoreId, out xcGameDBName, out password, out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
            }
            //验证设备信息
            XCCloudService.Model.XCGameManager.t_device deviceModel = null;
            XCCloudService.Model.XCGame.t_member memberModel = null;
            if (!DeviceManaBusiness.ExistDevice(deviceToken, ref deviceModel,out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
            }
            //验证设备门店信息和会员门店信息
            if (!memberTokenModel.StoreId.Equals(deviceModel.StoreId))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
            }
            //验证会员信息
            if (!MemberBusiness.IsEffectiveStore(memberTokenModel.Mobile, xcGameDBName, ref memberModel, out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
            }
            //验证头信息
            XCCloudService.BLL.IBLL.XCGame.IHeadService headService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IHeadService>(xcGameDBName);
            var headModel = headService.GetModels(p => p.MCUID.Equals(deviceModel.DeviceId)).FirstOrDefault<XCCloudService.Model.XCGame.t_head>();
            if (headModel == null)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备门店头部信息不存在");
            }
            //验证游戏机信息
            XCCloudService.BLL.IBLL.XCGame.IGameService gameService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IGameService>(xcGameDBName);
            var gameModel = gameService.GetModels(p => p.GameID.Equals(headModel.GameID)).FirstOrDefault<XCCloudService.Model.XCGame.t_game>();
            if (gameModel == null)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "设备门店头部信息不存在");
            }
            //获取游戏机投币规则
            XCCloudService.BLL.IBLL.XCGame.IPushRuleService pushRuleService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IPushRuleService>(xcGameDBName);
            List<XCCloudService.Model.XCGame.t_push_rule> pushRuleModelList = pushRuleService.GetModels(p => p.game_id.Equals(headModel.GameID) && p.member_level_id == memberModel.MemberLevelID).OrderBy(p=>p.level).ToList<XCCloudService.Model.XCGame.t_push_rule>();
            List<int> pushRuleModelResultList = new List<int>();

            //验证会员是否存在优惠记录
            List<object> gameFreeRuleObjList = new List<object>();
            List<XCCloudService.Model.XCGame.t_game_free_rule> gameFreeRuleModelList = null;
            XCCloudService.BLL.IBLL.XCGame.IFlwGameFreeService flwGameFreeService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IFlwGameFreeService>(xcGameDBName);
            var flwGameFreeModel = flwGameFreeService.GetModels(p => p.GameID.Equals(headModel.GameID) && p.ICCardID.Equals(memberModel.ICCardID.ToString())).FirstOrDefault<XCCloudService.Model.XCGame.flw_game_free>();
            if (flwGameFreeModel == null)
            {
                //如果不存在优惠记录，则返回优惠信息
                XCCloudService.BLL.IBLL.XCGame.IGameFreeRuleService gameFreeRuleService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IGameFreeRuleService>(xcGameDBName);
                gameFreeRuleModelList = gameFreeRuleService.GetModels(
                                    p=>p.GameID.Equals(headModel.GameID) 
                                    && p.MemberLevelID == memberModel.MemberLevelID
                                    && p.State == "启用").ToList<XCCloudService.Model.XCGame.t_game_free_rule>();
            }

            gameFreeRuleModelList = gameFreeRuleModelList.OrderBy(p => p.FreeCoin / decimal.Parse(p.NeedCoin.ToString())).ToList<XCCloudService.Model.XCGame.t_game_free_rule>();

            for (int i = 0; i < gameFreeRuleModelList.Count; i++)
            {
                DateTime currentTime = System.DateTime.Now;
                if (currentTime >= gameFreeRuleModelList[i].StartTime && currentTime <= gameFreeRuleModelList[i].EndTime)
                {
                    var obj = new { 
                        id = gameFreeRuleModelList[i].ID,
                        needCoin = gameFreeRuleModelList[i].NeedCoin,
                        freeCoin = gameFreeRuleModelList[i].FreeCoin,
                        exitCoin = gameFreeRuleModelList[i].ExitCoin
                    };
                    gameFreeRuleObjList.Add(obj);
                }
            }

            foreach (var pushRuleModel in pushRuleModelList)
            {
                if (gameModel.PushReduceFromCard > pushRuleModel.coin)
                {
                    pushRuleModelResultList.Add(Convert.ToInt32(gameModel.PushReduceFromCard));
                }
            }

            if (pushRuleModelResultList.Count == 0)
            {
                pushRuleModelResultList.Add(Convert.ToInt32(gameModel.PushReduceFromCard));
            }

            var obj2 = new {
                gameId = gameModel.GameID,
                gameName = gameModel.GameName,
                gameType = GetGameType(gameModel.GameType),
                price = gameModel.PushReduceFromCard,
                discountPrice = pushRuleModelResultList[0],
                gameFreeRule = gameFreeRuleObjList
            };
            return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, obj2);
        }

        private string GetGameType(string type)
        {
            if (type == "平板6机头")
            { 
                return "投币类";
            }
            else if (type == "平板4机头")
            {
                return "博彩类";
            }
            else
            {
                return "";
            }
        }
    }


}