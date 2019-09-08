using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shared {
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
        [SerializeField] private List<TKey> keys = new List<TKey>();

        [SerializeField] private List<TValue> values = new List<TValue>();

        // save the dictionary to lists
        public void OnBeforeSerialize() {
            keys.Clear();
            values.Clear();
            foreach (var pair in this) {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize() {
            Clear();

            if (keys.Count != values.Count)
                throw new Exception(
                    $"there are {keys.Count} keys and {values.Count} values after deserialization. Make sure that both key and value types are serializable.");

            for (var i = 0; i < keys.Count; i++) Add(keys[i], values[i]);
        }
    }
}