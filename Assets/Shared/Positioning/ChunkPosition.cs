using System;
using Shared.SpaceWrapping;
using UnityEngine;

namespace Shared.Positioning {
    public readonly partial struct WorldPosition {
        /// <summary>
        /// Position of chunk
        /// </summary>
        [Serializable]
        public readonly struct ChunkPosition {
            public readonly int x;
            public readonly int y;
            public readonly int z;
            public ulong Index => Wrapper.Wrap(x, y, z);

            public ChunkPosition(int x, int y, int z) {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public ChunkPosition(ulong index) => Wrapper.Unwrap(index, out x, out y, out z);

            public static readonly ChunkPosition Zero = new ChunkPosition(0, 0, 0);
            public static readonly ChunkPosition One = new ChunkPosition(1, 1, 1);
            public static readonly ChunkPosition Right = new ChunkPosition(1, 0, 0);
            public static readonly ChunkPosition Left = new ChunkPosition(-1, 0, 0);
            public static readonly ChunkPosition Up = new ChunkPosition(0, 1, 0);
            public static readonly ChunkPosition Down = new ChunkPosition(0, -1, 0);
            public static readonly ChunkPosition Forward = new ChunkPosition(0, 0, 1);
            public static readonly ChunkPosition Back = new ChunkPosition(0, 0, -1);

            #region Equality Comparison
            public static bool operator ==(ChunkPosition a, ChunkPosition b) => a.Index == b.Index;
            public static bool operator !=(ChunkPosition a, ChunkPosition b) => !(a == b);

            public override bool Equals(object obj) => obj is ChunkPosition other && this == other;
            public override int GetHashCode() => (int) Index;
            #endregion


            public static explicit operator Vector3(ChunkPosition position) =>
                new Vector3(position.x, position.y, position.z);
            public static explicit operator Vector3Int(ChunkPosition position) =>
                new Vector3Int(position.x, position.y, position.z);

            public static ChunkPosition operator -(ChunkPosition a) => new ChunkPosition(-a.x, -a.y, -a.z);

            public static ChunkPosition operator +(ChunkPosition a, ChunkPosition b) =>
                new ChunkPosition(a.x + b.x, a.y + b.y, a.z + b.z);

            public static ChunkPosition operator -(ChunkPosition a, ChunkPosition b) => a + -b;

            public void Deconstruct(out int outX, out int outY, out int outZ) {
                outX = x;
                outY = y;
                outZ = z;
            }
            public override string ToString() => $"({x}, {y}, {z})";
        }
    }
}