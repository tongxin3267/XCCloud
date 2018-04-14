using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.DAL.XCGameManager;
using XCCloudService.Model.XCGameManager;

namespace XCCloudService.Common
{
    public class ApiRequestLog
    {
        public string show(int num, string requestUrl, string postJson, string returnCode, string exception, string sysId, string logMessage = "")
        {
            try
            {                
                IApiRequestLogService apirequestlogService = BLLContainer.Resolve<IApiRequestLogService>();
                t_apiRequestLog log = new t_apiRequestLog();
                log.ApiType = (byte)num;
                log.CreateTime = System.DateTime.Now;
                log.RequestUrl = requestUrl;
                log.RequestContent = postJson;
                log.ReturnCode = returnCode;
                log.Exception = exception;
                log.LogMessage = logMessage;
                log.SysId = sysId;
                apirequestlogService.Add(log);
            }
            catch
            {

            }
            return "";
        }
      
    }
}