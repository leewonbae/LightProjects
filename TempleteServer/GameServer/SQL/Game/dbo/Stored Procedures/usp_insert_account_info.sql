CREATE PROCEDURE [dbo].[usp_insert_account_info]
	@nickname nvarchar(50)
AS
	INSERT INTO dbo.account_info 
	(
	nickname
	)
	VALUES
	(
	@nickname
	)

	SELECT SCOPE_IDENTITY() AS Id  -- 생성된 ID 반환
RETURN 0
