using System;
using System.Collections.Generic;

namespace Shared {
    public static class NumericCacher {
        private static readonly Dictionary<ulong, ulong> SqrtDictionary = new Dictionary<ulong, ulong>();
        public static ulong Sqrt(this ulong value) =>
            SqrtDictionary.TryGetValue(value, out var result)
                ? result
                : SqrtDictionary[value] = (ulong) Math.Floor(Math.Sqrt(value));
        public static ulong Sqrt(this uint value) => ((ulong) value).Sqrt();
        public static ulong Sqrt(this int value) =>
            value < 0
                ? throw new ArgumentOutOfRangeException(nameof(value), "Value should  be non-negative")
                : ((long) value).Sqrt();
        public static ulong Sqrt(this long value) =>
            value < 0
                ? throw new ArgumentOutOfRangeException(nameof(value), "Value should  be non-negative")
                : ((ulong) value).Sqrt();
    }
}