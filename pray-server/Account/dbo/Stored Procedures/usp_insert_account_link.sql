CREATE PROCEDURE [dbo].[usp_insert_account_link]
	@login_token NVARCHAR(50),
	@platform_type TINYINT,
    @account_id BIGINT,
    @create_dt DATETIME
AS
	INSERT INTO dbo.account_link ([login_token], [platform_type], [account_id], [create_dt])
         VALUES (@login_token, @platform_type, @account_id, @create_dt)
RETURN 0