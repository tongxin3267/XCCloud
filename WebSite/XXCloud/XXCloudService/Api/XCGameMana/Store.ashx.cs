using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.Business;
using XCCloudService.Business.Common;
using XCCloudService.Business.XCGame;
using XCCloudService.Business.XCGameMana;
using XCCloudService.CacheService;
using XCCloudService.CacheService.XCGameMana;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCGameManager;
using XCCloudService.Model.XCGameManager;

namespace XCCloudService.Api.XCGameMana
{
    /// <summary>
    /// Token 的摘要说明
    /// </summary>
    public class Store : ApiBase
    {
        /// <summary>
        /// 终端号获取设备信息
        /// </summary>
        /// <param name="dicParas"></param>
        /// <returns></returns>
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object storeInfoByTerminalNo(Dictionary<string, object> dicParas)
        {
            try
            {
                string terminalNo = dicParas.ContainsKey("deviceToken") ? dicParas["deviceToken"].ToString() : string.Empty;
                IDeviceService deviceService = BLLContainer.Resolve<IDeviceService>();
                var deviceModel = deviceService.GetModels(p => p.TerminalNo.Equals(terminalNo, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<t_device>();
                if (deviceModel == null)
                {
                    NoSignKeyResponseModel responseModel = new NoSignKeyResponseModel(Return_Code.T, "", Result_Code.F, "终端号不存在");
                    return responseModel;
                }
                else
                {
                    IStoreService storeService = BLLContainer.Resolve<IStoreService>();
                    var storeModel = storeService.GetModels(p => p.id.ToString().Equals(deviceModel.StoreId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<t_store>();
                    if (storeModel == null)
                    {
                        NoSignKeyResponseModel responseModel = new NoSignKeyResponseModel(Return_Code.T, "", Result_Code.F, "终端号对应的门店不存在");
                        return responseModel;
                    }
                    else
                    {
                        //获取设备缓存状态
                        string state = "0";
                        string stateName = string.Empty;
                        if (DeviceStateBusiness.ExistDeviceState(deviceModel.StoreId, deviceModel.DeviceId))
                        {
                            state = DeviceStateBusiness.GetDeviceState(deviceModel.StoreId, deviceModel.DeviceId);
                        }
                        stateName = DeviceStateBusiness.GetStateName(state);
                        TerminalStoreReponseModel model = new TerminalStoreReponseModel(storeModel.id.ToString(), storeModel.companyname, deviceModel.DeviceName, deviceModel.DeviceType, state, stateName);
                        NoSignKeyResponseModel<TerminalStoreReponseModel> responseModel = new NoSignKeyResponseModel<TerminalStoreReponseModel>(model);
                        return responseModel;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken)]
        public object getstorepassword(Dictionary<string, object> dicParas)
        {
            string storeId=  dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
            string Dogid = dicParas.ContainsKey("dogid") ? dicParas["dogid"].ToString() : string.Empty;
            StoreBusiness store = new StoreBusiness();
            string xcGameDBName = string.Empty;
            string password = string.Empty;
            string errMsg = string.Empty;
            if (!store.IsExistDog(storeId, Dogid))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "加密不合法");
            }
            if (storeId == "")
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "店号不能为空");
            }

            if (!store.IsEffectiveStore(storeId, out xcGameDBName, out password, out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
            }            
            storeId = storeId.PadRight(8, '0');
            string pass = Utils.EncryptDES(password, storeId);
            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, pass);


        }

        public object BindStoreOpenId(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
            string openId = dicParas.ContainsKey("openId") ? dicParas["openId"].ToString() : string.Empty;
            string smsCode = dicParas.ContainsKey("smsCode") ? dicParas["smsCode"].ToString() : string.Empty;
            string mobile = dicParas.ContainsKey("mobile") ? dicParas["mobile"].ToString() : string.Empty;

            string key = mobile + "_" + smsCode;
            if (!FilterMobileBusiness.IsTestSMS)
            {
                if (!SMSCodeCache.IsExist(key))
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "短信验证码无效");
                }
            }

            //验证门店信息
            //StoreCacheModel storeModel = null;
            //StoreBusiness store = new StoreBusiness();
            //if (!store.IsEffectiveStore(storeId, ref storeModel, out errMsg))
            //{
            //    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
            //}

            IStoreService storeService = BLLContainer.Resolve<IStoreService>();
            var storeModel = storeService.GetModels(p => p.id.ToString().Equals(storeId)).FirstOrDefault<t_store>();
            if (storeModel == null)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店信息不存在");
            }

            if (!string.IsNullOrEmpty(storeModel.openId))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店已绑定微信，不能重复绑定");
            }

