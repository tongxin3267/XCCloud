using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using XCCloudService.Base;

namespace XCCloudService.DBService.Model
{
    #region "代币入库"
    /// <summary>
    /// 代币入库模式
    /// </summary>
    [DataContract]
    public class CoinStoragePageModel
    {
        public CoinStoragePageModel(List<CoinStorageModel> coinStorageDetail, Page coinStorageDetailPage)
        {
            this.CoinStorageDetail = coinStorageDetail;
            this.CoinStorageDetailPage = coinStorageDetailPage;
        }

        /// <summary>
        /// 代币销售明细模式
        /// </summary>
        [DataMember(Name = "detail", Order = 1)]
        public List<CoinStorageModel> CoinStorageDetail { set; get; }

        /// <summary>
        /// 代币销售分页
        /// </summary>
        [DataMember(Name = "page", Order = 2)]
        public Page CoinStorageDetailPage { set; get; }
    }

        /// <summary>
        /// 代币入库明细模式
        /// </summary>
        [DataContract]
        public class CoinStorageModel
        {
            /// <summary>
            /// 入库时间
            /// </summary>
            [DataMember(Name = "storagetime", Order = 1)]
            public DateTime StorageTime { set; get; }

            /// <summary>
            /// 入库仓库
            /// </summary>
            [DataMember(Name = "storename", Order = 2)]
            public DateTime StoreName { set; get; }

            /// <summary>
            /// 经办人
            /// </summary>
            [DataMember(Name = "username", Order = 3)]
            public string UserName { set; get; }

            /// <summary>
            /// 入库数量
            /// </summary>
            [DataMember(Name = "storagecount", Order = 4)]
            public int StorageCount { set; get; }

            /// <summary>
            /// 备注
            /// </summary>
            [DataMember(Name = "note", Order = 5)]
            public string Note { set; get; }

        }

    #endregion


    #region "代币销毁"

        /// <summary>
        /// 代币销毁模式
        /// </summary>
        [DataContract]
        public class CoinDestroyPageModel
        {
            public CoinDestroyPageModel(List<CoinDestroyModel> coinDestroyDetail, Page coinStorageDetailPage)
            {
                this.CoinDestroyDetail = coinDestroyDetail;
                this.CoinDestroyDetailPage = coinStorageDetailPage;
            }

            /// <summary>
            /// 代币销售明细模式
            /// </summary>
            [DataMember(Name = "detail", Order = 1)]
            public List<CoinDestroyModel> CoinDestroyDetail { set; get; }

            /// <summary>
            /// 代币销售分页
            /// </summary>
            [DataMember(Name = "page", Order = 2)]
            public Page CoinDestroyDetailPage { set; get; }
        }

        /// <summary>
        /// 代币销毁明细模式
        /// </summary>
        [DataContract]
        public class CoinDestroyModel
        {
            /// <summary>
            /// 销毁时间
            /// </summary>
            [DataMember(Name = "destroytime", Order = 1)]
            public DateTime DestroyTime { set; get; }

            /// <summary>
            /// 入库仓库
            /// </summary>
            [DataMember(Name = "storename", Order = 2)]
            public string StoreName { set; get; }

            /// <summary>
            /// 经办人
            /// </summary>
            [DataMember(Name = "username", Order = 3)]
            public string UserName { set; get; }

            /// <summary>
            /// 销毁数量
            /// </summary>d
            [DataMember(Name = "destroycount", Order = 4)]
            public int DestroyCount { set; get; }

            /// <summary>
            /// 备注
            /// </summary>
            [DataMember(Name = "note", Order = 5)]
            public string Note { set; get; }

        }

    #endregion


    #region "代币销售模式"

        /// <summary>
        /// 代币销售明细
        /// </summary>
        [DataContract]
        public class CoinSalesDetailModel
        {
            /// <summary>
            /// 销售时间
            /// </summary>
            [DataMember(Name = "realtime", Order = 1)]
            public string RealTime { set; get; }

            /// <summary>
            /// 卡号
            /// </summary>
            [DataMember(Name = "iccardid", Order = 2)]
            public string ICCardID { set; get; }

            /// <summary>
            /// 售币数量
            /// </summary>
            [DataMember(Name = "coins", Order = 3)]
            public int Coins { set; get; }

            /// <summary>
            /// 销毁数量
            /// </summary>
            [DataMember(Name = "workstation", Order = 4)]
            public string WorkStation { set; get; }

            /// <summary>
            /// 销毁数量
            /// </summary>d
            [DataMember(Name = "worktypename", Order = 5)]
            public string WorkTypeName { set; get; }

            /// <summary>
            /// 备注
            /// </summary>
            [DataMember(Name = "note", Order = 6)]
            public string Note { set; get; }
        }

        /// <summary>
        /// 代币销售设备汇总
        /// </summary>
        [DataContract]
        public class CoinSalesDeviceSummaryModel
        {
            /// <summary>
            /// 售币设备
            /// </summary>
            [DataMember(Name = "devicename", Order = 1)]
            public string WorkStation { set; get; }

            /// <summary>
            /// 售币总数
            /// </summary>
            [DataMember(Name = "cointotalcount", Order = 2)]
            public int Coins { set; get; }
        }

        /// <summary>
        /// 代币销售模式
        /// </summary>
        [DataContract]
        public class CoinSalesModel
        {
            public CoinSalesModel(int totalCount, List<CoinSalesDetailModel> salesDetail, List<CoinSalesDeviceSummaryModel> salesDeviceSummaryDetail, Page salesDetailPage)
            {
                this.CoinTotalCount = totalCount;
                this.SalesDetail = salesDetail;
                this.SalesDeviceSummaryDetail = salesDeviceSummaryDetail;
                this.SalesDetailPage = salesDetailPage;
            }

            /// <summary>
            /// 总币数
            /// </summary>
            [DataMember(Name = "cointotalcount", Order = 1)]
            public int CoinTotalCount { set; get; }

            /// <summary>
            /// 代币销售明细
            /// </summary>
            [DataMember(Name = "saledetail", Order = 2)]
            public List<CoinSalesDetailModel> SalesDetail { set; get; }

            /// <summary>
            /// 代币销售明细
            /// </summary>
            [DataMember(Name = "saledetailpage", Order = 3)]
            public Page SalesDetailPage { set; get; }

            /// <summary>
            /// 代币销售设备汇总明细
            /// </summary>
            [DataMember(Name = "devicesummarydetail", Order = 4)]
            public List<CoinSalesDeviceSummaryModel> SalesDeviceSummaryDetail { set; get; }
        }

    #endregion
}