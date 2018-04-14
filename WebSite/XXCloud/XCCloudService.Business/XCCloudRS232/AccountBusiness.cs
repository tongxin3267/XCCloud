using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.CommonBLL;

namespace XCCloudService.Business.XCCloudRS232
{
    public class AccountBusiness
    {
        #region 统计商户营收总览
        /// <summary>
        /// 统计商户营收总览
        /// </summary>
        /// <returns></returns>
        public static DataSet GetMerchRevenue(int merchId, Nullable<int> routerId = null, string sDate = null, string eDate = null)
        {
            string strRouterWhere = string.Empty, strParentWhere= string.Empty;
            if (routerId != null)
            {
                strRouterWhere += string.Format(" AND ID = {0}", routerId);
                strParentWhere += string.Format(" AND ParentID = {0}", routerId);
            }

            string strWhere = string.Empty;

            if (!string.IsNullOrWhiteSpace(sDate) && string.IsNullOrWhiteSpace(eDate))
            {
                strWhere = string.Format(" AND RealTime >= '{0}'", sDate);
            }
            else if (string.IsNullOrWhiteSpace(sDate) && !string.IsNullOrWhiteSpace(eDate))
            {
                strWhere = string.Format(" AND RealTime <= '{0}'", eDate);
            }
            else if (!string.IsNullOrWhiteSpace(sDate) && !string.IsNullOrWhiteSpace(eDate))
            {
                strWhere = string.Format(" AND RealTime BETWEEN '{0}' AND '{1}'", sDate, eDate);
            }
            else
            {
                sDate = DateTime.Now.ToString("yyyy-MM-dd");
                eDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                strWhere = string.Format(" AND RealTime BETWEEN '{0}' AND '{1}'", sDate, eDate);
            }

            #region Sql语句
            string strSql = @"
                            WITH foodSale
                            AS
                            (
                                SELECT FoodID, MerchID, DeviceID, RealTime,
                                SUM(CASE FlowType WHEN 0 THEN PayTotal END) AS RechargeAmount,
                                SUM(CASE FlowType WHEN 1 THEN PayTotal  END) AS SaleAmount,
                                SUM(CASE PayType WHEN 0 THEN PayTotal END) AS CashPayAmount,
                                SUM(CASE PayType WHEN 1 THEN PayTotal END) AS WechatPayAmount,
                                SUM(CASE PayType WHEN 2 THEN PayTotal END) AS AliPayAmount,
                                SUM(CASE PayType WHEN 3 THEN PayTotal END) AS UnionPayAmount
                                FROM flw_food_sale
                                WHERE MerchID = {0} {3}
                                GROUP BY FoodID, MerchID, DeviceID, RealTime
                            ),
                            Foods
                            AS
                            (
                                SELECT FoodID FROM t_foods WHERE MerchID = {0}
                            ),
                            MerchInfo
                            AS
                            (
                                --商户
                                SELECT ID, MerchName, Token FROM Base_MerchInfo
                                WHERE ID = {0}
                            )
                            ,
                            Router
                            AS
                            (
                                --控制器
                                SELECT ID, MerchID FROM Base_DeviceInfo
                                WHERE DeviceType = 0 {1}
                            ),
                            Devices AS
                            (
                                --商户绑定的设备
                                SELECT ID FROM Base_DeviceInfo
                                WHERE MerchID = {0}
                            )
                            SELECT mi.MerchName, mi.Token, SUM(fs.RechargeAmount) AS RechargeAmount, SUM(fs.SaleAmount) AS SaleAmount, 
                            SUM(fs.CashPayAmount) AS CashPayAmount, SUM(fs.WechatPayAmount) AS WechatPayAmount, 
                            SUM(fs.AliPayAmount) AS AliPayAmount, SUM(fs.UnionPayAmount) AS UnionPayAmount, 
                            SUM(ISNULL(fs.RechargeAmount,0) + ISNULL(fs.SaleAmount,0)) AS Amount
                            FROM foodSale fs
                            INNER JOIN Foods f ON f.FoodID = fs.FoodID
                            INNER JOIN MerchInfo mi ON mi.ID = fs.MerchID
                            INNER JOIN Router r ON r.MerchID = mi.ID
                            INNER JOIN Devices d ON d.ID = fs.DeviceID
                            GROUP BY mi.MerchName, mi.Token
                            ;
                            --存币
                            SELECT ISNULL(SUM(Coins), 0) AS SaveCoins
                            FROM flw_485_savecoin a
                            INNER JOIN Data_MerchDevice b ON b.DeviceID = a.DeviceID
                            WHERE MerchID = {0} {2} {3}
                            ;
                            --提币
                            SELECT ISNULL(SUM(fcs.Coins), 0) AS SaleCoins
                            FROM flw_coin_sale fcs
                            INNER JOIN Data_MerchDevice b ON b.DeviceID = fcs.DeviceID
                            WHERE fcs.WorkType IN (3,6,7) AND MerchID = {0} {2} {3}
                            "; 
            #endregion
            strSql = string.Format(strSql, merchId, strRouterWhere, strParentWhere, strWhere);

            DataSet ds = XCCloudRS232BLL.ExecuterSqlToDataSet(strSql);
            return ds;
        } 
        #endregion

