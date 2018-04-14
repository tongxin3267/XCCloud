using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XCCloudService.Common
{
    public class MapService
    {
        public static string GetBaiduMapCoordinateAnalysis(string latitude, string longitude)
        {
            string APK = System.Configuration.ConfigurationManager.AppSettings["baidumapAPK"].ToString();
            string url = string.Format("http://api.map.baidu.com/geocoder/v2/?location={0},{1}&output=json&pois=0&ak={2}", latitude, longitude, APK);
            string json = Utils.GetUrlRequestResult(url);
            return json;
        }

        public static string GetBaiduMapIPAnalysis(string ip)
        {
            string APK = System.Configuration.ConfigurationManager.AppSettings["baidumapAPK"].ToString();
            string url = string.Format("http://api.map.baidu.com/location/ip?ip={0}&ak={1}&coor=bd09ll", ip, APK);
            string json = Utils.GetUrlRequestResult(url);
            return json;
        }
    }
}