using System;

namespace GameServer.Commons.SharedUtils
{
    public static class MathUtils
    {
        public static double Round(double value, int digit)
        {
            return Math.Round(value, digit, MidpointRounding.AwayFromZero);
        }

        public static float Round(float value, int digit)
        {
            return MathF.Round(value, digit, MidpointRounding.AwayFromZero);
        }
    }
}