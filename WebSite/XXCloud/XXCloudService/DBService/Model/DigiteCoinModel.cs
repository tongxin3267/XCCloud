using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using XCCloudService.Base;

namespace XCCloudService.DBService.Model
{

    #region "数字币销售"

        /// <summary>
        /// 数字币销售明细模式
        /// </summary>
        [DataContract]
        public class DigiteCoinSaleDetailModel
        {
            [DataMember(Name = "saletime", Order = 1)]
            public DateTime SaleTime { set; get; }

            [DataMember(Name = "foodname", Order = 2)]
            public string FoodName { set; get; }

            [DataMember(Name = "countnum", Order = 3)]
            public int CountNum { set; get; }

            [DataMember(Name = "totalmoney", Order = 4)]
            public decimal TotalMoney { set; get; }

            [DataMember(Name = "note", Order = 5)]
            public string Note { set; get; }

            [DataMember(Name = "workstation", Order = 6)]
            public string WorkStation { set; get; }
        }

        /// <summary>
        /// 数字币销售明细汇总模式
        /// </summary>
        [DataContract]
        public class DigiteCoinSaleDetailSumModel
        {
            [DataMember(Name = "foodname", Order = 1)]
            public string FoodName { set; get; }

            [DataMember(Name = "totalmoney", Order = 2)]
            public decimal TotalMoney { set; get; }
        }

        /// <summary>
        /// 数字币销售模式
        /// </summary>
        [DataContract]
        public class DigiteCoinSaleDataModel
        {
            public DigiteCoinSaleDataModel(List<DigiteCoinSaleDetailModel> detail, List<DigiteCoinSaleDetailSumModel> sumData)
            {
                this.Detail = detail;
                this.SumData = sumData;
                totalMoney = sumData.Sum(p => p.TotalMoney);
            }
            /// <summary>
            /// 数字币销售明细
            /// </summary>
            [DataMember(Name = "detail", Order = 1)]
            public List<DigiteCoinSaleDetailModel> Detail;

            /// <summary>
            /// 数字币销售明细模式
            /// </summary>
            [DataMember(Name = "sumdetail", Order = 2)]
            public List<DigiteCoinSaleDetailSumModel> SumData;

            /// <summary>
            /// 总金额
            /// </summary>
            [DataMember(Name = "totalMoney", Order = 3)]
            public decimal totalMoney { set; get; }
        }

    #endregion



    #region "数字币销毁"
        
        /// <summary>
        /// 数字币销毁模式
        /// </summary>
        [DataContract]
        public class DigiteCoinDestroyModel
        {
            public DigiteCoinDestroyModel(List<DigiteCoinDestroyDetialModel> detail, Page detailPage)
            {
                this.Detail = detail;
                this.DetailPage = detailPage;
            }

            /// <summary>
            /// 数字币销毁明细List
            /// </summary>
            [DataMember(Name = "detial", Order = 1)]
            public List<DigiteCoinDestroyDetialModel> Detail { set; get; }

            /// <summary>
            /// 数字币销毁明细分页
            /// </summary>
            [DataMember(Name = "detialpage", Order = 1)]
            public Page DetailPage { set; get; }
        }

        /// <summary>
        /// 数字币销毁明细模式
        /// </summary>
        [DataContract]
        public class DigiteCoinDestroyDetialModel
        {
            /// <summary>
            /// 销毁时间
            /// </summary>
            [DataMember(Name = "destroytime", Order = 1)]
            public DateTime DestroyTime { set; get; }

            /// <summary>
            /// 销毁币编号
            /// </summary>
            [DataMember(Name = "iccardid", Order = 2)]
            public string ICCardID { set; get; }

            /// <summary>
            /// 经办人
            /// </summary>
            [DataMember(Name = "username", Order = 3)]
            public string UserName { set; get; }

            /// <summary>
            /// 注释
            /// </summary>
            [DataMember(Name = "note", Order = 4)]
            public string Note { set; get; }
        }

    #endregion
}