namespace pray_server.Helpers;

public static class ServerDateTime
{
    private static DateTime s_customServerDateTimeStamp;
    private static DateTime s_originServerDateTimeStamp;

    private static bool s_isIgnoredCustomDateTime = false;

    static ServerDateTime()
    {
        Reset();
    }

    public static DateTime Now
    {
        get
        {
            if (s_isIgnoredCustomDateTime)
            {
                return DateTime.Now;
            }

            var diff = DateTime.Now - s_originServerDateTimeStamp;
            return s_customServerDateTimeStamp.AddMilliseconds(diff.TotalMilliseconds);
        }
    }

    public static void SetIgnoreCustomDateTime(bool isIgnoredCustomDateTime)
    {
        s_isIgnoredCustomDateTime = isIgnoredCustomDateTime;
    }

    public static void Reset()
    {
        SetServerDateTime(DateTime.Now);
    }

    public static void SetServerDateTime(DateTime newServerDateTime)
    {
        if (s_isIgnoredCustomDateTime)
        {
            return;
        }

        s_customServerDateTimeStamp = newServerDateTime;
        s_originServerDateTimeStamp = DateTime.Now;
    }
}

