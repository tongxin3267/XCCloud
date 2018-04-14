using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud
{
    public class Base_MerchantInfoListModel
    {
        public string MerchID { get; set; }
        public string MerchName { get; set; }
        public string Mobil { get; set; }
        public string MerchTypeStr { get; set; }
        public string MerchAccount { get; set; }        
        public string AllowCreateSubStr { get; set; }
        public Nullable<int> AllowCreateCount { get; set; }
        public string MerchStatusStr { get; set; }
    }

    [DataContract]
    public partial class Base_MerchInfoModel
    {
        [DataMember(Name = "merchId", Order = 1)]
        public string MerchID { get; set; }

        [DataMember(Name = "merchType", Order = 2)]
        public Nullable<int> MerchType { get; set; }

        [DataMember(Name = "merchStatus", Order = 3)]
        public Nullable<int> MerchStatus { get; set; }

        [DataMember(Name = "merchAccount", Order = 4)]
        public string MerchAccount { get; set; }

        [DataMember(Name = "merchName", Order = 5)]
        public string MerchName { get; set; }

        [DataMember(Name = "mobil", Order = 6)]
        public string Mobil { get; set; }

        [DataMember(Name = "allowCreateSub", Order = 7)]
        public Nullable<int> AllowCreateSub { get; set; }

        [DataMember(Name = "allowCreateCount", Order = 8)]
        public Nullable<int> AllowCreateCount { get; set; }

        [DataMember(Name = "createUserId", Order = 9)]
        public string CreateUserID { get; set; }

        [DataMember(Name = "comment", Order = 10)]
        public string Comment { get; set; }

        [DataMember(Name = "merchFunction", Order = 11)]
        public List<Base_MerchFunctionModel> MerchFunction { get; set; }

        [DataMember(Name = "merchTypes", Order = 12)]
        public List<DictionaryResponseModel> MerchTypes { get; set; }

        [DataMember(Name = "merchTag", Order = 13)]
        public Nullable<int> MerchTag { get; set; }
    }

    [DataContract]
    public partial class Base_MerchFunctionModel
    {
        [DataMember(Name = "functionId", Order = 1)]
        public int FunctionID { get; set; }

        [IgnoreDataMember]
        public Nullable<int> ParentID { get; set; }

        [DataMember(Name = "checked", Order = 2)]
        public Nullable<int> FunctionEN { get; set; }

        [DataMember(Name = "name", Order = 3)]
        public string FunctionName { get; set; }

        [DataMember(Name = "children", Order = 4)]
        public List<Base_MerchFunctionModel> Children { get; set; }
    }
}
