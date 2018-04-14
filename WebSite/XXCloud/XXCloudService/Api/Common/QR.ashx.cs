using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using XCCloudService.Common;
using XCCloudService.WeiXin.WeixinOAuth;

namespace XCCloudService.Api.Common
{
    /// <summary>
    /// Client 的摘要说明
    /// </summary>
    public class QR : ApiBase
    {
        [ApiMethodAttribute(SignKeyEnum = SignKeyEnum.MethodToken, RespDataTypeEnum = RespDataTypeEnum.ImgStream, SysIdAndVersionNo = false)]
        public object getQR(Dictionary<string, object> dicParas)
        {
            string type = dicParas.ContainsKey("type") ? dicParas["type"].ToString() : string.Empty;
            switch(type)
            {
                case "a": return CreateDeviceTokenQRImage(dicParas);//生成设备token的二维码图(可跳转小程序)
                case "b": return CreateWeiXinPubNoLoginQRImage();//生成设备token的二维码图(可跳转小程序)
                //case "c": return CreateXcBizAuthQRImage();//生成业务授权二维码图
                case "d": return CreateTicketQRImage(dicParas);
            }
            return null;
        }

        private byte[] CreateTicketQRImage(Dictionary<string, object> dicParas)
        {
            string ticketNo = dicParas.ContainsKey("para1") ? dicParas["para1"].ToString() : string.Empty;
            string url = string.Format("https://mp.4000051530.com/t/{0}", ticketNo);
            return QRHelper.CreateQR(url, 4, 8);
        }

        private byte[] CreateDeviceTokenQRImage(Dictionary<string, object> dicParas)
        {
            string deviceToken = dicParas.ContainsKey("para1") ? dicParas["para1"].ToString() : string.Empty;
            string url = string.Format("https://mp.4000051530.com/b/{0}", deviceToken);
            return QRHelper.CreateQR(url, 4, 8);
        }

        private byte[] CreateWeiXinPubNoLoginQRImage()
        {
            string url = WeiXinQR.GetRQImageBySnsapi_Login();
            return QRHelper.CreateQR(url, 4, 11);
        }

        //private byte[] CreateWeiXinMerchLoginQRImage()
        //{
        //    Image image = null;
        //    string url = WeiXinQR.GetRQImageByMerch_Login();
        //    string page = Utils.WebClientDownloadString(url);
        //    var doc = new HtmlDocument();
        //    //doc.Load(System.Web.HttpContext.Current.Server.MapPath("~/htmltest.txt"));
        //    doc.LoadHtml(page);
        //    var nodesMatchingXPath = doc.DocumentNode.Descendants("img").Where(x => x.Attributes["class"].Value == "qrcode lightBorder").FirstOrDefault<HtmlNode>();
        //    if(nodesMatchingXPath != null && nodesMatchingXPath.Attributes["src"] != null)
        //    {
        //        string imageUrl = "https://open.weixin.qq.com/connect/qrconnect" + nodesMatchingXPath.Attributes["src"].Value;
        //        image = Utils.DownloadImageFromUrl(imageUrl);
        //    }

        //    return QRHelper.CreateQR(image, 4, 11);
        //}
    }
}