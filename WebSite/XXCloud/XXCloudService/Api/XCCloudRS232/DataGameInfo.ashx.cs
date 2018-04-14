using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using XCCloudService.Base;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloudRS232;
using XCCloudService.Business.Common;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.Model.CustomModel.XCCloudRS232;
using XCCloudService.Model.XCCloudRS232;

namespace XXCloudService.Api.XCCloudRS232
{
    /// <summary>
    /// DataGameInfo 的摘要说明
    /// </summary>
    public class DataGameInfo : ApiBase
    {
        /// <summary>
        /// 新增分组
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object addGroup(Dictionary<string, object> dicParas)
        {
            try
            {
                string MobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;//获取手机令牌
                string RouterToken = dicParas.ContainsKey("routerToken") ? dicParas["routerToken"].ToString() : string.Empty;//获取控制器令牌
                if (RouterToken == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "控制器令牌不能为空");
                }


                string GroupName = dicParas.ContainsKey("groupName") ? dicParas["groupName"].ToString() : string.Empty;//获取分组名称
                if (GroupName == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "分组名称不能为空");
                }
                string GroupType = dicParas.ContainsKey("groupType") ? dicParas["groupType"].ToString() : string.Empty;//获取分组类别               
                string PushReduceFromCard = dicParas.ContainsKey("pushReduceFromCard") ? dicParas["pushReduceFromCard"].ToString() : string.Empty;//获取投币时扣卡里币数

                string PushLevel = dicParas.ContainsKey("pushLevel") ? dicParas["pushLevel"].ToString() : string.Empty;//获取投币电平

                string LotteryMode = dicParas.ContainsKey("lotteryMode") ? dicParas["lotteryMode"].ToString() : string.Empty;//获取电子彩票模式

                string OnlyExitLottery = dicParas.ContainsKey("onlyExitLottery") ? dicParas["onlyExitLottery"].ToString() : string.Empty;//获取只退实物彩票

                string ReadCat = dicParas.ContainsKey("readCat") ? dicParas["readCat"].ToString() : string.Empty;//获取刷卡即扣模式

                string ChkCheckGift = dicParas.ContainsKey("chkCheckGift") ? dicParas["chkCheckGift"].ToString() : string.Empty;//获取礼品掉落检测

                string ReturnCheck = dicParas.ContainsKey("returnCheck") ? dicParas["returnCheck"].ToString() : "1";//获取启用遥控器偷分检测

                string OutsideAlertCheck = dicParas.ContainsKey("outsideAlertCheck") ? dicParas["outsideAlertCheck"].ToString() : string.Empty;//获取启用游戏机外部报警检测

                string IcticketOperation = dicParas.ContainsKey("icticketOperation") ? dicParas["icticketOperation"].ToString() : string.Empty;//获取二合一专版模式

                string NotGiveBack = dicParas.ContainsKey("notGiveBack") ? dicParas["notGiveBack"].ToString() : string.Empty;//获取不参与返还

                string AllowElecPush = dicParas.ContainsKey("allowElecPush") ? dicParas["allowElecPush"].ToString() : string.Empty;//获取允许电子投币

                string AllowDecuplePush = dicParas.ContainsKey("allowDecuplePush") ? dicParas["allowDecuplePush"].ToString() : string.Empty;//获取允许十倍投币

                string GuardConvertCard = dicParas.ContainsKey("guardConvertCard") ? dicParas["guardConvertCard"].ToString() : string.Empty;//获取防止转卡(启用专卡专用)

                string AllowRealPush = dicParas.ContainsKey("allowRealPush") ? dicParas["allowRealPush"].ToString() : string.Empty;//获取允许实物投币

                string BanOccupy = dicParas.ContainsKey("banOccupy") ? dicParas["banOccupy"].ToString() : string.Empty;//获取防止霸位检测

                string StrongGuardConvertCard = dicParas.ContainsKey("strongGuardConvertCard") ? dicParas["strongGuardConvertCard"].ToString() : string.Empty;//获取增强防止转卡

                string AllowElecOut = dicParas.ContainsKey("allowElecOut") ? dicParas["allowElecOut"].ToString() : string.Empty;//获取允许电子退币(允许打票)

                string NowExit = dicParas.ContainsKey("nowExit") ? dicParas["nowExit"].ToString() : string.Empty;//获取即中即退模式

