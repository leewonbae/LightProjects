CREATE PROCEDURE [dbo].[usp_select_game_account]
	@account_id bigint
AS
	SELECT * FROM dbo.game_account
	WHERE [account_id] = @account_id
RETURN 0
