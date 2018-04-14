using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.CacheService;
using XCCloudService.Model.CustomModel.Common;
using XCCloudService.Model.XCGameManager;

namespace XCCloudService.Business.XCGameMana
{
    public class MPOrderBusiness
    {
        public static void AddTCPAnswerOrder(string orderNo,string mobile,int coins,string action,string iccardId,string storeId)
        {
            TCPAnswerOrderModel model = new TCPAnswerOrderModel(orderNo, mobile, coins, action, iccardId,storeId);
            TCPAnswerOrderCache.Add(orderNo, model);
        }

        public static bool ExistTCPAnswerOrder(string orderNo,ref TCPAnswerOrderModel model)
        {
            var obj = TCPAnswerOrderCache.GetValue(orderNo);
            if (obj == null)
            {
                model = null;
                return false;
            }
            else
            {
                model = (TCPAnswerOrderModel)obj;
                return true;
            }
        }

        public static void RemoveTCPAnswerOrder(string orderNo)
        {
            TCPAnswerOrderCache.Remove(orderNo);
        }


        /// <summary>
        /// 获取订单号
        /// </summary>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        public static string GetOrderNo(string storeId, decimal price, decimal fee, int orderType, string productName, string mobile, string buyType, int coins)
        {
            string sql = "exec InsertMPOrder @StoreId,@Price,@Fee,@OrderType,@ProductName,@Mobile,@BuyType,@Coins ";
            SqlParameter[] parameters = new SqlParameter[8];
            parameters[0] = new SqlParameter("@StoreId", storeId);
            parameters[1] = new SqlParameter("@Price", price);
            parameters[2] = new SqlParameter("@Fee", fee);
            parameters[3] = new SqlParameter("@OrderType", orderType);
            parameters[4] = new SqlParameter("@ProductName", productName);
            parameters[5] = new SqlParameter("@Mobile", mobile);
            parameters[6] = new SqlParameter("@BuyType", buyType);
            parameters[7] = new SqlParameter("@Coins", coins);
            System.Data.DataSet ds = XCGameManabll.ExecuteQuerySentence(sql, parameters);
            return ds.Tables[0].Rows[0][0].ToString();
        }

        public static bool UpdateOrderForPaySuccess(string orderId,string tradeNo)
        {
            string sql = "exec UpdateOrderForPaySuccess @OrderId,@TradeNo";
            SqlParameter[] parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("@OrderId", orderId);
            parameters[1] = new SqlParameter("@TradeNo", tradeNo);
            System.Data.DataSet ds = XCGameManabll.ExecuteQuerySentence(sql, parameters);
            int result = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
            if (result == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Model.XCGameManager.Data_Order> GetOrderList()
        {
            BLL.IBLL.XCGameManager.IDataOrderService orderService = BLLContainer.Resolve<BLL.IBLL.XCGameManager.IDataOrderService>();
            var orderList = orderService.GetModels(d => true);
            return orderList;
        }

        /// <summary>
        /// 根据订单号获取订单实体
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Model.XCGameManager.Data_Order GetOrderModel(string orderId)
        {
            return GetOrderList().FirstOrDefault(m => m.OrderID.Equals(orderId));
        }
    }
}
