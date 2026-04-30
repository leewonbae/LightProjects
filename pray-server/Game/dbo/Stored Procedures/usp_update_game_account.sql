CREATE PROCEDURE [dbo].[usp_update_game_account]
	@account_id  bigint,
	@nickname nvarchar(50),
	@level int,
	@last_login_dt datetime,
	@exp int
AS
	if exists (select 1 from dbo.game_account where account_id = @account_id)
	begin
		update dbo.game_account
		set [nickname] = @nickname,
			[level] = @level,
			[last_login_dt] = @last_login_dt,
			[exp] = @exp
		where account_id = @account_id
	end
	else
RETURN 0
