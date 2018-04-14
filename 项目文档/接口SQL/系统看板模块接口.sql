--会员卡销售查询
select CheckDate,RealTime,WorkStation,ICCardID,MemberID,MemberName,MemberLevelName,MemberState,JoinTime,FlowType,FoodID,FoodName,Deposit,OpenFee,CoinQuantity,TotalMoney,RealName,Note 
from view_member_join where 1=1 order by realtime

--退卡查询
select CheckDate,RealTime,WorkStation,ICCardID,MemberLevelName,Coins,CoinMoney,Deposit,RealName,Note from view_member_exit_card where 1=1 order by realtime

--换卡查询
select CheckDate,RealTime,WorkStation,OldICCardID,oldMemberID,NewICCardID,NewMemberID,MemberName,MemberLevelName,MemberState,JoinTime,ChangeFee,RealName,Note 
from view_food_sale_change where 1=1 order by realtime

--续卡查询
select CheckDate,RealTime,WorkStation,OldICCardID,oldMemberID,NewICCardID,NewMemberID,MemberName,MemberLevelName,MemberState,JoinTime,RealName,Note 
from view_food_sale_continue where 1=1 order by realtime

--套餐销售查询
select FoodID,FoodName,ICCardID,RealTime,flow_type,FoodType,CoinQuantity,Point,TotalMoney,CouponMoney,RealName,WorkStation,CheckDate,Note 
from view_food_sale where 1=1 order by realtime

--套餐销售汇总
select CheckDate,FoodName,total_count,total_money,total_coins,coupon_money,flow_type from 
(
SELECT ISNULL(CONVERT(VARCHAR(10), cd.CheckDate,120),'null') AS CheckDate, t.FoodName AS FoodName, isnull(COUNT(f.ID), 0) AS total_count, isnull(SUM(f.TotalMoney), 0) AS total_money, 
isnull(SUM(f.CoinQuantity), 0) AS total_coins, isnull(SUM(c.CouponPrice), 0) AS coupon_money, ( CASE f.FlowType WHEN '0' THEN '入会' WHEN '1' THEN '售餐' WHEN 'X' THEN '退餐' END ) AS flow_type 
FROM flw_food_sale f LEFT JOIN flw_checkdate cd ON f.RealTime BETWEEN cd.StartTime AND cd.EndTime LEFT JOIN t_foods t ON t.FoodID = f.FoodID 
LEFT JOIN t_member_coupon c ON c.FlowID = f.ID WHERE f.FlowType != 'X' AND f.BuyFoodType='1' 
GROUP BY cd.CheckDate, t.FoodName, f.FlowType 
UNION ALL 
SELECT ISNULL(CONVERT(VARCHAR(10), cd.CheckDate,120),'null') AS CheckDate, t.FoodName AS FoodName, -isnull(COUNT(f.ID), 0) AS total_count, isnull(SUM(f.TotalMoney), 0) AS total_money, 
isnull(SUM(f.CoinQuantity), 0) AS total_coins, -isnull(SUM(c.CouponPrice), 0) AS coupon_money, ( CASE f.FlowType WHEN '0' THEN '入会' WHEN '1' THEN '售餐' WHEN 'X' THEN '退餐' END ) AS flow_type 
FROM flw_food_sale f LEFT JOIN flw_checkdate cd ON f.RealTime BETWEEN cd.StartTime AND cd.EndTime LEFT JOIN t_foods t ON t.FoodID = f.FoodID 
LEFT JOIN t_member_coupon c ON c.FlowID = f.ID WHERE f.FlowType = 'X' AND f.BuyFoodType='1' 
GROUP BY cd.CheckDate, t.FoodName, f.FlowType
) t where 1=1 order by checkdate


