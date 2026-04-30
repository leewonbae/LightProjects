using Microsoft.EntityFrameworkCore;
using pray_server.Commons;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pray_server.Databases.Models.AccountDB;

[Table("account_link")]
[PrimaryKey(nameof(LoginToken), nameof(LoginPlatFormType))]
public class AccountLinkDto
{
    [Column("login_token")]
    public string LoginToken { get; set; }  // 플랫폼 로그인 토큰 (Google, Apple, Default 등)
    [Column("platform_type")]
    public E_LOGIN_PLATFORM_TYPE LoginPlatFormType { get; set; }
    [Column("account_id")]
    public long AccountId { get; set; }     // 연결된 계정 ID (account 테이블의 id)
    [Column("create_dt")]
    public DateTime CreateDt { get; set; }
}
