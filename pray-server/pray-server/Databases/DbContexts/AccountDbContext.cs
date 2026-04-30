using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using pray_server.Databases.Models.AccountDB;
using Snowpipe.Commons.Packets;
using System.Threading.Tasks;

namespace pray_server.Databases.DbContexts
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
        {
        }

        public async Task<AccountLinkDto?> SelectAccountLinkByLoginTokenAsync(string loginToken)
        {
            var list = await Database.SqlQuery<AccountLinkDto>($"EXEC dbo.usp_select_account_link {loginToken}").ToListAsync();

            return list.FirstOrDefault();
        }

        public async Task<long> InsertAndSelectAccountIdAsync(AccountDto emptyAccountDto)
        {
            var result = await Database.SqlQuery<long>($"EXEC dbo.usp_insert_account {emptyAccountDto.AccountKey}, {0}, {emptyAccountDto.Nickname}, {emptyAccountDto.RegistDt}").ToListAsync();

            return result.FirstOrDefault();
        }
    }
}
