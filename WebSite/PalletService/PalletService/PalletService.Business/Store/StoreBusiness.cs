using PalletService.Business.SysConfig;
using PalletService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PalletService.Business.Store
{
    public class StoreBusiness
    {
        public static bool GetStorePassword(string userToken,out string password,out string errMsg)
        {
            //获取店密码接口
            string url = SysConfigBusiness.XCCloudHost + "/xccloud/storeinfo?action=getStorePassword";
            var obj = new
            {
                userToken = userToken
            };

            password = string.Empty;
            errMsg = string.Empty;

            try
            {
                string param = Utils.SerializeObject(obj);
                string resultJson = Utils.HttpPost(url, param);
                object result_data = null;
                if (Utils.CheckApiReturnJson(resultJson, ref result_data, out errMsg))
                {
                    password = Utils.GetJsonObjectValue(result_data, "storePassword").ToString();
                    return true;
                }
                else
                {

                    return false;
                }

            }
            catch
            {
                errMsg = "获取店密码出错";
                return false;
            }
        }
    }
}