        #region 按控制器查询外设游戏币营收
        /// <summary>
        /// 按控制器查询外设游戏币营收
        /// </summary>
        /// <returns></returns>
        public static DataSet GetRoutersAmount(int merchId, Nullable<int> routerId = null, string sDate = null, string eDate = null)
        {
            string strWhere = string.Empty;

            if (!string.IsNullOrWhiteSpace(sDate) && string.IsNullOrWhiteSpace(eDate))
            {
                strWhere = string.Format(" AND RealTime >= '{0}'", sDate);
            }
            else if (string.IsNullOrWhiteSpace(sDate) && !string.IsNullOrWhiteSpace(eDate))
            {
                strWhere = string.Format(" AND RealTime <= '{0}'", eDate);
            }
            else if (!string.IsNullOrWhiteSpace(sDate) && !string.IsNullOrWhiteSpace(eDate))
            {
                strWhere = string.Format(" AND RealTime BETWEEN '{0}' AND '{1}'", sDate, eDate);
            }
            else
            {
                sDate = DateTime.Now.ToString("yyyy-MM-dd");
                eDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                strWhere = string.Format(" AND RealTime BETWEEN '{0}' AND '{1}'", sDate, eDate);
            }

            #region Sql语句
            string strSql = @"
                            --存币
                            SELECT c.DeviceName, b.HeadAddress, ISNULL(SUM(Coins), 0) AS SaveCoins
                            FROM flw_485_savecoin a
                            INNER JOIN Data_MerchDevice b ON b.DeviceID = a.DeviceID
                            INNER JOIN Base_DeviceInfo c ON c.ID = a.DeviceID
                            WHERE a.MerchID = {0} AND b.ParentID = {1} {2}
                            GROUP BY c.DeviceName, b.HeadAddress
                            ;
                            --提币
                            SELECT c.DeviceName, b.HeadAddress, ISNULL(SUM(a.Coins), 0) AS SaleCoins
                            FROM flw_coin_sale a
                            INNER JOIN Data_MerchDevice b ON b.DeviceID = a.DeviceID
                            INNER JOIN Base_DeviceInfo c ON c.ID = a.DeviceID
                            WHERE a.WorkType IN (3,6,7) AND a.MerchID = {0} AND b.ParentID = {1} {2}
                            GROUP BY c.DeviceName, b.HeadAddress
                            ;
                              "; 
            #endregion

            strSql = string.Format(strSql, merchId, routerId, strWhere);

            DataSet ds = XCCloudRS232BLL.ExecuterSqlToDataSet(strSql);
            return ds;
        }
        #endregion

