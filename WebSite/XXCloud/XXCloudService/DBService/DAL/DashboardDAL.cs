using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XCCloudService.DBService.Model;
using XCCloudService.DBService.SQLDAL;
using System.Data;
using XCCloudService.Common;

namespace XCCloudService.DBService.DAL
{
    public class DashboardDAL
    {
        /// <summary>
        /// 今日营收总览,获取上一个营业日期的现金数与当前营业日期的现金数进行环比查询
        /// </summary>
        /// <returns></returns>
        public static MOMModel GetMOM()
        {
            string sql = @"select v1.search,v1.total,v2.lasttotal from 
                        (select '环比' as search,SUM(ysxj+hlwfk) as total from view_schedule_all where CheckDate is null) v1,
                        (select '环比' as search,CheckDate,SUM(ysxj+hlwfk) as lasttotal from view_schedule_all where checkdate=(select MAX(checkdate) as checkdate from flw_checkdate) group by CheckDate) v2
                        where v1.search=v2.search";
            DataAccess ac = new DataAccess(DataAccessDB.XCGameDB);
            DataTable dt = ac.ExecuteQuery(sql).Tables[0];
            List<MOMModel> list = Utils.GetModelList<MOMModel>(dt);
            return list[0];
        }

        /// <summary>
        /// 今日营收分析
        /// </summary>
        /// <returns></returns>
        public static TodayRevenueModel GetTodayRevenue()
        {
            string sql = @"select 
                        (select isnull(SUM(total_money),0) as food_total from dbo.view_food_total where CheckDate = 'null') as foodtotal,
                        (select isnull(SUM(Price * quantity),0) as good_total from dbo.view_goods_total where CheckDate = 'null') as goodtotal,
                        (select isnull(SUM(total_money),0) as digite_total from view_digite_total where CheckDate = 'null') as digitetotal";
            DataAccess ac = new DataAccess(DataAccessDB.XCGameDB);
            DataTable dt = ac.ExecuteQuery(sql).Tables[0];
            List<TodayRevenueModel> list = Utils.GetModelList<TodayRevenueModel>(dt);
            return list[0];
        }

        /// <summary>
        /// 今日游戏机排行榜
        /// </summary>
        /// <returns></returns>
        public static List<TodayGameRevenueModel> GetTodayGameRevenue()
        {
            string sql = @" declare @StartTime datetime
                            declare @EndTime datetime
                            select @StartTime = StartTime,@EndTime = EndTime from 
                            (
                            select CheckDate,StartTime,EndTime from flw_checkdate where CheckDate BETWEEN '2017-09-16' and '2017-09-16' 
                            UNION ALL 
                            SELECT null as CheckDate,DATEADD(second,1,(select isnull(max(EndTime),'2015/01/01 0:00:00') from flw_checkdate)) as StartTime,getdate() as EndTime
                            ) a


                            select f.gameId,ISNULL(GameName,'') as gamename,totalcount from 
                            (
	                            select isnull(c.GameID,d.GameID) as gameId,
		                              (isnull(in_ele,0) + isnull(in_ic,0) + isnull(in_real,0) + isnull(free_coin,0) - isnull(out_ic,0) - isnull(out_ticket,0) - isnull(out_real,0)) AS totalcount 
	                            from 
	                            (
		                            select isnull(a.GameID,b.GameID) as GameID,isnull(in_real,0) as in_real,isnull(in_ic,0) as in_ic,isnull(out_real,0) as out_real,  
		                            isnull(out_ic,0) as out_ic,isnull(in_ele,0) as in_ele,ISNULL(out_ticket,0) as out_ticket 
		                            from 
		                            (
			                            select g.GameID,
			                            sum( isnull( CASE WHEN CoinType = '0' THEN Coins END, 0 )) AS in_real, 
			                            sum( isnull( CASE WHEN CoinType = '1' THEN Coins END, 0 )) AS in_ic, 
			                            sum( isnull( CASE WHEN CoinType = '2' THEN Coins END, 0 )) AS out_real, 
			                            sum( isnull( CASE WHEN CoinType = '3' THEN Coins END, 0 )) AS out_ic, 
			                            sum( isnull( CASE WHEN CoinType = '4' THEN Coins END, 0 )) AS in_ele 
			                            from flw_485_coin f,t_head h,t_game g 
			                            where f.Segment = h.Segment and f.HeadAddress = h.HeadAddress and h.GameID=g.GameID 
			                            and f.RealTime between @StartTime and GETDATE()
			                            group by g.GameID
		                            ) a
		                            full join
		                            (
			                            SELECT g.GameID,sum(isnull(f.Coins, 0)) AS out_ticket 
			                            FROM flw_ticket_exit f,t_head h,t_game g
			                            where f.Segment = h.Segment and f.HeadAddress = h.HeadAddress and h.GameID=g.GameID 
			                            and f.RealTime between @StartTime and GETDATE()
			                            GROUP BY g.GameID
		                            ) b
		                            on a.GameID = b.GameID 
	                            ) c 
	                            full join 
	                            (
		                            SELECT g.GameID,sum(isnull(f.FreeCoin, 0)) AS free_coin 
		                            FROM flw_game_free f,t_head h,t_game g
		                            where f.HeadID = h.HeadID and h.GameID=g.GameID 
		                            and f.RealTime between @StartTime and GETDATE()
		                            GROUP BY g.GameID
	                            ) d
	                            on c.GameID = d.GameID
                            ) e
                            left join t_game f
                            on e.gameId = f.GameID";
            DataAccess ac = new DataAccess(DataAccessDB.XCGameDB);
            DataTable dt = ac.ExecuteQuery(sql).Tables[0];
            List<TodayGameRevenueModel> list = Utils.GetModelList<TodayGameRevenueModel>(dt);
            return list;
        }