            if (!storeService.Update(storeModel))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店已绑定微信出错");
            }

            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
        }

        private bool CheckAddStore(Dictionary<string, object> dicParas, out string errMsg)
        {
            errMsg = string.Empty;
            string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
            string companyname = dicParas.ContainsKey("companyname") ? dicParas["companyname"].ToString() : string.Empty;
            string province = dicParas.ContainsKey("province") ? dicParas["province"].ToString() : string.Empty;
            string address = dicParas.ContainsKey("address") ? dicParas["address"].ToString() : string.Empty;
            string boss = dicParas.ContainsKey("boss") ? dicParas["boss"].ToString() : string.Empty;
            string phone = dicParas.ContainsKey("phone") ? dicParas["phone"].ToString() : string.Empty;
            string telphone = dicParas.ContainsKey("telphone") ? dicParas["telphone"].ToString() : string.Empty;
            string clienttype = dicParas.ContainsKey("clienttype") ? dicParas["clienttype"].ToString() : string.Empty;
            string power_due_date = dicParas.ContainsKey("power_due_date") ? dicParas["power_due_date"].ToString() : string.Empty;
            string note = dicParas.ContainsKey("note") ? dicParas["note"].ToString() : string.Empty;
            string parentid = dicParas.ContainsKey("parentid") ? dicParas["parentid"].ToString() : string.Empty;
            string developer = dicParas.ContainsKey("developer") ? dicParas["developer"].ToString() : string.Empty;
            string store_password = dicParas.ContainsKey("store_password") ? dicParas["store_password"].ToString() : string.Empty;
            string wxfee = dicParas.ContainsKey("wxfee") ? dicParas["wxfee"].ToString() : string.Empty;

            if (string.IsNullOrEmpty(storeId))
            {
                errMsg = "店Id不能为空";
                return false;
            }

            if (!Utils.isNumber(storeId))
            {
                errMsg = "店Id只能使用数字";
                return false;
            }

            if (string.IsNullOrEmpty(companyname))
            {
                errMsg = "店名称不能为空";
                return false;
            }

            if (string.IsNullOrEmpty(address))
            {
                errMsg = "地址不能为空";
                return false;
            }

            if (string.IsNullOrEmpty(store_password))
            {
                errMsg = "店密码不能为空";
                return false;
            }

            if (string.IsNullOrEmpty(wxfee))
            {
                errMsg = "费率不能为空";
                return false;
            }

            if (!Utils.isNumber(wxfee) || int.Parse(wxfee) > 100)
            {
                errMsg = "费率无效";
                return false;
            }

            if (phone.Length != 11)
            {
                errMsg = "手机号码无效";
                return false;
            }

            //if (!client_level.Equals("普通级", StringComparison.OrdinalIgnoreCase))
            //{
            //    errMsg = "门店级别无效";
            //    return false;
            //}

            if (!clienttype.Equals("代理商", StringComparison.OrdinalIgnoreCase) && !clienttype.Equals("终端客户", StringComparison.OrdinalIgnoreCase))
            {
                errMsg = "门店类别无效";
                return false;
            }

            if (!string.IsNullOrEmpty(parentid))
            {
                string[] parentIdArr = parentid.Split(',');
                for (int i = 0; i < parentIdArr.Length; i++)
                {
                    if (string.IsNullOrEmpty(parentIdArr[i]))
                    {
                        errMsg = "门店类别无效";
                        return false;
                    }

                    StoreBusiness storeBusiness = new StoreBusiness();
                    if (!storeBusiness.IsEffectiveStore(parentIdArr[i], out errMsg))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameAdminToken)]
        public object addStore(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
            string companyname = dicParas.ContainsKey("companyname") ? dicParas["companyname"].ToString() : string.Empty;
            string province = dicParas.ContainsKey("province") ? dicParas["province"].ToString() : string.Empty;
            string address = dicParas.ContainsKey("address") ? dicParas["address"].ToString() : string.Empty;
            string boss = dicParas.ContainsKey("boss") ? dicParas["boss"].ToString() : string.Empty;
            string phone = dicParas.ContainsKey("phone") ? dicParas["phone"].ToString() : string.Empty;
            string telphone = dicParas.ContainsKey("telphone") ? dicParas["telphone"].ToString() : string.Empty;
            string clienttype = dicParas.ContainsKey("clienttype") ? dicParas["clienttype"].ToString() : string.Empty;
            string power_due_date = dicParas.ContainsKey("power_due_date") ? dicParas["power_due_date"].ToString() : string.Empty;
            string note = dicParas.ContainsKey("note") ? dicParas["note"].ToString() : string.Empty;
            string parentid = dicParas.ContainsKey("parentid") ? dicParas["parentid"].ToString() : string.Empty;
            string developer = dicParas.ContainsKey("developer") ? dicParas["developer"].ToString() : string.Empty;
            string store_password = dicParas.ContainsKey("store_password") ? dicParas["store_password"].ToString() : string.Empty;
            string wxfee = dicParas.ContainsKey("wxfee") ? dicParas["wxfee"].ToString() : string.Empty;

            if (!CheckAddStore(dicParas,out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
            }

            IStoreService storeService = BLLContainer.Resolve<IStoreService>();
            var storeModel = storeService.GetModels(p => p.id.ToString().Equals(storeId)).FirstOrDefault<t_store>();
            if (storeModel != null)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店Id已存在");
            }

            string store_dbname = string.Empty;
            t_store store = new t_store();
            store.id = int.Parse(storeId);
            store.companyname = companyname;
            store.province = province;
            store.address = address;
            store.boss = boss;
            store.phone = phone;
            store.telphone = telphone;
            store.client_level = "普通级";
            store.clienttype = clienttype;
            store.createtime = System.DateTime.Now;
            store.updatetime = System.DateTime.Now;
            store.power_due_date = System.DateTime.Parse(System.DateTime.Parse(power_due_date).ToString("yyyy-MM-dd") + " 00:00:00");
            store.note = note;
            store.parentid = parentid;
            store.developer = developer;
            store.store_password = store_password;
            store.store_dbname = "xcgamedb" + storeId;
            store.wxfee = Convert.ToDecimal(decimal.Parse(wxfee) / 1000);
            store.StoreType = 0;
            store.State = 1;
            store.Token = "";

            if (storeService.Add(store))
            {
                List<StoreCacheModel> list = StoreCache.GetStore();
                StoreCacheModel model = new StoreCacheModel();
                model.StoreID = int.Parse(storeId);
                model.StoreName = companyname;
                model.StorePassword = store_password;
                model.StoreType = Convert.ToInt32(store.StoreType);
                model.StoreDBName = store_dbname;
                list.Add(model);
            }

            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameAdminToken)]
        public object removeStore(Dictionary<string, object> dicParas)
        {
            try
            {
                string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
                IStoreService storeService = BLLContainer.Resolve<IStoreService>();
                var storeModel = storeService.GetModels(p => p.id.ToString().Equals(storeId)).FirstOrDefault<t_store>();
                if (storeModel == null)
                {
                    return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店不存在");
                }

                if (storeService.Delete(storeModel))
                {
                    List<StoreCacheModel> list = StoreCache.GetStore();
                    StoreCacheModel storeCacheModel = list.Where<StoreCacheModel>(p => p.StoreID == int.Parse(storeId)).FirstOrDefault<StoreCacheModel>();
                    if (storeCacheModel != null)
                    { 
                        list.Remove(storeCacheModel);
                    }    
                }

                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameAdminToken)]
        public object updateStore(Dictionary<string, object> dicParas)
        {
            string errMsg = string.Empty;
            string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
            string companyname = dicParas.ContainsKey("companyname") ? dicParas["companyname"].ToString() : string.Empty;
            string province = dicParas.ContainsKey("province") ? dicParas["province"].ToString() : string.Empty;
            string address = dicParas.ContainsKey("address") ? dicParas["address"].ToString() : string.Empty;
            string boss = dicParas.ContainsKey("boss") ? dicParas["boss"].ToString() : string.Empty;
            string phone = dicParas.ContainsKey("phone") ? dicParas["phone"].ToString() : string.Empty;
            string telphone = dicParas.ContainsKey("telphone") ? dicParas["telphone"].ToString() : string.Empty;
            string clienttype = dicParas.ContainsKey("clienttype") ? dicParas["clienttype"].ToString() : string.Empty;
            string power_due_date = dicParas.ContainsKey("power_due_date") ? dicParas["power_due_date"].ToString() : string.Empty;
            string note = dicParas.ContainsKey("note") ? dicParas["note"].ToString() : string.Empty;
            string parentid = dicParas.ContainsKey("parentid") ? dicParas["parentid"].ToString() : string.Empty;
            string developer = dicParas.ContainsKey("developer") ? dicParas["developer"].ToString() : string.Empty;
            string store_password = dicParas.ContainsKey("store_password") ? dicParas["store_password"].ToString() : string.Empty;
            string wxfee = dicParas.ContainsKey("wxfee") ? dicParas["wxfee"].ToString() : string.Empty;

            if (!CheckAddStore(dicParas, out errMsg))
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, errMsg);
            }

            IStoreService storeService = BLLContainer.Resolve<IStoreService>();
            var store = storeService.GetModels(p => p.id.ToString().Equals(storeId)).FirstOrDefault<t_store>();
            if (store == null)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店不存在");
            }

            string store_dbname = string.Empty;
            store.companyname = companyname;
            store.province = province;
            store.address = address;
            store.boss = boss;
            store.phone = phone;
            store.telphone = telphone;
            store.clienttype = clienttype;
            store.createtime = System.DateTime.Now;
            store.updatetime = System.DateTime.Now;
            store.power_due_date = System.DateTime.Parse(System.DateTime.Parse(power_due_date).ToString("yyyy-MM-dd") + " 00:00:00");
            store.note = note;
            store.parentid = parentid;
            store.developer = developer;
            store.store_password = store_password;
            store.store_dbname = "xcgamedb" + storeId;
            store.wxfee = Convert.ToDecimal(decimal.Parse(wxfee)/1000);

            if (storeService.Update(store))
            {
                List<StoreCacheModel> list = StoreCache.GetStore();
                StoreCacheModel storeCacheModel = list.Where<StoreCacheModel>(p => p.StoreID == int.Parse(storeId)).FirstOrDefault<StoreCacheModel>();
                if (storeCacheModel == null)
                {
                    StoreCacheModel model = new StoreCacheModel();
                    model.StoreID = int.Parse(storeId);
                    model.StoreName = companyname;
                    model.StorePassword = store_password;
                    model.StoreType = Convert.ToInt32(store.StoreType);
                    model.StoreDBName = store.store_dbname;
                    list.Add(model);
                }
                else
                {
                    storeCacheModel.StoreName = companyname;
                    storeCacheModel.StorePassword = store_password;
                    storeCacheModel.StoreType = Convert.ToInt32(store.StoreType);
                    storeCacheModel.StoreDBName = store.store_dbname;        
                }
            }

            return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.T, "");
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameAdminToken)]
        public object getStore(Dictionary<string, object> dicParas)
        {
            string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
            IStoreService storeService = BLLContainer.Resolve<IStoreService>();
            var store = storeService.GetModels(p => p.id.ToString().Equals(storeId)).FirstOrDefault<t_store>();
            if (store == null)
            {
                return ResponseModelFactory.CreateModel(isSignKeyReturn, Return_Code.T, "", Result_Code.F, "门店不存在");
            }
            else
            {
                var obj = new {
                    id = store.id,
                    companyname = store.companyname,
                    province = store.province,
                    address = store.address,
                    boss = store.boss,
                    phone = store.phone,
                    telphone = store.telphone,
                    client_level = store.client_level,
                    createtime = Convert.ToDateTime(store.createtime).ToString("yyyy-MM-dd HH:mm:ss"),
                    updatetime = Convert.ToDateTime(store.updatetime).ToString("yyyy-MM-dd HH:mm:ss"),
                    power_due_date = Convert.ToDateTime(store.power_due_date).ToString("yyyy-MM-dd"),
                    note = store.note,
                    parentid = store.parentid,
                    developer = store.developer,
                    store_password = store.store_password,
                    store_dbname = store.store_dbname,
                    wxfee = store.wxfee * 1000           
                };
                return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, obj);
            }
        }

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.XCGameAdminToken)]
        public object getStoreList(Dictionary<string, object> dicParas)
        {
            List<object> objList = new List<object>();
            IStoreService storeService = BLLContainer.Resolve<IStoreService>();

            string storeId = dicParas.ContainsKey("storeId") ? dicParas["storeId"].ToString() : string.Empty;
            string companyname = dicParas.ContainsKey("companyname") ? dicParas["companyname"].ToString() : string.Empty;
            string phone = dicParas.ContainsKey("phone") ? dicParas["phone"].ToString() : string.Empty;

            string sql = " select * from t_store where id like '%' + @storeId + '%' and companyname like '%' + @companyname + '%' and phone like '%' + @phone + '%' ";
            SqlParameter[] parameters = new SqlParameter[3];
            parameters[0] = new SqlParameter("@storeId", storeId);
            parameters[1] = new SqlParameter("@companyname", companyname);
            parameters[2] = new SqlParameter("@phone", phone);

            var storeList = storeService.SqlQuery<t_store>(sql, parameters).ToList<t_store>();
            for (int i = 0; i < storeList.Count;i++ )
            {
                var store = storeList[i];
                var obj = new
                {
                    id = store.id,
                    companyname = store.companyname,
                    province = store.province,
                    address = store.address,
                    boss = store.boss,
                    phone = store.phone,
                    telphone = store.telphone,
                    client_level = store.client_level,
                    createtime = Convert.ToDateTime(store.createtime).ToString("yyyy-MM-dd HH:mm:ss"),
                    updatetime = Convert.ToDateTime(store.updatetime).ToString("yyyy-MM-dd HH:mm:ss"),
                    power_due_date = Convert.ToDateTime(store.power_due_date).ToString("yyyy-MM-dd"),
                    note = store.note,
                    parentid = store.parentid,
                    developer = store.developer,
                    store_password = store.store_password,
                    store_dbname = store.store_dbname,
                    wxfee = store.wxfee * 1000
                };
                objList.Add(obj);              
            }
            return ResponseModelFactory.CreateAnonymousSuccessModel(isSignKeyReturn, objList);
        }
    }
}