        #region 统计指定存币机营收
        /// <summary>
        /// 统计指定存币机营收
        /// </summary>
        /// <returns></returns>
        public static DataSet GetSaveCoinMachineRevenue(int merchId, int deviceId, string sDate = null, string eDate = null, int pageIndex = 1, int pageSize = 10)
        {
            string strWhere = string.Empty;

            if (!string.IsNullOrWhiteSpace(sDate) && string.IsNullOrWhiteSpace(eDate))
            {
                strWhere = string.Format(" AND RealTime >= '{0}'", sDate);
            }
            else if (string.IsNullOrWhiteSpace(sDate) && !string.IsNullOrWhiteSpace(eDate))
            {
                strWhere = string.Format(" AND RealTime <= '{0}'", eDate);
            }
            else if (!string.IsNullOrWhiteSpace(sDate) && !string.IsNullOrWhiteSpace(eDate))
            {
                strWhere = string.Format(" AND RealTime BETWEEN '{0}' AND '{1}'", sDate, eDate);
            }
            else
            {
                sDate = DateTime.Now.ToString("yyyy-MM-dd");
                eDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                strWhere = string.Format(" AND RealTime BETWEEN '{0}' AND '{1}'", sDate, eDate);
            }

            #region Sql语句
            string strSql = @"
                            --存币
                            SELECT c.DeviceName, b.HeadAddress, ISNULL(SUM(Coins), 0) AS SaveCoins
                            FROM flw_485_savecoin a
                            INNER JOIN Data_MerchDevice b ON b.DeviceID = a.DeviceID
                            INNER JOIN Base_DeviceInfo c ON c.ID = a.DeviceID
                            WHERE a.MerchID = {0} AND a.DeviceID = {1} {2}
                            GROUP BY c.DeviceName, b.HeadAddress
                            ;
                            SELECT ICCardID, MemberName, Mobile, '存币' AS FlowType, Coins, Balance, CONVERT(varchar(100), RealTime, 120) AS RealTime
                            FROM 
                            (
                                SELECT  ROW_NUMBER() OVER (ORDER BY RealTime DESC) rownumber , f.ICCardID, m.MemberName, m.Mobile, f.Coins, f.Balance, f.RealTime
                                FROM flw_485_savecoin f
	                            INNER JOIN t_member m ON m.ICCardID = f.ICCardID
	                            WHERE f.MerchID = {0} {2}
                            ) AS a
                            WHERE rownumber BETWEEN {3} AND {4}
                            ";
            #endregion

            strSql = string.Format(strSql, merchId, deviceId, strWhere, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);

            DataSet ds = XCCloudRS232BLL.ExecuterSqlToDataSet(strSql);
            return ds;
        }
        #endregion