--商品销售汇总
select Barcode,GoodsName,Price,Quantity,AllPrice,AllCoin,AllPoint,AllLottery,CheckDate from  
(
select  ISNULL( CONVERT(varchar(10), bcinfo.CheckDate,120),'null') as CheckDate,goodslist.Barcode,goodslist.GoodsName,goodslist.Price,
SUM(goodslist.Quantity) as Quantity,SUM(goodslist.AllPrice) as AllPrice,SUM(goodslist.AllCoin) as AllCoin,
SUM(goodslist.AllPoint) as AllPoint,SUM(goodslist.AllLottery) as AllLottery from 
(select f.GoodsID,f.RealTime,f.ICCardID,f.CreditFee,f.PayType,f.ScheduleID,d.Barcode,d.GoodsName,d.Quantity,d.AllPrice,
d.AllCoin,d.AllPoint,d.AllLottery,g.Units,g.Price from flw_goods f,flw_good_detail d,(select * from t_goods where State='1') g 
where f.GoodsID=d.GoodsID and d.Barcode=g.Barcode) as goodslist 
LEFT JOIN (select s.ID,s.WorkStation,u.RealName,fc.CheckDate from flw_schedule s,u_users u,flw_checkdate fc,flw_checkdate_schedule fs 
where s.UserID=u.UserID and fc.ID=fs.CheckID and s.ID=fs.ScheduleID) bcinfo on goodslist.ScheduleID=bcinfo.ID 
GROUP BY goodslist.Barcode,goodslist.GoodsName,goodslist.Price,bcinfo.CheckDate
) v 
where 1=1 order by checkdate

--商品销售查询
select GoodsID,RealTime,GoodsName,Quantity,Price,AllPrice,ICCardID,AllCoin,AllPoint,AllLottery,Barcode,RealName,CheckDate,WorkStation from  
(
select * from (select f.GoodsID,f.RealTime,f.ICCardID,f.CreditFee,f.PayType,f.ScheduleID,d.Barcode,d.GoodsName,d.Quantity,d.AllPrice,d.AllCoin,d.AllPoint,d.AllLottery,g.Units,g.Price 
from flw_goods f,flw_good_detail d,
(select * from t_goods where State='1') g where f.GoodsID=d.GoodsID and d.Barcode=g.Barcode) as goodslist 
LEFT JOIN (select s.ID,s.WorkStation,u.RealName, ISNULL( CONVERT(varchar(10), fc.CheckDate,120),'null') as CheckDate 
from flw_schedule s,u_users u,flw_checkdate fc,flw_checkdate_schedule fs where s.UserID=u.UserID and fc.ID=fs.CheckID and s.ID=fs.ScheduleID) bcinfo on goodslist.ScheduleID=bcinfo.ID
) v where 1=1 order by realtime

--数字币销售
select ID,RealTime,CoinQuantity,CountNum,TotalMoney,FoodName,CheckDate,RealName,WorkStation,Note from view_digite_flw where 1=1 order by realtime



select flw_game_alarm.* from flw_game_alarm where CONVERT(VARCHAR(10),HappenTime,120)=CONVERT(VARCHAR(10),getdate(),120) and flw_game_alarm.state<>'2';

select CheckDate,StartTime,EndTime from flw_checkdate where CheckDate BETWEEN '2017-09-15' and '2017-09-15' UNION ALL SELECT null as CheckDate,DATEADD(second,1,(select isnull(max(EndTime),'2015/01/01 0:00:00') from flw_checkdate)) as StartTime,getdate() as EndTime 

