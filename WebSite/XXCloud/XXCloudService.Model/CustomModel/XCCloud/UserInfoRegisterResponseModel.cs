using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloud
{
    [DataContract]
    public class UserInfoRegisterResponseModel
    {
        [DataMember(Name = "userId", Order = 1)]
        public int UserID { get; set; }

        [DataMember(Name = "storeId", Order = 2)]
        public string StoreID { get; set; }

        [DataMember(Name = "storeName", Order = 3)]
        public string StoreName { get; set; }

        [DataMember(Name = "loginName", Order = 4)]
        public string LogName { get; set; }

        [DataMember(Name = "realName", Order = 5)]
        public string RealName { get; set; }

        [DataMember(Name = "mobile", Order = 6)]
        public string Mobile { get; set; }

        public DateTime CreateTime { set; get; }

        [DataMember(Name = "createTime", Order = 7)]
        public string CreateTimeString
        {
            set { this.CreateTimeString = value; }
            get { return this.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }
    }

    [DataContract]
    public class UserInfoResponseModel
    {
        [DataMember(Name = "userId", Order = 1)]
        public int UserID { get; set; }

        [DataMember(Name = "loginName", Order = 2)]
        public string LogName { get; set; }

        [DataMember(Name = "realName", Order = 3)]
        public string RealName { get; set; }

        [DataMember(Name = "mobile", Order = 4)]
        public string Mobile { get; set; }

        [DataMember(Name = "icCardId", Order = 5)]
        public string ICCardID {get;set;}

        [DataMember(Name = "allowUnlock", Order = 6)]
        public int AllowUnlock {get;set;}

        [DataMember(Name = "allowExitCoin", Order = 7)]
        public int AllowExitCoin { get; set; }

        public DateTime CreateTime { set; get; }

        [DataMember(Name = "createTime", Order = 8)]
        public string CreateTimeString
        {
            set { this.CreateTimeString = value; }
            get { return this.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        [DataMember(Name = "stauts", Order = 9)]
        public int Status {set;get;}

        [DataMember(Name = "stautsName", Order = 10)]
        public string StatusName 
        {
            set { this.StatusName = value; }
            get {
                    if (this.Status == 0) {
                        return "审核中";
                    }
                    else if (this.Status == 1)
                    {
                        return "已启用";
                    }
                    else if (this.Status == 2)
                    {
                        return "已禁用";
                    }
                    else {
                        return "";
                    }
            }
        }

        [DataMember(Name = "storeList", Order = 11)]
        public List<Base_StoreInfoModel> StoreList { set; get; }

        [DataMember(Name = "auditor", Order = 12)]
        public int Auditor {set;get;}

        [DataMember(Name = "authorGroup", Order = 13)]
        public int AuthorGroup { set; get; }
    }


}
