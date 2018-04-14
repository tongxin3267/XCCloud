--数字币销售
declare @BusinessDate varchar(10)
set  @BusinessDate = '2017-09-20 10:17:00'
select a.RealTime as SaleTime,FoodName,CountNum,TotalMoney,Note,a.WorkStation from dbo.flw_digite_coin a inner join dbo.flw_digite_coin_detial b on a.ID = b.ID left join t_foods c on a.FoodID = c.FoodID where 1 = 1 and DATEDIFF(dy,a.RealTime,@BusinessDate) = 0
select top 5 * from 
(
    select FoodName, sum(TotalMoney) as TotalMoney from dbo.flw_digite_coin a inner join dbo.flw_digite_coin_detial b on a.ID = b.ID left join t_foods c on a.FoodID = c.FoodID where 1 = 1 and DATEDIFF(dy,a.RealTime,@BusinessDate) = 0 Group by FoodName
) a


--数字币销毁
declare @BusinessDate varchar(10)
set  @BusinessDate = '2017-09-20 10:17:00'
declare @ICCardID varchar(20)
set @ICCardID = '111111'
select DestroyTime,a.ICCardID,ISNULL(b.realName,b.UserID) as UserName,Note from Data_DigitCoinDestroy a left join Base_UserInfo b on a.UserID = b.UserID where DATEDIFF(DY,DestroyTime,@BusinessDate) = 0 and a.ICCardID = @ICCardID

--数字币盘点