--游戏机查询
SELECT null AS CheckDate, GameID, GameName, sum(in_ele) AS in_ele, sum(in_ic) AS in_ic, sum(in_real) AS in_real, sum(in_real) AS in_real, sum(out_ic) AS out_ic, sum(out_real) AS out_real, 
sum(out_ticket) AS out_ticket, sum(free_coin) AS free_coin, sum(all_in) AS all_in, sum(all_in_money) AS all_in_money, sum(all_out) AS all_out, sum(all_out_money) AS all_out_money,
case when sum(all_in)=0 then 0 else CONVERT(numeric(18,2),isnull(CONVERT(numeric(18,2),sum(all_out)) / CONVERT(numeric(18,2), sum(all_in)), 0)* 100) end AS out_percent, 
sum(all_win) AS all_win, sum(all_win_money) AS all_win_money FROM 
( 
SELECT v1.GameID, v1.GameName, v1.HeadID, v1.in_ele, v1.in_ic, v1.in_real, v1.out_ic, v1.out_real, v1.out_ticket, v1.free_coin, ( v1.in_ele + v1.in_ic + v1.in_real + v1.free_coin ) AS all_in, 
(( v1.in_ele + v1.in_ic + v1.in_real + v1.free_coin ) * CONVERT(numeric(18,2), dbo.get_coin_to_money())) AS all_in_money, ( v1.out_ic + v1.out_ticket + v1.out_real ) AS all_out, 
(( v1.out_ic + v1.out_ticket + v1.out_real ) * CONVERT(numeric(18,2), dbo.get_coin_to_money())) AS all_out_money,
case when v1.in_ele + v1.in_ic + v1.in_real=0 then 0 else isnull( cast((( v1.out_ic + v1.out_ticket + v1.out_real + v1.free_coin ) / ( v1.in_ele + v1.in_ic + v1.in_real )) AS DECIMAL (11, 2)), 0 ) * 100 end AS out_percent, 
( v1.in_ele + v1.in_ic + v1.in_real + v1.free_coin - v1.out_ic - v1.out_ticket - v1.out_real ) AS all_win, (( v1.in_ele + v1.in_ic + v1.in_real + v1.free_coin - v1.out_ic - v1.out_ticket - v1.out_real ) * CONVERT(numeric(18,2), 
dbo.get_coin_to_money())) AS all_win_money FROM ( SELECT g.GameID AS GameID, g.GameName AS GameName, h.HeadID AS HeadID, isnull(f1.in_real, 0) AS in_real, isnull(f1.in_ic, 0) AS in_ic, 
isnull(f1.in_ele, 0) AS in_ele, isnull(f1.out_real, 0) AS out_real, isnull(f1.out_ic, 0) AS out_ic, f2.out_ticket, f2.free_coin 
FROM t_head h LEFT JOIN 
( 
  SELECT Segment, HeadAddress, sum( isnull( CASE WHEN CoinType = '0' THEN Coins END, 0 )) AS in_real, sum( isnull( CASE WHEN CoinType = '1' THEN Coins END, 0 )) AS in_ic, 
  sum( isnull( CASE WHEN CoinType = '2' THEN Coins END, 0 )) AS out_real, sum( isnull( CASE WHEN CoinType = '3' THEN Coins END, 0 )) AS out_ic, 
  sum( isnull( CASE WHEN CoinType = '4' THEN Coins END, 0 )) AS in_ele FROM flw_485_coin WHERE 1 = 1 AND RealTime BETWEEN  '2017-09-06 00:07' and '2017-09-15 13:30' 
  GROUP BY Segment, HeadAddress ) f1 ON h.HeadAddress = f1.HeadAddress AND h.Segment = f1.Segment 
  LEFT JOIN 
  ( SELECT h.HeadID AS HeadID, sum(isnull(t1.Coins, 0)) AS out_ticket, sum(isnull(t2.FreeCoin, 0)) AS free_coin FROM t_head h 
  LEFT JOIN flw_ticket_exit t1 ON h.HeadAddress = t1.HeadAddress AND h.Segment = t1.Segment AND t1.RealTime BETWEEN  '2017-09-06 00:07' and '2017-09-15 13:30' 
  LEFT JOIN flw_game_free t2 ON h.HeadID = t2.HeadID AND t2.RealTime BETWEEN  '2017-09-06 00:07' and '2017-09-15 13:30' 
  GROUP BY h.HeadID ) f2 ON h.HeadID = f2.HeadID LEFT JOIN t_game g ON g.GameID = h.GameID ) v1 
) v 
where 1=1 GROUP BY v.GameID,v.gamename


--游戏机排行榜
declare @StartTime datetime
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
on e.gameId = f.GameID



--游戏机排行榜
--select * from t_workstation
select * from flw_schedule--(班次表)
select * from flw_food_sale--(套餐销售流水表)
select * from flw_digite_coin--(数字币销售流水表)
select * from flw_goods--(商品销售流水表)
select * from flw_food_ticket_sale--(门票销售表)
select * from flw_ticket_exit--(退票表)
select * from flw_coin_exit--(退卡退钱流水表)

select * from [view_food_total]


