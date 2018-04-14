using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.DBService.Model;
using XCCloudService.DBService.BLL;

namespace XCCloudService.Api
{
    /// <summary>
    /// Query 的摘要说明
    /// </summary>
    public class Query : ApiBase
    {
        public object init(Dictionary<string, object> dicParas)
        {
            string pageName = string.Empty;
            string processName = string.Empty;
            int userId = 0;

            if (dicParas.ContainsKey("pagename"))
            {
                pageName = dicParas["pagename"].ToString();
            }

            if (dicParas.ContainsKey("processname"))
            {
                processName = dicParas["processname"].ToString();
            }

            if (dicParas.ContainsKey("userid"))
            {
                int.TryParse(dicParas["userid"].ToString(),out userId);
            }

            string errMsg = string.Empty;
            if (string.IsNullOrEmpty(pageName))
            {
                errMsg = "页面名参数不存在";
            }
            else if (string.IsNullOrEmpty(processName))
            {
                errMsg = "功能名参数不存在";
            }

            if (!string.IsNullOrEmpty(errMsg))
            {
                ResponseModel<List<InitModel>> responseModel = new ResponseModel<List<InitModel>>();
                responseModel.Result_Code = Result_Code.F;
                responseModel.Result_Msg = errMsg;
                return responseModel;        
            }
            else
            { 
                List<InitModel> listInitModel = null;
                List<Dict_SystemModel> listDict_SystemModel = null;
                QueryBLL.GetInit(pageName, processName, userId, ref listInitModel, ref listDict_SystemModel);
                ResponseModel<List<InitModel>> responseModel = new ResponseModel<List<InitModel>>();
                responseModel.Result_Data = listInitModel;
                return responseModel;                
            }
        }
    }
}