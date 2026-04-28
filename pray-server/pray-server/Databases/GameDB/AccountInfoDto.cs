using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace pray_server.Databases.GameDB
{
    [Table("account_info")]
    [PrimaryKey(nameof(Id))]
    public class AccountInfoDto
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("nickname")]
        public string Nickname { get; set; } = string.Empty;
    }
}
