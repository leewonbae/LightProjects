CREATE TABLE [dbo].[account_link]
(
	[login_token]    NVARCHAR (50) NOT NULL,
    [platform_type]  INT       NOT NULL,
    [account_id]     BIGINT        NOT NULL,
    [create_dt]      DATETIME       NOT NULL,
   
   PRIMARY KEY CLUSTERED ([login_token] ASC, [platform_type] ASC)
)
