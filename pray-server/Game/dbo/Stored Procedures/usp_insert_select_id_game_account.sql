CREATE PROCEDURE [dbo].[usp_insert_select_id_game_account]
	@nickname nvarchar(50),
	@level int,
	@create_dt datetime,
	@exp int,
	@last_login_dt datetime
AS
	INSERT INTO dbo.game_account 
	(
		[nickname],
		[level],
		[create_dt],
		[exp],
		[last_login_dt]
	)
	VALUES
	(
		@nickname,
		@level,
		@create_dt,
		@exp,
		@last_login_dt
	)

	SELECT SCOPE_IDENTITY() AS Id  -- 생성된 ID 반환
RETURN 0
