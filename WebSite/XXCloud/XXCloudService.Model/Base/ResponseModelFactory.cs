using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XCCloudService.Base
{
    public class ResponseModelFactory
    {
        public static object CreateModel(bool isSignKeyReturn,string returnCode,string returnMsg,string resultCode,string resultMsg)
        {
            if (!isSignKeyReturn)
            {
                return new NoSignKeyResponseModel(returnCode, returnMsg, resultCode, resultMsg);
            }
            else
            {
                return new ResponseModel(returnCode, returnMsg, resultCode, resultMsg);
            }
        }

        public static object CreateSuccessModel(bool isSignKeyReturn)
        {
            if (!isSignKeyReturn)
            {
                return new NoSignKeyResponseModel("1", string.Empty, "1", string.Empty);
            }
            else
            {
                return new ResponseModel("1", string.Empty, "1", string.Empty);
            }
        }

        public static object CreateSuccessModel<T>(bool isSignKeyReturn, T obj)
        {
            if (!isSignKeyReturn)
            {
                return new NoSignKeyResponseModel<T>(obj);
            }
            else
            {
                return new ResponseModel<T>(obj);
            }
        }

        public static object CreateFailModel(bool isSignKeyReturn, string errMsg)
        {
            if (!isSignKeyReturn)
            {
                return new NoSignKeyResponseModel("1", string.Empty, "0", errMsg);
            }
            else
            {
                return new ResponseModel("1", string.Empty, "0", errMsg);
            }
        }

        public static object CreateReturnModel(bool isSignKeyReturn, string return_code, string return_msg)
        {
            if (isSignKeyReturn)
            {
                return new ReturnResponseModel(return_code, return_msg);
            }
            else
            {
                return new NoSignKeyReturnResponseModel(return_code, return_msg);
            }
        }

        public static void CreateModel(object isSignKeyReturn, string t, string v1, string f, string v2)
        {
            throw new NotImplementedException();
        }

        public static object CreateAnonymousSuccessModel(bool isSignKeyReturn, object obj)
        {
            if (!isSignKeyReturn)
            {
                var resObj = new
                {
                    return_code = "1",
                    return_msg = string.Empty,
                    result_code = "1",
                    result_msg = string.Empty,
                    result_data = obj
                };
                return resObj;
            }
            else
            {
                var resObj = new
                {
                    return_code = "1",
                    return_msg = string.Empty,
                    result_code = "1",
                    result_msg = string.Empty,
                    result_data = obj,
                    signkey = string.Empty
                };
                return resObj;
            }
        }
       
        public static object CreateAnonymousFailModel(bool isSignKeyReturn,string errMsg)
        {
            if (!isSignKeyReturn)
            {
                var resObj = new
                {
                    return_code = "1",
                    return_msg = string.Empty,
                    result_code = "0",
                    result_msg = errMsg,
                };
                return resObj;
            }
            else
            {
                var resObj = new
                {
                    return_code = "1",
                    return_msg = string.Empty,
                    result_code = "0",
                    result_msg = errMsg,
                    signkey = string.Empty
                };
                return resObj;
            }
        }

        public static object CreateAnonymousModelByFail(bool isSignKeyReturn, string errMsg)
        {
            if (!isSignKeyReturn)
            {
                var resObj = new
                {
                    return_code = "1",
                    return_msg = string.Empty,
                    result_code = "0",
                    result_msg = errMsg,
                };
                return resObj;
            }
            else
            {
                var resObj = new
                {
                    return_code = "1",
                    return_msg = string.Empty,
                    result_code = "0",
                    result_msg = errMsg,
                    signkey = string.Empty
                };
                return resObj;
            }
        }
    }

    public class ResponseModelFactory<T>
    {
        public static object CreateModel(bool isSignKeyReturn, T t)
        {
            if (!isSignKeyReturn)
            {
                return new NoSignKeyResponseModel<T>(t);
            }
            else
            {
                return new ResponseModel<T>(t);
            }
        }
    }
}