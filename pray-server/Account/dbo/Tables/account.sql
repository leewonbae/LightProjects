CREATE TABLE [dbo].[account]
(
    [id]            BIGINT    IDENTITY (1, 1) NOT NULL,
    [account_type]  INT       NOT NULL,
    [account_key]   NVARCHAR (50) NOT NULL,
    [game_db_idx]   INT       NOT NULL,
    [nickname]      NVARCHAR (50) NOT NULL,
    [adid]          NVARCHAR (50) NULL,
    [idfa]          NVARCHAR (50) NULL,
    [fcm_token]     NVARCHAR (50) NULL,
    [account_token] NVARCHAR (50) NULL,
    [reg_dt]        DATETIME      NULL,
    [is_deleted]    BIT NOT NULL DEFAULT 0, 
    PRIMARY KEY CLUSTERED ([id] ASC)
)
