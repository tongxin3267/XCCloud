using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.Common;

namespace XXCloudService.Api.Service
{
    /// <summary>
    /// GetHolidays 的摘要说明
    /// </summary>
    public class GetHolidays : ApiBase
    {

        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken, SysIdAndVersionNo = false)]
        public object IsHoliday(Dictionary<string, object> dicParas)
        {
            try
            {
                string errMsg = string.Empty;
                string date = dicParas.ContainsKey("date") ? Convert.ToString(dicParas["date"]) : string.Empty;

                //DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();
                //dtFormat.ShortDatePattern = "yyyyMMdd";
                //var dt = Convert.ToDateTime(date, dtFormat);
                var dt = DateTime.ParseExact(date, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                var result = HolidayHelper.GetInstance().IsHoliday(dt);
                return ResponseModelFactory.CreateSuccessModel(isSignKeyReturn, result);
            }
            catch (Exception e)
            {
                return ResponseModelFactory.CreateReturnModel(isSignKeyReturn, Return_Code.F, e.Message);
            }
        }
    }
}