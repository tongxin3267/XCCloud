using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XCCloudService.Model.WeiXin.Message
{
    [WeixinMsgConfigAttribute(MsgType = "MerchResetPassword")]
    [XmlRoot(ElementName = "msgtemplate")]
    public class MerchResetPasswordConfigModel
    {
        [XmlElement(ElementName="title")]
        public string Title { set;get; }

        [XmlElement(ElementName = "templateid")]
        public string TemplateId { set; get; }

        [XmlElement(ElementName = "detailsurl")]
        public string DetailsUrl { set; get; }

        [XmlElement(ElementName = "sapppagepath")]
        public string SAppPagePath { set; get; }

        [XmlElement(ElementName = "remark")]
        public string Remark { set; get; }

        [XmlElement(ElementName = "firstcolor")]
        public string FirstColor { set; get; }

        [XmlElement(ElementName = "keynote1color")]
        public string Keynote1Color { set; get; }

        [XmlElement(ElementName = "keynote2color")]
        public string Keynote2Color { set; get; }

        [XmlElement(ElementName = "remarkcolor")]
        public string RemarkColor { set; get; }
    }
}
