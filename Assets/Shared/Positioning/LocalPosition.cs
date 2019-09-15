using System;
using UnityEngine;

namespace Shared.Positioning {
    public readonly partial struct WorldPosition {
        /// <summary>
        /// Position in chunk
        /// </summary>
        [Serializable]
        public readonly struct LocalPosition {
            private const uint MaxIndex = ChunkSize * ChunkSize * ChunkSize - 1;

            public readonly uint x;
            public readonly uint y;
            public readonly uint z;

            public uint Index => x * ChunkSize * ChunkSize + y * ChunkSize + z;

            public LocalPosition(uint x, uint y, uint z) {
                Debug.Assert(x < ChunkSize, $"Local position's x ({x}) was greater or equal to ChunkSize {ChunkSize}");
                Debug.Assert(y < ChunkSize, $"Local position's x ({x}) was greater or equal to ChunkSize {ChunkSize}");
                Debug.Assert(z < ChunkSize, $"Local position's x ({x}) was greater or equal to ChunkSize {ChunkSize}");

                this.x = x;
                this.y = y;
                this.z = z;
            }
            public LocalPosition(uint index) {
                Debug.Assert(index <= MaxIndex,
                    $"Local position's index ({index}) was greater than maximum index {MaxIndex}");

                x = index / (ChunkSize * ChunkSize);
                y = (index - x * ChunkSize * ChunkSize) / ChunkSize;
                z = index - x * ChunkSize * ChunkSize - y * ChunkSize;
            }

            public static readonly LocalPosition Zero = new LocalPosition(0, 0, 0);
            public static readonly LocalPosition One = new LocalPosition(1, 1, 1);
            public static readonly LocalPosition Right = new LocalPosition(1, 0, 0);
            public static readonly LocalPosition Top = new LocalPosition(0, 1, 0);
            public static readonly LocalPosition Forward = new LocalPosition(0, 0, 1);

            #region Equality Comparison
            public static bool operator ==(LocalPosition a, LocalPosition b) => a.Index == b.Index;
            public static bool operator !=(LocalPosition a, LocalPosition b) => !(a == b);

            public override bool Equals(object obj) => obj is LocalPosition other && this == other;
            public override int GetHashCode() => (int) Index;
            #endregion

            public static explicit operator Vector3(LocalPosition position) =>
                new Vector3(position.x, position.y, position.z);
            public static explicit operator Vector3Int(LocalPosition position) =>
                new Vector3Int((int) position.x, (int) position.y, (int) position.z);

            public LocalPosition Add(LocalPosition other, out ChunkPosition offset) {
                var totalX = x + other.x;
                var totalY = y + other.y;
                var totalZ = z + other.z;

                offset = new ChunkPosition((int) (totalX / ChunkSize),
                    (int) (totalY / ChunkSize), (int) (totalZ / ChunkSize));

                return new LocalPosition(totalX % ChunkSize, totalY % ChunkSize, totalZ % ChunkSize);
            }
            public LocalPosition Subtract(LocalPosition other, out ChunkPosition offset) {
                uint totalX;
                uint totalY;
                uint totalZ;

                int chunkX;
                int chunkY;
                int chunkZ;

                if (x > other.x) {
                    totalX = x - other.x;
                    chunkX = (int) (totalX / ChunkSize);
                } else {
                    totalX = other.x - x;
                    chunkX = -(int) (totalX / ChunkSize);
                }

                if (y > other.y) {
                    totalY = y - other.y;
                    chunkY = (int) (totalY / ChunkSize);
                } else {
                    totalY = other.y - y;
                    chunkY = -(int) (totalY / ChunkSize);
                }

                if (z > other.z) {
                    totalZ = z - other.z;
                    chunkZ = (int) (totalZ / ChunkSize);
                } else {
                    totalZ = other.z - z;
                    chunkZ = -(int) (totalZ / ChunkSize);
                }

                offset = new ChunkPosition(chunkX, chunkY, chunkZ);

                return new LocalPosition(totalX % ChunkSize, totalY % ChunkSize, totalZ & ChunkSize);
            }

            public void Deconstruct(out uint outX, out uint outY, out uint outZ) {
                outX = x;
                outY = y;
                outZ = z;
            }

            public override string ToString() => $"[{x}, {y}, {z}]";
        }
    }
}