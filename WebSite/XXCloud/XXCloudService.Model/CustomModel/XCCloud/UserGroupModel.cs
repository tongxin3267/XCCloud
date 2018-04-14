using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud
{

    [DataContract]
    public class UserGroupModel
    {
        [DataMember(Name = "groupId", Order = 1)]
        public int ID { get; set; }

        [DataMember(Name = "groupName", Order = 2)]
        public string GroupName { get; set; }

        [DataMember(Name = "note", Order = 3)]
        public string Note { get; set; }

        [DataMember(Name = "userGroupGrants", Order = 4)]
        public List<UserGroupGrantModel> UserGroupGrants { get; set; }

        [DataMember(Name = "checked", Order = 5)]
        public Nullable<int> UserState { get; set; }
    }

    [DataContract]
    public class UserGroupGrantModel
    {
        [DataMember(Name = "functionId", Order = 1)]
        public int FunctionID { get; set; }

        [IgnoreDataMember]
        public Nullable<int> ParentID { get; set; }

        [DataMember(Name = "checked", Order = 2)]
        public Nullable<int> IsAllow { get; set; }

        [DataMember(Name = "name", Order = 3)]
        public string FunctionName { get; set; }

        [DataMember(Name = "children", Order = 4)]
        public List<UserGroupGrantModel> Children { get; set; }
    }
}
