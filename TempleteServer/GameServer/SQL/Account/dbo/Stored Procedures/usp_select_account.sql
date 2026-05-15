CREATE PROCEDURE [dbo].[usp_select_account]
	@id bigint
AS
	SELECT * FROM dbo.[account] 	
	WHERE [id] = @id
RETURN 0
