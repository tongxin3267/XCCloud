using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Model.CustomModel.XCCloudRS232
{
     [DataContract]
     public class MerchRevenueModel
     {
         public MerchRevenueModel()
         {
             Amount = "0.00";
         }

         [DataMember(Name = "MerchName", Order = 0)]
         public string MerchName { get; set; }

         [DataMember(Name = "Amount", Order = 1)]
         public string Amount { get; set; }

         [DataMember(Name = "Revenues", Order = 2)]
         public List<RevenueDataModel> Revenues { get; set; }

         [DataMember(Name = "Payment", Order = 3)]
         public List<RevenueDataModel> Payment { get; set; }

         [DataMember(Name = "Coins", Order = 4)]
         public List<RevenueDataModel> Coins { get; set; }
     }

     public class RevenueDataModel
     {
         public string Title { get; set; }

         public string Value { get; set; }
     }

     [DataContract]
     public class MerchRoutersRevenueModel
     {
         [DataMember(Name = "RouterName", Order = 0)]
         public string RouterName { get; set; }

         [DataMember(Name = "Token", Order = 1)]
         public string Token { get; set; }

         [DataMember(Name = "SaveCoins", Order = 2)]
         public CoinRevenueModel SaveCoins { get; set; }

         [DataMember(Name = "SaleCoins", Order = 3)]
         public CoinRevenueModel SaleCoins { get; set; }
     }

    [DataContract]
     public class CoinRevenueModel
     {
         public CoinRevenueModel()
         {
             this.CoinsTotal = "0";
         }

         [DataMember(Name = "CoinsTotal", Order = 0)]
         public string CoinsTotal { get; set; }

         [DataMember(Name = "Coins", Order = 1)]
         public List<RevenueDataModel> Coins { get; set; }
     }

     [DataContract]
     public class PeripheralRevenueModel
     {
         public PeripheralRevenueModel()
         {
             this.Coins = new List<RevenueDataModel>();
             this.CoinSerials = new List<CoinSerialModel>();
         }

         [DataMember(Name = "DeviceName", Order = 1)]
         public string DeviceName { get; set; }

         [DataMember(Name = "SN", Order = 2)]
         public string SN { get; set; }

         [DataMember(Name = "Coins", Order = 3)]
         public List<RevenueDataModel> Coins { get; set; }

         [DataMember(Name = "CoinSerials", Order = 4)]
         public List<CoinSerialModel> CoinSerials { get; set; }
     }

     [DataContract]
     public class CoinSerialModel
     {
         [DataMember(Name = "ICCardID", Order = 1)]
         public int ICCardID { get; set; }

         [DataMember(Name = "MemberName", Order = 2)]
         public string MemberName { get; set; }

         [DataMember(Name = "Mobile", Order = 3)]
         public string Mobile { get; set; }

         [DataMember(Name = "FlowType", Order = 4)]
         public string FlowType { get; set; }

         [DataMember(Name = "Amount", Order = 5)]
         public decimal Amount { get; set; }

         [DataMember(Name = "Coins", Order = 6)]
         public int Coins { get; set; }

         [DataMember(Name = "Balance", Order = 7)]
         public int Balance { get; set; }

         [DataMember(Name = "RealTime", Order = 8)]
         public string RealTime { get; set; }
     }

     [DataContract]
     public class MemberExpenseDetailModel
     {
         public MemberExpenseDetailModel()
         {
             this.MemberName = "";
         }

         [DataMember(Name = "ICCardID", Order = 0)]
         public int ICCardID { get; set; }

         [DataMember(Name = "MemberName", Order = 1)]
         public string MemberName { get; set; }

         [DataMember(Name = "Mobile", Order = 2)]
         public string Mobile { get; set; }

         [DataMember(Name = "Flow", Order = 3)]
         public string Flow { get; set; }

         [DataMember(Name = "Amount", Order = 4)]
         public decimal Amount { get; set; }

         [DataMember(Name = "Coins", Order = 5)]
         public int Coins { get; set; }

         [DataMember(Name = "Balance", Order = 6)]
         public int Balance { get; set; }

         [DataMember(Name = "RealTime", Order = 7)]
         public string RealTime { get; set; } 
     }

    [DataContract]
     public class MemberViewModel
     {
        [DataMember(Name = "MemberTotal", Order = 1)]
        public int MemberTotal { get; set; }

        [DataMember(Name = "BalanceTotal", Order = 2)]
        public int BalanceTotal { get; set; }

        [DataMember(Name = "TodayJoinMembers", Order = 3)]
        public int TodayJoinMembers { get; set; }

        [DataMember(Name = "TodayGameMembers", Order = 4)]
        public int TodayGameMembers { get; set; }

        [DataMember(Name = "MemberList", Order = 5)]
        public List<MemberListModel> MemberList { get; set; }
     }

    [DataContract]
    public class MemberListModel
    {
        [DataMember(Name = "MemberID", Order = 1)]
        public int MemberID { get; set; }

        [DataMember(Name = "ICCardID", Order = 2)]
        public int ICCardID { get; set; }

        [DataMember(Name = "MemberName", Order = 3)]
        public string MemberName { get; set; }

        [DataMember(Name = "Mobile", Order = 4)]
        public string Mobile { get; set; }

        [DataMember(Name = "Balance", Order = 5)]
        public int Balance { get; set; }

        [DataMember(Name = "JoinTime", Order = 6)]
        public string JoinTime { get; set; }
    }

    [DataContract]
    public class MemberDetailModel
    {
        [DataMember(Name = "ICCardID", Order = 1)]
        public string ICCardID { get; set; }

        [DataMember(Name = "MemberName", Order = 2)]
        public string MemberName { get; set; }

        [DataMember(Name = "Mobile", Order = 3)]
        public string Mobile { get; set; }

        [DataMember(Name = "Birthday", Order = 4)]
        public string Birthday { get; set; }

        [DataMember(Name = "CertificalID", Order = 5)]
        public string CertificalID { get; set; }

        [DataMember(Name = "Balance", Order = 6)]
        public string Balance { get; set; }

        [DataMember(Name = "Lottery", Order = 7)]
        public string Lottery { get; set; }

        [DataMember(Name = "Point", Order = 8)]
        public string Point { get; set; }

        [DataMember(Name = "Type", Order = 9)]
        public string Type { get; set; }

        [DataMember(Name = "JoinTime", Order = 10)]
        public string JoinTime { get; set; }

        [DataMember(Name = "EndDate", Order = 11)]
        public string EndDate { get; set; }

        [DataMember(Name = "MemberPassword", Order = 12)]
        public string MemberPassword { get; set; }

        [DataMember(Name = "Note", Order = 13)]
        public string Note { get; set; }

        [DataMember(Name = "Lock", Order = 14)]
        public string Lock { get; set; }

        [DataMember(Name = "LockDate", Order = 15)]
        public string LockDate { get; set; }
    }
}
