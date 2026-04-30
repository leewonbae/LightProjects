namespace pray_server.Commons
{
    public class SharedConstants
    {
    }

    public enum E_LOGIN_STATUS_TYPE
    {
        NONE = 0,
        LOGINED = 1,
        DUPLICATED = 2,
        KICKED = 3,
        BANNED = 4
    }

    public enum E_LOGIN_PLATFORM_TYPE
    {
        DEFAULT = 0,
        GOOGLE = 1,
        APPLE = 2,
        FACEBOOK = 3,
        TWITTER = 4
    }
}