        #region 统计指定售币机营收
        /// <summary>
        /// 统计指定售币机营收
        /// </summary>
        /// <returns></returns>
        public static DataSet GetSaleCoinMachineRevenue(int merchId, int deviceId, string sDate = null, string eDate = null, int pageIndex = 1, int pageSize = 10)
        {
            string strWhere = string.Empty, strSaleWhere = string.Empty;

            if (!string.IsNullOrWhiteSpace(sDate) && string.IsNullOrWhiteSpace(eDate))
            {
                strWhere = string.Format(" AND RealTime >= '{0}'", sDate);
                strSaleWhere = string.Format(" AND RealTime >= '{0}'", sDate);
            }
            else if (string.IsNullOrWhiteSpace(sDate) && !string.IsNullOrWhiteSpace(eDate))
            {
                strWhere = string.Format(" AND RealTime <= '{0}'", eDate);
                strSaleWhere = string.Format(" AND RealTime <= '{0}'", eDate);
            }
            else if (!string.IsNullOrWhiteSpace(sDate) && !string.IsNullOrWhiteSpace(eDate))
            {
                strWhere = string.Format(" AND RealTime BETWEEN '{0}' AND '{1}'", sDate, eDate);
                strSaleWhere = string.Format(" AND RealTime BETWEEN '{0}' AND '{1}'", sDate, eDate);
            }
            else
            {
                sDate = DateTime.Now.ToString("yyyy-MM-dd");
                eDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                strWhere = string.Format(" AND RealTime BETWEEN '{0}' AND '{1}'", sDate, eDate);
                strSaleWhere = string.Format(" AND RealTime BETWEEN '{0}' AND '{1}'", sDate, eDate);
            }

            #region Sql语句
            string strSql = @"
                            WITH SaleCoinAmount AS 
                            (
	                            --售币
	                            SELECT f.DeviceID, SUM(CoinQuantity) AS SaleCoins, SUM(TotalMoney) AS SaleCoinAmount
	                            FROM flw_food_sale f
	                            INNER JOIN Base_DeviceInfo d ON d.ID = f.DeviceID
	                            WHERE FlowType = 1 AND PayState = 1 AND f.MerchID = {0} AND DeviceID = {1} {3}
	                            GROUP BY f.DeviceID
                            ),
                            PullCoins AS 
                            (
	                            --提币
	                            SELECT f.DeviceID, ISNULL(SUM(f.Coins), 0) AS PullCoins
	                            FROM flw_coin_sale f
	                            INNER JOIN Base_DeviceInfo d ON d.ID = f.DeviceID
	                            WHERE f.WorkType IN (3,6,7) AND f.MerchID = {0} AND f.DeviceID = {1} {2}
	                            GROUP BY f.DeviceID
                            )
                            --提币
                            SELECT s.SaleCoins, s.SaleCoinAmount, p.PullCoins
                            FROM Base_DeviceInfo d
                            LEFT JOIN SaleCoinAmount s ON d.ID = s.DeviceID
                            LEFT JOIN PullCoins p ON d.ID = p.DeviceID
                            WHERE d.ID = {1}
                            ;
                            WITH PullCoinList AS
                            (
	                            SELECT f.ICCardID, m.MemberName, m.Mobile, '提币' AS FlowType, '0' AS Amount, f.Coins, f.Balance, f.RealTime
                                FROM flw_coin_sale f
	                            INNER JOIN t_member m ON m.ICCardID = f.ICCardID
	                            WHERE f.MerchID = {0} AND f.DeviceID = {1} {2}
	                            UNION ALL 
	                            SELECT f.ICCardID,m.MemberName, m.Mobile, '售币' AS FlowType, f.TotalMoney AS Amount, f.CoinQuantity, f.Balance, f.RealTime
	                            FROM flw_food_sale f
	                            INNER JOIN t_member m ON m.ICCardID = f.ICCardID
	                            WHERE f.MerchID = {0} AND f.DeviceID = {1} {2}
                            )
                            SELECT ICCardID, MemberName, Mobile, FlowType, Amount, Coins, Balance, CONVERT(varchar(100), RealTime, 120) AS RealTime  
                            FROM 
                            (
                                SELECT  ROW_NUMBER() OVER (ORDER BY RealTime DESC) rownumber , ICCardID, MemberName, Mobile, FlowType, Amount, Coins, Balance, RealTime
                                FROM PullCoinList
                            ) AS a
                            WHERE rownumber BETWEEN {4} AND {5}
                            ;
                            ";
            #endregion

            strSql = string.Format(strSql, merchId, deviceId, strWhere, strSaleWhere, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);

            DataSet ds = XCCloudRS232BLL.ExecuterSqlToDataSet(strSql);
            return ds;
        }
        #endregion

