using System;
using System.Collections.Generic;

namespace Shared {
    public static class GeneralExtensions {


        /// <summary>
        /// Takes an index of enum value and retrieves value from array with that index
        /// </summary>
        /// <example>
        /// <code>
        /// enum Foo {
        ///    A,
        ///    B,
        ///    C
        /// }
        ///
        ///
        /// ...
        ///
        ///
        /// var a = Foo.A;
        /// var values = new [] {"a", "b", "c"};
        ///
        /// Console.WriteLine(a.ArrayValueIn(values)); // "a"
        /// </code>
        /// </example>
        /// <returns></returns>
        public static TResult ArrayValueIn<TEnum, TResult>(this TEnum enumValue, IList<TResult> values) where TEnum : Enum {
            var types = (TEnum[]) Enum.GetValues(typeof(TEnum));
            for (var i = 0; i < types.Length; i++)
                if (Equals(types[i], enumValue))
                    return values[i];
            return default;
        }
    }
}