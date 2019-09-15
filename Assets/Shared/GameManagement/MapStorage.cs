using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shared.Positioning;

namespace Shared.GameManagement {
    public class MapStorage<T> : IEnumerable<(ulong index, MapStorage<T>.Chunk chunk)> {
        public class Chunk : IEnumerable<(uint index, T data)> {
            private readonly SortedDictionary<uint, T> values = new SortedDictionary<uint, T>();
            public int Count => values.Count;
            public bool TryGetValue(WorldPosition.LocalPosition position, out T value) =>
                values.TryGetValue(position.Index, out value);
            public bool ContainsKey(WorldPosition.LocalPosition position) => values.ContainsKey(position.Index);
            public T this[WorldPosition.LocalPosition position] {
                get => values[position.Index];
                set => values[position.Index] = value;
            }
            public bool Remove(WorldPosition.LocalPosition position) => values.Remove(position.Index);
            public IEnumerator<(uint index, T data)> GetEnumerator() =>
                values.Select(kvp => (kvp.Key, kvp.Value)).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => values.GetEnumerator();
        }

        private readonly SortedDictionary<ulong, Chunk> storage =
            new SortedDictionary<ulong, Chunk>();

        public bool TryGetChunk(WorldPosition.ChunkPosition position, out Chunk value) =>
            storage.TryGetValue(position.Index, out value);

        public bool TryGetValue(WorldPosition position, out T value) {
            var (outChunkPosition, outLocalPosition) = position;
            if (!storage.TryGetValue(outChunkPosition.Index, out var dict)) {
                value = default;
                return false;
            }

            return dict.TryGetValue(outLocalPosition, out value);
        }
        public T this[WorldPosition position] {
            get => storage[position.chunkPosition.Index][position.localPosition];
            set {
                if (storage.TryGetValue(position.chunkPosition.Index, out var dict))
                    dict[position.localPosition] = value;
                else
                    (storage[position.chunkPosition.Index] = new Chunk())[position.localPosition] = value;
            }
        }

        public Chunk this[in WorldPosition.ChunkPosition chunkPosition] {
            get => storage[chunkPosition.Index];
            set => storage[chunkPosition.Index] = value;
        }
        public int Count => storage.Count;
        /// <summary>
        /// True if there is data at world position
        /// </summary>
        public bool ContainsKey(in WorldPosition position) =>
            storage.TryGetValue(position.chunkPosition.Index, out var dict)
            && dict.ContainsKey(position.localPosition);
        /// <summary>
        /// True if there is <see cref="Chunk"/> at chunk position
        /// </summary>
        public bool ContainsKey(in WorldPosition.ChunkPosition position) => storage.ContainsKey(position.Index);
        /// <summary>
        /// Removes data at world position
        /// </summary>
        public bool Remove(in WorldPosition position) =>
            storage.TryGetValue(position.chunkPosition.Index, out var dict)
            && dict.Remove(position.localPosition);
        /// <summary>
        /// Removes data at world position
        /// </summary>
        public bool Remove(in WorldPosition.ChunkPosition position) => storage.Remove(position.Index);
        public IEnumerator<(ulong index, Chunk chunk)> GetEnumerator() =>
            storage.Select(kvp => (kvp.Key, kvp.Value)).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}