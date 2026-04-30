using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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
        public async Task<GameAccountDto?> SelectAccountInfoAsync(long id)
        {
            return await Database.SqlQuery<GameAccountDto>($"EXEC usp_select_account_info {id}").FirstOrDefaultAsync();
        }

        public async Task<long> InsertAccountWithIdAsync(GameAccountDto account)
        {
            return await Database.SqlQuery<long>($"EXEC usp_insert_account_info {account.Nickname}").FirstOrDefaultAsync();
        }

        public async Task UpdateAccountAsync(GameAccountDto account)
        {
            await Database.ExecuteSqlAsync($"EXEC usp_update_account_info {account.Nickname}");
        }
    }
}