        #region 会员数据统计
        /// <summary>
        /// 会员数据统计
        /// </summary>
        /// <returns></returns>
        public static DataSet GetMemberSummary(int merchId, string ICCardID = null, string sDate = null, string eDate = null, int pageIndex = 1, int pageSize = 10)
        {
            string strWhere = string.Empty;

            if (!string.IsNullOrWhiteSpace(ICCardID))
            {
                strWhere += string.Format(" AND ICCardID = {0}", ICCardID);
            }

            if (!string.IsNullOrWhiteSpace(sDate) && string.IsNullOrWhiteSpace(eDate))
            {
                strWhere += string.Format(" AND JoinTime >= '{0}'", sDate);
            }
            else if (string.IsNullOrWhiteSpace(sDate) && !string.IsNullOrWhiteSpace(eDate))
            {
                strWhere += string.Format(" AND JoinTime <= '{0}'", eDate);
            }
            else if (!string.IsNullOrWhiteSpace(sDate) && !string.IsNullOrWhiteSpace(eDate))
            {
                strWhere += string.Format(" AND JoinTime BETWEEN '{0}' AND '{1}'", sDate, eDate);
            }
            else
            {
                sDate = DateTime.Now.ToString("yyyy-MM-dd");
                eDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                strWhere += string.Format(" AND JoinTime BETWEEN '{0}' AND '{1}'", sDate, eDate);
            }

            #region Sql语句
            string strSql = @"
                            WITH MembersTotal AS
                            (
	                            --会员总数、币余额总数
	                            SELECT COUNT(1) AS MemberTotal, SUM(Balance)  AS BalanceTotal
	                            FROM t_member
	                            WHERE MerchID = {0}
                            ),
                            TodayJoinMembers AS
                            (
	                            --今日新增会员数
	                            SELECT COUNT(1) AS TodayJoinMembers
	                            FROM t_member
	                            WHERE MerchID = {0} AND DATEDIFF(DAY, GETDATE(), JoinTime) = 0
                            ),
                            SaveCoinMember AS
                            (
	                            --今日存币会员数
	                            SELECT DISTINCT ICCardID FROM flw_485_savecoin
	                            WHERE MerchID = {0} AND DATEDIFF(DAY, GETDATE(), RealTime) = 0
                            ),
                            SaleCoinMember AS 
                            (
	                            --今日提币会员数
	                            SELECT DISTINCT ICCardID FROM flw_coin_sale
	                            WHERE MerchID = {0} AND DATEDIFF(DAY, GETDATE(), RealTime) = 0
                            ),
                            FoodSaleMember AS 
                            (
	                            --今日购币会员数
	                            SELECT DISTINCT ICCardID FROM flw_food_sale
	                            WHERE MerchID = {0} AND DATEDIFF(DAY, GETDATE(), RealTime) = 0
                            ),
                            Members AS
                            (
	                            --今日全部游戏会员
	                            SELECT ICCardID FROM FoodSaleMember 
	                            UNION
	                            SELECT ICCardID FROM SaveCoinMember
	                            UNION 
	                            SELECT ICCardID FROM SaleCoinMember
                            ),
                            TodayGameMembers AS 
                            (
	                            --今日全部参与
	                            SELECT COUNT(1) AS TodayGameMembers FROM Members
                            )
                            SELECT ISNULL(a.MemberTotal,0) AS MemberTotal, ISNULL(a.BalanceTotal,0) AS BalanceTotal, ISNULL(b.TodayJoinMembers, 0) AS TodayJoinMembers, ISNULL(c.TodayGameMembers, 0) as TodayGameMembers
                            FROM MembersTotal a
                            LEFT JOIN TodayJoinMembers b ON 1=1
                            LEFT JOIN TodayGameMembers c ON 1=1
                            ;
                            SELECT MemberID, ICCardID, ISNULL(MemberName, Mobile) AS MemberName, Mobile, Balance, CONVERT(varchar(100), JoinTime, 120) AS JoinTime
                            FROM 
                            (
                                SELECT  ROW_NUMBER() OVER (ORDER BY JoinTime DESC) rownumber , MemberID, ICCardID, MemberName, Mobile, Balance, JoinTime
                                FROM    t_member
                                WHERE MerchID = {0} {1}
                            ) AS a
                            WHERE rownumber BETWEEN {2} AND {3}
                            ;";
            #endregion

            strSql = string.Format(strSql, merchId, strWhere, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);

            DataSet ds = XCCloudRS232BLL.ExecuterSqlToDataSet(strSql);
            return ds;
        }
        #endregion

        #region 会员详细信息
        /// <summary>
        /// 会员详细信息
        /// </summary>
        /// <returns></returns>
        public static DataTable GetMemberDetail(string memberId)
        {
            string strWhere = string.Empty;

            if (!string.IsNullOrWhiteSpace(memberId))
            {
                strWhere = string.Format("WHERE MemberID = {0}", memberId);
            }

            #region Sql语句
            string strSql = @"
                            SELECT ICCardID, MemberName, Mobile, Birthday, CertificalID, Balance, Lottery, Point, [Type], JoinTime, EndDate, MemberPassword, Note, Lock, LockDate
                            FROM t_member
                            {0}
                            ";
            #endregion

            strSql = string.Format(strSql, strWhere);

            DataTable table = XCCloudRS232BLL.ExecuterSqlToTable(strSql);
            return table;
        }
        #endregion

