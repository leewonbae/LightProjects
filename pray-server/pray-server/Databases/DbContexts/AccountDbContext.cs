using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using GameServer.Commons;
using GameServer.Databases.Models.AccountDB;
using Snowpipe.Commons.Packets;
using System.Threading.Tasks;

namespace GameServer.Databases.DbContexts
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
            var result = await Database.SqlQuery<long>($"EXEC dbo.usp_insert_account {emptyAccountDto.Nickname},{emptyAccountDto.AccountKey}, {0}, {emptyAccountDto.RegistDt}").ToListAsync();

            return result.FirstOrDefault();
        }

        public async Task InsertAccountLinkAsync(string loginToken, E_LOGIN_PLATFORM_TYPE loginPlatformType, long accountId, DateTime serverDt)
        {
            await Database.ExecuteSqlAsync($"EXEC dbo.usp_insert_account_link {loginToken}, {loginPlatformType}, {accountId}, {serverDt}");
        }

        public async Task<AccountDto?> SelectAccountAsync(long id)
        {
            var result = await Database.SqlQuery<AccountDto>($"EXEC dbo.usp_select_account {id}").ToListAsync();

            return result.FirstOrDefault();
        }

        public async Task UpdateAccountAsync(AccountDto accountDto)
        {
            await Database.ExecuteSqlAsync($"EXEC dbo.usp_update_account {accountDto.Id}, {accountDto.Idfa},{accountDto.Adid}, {accountDto.FCMToken}, {accountDto.SessionToken}");
        }
    }
}
