using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using pray_server.Databases.Models.AccountDB;
using pray_server.Databases.Models.GameDB;
using System.Threading.Tasks;

namespace pray_server.Databases.DbContexts
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {
        }
        // Define your DbSets here, for example:
        // public DbSet<Player> Players { get; set; }
        //private DbSet<AccountInfoDto> AccountInfoDtoSet { get; set; }
        public async Task<GameAccountDto?> SelectAccountInfoAsync(long accountId)
        {
            var list = await Database.SqlQuery<GameAccountDto>($"EXEC usp_select_game_account {accountId}").ToListAsync();

            return list.FirstOrDefault();
        }

        public async Task UpsertGameAccountAsync(GameAccountDto gameAccountDto)
        {
            await Database.ExecuteSqlAsync($"EXEC usp_upsert_game_account {gameAccountDto.AccountId},{gameAccountDto.Nickname},{gameAccountDto.Level},{gameAccountDto.Exp},{gameAccountDto.LastLoginDt},{gameAccountDto.CreateDt}");
        }
    }
}
