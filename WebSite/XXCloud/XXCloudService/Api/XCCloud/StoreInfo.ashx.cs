using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCCloud;
using XCCloudService.BLL.XCCloud;
using XCCloudService.Business.XCCloud;
using XCCloudService.Common;
using XCCloudService.Common.Enum;
using XCCloudService.DAL;
using XCCloudService.DBService.BLL;
using XCCloudService.Model.CustomModel.XCCloud;
using XCCloudService.Model.WeiXin.Message;
using XCCloudService.Model.XCCloud;
using XCCloudService.WeiXin.Message;

namespace XCCloudService.Api.XCCloud
{
    /// <summary>
    /// StoreInfo 的摘要说明
    /// </summary>
    public class StoreInfo : ApiBase
    {        
        private bool getAdcode(string lat, string lng, out string adcode, out string errMsg)
        {
            adcode = string.Empty;
            errMsg = string.Empty;

            //解析维度、精度对应的地址信息
            string json = MapService.GetBaiduMapCoordinateAnalysis(lat, lng);
            CoordinateAnalysisModel coordinateAnalysisModel = Utils.DataContractJsonDeserializer<CoordinateAnalysisModel>(json);

            if (coordinateAnalysisModel == null || coordinateAnalysisModel.Status != 0)
            {
                errMsg = "位置解析出错";
                return false;
            }

            //获取行政区划代码
            adcode = coordinateAnalysisModel.Result.AddressComponent.Adcode;

            //行政区划代码是否存在
            IDict_AreaService dict_AreaService = BLLContainer.Resolve<IDict_AreaService>();
            var ac = adcode;
            var dict_AreaModel = dict_AreaService.GetModels(p => p.ID.ToString().Equals(ac)).FirstOrDefault<Dict_Area>();
            if (dict_AreaModel == null)
            {
                errMsg = "行政区划代码无效";
                return false;
            }

            return true;
        }

        private bool genNo(string merchId, string adcode, out string storeId, out string errMsg)
        {
            errMsg = string.Empty;
            storeId = string.Empty;
            IBase_StoreInfoService base_StoreInfoService = BLLContainer.Resolve<IBase_StoreInfoService>();
            string lastStoreID = base_StoreInfoService.SqlQuery("select * from Base_StoreInfo where StoreID like '" + merchId + adcode + "%'").Max(m => m.StoreID);
            if (string.IsNullOrEmpty(lastStoreID))
            {
                lastStoreID = merchId + adcode + "000";
            }

            var sNo = Convert.ToInt32(lastStoreID.Substring(lastStoreID.Length - 3));
            if (sNo >= 999)
            {
                errMsg = "门店数量已满额，每个商户整个行政区域内最多支持999个门店";
                return false;
            }

            sNo = sNo + 1;
            storeId = merchId + adcode + sNo.ToString().PadLeft(3, '0');
            return true;
        }

