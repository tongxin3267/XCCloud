using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud
{
    [DataContract]
    public class MenuInfoModel
    {
        [DataMember(Name = "functionId", Order = 1)]
        public int FunctionID { get; set; }

        [DataMember(Name = "menuEnable", Order = 2)]
        public int MenuEnable { get; set; }

        [IgnoreDataMember]
        public Nullable<int> ParentID { get; set; }

        [DataMember(Name = "name", Order = 3)]
        public string FunctionName { get; set; }

        [DataMember(Name = "pageName", Order = 4)]
        public string PageName { get; set; }

        [DataMember(Name = "Icon", Order = 5)]
        public string Icon { get; set; }

        [DataMember(Name = "children", Order = 6)]
        public List<MenuInfoModel> Children { get; set; }

    }    
}
