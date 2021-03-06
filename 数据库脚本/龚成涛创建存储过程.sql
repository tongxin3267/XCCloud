USE [XCCloudRS232]
GO
/****** Object:  StoredProcedure [dbo].[RemoveMerchRouter]    Script Date: 12/14/2017 17:01:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--控制器解除绑定
CREATE PROCEDURE [RemoveMerchRouter]
(
	@RouterId INT
)
AS
BEGIN TRAN T

	SET NOCOUNT ON 
	SET XACT_ABORT ON 
	                 
	--删除商户与当前控制中的分组关联
	DELETE Data_MerchSegment
	WHERE ParentID = @RouterId;
	
	--删除当前控制器中的分组
	DELETE Data_GameInfo
	WHERE DeviceID = @RouterId;
	
	--删除当前控制器中的外设
	DELETE Data_MerchDevice
	WHERE ParentID = @RouterId;
	
	--重置控制器绑定商户
	UPDATE Base_DeviceInfo
	SET MerchID = 0, [Status] = 0
	WHERE ID = @RouterId;
		
			
	IF (@@ERROR<>0)
	BEGIN
		ROLLBACK TRAN T
		RETURN 0
	END
	ELSE
	BEGIN
		COMMIT TRAN T
		RETURN 1
	END
GO

/****** Object:  StoredProcedure [dbo].[UpdateMemberBalance]    Script Date: 10/31/2017 17:05:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--替换设备
CREATE PROCEDURE [ReplaceDevice]
(
	@oldDeviceId INT, --旧设备令牌
	@newDeviceId INT, --新设备令牌
	@merchId INT,  --商户ID
	@category INT --设备类别 1：控制器 2：外设 3：终端
)
AS
BEGIN TRAN T

	SET NOCOUNT ON 
	SET XACT_ABORT ON 
	                 
	----删除商户与当前控制中的分组关联
	--DELETE Data_MerchSegment
	--WHERE ParentID = @RouterId;
	
	----删除当前控制器中的分组
	--DELETE Data_GameInfo
	--WHERE DeviceID = @RouterId;
	
	----删除当前控制器中的外设
	--DELETE Data_MerchDevice
	--WHERE ParentID = @RouterId;
	
	--重置旧设备绑定的商户为空,状态修改为未激活
	UPDATE Base_DeviceInfo
	SET MerchID = 0, [Status] = 0
	WHERE ID = @oldDeviceId;
	
	--将新设备与当前商户绑定，状态改为激活
	UPDATE Base_DeviceInfo
	SET MerchID = @merchId, [Status] = 1
	WHERE ID = @newDeviceId;
	
	IF(@category = 1)
	BEGIN
		--将旧控制器的分组与新设备关联
		UPDATE Data_GameInfo
		SET DeviceID = @newDeviceId
		WHERE DeviceID = @oldDeviceId
		
		--将终端与新控制器关联
		UPDATE Data_MerchSegment
		SET ParentID = @newDeviceId
		WHERE ParentID = @oldDeviceId
		
		--将外设与新控制器关联
		UPDATE Data_MerchDevice
		SET ParentID = @newDeviceId
		WHERE ParentID = @oldDeviceId
	END
	ELSE IF(@category = 2)
	BEGIN
		--将旧外设替换为新外设
		UPDATE Data_MerchDevice
		SET DeviceID = @newDeviceId
		WHERE DeviceID = @oldDeviceId
	END
	ELSE IF (@category = 3)
	BEGIN
		--将终端设备替换为新终端设备
		UPDATE Data_MerchSegment
		SET DeviceID = @newDeviceId
		WHERE DeviceID = @oldDeviceId
	END
			
	IF (@@ERROR<>0)
	BEGIN
		ROLLBACK TRAN T
		RETURN 0
	END
	ELSE
	BEGIN
		COMMIT TRAN T
		RETURN 1
	END
GO