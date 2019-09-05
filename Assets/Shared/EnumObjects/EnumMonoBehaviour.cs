using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shared.EnumObjects {
    /// <summary>
    /// <see cref="MonoBehaviour"/>, that maps some enum value to objects, acts like a <see cref="Dictionary{TKey,TValue}"/>
    /// </summary>
    /// <typeparam name="TKeyEnum">Enum type</typeparam>
    /// <typeparam name="TValue">Mapped return object type</typeparam>
    public abstract class EnumMonoBehaviour<TKeyEnum, TValue> : MonoBehaviour where TKeyEnum : Enum {
        /// <summary>
        /// Values storage
        /// </summary>
        [SerializeField] protected TValue[] values;

        /// <summary>
        /// Get object, mapped to enum
        /// </summary>
        public TValue this[TKeyEnum enumValue] => enumValue.ArrayValueIn(values);
    }
}