                string BOLock = dicParas.ContainsKey("bOLock") ? dicParas["bOLock"].ToString() : string.Empty;//获取退币锁定模式

                string AllowRealOut = dicParas.ContainsKey("allowRealOut") ? dicParas["allowRealOut"].ToString() : string.Empty;//获取允许实物出币

                string BOKeep = dicParas.ContainsKey("bOKeep") ? dicParas["bOKeep"].ToString() : string.Empty;//获取退币按钮保持

                string PushAddToGame = dicParas.ContainsKey("pushAddToGame") ? dicParas["pushAddToGame"].ToString() : string.Empty;//获取投币时给游戏机脉冲数

                string PushSpeed = dicParas.ContainsKey("pushSpeed") ? dicParas["pushSpeed"].ToString() : string.Empty;//获取投币速度

                string PushPulse = dicParas.ContainsKey("pushPulse") ? dicParas["pushPulse"].ToString() : string.Empty;//获取投币脉宽

                string PushStartInterval = dicParas.ContainsKey("pushStartInterval") ? dicParas["pushStartInterval"].ToString() : string.Empty;//获取首次投币脉冲间隔

                string UseSecondPush = dicParas.ContainsKey("useSecondPush") ? dicParas["useSecondPush"].ToString() : string.Empty;//获取允许第二路上分信号

                string SecondReduceFromCard = dicParas.ContainsKey("secondReduceFromCard") ? dicParas["secondReduceFromCard"].ToString() : string.Empty;//获取接上分线时扣卡里币数

                string SecondAddToGame = dicParas.ContainsKey("secondAddToGame") ? dicParas["secondAddToGame"].ToString() : string.Empty;//获取接上分线时给游戏机脉冲数

                string SecondSpeed = dicParas.ContainsKey("secondSpeed") ? dicParas["secondSpeed"].ToString() : string.Empty;//获取上分速度

                string SecondPulse = dicParas.ContainsKey("secondPulse") ? dicParas["secondPulse"].ToString() : string.Empty;//获取上分脉宽

                string SecondLevel = dicParas.ContainsKey("secondLevel") ? dicParas["secondLevel"].ToString() : string.Empty;//获取上分电平

                string SecondStartInterval = dicParas.ContainsKey("secondStartInterval") ? dicParas["secondStartInterval"].ToString() : string.Empty;//获取首次上分脉冲间隔

                string OutSpeed = dicParas.ContainsKey("outSpeed") ? dicParas["outSpeed"].ToString() : string.Empty;//获取退币速度

                string OutPulse = dicParas.ContainsKey("outPulse") ? dicParas["outPulse"].ToString() : string.Empty;//获取币脉宽

                string CountLevel = dicParas.ContainsKey("countLevel") ? dicParas["countLevel"].ToString() : string.Empty;//获取数币电平

                string OutLevel = dicParas.ContainsKey("outLevel") ? dicParas["outLevel"].ToString() : string.Empty;//获取退币电平(SSR)

                string OutReduceFromGame = dicParas.ContainsKey("outReduceFromGame") ? dicParas["outReduceFromGame"].ToString() : string.Empty;//获取退币时游戏机出币数

                string OutAddToCard = dicParas.ContainsKey("outAddToCard") ? dicParas["outAddToCard"].ToString() : string.Empty;//获取退币时卡上加币数

                string OnceOutLimit = dicParas.ContainsKey("onceOutLimit") ? dicParas["onceOutLimit"].ToString() : string.Empty;//获取每次退币限额

                string OncePureOutLimit = dicParas.ContainsKey("oncePureOutLimit") ? dicParas["oncePureOutLimit"].ToString() : string.Empty;//获取每次净退币限额

                string SsrtimeOut = dicParas.ContainsKey("ssrtimeOut") ? dicParas["ssrtimeOut"].ToString() : string.Empty;//获取SSR检测延时

                string ExceptOutTest = dicParas.ContainsKey("exceptOutTest") ? dicParas["exceptOutTest"].ToString() : string.Empty;//获取异常退币检测

                string ExceptOutSpeed = dicParas.ContainsKey("exceptOutSpeed") ? dicParas["exceptOutSpeed"].ToString() : string.Empty;//获取异常SSR检测速度

