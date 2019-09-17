using System.Collections;
using Shared.Blocks;
using Shared.Positioning;
using Shared.SingletonBehaviour;

namespace Shared.GameManagement {
    public class Player : SingletonBehaviour<Player> {
        private static WorldPosition position;

        /// <summary>
        /// Player position
        /// </summary>
        public static WorldPosition Position {
            get => position;
            set {
                if (position.chunkPosition != value.chunkPosition)
                    Instance.StartCoroutine(OnChunkChanged(position.chunkPosition, value.chunkPosition));
                position = value;
            }
        }

        private static IEnumerator OnChunkChanged(WorldPosition.ChunkPosition previous,
            WorldPosition.ChunkPosition current) {
            IEnumerator Change(int dx, int dy, int dz, int cx, int cy, int cz) {
                Block.DestroyChunk(previous + new WorldPosition.ChunkPosition(dx, dy, dz));
                yield return null;

                var pos = current + new WorldPosition.ChunkPosition(cx, cy, cz);
                if (pos.y >= 0) {
                    Block.InstantiateChunk(Map.GetChunk(pos), pos);
                }

                yield return null;
            }

            var diff = current - previous;
            if (diff.x != 0) {
                for (var z = -1; z <= 1; z++)
                for (var y = -1; y <= 1; y++)
                    yield return Change(-diff.x, y, z, diff.x, y, z);
            }

            if (diff.y != 0) {
                for (var z = -1; z <= 1; z++)
                for (var x = -1; x <= 1; x++)
                    yield return Change(x, -diff.y, z, x, diff.y, z);
            }

            if (diff.z != 0) {
                for (var y = -1; y <= 1; y++)
                for (var x = -1; x <= 1; x++)
                    yield return Change(x, y, -diff.z, x, y, diff.z);
            }
        }
    }
}