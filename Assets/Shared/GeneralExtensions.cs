using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


        /// <summary>
        /// Sets alpha value of color
        /// </summary>
        public static Color SetAlpha(this Color color, float alpha) {
            color.a = alpha;
            return color;
        }

        /// <summary>
        /// Sets alpha value of image's color
        /// </summary>
        public static void SetColorAlpha(this Image image, float alpha) => image.color = image.color.SetAlpha(alpha);
    }
}