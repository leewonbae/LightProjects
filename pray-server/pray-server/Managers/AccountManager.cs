using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using pray_server.Commons;
using pray_server.Databases;
using pray_server.Exceptions;
using pray_server.Extensions;
using Snowpipe.Commons.Packets;

namespace pray_server.Managers
{
    public interface ILoginVerifier
    {
        string VerifyToken(string token);
    }

    [InjectableClass(ServiceLifetime.Singleton)]
    public class AccountManager
    {
        private readonly GoogleLoginVerifier _googleLoginVerifier;
        private readonly AppleLoginVerifier _appleLoginVerifier;
        private readonly DefaultLoginVerifier _defaultLoginVerifier;

        private IDbContextFactory<GameDbContext> _gameDbContextFactory;

        public AccountManager(IServiceProvider serviceProvider)
        {
            _googleLoginVerifier = serviceProvider.GetRequiredService<GoogleLoginVerifier>();
            _appleLoginVerifier = serviceProvider.GetRequiredService<AppleLoginVerifier>();
            _defaultLoginVerifier = serviceProvider.GetRequiredService<DefaultLoginVerifier>();
        }

        public void Register(ReqRegister req)
        {
            // 플랫폼 별  토큰 확인 
            var loginToken = GetLoginTokenByPlatformToken(req.PlatformType, req.PlatformToken);

            // DB에 로그인 토큰이 존재하는지 확인

            // 신규 계정 생성 
            // accountDB 에 추가, - 플랫폼 별 정보
            // gameDB 에 추가 - 계정 별 게임 정보

        }





        private string GetLoginTokenByPlatformToken(E_LOGIN_PLATFORM_TYPE platformType, string platformToken)
        {
            if (platformToken.IsNullOrEmpty())
            {
                throw new GameServerException(E_PACKET_ERROR_CODE.PLATFORM_LOGIN_EMPTY_TOKEN, $"Invalid Platform type Req platformToken  [{platformToken}]");
            }

            ILoginVerifier loginVerifier;
            switch (platformType)
            {
                case E_LOGIN_PLATFORM_TYPE.GOOGLE:
                    loginVerifier = _googleLoginVerifier;
                    break;
                case E_LOGIN_PLATFORM_TYPE.APPLE:
                    loginVerifier = _appleLoginVerifier;
                    break;
                case E_LOGIN_PLATFORM_TYPE.DEFAULT:
                    loginVerifier = _defaultLoginVerifier;
                    break;
                default:
                    throw new GameServerException(E_PACKET_ERROR_CODE.PLATFORM_LOGIN_NOT_FOUND_VERIFIER,
                       $"Not Found LoginVerifier PlatformType [{platformType}]");
            }

            return loginVerifier.VerifyToken(platformToken);
        }
    }
}
