using System.Collections.Generic;
using Shared.Blocks;

namespace Shared.GameManagement {
    // todo: use dictionary internally, but implement IDictionary, etc.
    // todo: use custom structure for values as well
    public class MapStorage<T> : SortedDictionary<long, SortedDictionary<int, T>> {
        public bool TryGetValue(long key1, int key2, out T value) {
            if (!TryGetValue(key1, out var dict)) {
                value = default;
                return false;
            }

            return dict.TryGetValue(key2, out value);
        }

    }
}