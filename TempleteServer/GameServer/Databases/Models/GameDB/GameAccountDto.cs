using GameServer.Databases.Models.AccountDB;
using Microsoft.EntityFrameworkCore;
using Snowpipe.Commons;
using Snowpipe.Commons.Packets;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Databases.Models.GameDB
{
    [Table("game_account")]
    [PrimaryKey(nameof(AccountId))]
    public class GameAccountDto
    {
        [Column("account_id")]
        public long AccountId { get; set; }     // 유저 ID
        [Column("nickname")]
        public string Nickname { get; set; }    // 유저 닉네임
        [Column("level")]
        public int Level { get; set; }          // 유저 레벨
        [Column("exp")]
        public int Exp { get; set; }
        [Column("last_login_dt")]
        public DateTime? LastLoginDt { get; set; }
        [Column("create_dt")]
        public DateTime CreateDt { get; set; }

        public GameAccountDto() { }
        public GameAccountDto(long accountId, DateTime serverDt, string nickName)
        {
            AccountId = accountId;
            Nickname = nickName;
            Level = 1;
            CreateDt = serverDt;
            Exp = 0;
            LastLoginDt = null;
        }

        public static GameAccountDto CreateDto(AccountDto accountDto, DateTime serverDt)
        {
            return new GameAccountDto(accountDto.Id, serverDt, accountDto.Nickname);
        }

        public void SetLastLoginDt(DateTime serverDt)
        {
            LastLoginDt = serverDt;
        }

        public GameAccountVo ToVo()
        {
            return new GameAccountVo
            {
                Nickname = Nickname,
                Level = Level,
                CreateDt = CreateDt,
                Exp = Exp,
                LastLoginDt = LastLoginDt,
            };
        }
    }
}
