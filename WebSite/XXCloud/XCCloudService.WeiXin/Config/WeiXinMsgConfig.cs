using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using XCCloudService.Common;
using XCCloudService.Model.WeiXin.Message;

namespace XCCloudService.WeiXin.Config
{
    public class WeiXinMsgConfig
    {
        public static bool GetMsgTemplateConfig<T>(ref T t)
        {
            Type type = typeof(T);
            Attribute[] attributeArray = type.GetCustomAttributes(typeof(WeixinMsgConfigAttribute), false) as Attribute[];
            if (attributeArray.Count() == 0)
            {
                return false;
            }

            WeixinMsgConfigAttribute configAttribute = (WeixinMsgConfigAttribute)(attributeArray[0]);
            if (configAttribute == null)
            {
                return false;
            }

            

            string xmlFilePath = HttpContext.Current.Server.MapPath("/Config/WeiXinMessageTemplate.xml");
            XmlDocument xd = new XmlDocument();
            xd.Load(xmlFilePath);
            var node = xd.SelectSingleNode(string.Format("/msgtemplates/msgtemplate[@id=\"{0}\"]", configAttribute.MsgType));
            if (node == null)
            {
                return false;
            }

            object obj = Utils.XmlDeserialize(typeof(T), node.OuterXml.Trim());
            t = (T)obj;
            return true;
        }
    }
}
