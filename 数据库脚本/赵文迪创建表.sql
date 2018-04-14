IF EXISTS(SELECT * FROM sysobjects WHERE name='T_MemberToken' AND xtype='U') DROP TABLE T_MemberToken
GO
create table T_MemberToken
(
ID int identity(1,1)primary key ,  --ID
Token varchar(200),			--令牌
StoreId varchar(200),		--门店ID
Phone varchar(200) ,
CreateTime datetime default(GetDate()),	--创建日期
UpdateTime datetime default(GetDate()),	--创建日期
)

IF EXISTS(SELECT * FROM sysobjects WHERE name='t_MobileToken' AND xtype='U') DROP TABLE t_MobileToken
GO
create table t_MobileToken
(
ID int identity(1,1),  --ID
Token varchar(200),			--令牌
Phone varchar(200)primary key  ,
CreateTime datetime default(GetDate()),	--创建日期
UpdateTime datetime default(GetDate()),	--创建日期
)

IF EXISTS(SELECT * FROM sysobjects WHERE name='t_MPOrder' AND xtype='U') DROP TABLE t_MPOrder
GO
create table t_MPOrder
(
OrderNumber varchar(200),
StoreId varchar(200),
CreateTime varchar(200)primary key,	--创建日期
)

IF EXISTS(SELECT * FROM sysobjects WHERE name='t_TCP' AND xtype='U') DROP TABLE t_TCP
GO
create table t_TCP
(
ID int identity(1,1) primary key,  --ID
ManageType varchar(2),		--消息类型
SendStoreId varchar(20),		--发送方门店号
SendMobile varchar(15),		--发送方手机号码
ReceiveStoreId varchar(20),	--接受方门店号
ReceiveMobile varchar(15),		--接收方手机号码
TextData varchar(max),			--消息内容
[State] int default(0),			--状态值
CreateTime datetime default(GetDate()),--创建时间
)
use XCGameManagerDB
go

IF EXISTS(SELECT * FROM sysobjects WHERE name='t_UserRegister' AND xtype='U') DROP TABLE t_UserRegister
GO
create table t_UserRegister
(
ID int identity(1,1) primary key,  --ID
UserName	varchar(50),			--用户账号
[PassWord] varchar(200),			--用户密码
Mobile varchar(15),--手机号码
StoreId	varchar(20),--门店ID
[State] int default(0),--0代表未审批
CreateTime datetime default(GetDate()),--创建时间
AuditingTime datetime,					--审核时间
AuditingUser varchar(50)				--审核人
)
use XCGameManagerDB
go
IF EXISTS(SELECT * FROM sysobjects WHERE name='[t_food_sale]' AND xtype='U') DROP TABLE [t_food_sale]
GO
CREATE TABLE [dbo].[t_food_sale](
	[ID] [int] IDENTITY(1,1) NOT NULL primary key,  --ID,
	[FlowType] [varchar](1) NULL,
	[ICCardID] [int] NULL,
	[FoodID] [int] NULL,
	[CoinQuantity] [int] NULL,
	[Point] [int] NULL,
	[Balance] [int] NULL,
	[MemberLevelID] [int] NULL,
	[Deposit] [decimal](18, 2) NULL,
	[OpenFee] [decimal](18, 2) NULL,
	[RenewFee] [decimal](18, 2) NULL,
	[ChangeFee] [decimal](18, 2) NULL,
	[CreditFee] [decimal](18, 2) NULL,
	[TotalMoney] [decimal](18, 2) NULL,
	[Note] [varchar](500) NULL,
	[PayType] [varchar](1) NULL,
	[BuyFoodType] [varchar](1) NULL,
	[UserID] [int] NULL,
	[ScheduleID] [int] NULL,
	[AuthorID] [int] NULL,
	[RealTime] [datetime] NULL,
	[WorkStation] [varchar](50) NULL,
	[MacAddress] [varchar](50) NULL,
	[DiskID] [varchar](50) NULL,
	[ExitRealTime] [datetime] NULL,
	[ExitBalance] [int] NULL,
	[ExitUserID] [int] NULL,
	[ExitScheduleID] [int] NULL,
	[ExitWorkStation] [varchar](50) NULL,
	[OrderID] [varchar](32) NULL,
	[StoreId] varchar(50)
)

IF EXISTS(SELECT * FROM sysobjects WHERE name='[t_membercard]' AND xtype='U') DROP TABLE [t_membercard]
GO
CREATE TABLE [dbo].[t_membercard](
	ID [int] IDENTITY(1,1),  --ID,
	ICCardID  varchar(50),
	StoreId  varchar(50),
	Mobile varchar(15),
	Createtime datetime default(Getdate())
)


use XCGameManagerDB
go
--新闻
IF EXISTS(SELECT * FROM sysobjects WHERE name='t_promotion' AND xtype='U') DROP TABLE t_promotion
GO
CREATE TABLE t_promotion
(
	[ID] [int] IDENTITY(1,1) NOT NULL primary key,  --ID,
	StoreName	varchar(100),						--活动店名
	StoreID varchar(50),							--门店ID
	[Time] datetime,								--活动时间
	ReleaseTime datetime,						--发布时间
	PicturePath varchar(200),						--图片路径
	Title varchar(100),								--标题
	PagePath varchar(Max),							--跳转路径
	Publisher varchar(50),							--发布人
	[State] int default(1),							--1显示0不显示
	PublishType int ,						--0轮播1列表
	PromotionType int						--0开业促销,1限时抢购，2预售开团，4 秒杀
)
IF EXISTS(SELECT * FROM sysobjects WHERE name='t_store_dog' AND xtype='U') DROP TABLE t_store_dog
GO
CREATE TABLE t_store_dog
(
Id [int] IDENTITY(1,1) NOT NULL primary key,  --ID,
StoreId varchar(10),
DogId varchar(50),
)