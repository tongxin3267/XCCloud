using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.XCCloud
{
    
}

namespace XCCloudService.Model.CustomModel.XCCloud
{    

    [DataContract]
    public class Base_StoreInfoModel
    {
        [DataMember(Name = "storeId", Order = 1)]
        public string StoreID { set; get; }

        [DataMember(Name = "storeName", Order = 2)]
        public string StoreName { set; get; }
    }

    public partial class StoreInfoModel
    {
        public string StoreID { get; set; }
        public string ParentID { get; set; }
        public string MerchID { get; set; }
        public string StoreName { get; set; }
        public string Password { get; set; }
        public Nullable<System.DateTime> AuthorExpireDate { get; set; }
        public string AreaCode { get; set; }
        public string Address { get; set; }
        public Nullable<decimal> Lng { get; set; }
        public Nullable<decimal> Lat { get; set; }
        public string Contacts { get; set; }
        public string IDCard { get; set; }
        public Nullable<System.DateTime> IDExpireDate { get; set; }
        public string Mobile { get; set; }
        public string ShopSignPhoto { get; set; }
        public string LicencePhoto { get; set; }
        public string LicenceID { get; set; }
        public Nullable<System.DateTime> LicenceExpireDate { get; set; }
        public string BankType { get; set; }
        public string BankCode { get; set; }
        public string BankAccount { get; set; }
        public Nullable<int> SelttleType { get; set; }
        public Nullable<int> SettleID { get; set; }
        public Nullable<int> StoreState { get; set; }
        public string BankTypeStr { get; set; }
        public string SelttleTypeStr { get; set; }
        public Dictionary<string, object> SettleFields { get; set; }
    }

    public class Base_StoreInfoListModel
    {
        public string StoreID { get; set; }
        public string MerchID { get; set; }
        public string StoreName { get; set; }
        public string Password { get; set; }
        public Nullable<DateTime> AuthorExpireDate { get; set; }
        public string AreaCode { get; set; }
        public string Address { get; set; }
        public string Contacts { get; set; }
        public string Mobile { get; set; }
        public string SelttleTypeStr { get; set; }
        public string StoreStateStr { get; set; }
        public Nullable<DateTime> AuditTime { get; set; }
    }
}
