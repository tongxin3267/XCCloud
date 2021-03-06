﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace XCCloudService.Model.XCCloud
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class XCCloudDBEntities : DbContext
    {
        public XCCloudDBEntities()
            : base("name=XCCloudDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Base_EnumParams> Base_EnumParams { get; set; }
        public virtual DbSet<Base_MemberInfo> Base_MemberInfo { get; set; }
        public virtual DbSet<Base_MerchantInfo> Base_MerchantInfo { get; set; }
        public virtual DbSet<Base_MerchFunction> Base_MerchFunction { get; set; }
        public virtual DbSet<Base_SettleOrg> Base_SettleOrg { get; set; }
        public virtual DbSet<Base_SettlePPOS> Base_SettlePPOS { get; set; }
        public virtual DbSet<Base_StoreDogList> Base_StoreDogList { get; set; }
        public virtual DbSet<Base_StoreInfo> Base_StoreInfo { get; set; }
        public virtual DbSet<Base_UserGrant> Base_UserGrant { get; set; }
        public virtual DbSet<Base_UserGroup> Base_UserGroup { get; set; }
        public virtual DbSet<Base_UserGroup_Grant> Base_UserGroup_Grant { get; set; }
        public virtual DbSet<Data_BillInfo> Data_BillInfo { get; set; }
        public virtual DbSet<Data_Card_Right_StoreList> Data_Card_Right_StoreList { get; set; }
        public virtual DbSet<Data_DigitCoin> Data_DigitCoin { get; set; }
        public virtual DbSet<Data_DigitCoinDestroy> Data_DigitCoinDestroy { get; set; }
        public virtual DbSet<Data_GivebackRule> Data_GivebackRule { get; set; }
        public virtual DbSet<Data_Message> Data_Message { get; set; }
        public virtual DbSet<Data_Parameters> Data_Parameters { get; set; }
        public virtual DbSet<Dict_Area> Dict_Area { get; set; }
        public virtual DbSet<Dict_FunctionMenu> Dict_FunctionMenu { get; set; }
        public virtual DbSet<Flw_CheckDate> Flw_CheckDate { get; set; }
        public virtual DbSet<Flw_Digite_Coin> Flw_Digite_Coin { get; set; }
        public virtual DbSet<Flw_Order_Detail> Flw_Order_Detail { get; set; }
        public virtual DbSet<Flw_Order_SerialNumber> Flw_Order_SerialNumber { get; set; }
        public virtual DbSet<Log_Operation> Log_Operation { get; set; }
        public virtual DbSet<Search_Template> Search_Template { get; set; }
        public virtual DbSet<Search_Template_Detail> Search_Template_Detail { get; set; }
        public virtual DbSet<Store_GameTotal> Store_GameTotal { get; set; }
        public virtual DbSet<Store_HeadTotal> Store_HeadTotal { get; set; }
        public virtual DbSet<XC_WorkInfo> XC_WorkInfo { get; set; }
        public virtual DbSet<Data_LotteryInventory> Data_LotteryInventory { get; set; }
        public virtual DbSet<Data_LotteryStorage> Data_LotteryStorage { get; set; }
        public virtual DbSet<Log_GameAlarm> Log_GameAlarm { get; set; }
        public virtual DbSet<Store_CheckDate> Store_CheckDate { get; set; }
        public virtual DbSet<Flw_Schedule> Flw_Schedule { get; set; }
        public virtual DbSet<Flw_Digite_Coin_Detail> Flw_Digite_Coin_Detail { get; set; }
        public virtual DbSet<Flw_Food_ExitDetail> Flw_Food_ExitDetail { get; set; }
        public virtual DbSet<Flw_Food_SaleDetail> Flw_Food_SaleDetail { get; set; }
        public virtual DbSet<Data_CoinDestory> Data_CoinDestory { get; set; }
        public virtual DbSet<Data_CoinInventory> Data_CoinInventory { get; set; }
        public virtual DbSet<Data_CoinStorage> Data_CoinStorage { get; set; }
        public virtual DbSet<Data_MemberLevel> Data_MemberLevel { get; set; }
        public virtual DbSet<Data_Food_Detial> Data_Food_Detial { get; set; }
        public virtual DbSet<Flw_Food_Sale> Flw_Food_Sale { get; set; }
        public virtual DbSet<Data_Project_BandPrice> Data_Project_BandPrice { get; set; }
        public virtual DbSet<Base_ChainRule> Base_ChainRule { get; set; }
        public virtual DbSet<Base_ChainRule_Store> Base_ChainRule_Store { get; set; }
        public virtual DbSet<Base_StoreWeight> Base_StoreWeight { get; set; }
        public virtual DbSet<Flw_Good_Detail> Flw_Good_Detail { get; set; }
        public virtual DbSet<Base_DeviceInfo> Base_DeviceInfo { get; set; }
        public virtual DbSet<Data_Project_Device> Data_Project_Device { get; set; }
        public virtual DbSet<Dict_System> Dict_System { get; set; }
        public virtual DbSet<Data_Card_Right> Data_Card_Right { get; set; }
        public virtual DbSet<Flw_485_Coin> Flw_485_Coin { get; set; }
        public virtual DbSet<Flw_485_SaveCoin> Flw_485_SaveCoin { get; set; }
        public virtual DbSet<Flw_Coin_Exit> Flw_Coin_Exit { get; set; }
        public virtual DbSet<Flw_Food_Exit> Flw_Food_Exit { get; set; }
        public virtual DbSet<Flw_Goods> Flw_Goods { get; set; }
        public virtual DbSet<Flw_Jackpot> Flw_Jackpot { get; set; }
        public virtual DbSet<Flw_Lottery> Flw_Lottery { get; set; }
        public virtual DbSet<Flw_Order> Flw_Order { get; set; }
        public virtual DbSet<Flw_Project_Play_Time> Flw_Project_Play_Time { get; set; }
        public virtual DbSet<Flw_Ticket_Exit> Flw_Ticket_Exit { get; set; }
        public virtual DbSet<Flw_Coin_Sale> Flw_Coin_Sale { get; set; }
        public virtual DbSet<Flw_Game_Free> Flw_Game_Free { get; set; }
        public virtual DbSet<Flw_Giveback> Flw_Giveback { get; set; }
        public virtual DbSet<Flw_Project_BuyDetail> Flw_Project_BuyDetail { get; set; }
        public virtual DbSet<Flw_Transfer> Flw_Transfer { get; set; }
        public virtual DbSet<Data_Member_Card> Data_Member_Card { get; set; }
        public virtual DbSet<Data_Card_Balance_StoreList> Data_Card_Balance_StoreList { get; set; }
        public virtual DbSet<Data_Member_Card_Store> Data_Member_Card_Store { get; set; }
        public virtual DbSet<Data_Card_Balance> Data_Card_Balance { get; set; }
        public virtual DbSet<Base_StoreWeight_Game> Base_StoreWeight_Game { get; set; }
        public virtual DbSet<Data_GoodStorage> Data_GoodStorage { get; set; }
        public virtual DbSet<Data_GoodInventory> Data_GoodInventory { get; set; }
        public virtual DbSet<Data_Workstation> Data_Workstation { get; set; }
        public virtual DbSet<Data_GameFreeRule> Data_GameFreeRule { get; set; }
        public virtual DbSet<Data_GameFreeRule_List> Data_GameFreeRule_List { get; set; }
        public virtual DbSet<Data_Push_Rule> Data_Push_Rule { get; set; }
        public virtual DbSet<Data_Food_Level> Data_Food_Level { get; set; }
        public virtual DbSet<Data_CouponRight> Data_CouponRight { get; set; }
        public virtual DbSet<Data_Jackpot_Level> Data_Jackpot_Level { get; set; }
        public virtual DbSet<Data_Jackpot_Matrix> Data_Jackpot_Matrix { get; set; }
        public virtual DbSet<Data_JackpotInfo> Data_JackpotInfo { get; set; }
        public virtual DbSet<Data_Head> Data_Head { get; set; }
        public virtual DbSet<Data_GameInfo> Data_GameInfo { get; set; }
        public virtual DbSet<Data_Reload> Data_Reload { get; set; }
        public virtual DbSet<Base_StorageInfo> Base_StorageInfo { get; set; }
        public virtual DbSet<Data_Storage_Record> Data_Storage_Record { get; set; }
        public virtual DbSet<Base_DepotInfo> Base_DepotInfo { get; set; }
        public virtual DbSet<Data_BalanceType_StoreList> Data_BalanceType_StoreList { get; set; }
        public virtual DbSet<Data_CouponInfo> Data_CouponInfo { get; set; }
        public virtual DbSet<Data_CouponList> Data_CouponList { get; set; }
        public virtual DbSet<Data_Food_StoreList> Data_Food_StoreList { get; set; }
        public virtual DbSet<Data_Food_WorkStation> Data_Food_WorkStation { get; set; }
        public virtual DbSet<Data_Game_StockInfo> Data_Game_StockInfo { get; set; }
        public virtual DbSet<Data_GameInfo_Ext> Data_GameInfo_Ext { get; set; }
        public virtual DbSet<Data_GameInfo_Photo> Data_GameInfo_Photo { get; set; }
        public virtual DbSet<Data_GoodsStock> Data_GoodsStock { get; set; }
        public virtual DbSet<Dict_BalanceType> Dict_BalanceType { get; set; }
        public virtual DbSet<Flw_Game_Watch> Flw_Game_Watch { get; set; }
        public virtual DbSet<Flw_Game_WinPrize> Flw_Game_WinPrize { get; set; }
        public virtual DbSet<Data_GoodStock_Record> Data_GoodStock_Record { get; set; }
        public virtual DbSet<Flw_CouponUse> Flw_CouponUse { get; set; }
        public virtual DbSet<Base_GoodsInfo> Base_GoodsInfo { get; set; }
        public virtual DbSet<Data_FoodInfo> Data_FoodInfo { get; set; }
        public virtual DbSet<Data_ProjectInfo> Data_ProjectInfo { get; set; }
        public virtual DbSet<Data_Coupon_StoreList> Data_Coupon_StoreList { get; set; }
        public virtual DbSet<Base_UserInfo> Base_UserInfo { get; set; }
    }
}
