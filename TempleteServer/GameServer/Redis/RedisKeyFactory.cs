namespace GameServer.Redis
{

    public static class RedisKeyFactory
    {
        public static string _prefix = string.Empty;

        public static void SetPrefix(string projectName, string environmentName)
        {
            if (environmentName.StartsWith(""))
                _prefix = $"{projectName}:{environmentName}:";
        }

        public static string GetAccountInfoKey(string sessionToken)
        {
            return string.Concat(_prefix, $"ACCOUNT_INFO:{sessionToken}");
        }

    }
}
