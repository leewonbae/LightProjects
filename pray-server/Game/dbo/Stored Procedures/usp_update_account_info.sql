CREATE PROCEDURE [dbo].[usp_update_account_info]
	@id  bigint,
	@nickname nvarchar(50)
AS
	if exists (select 1 from dbo.account_info where id = @id)
	begin
		update dbo.account_info
		set nickname = @nickname
		where id = @id
	end
	else
RETURN 0
