using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace pray_server.Databases.Models.GameDB
{
    [Table("game_account")]
    [PrimaryKey(nameof(AccountId))]
    public class GameAccountDto
    {
        [Column("account_id")]
        public long AccountId { get; set; }     // 유저 ID
        [Column("nickname")]
        public string Nickname { get; set; }    // 유저 닉네임
        [Column("lv")]
        public int Level { get; set; }          // 유저 레벨
        [Column("create_dt")]
        public DateTime CreateDt { get; set; }
        [Column("exp")]
        public int Exp { get; set; }
        [Column("last_login_dt")]
        public DateTime? LastLoginDt { get; set; }
        [Column("tutorial_step")]
        public int TutorialStep { get; set; }
    }
}