                string Frequency = dicParas.ContainsKey("frequency") ? dicParas["frequency"].ToString() : string.Empty;//获取异常退币检测次数

                if (MobileToken == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机令牌不能为空");
                }
                string mobile = string.Empty;
                if (!MobileTokenBusiness.ExistToken(MobileToken, out mobile))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机token无效");
                }
                IDeviceService deviceService = BLLContainer.Resolve<IDeviceService>("XCCloudRS232");
                var devicelist = deviceService.GetModels(x => x.Token == RouterToken && x.Status == 1).FirstOrDefault<Base_DeviceInfo>();
                if (deviceService == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "路由控制器无效");
                }
                IDataGameInfoService dataGameInfoService = BLLContainer.Resolve<IDataGameInfoService>("XCCloudRS232");
                int deviceID = 1;
                var dataGameInfolist = dataGameInfoService.GetModels(x => x.DeviceID == deviceID).ToList();
                if (dataGameInfolist.Count < 99)
                {
                    Data_GameInfo gameInfo = new Data_GameInfo();
                    gameInfo.DeviceID = deviceID;
                    gameInfo.GroupName = GroupName;
                    gameInfo.GroupType = Convert.ToInt32(GroupType);
                    gameInfo.PushReduceFromCard = Convert.ToInt32(PushReduceFromCard);
                    gameInfo.PushLevel = Convert.ToInt32(PushLevel);
                    gameInfo.LotteryMode = Convert.ToInt32(LotteryMode);
                    gameInfo.OnlyExitLottery = Convert.ToInt32(OnlyExitLottery);
                    gameInfo.ReadCat = Convert.ToInt32(ReadCat);
                    gameInfo.chkCheckGift = Convert.ToInt32(ChkCheckGift);
                    gameInfo.ReturnCheck = int.Parse(ReturnCheck);
                    gameInfo.OutsideAlertCheck = Convert.ToInt32(OutsideAlertCheck);
                    gameInfo.ICTicketOperation = Convert.ToInt32(IcticketOperation);
                    gameInfo.NotGiveBack = Convert.ToInt32(NotGiveBack);
                    gameInfo.AllowElecPush = Convert.ToInt32(AllowElecPush);
                    gameInfo.AllowDecuplePush = Convert.ToInt32(AllowDecuplePush);
                    gameInfo.GuardConvertCard = Convert.ToInt32(GuardConvertCard);
                    gameInfo.AllowRealPush = Convert.ToInt32(AllowRealPush);
                    gameInfo.BanOccupy = Convert.ToInt32(BanOccupy);
                    gameInfo.StrongGuardConvertCard = Convert.ToInt32(StrongGuardConvertCard);
                    gameInfo.AllowElecOut = Convert.ToInt32(AllowElecOut);
                    gameInfo.NowExit = Convert.ToInt32(NowExit);
                    gameInfo.BOLock = Convert.ToInt32(BOLock);
                    gameInfo.AllowRealOut = Convert.ToInt32(AllowRealOut);
                    gameInfo.BOKeep = Convert.ToInt32(BOKeep);
                    gameInfo.PushAddToGame = Convert.ToInt32(PushAddToGame);
                    gameInfo.PushSpeed = Convert.ToInt32(PushSpeed);
                    gameInfo.PushPulse = Convert.ToInt32(PushPulse);
                    gameInfo.PushStartInterval = Convert.ToInt32(PushStartInterval);
                    gameInfo.UseSecondPush = Convert.ToInt32(UseSecondPush);
                    gameInfo.SecondReduceFromCard = Convert.ToInt32(SecondReduceFromCard);
                    gameInfo.SecondAddToGame = Convert.ToInt32(SecondAddToGame);
                    gameInfo.SecondSpeed = Convert.ToInt32(SecondSpeed);
                    gameInfo.SecondPulse = Convert.ToInt32(SecondPulse);
                    gameInfo.SecondLevel = Convert.ToInt32(SecondLevel);
                    gameInfo.SecondStartInterval = Convert.ToInt32(SecondStartInterval);
                    gameInfo.OutSpeed = Convert.ToInt32(OutSpeed);
                    gameInfo.OutPulse = Convert.ToInt32(OutPulse);
                    gameInfo.CountLevel = Convert.ToInt32(CountLevel);
                    gameInfo.OutLevel = Convert.ToInt32(OutLevel);
                    gameInfo.OutReduceFromGame = Convert.ToInt32(OutReduceFromGame);
                    gameInfo.OutAddToCard = Convert.ToInt32(OutAddToCard);
                    gameInfo.OnceOutLimit = Convert.ToInt32(OnceOutLimit);
                    gameInfo.OncePureOutLimit = Convert.ToInt32(OncePureOutLimit);
                    gameInfo.SSRTimeOut = Convert.ToInt32(SsrtimeOut);
                    gameInfo.ExceptOutTest = Convert.ToInt32(ExceptOutTest);
                    gameInfo.ExceptOutSpeed = Convert.ToInt32(ExceptOutSpeed);
                    gameInfo.Frequency = Convert.ToInt32(Frequency);
                    dataGameInfoService.Add(gameInfo);
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
                }
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "该设备已经存在过多数据");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 修改分组
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object editGroup(Dictionary<string, object> dicParas)
        {
            try
            {
                string MobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;//获取手机令牌
                string GroupID = dicParas.ContainsKey("groupID") ? dicParas["groupID"].ToString() : string.Empty;//获取分组ID
                if (GroupID == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "分组ID不能为空");
                }
                string Mode = dicParas.ContainsKey("mode") ? dicParas["mode"].ToString() : string.Empty;//获取参数设置模式
                if (Mode == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "参数设置模式不能为空");
                }
                string GroupName = dicParas.ContainsKey("groupName") ? dicParas["groupName"].ToString() : string.Empty;//获取分组名称
                if (GroupName == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "分组名称不能为空");
                }
                string GroupType = dicParas.ContainsKey("groupType") ? dicParas["groupType"].ToString() : string.Empty;//获取分组类别

                string PushReduceFromCard = dicParas.ContainsKey("pushReduceFromCard") ? dicParas["pushReduceFromCard"].ToString() : string.Empty;//获取投币时扣卡里币数

                string PushLevel = dicParas.ContainsKey("pushLevel") ? dicParas["pushLevel"].ToString() : string.Empty;//获取投币电平

                string LotteryMode = dicParas.ContainsKey("lotteryMode") ? dicParas["lotteryMode"].ToString() : string.Empty;//获取电子彩票模式

                string OnlyExitLottery = dicParas.ContainsKey("onlyExitLottery") ? dicParas["onlyExitLottery"].ToString() : string.Empty;//获取只退实物彩票

                string ReadCat = dicParas.ContainsKey("readCat") ? dicParas["readCat"].ToString() : string.Empty;//获取刷卡即扣模式

                string ChkCheckGift = dicParas.ContainsKey("chkCheckGift") ? dicParas["chkCheckGift"].ToString() : string.Empty;//获取礼品掉落检测

                string ReturnCheck = dicParas.ContainsKey("returnCheck") ? dicParas["returnCheck"].ToString() : "1";//获取启用遥控器偷分检测

                string OutsideAlertCheck = dicParas.ContainsKey("outsideAlertCheck") ? dicParas["outsideAlertCheck"].ToString() : string.Empty;//获取启用游戏机外部报警检测

                string IcticketOperation = dicParas.ContainsKey("icticketOperation") ? dicParas["icticketOperation"].ToString() : string.Empty;//获取二合一专版模式

                string NotGiveBack = dicParas.ContainsKey("notGiveBack") ? dicParas["notGiveBack"].ToString() : string.Empty;//获取不参与返还

                string AllowElecPush = dicParas.ContainsKey("allowElecPush") ? dicParas["allowElecPush"].ToString() : string.Empty;//获取允许电子投币

                string AllowDecuplePush = dicParas.ContainsKey("allowDecuplePush") ? dicParas["allowDecuplePush"].ToString() : string.Empty;//获取允许十倍投币

                string GuardConvertCard = dicParas.ContainsKey("guardConvertCard") ? dicParas["guardConvertCard"].ToString() : string.Empty;//获取防止转卡(启用专卡专用)

                string AllowRealPush = dicParas.ContainsKey("allowRealPush") ? dicParas["allowRealPush"].ToString() : string.Empty;//获取允许实物投币

                string BanOccupy = dicParas.ContainsKey("banOccupy") ? dicParas["banOccupy"].ToString() : string.Empty;//获取防止霸位检测

                string StrongGuardConvertCard = dicParas.ContainsKey("strongGuardConvertCard") ? dicParas["strongGuardConvertCard"].ToString() : string.Empty;//获取增强防止转卡

                string AllowElecOut = dicParas.ContainsKey("allowElecOut") ? dicParas["allowElecOut"].ToString() : string.Empty;//获取允许电子退币(允许打票)

                string NowExit = dicParas.ContainsKey("nowExit") ? dicParas["nowExit"].ToString() : string.Empty;//获取即中即退模式

                string BOLock = dicParas.ContainsKey("bOLock") ? dicParas["bOLock"].ToString() : string.Empty;//获取退币锁定模式

                string AllowRealOut = dicParas.ContainsKey("allowRealOut") ? dicParas["allowRealOut"].ToString() : string.Empty;//获取允许实物出币

                string BOKeep = dicParas.ContainsKey("bOKeep") ? dicParas["bOKeep"].ToString() : string.Empty;//获取退币按钮保持

                string PushAddToGame = dicParas.ContainsKey("pushAddToGame") ? dicParas["pushAddToGame"].ToString() : string.Empty;//获取投币时给游戏机脉冲数

                string PushSpeed = dicParas.ContainsKey("pushSpeed") ? dicParas["pushSpeed"].ToString() : string.Empty;//获取投币速度

                string PushPulse = dicParas.ContainsKey("pushPulse") ? dicParas["pushPulse"].ToString() : string.Empty;//获取投币脉宽

                string PushStartInterval = dicParas.ContainsKey("pushStartInterval") ? dicParas["pushStartInterval"].ToString() : string.Empty;//获取首次投币脉冲间隔

                string UseSecondPush = dicParas.ContainsKey("useSecondPush") ? dicParas["useSecondPush"].ToString() : string.Empty;//获取允许第二路上分信号

                string SecondReduceFromCard = dicParas.ContainsKey("secondReduceFromCard") ? dicParas["secondReduceFromCard"].ToString() : string.Empty;//获取接上分线时扣卡里币数

                string SecondAddToGame = dicParas.ContainsKey("secondAddToGame") ? dicParas["secondAddToGame"].ToString() : string.Empty;//获取接上分线时给游戏机脉冲数

                string SecondSpeed = dicParas.ContainsKey("secondSpeed") ? dicParas["secondSpeed"].ToString() : string.Empty;//获取上分速度

                string SecondPulse = dicParas.ContainsKey("secondPulse") ? dicParas["secondPulse"].ToString() : string.Empty;//获取上分脉宽

                string SecondLevel = dicParas.ContainsKey("secondLevel") ? dicParas["secondLevel"].ToString() : string.Empty;//获取上分电平

                string SecondStartInterval = dicParas.ContainsKey("secondStartInterval") ? dicParas["secondStartInterval"].ToString() : string.Empty;//获取首次上分脉冲间隔

                string OutSpeed = dicParas.ContainsKey("outSpeed") ? dicParas["outSpeed"].ToString() : string.Empty;//获取退币速度

                string OutPulse = dicParas.ContainsKey("outPulse") ? dicParas["outPulse"].ToString() : string.Empty;//获取币脉宽

                string CountLevel = dicParas.ContainsKey("countLevel") ? dicParas["countLevel"].ToString() : string.Empty;//获取数币电平

                string OutLevel = dicParas.ContainsKey("outLevel") ? dicParas["outLevel"].ToString() : string.Empty;//获取退币电平(SSR)

                string OutReduceFromGame = dicParas.ContainsKey("outReduceFromGame") ? dicParas["outReduceFromGame"].ToString() : string.Empty;//获取退币时游戏机出币数

                string OutAddToCard = dicParas.ContainsKey("outAddToCard") ? dicParas["outAddToCard"].ToString() : string.Empty;//获取退币时卡上加币数

                string OnceOutLimit = dicParas.ContainsKey("onceOutLimit") ? dicParas["onceOutLimit"].ToString() : string.Empty;//获取每次退币限额

                string OncePureOutLimit = dicParas.ContainsKey("oncePureOutLimit") ? dicParas["oncePureOutLimit"].ToString() : string.Empty;//获取每次净退币限额

                string SsrtimeOut = dicParas.ContainsKey("ssrtimeOut") ? dicParas["ssrtimeOut"].ToString() : string.Empty;//获取SSR检测延时

                string ExceptOutTest = dicParas.ContainsKey("exceptOutTest") ? dicParas["exceptOutTest"].ToString() : string.Empty;//获取异常退币检测

                string ExceptOutSpeed = dicParas.ContainsKey("exceptOutSpeed") ? dicParas["exceptOutSpeed"].ToString() : string.Empty;//获取异常SSR检测速度

                string Frequency = dicParas.ContainsKey("frequency") ? dicParas["frequency"].ToString() : string.Empty;//获取异常退币检测次数
                if (Frequency == "")
                {
                    Frequency = "0";
                }
                if (MobileToken == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机令牌不能为空");
                }
                string mobile = string.Empty;
                if (!MobileTokenBusiness.ExistToken(MobileToken, out mobile))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机token无效");
                }
                IDataGameInfoService dataGameInfoService = BLLContainer.Resolve<IDataGameInfoService>("XCCloudRS232");
                int deviceID = 1;
                int groupids = int.Parse(GroupID);
                var gameInfo = dataGameInfoService.GetModels(x => x.GroupID == groupids).FirstOrDefault<Data_GameInfo>();
                if (gameInfo == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "未查询到该数据");
                }
                if (Mode == "1")
                {
                    gameInfo.DeviceID = deviceID;
                    gameInfo.GroupName = GroupName;
                    gameInfo.GroupType = Convert.ToInt32(GroupType);
                    gameInfo.PushReduceFromCard = Convert.ToInt32(PushReduceFromCard);
                    gameInfo.PushLevel = Convert.ToInt32(PushLevel);
                    gameInfo.LotteryMode = Convert.ToInt32(LotteryMode);
                    gameInfo.OnlyExitLottery = Convert.ToInt32(OnlyExitLottery);
                    gameInfo.ReadCat = Convert.ToInt32(ReadCat);
                    gameInfo.chkCheckGift = Convert.ToInt32(ChkCheckGift);
                    dataGameInfoService.Update(gameInfo);
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
                }
                else if (Mode == "2")
                {
                    gameInfo.DeviceID = deviceID;
                    gameInfo.GroupName = GroupName;
                    gameInfo.GroupType = Convert.ToInt32(GroupType);
                    gameInfo.PushReduceFromCard = Convert.ToInt32(PushReduceFromCard);
                    gameInfo.PushLevel = Convert.ToInt32(PushLevel);
                    gameInfo.LotteryMode = Convert.ToInt32(LotteryMode);
                    gameInfo.OnlyExitLottery = Convert.ToInt32(OnlyExitLottery);
                    gameInfo.ReadCat = Convert.ToInt32(ReadCat);
                    gameInfo.chkCheckGift = Convert.ToInt32(ChkCheckGift);
                    gameInfo.ReturnCheck = int.Parse(ReturnCheck);
                    gameInfo.OutsideAlertCheck = Convert.ToInt32(OutsideAlertCheck);
                    gameInfo.ICTicketOperation = Convert.ToInt32(IcticketOperation);
                    gameInfo.NotGiveBack = Convert.ToInt32(NotGiveBack);
                    gameInfo.AllowElecPush = Convert.ToInt32(AllowElecPush);
                    gameInfo.AllowDecuplePush = Convert.ToInt32(AllowDecuplePush);
                    gameInfo.GuardConvertCard = Convert.ToInt32(GuardConvertCard);
                    gameInfo.AllowRealPush = Convert.ToInt32(AllowRealPush);
                    gameInfo.BanOccupy = Convert.ToInt32(BanOccupy);
                    gameInfo.StrongGuardConvertCard = Convert.ToInt32(StrongGuardConvertCard);
                    gameInfo.AllowElecOut = Convert.ToInt32(AllowElecOut);
                    gameInfo.NowExit = Convert.ToInt32(NowExit);
                    gameInfo.BOLock = Convert.ToInt32(BOLock);
                    gameInfo.AllowRealOut = Convert.ToInt32(AllowRealOut);
                    gameInfo.BOKeep = Convert.ToInt32(BOKeep);
                    gameInfo.PushAddToGame = Convert.ToInt32(PushAddToGame);
                    gameInfo.PushSpeed = Convert.ToInt32(PushSpeed);
                    gameInfo.PushPulse = Convert.ToInt32(PushPulse);
                    gameInfo.PushStartInterval = Convert.ToInt32(PushStartInterval);
                    gameInfo.UseSecondPush = Convert.ToInt32(UseSecondPush);
                    gameInfo.SecondReduceFromCard = Convert.ToInt32(SecondReduceFromCard);
                    gameInfo.SecondAddToGame = Convert.ToInt32(SecondAddToGame);
                    gameInfo.SecondSpeed = Convert.ToInt32(SecondSpeed);
                    gameInfo.SecondPulse = Convert.ToInt32(SecondPulse);
                    gameInfo.SecondLevel = Convert.ToInt32(SecondLevel);
                    gameInfo.SecondStartInterval = Convert.ToInt32(SecondStartInterval);
                    gameInfo.OutSpeed = Convert.ToInt32(OutSpeed);
                    gameInfo.OutPulse = Convert.ToInt32(OutPulse);
                    gameInfo.CountLevel = Convert.ToInt32(CountLevel);
                    gameInfo.OutLevel = Convert.ToInt32(OutLevel);
                    gameInfo.OutReduceFromGame = Convert.ToInt32(OutReduceFromGame);
                    gameInfo.OutAddToCard = Convert.ToInt32(OutAddToCard);
                    gameInfo.OnceOutLimit = Convert.ToInt32(OnceOutLimit);
                    gameInfo.OncePureOutLimit = Convert.ToInt32(OncePureOutLimit);
                    gameInfo.SSRTimeOut = Convert.ToInt32(SsrtimeOut);
                    gameInfo.ExceptOutTest = Convert.ToInt32(ExceptOutTest);
                    gameInfo.ExceptOutSpeed = Convert.ToInt32(ExceptOutSpeed);
                    gameInfo.Frequency = Convert.ToInt32(Frequency);
                    dataGameInfoService.Update(gameInfo);
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
                }
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "参数模式有误");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 删除分组
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object deleteGroup(Dictionary<string, object> dicParas)
        {
            try
            {
                string MobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;//获取手机令牌
                string GroupID = dicParas.ContainsKey("groupID") ? dicParas["groupID"].ToString() : string.Empty;//获取分组ID
                if (GroupID == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "分组ID不能为空");
                }
                if (MobileToken == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机令牌不能为空");
                }
                string mobile = string.Empty;
                if (!MobileTokenBusiness.ExistToken(MobileToken, out mobile))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机token无效");
                }
                string sql = "exec deleteDataGameInfo @GroupID,@Return output";
                SqlParameter[] parameters = new SqlParameter[2];
                parameters[0] = new SqlParameter("@GroupID", GroupID);
                parameters[1] = new SqlParameter("@Return", 0);
                parameters[1].Direction = System.Data.ParameterDirection.Output;
                XCCloudRS232BLL.ExecuteQuerySentence(sql, parameters);
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 获取分组信息
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getGroup(Dictionary<string, object> dicParas)
        {
            try
            {
                string MobileToken = dicParas.ContainsKey("mobileToken") ? dicParas["mobileToken"].ToString() : string.Empty;//获取手机令牌
                string GroupID = dicParas.ContainsKey("groupID") ? dicParas["groupID"].ToString() : string.Empty;//获取分组ID
                if (GroupID == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "分组ID不能为空");
                }
                if (MobileToken == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机令牌不能为空");
                }
                string mobile = string.Empty;
                if (!MobileTokenBusiness.ExistToken(MobileToken, out mobile))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机token无效");
                }
                int GroupIDs = int.Parse(GroupID);
                IDataGameInfoService dataGameInfoService = BLLContainer.Resolve<IDataGameInfoService>("XCCloudRS232");
                var menlist = dataGameInfoService.GetModels(x => x.GroupID == GroupIDs).ToList<Data_GameInfo>();
                if (menlist.Count > 0)
                {

                    List<DataGameInfoModel> gameinfolist = Utils.GetCopyList<DataGameInfoModel, Data_GameInfo>(menlist);
                    return ResponseModelFactory<List<DataGameInfoModel>>.CreateModel(isSignKeyReturn, gameinfolist);
                }
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "无数据");              

            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}