select * from 
(
	select ISNULL( CONVERT(varchar(10), v1.CheckDate,120),'null') as CheckDate, v1.FoodName AS FoodName, isnull(v2.total_count, 0) AS total_count, isnull(v2.total_money, 0) AS total_money, isnull(v2.total_coins, 0) AS total_coins, 
    isnull(v2.coupon_money, 0) AS coupon_money, (CASE v2.FlowType WHEN '0' THEN '入会' WHEN '1' THEN '售餐' WHEN 'X' THEN '退餐' END) AS flow_type, v2.FlowType AS FlowType from 
	(
		SELECT     TOP (100) PERCENT cd.CheckDate AS CheckDate, f.FoodID, foods.FoodName
		FROM         dbo.flw_food_sale AS f LEFT OUTER JOIN
							  dbo.flw_schedule AS s ON f.ScheduleID = s.ID LEFT OUTER JOIN
							  dbo.flw_checkdate_schedule AS slist ON s.ID = slist.ScheduleID LEFT OUTER JOIN
							  dbo.flw_checkdate AS cd ON slist.CheckID = cd.ID LEFT OUTER JOIN
							  dbo.t_foods AS foods ON f.FoodID = foods.FoodID LEFT OUTER JOIN
							  dbo.t_member_coupon AS c ON c.FlowID = f.ID
		where CheckDate is null
		GROUP BY f.FoodID, cd.CheckDate, foods.FoodName
		ORDER BY foods.FoodName
	) v1
	left join 
	(
		SELECT     TOP (100) PERCENT cd.CheckDate AS CheckDate, f.FoodID, foods.FoodName, COUNT(f.FoodID) AS total_count, SUM(f.TotalMoney) AS total_money, SUM(f.CoinQuantity) AS total_coins, 
							  SUM(foods.CoinQuantity - foods.FoodPrice) AS free_coins, SUM(ISNULL(c.CouponPrice, 0)) AS coupon_money, SUM(ISNULL(f.Deposit, 0)) AS Deposit, f.FlowType
		FROM         dbo.flw_food_sale AS f LEFT OUTER JOIN
							  dbo.flw_schedule AS s ON f.ScheduleID = s.ID LEFT OUTER JOIN
							  dbo.flw_checkdate_schedule AS slist ON s.ID = slist.ScheduleID LEFT OUTER JOIN
							  dbo.flw_checkdate AS cd ON slist.CheckID = cd.ID LEFT OUTER JOIN
							  dbo.t_foods AS foods ON f.FoodID = foods.FoodID LEFT OUTER JOIN
							  dbo.t_member_coupon AS c ON c.FlowID = f.ID
		WHERE     (f.FlowType IN ('0', '1')) and cd.CheckDate is null
		GROUP BY f.FoodID, cd.CheckDate, foods.FoodName, f.FlowType
		ORDER BY foods.FoodName
	) v2
	ON v1.CheckDate = v2.CheckDate AND v1.FoodID = v2.FoodID
) a where a.CheckDate = 'null'



select * from flw_schedule
select * from flw_checkdate_schedule
select * from flw_checkdate
select * from t_foods
select * from t_member_coupon

		SELECT     TOP (100) PERCENT cd.CheckDate AS CheckDate, f.FoodID, foods.FoodName
		FROM         dbo.flw_food_sale AS f LEFT OUTER JOIN
							  dbo.flw_schedule AS s ON f.ScheduleID = s.ID LEFT OUTER JOIN
							  dbo.flw_checkdate_schedule AS slist ON s.ID = slist.ScheduleID LEFT OUTER JOIN
							  dbo.flw_checkdate AS cd ON slist.CheckID = cd.ID LEFT OUTER JOIN
							  dbo.t_foods AS foods ON f.FoodID = foods.FoodID LEFT OUTER JOIN
							  dbo.t_member_coupon AS c ON c.FlowID = f.ID
		where CheckDate is null
		GROUP BY f.FoodID, cd.CheckDate, foods.FoodName
		ORDER BY foods.FoodName
		
		select * from flw_food_sale

		SELECT     TOP (100) PERCENT cd.CheckDate AS CheckDate, f.FoodID, foods.FoodName, COUNT(f.FoodID) AS total_count, SUM(f.TotalMoney) AS total_money, SUM(f.CoinQuantity) AS total_coins, 
							  SUM(foods.CoinQuantity - foods.FoodPrice) AS free_coins, SUM(ISNULL(c.CouponPrice, 0)) AS coupon_money, SUM(ISNULL(f.Deposit, 0)) AS Deposit, f.FlowType
		FROM         dbo.flw_food_sale AS f LEFT OUTER JOIN
							  dbo.flw_schedule AS s ON f.ScheduleID = s.ID LEFT OUTER JOIN
							  dbo.flw_checkdate_schedule AS slist ON s.ID = slist.ScheduleID LEFT OUTER JOIN
							  dbo.flw_checkdate AS cd ON slist.CheckID = cd.ID LEFT OUTER JOIN
							  dbo.t_foods AS foods ON f.FoodID = foods.FoodID LEFT OUTER JOIN
							  dbo.t_member_coupon AS c ON c.FlowID = f.ID
		WHERE     (f.FlowType IN ('0', '1')) and cd.CheckDate is null
		GROUP BY f.FoodID, cd.CheckDate, foods.FoodName, f.FlowType
		ORDER BY foods.FoodName



select * from dbo.view_food_total where CheckDate = 'null'