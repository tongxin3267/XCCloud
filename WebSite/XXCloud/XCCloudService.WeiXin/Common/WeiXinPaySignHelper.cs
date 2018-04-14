using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.Common;
using XCCloudService.Common.Enum;

namespace XCCloudService.WeiXin.Common
{
    public class WeiXinPaySignHelper
    {
        //paySign = MD5(
        //appId=wxd678efh567hg6787
        //&nonceStr=5K8264ILTKCH16CQ2502SI8ZNMTM67VS
        //&package=prepay_id=wx2017033010242291fcfe0db70013231072
        //&signType=MD5
        //&timeStamp=1490840662
        //&key=qazwsxedcrfvtgbyhnujmikolp111111) = 22D9B4E54AB1950F51E0649E8810ACD6

        public static string GetSAppPaySignKey(string nonceStr, string prepay_id, string signType, string timeStamp)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("appId={0}",WeiXinConfig.WXSmallAppId));
            sb.Append(string.Format("&nonceStr={0}", nonceStr));
            sb.Append(string.Format("&package=prepay_id={0}", prepay_id));
            sb.Append(string.Format("&signType={0}", "MD5"));
            sb.Append(string.Format("&timeStamp={0}", timeStamp));
            sb.Append(string.Format("&key={0}", WeiXinConfig.KEY));
            LogHelper.SaveLog(TxtLogType.WeiXin, TxtLogContentType.Common, TxtLogFileType.Day, sb.ToString());
            return Utils.MD5(sb.ToString()).ToUpper();
        }
    }
}
