using System;
using System.Collections.Generic;

namespace Shared {
    public static class NumericCacher {
        private static readonly Dictionary<long, long> SqrtDictionary = new Dictionary<long, long>();
        public static long Sqrt(this long value) {
            if (SqrtDictionary.ContainsKey(value)) return SqrtDictionary[value];
            return SqrtDictionary[value] = (long) Math.Floor(Math.Sqrt(value));
        }

        public static int Sqrt(this int value) => (int) ((long) value).Sqrt();
    }
}