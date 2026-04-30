CREATE PROCEDURE [dbo].[usp_insert_account]
    @nickname NVARCHAR(50),
    @account_key NVARCHAR(50),
	@game_db_idx TINYINT,
    @regist_dt DATETIME
AS
    SET NOCOUNT ON;

	INSERT INTO dbo.account ([account_type],[nickname], [account_key], [game_db_idx], [regist_dt])
         OUTPUT inserted.id
         VALUES (0,@nickname, @account_key, @game_db_idx, @regist_dt);
RETURN 0