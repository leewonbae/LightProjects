CREATE PROCEDURE [dbo].[usp_select_account_link]
	@login_token NVARCHAR(50)
AS
	SELECT TOP(1) * FROM dbo.account_link 
	WHERE [login_token] = @login_token
RETURN 0
