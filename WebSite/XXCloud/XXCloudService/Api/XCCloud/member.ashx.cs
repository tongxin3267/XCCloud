using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.Business.XCCloud;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.CustomModel.XCCloud.Member;
using XCCloudService.Model.XCCloud;
using System.Activities.Statements;
using XCCloudService.Business.XCGameMana;
using XCCloudService.Model.CustomModel.XCCloud.Store;
using XCCloudService.Business.XCGame;
using System.IO;
using Microsoft.SqlServer.Server;

namespace XCCloudService.Api.XCCloud
{
    [Authorize(Roles = "StoreUser")]
    /// <summary>
    /// member 的摘要说明
    /// </summary>
    public class Member : ApiBase
    {
        #region "登录"

            public object login(Dictionary<string, object> dicParas)
            {
                string errMsg = string.Empty;
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;
                string password = dicParas.ContainsKey("password") ? dicParas["password"].ToString() : string.Empty;

                if (!checkLoginParas(dicParas, out errMsg))
                {
                    ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, errMsg);
                    return responseModel;
                }

                //获取会员信息
                IBase_MemberInfoService memberService = BLLContainer.Resolve<IBase_MemberInfoService>();
                var member = memberService.GetModels(p => p.Mobile.Equals(mobile, StringComparison.OrdinalIgnoreCase)).LastOrDefault<Base_MemberInfo>();
                if (member == null)
                {
                    ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, "会员信息不存在");
                    return responseModel;
                }
                else
                {
                    if (member.UserPassword.Equals(password))
                    {
                        ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.T, "");
                        return responseModel;
                    }
                    else
                    {
                        ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, "用户名或密码错");
                        return responseModel;
                    }
                }
            }

            /// <summary>
            /// 验证登录信息
            /// </summary>
            /// <param name="dicParas"></param>
            /// <param name="errMsg"></param>
            /// <returns></returns>
            private bool checkLoginParas(Dictionary<string, object> dicParas, out string errMsg)
            {
                errMsg = string.Empty;
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;
                string password = dicParas.ContainsKey("password") ? dicParas["password"].ToString() : string.Empty;

                if (!Utils.CheckMobile(mobile))
                {
                    errMsg = "手机号码参数不正确";
                    return false;
                }

                if (string.IsNullOrEmpty(password))
                {
                    errMsg = "用户名参数不能为空";
                    return false;
                }

                if (password.Length > 20)
                {
                    errMsg = "用户名参数长度不能超过20个字符";
                    return false;
                }

                return true;
            }

        #endregion


        #region "验证会员是否需要注册"

            [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
            public object checkOpenCard(Dictionary<string, object> dicParas)
            {
                XCCloudUserTokenModel userTokenModel = (XCCloudUserTokenModel)(dicParas[Constant.XCCloudUserTokenModel]);
                StoreIDDataModel userTokenDataModel = (StoreIDDataModel)(userTokenModel.DataModel);
                string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;
                if (!userTokenDataModel.StoreId.Equals(storeId))
                {
                    ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, "门店信息不正确");
                    return responseModel;
                }

                string storedProcedure = "CheckStoreCanOpenCard";
                SqlParameter[] sqlParameter = new SqlParameter[4];
                sqlParameter[0] = new SqlParameter("@StoreId", SqlDbType.VarChar, 15);
                sqlParameter[0].Value = storeId;
                sqlParameter[1] = new SqlParameter("@Mobile", SqlDbType.VarChar, 11);
                sqlParameter[1].Value = mobile;
                sqlParameter[2] = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 200);
                sqlParameter[2].Direction = ParameterDirection.Output;
                sqlParameter[3] = new SqlParameter("@Return", SqlDbType.Int);
                sqlParameter[3].Direction = ParameterDirection.ReturnValue;
                XCCloudBLL.ExecuteStoredProcedureSentence(storedProcedure, sqlParameter);
                if (sqlParameter[3].Value.ToString() == "1")
                {
                    return new ResponseModel(Return_Code.T, "", Result_Code.T, "");
                }
                else
                {
                    return new ResponseModel(Return_Code.T, "", Result_Code.F, sqlParameter[2].Value.ToString());
                }
            }

        #endregion

        /// <summary>
        /// 注册会员
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken,SysIdAndVersionNo=false)]
        public object register(Dictionary<string, object> dicParas)
        {
            XCCloudUserTokenModel userTokenModel = (XCCloudUserTokenModel)(dicParas[Constant.XCCloudUserTokenModel]);
            StoreIDDataModel userTokenDataModel = (StoreIDDataModel)(userTokenModel.DataModel);
            string errMsg = string.Empty;
            string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
            string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;
            string wechat = dicParas.ContainsKey("wechat") ? dicParas["wechat"].ToString() : string.Empty;
            string qq = dicParas.ContainsKey("qq") ? dicParas["qq"].ToString() : string.Empty;
            string imme = dicParas.ContainsKey("imme") ? dicParas["imme"].ToString() : string.Empty;
            string cardShape = dicParas.ContainsKey("cardShape") ? dicParas["cardShape"].ToString() : string.Empty;
            string memberName = dicParas.ContainsKey("memberName") ? dicParas["memberName"].ToString() : string.Empty;
            string birthday = dicParas.ContainsKey("birthday") ? dicParas["birthday"].ToString() : string.Empty;
            string gender = dicParas.ContainsKey("gender") ? dicParas["gender"].ToString() : string.Empty;
            string identityCard = dicParas.ContainsKey("identityCard") ? dicParas["identityCard"].ToString() : string.Empty;
            string email = dicParas.ContainsKey("email") ? dicParas["email"].ToString() : string.Empty;
            string leftHandCode = dicParas.ContainsKey("leftHandCode") ? dicParas["leftHandCode"].ToString() : string.Empty;
            string rightHandCode = dicParas.ContainsKey("rightHandCode") ? dicParas["rightHandCode"].ToString() : string.Empty;
            string photo = dicParas.ContainsKey("photo") ? dicParas["photo"].ToString() : string.Empty;
            string memberLevelId = dicParas.ContainsKey("memberLevelId") ? dicParas["memberLevelId"].ToString() : string.Empty;     
            string foodId = dicParas.ContainsKey("foodId") ? dicParas["foodId"].ToString() : string.Empty;
            string payCount = dicParas.ContainsKey("payCount") ? dicParas["payCount"].ToString() : string.Empty;
            string realPay = dicParas.ContainsKey("realPay") ? dicParas["realPay"].ToString() : string.Empty;
            string freePay = dicParas.ContainsKey("freePay") ? dicParas["freePay"].ToString() : string.Empty;
            string repeatCode = dicParas.ContainsKey("repeatCode") ? dicParas["repeatCode"].ToString() : string.Empty;     
            string icCardId = dicParas.ContainsKey("icCardId") ? dicParas["icCardId"].ToString() : string.Empty;
            string workStation = dicParas.ContainsKey("workStation") ? dicParas["workStation"].ToString() : string.Empty;
            string note = dicParas.ContainsKey("note") ? dicParas["note"].ToString() : string.Empty;
            string deposit = dicParas.ContainsKey("deposit") ? dicParas["deposit"].ToString() : string.Empty;
            string payType = dicParas.ContainsKey("payType") ? dicParas["payType"].ToString() : string.Empty;
            string saleCoinType = dicParas.ContainsKey("saleCoinType") ? dicParas["saleCoinType"].ToString() : string.Empty;


            if (!checkRegisterParas(dicParas,out errMsg))
            {
                ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, errMsg);
                return responseModel;
            }

            string storedProcedure = "RegisterMember";
            String[] Ary = new String[] { 
                "数据0", "数据1", "数据2", "数据3", "数据4", 
                "数据5", "数据6", "数据7", "数据8", "数据9",
                "数据10", "数据11", "数据12", "数据13", "数据14",
                "数据15", "数据16", "数据17", "数据18", "数据19",
                "数据20", "数据21", "数据22", "数据23", "数据24",
                "数据25", "数据26", "数据27"
            };
                
            List<object> listParas = new List<object>();
            listParas.Add(storeId);//StoreId
            listParas.Add(mobile);//Mobile
            listParas.Add(wechat);//WeChat
            listParas.Add(qq);//QQ
            listParas.Add(imme);//IMME

            listParas.Add(int.Parse(cardShape));//CardShape
            listParas.Add(memberName);//MemberName
            listParas.Add("888888");//MemberPassword
            listParas.Add(birthday);//Birthday
            listParas.Add(gender);//Gender

            listParas.Add(identityCard);//IdentityCard
            listParas.Add(email);//EMail
            listParas.Add(leftHandCode);//LeftHandCode
            listParas.Add(rightHandCode);//RightHandCode
            listParas.Add(photo);//Photo

            listParas.Add(int.Parse(memberLevelId));//MemberLevelId 
            listParas.Add(int.Parse(foodId));//FoodId
            listParas.Add(decimal.Parse(payCount));//payCount
            listParas.Add(decimal.Parse(realPay));//realPay
            listParas.Add(decimal.Parse(freePay));//freePay

            listParas.Add(int.Parse(repeatCode));//repeatCode 
            listParas.Add(int.Parse(icCardId));//icCardId
            listParas.Add(workStation);//workStation
            listParas.Add(int.Parse(userTokenModel.LogId));//UserId
            listParas.Add(decimal.Parse(deposit));//deposit

            listParas.Add((long)0);//icCardUID
            listParas.Add(int.Parse(payType));//payType
            listParas.Add(int.Parse(saleCoinType));//saleCoinType


            List<SqlDataRecord> listSqlDataRecord = new List<SqlDataRecord>();
            SqlMetaData[] MetaDataArr = new SqlMetaData[] { 
                new SqlMetaData("StoreId", SqlDbType.VarChar,15), 
                new SqlMetaData("Mobile", SqlDbType.VarChar,20),
                new SqlMetaData("WeChat", SqlDbType.VarChar,64),
                new SqlMetaData("QQ", SqlDbType.VarChar,64),
                new SqlMetaData("IMME", SqlDbType.VarChar,64),

                new SqlMetaData("CardShape", SqlDbType.Int),
                new SqlMetaData("MemberName", SqlDbType.VarChar,50),
                new SqlMetaData("MemberPassword", SqlDbType.VarChar,20),
                new SqlMetaData("Birthday", SqlDbType.VarChar,16),
                new SqlMetaData("Gender", SqlDbType.VarChar,1),

                new SqlMetaData("IdentityCard", SqlDbType.VarChar,50),
                new SqlMetaData("EMail", SqlDbType.VarChar,50),
                new SqlMetaData("LeftHandCode", SqlDbType.VarChar,5000),
                new SqlMetaData("RightHandCode", SqlDbType.VarChar,5000),
                new SqlMetaData("Photo", SqlDbType.VarChar,100),

                new SqlMetaData("MemberLevelId", SqlDbType.Int),
                new SqlMetaData("FoodId", SqlDbType.Int),
                new SqlMetaData("PayCount", SqlDbType.Decimal),
                new SqlMetaData("RealPay", SqlDbType.Decimal),
                new SqlMetaData("FreePay", SqlDbType.Decimal),

                new SqlMetaData("RepeatCode", SqlDbType.Int),
                new SqlMetaData("ICCardId", SqlDbType.Int),
                new SqlMetaData("WorkStation", SqlDbType.VarChar,50),
                new SqlMetaData("UserId", SqlDbType.Int),
                new SqlMetaData("Deposit", SqlDbType.Decimal),

                new SqlMetaData("ICCardUID", SqlDbType.BigInt),
                new SqlMetaData("PayType", SqlDbType.Int),
                new SqlMetaData("SaleCoinType", SqlDbType.Int)
            };

            var record = new SqlDataRecord(MetaDataArr);
            for (int i = 0; i < Ary.Length; i++)
            {  
                record.SetValue(i, listParas[i]);    
            }
            listSqlDataRecord.Add(record);
      
            SqlParameter[] sqlParameter = new SqlParameter[4];
            sqlParameter[0] = new SqlParameter("@RegisterMember", SqlDbType.Structured);
            sqlParameter[0].Value = listSqlDataRecord;
            sqlParameter[1] = new SqlParameter("@Note", SqlDbType.Text);
            sqlParameter[1].Value = note;
            sqlParameter[2] = new SqlParameter("@ErrMsg", SqlDbType.VarChar,200);
            sqlParameter[2].Direction = ParameterDirection.Output;
            sqlParameter[3] = new SqlParameter("@Result", SqlDbType.Int);
            sqlParameter[3].Direction = ParameterDirection.Output;
            System.Data.DataSet ds = XCCloudBLL.GetStoredProcedureSentence(storedProcedure, sqlParameter);
            if (sqlParameter[3].Value.ToString() == "1")
            {
                var baseMemberModel = Utils.GetModelList<BaseMemberModel>(ds.Tables[0]).ToList()[0];
                return ResponseModelFactory<BaseMemberModel>.CreateModel(isSignKeyReturn, baseMemberModel);
            }
            else
            {
                return new ResponseModel(Return_Code.T, "", Result_Code.F, sqlParameter[2].Value.ToString());
            }
        }


        /// <summary>
        /// 退出会员
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object exit(Dictionary<string, object> dicParas)
        {
            return null;
        }

        /// <summary>
        /// 验证注册参数
        /// </summary>
        /// <returns></returns>
        private bool checkRegisterParas(Dictionary<string, object> dicParas,out string errMsg)
        {
            errMsg = string.Empty;
            string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
            string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;
            string wechat = dicParas.ContainsKey("wechat") ? dicParas["wechat"].ToString() : string.Empty;
            string qq = dicParas.ContainsKey("qq") ? dicParas["qq"].ToString() : string.Empty;
            string imme = dicParas.ContainsKey("imme") ? dicParas["imme"].ToString() : string.Empty;
            string cardShape = dicParas.ContainsKey("cardShape") ? dicParas["cardShape"].ToString() : string.Empty;
            string memberName = dicParas.ContainsKey("memberName") ? dicParas["memberName"].ToString() : string.Empty;
            string birthday = dicParas.ContainsKey("birthday") ? dicParas["birthday"].ToString() : string.Empty;
            string gender = dicParas.ContainsKey("gender") ? dicParas["gender"].ToString() : string.Empty;
            string identityCard = dicParas.ContainsKey("identityCard") ? dicParas["identityCard"].ToString() : string.Empty;
            string email = dicParas.ContainsKey("email") ? dicParas["email"].ToString() : string.Empty;
            string leftHandCode = dicParas.ContainsKey("leftHandCode") ? dicParas["leftHandCode"].ToString() : string.Empty;
            string rightHandCode = dicParas.ContainsKey("rightHandCode") ? dicParas["rightHandCode"].ToString() : string.Empty;
            string photo = dicParas.ContainsKey("photo") ? dicParas["photo"].ToString() : string.Empty;
            string memberLevelId = dicParas.ContainsKey("memberLevelId") ? dicParas["memberLevelId"].ToString() : string.Empty;
            string foodId = dicParas.ContainsKey("foodId") ? dicParas["foodId"].ToString() : string.Empty;
            string payCount = dicParas.ContainsKey("payCount") ? dicParas["payCount"].ToString() : string.Empty;
            string realPay = dicParas.ContainsKey("realPay") ? dicParas["realPay"].ToString() : string.Empty;
            string freePay = dicParas.ContainsKey("freePay") ? dicParas["freePay"].ToString() : string.Empty;
            string repeatCode = dicParas.ContainsKey("repeatCode") ? dicParas["repeatCode"].ToString() : string.Empty;
            string icCardId = dicParas.ContainsKey("icCardId") ? dicParas["icCardId"].ToString() : string.Empty;
            string workStation = dicParas.ContainsKey("workStation") ? dicParas["workStation"].ToString() : string.Empty;
            string note = dicParas.ContainsKey("note") ? dicParas["note"].ToString() : string.Empty;
            string deposit = dicParas.ContainsKey("deposit") ? dicParas["deposit"].ToString() : string.Empty;
            string payType = dicParas.ContainsKey("payType") ? dicParas["payType"].ToString() : string.Empty;
            string saleCoinType = dicParas.ContainsKey("saleCoinType") ? dicParas["saleCoinType"].ToString() : string.Empty;

            if (string.IsNullOrEmpty(mobile))
            {
                errMsg = "会员手机号码不能为空";
                return false;
            }

            if (!Utils.CheckMobile(mobile))
            {
                errMsg = "会员手机号码格式不正确";
                return false;
            }

            if (string.IsNullOrEmpty(memberName))
            {
                errMsg = "会员姓名不能为空";
                return false;
            }

            if (string.IsNullOrEmpty(birthday))
            {
                errMsg = "会员生日不能为空";
                return false;
            }

            if (string.IsNullOrEmpty(birthday))
            {
                errMsg = "会员生日不能为空";
                return false;
            }

            if (string.IsNullOrEmpty(gender))
            {
                errMsg = "会员性别不能为空";
                return false;
            }

            if (!gender.Equals("0") && !gender.Equals("1"))
            {
                errMsg = "会员性别无效";
                return false;
            }

            if (string.IsNullOrEmpty(identityCard))
            {
                errMsg = "证件号码不能为空";
                return false;
            }

            if (string.IsNullOrEmpty(memberLevelId))
            {
                errMsg = "会员级别不能为空";
                return false;
            }

            if (string.IsNullOrEmpty(icCardId))
            {
                errMsg = "IC卡卡号不能为空";
                return false;
            }

            if (string.IsNullOrEmpty(repeatCode))
            {
                errMsg = "IC卡重复码不能为空";
                return false;
            }

            if (!payType.Equals("0") && !payType.Equals("1") && !payType.Equals("2") && !payType.Equals("3"))
            {
                errMsg = "支付类型无效";
                return false;
            }

            if (!saleCoinType.Equals("1") && !saleCoinType.Equals("2") && !saleCoinType.Equals("3") )
            {
                errMsg = "售币类型无效";
                return false;
            }

            if (!Utils.IsNumeric(payCount) || decimal.Parse(payCount) < 0)
            {
                errMsg = "应付金额无效";
                return false;
            }

            if (!Utils.IsNumeric(realPay) || decimal.Parse(realPay) < 0)
            {
                errMsg = "实付金额无效";
                return false;
            }

            if (!Utils.IsNumeric(freePay) || decimal.Parse(freePay) < 0 )
            {
                errMsg = "免费金额无效";
                return false;
            }

            if (decimal.Parse(payCount) - decimal.Parse(freePay) != decimal.Parse(realPay))
            {
                errMsg = "实付金额无效";
                return false;
            }

            //0 普通卡;1 数字手环;2其他
            if (!cardShape.Equals("0") && !cardShape.Equals("1") && !cardShape.Equals("2"))
            {
                errMsg = "卡类型无效";
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取会员等级
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object getMemberLevel(Dictionary<string, object> dicParas)
        {
            XCCloudUserTokenModel userTokenModel = (XCCloudUserTokenModel)(dicParas[Constant.XCCloudUserTokenModel]);
            StoreIDDataModel userTokenDataModel = (StoreIDDataModel)(userTokenModel.DataModel);

            string storedProcedure = "GetMemberLevel";
            SqlParameter[] parameters = new SqlParameter[1];
            parameters[0] = new SqlParameter("@StoreId", userTokenDataModel.StoreId);
            System.Data.DataSet ds = XCCloudBLL.GetStoredProcedureSentence(storedProcedure, parameters);
            List<Data_MemberLevelModel> list = Utils.GetModelList<Data_MemberLevelModel>(ds.Tables[0]);

            return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, list);
        }

        /// <summary>
        /// 获取会员
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken,SysIdAndVersionNo=false)]
        public object getMemberRepeatCode(Dictionary<string, object> dicParas)
        {
            XCCloudUserTokenModel userTokenModel = (XCCloudUserTokenModel)(dicParas[Constant.XCCloudUserTokenModel]);
            string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
            string icCardId = dicParas.ContainsKey("icCardId") ? dicParas["icCardId"].ToString() : string.Empty;

            if (string.IsNullOrEmpty(storeId))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店号不能为空");
            }

            StoreInfoCacheModel storeInfo = null;
            if(!XCCloudStoreBusiness.IsEffectiveStore(storeId,ref storeInfo))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店号无效");
            }

            if (string.IsNullOrEmpty(icCardId))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "会员卡号无效");
            }
            //获取会员信息
            string storedProcedure = "GetMemberRepeatCode";
            SqlParameter[] parameters = new SqlParameter[5];
            parameters[0] = new SqlParameter("@ICCardID", icCardId);
            parameters[1] = new SqlParameter("@StoreID", storeId);
            parameters[2] = new SqlParameter("@RepeatCode", SqlDbType.Int);
            parameters[2].Direction = System.Data.ParameterDirection.Output;
            parameters[3] = new SqlParameter("@ErrMsg", SqlDbType.VarChar, 200);
            parameters[3].Direction = System.Data.ParameterDirection.Output;
            parameters[4] = new SqlParameter("@Return", SqlDbType.Int);
            parameters[4].Direction = System.Data.ParameterDirection.ReturnValue;
            XCCloudBLL.ExecuteStoredProcedureSentence(storedProcedure, parameters);
            if (parameters[4].Value.ToString() == "1")
            {
                var obj = new {
                    repeatCode = parameters[2].Value.ToString()
                };
                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, obj);
            }
            else
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "会员卡无效");
            }    
        }

        /// <summary>
        /// 获取会员
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object getMember(Dictionary<string, object> dicParas)
        {
            XCCloudUserTokenModel userTokenModel = (XCCloudUserTokenModel)(dicParas[Constant.XCCloudUserTokenModel]);
            StoreIDDataModel userTokenDataModel = (StoreIDDataModel)(userTokenModel.DataModel);

            string icCardId = dicParas.ContainsKey("icCardId") ? dicParas["icCardId"].ToString() : string.Empty;
            string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;

            if (string.IsNullOrEmpty(icCardId))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "会员卡号无效");
            }
            if (string.IsNullOrEmpty(storeId))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店号无效");
            }

            string storedProcedure = "GetMember";
            SqlParameter[] parameters = new SqlParameter[4];
            parameters[0] = new SqlParameter("@ICCardID", icCardId);
            parameters[1] = new SqlParameter("@StoreID", storeId);
            parameters[2] = new SqlParameter("@Result",SqlDbType.Int);
            parameters[2].Direction = System.Data.ParameterDirection.Output;
            parameters[3] = new SqlParameter("@ErrMsg", SqlDbType.VarChar,200);
            parameters[3].Direction = System.Data.ParameterDirection.Output;
            System.Data.DataSet ds = XCCloudBLL.GetStoredProcedureSentence(storedProcedure, parameters);
            if (parameters[2].Value.ToString() == "1")
            {
                var baseMemberModel = Utils.GetModelList<BaseMemberModel>(ds.Tables[0]).ToList()[0];
                return ResponseModelFactory<BaseMemberModel>.CreateModel(isSignKeyReturn, baseMemberModel);
            }
            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "会员信息不存在");
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetStoreMemberLevel(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;
                IData_MemberLevelService data_MemberLevelService = BLLContainer.Resolve<IData_MemberLevelService>();
                Dictionary<int, string> memberLevel = data_MemberLevelService.GetModels(p => p.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase) && p.State == 1).Select(o => new
                {
                    MemberLevelID = o.MemberLevelID,
                    MemberLevelName = o.MemberLevelName
                }).Distinct().ToDictionary(d => d.MemberLevelID, d => d.MemberLevelName);
                
                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, memberLevel);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [Authorize(Roles = "MerchUser")]
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetMemberDic(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string merchId = userTokenKeyModel.DataModel.MerchID;

                IBase_StoreInfoService base_StoreInfoService = BLLContainer.Resolve<IBase_StoreInfoService>(resolveNew:true);
                IBase_MemberInfoService base_MemberInfoService = BLLContainer.Resolve<IBase_MemberInfoService>(resolveNew: true);
                IData_Member_CardService data_Member_CardService = BLLContainer.Resolve<IData_Member_CardService>(resolveNew: true);
                IData_Member_Card_StoreService data_Member_Card_StoreService = BLLContainer.Resolve<IData_Member_Card_StoreService>(resolveNew: true);
                var linq = from a in base_StoreInfoService.GetModels(p => p.MerchID.Equals(merchId, StringComparison.OrdinalIgnoreCase))
                           join b in data_Member_Card_StoreService.GetModels() on a.StoreID equals b.StoreID
                           join c in data_Member_CardService.GetModels() on b.CardID equals c.ID
                           join d in base_MemberInfoService.GetModels() on c.MemberID equals d.ID
                           select new 
                           {
                               MemberID = d.ID,
                               MemberName = d.UserName
                           };

                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, linq);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object QueryMemberLevel(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string memberLevelID = dicParas.ContainsKey("memberLevelID") ? dicParas["memberLevelID"].ToString() : string.Empty;
                string memberLevelName = dicParas.ContainsKey("memberLevelName") ? dicParas["memberLevelName"].ToString() : string.Empty;
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;


                IData_MemberLevelService data_MemberLevelService = BLLContainer.Resolve<IData_MemberLevelService>();
                var query = data_MemberLevelService.GetModels(p => p.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(memberLevelID))
                {
                    var iMemberLevelID = Convert.ToInt32(memberLevelID);
                    query = query.Where(w => w.MemberLevelID == iMemberLevelID);
                }

                if (!string.IsNullOrEmpty(memberLevelName))
                {
                    query = query.Where(w => w.MemberLevelName.Contains(memberLevelName));
                }

                IDict_SystemService dict_SystemService = BLLContainer.Resolve<IDict_SystemService>();
                int MustInputId = dict_SystemService.GetModels(p => p.DictKey.Equals("入会时必须输入")).FirstOrDefault().ID;
                int BirthdayFreeId = dict_SystemService.GetModels(p => p.DictKey.Equals("生日送币方式")).FirstOrDefault().ID;
                int FreeRateId = dict_SystemService.GetModels(p => p.DictKey.Equals("送币频率")).FirstOrDefault().ID;
                var linq = query.ToList().Select(o => new
                {
                    MemberLevelModel = o,
                    MustInput = dict_SystemService.GetModels(p => p.PID == MustInputId),
                    BirthdayFree = dict_SystemService.GetModels(p => p.PID == BirthdayFreeId),
                    FreeRate = dict_SystemService.GetModels(p => p.PID == FreeRateId)
                }).Select(oo => new
                {
                    MemberLevelID = oo.MemberLevelModel.MemberLevelID,
                    MemberLevelName = oo.MemberLevelModel.MemberLevelName,
                    Deposit = oo.MemberLevelModel.Deposit,
                    ExitMoney = oo.MemberLevelModel.ExitMoney,
                    ExitCoin = oo.MemberLevelModel.ExitCoin,
                    ExitPrice = oo.MemberLevelModel.ExitPrice,
                    Validday = oo.MemberLevelModel.Validday,
                    FreeRate = oo.MemberLevelModel.FreeRate,
                    FreeCoin = oo.MemberLevelModel.FreeCoin,
                    BirthdayFree = oo.MemberLevelModel.BirthdayFree,
                    BirthdayFreeCoin = oo.MemberLevelModel.BirthdayFreeCoin,
                    MustInput = oo.MemberLevelModel.MustInput,
                    AllowExitCard = oo.MemberLevelModel.AllowExitCard == 1 ? "允许" : oo.MemberLevelModel.AllowExitCard == 0 ? "禁止" : string.Empty,
                    AllowExitMoney = oo.MemberLevelModel.AllowExitMoney == 1 ? "允许" : oo.MemberLevelModel.AllowExitMoney == 0 ? "禁止" : string.Empty,
                    AllowExitCoinToCard = oo.MemberLevelModel.AllowExitCoinToCard == 1 ? "允许" : oo.MemberLevelModel.AllowExitCoinToCard == 0 ? "禁止" : string.Empty,
                    LockHead = oo.MemberLevelModel.LockHead == 1 ? "允许" : oo.MemberLevelModel.LockHead == 0 ? "禁止" : string.Empty,
                    MinExitCoin = oo.MemberLevelModel.MinExitCoin,
                    FreeType = oo.MemberLevelModel.FreeType,
                    FreeNeedWin = oo.MemberLevelModel.FreeNeedWin,
                    MustInputStr = oo.MustInput.Any(s => s.DictValue.Equals(oo.MemberLevelModel.MustInput + "")) ? oo.MustInput.Single(s => s.DictValue.Equals(oo.MemberLevelModel.MustInput + "")).DictKey : string.Empty,
                    BirthdayFreeStr = oo.BirthdayFree.Any(s => s.DictValue.Equals(oo.MemberLevelModel.BirthdayFree + "")) ? oo.BirthdayFree.Single(s => s.DictValue.Equals(oo.MemberLevelModel.BirthdayFree + "")).DictKey : string.Empty,
                    FreeRateStr = oo.FreeRate.Any(s => s.DictValue.Equals(oo.MemberLevelModel.FreeRate + "")) ? oo.FreeRate.Single(s => s.DictValue.Equals(oo.MemberLevelModel.FreeRate + "")).DictKey : string.Empty,
                    StateStr = (oo.MemberLevelModel.State == 1) ? "启用" : "停用"
                });

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, linq.ToList());
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object SaveMemberLevel(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;

                string errMsg = string.Empty;
                string memberLevelID = dicParas.ContainsKey("memberLevelID") ? (dicParas["memberLevelID"] + "") : string.Empty;
                string memberLevelName = dicParas.ContainsKey("memberLevelName") ? (dicParas["memberLevelName"] + "") : string.Empty;
                string deposit = dicParas.ContainsKey("deposit") ? (dicParas["deposit"] + "") : string.Empty;
                string exitMoney = dicParas.ContainsKey("exitMoney") ? (dicParas["exitMoney"] + "") : string.Empty;
                string exitCoin = dicParas.ContainsKey("exitCoin") ? (dicParas["exitCoin"] + "") : string.Empty;
                string exitPrice = dicParas.ContainsKey("exitPrice") ? (dicParas["exitPrice"] + "") : string.Empty;
                string validday = dicParas.ContainsKey("validday") ? (dicParas["validday"] + "") : string.Empty;
                string freeRate = dicParas.ContainsKey("freeRate") ? (dicParas["freeRate"] + "") : string.Empty;
                string freeCoin = dicParas.ContainsKey("freeCoin") ? (dicParas["freeCoin"] + "") : string.Empty;
                string birthdayFree = dicParas.ContainsKey("birthdayFree") ? (dicParas["birthdayFree"] + "") : string.Empty;
                string birthdayFreeCoin = dicParas.ContainsKey("birthdayFreeCoin") ? (dicParas["birthdayFreeCoin"] + "") : string.Empty;
                string mustInput = dicParas.ContainsKey("mustInput") ? (dicParas["mustInput"] + "") : string.Empty;
                string allowExitCard = dicParas.ContainsKey("allowExitCard") ? (dicParas["allowExitCard"] + "") : string.Empty;
                string allowExitMoney = dicParas.ContainsKey("allowExitMoney") ? (dicParas["allowExitMoney"] + "") : string.Empty;
                string allowExitCoinToCard = dicParas.ContainsKey("allowExitCoinToCard") ? (dicParas["allowExitCoinToCard"] + "") : string.Empty;
                string lockHead = dicParas.ContainsKey("lockHead") ? (dicParas["lockHead"] + "") : string.Empty;
                string minExitCoin = dicParas.ContainsKey("minExitCoin") ? (dicParas["minExitCoin"] + "") : string.Empty;
                string freeType = dicParas.ContainsKey("freeType") ? (dicParas["freeType"] + "") : string.Empty;
                string freeNeedWin = dicParas.ContainsKey("freeNeedWin") ? (dicParas["freeNeedWin"] + "") : string.Empty;
                string state = dicParas.ContainsKey("state") ? (dicParas["state"] + "") : string.Empty;
                string cardUIURL = dicParas.ContainsKey("cardUIURL") ? (dicParas["cardUIURL"] + "") : string.Empty;
                string needAuthor = dicParas.ContainsKey("needAuthor") ? (dicParas["needAuthor"] + "") : string.Empty;

                if (string.IsNullOrEmpty(memberLevelName))
                {
                    errMsg = "会员级别名称不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(cardUIURL))
                {
                    errMsg = "会员卡封面地址不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iMemberLevelID = 0;
                int.TryParse(memberLevelID, out iMemberLevelID);
                IData_MemberLevelService data_MemberLevelService = BLLContainer.Resolve<IData_MemberLevelService>();
                if (data_MemberLevelService.Any(a => a.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase) &&
                    a.MemberLevelName.Equals(memberLevelName, StringComparison.OrdinalIgnoreCase) && a.MemberLevelID != iMemberLevelID))
                {
                    errMsg = "该会员级别名称已存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                var data_MemberLevel = new Data_MemberLevel();
                data_MemberLevel.MemberLevelID = iMemberLevelID;
                data_MemberLevel.MemberLevelName = memberLevelName;
                data_MemberLevel.Deposit = !string.IsNullOrEmpty(deposit) ? Convert.ToDecimal(deposit) : (decimal?)null;
                data_MemberLevel.ExitMoney = !string.IsNullOrEmpty(exitMoney) ? Convert.ToDecimal(exitMoney) : (decimal?)null;
                data_MemberLevel.ExitCoin = !string.IsNullOrEmpty(exitCoin) ? Convert.ToInt32(exitCoin) : (int?)null;
                data_MemberLevel.ExitPrice = !string.IsNullOrEmpty(exitPrice) ? Convert.ToDecimal(exitPrice) : (decimal?)null;
                data_MemberLevel.Validday = !string.IsNullOrEmpty(validday) ? Convert.ToInt32(validday) : (int?)null;
                data_MemberLevel.FreeRate = !string.IsNullOrEmpty(freeRate) ? Convert.ToInt32(freeRate) : (int?)null;
                data_MemberLevel.FreeCoin = !string.IsNullOrEmpty(freeCoin) ? Convert.ToInt32(freeCoin) : (int?)null;
                data_MemberLevel.BirthdayFree = !string.IsNullOrEmpty(birthdayFree) ? Convert.ToInt32(birthdayFree) : (int?)null;
                data_MemberLevel.BirthdayFreeCoin = !string.IsNullOrEmpty(birthdayFreeCoin) ? Convert.ToInt32(birthdayFreeCoin) : (int?)null;
                data_MemberLevel.MustInput = !string.IsNullOrEmpty(mustInput) ? Convert.ToInt32(mustInput) : (int?)null;
                data_MemberLevel.AllowExitCard = !string.IsNullOrEmpty(allowExitCard) ? Convert.ToInt32(allowExitCard) : (int?)null;
                data_MemberLevel.AllowExitMoney = !string.IsNullOrEmpty(allowExitMoney) ? Convert.ToInt32(allowExitMoney) : (int?)null;
                data_MemberLevel.AllowExitCoinToCard = !string.IsNullOrEmpty(allowExitCoinToCard) ? Convert.ToInt32(allowExitCoinToCard) : (int?)null;
                data_MemberLevel.LockHead = !string.IsNullOrEmpty(lockHead) ? Convert.ToInt32(lockHead) : (int?)null;
                data_MemberLevel.MinExitCoin = !string.IsNullOrEmpty(minExitCoin) ? Convert.ToInt32(minExitCoin) : (int?)null;
                data_MemberLevel.FreeType = !string.IsNullOrEmpty(freeType) ? Convert.ToInt32(freeType) : (int?)null;
                data_MemberLevel.FreeNeedWin = !string.IsNullOrEmpty(freeNeedWin) ? Convert.ToInt32(freeNeedWin) : (int?)null;
                data_MemberLevel.State = !string.IsNullOrEmpty(state) ? Convert.ToInt32(state) : (int?)null;
                data_MemberLevel.StoreID = storeId;
                data_MemberLevel.CardUIURL = cardUIURL;
                data_MemberLevel.NeedAutor = !string.IsNullOrEmpty(needAuthor) ? Convert.ToInt32(needAuthor) : (int?)null;

                if (!data_MemberLevelService.Any(a => a.MemberLevelID == iMemberLevelID))
                {
                    //新增
                    if (!data_MemberLevelService.Add(data_MemberLevel))
                    {
                        errMsg = "添加会员级别失败";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                }
                else
                {
                    //修改
                    if (!data_MemberLevelService.Update(data_MemberLevel))
                    {
                        errMsg = "修改会员级别失败";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                }

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetMemberLevelInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string storeId = (userTokenKeyModel.DataModel as UserDataModel).StoreID;

                string errMsg = string.Empty;
                string memberLevelID = dicParas.ContainsKey("memberLevelID") ? dicParas["memberLevelID"].ToString() : string.Empty;

                if (string.IsNullOrEmpty(memberLevelID))
                {
                    errMsg = "会员级别ID不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                int iMemberLevelID = Convert.ToInt32(memberLevelID);
                IData_MemberLevelService data_MemberLevelService = BLLContainer.Resolve<IData_MemberLevelService>();
                if (!data_MemberLevelService.Any(a => a.MemberLevelID == iMemberLevelID))
                {
                    errMsg = "该会员级别不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                var data_MemberLevelModel = data_MemberLevelService.GetModels(p => p.MemberLevelID == iMemberLevelID).FirstOrDefault();

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, data_MemberLevelModel);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object UploadCardCover(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;

                #region 验证参数

                var file = HttpContext.Current.Request.Files[0];
                if (file == null)
                {
                    errMsg = "未找到图片";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (file.ContentLength > int.Parse(System.Configuration.ConfigurationManager.AppSettings["MaxImageSize"].ToString()))
                {
                    errMsg = "超过图片的最大限制为1M";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                #endregion

                string picturePath = System.Configuration.ConfigurationManager.AppSettings["UploadImageUrl"].ToString() + "/XCCloud/Store/Member/";
                string path = System.Web.HttpContext.Current.Server.MapPath(picturePath);
                //如果不存在就创建file文件夹
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = Path.GetFileNameWithoutExtension(file.FileName) + Utils.ConvertDateTimeToLong(DateTime.Now) + Path.GetExtension(file.FileName);

                if (File.Exists(path + fileName))
                {
                    errMsg = "图片名称已存在，请重命名后上传";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                file.SaveAs(path + fileName);

                Dictionary<string, string> dicStoreInfo = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                dicStoreInfo.Add("CardUIURL", picturePath + fileName);
                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, dicStoreInfo);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

    }
}