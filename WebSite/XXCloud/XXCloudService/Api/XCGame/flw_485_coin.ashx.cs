using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using XCCloudService.CacheService;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.Business.Common;
using XCCloudService.Business.XCGameMana;
using XCCloudService.Common;
using XCCloudService.Model.XCGameManager;
using XCCloudService.Model.Socket.UDP;
using XCCloudService.SocketService.UDP;
using XCCloudService.SocketService.UDP.Factory;
using XCCloudService.SocketService.UDP.Security;
using XCCloudService.Common.Enum;
using System.Transactions;
using XCCloudService.Model.CustomModel.XCGame;
using XXCloudService.Utility;
using XCCloudService.Model.WeiXin.SAppMessage;
using XCCloudService.WeiXin.Message;

namespace XCCloudService.Api.XCGame
{
    /// <summary>
    /// flw_485_coin 的摘要说明
    /// </summary>
    public class flw_485_coin : ApiBase
    {
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameMemberOrMobileToken)]
        public object flw_485_coinSale(Dictionary<string, object> dicParas)
        {
            try
            {
                string mobile = string.Empty;
                string storeId = string.Empty;
                string deviceId = string.Empty;
                string errMsg = string.Empty;
                string segment = string.Empty;
                string mcuId = string.Empty;
                string xcGameDBName = string.Empty;
                int memberLevelId = 0;
                string storeName = string.Empty;
                int balance = 0;
                int icCardId = 0;
                int deviceIdentityId = 0;
                int lastBalance = 0;
                string storePassword = string.Empty; 
                string orderId = dicParas.ContainsKey("orderId") ? dicParas["orderId"].ToString() : string.Empty;
                string deviceToken = dicParas.ContainsKey("deviceToken") ? dicParas["deviceToken"].ToString() : string.Empty;
                int coins = Convert.ToInt32(dicParas["coins"]);
                XCGameMemberTokenModel memberTokenModel = (XCGameMemberTokenModel)(dicParas[Constant.XCGameMemberTokenModel]);
                mobile = memberTokenModel.Mobile;

                if (string.IsNullOrEmpty(orderId))
                {
                    orderId = System.Guid.NewGuid().ToString("N");
                }

                //根据终端号查询终端号是否存在
                XCGameManaDeviceStoreType deviceStoreType;
                if (!ExtendBusiness.checkXCGameManaDeviceInfo(deviceToken, out deviceStoreType, out storeId, out deviceId))
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
                //验证余额
                if (coins > balance)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "币数余额不足");
                }
                //数据库提币操作
                if (!getCoins(deviceStoreType, xcGameDBName, mobile, balance, coins, icCardId, segment, deviceId, deviceIdentityId,out lastBalance,out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }
                //请求雷达处理出币
                if (!IConUtiltiy.DeviceOutputCoin(deviceStoreType, DevieControlTypeEnum.出币, storeId, mobile, icCardId, orderId, segment, mcuId, storePassword, 0, coins, string.Empty,out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                //设置推送消息的缓存结构
                string form_id = dicParas.ContainsKey("form_id") ? dicParas["form_id"].ToString() : string.Empty;
                MemberCoinsOperationNotifyDataModel dataModel = new MemberCoinsOperationNotifyDataModel("提币", storeName, mobile, icCardId, coins, lastBalance);
                SAppMessageMana.SetMemberCoinsMsgCacheData(SAppMessageType.MemberCoinsOperationNotify,orderId, form_id, mobile,  dataModel, out errMsg);

                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        private bool getCoins(XCGameManaDeviceStoreType deviceStoreType,string xcGameDBName,string mobile,int balance, int coins, int icCardId, string segment, string deviceId,int deviceIdentityId,out int lastBalance,out string errMsg)
        {
            errMsg = string.Empty;
            lastBalance = 0;
            //提币数据库操作
            if(deviceStoreType == XCGameManaDeviceStoreType.Store)
            {
                XCCloudService.BLL.IBLL.XCGame.IDeviceService deviceService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IDeviceService>(xcGameDBName);
                var deviceModel = deviceService.GetModels(p => p.MCUID.Equals(deviceId)).FirstOrDefault<XCCloudService.Model.XCGame.t_device>();
                if (deviceModel == null)
                {
                    errMsg = "设备不存在";
                    return false;
                }
                XCCloudService.BLL.IBLL.XCGame.Iflw_485_coinService coinService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.Iflw_485_coinService>(xcGameDBName);
                XCCloudService.BLL.IBLL.XCGame.IMemberService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IMemberService>(xcGameDBName);
                var memberModel = memberService.GetModels(p => p.Mobile.Equals(mobile, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XCCloudService.Model.XCGame.t_member>();
                memberModel.Balance = Convert.ToInt32(memberModel.Balance) - coins;
                lastBalance = (int)(memberModel.Balance);
                XCCloudService.Model.XCGame.flw_485_coin coinModel = new Model.XCGame.flw_485_coin();
                coinModel.Balance = Convert.ToInt32(memberModel.Balance) - coins;
                coinModel.ICCardID = icCardId;
                coinModel.Segment = segment;
                coinModel.HeadAddress = deviceModel.address;
                coinModel.Coins = coins;
                coinModel.CoinType = "3";
                coinModel.RealTime = System.DateTime.Now;
                coinModel.SN = 0;

                using (var transactionScope = new System.Transactions.TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    coinService.Add(coinModel);
                    memberService.Update(memberModel);
                    transactionScope.Complete();
                }
                return true;
            }
            else if (deviceStoreType == XCGameManaDeviceStoreType.Merch)
            {
                XCCloudService.BLL.IBLL.XCCloudRS232.Iflw_485_coinService coinService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCCloudRS232.Iflw_485_coinService>(xcGameDBName);
                XCCloudService.BLL.IBLL.XCCloudRS232.IMemberService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCCloudRS232.IMemberService>(xcGameDBName);
                var memberModel = memberService.GetModels(p => p.Mobile.Equals(mobile, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XCCloudService.Model.XCCloudRS232.t_member>();
                memberModel.Balance = Convert.ToInt32(memberModel.Balance) - coins;
                lastBalance = (int)(memberModel.Balance);
                XCCloudService.Model.XCCloudRS232.flw_485_coin coinModel = new XCCloudService.Model.XCCloudRS232.flw_485_coin();
                coinModel.Balance = Convert.ToInt32(memberModel.Balance) - coins;
                coinModel.ICCardID = icCardId;
                coinModel.DeviceID = deviceIdentityId;
                coinModel.Coins = coins;
                coinModel.CoinType = 3;
                using (var transactionScope = new System.Transactions.TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    coinService.Add(coinModel);
                    memberService.Update(memberModel);
                    transactionScope.Complete();
                }
                return true;
            }
            else
            {
                return false;
            }
        }


        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameMemberOrMobileToken)]
        public object flw_485_coinPut(Dictionary<string, object> dicParas)
        {
            try
            {
                string mobile = string.Empty;
                string storeId = string.Empty;
                string deviceId = string.Empty;
                string errMsg = string.Empty;
                string segment = string.Empty;
                string mcuId = string.Empty;
                string headId = string.Empty;
                string gameId = string.Empty;
                string xcGameDBName = string.Empty;
                int memberLevelId = 0;
                string storeName = string.Empty;
                int balance = 0;
                int icCardId = 0;
                string storePassword = string.Empty;
                string orderId = dicParas.ContainsKey("orderId") ? dicParas["orderId"].ToString() : string.Empty;
                string deviceToken = dicParas.ContainsKey("deviceToken") ? dicParas["deviceToken"].ToString() : string.Empty;
                int coins = Convert.ToInt32(dicParas["coins"]);
                string gameRuleId = dicParas.ContainsKey("gameRuleId") ? dicParas["gameRuleId"].ToString() : string.Empty;
                XCGameMemberTokenModel memberTokenModel = (XCGameMemberTokenModel)(dicParas[Constant.XCGameMemberTokenModel]);
                mobile = memberTokenModel.Mobile;

                if (string.IsNullOrEmpty(orderId))
                {
                    orderId = System.Guid.NewGuid().ToString("N");
                }

                //根据终端号查询终端号是否存在
                XCGameManaDeviceStoreType deviceStoreType;
                if (!ExtendBusiness.checkXCGameManaDeviceInfo(deviceToken, out deviceStoreType, out storeId, out deviceId))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "终端号不存在");
                }
                //验证会员令牌的门店号和设备门店号
                if (!memberTokenModel.StoreId.Equals(storeId))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "会员令牌不能再此设备上操作");
                }
                //验证门店信息和设备状态是否为启用状态
                if (!ExtendBusiness.checkStoreGameDeviceInfo(deviceStoreType,storeId,deviceId,out segment,out mcuId,out headId,out gameId,out xcGameDBName,out storePassword, out storeName, out errMsg))
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
                //验证余额
                if (coins > balance)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "币数余额不足");
                }
                //请求雷达处理出币
                if (!IConUtiltiy.DeviceOutputCoin(deviceStoreType, DevieControlTypeEnum.投币, storeId, mobile, icCardId, orderId, segment, mcuId, storePassword, 0, coins, gameRuleId, out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameMemberOrMobileToken)]
        public object flw_485_coinExit(Dictionary<string, object> dicParas)
        {
            try
            {
                string mobile = string.Empty;
                string storeId = string.Empty;
                string deviceId = string.Empty;
                string errMsg = string.Empty;
                string segment = string.Empty;
                string mcuId = string.Empty;
                string headId = string.Empty;
                string gameId = string.Empty;
                string xcGameDBName = string.Empty;
                int memberLevelId = 0;
                string storeName = string.Empty;
                int balance = 0;
                int icCardId = 0;
                string storePassword = string.Empty;
                string orderId = dicParas.ContainsKey("orderId") ? dicParas["orderId"].ToString() : string.Empty;
                string deviceToken = dicParas.ContainsKey("deviceToken") ? dicParas["deviceToken"].ToString() : string.Empty;
                int coins = 0;
                XCGameMemberTokenModel memberTokenModel = (XCGameMemberTokenModel)(dicParas[Constant.XCGameMemberTokenModel]);
                mobile = memberTokenModel.Mobile;

                if (string.IsNullOrEmpty(orderId))
                {
                    orderId = System.Guid.NewGuid().ToString("N");
                }

                //根据终端号查询终端号是否存在
                XCGameManaDeviceStoreType deviceStoreType;
                if (!ExtendBusiness.checkXCGameManaDeviceInfo(deviceToken, out deviceStoreType, out storeId, out deviceId))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "终端号不存在");
                }
                //验证会员令牌的门店号和设备门店号
                if (!memberTokenModel.StoreId.Equals(storeId))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "会员令牌不能再此设备上操作");
                }
                //验证门店信息和设备状态是否为启用状态
                if (!ExtendBusiness.checkStoreGameDeviceInfo(deviceStoreType, storeId, deviceId, out segment, out mcuId, out headId, out gameId, out xcGameDBName, out storePassword, out storeName, out errMsg))
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
                //请求雷达处理出币
                if (!IConUtiltiy.DeviceOutputCoin(deviceStoreType, DevieControlTypeEnum.退币, storeId, mobile, icCardId, orderId, segment, mcuId, storePassword, 0, coins, string.Empty, out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private bool putCoins(XCGameManaDeviceStoreType deviceStoreType, string xcGameDBName, string mobile, int balance, int coins, int icCardId, string segment, string deviceId, int deviceIdentityId, out int lastBalance, out string errMsg)
        {
            errMsg = string.Empty;
            lastBalance = 0;
            //提币数据库操作
            if (deviceStoreType == XCGameManaDeviceStoreType.Store)
            {
                XCCloudService.BLL.IBLL.XCGame.Iflw_485_coinService coinService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.Iflw_485_coinService>(xcGameDBName);
                XCCloudService.BLL.IBLL.XCGame.IMemberService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IMemberService>(xcGameDBName);
                var memberModel = memberService.GetModels(p => p.Mobile.Equals(mobile, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XCCloudService.Model.XCGame.t_member>();
                memberModel.Balance = Convert.ToInt32(memberModel.Balance) - coins;
                lastBalance = (int)(memberModel.Balance);
                XCCloudService.Model.XCGame.flw_485_coin coinModel = new Model.XCGame.flw_485_coin();
                coinModel.Balance = Convert.ToInt32(memberModel.Balance) - coins;
                coinModel.ICCardID = icCardId;
                coinModel.Segment = segment;
                coinModel.HeadAddress = "";
                coinModel.Coins = coins;
                coinModel.CoinType = "3";
                coinModel.RealTime = System.DateTime.Now;
                coinModel.SN = 0;

                using (var transactionScope = new System.Transactions.TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    coinService.Add(coinModel);
                    memberService.Update(memberModel);
                    transactionScope.Complete();
                }
                return true;
            }
            else if (deviceStoreType == XCGameManaDeviceStoreType.Merch)
            {
                XCCloudService.BLL.IBLL.XCCloudRS232.Iflw_485_coinService coinService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCCloudRS232.Iflw_485_coinService>(xcGameDBName);
                XCCloudService.BLL.IBLL.XCCloudRS232.IMemberService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCCloudRS232.IMemberService>(xcGameDBName);
                var memberModel = memberService.GetModels(p => p.Mobile.Equals(mobile, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XCCloudService.Model.XCCloudRS232.t_member>();
                memberModel.Balance = Convert.ToInt32(memberModel.Balance) - coins;
                lastBalance = (int)(memberModel.Balance);
                XCCloudService.Model.XCCloudRS232.flw_485_coin coinModel = new XCCloudService.Model.XCCloudRS232.flw_485_coin();
                coinModel.Balance = Convert.ToInt32(memberModel.Balance) - coins;
                coinModel.ICCardID = icCardId;
                coinModel.DeviceID = deviceIdentityId;
                coinModel.Coins = coins;
                coinModel.CoinType = 3;
                using (var transactionScope = new System.Transactions.TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    coinService.Add(coinModel);
                    memberService.Update(memberModel);
                    transactionScope.Complete();
                }
                return true;
            }
            else
            {
                return false;
            }
        }


        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object coinPut()
        {
            string orderId = System.DateTime.Now.ToString("yyyyMMddHHmmss") ;
            string storeId = "100027";
            string mobile = "15618920033";
            int icCardId = 10000002;
            string action = "6";
            int coins = 5;
            string segment = "0001";
            string mcuId = "20170518000023";
            string sn = UDPSocketAnswerBusiness.GetSN();
            string errMsg = string.Empty;

            DeviceControlRequestDataModel deviceControlModel = 
                new DeviceControlRequestDataModel(storeId, mobile, icCardId.ToString(), segment, mcuId, action, coins, sn, orderId, "778852013146", 0,"");
            MPOrderBusiness.AddTCPAnswerOrder(orderId, mobile, coins, action, "", storeId);
            IconOutLockBusiness.Add(mobile, coins);
            if (!DataFactory.SendDataToRadar(deviceControlModel, out errMsg))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //[ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameMemberOrMobileToken)]
        //public object flw_485_coinSave(Dictionary<string, object> dicParas)
        //{
        //    try
        //    {
        //        string mobile = string.Empty;
        //        string storeId = string.Empty;
        //        string deviceId = string.Empty;
        //        string errMsg = string.Empty;
        //        string segment = string.Empty;
        //        string mcuId = string.Empty;
        //        string xcGameDBName = string.Empty;
        //        string storeName = string.Empty;
        //        int memberLevelId = 0;
        //        int lastBalance = 0;
        //        int balance = 0;
        //        int icCardId = 0;
        //        int deviceIdentityId = 0;
        //        string storePassword = string.Empty;
        //        string orderId = dicParas.ContainsKey("orderId") ? dicParas["orderId"].ToString() : string.Empty;
        //        string deviceToken = dicParas.ContainsKey("deviceToken") ? dicParas["deviceToken"].ToString() : string.Empty;
        //        int coins = Convert.ToInt32(dicParas["coins"]);
        //        XCGameMemberTokenModel memberTokenModel = (XCGameMemberTokenModel)(dicParas[Constant.XCGameMemberTokenModel]);
        //        mobile = memberTokenModel.Mobile;

        //        //根据终端号查询终端号是否存在
        //        XCGameManaDeviceStoreType deviceStoreType;
        //        if (!ExtendBusiness.checkXCGameManaDeviceInfo(deviceToken, out deviceStoreType, out storeId, out deviceId))
        //        {
        //            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "终端号不存在");
        //        }
        //        //验证会员令牌的门店号和设备门店号
        //        if (!memberTokenModel.StoreId.Equals(storeId))
        //        {
        //            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "会员令牌不能再此设备上操作");
        //        }
        //        //验证门店信息和设备状态是否为启用状态
        //        if (!ExtendBusiness.checkStoreDeviceInfo(deviceStoreType, storeId, deviceId, out segment, out mcuId, out xcGameDBName, out deviceIdentityId, out storePassword,out storeName, out errMsg))
        //        {
        //            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
        //        }
        //        //验证雷达设备缓存状态
        //        if (!ExtendBusiness.checkRadarDeviceState(deviceStoreType, storeId, deviceId, out errMsg))
        //        {
        //            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
        //        }
        //        //获取会员信息
        //        if (!ExtendBusiness.GetMemberInfo(deviceStoreType, mobile, xcGameDBName, out balance, out icCardId, out memberLevelId, out errMsg))
        //        {
        //            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
        //        }
        //        //验证余额
        //        if (coins > balance)
        //        {
        //            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "币数余额不足");
        //        }
        //        //数据库存币操作
        //        if (!getCoins(deviceStoreType, xcGameDBName, mobile, balance, coins, icCardId, segment, deviceId, deviceIdentityId,out lastBalance,out errMsg))
        //        {
        //            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
        //        }
        //        //请求雷达处理出币
        //        if (!IConUtiltiy.DeviceOutputCoin(deviceStoreType, storeId, mobile, icCardId, orderId, segment, mcuId, storePassword, 0, coins, out errMsg))
        //        {
        //            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}




        //private bool saveCoins(XCGameManaDeviceStoreType deviceStoreType, string xcGameDBName, string mobile, int balance, int coins, int icCardId, string segment, string deviceId, int deviceIdentityId, out string errMsg)
        //{
        //    errMsg = string.Empty;
        //    //提币数据库操作
        //    if (deviceStoreType == XCGameManaDeviceStoreType.Store)
        //    {
        //        XCCloudService.BLL.IBLL.XCGame.IDeviceService deviceService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IDeviceService>(xcGameDBName);
        //        var deviceModel = deviceService.GetModels(p => p.MCUID.Equals(deviceId)).FirstOrDefault<XCCloudService.Model.XCGame.t_device>();
        //        if (deviceModel == null)
        //        {
        //            errMsg = "设备不存在";
        //            return false;
        //        }
        //        XCCloudService.BLL.IBLL.XCGame.Iflw_485_savecoinService coinService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.Iflw_485_savecoinService>(xcGameDBName);
        //        XCCloudService.BLL.IBLL.XCGame.IMemberService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IMemberService>(xcGameDBName);
        //        var memberModel = memberService.GetModels(p => p.Mobile.Equals(mobile, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XCCloudService.Model.XCGame.t_member>();
        //        memberModel.Balance = Convert.ToInt32(memberModel.Balance) + coins;

        //        XCCloudService.Model.XCGame.flw_485_savecoin coinModel = new Model.XCGame.flw_485_savecoin();
        //        coinModel.Balance = Convert.ToInt32(memberModel.Balance) + coins;
        //        coinModel.ICCardID = icCardId;
        //        coinModel.Segment = segment;
        //        coinModel.HeadAddress = deviceModel.address;
        //        coinModel.Coins = coins;
        //        coinModel.RealTime = System.DateTime.Now;

        //        using (var transactionScope = new System.Transactions.TransactionScope(TransactionScopeOption.RequiresNew))
        //        {
        //            coinService.Add(coinModel);
        //            memberService.Update(memberModel);
        //            transactionScope.Complete();
        //        }
        //        return true;
        //    }
        //    else if (deviceStoreType == XCGameManaDeviceStoreType.Merch)
        //    {
        //        XCCloudService.BLL.IBLL.XCCloudRS232.Iflw_485_savecoinService coinService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCCloudRS232.Iflw_485_savecoinService>();
        //        XCCloudService.BLL.IBLL.XCCloudRS232.IMemberService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCCloudRS232.IMemberService>();
        //        var memberModel = memberService.GetModels(p => p.Mobile.Equals(mobile, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<XCCloudService.Model.XCCloudRS232.t_member>();
        //        memberModel.Balance = Convert.ToInt32(memberModel.Balance) + coins;

        //        XCCloudService.Model.XCCloudRS232.flw_485_savecoin coinModel = new XCCloudService.Model.XCCloudRS232.flw_485_savecoin();
        //        coinModel.Balance = Convert.ToInt32(memberModel.Balance) + coins;
        //        coinModel.ICCardID = icCardId;
        //        coinModel.DeviceID = deviceIdentityId;
        //        coinModel.Coins = coins;
        //        coinModel.RealTime = System.DateTime.Now;

        //        using (var transactionScope = new System.Transactions.TransactionScope(TransactionScopeOption.RequiresNew))
        //        {
        //            coinService.Add(coinModel);
        //            memberService.Update(memberModel);
        //            transactionScope.Complete();
        //        }
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

    }
}