        #region 套餐销售流水详情分页
        /// <summary>
        /// 套餐销售流水详情分页
        /// </summary>
        /// <param name="merchId">商户ID</param>
        /// <param name="routerId">控制器ID</param>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        public static DataTable GetMemberExpenseDetail(int merchId, string icCardId, string flowType = "0", string sDate = null, string eDate = null, int pageIndex = 1, int pageSize = 10)
        {
            string strWhere = string.Empty;

            if (!string.IsNullOrWhiteSpace(sDate) && string.IsNullOrWhiteSpace(eDate))
            {
                strWhere = string.Format(" AND RealTime >= '{0}'", sDate);
            }
            else if (string.IsNullOrWhiteSpace(sDate) && !string.IsNullOrWhiteSpace(eDate))
            {
                strWhere = string.Format(" AND RealTime <= '{0}'", eDate);
            }
            else if (!string.IsNullOrWhiteSpace(sDate) && !string.IsNullOrWhiteSpace(eDate))
            {
                strWhere = string.Format(" AND RealTime BETWEEN '{0}' AND '{1}'", sDate, eDate);
            }
            else
            {
                sDate = DateTime.Now.ToString("yyyy-MM-dd");
                eDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                strWhere = string.Format(" AND RealTime BETWEEN '{0}' AND '{1}'", sDate, eDate);
            }

            string strFlow = string.Empty;
            if (!string.IsNullOrWhiteSpace(flowType) && flowType != "0")
            {
                strFlow = string.Format("AND a.FlowType = {0}", flowType);
            }

            #region Sql语句
            string strSql = @"
                            WITH MemberExpenseList AS
                            (
	                            SELECT f.ICCardID, m.MemberName, m.Mobile, '提币' AS Flow, 1 AS FlowType, '0' AS Amount, f.Coins, f.Balance, f.RealTime
                                FROM flw_coin_sale f
	                            INNER JOIN t_member m ON m.ICCardID = f.ICCardID
	                            WHERE f.MerchID = {0} AND f.ICCardID = {1} {2}
	                            UNION ALL
	                            SELECT f.ICCardID,m.MemberName,m.Mobile, '存币' AS Flow, 2 AS FlowType, '0' AS Amount, f.Coins, f.Balance, f.RealTime
	                            FROM flw_485_savecoin f
	                            INNER JOIN t_member m ON m.ICCardID = f.ICCardID
	                            WHERE f.MerchID = {0} AND f.ICCardID = {1} {2}
	                            UNION ALL 
	                            SELECT f.ICCardID,m.MemberName, m.Mobile, 
	                            CASE f.FlowType WHEN 0 THEN '充值' WHEN 1 THEN '购币' END AS Flow,
	                            CASE f.FlowType WHEN 0 THEN 3 WHEN 1 THEN 4 END AS FlowType,
	                            f.TotalMoney AS Amount, f.CoinQuantity, f.Balance, f.RealTime
	                            FROM flw_food_sale f
	                            INNER JOIN t_member m ON m.ICCardID = f.ICCardID
	                            WHERE f.PayState = 1 AND f.MerchID = {0} AND f.ICCardID = {1} {2}
                            )
                            SELECT ICCardID, MemberName, Mobile, Flow, Amount, Coins, Balance, CONVERT(varchar(100), RealTime, 120) AS RealTime  
                            FROM 
                            (
                                SELECT  ROW_NUMBER() OVER (ORDER BY RealTime DESC) rownumber , ICCardID, MemberName, Mobile, Flow, FlowType, Amount, Coins, Balance, RealTime
                                FROM MemberExpenseList
                            ) AS a
                            WHERE rownumber BETWEEN {4} AND {5} {3}
                              ";
            #endregion

            strSql = string.Format(strSql, merchId, icCardId, strWhere, strFlow, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);

            DataTable table = XCCloudRS232BLL.ExecuterSqlToTable(strSql);
            return table;
        }
        #endregion
    }
}
