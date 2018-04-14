using PalletService.Business.SysConfig;
using PalletService.Common;
using PalletService.Model.SocketData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PalletService.Business.Member
{
    public class MemberBusiness
    {
        public static bool GetMemberRepeatCode(string userToken,string storeId,string icCardId,out string repeatCode,out string errMsg)
        {
            repeatCode = string.Empty;
            errMsg = string.Empty;
            string url = SysConfigBusiness.XCCloudHost + "/xccloud/member?action=getMemberRepeatCode";
            var obj = new {
                userToken = userToken,
                storeId = storeId,
                icCardId = icCardId
            };
            string param = Utils.SerializeObject(obj);
            string resultJson = Utils.HttpPost(url, param);
            object result_data = null;
            if (Utils.CheckApiReturnJson(resultJson, ref result_data,out errMsg))
            {
                if (result_data == null)
                {
                    errMsg = "获取会员卡信息出错";
                    return false;
                }
                else
                {
                    object repeatCodeObject = Utils.GetJsonObjectValue(result_data, "repeatCode");
                    if (repeatCodeObject == null)
                    {
                        errMsg = "获取会员卡信息出错";
                        return false;
                    }
                    else
                    {
                        repeatCode = repeatCodeObject.ToString();
                        return true;
                    }
                }
            }
            else
            {
                errMsg = "获取会员卡信息出错";
                return false;
            }
        }


        public static bool MemberRegister(MemberRegisterModel memberRegisterModel,out int repeatCode,out string errMsg, ref object result_data)
        {
            errMsg = string.Empty;
            repeatCode = 0;
            string url = SysConfigBusiness.XCCloudHost + "/xccloud/member?action=register";
            string param = Utils.SerializeObject(memberRegisterModel);
            string resultJson = Utils.HttpPost(url, param);
            if (Utils.CheckApiReturnJson(resultJson, ref result_data,out errMsg))
            {
                object repeatCodeObj = Utils.GetJsonObjectValue(result_data, "repeatCode");
                repeatCode = Convert.ToInt32(repeatCodeObj);
                return true;
            }
            else
            {
                return false;
            }
        }


        public static bool CheckMemberCanRegister(string userToken,string storeId,string mobile,out string errMsg)
        {
            errMsg = string.Empty;
            string url = SysConfigBusiness.XCCloudHost + "/xccloud/member?action=checkOpenCard";
            var obj = new {
                userToken = userToken,
                storeId = storeId,
                mobile = mobile
            };
            string param = Utils.SerializeObject(obj);
            string resultJson = Utils.HttpPost(url, param);
            object result_data = null;
            if (Utils.CheckApiReturnJson(resultJson, ref result_data, out errMsg))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool GetMember(string userToken, string storeId, string icCardId, ref object result_data, out string errMsg)
        {
            errMsg = string.Empty;
            string url = SysConfigBusiness.XCCloudHost + "/xccloud/member?action=getMember";
            var obj = new
            {
                userToken = userToken,
                storeId = storeId,
                icCardId = icCardId
            };
            string param = Utils.SerializeObject(obj);
            string resultJson = Utils.HttpPost(url, param);
            if (Utils.CheckApiReturnJson(resultJson, ref result_data, out errMsg))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
