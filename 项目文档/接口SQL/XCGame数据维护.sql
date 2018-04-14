1.
2.
--创建存储过程RegisterMember

CREATE Proc [dbo].[RegisterMember](@Mobile varchar(11),@MemberPassword varchar(6),@WXOpenID varchar(100) = '',@Return int output)
as
 begin try
	begin transaction tran1
	declare @ICCardID int = 0;
	select @ICCardID = MAX(ICCardID) from t_member where ICCardID > 90000000
	set @ICCardID = @ICCardID + 1
	insert into t_member(ICCardID,RootICCardID,MemberID,Mobile,MemberPassword,Note,JoinTime) values(@ICCardID,@ICCardID,@ICCardID,@Mobile,@MemberPassword,@WXOpenID,GETDATE())
	begin transaction tran1
	set @Return = 1
 end try
 begin catch
	rollback transaction tran1
	set @Return = 0
 end catch