using System;
using UnityEngine;

namespace Shared.Positioning {
    /// <summary>
    /// Describes position in world coordinates
    /// </summary>
    [Serializable]
    public readonly partial struct WorldPosition {
        public const uint ChunkSize = 20;

        public readonly ChunkPosition chunkPosition;
        public readonly LocalPosition localPosition;

        public WorldPosition(ChunkPosition chunkPosition, LocalPosition localPosition) {
            this.localPosition = localPosition;
            this.chunkPosition = chunkPosition;
        }

        public WorldPosition(int x, int y, int z) {
            var chunkX = Mathf.FloorToInt((float) x / ChunkSize);
            var chunkY = Mathf.FloorToInt((float) y / ChunkSize);
            var chunkZ = Mathf.FloorToInt((float) z / ChunkSize);

            chunkPosition = new ChunkPosition(chunkX, chunkY, chunkZ);
            localPosition = new LocalPosition((uint) (x - chunkX * ChunkSize),
                (uint) (y - chunkY * ChunkSize), (uint) (z - chunkX * ChunkSize));
        }

        public static readonly WorldPosition Right = (WorldPosition) Vector3.right;
        public static readonly WorldPosition Left = (WorldPosition) Vector3.left;
        public static readonly WorldPosition Up = (WorldPosition) Vector3.up;
        public static readonly WorldPosition Down = (WorldPosition) Vector3.down;
        public static readonly WorldPosition Forward = (WorldPosition) Vector3.forward;
        public static readonly WorldPosition Back = (WorldPosition) Vector3.back;

        public static readonly WorldPosition[] AdjoiningVectors = {Right, Left, Up, Down, Forward, Back};

        #region Equality Comparison
        public static bool operator ==(WorldPosition a, WorldPosition b) =>
            a.chunkPosition == b.chunkPosition && a.localPosition == b.localPosition;
        public static bool operator !=(WorldPosition a, WorldPosition b) => !(a == b);

        public override bool Equals(object obj) => obj is WorldPosition other && this == other;
        public override int GetHashCode() {
            unchecked {
                return (chunkPosition.GetHashCode() * 397) ^ localPosition.GetHashCode();
            }
        }
        #endregion

        #region Conversions
        public static explicit operator Vector3(WorldPosition position) =>
            ChunkSize * (Vector3) position.chunkPosition + (Vector3) position.localPosition;

        public static explicit operator Vector3Int(WorldPosition position) =>
            (int) ChunkSize * (Vector3Int) position.chunkPosition + (Vector3Int) position.localPosition;

        public static explicit operator WorldPosition(Vector3 position) {
            var round = Vector3Int.RoundToInt(position);
            var chunkPosition = new ChunkPosition(
                Mathf.FloorToInt((float) round.x / ChunkSize),
                Mathf.FloorToInt((float) round.y / ChunkSize),
                Mathf.FloorToInt((float) round.z / ChunkSize));
            var localPosition = new LocalPosition(
                (uint) (round.x - chunkPosition.x * ChunkSize),
                (uint) (round.y - chunkPosition.y * ChunkSize),
                (uint) (round.z - chunkPosition.z * ChunkSize));
            return new WorldPosition(chunkPosition, localPosition);
        }

        public static explicit operator WorldPosition(Vector3Int position) => (WorldPosition) (Vector3) position;
        #endregion

        public static WorldPosition operator +(WorldPosition a, WorldPosition b) {
            var localPosition = a.localPosition.Add(b.localPosition, out var offset);
            return new WorldPosition(a.chunkPosition + b.chunkPosition + offset, localPosition);
        }

        public static WorldPosition operator -(WorldPosition a, WorldPosition b) {
            var localPosition = a.localPosition.Subtract(b.localPosition, out var offset);
            return new WorldPosition(a.chunkPosition - b.chunkPosition + offset, localPosition);
        }

        public override string ToString() => $"{chunkPosition}: {localPosition}";
        public void Deconstruct(out ChunkPosition outChunkPosition, out LocalPosition outLocalPosition) {
            outChunkPosition = chunkPosition;
            outLocalPosition = localPosition;
        }
    }
}