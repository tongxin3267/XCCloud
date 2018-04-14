using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XCCloudService.Model.WeiXin.Message;

namespace XCCloudService.Model.WeiXin.SAppMessage
{
    [SAppMsgConfigAttribute(MsgType = "FoodSaleNotify")]
    [XmlRoot(ElementName = "msgtemplate")]
    public class MemberFoodSaleConfigModel
    {
        [XmlElement(ElementName="title")]
        public string Title { set;get; }

        [XmlElement(ElementName = "templateid")]
        public string TemplateId { set; get; }

        [XmlElement(ElementName = "detailsurl")]
        public string DetailsUrl { set; get; }
    }
}
