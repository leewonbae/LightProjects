using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Commons.SharedExtentions
{
    public static class CollectionExtentions
    {
        public static bool HasValue<T>(T collection) where T : ICollection
        {
            if (collection == null)
            {
                return false;
            }

            if (collection.Count <= 0)
            {
                return false;
            }

            return true;
        }

        public static T Random<T>(this IList<T> list)
        {
            if (list.Count == 0)
            {
                throw new IndexOutOfRangeException("List needs at least one entry to call Random()");
            }

            if (list.Count == 1)
            {
                return list[0];
            }

            var random = new Random();
            return list[random.Next(0, list.Count)];
        }

        // 다음 데이터가 있는지 없는지 체크 + 있다면 다음 데이터 Peek()
        public static T PeekNext<T>(this Queue<T> queue) where T : class
        {
            if (queue == null)
            {
                return null;
            }

            if (queue.Count() <= 0)
            {
                return null;
            }

            return queue.Peek();
        }
    }
}