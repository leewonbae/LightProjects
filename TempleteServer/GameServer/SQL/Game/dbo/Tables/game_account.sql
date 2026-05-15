CREATE TABLE [dbo].[game_account]
(
	[account_id] BIGINT NOT NULL PRIMARY KEY, 
    [nickname] NVARCHAR(50) NOT NULL, 
    [level] INT NOT NULL, 
    [exp] INT NOT NULL DEFAULT 0, 
    [last_login_dt] DATETIME NULL,
    [create_dt] DATETIME NOT NULL, 
)
