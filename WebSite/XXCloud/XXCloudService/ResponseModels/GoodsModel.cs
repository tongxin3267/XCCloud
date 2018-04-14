using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.Base;
using System.Runtime.Serialization;
using XCCloudService.Model;

namespace XCCloudService.ResponseModels
{
    #region "商品信息查询模式"

    /// <summary>
    /// 商品信息模式
    /// </summary>
    [DataContract]
    public class GoodsInfoPageResponseModel
    {
        public GoodsInfoPageResponseModel(List<GoodsInfoResponseModel> goodsDetail, Page page)
        {
            this.GoodsDetail = goodsDetail;
            this.GoodsDetailPage = page;
        }
        /// <summary>
        /// 商品信息明细
        /// </summary>
        [DataMember(Name = "detail", Order = 2)]
        public List<GoodsInfoResponseModel> GoodsDetail { set; get; }

        /// <summary>
        /// 分页信息对象
        /// </summary>
        [DataMember(Name = "page", Order = 2)]
        public Page GoodsDetailPage { set; get; }
    }

    [DataContract]
    public class GoodsInfoResponseModel
    {
        [DataMember(Name = "barcode", Order = 1)]
        public string Barcode { get; set; }

        [DataMember(Name = "goodsName", Order = 2)]
        public string GoodsName { get; set; }

        [DataMember(Name = "price", Order = 3)]
        public decimal Price { get; set; }

        [DataMember(Name = "cost", Order = 4)]
        public decimal Cost { get; set; }

        [DataMember(Name = "units", Order = 5)]
        public string Units { get; set; }

        [DataMember(Name = "quantity", Order = 6)]
        public int Quantity { get; set; }

        [DataMember(Name = "allowPointFee", Order = 7)]
        public int AllowPointFee { get; set; }

        [DataMember(Name = "point", Order = 8)]
        public int Point { get; set; }

        [DataMember(Name = "allowCoinFee", Order = 9)]
        public int AllowCoinFee { get; set; }

        [DataMember(Name = "coin", Order = 10)]
        public int Coin { get; set; }

        [DataMember(Name = "note", Order = 11)]
        public string Note { get; set; }

        [DataMember(Name = "userID", Order = 12)]
        public int UserID { get; set; }

        [DataMember(Name = "updateTime", Order = 13)]
        public string UpdateTime { get; set; }

        [DataMember(Name = "state", Order = 14)]
        public string State { get; set; }

        [DataMember(Name = "allowLottery", Order = 15)]
        public int AllowLottery { get; set; }

        [DataMember(Name = "lottery", Order = 16)]
        public int Lottery { get; set; }

        [DataMember(Name = "windowID", Order = 17)]
        public int WindowID { get; set; }

        [DataMember(Name = "allowCashFee", Order = 18)]
        public int AllowCashFee { get; set; }

    }

    #endregion
}