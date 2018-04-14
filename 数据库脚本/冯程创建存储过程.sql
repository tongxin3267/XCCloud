USE [XCCloud]
GO

/****** Object:  StoredProcedure [dbo].[SP_RegisterUserFromWx]    Script Date: 11/21/2017 14:31:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE Proc [dbo].[SP_RegisterUserFromWx](@StoreId varchar(15),@MerchId varchar(15),@UserType int,@Mobile varchar(16),@Username varchar(50),@Realname varchar(50),@UserPassword varchar(6),@Message varchar(500) = '',@WXOpenID varchar(100) = '',@UnionID varchar(100) = '',@WorkID int output, @Return int output)
as

 --用户不能重复注册
 if exists (select 1 from Base_UserInfo where OpenID = @WXOpenID)
 	begin
 		set @Return = 0
 		return
 	end

 begin transaction tran1
 begin try
	declare @UserId int 
	declare @AuditorId int
	--添加门店用户
	select @AuditorId = UserId from Base_UserInfo where OpenID = (select WXOpenID from Base_MerchantInfo where MerchID = (select MerchID from Base_StoreInfo where StoreID=@StoreId))
	insert into Base_UserInfo(StoreID,MerchID,UserType,LogName,LogPassword,OpenID,UnionID,RealName,Mobile,ICCardID,CreateTime,[Status],Auditor)
	values(@StoreId,@MerchId,@UserType,@Username,@UserPassword,@WXOpenID,@UnionID,@Realname,@Mobile,'',GETDATE(),0,@AuditorId)
	set @UserId = @@identity	
	--添加工单	
	insert into XC_WorkInfo(SenderID,SenderTime,WorkType,WorkState,WorkBody,AuditorID)
	values(@UserId,GETDATE(),0,0,@Message,@AuditorId) 
	set @WorkID = @@identity
	--添加消息
	insert into Data_Message(Sender, Receiver, SendTime, MsgText, ReadFlag) 
	values(@UserId, @AuditorId, GETDATE(), @Message, 0)
	--添加日志
	insert into Log_Operation(StoreID, UserID, AuthorID, Content)
	values(@StoreId, @UserId, @AuditorId, '微信新用户注册审核' + @Message)
	
	commit transaction tran1
	set @Return = 1
	select * from XC_WorkInfo where WorkID = @WorkID
 end try
 begin catch
	rollback transaction tran1
	set @Return = 0
 end catch

GO

CREATE Proc [dbo].[SelectUserGroup](@UserID int)
as
 begin
	select a.ID, a.MerchID, a.GroupName, a.Note
	from Base_UserGroup a inner join Base_StoreInfo c on a.MerchID=c.MerchID inner join Base_UserInfo b on c.StoreID=b.StoreID
	where b.UserID=@UserID
	order by a.ID
 end 

GO

CREATE Proc [dbo].[SelectUserGrant](@UserID int)
as
 begin
	select a.ID, a.PID, a.DictKey, a.DictValue, a.OrderID, d.GrantEN
	from Dict_System a 
	left join Dict_System b on a.PID=b.ID
	--inner join Base_StoreInfo c on a.MerchID=c.MerchID 
	--inner join Base_UserInfo b on c.StoreID=b.StoreID
	left join Base_UserGrant d on a.ID=d.GrantID and d.UserID=@UserID	
	where a.Enabled=1 and b.DictKey='权限列表' and a.MerchID='1'
	order by OrderID,PID	
 end

GO


Create Proc [dbo].[SP_DictionaryNodes](@MerchID nvarchar(15), @DictKey nvarchar(50), @RootID int output)
as
 begin
	declare @sql nvarchar(max)
	SET @sql = ''
	SET @sql = @sql + 'select * from Dict_System where 1=1'
	if(@MerchID is not null and @MerchID <> '')
		SET @sql = @sql + ' and (MerchID=@MerchID or MerchID is null or MerchID='''')'
	if(@DictKey is not null and @DictKey <> '')
		begin	
			select @RootID=ID from (select top 1 ID from Dict_System where DictKey=@DictKey and (MerchID=@MerchID or MerchID is null or MerchID='')) m
			if not exists (select 0 from Dict_System where PID=@RootID)
				return
			SET @sql = @sql + ' and PID=@RootID'
		end
	--exec (@sql)
	exec sp_executesql @sql, N'@MerchID nvarchar(15), @DictKey nvarchar(50), @RootID int', @MerchID, @DictKey, @RootID
 end

GO

CREATE Proc [dbo].[SelectMerchFunction](@MerchID varchar(15))
as
 begin
	declare @MerchTag int = 0
	select @MerchTag=MerchTag from Base_MerchantInfo where MerchID=@MerchID
	select distinct a.ParentID, a.FunctionID, a.FunctionName, (case when c.FunctionEN is null then (case when b.FunctionEN is null then null else b.FunctionEN end) else c.FunctionEN end) as FunctionEN
	from Dict_FunctionMenu a left join (select b.MerchID,c.FunctionID,c.FunctionEN from Base_MerchantInfo a inner join Base_UserInfo b on a.CreateUserID=b.UserID inner join Base_MerchFunction c on b.MerchID=c.MerchID and a.MerchType=3) b on a.FunctionID=b.FunctionID
	left join Base_MerchFunction c on a.FunctionID=c.FunctionID and c.MerchID=@MerchID 
	where a.MenuType in (1, case when @MerchTag=1 then 5 else 1 end)
	order by a.ParentID, a.FunctionID
 end

GO

CREATE Proc [dbo].[SP_GetMenus](@LogType int, @LogID int, @MerchID varchar(15))
as
begin
	
	CREATE TABLE #MENU (FunctionID INT NULL)	
	if @LogType=3 --商户用户
	begin
		declare @MerchType int 
		SELECT @MerchType=MerchType FROM Base_MerchantInfo WHERE MerchID=@MerchID
		insert into #MENU
		select FunctionID from Dict_FunctionMenu where MenuType=(case when @MerchType in (1, 2) then 3 when @MerchType=3 then 4 else null end)
	end
	else if @LogType=1 --莘宸管理员
	begin
		insert into #MENU
		select FunctionID from Dict_FunctionMenu where MenuType=0
	end
	else if @LogType=0 --莘宸员工
	begin
		insert into #MENU
		select a.FunctionID from Dict_FunctionMenu a left join Base_UserGroup_Grant b on a.FunctionID=b.FunctionID
		left join Base_UserInfo c on b.GroupID=c.UserGroupID and c.UserID=@LogID
		where a.MenuType=0 and b.IsAllow=1
	end
	else if @LogType=2 --门店用户
	begin
		insert into #MENU
		select FunctionID from Dict_FunctionMenu where MenuType=1
	end
					
	;WITH 
	LOCS(FunctionID,ParentID,FunctionName,PageName,Icon)
	AS
	(
	SELECT FunctionID,ParentID,FunctionName,PageName,Icon FROM Dict_FunctionMenu WHERE FunctionID in (select FunctionID from #MENU)
	UNION ALL
	SELECT A.FunctionID,A.ParentID,A.FunctionName,A.PageName,A.ICON FROM Dict_FunctionMenu A JOIN LOCS B ON 
	B.ParentID=A.FunctionID 
	--A.pid = B.id
	)
	
	SELECT DISTINCT FunctionID,ParentID,FunctionName,PageName,Icon from LOCS
	
 end

GO

CREATE Proc [dbo].[SelectUserGroupGrant](@GroupID int, @MerchID varchar(15))
as
 begin
	select ID,GroupName,Note from Base_UserGroup where ID=@GroupID
	CREATE TABLE #MENU (FunctionID INT NULL, IsAllow INT NULL)
	
	insert into #MENU
	select a.FunctionID, c.IsAllow
	from Base_MerchFunction a left join Base_UserGroup b on a.MerchID=b.MerchID and b.ID=@GroupID
	left join Base_UserGroup_Grant c on a.FunctionID=c.FunctionID and b.ID=c.GroupID 
	where a.FunctionEN=1 and a.MerchID=@MerchID 
	order by a.ID
	
		
	;WITH 
	LOCS(FunctionID,ParentID,FunctionName)
	AS
	(
	SELECT FunctionID,ParentID,FunctionName FROM Dict_FunctionMenu WHERE FunctionID in (select FunctionID from #MENU)
	UNION ALL
	SELECT A.FunctionID,A.ParentID,A.FunctionName FROM Dict_FunctionMenu A JOIN LOCS B ON 
	B.ParentID=A.FunctionID 
	--A.pid = B.id
	)
	select DISTINCT a.FunctionID,a.ParentID,a.FunctionName,b.IsAllow from LOCS a Left JOIN #MENU b on a.FunctionID=b.FunctionID
	
 end

GO

CREATE Proc [dbo].[SelectStoreUnchecked](@UserID int)
as
 begin
	select a.StoreID, a.MerchID, a.StoreName, a.Password, a.AuthorExpireDate, a.AreaCode, a.Address, a.Contacts, a.Mobile, b.DictKey as SelttleTypeStr, c.DictKey as StoreStateStr from Base_StoreInfo a 
	left join (select b.* from Dict_System a inner join Dict_System b on a.ID=b.PID where a.DictKey='结算类型') b on a.SelttleType=convert(int,b.DictValue) 
	left join (select b.* from Dict_System a inner join Dict_System b on a.ID=b.PID where a.DictKey='门店状态') c on a.StoreState=convert(int,c.DictValue) 
	left join Base_MerchantInfo d on a.MerchID=d.MerchID and d.CreateType=1 
	left join Base_MerchantInfo e on a.MerchID=e.MerchID and e.CreateType=2 
	left join Base_MerchantInfo f on e.CreateUserID=f.MerchID 
	where a.StoreState=0 and (d.CreateUserID=@UserID or f.CreateUserID=@UserID)
 end


GO


CREATE Proc [dbo].[SelectFunctionForXA](@GroupID int)
as
begin
	select ID,GroupName,Note from Base_UserGroup where ID=@GroupID
	select distinct a.ParentID, a.FunctionID, a.FunctionName, b.IsAllow 
	from Dict_FunctionMenu a left join Base_UserGroup_Grant b on a.FunctionID=b.FunctionID and b.GroupID=@GroupID
	where a.MenuType=0
	order by a.ParentID, a.FunctionID
end

GO

CREATE Proc [dbo].[SelectXcUserGrant](@UserID int)
as
 begin
	select a.ID, a.PID, a.DictKey, a.DictValue, a.OrderID, c.GrantEN
	from Dict_System a 
	left join Dict_System b on a.PID=b.ID
	left join Base_UserGrant c on a.ID=c.GrantID and c.UserID=@UserID
	where a.Enabled=1 and b.DictKey='权限列表' and (a.MerchID is null or a.MerchID='') 
	order by OrderID,PID
 end

GO

CREATE Proc [dbo].[SP_RowToCol](@PID int)
as
 begin transaction tran1
 begin try	
	declare @temp table (ColumnName varchar(50))	
	declare @DictKey varchar(50)
	DECLARE MyCursor CURSOR FOR select DictKey from Dict_System where PID= @PID	
	--打开一个游标    
	OPEN MyCursor

	--循环一个游标
	FETCH NEXT FROM  MyCursor INTO @DictKey
	WHILE @@FETCH_STATUS =0
    	BEGIN
	
		insert into @temp (ColumnName) values (@DictKey)

	--循环一个游标
        FETCH NEXT FROM  MyCursor INTO @DictKey
    	END    
	--关闭游标
	CLOSE MyCursor
	--释放资源
	DEALLOCATE MyCursor

	declare @ColumnName varchar(50)
	declare @s varchar(MAX)=''
	set @s = 'select *'	
	DECLARE MyCursor CURSOR FOR select ColumnName from @temp	
	--打开一个游标    
	OPEN MyCursor

	--循环一个游标
	FETCH NEXT FROM  MyCursor INTO @ColumnName
	WHILE @@FETCH_STATUS =0
    	BEGIN
	
		set @s = @s + ',(case DictKey when ''' + @ColumnName + ''' then DictValue else null end) ' + @ColumnName

	--循环一个游标
        FETCH NEXT FROM  MyCursor INTO @ColumnName
    	END    
	--关闭游标
	CLOSE MyCursor
	--释放资源
	DEALLOCATE MyCursor
	
	set @s = @s + ' from Dict_System where PID=' + convert(varchar, @PID)

	exec(@s)

COMMIT TRAN 

end try   
begin catch  
	SELECT 'There was an error! ' + ERROR_MESSAGE()   
    ROLLBACK  
end catch 

GO

CREATE Proc [dbo].[SP_RowToCol2](@PID int)
as	
 begin transaction tran1
 begin try
	declare @temp table (ColumnName varchar(50))	
	insert @temp select DictKey as ColumnName from Dict_System with(nolock) where PID=@PID
	
	declare @s varchar(MAX)='select *'
	declare @ColumnName varchar(50)

	WHILE EXISTS(SELECT [ColumnName] FROM @temp)
	BEGIN 

	SET ROWCOUNT 1
	select @ColumnName=t.ColumnName from @temp as t
	set @s = @s + ',(case DictKey when ''' + @ColumnName + ''' then DictValue else null end) [' + @ColumnName + ']'	
	SET ROWCOUNT 0
	--删除临时表中的使用完的数据
	DELETE from @temp where [ColumnName] = @ColumnName

	END 		
	
	set @s = @s + ' from Dict_System where PID=' + convert(varchar, @PID)

	exec(@s)

 end try   
 begin catch  
	SELECT 'There was an error! ' + ERROR_MESSAGE()   
    	ROLLBACK  
 end catch 

GO

CREATE Proc [dbo].[GetOrdersCheck](@CheckDate varchar(10), @MerchId varchar(50))
as
 begin
	select MAX(T1.ID) as ID, T1.CheckDate,T1.StoreName, T1.ScheduleState, SUM(T2.PayCount) as PayCount, SUM(T3.WechatRealPay) as WechatRealPay,
			SUM(T4.AliRealPay) as AliRealPay, SUM(T5.GroupBuyRealPay) as GroupBuyRealPay, SUM(T6.CoinMoney) as CoinMoney,
			SUM(T7.TicketCoins) as TicketCoins, SUM(T7.TicketCoinMoney) as TicketCoinMoney, SUM(T8.FreeCoin) as FreeCoin, 
			SUM(T9.SaveCoin) as SaveCoin, SUM(T10.OpenCount) as OpenCount, SUM(T10.OpenDeposit) as OpenDeposit, SUM(T10.OpenFee) as OpenFee,
			SUM(T11.RefundCount) as RefundCount, SUM(T11.RefundDeposit) as RefundDeposit, SUM(T12.TokenRealPay) as TokenRealPay, SUM(T13.RechargeRealPay) as RechargeRealPay,
			SUM(T14.CoinRealPay) as CoinRealPay, SUM(T15.GoodRealPay) as GoodRealPay, SUM(T16.TicketRealPay) as TicketRealPay, SUM(T17.GroupBuyCount) as GroupBuyCount		
	from 
	(
	SELECT     c.ID, b.CheckDate, a.StoreName, b.ID AS ScheduleID, (CASE WHEN EXISTS
						  (SELECT 1
							FROM dbo.Flw_Schedule
							where ID=b.ID and CheckDateID is null) then '有空班' WHEN EXISTS
                          (SELECT     1
                            FROM          dbo.Flw_Schedule
                            WHERE      [State] = 0 AND CheckDateID = c.ID and c.ID is not null) THEN '进行中' WHEN NOT EXISTS
                          (SELECT     1
                            FROM          dbo.Flw_Schedule
                            WHERE      [State] IN (0, 2) AND CheckDateID = c.ID and c.ID is not null) THEN '未审核' ELSE '已审核' END) AS ScheduleState
	FROM         dbo.Base_StoreInfo AS a INNER JOIN
				dbo.Flw_Schedule AS b on a.StoreID = b.StoreID left join
				  dbo.Flw_CheckDate AS c ON b.CheckDateID = c.ID and a.StoreID = c.StoreID and b.CheckDate = c.CheckDate
    where b.CheckDate=CONVERT(date, @CheckDate) and a.MerchID=@MerchId
    ) as T1 left join
    (
	  SELECT     ScheduleID, PayCount
	  FROM		dbo.Flw_Order
    ) as T2 on T1.ScheduleID=T2.ScheduleID left join
    (
		SELECT     ScheduleID, RealPay AS WechatRealPay
        FROM          dbo.Flw_Order 
        WHERE      (PayType = 1) 
    ) as T3 on T1.ScheduleID=T3.ScheduleID left join
    (
		SELECT     ScheduleID, RealPay AS AliRealPay
        FROM          dbo.Flw_Order
        WHERE      (PayType = 2)
    ) as T4 on T1.ScheduleID=T4.ScheduleID left join
    (
		SELECT     ScheduleID, RealPay AS GroupBuyRealPay
        FROM          dbo.Flw_Order
        WHERE      (OrderSource IN (2, 3, 4))
    ) as T5 on T1.ScheduleID=T5.ScheduleID left join
    (
		SELECT     ScheduleID, CoinMoney
		FROM         dbo.Flw_Coin_Exit 
		WHERE     (FlowType = 0)
    ) as T6 on T1.ScheduleID=T6.ScheduleID left join
    (
		SELECT     ScheduleID, Coins AS TicketCoins, CoinMoney AS TicketCoinMoney
		FROM         dbo.Flw_Ticket_Exit
		WHERE     (State = 1) 
    ) as T7 on T1.ScheduleID=T7.ScheduleID left join
    (
		SELECT     ScheduleID, Coins AS FreeCoin
	   FROM          dbo.Flw_Coin_Sale
	   WHERE      (WorkType IN (2, 5, 8))
    ) as T8 on T1.ScheduleID=T8.ScheduleID left join
    (
		SELECT     ScheduleID, Coins AS SaveCoin
		FROM          dbo.Flw_Coin_Sale
		WHERE      (WorkType = 4)
    ) as T9 on T1.ScheduleID=T9.ScheduleID left join
    (
		select a.ScheduleID, c.ID as OpenCount, c.Deposit as OpenDeposit, c.OpenFee
		from dbo.Flw_Order a
		inner join dbo.Flw_Order_Detail b on a.ID=b.OrderFlwID
		inner join dbo.Flw_Food_Sale c on b.FoodFlwID=c.ID
		where c.FlowType=1
    ) as T10 on t1.ScheduleID=t10.ScheduleID left join 
    (
		select a.ScheduleID, c.ID as RefundCount, c.Deposit as RefundDeposit
		from dbo.Flw_Order a
		inner join dbo.Flw_Order_Detail b on a.ID=b.OrderFlwID
		inner join dbo.Flw_Food_Sale c on b.FoodFlwID=c.ID
		where c.FlowType=7
    ) as t11 on t1.ScheduleID=t11.ScheduleID left join
    (
		select a.ScheduleID, a.RealPay as TokenRealPay
		from dbo.Flw_Order a
		inner join dbo.Flw_Order_Detail b on a.ID=b.OrderFlwID
		inner join dbo.Flw_Food_Sale c on b.FoodFlwID=c.ID
		where c.FlowType=2
    ) as T12 on t1.ScheduleID=t12.ScheduleID left join
    (
		select a.ScheduleID, a.RealPay as RechargeRealPay
		from dbo.Flw_Order a
		inner join dbo.Flw_Order_Detail b on a.ID=b.OrderFlwID
		inner join dbo.Flw_Food_Sale c on b.FoodFlwID=c.ID
		where c.FlowType=0 and c.BuyFoodType=1
    ) as T13 on t1.ScheduleID=t13.ScheduleID left join
    (
		select a.ScheduleID, a.RealPay as CoinRealPay
		from dbo.Flw_Order a
		inner join dbo.Flw_Order_Detail b on a.ID=b.OrderFlwID
		inner join dbo.Flw_Food_Sale c on b.FoodFlwID=c.ID
		where c.FlowType=0 and c.BuyFoodType in (2,3)
    ) as T14 on t1.ScheduleID=t14.ScheduleID left join
    (
		select a.ScheduleID, a.RealPay as GoodRealPay
		from dbo.Flw_Order a
		inner join dbo.Flw_Order_Detail b on a.ID=b.OrderFlwID
		inner join dbo.Flw_Food_Sale c on b.FoodFlwID=c.ID
		where c.FlowType=3 
    ) as T15 on t1.ScheduleID=t15.ScheduleID left join
    (
		select a.ScheduleID, a.RealPay as TicketRealPay
		from dbo.Flw_Order a
		inner join dbo.Flw_Order_Detail b on a.ID=b.OrderFlwID
		inner join dbo.Flw_Food_Sale c on b.FoodFlwID=c.ID
		where c.FlowType=4 
    ) as T16 on t1.ScheduleID=t16.ScheduleID left join
    (
		select a.ScheduleID, d.ContainCount as GroupBuyCount
		from dbo.Flw_Order a
		inner join dbo.Flw_Order_Detail b on a.ID=b.OrderFlwID
		inner join dbo.Flw_Food_Sale c on b.FoodFlwID=c.ID
		inner join dbo.Flw_Food_SaleDetail d on c.ID=d.FlwFoodID
		where a.OrderSource in (2,3,4) and d.SaleType=0
    ) as T17 on t1.ScheduleID=t17.ScheduleID
    
    group by T1.CheckDate, T1.StoreName, T1.ScheduleState      
                      
 end

GO

CREATE Proc [dbo].[CheckOrders](@CheckDate varchar(10), @MerchId varchar(50), @ID int, @Return int output)
as

 begin transaction tran1
 begin try
	if @ID>0
		begin
			update dbo.Flw_CheckDate set [Status]=2 where ID=@ID
			update dbo.Flw_Schedule set [State]=2 where CheckDateID=@ID
		end
	else
		begin
			declare @temp table (ID int)
			insert @temp select c.ID FROM dbo.Base_StoreInfo AS a INNER JOIN
				dbo.Flw_Schedule AS b on a.StoreID = b.StoreID left join
				  dbo.Flw_CheckDate AS c ON b.CheckDateID = c.ID and a.StoreID = c.StoreID and b.CheckDate = c.CheckDate
				  where b.CheckDate=CONVERT(date, @CheckDate) and a.MerchID=@MerchId and
						NOT EXISTS
                          (SELECT     1
                            FROM          dbo.Flw_Schedule
                            WHERE      [State] IN (0, 2) AND CheckDateID = c.ID and c.ID is not null) and
                        NOT EXISTS
						  (SELECT 1
							FROM dbo.Flw_Schedule
							where ID=b.ID and CheckDateID is null)
			update dbo.Flw_CheckDate set [Status]=2 where ID in (select ID from @temp)
			update dbo.Flw_Schedule set [State]=2 where CheckDateID in (select ID from @temp)
		end	
	
	commit transaction tran1
	set @Return = 1
 end try
 begin catch
	rollback transaction tran1
	set @Return = 0
 end catch

GO