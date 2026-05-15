using System;
using System.Collections.Generic;
using System.Linq;

namespace Snowpipe.Commons.Utils
{
    public static class RandomUtils
    {
        public static IEnumerable<T> SelectAccumulateRandom<T>(IEnumerable<T> source, Func<T, int> weightSelector, Random rand)
        {
            var total = source.Sum(weightSelector);
            var randNum = rand.Next(0, total);
            yield return source.FirstOrDefault(i => (randNum -= weightSelector(i)) < 0);
        }
    }
}

