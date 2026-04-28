using Microsoft.EntityFrameworkCore;
using pray_server.Databases;
using pray_server.Databases.GameDB;
using pray_server.Exceptions;
using pray_server.Extensions;
using System.Threading.Tasks;

namespace pray_server.Managers
{
    [InjectableClass(ServiceLifetime.Singleton)]
    public class ProductManager
    {
        private IDbContextFactory<GameDbContext> _gameDbContextFactory;
        public ProductManager(IDbContextFactory<GameDbContext> gameDbContextFactory)
        {
            _gameDbContextFactory = gameDbContextFactory;
        }

        public void Identify(string accountToken)
        {

        }

        public async Task<AccountInfoDto> Register(string accountToken)
        {
            using GameDbContext gameDbContext = _gameDbContextFactory.CreateDbContext();

            var accountInfoDto = new AccountInfoDto()
            {
                Nickname = "test"
            };

            using var transaction = await gameDbContext.Database.BeginTransactionAsync();
            try
            {
                var id = await gameDbContext.InsertAccountWithIdAsync(accountInfoDto);
                accountInfoDto.Id = id;

                accountInfoDto.Nickname = "test2";
                await gameDbContext.UpdateAccountAsync(accountInfoDto);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                throw new GameServerException(Snowpipe.Commons.Packets.E_PACKET_ERROR_CODE.SERVER_ERROR, "Server Error");
            }
            finally
            {
                await transaction.DisposeAsync();
            }

            return accountInfoDto;
        }

        public void Login()
        {

        }
    }
}
