CREATE PROCEDURE [dbo].[usp_update_account]
	@id BIGINT,
	@idfa NVARCHAR(50),
    @adid NVARCHAR(50),
    @fcm_token NVARCHAR(50),
    @session_token NVARCHAR(50)
AS
	UPDATE dbo.account
       SET [idfa] = @idfa,
           [adid] = @adid,
           [fcm_token] = @fcm_token,
           [session_token] = @session_token
     WHERE [id] = @id;
RETURN 0
