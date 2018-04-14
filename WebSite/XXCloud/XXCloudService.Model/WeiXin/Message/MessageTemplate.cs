using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.WeiXin.Message
{
    [DataContract]
    public class MsgChildDataTemplateModel
    {
        public MsgChildDataTemplateModel(string value,string color)
        {
            this.Value = value;
            this.Color = color;
        }

        [DataMember(Name = "value", Order = 1)]
        public string Value { set; get; }

        [DataMember(Name = "color", Order = 2)]
        public string Color { set; get; }
    }

    [DataContract]
    public class MsgDataTemplateModel
    {
        public MsgDataTemplateModel(MsgChildDataTemplateModel first, MsgChildDataTemplateModel keyNote1,
            MsgChildDataTemplateModel keyNote2, MsgChildDataTemplateModel keyNote3, MsgChildDataTemplateModel remark)
        {
            this.First = first;
            this.KeyNote1 = keyNote1;
            this.KeyNote2 = keyNote2;
            this.KeyNote3 = keyNote3;
            this.Remark = remark;
        }

        [DataMember(Name = "first", Order = 1)]
        public MsgChildDataTemplateModel First { set; get; }

        [DataMember(Name = "keynote1", Order = 2)]
        public MsgChildDataTemplateModel KeyNote1 { set; get; }

        [DataMember(Name = "keynote2", Order = 2)]
        public MsgChildDataTemplateModel KeyNote2 { set; get; }

        [DataMember(Name = "keynote3", Order = 2)]
        public MsgChildDataTemplateModel KeyNote3 { set; get; }

        [DataMember(Name = "remark", Order = 2)]
        public MsgChildDataTemplateModel Remark { set; get; }
    }

}
