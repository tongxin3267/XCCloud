

/****** Object:  StoredProcedure [dbo].[RegisterMember]    Script Date: 10/31/2017 17:05:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE Proc [dbo].[RegisterMember](@Mobile varchar(11),@MemberPassword varchar(6),@WXOpenID varchar(100) = '',@Return int output)
as
 begin transaction tran1
 begin try
	declare @ICCardID int = 0;
	select @ICCardID = MAX(ICCardID) from t_member where ICCardID > 90000000
	set @ICCardID = @ICCardID + 1
	insert into t_member(ICCardID,RootICCardID,MemberID,Mobile,MemberPassword,Note,JoinTime,MemberState) 
	values(@ICCardID,@ICCardID,@ICCardID,@Mobile,@MemberPassword,@WXOpenID,GETDATE(),1)
	commit transaction tran1
	set @Return = 1
	select * from t_member where ICCardID = @ICCardID
 end try
 begin catch
	rollback transaction tran1
	set @Return = 0
 end catch
GO






/****** Object:  StoredProcedure [dbo].[UpdateMemberBalance]    Script Date: 10/31/2017 17:05:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE Proc [dbo].[UpdateMemberBalance](@Balance varchar(200),@ICCardID varchar(200),@Segment varchar(200),@HeadAddress varchar(200),@Coins varchar(200),@Return int output)
as
 begin transaction tran1
 begin try
	update t_member set Balance=@Balance where ICCardID=@ICCardID
	insert into flw_485_coin (Segment,HeadAddress,ICCardID,Coins,CoinType,Balance,RealTime)
	values(@Segment,@HeadAddress,@ICCardID,@Coins,0,@Balance,GETDATE())
	commit transaction tran1
	set @Return = 1
	select * from t_member where ICCardID = @ICCardID
 end try
 begin catch
	rollback transaction tran1
	set @Return = 0
 end catch


GO