        private void MessagePush(string openId, string merchAccount, string registerTime, string storeName, string workId)
        {
            string errMsg = string.Empty;
            LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, "MessagePush");
            StoreRegisterRemindDataModel dataModel = new StoreRegisterRemindDataModel(merchAccount, registerTime, storeName, workId);
            if (MessageMana.PushMessage(WeiXinMesageType.StoreRegisterRemind, openId, dataModel, out errMsg))
            {
                LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, "true");
            }
            else
            {
                LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, errMsg);
            }
        }

        public object nearbyStores(Dictionary<string, object> dicParas)
        {
            //纬度
            string latitude = dicParas.ContainsKey("latitude") ? dicParas["latitude"].ToString() : string.Empty;
            //经度
            string longitude = dicParas.ContainsKey("longitude") ? dicParas["longitude"].ToString() : string.Empty;
            //解析维度、精度对应的地址信息
            string json = MapService.GetBaiduMapCoordinateAnalysis(latitude, longitude);
            CoordinateAnalysisModel coordinateAnalysisModel = Utils.DataContractJsonDeserializer<CoordinateAnalysisModel>(json);

            if (coordinateAnalysisModel == null || coordinateAnalysisModel.Status != 0)
            {
                ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, "位置解析出错");
                return responseModel;
            }

            //获取行政区划代码
            string adcode = coordinateAnalysisModel.Result.AddressComponent.Adcode;

            //行政区划代码是否存在
            IDict_AreaService dict_AreaService = new Dict_AreaService();
            var dict_AreaModel = dict_AreaService.GetModels(p => p.ID.ToString().Equals(adcode)).FirstOrDefault<Dict_Area>();
            if (dict_AreaModel == null)
            {
                ResponseModel responseModel = new ResponseModel(Return_Code.T, "", Result_Code.F, "行政区划代码无效");
                return responseModel;
            }

            //行政区划代码是否存在
            IBase_StoreInfoService storeInfoService = new Base_StoreInfoService();
            var storeInfoModelList = storeInfoService.GetModels(p => p.AreaCode.Equals(dict_AreaModel.PID.ToString())).ToList<Base_StoreInfo>();
            List<Base_StoreInfoModel> list = Utils.GetCopyList<Base_StoreInfoModel, Base_StoreInfo>(storeInfoModelList);
            ResponseModel<List<Base_StoreInfoModel>> responseModel1 = new ResponseModel<List<Base_StoreInfoModel>>(list);
            return responseModel1;
        }

        [Authorize(Roles = "XcUser,XcAdmin,MerchUser")]
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetStoreList(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string merchId = dicParas.ContainsKey("merchId") ? dicParas["merchId"].ToString() : string.Empty;
                object[] conditions = dicParas.ContainsKey("conditions") ? (object[])dicParas["conditions"] : null;

                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];               
                if (userTokenKeyModel.LogType == (int)RoleType.MerchUser)
                {
                    merchId = userTokenKeyModel.LogId;
                }

                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@merchId", merchId);

                string sqlWhere = string.Empty;                
                if (conditions != null && conditions.Length > 0)
                {
                    if (!QueryBLL.GenDynamicSql(conditions, "a.", ref sqlWhere, ref parameters, out errMsg))
                    {
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                }

                string sql = @"select a.StoreID, a.MerchID, a.StoreName, a.Password, a.AuthorExpireDate, a.AreaCode, a.Address, a.Contacts, a.Mobile, b.DictKey as SelttleTypeStr, c.DictKey as StoreStateStr from Base_StoreInfo a " +
                    " left join (select b.* from Dict_System a inner join Dict_System b on a.ID=b.PID where a.DictKey='结算类型' and a.PID=0) b on convert(varchar, a.SelttleType)=b.DictValue " +
                    " left join (select b.* from Dict_System a inner join Dict_System b on a.ID=b.PID where a.DictKey='门店状态' and a.PID=0) c on convert(varchar, a.StoreState)=c.DictValue " +
                    " where a.MerchID=@merchId ";
                sql = sql + sqlWhere;
                var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Base_StoreInfo).Namespace);
                var base_StoreInfo = dbContext.Database.SqlQuery<Base_StoreInfoListModel>(sql, parameters).ToList();                

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, base_StoreInfo);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [Authorize(Roles = "MerchUser")]
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetUnderStores(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;

                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string merchId = userTokenKeyModel.LogId;
                
                //从缓存获取
                var base_StoreInfo = XCCloudStoreBusiness.StoreInfoList.Where(p => p.MerchID == merchId).ToList();          
                Dictionary<string, string> pStoreList = base_StoreInfo.Select(o => new { StoreID = o.StoreID, StoreName = o.StoreName }).Distinct()
                    .ToDictionary(d => d.StoreID, d => d.StoreName, StringComparer.OrdinalIgnoreCase);

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, pStoreList);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object UploadShopPhoto(Dictionary<string, object> dicParas)
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

                string picturePath = System.Configuration.ConfigurationManager.AppSettings["UploadImageUrl"].ToString() + "/XCCloud/Store/Shop/";
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
                dicStoreInfo.Add("ShopSignPhoto", picturePath + fileName);
                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, dicStoreInfo);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object UploadLicencePhoto(Dictionary<string, object> dicParas)
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

                string picturePath = System.Configuration.ConfigurationManager.AppSettings["UploadImageUrl"].ToString() + "/XCCloud/Store/Licence/";
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
                dicStoreInfo.Add("LicencePhoto", picturePath + fileName);
                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, dicStoreInfo);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [Authorize(Roles = "XcUser,XcAdmin")]
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetStoreUncheckList(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;

                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string userId = userTokenKeyModel.LogId;

                //返回该用户待审核门店信息
                string sql = " exec  SelectStoreUnchecked @UserID";
                SqlParameter[]  parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@UserID", userId);

                //返回功能菜单信息
                DataSet ds = XCCloudBLL.ExecuteQuerySentence(sql, parameters);
                if (ds.Tables.Count != 1)
                {
                    errMsg = "获取数据异常";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                var base_StoreInfo = Utils.GetModelList<Base_StoreInfoListModel>(ds.Tables[0]);                

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, base_StoreInfo);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [Authorize(Roles = "MerchUser")]
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object SaveStoreInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string merchId = userTokenKeyModel.LogId;               
                string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
                string storeState = dicParas.ContainsKey("storeState") ? dicParas["storeState"].ToString() : string.Empty;
                string parentId = dicParas.ContainsKey("parentId") ? dicParas["parentId"].ToString() : string.Empty;
                string storeName = dicParas.ContainsKey("storeName") ? dicParas["storeName"].ToString() : string.Empty;
                string password = dicParas.ContainsKey("password") ? dicParas["password"].ToString() : string.Empty;
                string authorExpireDate = dicParas.ContainsKey("authorExpireDate") ? dicParas["authorExpireDate"].ToString() : string.Empty;
                string address = dicParas.ContainsKey("address") ? dicParas["address"].ToString() : string.Empty;
                string lng = dicParas.ContainsKey("lng") ? dicParas["lng"].ToString() : string.Empty;
                string lat = dicParas.ContainsKey("lat") ? dicParas["lat"].ToString() : string.Empty;
                string contracts = dicParas.ContainsKey("contracts") ? dicParas["contracts"].ToString() : string.Empty;
                string idCard = dicParas.ContainsKey("idCard") ? dicParas["idCard"].ToString() : string.Empty;
                string idExpireDate = dicParas.ContainsKey("idExpireDate") ? dicParas["idExpireDate"].ToString() : string.Empty;
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;
                string shopSignPhoto = dicParas.ContainsKey("shopSignPhoto") ? dicParas["shopSignPhoto"].ToString() : string.Empty;
                string licencePhoto = dicParas.ContainsKey("licencePhoto") ? dicParas["licencePhoto"].ToString() : string.Empty;
                string licenceId = dicParas.ContainsKey("licenceId") ? dicParas["licenceId"].ToString() : string.Empty;
                string licenceExpireDate = dicParas.ContainsKey("licenceExpireDate") ? dicParas["licenceExpireDate"].ToString() : string.Empty;
                string bankType = dicParas.ContainsKey("bankType") ? dicParas["bankType"].ToString() : string.Empty;
                string bankCode = dicParas.ContainsKey("bankCode") ? dicParas["bankCode"].ToString() : string.Empty;
                string bankAccount = dicParas.ContainsKey("bankAccount") ? dicParas["bankAccount"].ToString() : string.Empty;
                string selttleType = dicParas.ContainsKey("selttleType") ? dicParas["selttleType"].ToString() : string.Empty;

                #region 验证参数
                
                if (string.IsNullOrEmpty(storeName))
                {
                    errMsg = "门头名称不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }
                
                if (string.IsNullOrEmpty(password))
                {
                    errMsg = "消费密码不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!string.IsNullOrEmpty(authorExpireDate) && !Utils.CheckDate(authorExpireDate))
                {
                    errMsg = "授权到期日期格式";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(address))
                {
                    errMsg = "商户地址不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(contracts))
                {
                    errMsg = "联系人不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(idCard))
                {
                    errMsg = "身份证号不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!string.IsNullOrEmpty(idExpireDate) && !Utils.CheckDate(idExpireDate))
                {
                    errMsg = "身份证到期日期格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(mobile))
                {
                    errMsg = "联系电话不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.CheckMobile(mobile))
                {
                    errMsg = "联系电话格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(licenceId))
                {
                    errMsg = "营业执照号不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!string.IsNullOrEmpty(licenceExpireDate) && !Utils.CheckDate(licenceExpireDate))
                {
                    errMsg = "营业执照到期日期格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(bankType))
                {
                    errMsg = "银行卡类别不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(bankCode))
                {
                    errMsg = "银行卡账号不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(bankAccount))
                {
                    errMsg = "银行卡户名不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(selttleType))
                {
                    errMsg = "结算类型不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(selttleType))
                {
                    errMsg = "结算类型格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!string.IsNullOrEmpty(storeState) && !Utils.IsNumeric(storeState))
                {
                    errMsg = "门店状态格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(lng) || string.IsNullOrEmpty(lat))
                {
                    errMsg = "门店经纬度不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.IsDecimal(lng) || !Utils.IsDecimal(lat))
                {
                    errMsg = "经纬度格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                //根据经纬度获取邮政编码
                var adcode = string.Empty;
                if (!getAdcode(lat, lng, out adcode, out errMsg))
                {
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                #endregion

                IBase_StoreInfoService base_StoreInfoService = BLLContainer.Resolve<IBase_StoreInfoService>();
                Base_StoreInfo base_StoreInfo = base_StoreInfoService.GetModels(p => p.StoreID.ToString().Equals(storeId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Base_StoreInfo>() ?? new Base_StoreInfo();
                base_StoreInfo.ParentID = parentId;
                base_StoreInfo.MerchID = merchId;
                base_StoreInfo.StoreName = storeName;
                base_StoreInfo.Password = password;
                base_StoreInfo.AuthorExpireDate = !string.IsNullOrEmpty(authorExpireDate) ? Convert.ToDateTime(authorExpireDate) : default(DateTime?);
                base_StoreInfo.AreaCode = adcode;
                base_StoreInfo.Address = address;
                base_StoreInfo.Lng = Convert.ToDecimal(lng);
                base_StoreInfo.Lat = Convert.ToDecimal(lat);
                base_StoreInfo.Contacts = contracts;
                base_StoreInfo.IDCard = idCard;
                base_StoreInfo.IDExpireDate = !string.IsNullOrEmpty(idExpireDate) ? Convert.ToDateTime(idExpireDate) : default(DateTime?);
                base_StoreInfo.Mobile = mobile;
                base_StoreInfo.ShopSignPhoto = shopSignPhoto;
                base_StoreInfo.LicencePhoto = licencePhoto;
                base_StoreInfo.LicenceID = licenceId;
                base_StoreInfo.LicenceExpireDate = !string.IsNullOrEmpty(licenceExpireDate) ? Convert.ToDateTime(licenceExpireDate) : default(DateTime?);
                base_StoreInfo.BankType = bankType;
                base_StoreInfo.BankCode = bankCode;
                base_StoreInfo.BankAccount = bankAccount;
                base_StoreInfo.SelttleType = Convert.ToInt32(selttleType);
                base_StoreInfo.StoreState = string.IsNullOrEmpty(storeState) ? (int)StoreState.Invalid : Convert.ToInt32(storeState);

                if (string.IsNullOrEmpty(base_StoreInfo.StoreID))
                {
                    //给商户的创建者发送门店审核工单
                    IBase_MerchantInfoService base_MerchantInfoService = BLLContainer.Resolve<IBase_MerchantInfoService>();
                    IBase_UserInfoService base_UserInfoService = BLLContainer.Resolve<IBase_UserInfoService>();
                    var base_MerchantInfo = base_MerchantInfoService.GetModels(p => p.MerchID.Equals(merchId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (!base_UserInfoService.Any(p => p.OpenID.Equals(base_MerchantInfo.WxOpenID, StringComparison.OrdinalIgnoreCase)))
                    {
                        errMsg = "指定OpenId的用户不存在";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    int senderId = base_UserInfoService.GetModels(p => p.OpenID.Equals(base_MerchantInfo.WxOpenID, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().UserID;
                    int authorId;
                    string authorOpenId;
                    if (base_MerchantInfo.CreateType == (int)CreateType.Agent) //如果门店的商户是代理商，就发送给代理商的创建者审核
                    {
                        authorId = Convert.ToInt32(base_MerchantInfoService.GetModels(p => p.MerchID.Equals(base_MerchantInfo.CreateUserID, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().CreateUserID);
                    }
                    else
                    {
                        authorId = Convert.ToInt32(base_MerchantInfo.CreateUserID);
                    }
                    authorOpenId = base_UserInfoService.GetModels(p => p.UserID == authorId).FirstOrDefault().OpenID;

                    IXC_WorkInfoService xC_WorkInfoService = BLLContainer.Resolve<IXC_WorkInfoService>();
                    //商户一次只能有一个待审核门店
                    if (xC_WorkInfoService.Any(p => p.SenderID == senderId && p.WorkType == (int)WorkType.StoreCheck && p.WorkState == (int)WorkState.Pending))
                    {
                        errMsg = "您还有门店尚未通过审核，请等待通过后新建门店";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    //开启EF事务
                    using (TransactionScope ts = new TransactionScope())
                    {
                        try
                        {                            
                            var xC_WorkInfo = new XC_WorkInfo();
                            xC_WorkInfo.WorkType = (int)WorkType.StoreCheck;
                            xC_WorkInfo.WorkState = (int)WorkState.Pending;
                            xC_WorkInfo.SenderID = senderId;
                            xC_WorkInfo.AuditorID = authorId;
                            xC_WorkInfo.SenderTime = DateTime.Now;
                            xC_WorkInfo.WorkBody = "莘宸商户门店审核";
                            if (!xC_WorkInfoService.Add(xC_WorkInfo))
                            {
                                errMsg = "添加门店审核工单信息失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            //创建门店信息
                            if (!genNo(merchId, adcode, out storeId, out errMsg))
                            {
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            base_StoreInfo.StoreID = storeId;
                            if (!base_StoreInfoService.Add(base_StoreInfo))
                            {
                                errMsg = "保存门店信息失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            #region 初始化连锁店余额通用设置
                            //初始化连锁店余额通用设置
                            var base_ChainRule_StoreContext = DbContextFactory.CreateByModelNamespace(typeof(Base_ChainRule_Store).Namespace);
                            if (!base_ChainRule_StoreContext.Set<Base_ChainRule>().Any(a => a.MerchID.Equals(merchId, StringComparison.OrdinalIgnoreCase)))
                            {
                                errMsg = "初始化连锁店余额通用设置异常，找不到商户分组信息";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }
                            if (base_ChainRule_StoreContext.Set<Base_ChainRule_Store>().Any(a => a.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase)))
                            {
                                errMsg = "初始化连锁店余额通用设置异常，该门店ID已存在";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }
                            var base_ChainRule = base_ChainRule_StoreContext.Set<Base_ChainRule>().Where(w => w.MerchID.Equals(merchId, StringComparison.OrdinalIgnoreCase)).ToList();
                            foreach (var model in base_ChainRule)
                            {
                                var base_ChainRule_Store = new Base_ChainRule_Store();
                                base_ChainRule_Store.RuleGroupID = model.RuleGroupID;
                                base_ChainRule_Store.StoreID = storeId;
                                base_ChainRule_StoreContext.Entry(base_ChainRule_Store).State = EntityState.Added;
                            }
                            if (base_ChainRule_StoreContext.SaveChanges() < 0)
                            {
                                errMsg = "初始化连锁店余额通用设置失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }
                            #endregion

                            #region 初始化运营参数

                            //初始化门店运营参数配置
                            var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Data_Parameters).Namespace);
                            var data_Parameter = dbContext.Set<Data_Parameters>().AsQueryable();
                            if (base_MerchantInfo.MerchTag == (int)MerchTag.Lottery)
                            {
                                data_Parameter = data_Parameter.Where(p => p.StoreID.Equals(MerchTag.Lottery.ToString()));
                            }
                            else
                            {
                                data_Parameter = data_Parameter.Where(p => p.StoreID.Equals(MerchTag.Game.ToString()));
                            }

                            foreach (var model in data_Parameter.ToList())
                            {
                                var data_ParameterModel = new Data_Parameters();
                                data_ParameterModel.StoreID = storeId;
                                data_ParameterModel.ParameterName = model.ParameterName;
                                data_ParameterModel.ParameterValue = model.ParameterValue;
                                data_ParameterModel.System = model.System;
                                data_ParameterModel.IsAllow = model.IsAllow;
                                data_ParameterModel.Note = model.Note;
                                dbContext.Entry(data_ParameterModel).State = EntityState.Added;
                            }

                            if (dbContext.SaveChanges() < 0)
                            {
                                errMsg = "初始化门店运营参数失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            //初始化返回规则设定
                            dbContext = DbContextFactory.CreateByModelNamespace(typeof(Data_GivebackRule).Namespace);
                            var data_GivebackRule = dbContext.Set<Data_GivebackRule>().AsQueryable();
                            if (base_MerchantInfo.MerchTag == (int)MerchTag.Lottery)
                            {
                                data_GivebackRule = data_GivebackRule.Where(p => p.StoreID.Equals(MerchTag.Lottery.ToString()));
                            }
                            else
                            {
                                data_GivebackRule = data_GivebackRule.Where(p => p.StoreID.Equals(MerchTag.Game.ToString()));
                            }

                            foreach (var model in data_GivebackRule.ToList())
                            {
                                var data_GivebackRuleModel = new Data_GivebackRule();
                                data_GivebackRuleModel.StoreID = storeId;
                                data_GivebackRuleModel.MemberLevelID = model.MemberLevelID;
                                data_GivebackRuleModel.BackMin = model.BackMin;
                                data_GivebackRuleModel.BackMax = model.BackMax;
                                data_GivebackRuleModel.BackScale = model.BackScale;
                                data_GivebackRuleModel.ExitCardMin = model.ExitCardMin;
                                data_GivebackRuleModel.AllowBackPrincipal = model.AllowBackPrincipal;
                                data_GivebackRuleModel.Backtype = model.Backtype;
                                data_GivebackRuleModel.TotalDays = model.TotalDays;
                                data_GivebackRuleModel.AllowContainToday = model.AllowContainToday;
                                dbContext.Entry(data_GivebackRuleModel).State = EntityState.Added;
                            }

                            if (dbContext.SaveChanges() < 0)
                            {
                                errMsg = "初始化门店返回规则失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            #endregion

                            #region 初始化套餐设定

                            //初始化套餐设定
                            IData_FoodInfoService data_FoodInfoService = BLLContainer.Resolve<IData_FoodInfoService>();
                            var data_FoodInfo = new Data_FoodInfo();
                            data_FoodInfo.FoodName = "入会";
                            data_FoodInfo.FoodType = (int)FoodType.Member;
                            data_FoodInfo.FoodState = 1;
                            data_FoodInfo.AllowPrint = 1;
                            data_FoodInfo.ForeAuthorize = 1;
                            data_FoodInfo.StoreID = storeId;
                            if (!data_FoodInfoService.Add(data_FoodInfo))
                            {
                                errMsg = "添加入会套餐信息失败";
                                return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                            }

                            #endregion                            

                            ts.Complete();

                            //已提交工单，等待管理员审核
                            MessagePush(authorOpenId, base_MerchantInfo.MerchAccount, xC_WorkInfo.SenderTime.Value.ToString("f"), storeName, xC_WorkInfo.WorkID.ToString());
                        }
                        catch (Exception ex)
                        {
                            return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, ex.Message);
                        }
                    }                    
                }
                else
                {
                    if (!base_StoreInfoService.Update(base_StoreInfo))
                    {
                        errMsg = "保存门店信息失败";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                }                

                //更新缓存
                XCCloudStoreBusiness.Init();

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [Authorize(Roles = "MerchUser")]
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object EditStoreInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string merchId = userTokenKeyModel.LogId;
                string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
                string storeState = dicParas.ContainsKey("storeState") ? dicParas["storeState"].ToString() : string.Empty;
                string parentId = dicParas.ContainsKey("parentId") ? dicParas["parentId"].ToString() : string.Empty;
                string storeName = dicParas.ContainsKey("storeName") ? dicParas["storeName"].ToString() : string.Empty;
                string password = dicParas.ContainsKey("password") ? dicParas["password"].ToString() : string.Empty;
                string authorExpireDate = dicParas.ContainsKey("authorExpireDate") ? dicParas["authorExpireDate"].ToString() : string.Empty;
                string address = dicParas.ContainsKey("address") ? dicParas["address"].ToString() : string.Empty;
                string contracts = dicParas.ContainsKey("contracts") ? dicParas["contracts"].ToString() : string.Empty;                
                string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;                
                string selttleType = dicParas.ContainsKey("selttleType") ? dicParas["selttleType"].ToString() : string.Empty;

                #region 验证参数
                
                if (string.IsNullOrEmpty(storeName))
                {
                    errMsg = "门头名称不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(password))
                {
                    errMsg = "消费密码不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!string.IsNullOrEmpty(authorExpireDate) && !Utils.CheckDate(authorExpireDate))
                {
                    errMsg = "授权到期日期格式";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(address))
                {
                    errMsg = "商户地址不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(contracts))
                {
                    errMsg = "联系人不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }                

                if (string.IsNullOrEmpty(mobile))
                {
                    errMsg = "联系电话不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.CheckMobile(mobile))
                {
                    errMsg = "联系电话格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }                

                if (string.IsNullOrEmpty(selttleType))
                {
                    errMsg = "结算类型不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(selttleType))
                {
                    errMsg = "结算类型格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!string.IsNullOrEmpty(storeState) && !Utils.IsNumeric(storeState))
                {
                    errMsg = "门店状态格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }
                
                #endregion

                IBase_StoreInfoService base_StoreInfoService = BLLContainer.Resolve<IBase_StoreInfoService>();
                Base_StoreInfo base_StoreInfo = base_StoreInfoService.GetModels(p => p.StoreID.ToString().Equals(storeId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (base_StoreInfo == null)
                {
                    errMsg = "门店信息不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                base_StoreInfo.ParentID = parentId;
                base_StoreInfo.MerchID = merchId;
                base_StoreInfo.StoreName = storeName;
                base_StoreInfo.Password = password;
                base_StoreInfo.AuthorExpireDate = !string.IsNullOrEmpty(authorExpireDate) ? Convert.ToDateTime(authorExpireDate) : default(DateTime?);
                base_StoreInfo.Address = address;
                base_StoreInfo.Contacts = contracts;
                base_StoreInfo.Mobile = mobile;
                base_StoreInfo.SelttleType = Convert.ToInt32(selttleType);
                base_StoreInfo.StoreState = string.IsNullOrEmpty(storeState) ? (int)StoreState.Invalid : Convert.ToInt32(storeState);

                if (!base_StoreInfoService.Update(base_StoreInfo))
                {
                    errMsg = "修改门店信息失败";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                //更新缓存
                XCCloudStoreBusiness.Init();

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [Authorize(Roles = "XcUser,XcAdmin,MerchUser")]
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object GetStoreInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string logId = userTokenKeyModel.LogId;
                string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
                
                if (string.IsNullOrEmpty(storeId))
                {
                    errMsg = "门店编号不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }
                
                SqlParameter[] parameters = new SqlParameter[1];
                parameters[0] = new SqlParameter("@storeId", storeId);
                string sqlWhere = string.Empty + " and a.StoreID=@storeId";
                string sql = @"select a.*, b.DictKey as BankTypeStr, c.DictKey as SelttleTypeStr from Base_StoreInfo a " +
                    " left join (select b.* from Dict_System a inner join Dict_System b on a.ID=b.PID where a.DictKey='银行类别' and a.PID=0) b on convert(varchar, a.BankType)=b.DictValue " +
                    " left join (select b.* from Dict_System a inner join Dict_System b on a.ID=b.PID where a.DictKey='结算类型' and a.PID=0) c on convert(varchar, a.SelttleType)=c.DictValue where 1=1 ";
                sql = sql + sqlWhere;
                var dbContext = DbContextFactory.CreateByModelNamespace(typeof(Base_StoreInfo).Namespace);
                var base_StoreInfoModel = dbContext.Database.SqlQuery<StoreInfoModel>(sql, parameters).FirstOrDefault();
                if (base_StoreInfoModel == null)
                {
                    errMsg = "门店信息不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (base_StoreInfoModel.SettleID != null)
                {
                    int settleId = base_StoreInfoModel.SettleID.GetValueOrDefault();
                    if (base_StoreInfoModel.SelttleType == (int)SettleType.Org)
                    {
                        IBase_SettleOrgService base_SettleOrgService = BLLContainer.Resolve<IBase_SettleOrgService>();
                        Base_SettleOrg base_SettleOrg = base_SettleOrgService.GetModels(p => p.ID == settleId).FirstOrDefault();
                        if (base_SettleOrg == null)
                        {
                            errMsg = "结算信息不存在";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }
                        base_StoreInfoModel.SettleFields = new Dictionary<string, object>(base_SettleOrg.AsDictionary(), StringComparer.OrdinalIgnoreCase);
                    }
                    else if (base_StoreInfoModel.SelttleType == (int)SettleType.PPOS)
                    {
                        IBase_SettlePPOSService base_SettlePPOSService = BLLContainer.Resolve<IBase_SettlePPOSService>();
                        Base_SettlePPOS base_SettlePPOS = base_SettlePPOSService.GetModels(p => p.ID == settleId).FirstOrDefault();
                        if (base_SettlePPOS == null)
                        {
                            errMsg = "结算信息不存在";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }
                        base_StoreInfoModel.SettleFields = new Dictionary<string, object>(base_SettlePPOS.AsDictionary(), StringComparer.OrdinalIgnoreCase);
                    }
                }

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, base_StoreInfoModel);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [Authorize(Roles = "XcUser,XcAdmin")]
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object CheckStoreInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                XCCloudUserTokenModel userTokenKeyModel = (XCCloudUserTokenModel)dicParas[Constant.XCCloudUserTokenModel];
                string userId = userTokenKeyModel.LogId;
                string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
                string workState = dicParas.ContainsKey("workState") ? dicParas["workState"].ToString() : string.Empty;
                string remark = dicParas.ContainsKey("remark") ? dicParas["remark"].ToString() : string.Empty;
                string selttleType = dicParas.ContainsKey("selttleType") ? dicParas["selttleType"].ToString() : string.Empty;
                Dictionary<string, object> settleFields = dicParas.ContainsKey("settleFields") ? new Dictionary<string, object>((IDictionary<string, object>)dicParas["settleFields"], StringComparer.OrdinalIgnoreCase) : null;
                string settleId = dicParas.ContainsKey("settleId") ? dicParas["settleId"].ToString() : string.Empty;
                
                #region 验证参数
                if (string.IsNullOrEmpty(userId))
                {
                    errMsg = "当前用户无权访问";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(storeId))
                {
                    errMsg = "门店编号不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (string.IsNullOrEmpty(workState))
                {
                    errMsg = "审核状态不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (!Utils.isNumber(workState))
                {
                    errMsg = "审核状态格式不正确";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                if (Convert.ToInt32(workState) == (int)WorkState.Pass)
                {
                    if (string.IsNullOrEmpty(selttleType))
                    {
                        errMsg = "结算类型不能为空";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }

                    if (!Utils.isNumber(selttleType))
                    {
                        errMsg = "结算类型格式不正确";
                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                    }
                }
                
                
                #endregion

                //开启EF事务
                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        IBase_StoreInfoService base_StoreInfoService = BLLContainer.Resolve<IBase_StoreInfoService>();
                        var base_StoreInfoModel = base_StoreInfoService.GetModels(p => p.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Base_StoreInfo>();
                        if (base_StoreInfoModel == null)
                        {
                            errMsg = "门店信息不存在";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        //结算设置
                        if (Convert.ToInt32(workState) == (int)WorkState.Pass)
                        {
                            base_StoreInfoModel.StoreState = (int)StoreState.Valid;

                            if (settleFields != null && settleFields.Count > 0)
                            {
                                if (Convert.ToInt32(selttleType) == (int)SettleType.Org)
                                {
                                    IBase_SettleOrgService base_SettleOrgService = BLLContainer.Resolve<IBase_SettleOrgService>();
                                    Base_SettleOrg base_SettleOrg = base_SettleOrgService.GetModels(p => p.ID.ToString().Equals(settleId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Base_SettleOrg>() ?? new Base_SettleOrg();
                                    try
                                    {
                                        base_SettleOrg.SettleCount = settleFields.ContainsKey("SettleCount") ? Convert.ToInt32(settleFields["SettleCount"]) : default(int?);
                                    }
                                    catch
                                    {
                                        errMsg = "结算次数格式不正确";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    try
                                    {
                                        base_SettleOrg.SettleCycle = settleFields.ContainsKey("SettleCycle") ? Convert.ToInt32(settleFields["SettleCycle"]) : default(int?);
                                    }
                                    catch
                                    {
                                        errMsg = "结算周期格式不正确";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    try
                                    {
                                        base_SettleOrg.SettleFee = settleFields.ContainsKey("SettleFee") ? Convert.ToDecimal(settleFields["SettleFee"]) : default(decimal?);
                                    }
                                    catch
                                    {
                                        errMsg = "结算费率格式不正确";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    base_SettleOrg.WXPayOpenID = settleFields.ContainsKey("WXPayOpenID") ? (settleFields["WXPayOpenID"] + "") : null;
                                    if (string.IsNullOrEmpty(base_SettleOrg.WXPayOpenID))
                                    {
                                        errMsg = "微信OPENID不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    base_SettleOrg.WXName = settleFields.ContainsKey("WXName") ? (settleFields["WXName"] + "") : null;
                                    if (string.IsNullOrEmpty(base_SettleOrg.WXName))
                                    {
                                        errMsg = "微信实名不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    base_SettleOrg.AliPay = settleFields.ContainsKey("AliPay") ? (settleFields["AliPay"] + "") : null;
                                    if (string.IsNullOrEmpty(base_SettleOrg.AliPay))
                                    {
                                        errMsg = "支付宝账号不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    base_SettleOrg.AliPayName = settleFields.ContainsKey("AliPayName") ? (settleFields["AliPayName"] + "") : null;
                                    if (string.IsNullOrEmpty(base_SettleOrg.AliPayName))
                                    {
                                        errMsg = "支付宝实名不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    if (base_SettleOrg.ID <= 0)
                                    {
                                        if (!base_SettleOrgService.Add(base_SettleOrg))
                                        {
                                            errMsg = "更新数据库失败";
                                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                        }
                                        settleId = base_SettleOrg.ID.ToString();
                                    }
                                    else
                                    {
                                        if (!base_SettleOrgService.Update(base_SettleOrg))
                                        {
                                            errMsg = "更新数据库失败";
                                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                        }
                                    }
                                }
                                else if (Convert.ToInt32(selttleType) == (int)SettleType.PPOS)
                                {
                                    IBase_SettlePPOSService base_SettlePPOSService = BLLContainer.Resolve<IBase_SettlePPOSService>();
                                    Base_SettlePPOS base_SettlePPOS = base_SettlePPOSService.GetModels(p => p.ID.ToString().Equals(settleId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Base_SettlePPOS>() ?? new Base_SettlePPOS();
                                    base_SettlePPOS.MerchNo = settleFields.ContainsKey("MerchNo") ? (settleFields["MerchNo"] + "") : null;
                                    if (string.IsNullOrEmpty(base_SettlePPOS.MerchNo))
                                    {
                                        errMsg = "商户号不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    base_SettlePPOS.TerminalNo = settleFields.ContainsKey("TerminalNo") ? (settleFields["TerminalNo"] + "") : null;
                                    if (string.IsNullOrEmpty(base_SettlePPOS.TerminalNo))
                                    {
                                        errMsg = "终端号不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    base_SettlePPOS.Token = settleFields.ContainsKey("Token") ? (settleFields["Token"] + "") : null;
                                    if (string.IsNullOrEmpty(base_SettlePPOS.Token))
                                    {
                                        errMsg = "令牌不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    base_SettlePPOS.InstNo = settleFields.ContainsKey("InstNo") ? (settleFields["InstNo"] + "") : null;
                                    if (string.IsNullOrEmpty(base_SettlePPOS.InstNo))
                                    {
                                        errMsg = "机构号不能为空";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    try
                                    {
                                        base_SettlePPOS.SettleFee = settleFields.ContainsKey("SettleFee") ? Convert.ToDecimal(settleFields["SettleFee"]) : default(decimal?);
                                    }
                                    catch
                                    {
                                        errMsg = "手续费格式不正确";
                                        return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                    }

                                    if (base_SettlePPOS.ID <= 0)
                                    {
                                        if (!base_SettlePPOSService.Add(base_SettlePPOS))
                                        {
                                            errMsg = "更新数据库失败";
                                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                        }
                                        settleId = base_SettlePPOS.ID.ToString();
                                    }
                                    else
                                    {
                                        if (!base_SettlePPOSService.Update(base_SettlePPOS))
                                        {
                                            errMsg = "更新数据库失败";
                                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                        }
                                    }
                                }

                                if (string.IsNullOrEmpty(settleId))
                                {
                                    errMsg = "结算分类索引不能为空";
                                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                                }
                            } 
                        }                                                                       

                        base_StoreInfoModel.SettleID = !string.IsNullOrEmpty(settleId) ? Convert.ToInt32(settleId) : default(int?);
                        if (!base_StoreInfoService.Update(base_StoreInfoModel))
                        {
                            errMsg = "审核门店失败";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        //修改工单
                        IBase_MerchantInfoService base_MerchantInfoService = BLLContainer.Resolve<IBase_MerchantInfoService>();
                        IBase_UserInfoService base_UserInfoService = BLLContainer.Resolve<IBase_UserInfoService>();
                        var base_MerchantInfo = base_MerchantInfoService.GetModels(p => p.MerchID.Equals(base_StoreInfoModel.MerchID, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        if (!base_UserInfoService.Any(p => p.OpenID.Equals(base_MerchantInfo.WxOpenID, StringComparison.OrdinalIgnoreCase)))
                        {
                            errMsg = "指定OpenId的用户不存在";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        int senderId = base_UserInfoService.GetModels(p => p.OpenID.Equals(base_MerchantInfo.WxOpenID, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().UserID;

                        IXC_WorkInfoService xC_WorkInfoService = BLLContainer.Resolve<IXC_WorkInfoService>();
                        var xC_WorkInfo = xC_WorkInfoService.GetModels(p => p.WorkType == (int)WorkType.StoreCheck && p.WorkState == (int)WorkState.Pending && p.SenderID == senderId).FirstOrDefault();
                        if (xC_WorkInfo == null)
                        {
                            errMsg = "该门店审核工单不存在";
                            return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                        }

                        xC_WorkInfo.AuditorID = Convert.ToInt32(userId);
                        xC_WorkInfo.WorkState = Convert.ToInt32(workState);
                        xC_WorkInfo.AuditBody = remark;
                        xC_WorkInfo.AuditTime = DateTime.Now;
                        if (!xC_WorkInfoService.Update(xC_WorkInfo))
                        {
                            errMsg = "修改工单失败";
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

        [Authorize(Roles = "MerchUser")]
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object DeleteStoreInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;                
                
                if (string.IsNullOrEmpty(storeId))
                {
                    errMsg = "门店编号不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                IBase_StoreInfoService base_StoreInfoService = BLLContainer.Resolve<IBase_StoreInfoService>();
                var base_StoreInfoModel = base_StoreInfoService.GetModels(p => p.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (base_StoreInfoModel == null)
                {
                    errMsg = "门店信息不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                base_StoreInfoModel.StoreState = (int)StoreState.Cancel;
                if (!base_StoreInfoService.Update(base_StoreInfoModel))
                {
                    errMsg = "删除门店信息失败";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }                

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [Authorize(Roles = "MerchUser")]
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object OpenStoreInfo(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;

                if (string.IsNullOrEmpty(storeId))
                {
                    errMsg = "门店编号不能为空";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                IBase_StoreInfoService base_StoreInfoService = BLLContainer.Resolve<IBase_StoreInfoService>();
                var base_StoreInfoModel = base_StoreInfoService.GetModels(p => p.StoreID.Equals(storeId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (base_StoreInfoModel == null)
                {
                    errMsg = "门店信息不存在";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                base_StoreInfoModel.StoreState = (int)StoreState.Open;//开业
                if (!base_StoreInfoService.Update(base_StoreInfoModel))
                {
                    errMsg = "校验门店信息失败";
                    return ResponseModelFactory.CreateFailModel(isSignKeyReturn, errMsg);
                }

                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }

        [Authorize(Roles = "StoreUser")]
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCCloudUserCacheToken, SysIdAndVersionNo = false)]
        public object getStorePassword(Dictionary<string, object> dicParas)
        {
            string storePassword = string.Empty;
            string storeName = string.Empty;
            XCCloudUserTokenModel userTokenModel = (XCCloudUserTokenModel)(dicParas[Constant.XCCloudUserTokenModel]);
            
            StoreIDDataModel dataModel = (StoreIDDataModel)(userTokenModel.DataModel);

            if (XCCloudStoreBusiness.IsEffectiveStore(dataModel.StoreId, out storeName, out storePassword))
            {
                var obj = new {
                    storePassword = storePassword
                };
                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn,obj);
            }
            else
            {
                return ResponseModelFactory.CreateAnonymousFailModel(isSignKeyReturn, "门店信息不存在");
            }     
        }
    }
}