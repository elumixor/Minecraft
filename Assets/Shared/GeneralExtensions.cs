using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

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

        /// <summary>
        /// Destroys all children of transform
        /// </summary>
        /// <param name="transform">Transform, that should be cleared of children</param>
        /// <param name="destroyImmediate">If true, performs DestroyImmediate, else performs regular Destroy</param>
        public static void DestroyAllChildren(this Transform transform, bool destroyImmediate = false) {
            foreach (Transform child in transform) {
                if (destroyImmediate) Object.DestroyImmediate(child.gameObject);
                Object.Destroy(child.gameObject);
            }
        }
    }
}