CREATE PROCEDURE [dbo].[usp_upsert_game_account]
	@account_id bigint,
	@nickname nvarchar(50),
	@level int,
	@exp int,
	@last_login_dt datetime,
	@create_dt datetime
AS
	IF EXISTS (SELECT 1 FROM dbo.game_account WHERE [account_id] = @account_id)
		BEGIN
		UPDATE dbo.game_account
		SET 
			[nickname] = @nickname,
			[level] = @level,
			[exp] = @exp,
			[last_login_dt] = @last_login_dt
		WHERE [account_id] = @account_id
		END
	ELSE
		BEGIN
		INSERT INTO dbo.game_account 
	(
		[nickname],
		[level],
		[exp],
		[last_login_dt],
		[create_dt]
	)
	VALUES
	(
		@nickname,
		@level,
		@create_dt,
		@exp,
		@last_login_dt
	)
		END
	
RETURN 0
