namespace Snowpipe.Commons.Packets
{
    public enum E_PACKET_ERROR_CODE
    {
        SUCCESS = 0,
        //공용 System Error는 1000 이하로 지정한다.
        SERVER_ERROR = 1,
        INVALID_PACKET = 10,
        PLATFORM_LOGIN_EMPTY_TOKEN = 11,
        PLATFORM_LOGIN_NOT_FOUND_VERIFIER = 12,
        PLATFORM_LOGIN_GOOGLE_EMAIL_FAIL_VERIFIED = 13,
        PLATFORM_LOGIN_INVALID_ISSUER = 14,
        PLATFORM_LOGIN_EXPIRATION_TOKEN = 15,
        PLATFORM_LOGIN_APPLE_CERT_KEY_ERROR = 16,
        ALREADY_EXISTS_ACCOUNT_LINK = 17,
        
        // TODO : 기존에 1000 이하로 지정하던 부분이 꼬인듯? 수정 필요
        END_OF_SYSTEM_ERROR = 999,
        
        // account
        NOT_FOUND_ACCOUNT = 1000,
        ALREADY_EXISTS_ACCOUNT = 1001,
        REGISTER_FAILED = 1002,
        NEED_TO_LOGIN = 1003,
        DUPLICATED_LOGIN = 1004,
        KICKED_TOKEN = 1005,
        BANNED_TOKEN = 1006,
    }
}
