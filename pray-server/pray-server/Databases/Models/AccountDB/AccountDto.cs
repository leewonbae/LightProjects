using Microsoft.EntityFrameworkCore;
using Snowpipe.Commons.Packets;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Databases.Models.AccountDB;

[Table("account")]
[PrimaryKey(nameof(Id))]
public class AccountDto
{
    [Key]
    [Column("id")]
    public long Id { get; set; }      // 유저 ID
    [Column("nickname")]
    public string Nickname { get; set; }

    [Column("account_type")]
    public int AccountType { get; set; }    // 유저 계정 타입 (0: 일반, 1: GM)

    [Column("account_key")]
    public string AccountKey { get; set; }  // 유저 GUID

    [Column("game_db_idx")]
    public int GameDbIdx { get; set; }       // 유저 GameDB idx 

    [Column("adid")]
    public string? Adid { get; set; }        // 유저 광고 ID (aos)

    [Column("idfa")]
    public string? Idfa { get; set; }        // 유저 광고 ID (ios)

    [Column("fcm_token")]
    public string? FCMToken { get; set; }    // 유저 푸시 토큰 (FCM)

    [Column("session_token")]
    public string? SessionToken { get; set; }    // 로그인 할 때 발급되는 토큰 

    [Column("regist_dt")]
    public DateTime RegistDt { get; set; } //유저 등록 날짜 
    public AccountDto() { }
    public AccountDto(string accountKey, DateTime serverDt, ReqRegister req)
    {
        AccountType = 0;
        AccountKey = accountKey;
        Adid = string.Empty;
        Idfa = string.Empty;
        FCMToken = string.Empty;
        SessionToken = string.Empty;
        Nickname = req.Nickname;
        RegistDt = serverDt;
    }

    public void SetAccountId(long id)
    {
        Id = id;
    }

    public void SetSessionToken(string sessionToken)
    {
        SessionToken = sessionToken;
    }
}
