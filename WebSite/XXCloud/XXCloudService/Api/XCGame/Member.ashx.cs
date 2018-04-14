using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using XCCloudService.CacheService;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.Business.Common;
using XCCloudService.Business.XCGame;
using XCCloudService.Business.XCGameMana;
using XCCloudService.Common;
using XCCloudService.Model;
using XCCloudService.Model.CustomModel.XCGame;
using XCCloudService.ResponseModels;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.Model.XCGameManager;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.Common.Enum;
using XCCloudService.Model.CustomModel.XCGameManager;
using System.Transactions;
using XCCloudService.Model.XCGame;
using XCCloudService.SocketService.UDP.Factory;
using XCCloudService.Model.CustomModel.Common;
using XCCloudService.SocketService.UDP;
using XCCloudService.Model.Socket.UDP;

namespace XCCloudService.Api.XCGame
{
    /// <summary>
    /// Member 的摘要说明
    /// </summary>
    public class Member : ApiBase
    {
        #region"获取会员信息"

        /// <summary>
        /// 获取会员信息
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameMemberToken)]
        public object getInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                StoreCacheModel storeModel = null;
                System.Data.DataSet ds = null;
                //获取token模式
                XCGameMemberTokenModel memberTokenModel = (XCGameMemberTokenModel)(dicParas[Constant.XCGameMemberTokenModel]);

                //验证门店
                StoreBusiness storeBusiness = new StoreBusiness();
                if (!storeBusiness.IsEffectiveStore(memberTokenModel.StoreId, ref storeModel, out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }
                //如果是门店模式
                if (storeModel.StoreType == 0)
                {
                    XCCloudService.BLL.IBLL.XCGame.IMemberService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IMemberService>(storeModel.StoreDBName);
                    var count = memberService.GetModels(p => p.Mobile.Equals(memberTokenModel.Mobile, StringComparison.OrdinalIgnoreCase)).Count<XCCloudService.Model.XCGame.t_member>();
                    if (count == 0)
                    {
                        string sql = " exec RegisterMember @Mobile,@MemberPassword,@WXOpenID,@Return output ";
                        SqlParameter[] parameters = new SqlParameter[4];
                        parameters[0] = new SqlParameter("@Mobile", memberTokenModel.Mobile);
                        parameters[1] = new SqlParameter("@MemberPassword", "888888");
                        parameters[2] = new SqlParameter("@WXOpenID", "");
                        parameters[3] = new SqlParameter("@Return", 0);
                        parameters[3].Direction = System.Data.ParameterDirection.Output;
                        ds = XCGameBLL.ExecuteQuerySentence(sql, storeModel.StoreDBName, parameters);
                        MemberResponseModel memberResponseModel = Utils.GetModelList<MemberResponseModel>(ds.Tables[0])[0];
                        memberResponseModel.MemberState = MemberBusiness.GetMemberStateName(memberResponseModel.MemberState);
                        string meberToken = MemberTokenBusiness.SetMemberToken(memberTokenModel.StoreId, memberTokenModel.Mobile, memberResponseModel.MemberLevelName, storeModel.StoreName, memberResponseModel.ICCardID.ToString(), memberResponseModel.EndDate);
                        RegisterMemberTokenResponseModel registerMemberTokenResponseModel = new RegisterMemberTokenResponseModel(memberTokenModel.StoreId, storeModel.StoreName, meberToken, memberResponseModel);
                        return ResponseModelFactory<RegisterMemberTokenResponseModel>.CreateModel(isSignKeyReturn, registerMemberTokenResponseModel);
                    }
                    else
                    {
                        string sql = " exec GetMember @Mobile,@ICCardID";
                        SqlParameter[] parameters = new SqlParameter[2];
                        parameters[0] = new SqlParameter("@Mobile", memberTokenModel.Mobile);
                        parameters[1] = new SqlParameter("@ICCardID", "0");
                        ds = XCGameBLL.ExecuteQuerySentence(sql, storeModel.StoreDBName, parameters);
                        RegisterMemberResponseModel registerMemberResponseModel = Utils.GetModelList<RegisterMemberResponseModel>(ds.Tables[0])[0];
                        registerMemberResponseModel.StoreId = storeModel.StoreID.ToString();
                        registerMemberResponseModel.StoreName = storeModel.StoreName;
                        registerMemberResponseModel.MemberState = MemberBusiness.GetMemberStateName(registerMemberResponseModel.MemberState);
                        return ResponseModelFactory<RegisterMemberResponseModel>.CreateModel(isSignKeyReturn, registerMemberResponseModel);
                    }
                }
                //如果是商户模式
                else if (storeModel.StoreType == 1)
                {
                    XCCloudService.BLL.IBLL.XCCloudRS232.IMemberService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCCloudRS232.IMemberService>();
                    var count = memberService.GetModels(p => p.Mobile.Equals(memberTokenModel.Mobile, StringComparison.OrdinalIgnoreCase)).Count<XCCloudService.Model.XCCloudRS232.t_member>();
                    if (count == 0)
                    {
                        string sql = " exec RegisterMember @Mobile,@MerchId,@MemberPassword,@WXOpenID,@Return output ";
                        SqlParameter[] parameters = new SqlParameter[5];
                        parameters[0] = new SqlParameter("@Mobile", memberTokenModel.Mobile);
                        parameters[1] = new SqlParameter("@MerchId", storeModel.StoreID);
                        parameters[2] = new SqlParameter("@MemberPassword", "888888");
                        parameters[3] = new SqlParameter("@WXOpenID", "");
                        parameters[4] = new SqlParameter("@Return", 0);
                        parameters[4].Direction = System.Data.ParameterDirection.Output;
                        ds = XCCloudRS232BLL.ExecuteQuerySentence(sql, parameters);
                        MemberResponseModel memberResponseModel = Utils.GetModelList<MemberResponseModel>(ds.Tables[0])[0];
                        memberResponseModel.MemberState = MemberBusiness.GetMemberStateName(memberResponseModel.MemberState);
                        string meberToken = MemberTokenBusiness.SetMemberToken(memberTokenModel.StoreId, memberTokenModel.Mobile, memberResponseModel.MemberLevelName, storeModel.StoreName, memberResponseModel.ICCardID.ToString(), memberResponseModel.EndDate);
                        RegisterMemberTokenResponseModel registerMemberTokenResponseModel = new RegisterMemberTokenResponseModel(memberTokenModel.StoreId, storeModel.StoreName, meberToken, memberResponseModel);
                        return ResponseModelFactory<RegisterMemberTokenResponseModel>.CreateModel(isSignKeyReturn, registerMemberTokenResponseModel);
                    }
                    else
                    {
                        string sql = " exec GetMember @Mobile,@MerchId,@ICCardID";
                        SqlParameter[] parameters = new SqlParameter[3];
                        parameters[0] = new SqlParameter("@Mobile", memberTokenModel.Mobile);
                        parameters[1] = new SqlParameter("@MerchId", memberTokenModel.StoreId);
                        parameters[2] = new SqlParameter("@ICCardID", "0");
                        ds = XCCloudRS232BLL.ExecuteQuerySentence(sql, parameters);
                        RegisterMemberResponseModel registerMemberResponseModel = Utils.GetModelList<RegisterMemberResponseModel>(ds.Tables[0])[0];
                        registerMemberResponseModel.StoreId = storeModel.StoreID.ToString();
                        registerMemberResponseModel.StoreName = storeModel.StoreName;
                        registerMemberResponseModel.MemberState = MemberBusiness.GetMemberStateName(registerMemberResponseModel.MemberState);
                        return ResponseModelFactory<RegisterMemberResponseModel>.CreateModel(isSignKeyReturn, registerMemberResponseModel);
                    }
                }
                else
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店类型无效");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 检测会员信息查询参数
        /// </summary>
        private bool checkMemberParams(Dictionary<string, object> dicParas, out string mobile, out string errMsg)
        {
            bool isCheck = true;
            errMsg = string.Empty;
            mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;

            if (isCheck && string.IsNullOrEmpty(mobile))
            {
                errMsg = "mobile参数不能为空";
                isCheck = false;
            }

            return isCheck;
        }

        #endregion

        #region "注册会员"

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MobileToken)]
        public object register(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string mobile = string.Empty;
                XCGameManaDeviceStoreType deviceStoreType;
                string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
                //验证手机token
                MobileTokenModel mobileTokenModel = (MobileTokenModel)(dicParas[Constant.MobileTokenModel]);
                mobile = mobileTokenModel.Mobile;
                //验证门店
                StoreCacheModel storeModel = null;
                StoreBusiness store = new StoreBusiness();
                if (!store.IsEffectiveStore(storeId,out deviceStoreType,ref storeModel, out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                string meberToken = string.Empty;
                if (deviceStoreType == XCGameManaDeviceStoreType.Store)
                {
                    XCCloudService.BLL.IBLL.XCGame.IMemberService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IMemberService>(storeModel.StoreDBName);
                    //获取手机号是否存在注册记录
                    var count = memberService.GetModels(p => p.Mobile.Equals(mobile, StringComparison.OrdinalIgnoreCase)).Count<XCCloudService.Model.XCGame.t_member>();
                    if (count == 0)
                    {
                        string sql = " exec RegisterMember @Mobile,@MemberPassword,@WXOpenID,@Return output ";
                        SqlParameter[] parameters = new SqlParameter[4];
                        parameters[0] = new SqlParameter("@Mobile", mobile);
                        parameters[1] = new SqlParameter("@MemberPassword", "888888");
                        parameters[2] = new SqlParameter("@WXOpenID", "");
                        parameters[3] = new SqlParameter("@Return", 0);
                        parameters[3].Direction = System.Data.ParameterDirection.Output;
                        System.Data.DataSet ds = XCGameBLL.ExecuteQuerySentence(sql, storeModel.StoreDBName, parameters);
                        MemberResponseModel memberResponseModel = Utils.GetModelList<MemberResponseModel>(ds.Tables[0])[0];
                        memberResponseModel.MemberState = MemberBusiness.GetMemberStateName(memberResponseModel.MemberState);
                        meberToken = MemberTokenBusiness.SetMemberToken(storeId, mobile, memberResponseModel.MemberLevelName, storeModel.StoreName, memberResponseModel.ICCardID.ToString(), memberResponseModel.EndDate);
                        RegisterMemberTokenResponseModel registerMemberTokenResponseModel = new RegisterMemberTokenResponseModel(storeId, storeModel.StoreName, meberToken, memberResponseModel);
                        return ResponseModelFactory<RegisterMemberTokenResponseModel>.CreateModel(isSignKeyReturn, registerMemberTokenResponseModel);
                    }
                    else
                    {
                        //如果已注册，返回会员主要信息
                        string sql = " exec GetMember @Mobile,@ICCardID";
                        SqlParameter[] parameters = new SqlParameter[2];
                        parameters[0] = new SqlParameter("@Mobile", mobile);
                        parameters[1] = new SqlParameter("@ICCardID", "0");
                        System.Data.DataSet ds = XCGameBLL.ExecuteQuerySentence(sql, storeModel.StoreDBName, parameters);
                        MemberResponseModel memberResponseModel = Utils.GetModelList<MemberResponseModel>(ds.Tables[0])[0];
                        memberResponseModel.MemberState = MemberBusiness.GetMemberStateName(memberResponseModel.MemberState);
                        meberToken = MemberTokenBusiness.SetMemberToken(storeId, mobile, memberResponseModel.MemberLevelName, storeModel.StoreName, memberResponseModel.ICCardID.ToString(), memberResponseModel.EndDate);
                        RegisterMemberTokenResponseModel registerMemberTokenResponseModel = new RegisterMemberTokenResponseModel(storeId, storeModel.StoreName, meberToken, memberResponseModel);
                        return ResponseModelFactory<RegisterMemberTokenResponseModel>.CreateModel(isSignKeyReturn, registerMemberTokenResponseModel);
                    }
                }
                else if (deviceStoreType == XCGameManaDeviceStoreType.Merch)
                {
                    XCCloudService.BLL.IBLL.XCCloudRS232.IMemberService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCCloudRS232.IMemberService>();
                    //获取手机号是否存在注册记录
                    var count = memberService.GetModels(p => p.Mobile.Equals(mobile, StringComparison.OrdinalIgnoreCase)).Count<XCCloudService.Model.XCCloudRS232.t_member>();
                    if (count == 0)
                    {
                        string sql = " exec RegisterMember @Mobile,@MerchId,@MemberPassword,@WXOpenID,@Return output ";
                        SqlParameter[] parameters = new SqlParameter[5];
                        parameters[0] = new SqlParameter("@Mobile", mobile);
                        parameters[1] = new SqlParameter("@MerchId", storeId);
                        parameters[2] = new SqlParameter("@MemberPassword", "888888");
                        parameters[3] = new SqlParameter("@WXOpenID", "");
                        parameters[4] = new SqlParameter("@Return", 0);
                        parameters[4].Direction = System.Data.ParameterDirection.Output;
                        System.Data.DataSet ds = XCCloudRS232BLL.ExecuteQuerySentence(sql, parameters);
                        MemberResponseModel memberResponseModel = Utils.GetModelList<MemberResponseModel>(ds.Tables[0])[0];
                        memberResponseModel.MemberState = MemberBusiness.GetMemberStateName(memberResponseModel.MemberState);
                        meberToken = MemberTokenBusiness.SetMemberToken(storeId, mobile, memberResponseModel.MemberLevelName, storeModel.StoreName, memberResponseModel.ICCardID.ToString(), memberResponseModel.EndDate);
                        RegisterMemberTokenResponseModel registerMemberTokenResponseModel = new RegisterMemberTokenResponseModel(storeId, storeModel.StoreName, meberToken, memberResponseModel);
                        return ResponseModelFactory<RegisterMemberTokenResponseModel>.CreateModel(isSignKeyReturn, registerMemberTokenResponseModel);
                    }
                    else
                    {
                        //如果已注册，返回会员主要信息
                        string sql = " exec GetMember @Mobile,@MerchId,@ICCardID";
                        SqlParameter[] parameters = new SqlParameter[3];
                        parameters[0] = new SqlParameter("@Mobile", mobile);
                        parameters[1] = new SqlParameter("@MerchId", storeId);
                        parameters[2] = new SqlParameter("@ICCardID", "0");
                        System.Data.DataSet ds = XCCloudRS232BLL.ExecuteQuerySentence(sql, parameters);
                        MemberResponseModel memberResponseModel = Utils.GetModelList<MemberResponseModel>(ds.Tables[0])[0];
                        memberResponseModel.MemberState = "使用中";
                        meberToken = MemberTokenBusiness.SetMemberToken(storeId, mobile, memberResponseModel.MemberLevelName, storeModel.StoreName, memberResponseModel.ICCardID.ToString(), memberResponseModel.EndDate);
                        RegisterMemberTokenResponseModel registerMemberTokenResponseModel = new RegisterMemberTokenResponseModel(storeId, storeModel.StoreName, meberToken, memberResponseModel);
                        return ResponseModelFactory<RegisterMemberTokenResponseModel>.CreateModel(isSignKeyReturn, registerMemberTokenResponseModel);
                    }
                }
                else
                {
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, "店号无效");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region "修改会员资料"

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object update(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string mobile = string.Empty;
                StoreBusiness storeBusiness = new StoreBusiness();
                StoreCacheModel storeModel = null;
                string moblietoken = dicParas.ContainsKey("moblietoken") ? dicParas["moblietoken"].ToString() : string.Empty;
                string membername = dicParas.ContainsKey("membername") ? dicParas["membername"].ToString() : string.Empty;
                if (membername == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "会员姓名不能为空");
                }
                string gender = dicParas.ContainsKey("gender") ? dicParas["gender"].ToString() : string.Empty;
                if (gender == "")
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "会员性别不能为空");
                }

                string MemberPhoto = dicParas.ContainsKey("memberphoto") ? dicParas["memberphoto"].ToString() : string.Empty;
                if (!MobileTokenBusiness.ExistToken(moblietoken, out mobile))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "手机token无效");
                }

                IMemberTokenService memberTokenService = BLLContainer.Resolve<IMemberTokenService>();
                var membertokenlist = memberTokenService.GetModels(x => x.Phone == mobile).ToList();
                if (membertokenlist.Count == 0)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "未查询到会员信息");
                }

                //遍历会员表中的所有数据，更新会员信息
                for (int i = 0; i < membertokenlist.Count; i++)
                {
                    int iccard = Convert.ToInt32(membertokenlist[i].ICCardID);
                    int storeid = Convert.ToInt32(membertokenlist[i].StoreId);
                    if (storeBusiness.IsEffectiveStore(storeid.ToString(), ref storeModel, out errMsg))
                    {
                        if (storeModel.StoreType == 0)
                        {
                            XCCloudService.BLL.IBLL.XCGame.IMemberService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCGame.IMemberService>(storeModel.StoreDBName);
                            var model = memberService.GetModels(p => p.ICCardID == iccard).FirstOrDefault<XCCloudService.Model.XCGame.t_member>();
                            if (model != null)
                            {
                                model.MemberName = membername;
                                model.Gender = gender;
                                model.MemberPhoto = MemberPhoto;
                                memberService.Update(model);
                            }
                        }
                        else if (storeModel.StoreType == 1)
                        {
                            XCCloudService.BLL.IBLL.XCCloudRS232.IMemberService memberService = BLLContainer.Resolve<XCCloudService.BLL.IBLL.XCCloudRS232.IMemberService>();
                            var model = memberService.GetModels(p => p.ICCardID == iccard).FirstOrDefault<XCCloudService.Model.XCCloudRS232.t_member>();
                            if (model != null)
                            {
                                model.MemberName = membername;
                                memberService.Update(model);
                            }
                        }
                    }
                }
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private bool checkUpdateParams(Dictionary<string, object> dicParas, out string errMsg)
        {
            errMsg = string.Empty;
            string membername = dicParas.ContainsKey("membername") ? dicParas["membername"].ToString() : string.Empty;
            string gender = dicParas.ContainsKey("gender") ? dicParas["gender"].ToString() : string.Empty;
            string birthday = dicParas.ContainsKey("birthday") ? dicParas["birthday"].ToString() : string.Empty;
            string certificalid = dicParas.ContainsKey("certificalid") ? dicParas["certificalid"].ToString() : string.Empty;

            if (membername.Length > 10)
            {
                errMsg = "卡号长度不能超过10个字符";
                return false;
            }

            if (!gender.Equals("0") && !gender.Equals("1"))
            {
                errMsg = "性别参数不正确";
                return false;
            }

            if (!Utils.CheckBirthday(birthday))
            {
                errMsg = "出生日期参数不正确";
                return false;
            }

            return true;
        }

        #endregion

        #region "通过会员卡号获取流水信息"

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameMemberToken)]
        public object getMemberCardBill(Dictionary<string, object> dicParas)
        {
            try
            {
                string MemberToken = dicParas.ContainsKey("memberToken") ? dicParas["memberToken"].ToString() : string.Empty;
                string PageIndex = dicParas.ContainsKey("pageIndex") ? dicParas["pageIndex"].ToString() : string.Empty;
                if (PageIndex == "")
                {
                    PageIndex = "1";
                }
                XCGameMemberTokenModel memberTokenKeyModel = (XCGameMemberTokenModel)(dicParas[Constant.XCGameMemberTokenModel]);
                //IMemberTokenService memberTokenservice = BLLContainer.Resolve<IMemberTokenService>();
                //var memberlist = memberTokenservice.GetModels(p => p.Token == MemberToken).FirstOrDefault<T_MemberToken>();
                //if (memberlist == null)
                //{
                //    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "会员令牌不存在");
                //}
                string ICCardID = memberTokenKeyModel.ICCardId;
                string name = "";
                if (!MemberICICard.IsExist(ICCardID))
                {
                    MemberCardQueryBusiness.SetMemberCardQueryBusiness(ICCardID, out name);
                    if (name == "")
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "未查询到该会员的流水");
                    }
                }
                List<MenberICCardModel> list = MemberICICard.GetValue(ICCardID) as List<MenberICCardModel>;

                string PageSize = CommonConfig.DataOrderPageSize;
                int PageCout = 0;

                List<MenberICCardModel> list1 = null;

                if (Utils.GetPageList<MenberICCardModel>(list, Convert.ToInt32(PageIndex), Convert.ToInt32(PageSize), out PageCout, ref list1))
                {
                    MenberICListCardModel member = new MenberICListCardModel();
                    member.Lists = list1;
                    member.Page = PageCout.ToString();
                    return ResponseModelFactory<MenberICListCardModel>.CreateModel(isSignKeyReturn, member);
                }
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "未查询数据");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region"通过会员店号和卡号获取会员信息"

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getmemberInformation(Dictionary<string, object> dicParas)
        {
            string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
            string Iccard = dicParas.ContainsKey("iccard") ? dicParas["iccard"].ToString() : string.Empty;
            StoreBusiness store = new StoreBusiness();
            string xcGameDBName = string.Empty;
            string password = string.Empty;
            string errMsg = string.Empty;
            if (!store.IsEffectiveStore(storeId, out xcGameDBName, out password, out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
            }
            string sql = " exec GetMemberInformation @ICCardID";
            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter("@ICCardID", Iccard);
            System.Data.DataSet ds = XCGameBLL.ExecuteQuerySentence(sql, xcGameDBName, parameters);
            RegisterMemberResponseModel registerMemberResponseModel = Utils.GetModelList<RegisterMemberResponseModel>(ds.Tables[0])[0];           
            return ResponseModelFactory<RegisterMemberResponseModel>.CreateModel(isSignKeyReturn, registerMemberResponseModel);
        }
        #endregion

        #region "会员转账"

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameManaUserToken)]
        public object transICCardCoins(Dictionary<string, object> dicParas)
        {
            XCCloudManaUserTokenModel userTokenModel = (XCCloudManaUserTokenModel)(dicParas[Constant.XCGameManaUserToken]);

            string cardIdOut = dicParas.ContainsKey("cardIdOut") ? dicParas["cardIdOut"].ToString() : string.Empty;
            string cardIdOutPassword = dicParas.ContainsKey("cardIdOutPassword") ? dicParas["cardIdOutPassword"].ToString() : string.Empty;
            string cardIdIn = dicParas.ContainsKey("cardIdIn") ? dicParas["cardIdIn"].ToString() : string.Empty;
            string coins = dicParas.ContainsKey("coins") ? dicParas["coins"].ToString() : string.Empty;
            string mobileName = dicParas.ContainsKey("mobileName") ? dicParas["mobileName"].ToString() : string.Empty;

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
                string storeProc = "TransICCardCoins";
                SqlParameter[] parameters = new SqlParameter[8];
                parameters[0] = new SqlParameter("@CardIdOut", cardIdOut);
                parameters[1] = new SqlParameter("@CardIdOutPassword", cardIdOutPassword);
                parameters[2] = new SqlParameter("@CardIdIn", cardIdIn);
                parameters[3] = new SqlParameter("@Coins", coins);
                parameters[4] = new SqlParameter("@UserId", userTokenModel.XCGameUserId);
                parameters[5] = new SqlParameter("@Mobile", userTokenModel.Mobile);
                parameters[6] = new SqlParameter("@ErrMsg", System.Data.SqlDbType.VarChar,200);
                parameters[6].Direction = System.Data.ParameterDirection.Output;
                parameters[7] = new SqlParameter("@Return", 0);
                parameters[7].Direction = System.Data.ParameterDirection.ReturnValue;

                XCGameBLL.ExecuteStoredProcedureSentence(storeProc, storeModel.StoreDBName, parameters);
                if (Convert.ToInt32(parameters[7].Value) == 1)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
                }
                else
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, parameters[6].Value.ToString());
                }                
            }
            else if (storeModel.StoreDBDeployType == 1)
            {
                string sn = System.Guid.NewGuid().ToString().Replace("-", "");
                UDPSocketCommonQueryAnswerModel answerModel = null;
                string radarToken = string.Empty;
                if (DataFactory.SendMemberTransOperate(sn, storeModel.StoreID.ToString(),storeModel.StorePassword, mobileName, userTokenModel.Mobile, cardIdIn, cardIdOut, int.Parse(coins),out radarToken,out errMsg))
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
                    MemberTransOperateResultNotifyRequestModel model = (MemberTransOperateResultNotifyRequestModel)(answerModel.Result);
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
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店设置错误");
            }
        }

        #endregion

        /// <summary>
        /// 获取会员信息
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameManaUserToken)]
        public object getInfo2(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                StoreCacheModel storeModel = null;
                System.Data.DataSet ds = null;

                //获取token模式
                XCCloudManaUserTokenModel userTokenModel = (XCCloudManaUserTokenModel)(dicParas[Constant.XCGameManaUserToken]);
                string icCardId = dicParas.ContainsKey("icCardId") ? dicParas["icCardId"].ToString() : string.Empty;

                //验证门店
                StoreBusiness storeBusiness = new StoreBusiness();
                if (!storeBusiness.IsEffectiveStore(userTokenModel.StoreId, ref storeModel, out errMsg))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
                }

                if (storeModel.StoreDBDeployType == 0)
                { 
                    //如果是门店模式
                    string sql = " exec GetMember @Mobile,@ICCardID";
                    SqlParameter[] parameters = new SqlParameter[2];
                    parameters[0] = new SqlParameter("@Mobile", "");
                    parameters[1] = new SqlParameter("@ICCardID", icCardId);
                    ds = XCGameBLL.ExecuteQuerySentence(sql, storeModel.StoreDBName, parameters);
                    if (ds != null && ds.Tables.Count >= 1)
                    {
                        RegisterMemberResponseModel registerMemberResponseModel = Utils.GetModelList<RegisterMemberResponseModel>(ds.Tables[0])[0];
                        registerMemberResponseModel.StoreId = storeModel.StoreID.ToString();
                        registerMemberResponseModel.StoreName = storeModel.StoreName;
                        registerMemberResponseModel.MemberState = MemberBusiness.GetMemberStateName(registerMemberResponseModel.MemberState);
                        return ResponseModelFactory<RegisterMemberResponseModel>.CreateModel(isSignKeyReturn, registerMemberResponseModel);
                    }
                    else
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "");
                    }                
                }
                else if (storeModel.StoreDBDeployType == 1)
                {
                    string sn = System.Guid.NewGuid().ToString().Replace("-", "");
                    UDPSocketCommonQueryAnswerModel answerModel = null;
                    string radarToken = string.Empty;
                    if (DataFactory.SendDataMemberQuery(sn, storeModel.StoreID.ToString(), storeModel.StorePassword, icCardId, out radarToken, out errMsg))
                    {
                        answerModel = new UDPSocketCommonQueryAnswerModel(sn, storeModel.StoreID.ToString(), storeModel.StorePassword, 0, null, radarToken);
                        UDPSocketCommonQueryAnswerBusiness.AddAnswer(sn, answerModel);
                    }
                    else
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "");
                    }

                    answerModel = null;
                    while (answerModel == null)
                    {
                        //获取应答缓存数据
                        System.Threading.Thread.Sleep(1000);
                        answerModel = UDPSocketCommonQueryAnswerBusiness.GetAnswerModel(sn, 1);
                    }

                    if (answerModel != null)
                    {
                        MemberQueryResultNotifyRequestModel model = (MemberQueryResultNotifyRequestModel)(answerModel.Result);
                        //移除应答缓存数据
                        UDPSocketCommonQueryAnswerBusiness.Remove(sn);

                        if (model.Result_Code == "1")
                        {
                            RegisterMemberResponseModel registerMemberResponseModel = new RegisterMemberResponseModel();
                            registerMemberResponseModel.ICCardID = int.Parse(model.Result_Data.ICCardID);
                            registerMemberResponseModel.MemberName = model.Result_Data.MemberName;
                            registerMemberResponseModel.Gender = model.Result_Data.Gender;
                            registerMemberResponseModel.Birthday = model.Result_Data.Birthday;
                            registerMemberResponseModel.CertificalID = model.Result_Data.CertificalID;
                            registerMemberResponseModel.Mobile = model.Result_Data.Mobile;
                            registerMemberResponseModel.Balance = Convert.ToInt32(model.Result_Data.Balance);
                            registerMemberResponseModel.Point = Convert.ToInt32(model.Result_Data.Point);
                            registerMemberResponseModel.Deposit = model.Result_Data.Deposit;
                            registerMemberResponseModel.MemberState = model.Result_Data.MemberState;
                            registerMemberResponseModel.Lottery = Convert.ToInt32(model.Result_Data.Lottery);
                            registerMemberResponseModel.Note = model.Result_Data.Note;
                            registerMemberResponseModel.EndDate = model.Result_Data.EndDate;
                            registerMemberResponseModel.MemberLevelName = model.Result_Data.MemberLevelName;
                            registerMemberResponseModel.StoreId = storeModel.StoreID.ToString();
                            registerMemberResponseModel.StoreName = storeModel.StoreName;
                            registerMemberResponseModel.MemberState = MemberBusiness.GetMemberStateName(registerMemberResponseModel.MemberState);
                            return ResponseModelFactory<RegisterMemberResponseModel>.CreateModel(isSignKeyReturn, registerMemberResponseModel);
                        }
                        else
                        {
                            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "查询数据出错");
                        }
                    }
                    else
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "查询数据出错");
                    }
                }
                else
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店设置错误");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameManaUserToken)]
        public object getCustomCardInfo(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            StoreCacheModel storeModel = null;
            System.Data.DataSet ds = null;

            //获取token模式
            XCCloudManaUserTokenModel userTokenModel = (XCCloudManaUserTokenModel)(dicParas[Constant.XCGameManaUserToken]);
            //验证门店
            StoreBusiness storeBusiness = new StoreBusiness();
            if (!storeBusiness.IsEffectiveStore(userTokenModel.StoreId, ref storeModel, out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
            }

            if (storeModel.StoreDBDeployType == 0)
            { 
                string sql = @" select ICCardID,MemberName,Gender,convert(char(10),Birthday,120) as Birthday,CertificalID,Mobile,Balance,Point,a.Deposit,MemberState,Lottery,Note,convert(char(10),EndDate,120) as EndDate,MemberLevelName from t_member a left join t_memberlevel b on a.MemberLevelID = b.MemberLevelID 
                                where MemberLevelName = '黄牛卡' or  MemberLevelName = '收分卡' ";

                //如果是门店模式
                ds = XCGameBLL.ExecuteQuerySentence(sql, storeModel.StoreDBName, null);
                List<CattleMemberCardDetailModel> list = new List<CattleMemberCardDetailModel>();
                if (ds != null && ds.Tables.Count >= 1)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        CattleMemberCardDetailModel cattleMemberCardDetailModel = Utils.GetModelList<CattleMemberCardDetailModel>(ds.Tables[0])[i];
                        cattleMemberCardDetailModel.MemberState = MemberBusiness.GetMemberStateName(cattleMemberCardDetailModel.MemberState);
                        list.Add(cattleMemberCardDetailModel);
                    }
                    CattleMemberCardModel cardModel = new CattleMemberCardModel(storeModel.StoreID.ToString(), storeModel.StoreName, list);
                    return ResponseModelFactory<CattleMemberCardModel>.CreateModel(isSignKeyReturn, cardModel);
                }
                else
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "");
                }                
            }
            else if (storeModel.StoreDBDeployType == 1)
            {
                string sn = System.Guid.NewGuid().ToString().Replace("-", "");
                UDPSocketCommonQueryAnswerModel answerModel = null;
                string radarToken = string.Empty;
                if (DataFactory.SendDataCattleMemberCardQuery(sn, storeModel.StoreID.ToString(), storeModel.StorePassword, "0", out radarToken, out errMsg))
                {
                    answerModel = new UDPSocketCommonQueryAnswerModel(sn, storeModel.StoreID.ToString(), storeModel.StorePassword, 0, null, radarToken);
                    UDPSocketCommonQueryAnswerBusiness.AddAnswer(sn, answerModel);
                }
                else
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "");
                }

                answerModel = null;
                while (answerModel == null)
                {
                    //获取应答缓存数据
                    System.Threading.Thread.Sleep(1000);
                    answerModel = UDPSocketCommonQueryAnswerBusiness.GetAnswerModel(sn, 1);
                }

                if (answerModel != null)
                {
                    CattleMemberCardQueryResultNotifyRequestModel model1 = (CattleMemberCardQueryResultNotifyRequestModel)(answerModel.Result);
                    //移除应答缓存数据
                    UDPSocketCommonQueryAnswerBusiness.Remove(sn);

                    if (model1.Result_Code == "1")
                    {
                        List<CattleMemberCardDetailModel> list = new List<CattleMemberCardDetailModel>();
                        for (int i = 0; i < model1.Result_Data.Count; i++)
                        {
                            CattleMemberCardDetailModel detailModel = new CattleMemberCardDetailModel();
                            detailModel.ICCardID = int.Parse(model1.Result_Data[i].ICCardId);
                            detailModel.MemberName = model1.Result_Data[i].Name;
                            detailModel.MemberState = model1.Result_Data[i].MemberState;
                            detailModel.Mobile = model1.Result_Data[i].Phone;
                            list.Add(detailModel);
                        }
                        CattleMemberCardModel cardModel = new CattleMemberCardModel(storeModel.StoreID.ToString(), storeModel.StoreName, list);
                        return ResponseModelFactory<CattleMemberCardModel>.CreateModel(isSignKeyReturn, cardModel);
                    }
                    else
                    {
                        return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "查询数据出错");
                    }
                }
                else
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "查询数据出错");
                }
            }
            else
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "查询数据出错");
            }
        }
    }
}