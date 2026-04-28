CREATE PROCEDURE [dbo].[usp_select_account_info]
	@id bigint
AS
	SELECT * FROM dbo.account_info
	WHERE id = @id
RETURN 0