        /// <summary>
        /// 今日吧台营收
        /// </summary>
        /// <returns></returns>
        public static List<TodayBarCounterRevenueModel> GetTodayBarCounterRevenue()
        {
            string sql = @"select ISNULL(i.WorkStation,j.WorkStation) as WorkStation,ISNULL(i.TotalMoney,0) - ISNULL(j.TotalMoney,0) as TotalMoney from 
                            (
	                            select ISNULL(g.WorkStation,h.WorkStation) as WorkStation,ISNULL(g.TotalMoney,0) - ISNULL(h.TotalMoney,0) as TotalMoney from 
	                            (
		                            select ISNULL(e.WorkStation,f.WorkStation) as WorkStation,ISNULL(e.TotalMoney,0) + ISNULL(f.TotalMoney,0) as TotalMoney from 
		                            (
			                            select ISNULL(c.WorkStation,d.WorkStation) as WorkStation,ISNULL(c.TotalMoney,0) + ISNULL(d.TotalMoney,0) as TotalMoney from 
			                            (
				                            select ISNULL(a.WorkStation,b.WorkStation) as WorkStation,ISNULL(a.TotalMoney,0) + ISNULL(b.TotalMoney,0) as TotalMoney from 
				                            (
					                            SELECT f.WorkStation,SUM(TotalMoney) AS TotalMoney
					                            FROM  dbo.flw_food_sale AS f LEFT OUTER JOIN
						                              dbo.flw_schedule AS s ON f.ScheduleID = s.ID LEFT OUTER JOIN
						                              dbo.flw_checkdate_schedule AS slist ON s.ID = slist.ScheduleID LEFT OUTER JOIN
						                              dbo.flw_checkdate AS cd ON slist.CheckID = cd.ID
					                            WHERE  (f.FlowType IN ('0', '1')) and cd.CheckDate is null
					                            GROUP BY f.WorkStation
				                            ) a
				                            full join 
				                            (
					                            SELECT f.WorkStation,SUM(TotalMoney) AS TotalMoney
					                            FROM  dbo.flw_digite_coin AS f LEFT OUTER JOIN
						                              dbo.flw_schedule AS s ON f.ScheduleID = s.ID LEFT OUTER JOIN
						                              dbo.flw_checkdate_schedule AS slist ON s.ID = slist.ScheduleID LEFT OUTER JOIN
						                              dbo.flw_checkdate AS cd ON slist.CheckID = cd.ID
					                            WHERE cd.CheckDate is null
					                            GROUP BY f.WorkStation
				                            ) b
				                            on a.WorkStation = b.WorkStation
			                            ) c
			                            full join 
			                            (
				                            SELECT f.WorkStation,SUM(isnull(GoodsMoney,0)) AS TotalMoney
				                            FROM  dbo.flw_goods AS f LEFT OUTER JOIN
					                              dbo.flw_schedule AS s ON f.ScheduleID = s.ID LEFT OUTER JOIN
					                              dbo.flw_checkdate_schedule AS slist ON s.ID = slist.ScheduleID LEFT OUTER JOIN
					                              dbo.flw_checkdate AS cd ON slist.CheckID = cd.ID
				                            WHERE cd.CheckDate is null
				                            GROUP BY f.WorkStation
			                            ) d
			                            on c.WorkStation = d.WorkStation
		                            ) e
		                            full join 
		                            (
			                            SELECT f.WorkStation,SUM(isnull(Cash,0)) AS TotalMoney
			                            FROM  dbo.flw_food_ticket_sale AS f LEFT OUTER JOIN
				                              dbo.flw_schedule AS s ON f.ScheduleID = s.ID LEFT OUTER JOIN
				                              dbo.flw_checkdate_schedule AS slist ON s.ID = slist.ScheduleID LEFT OUTER JOIN
				                              dbo.flw_checkdate AS cd ON slist.CheckID = cd.ID
			                            WHERE cd.CheckDate is null
			                            GROUP BY f.WorkStation
		                            ) f
		                            on e.WorkStation = f.WorkStation
	                            ) g
	                            full join 
	                            (
		                            SELECT f.WorkStation,SUM(isnull(CoinMoney,0)) AS TotalMoney
		                            FROM  dbo.flw_ticket_exit AS f LEFT OUTER JOIN
			                              dbo.flw_schedule AS s ON f.ScheduleID = s.ID LEFT OUTER JOIN
			                              dbo.flw_checkdate_schedule AS slist ON s.ID = slist.ScheduleID LEFT OUTER JOIN
			                              dbo.flw_checkdate AS cd ON slist.CheckID = cd.ID
		                            WHERE cd.CheckDate is null
		                            GROUP BY f.WorkStation
	                            ) h
	                            on g.WorkStation = h.WorkStation
                            ) i
                            full join 
                            (
                                SELECT f.WorkStation,SUM(isnull(CoinMoney,0)) AS TotalMoney
	                            FROM  dbo.flw_coin_exit AS f LEFT OUTER JOIN
		                              dbo.flw_schedule AS s ON f.ScheduleID = s.ID LEFT OUTER JOIN
		                              dbo.flw_checkdate_schedule AS slist ON s.ID = slist.ScheduleID LEFT OUTER JOIN
		                              dbo.flw_checkdate AS cd ON slist.CheckID = cd.ID
	                            WHERE cd.CheckDate is null
	                            GROUP BY f.WorkStation
                            ) j
                            on i.WorkStation = j.WorkStation";
            DataAccess ac = new DataAccess(DataAccessDB.XCGameDB);
            DataTable dt = ac.ExecuteQuery(sql).Tables[0];
            List<TodayBarCounterRevenueModel> list = Utils.GetModelList<TodayBarCounterRevenueModel>(dt);
            return list;
        }

        /// <summary>
        /// 客流量分析
        /// </summary>
        /// <returns></returns>
        public static List<PassengerFlowModel> GetTodayPassengerFlow()
        {
            string sql = @" select cast(HourField as varchar) as HourField,COUNT(0) as PassengerCount from 
                         (
	                        select distinct DATEPART(HOUR,RealTime) as HourField,ICCardID from flw_485_coin where datediff(dy,RealTime,'2017-08-23 19:47:47.000') = 0 and CoinType = 1
                         ) a
                         group by HourField
                         order by HourField";
            DataAccess ac = new DataAccess(DataAccessDB.XCGameDB);
            DataTable dt = ac.ExecuteQuery(sql).Tables[0];
            List<PassengerFlowModel> list = Utils.GetModelList<PassengerFlowModel>(dt);
            return list;
        }
    }
}