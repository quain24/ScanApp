using System;
using System.Collections.Generic;

namespace ScanApp.Tests.Extensions
{
    public static class IListExtensions
    {
        public static IList<T> ShuffleMe<T>(this IList<T> list)
        {
            var random = new Random();
            var n = list.Count;

            for (var i = list.Count - 1; i > 1; i--)
            {
                var rnd = random.Next(i + 1);

                var value = list[rnd];
                list[rnd] = list[i];
                list[i] = value;
            }

            return list;
        }
    }
}