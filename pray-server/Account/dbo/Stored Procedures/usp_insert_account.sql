CREATE PROCEDURE [dbo].[usp_insert_account]
    @account_key NVARCHAR(50),
	@game_db_idx TINYINT,
    @nickname NVARCHAR(50),
    @reg_dt DATETIME
AS
    SET NOCOUNT ON;

	INSERT INTO dbo.account ([account_type], [account_key], [game_db_idx], [nickname], [reg_dt])
         OUTPUT inserted.id
         VALUES (0, @account_key, @game_db_idx, @nickname, @reg_dt);
RETURN 0