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
                    OnChunkChanged(position.chunkPosition, value.chunkPosition);
                position = value;
            }
        }

        private static void OnChunkChanged(WorldPosition.ChunkPosition previous, WorldPosition.ChunkPosition current) {
            var diff = current - previous;
            if (diff.x != 0) {
                for (var z = -1; z <= 1; z++)
                for (var y = -1; y <= 1; y++) {
                    Instance.StartCoroutine(
                        Block.DestroyChunk(previous + new WorldPosition.ChunkPosition(-diff.x, y, z)));
                    var pos = current + new WorldPosition.ChunkPosition(diff.x, y, z);
                    if (pos.y >= 0)
                        Instance.StartCoroutine(Block.InstantiateChunk(Map.GetChunk(pos), pos));
                }
            } else if (diff.y != 0) {
                for (var z = -1; z <= 1; z++)
                for (var x = -1; x <= 1; x++) {
                    Instance.StartCoroutine(
                        Block.DestroyChunk(previous + new WorldPosition.ChunkPosition(x, -diff.y, z)));
                    var pos = current + new WorldPosition.ChunkPosition(x, diff.y, z);
                    if (pos.y >= 0)
                        Instance.StartCoroutine(Block.InstantiateChunk(Map.GetChunk(pos), pos));
                }
            } else if (diff.z != 0) {
                for (var y = -1; y <= 1; y++)
                for (var x = -1; x <= 1; x++) {
                    Instance.StartCoroutine(Block.DestroyChunk(previous + new WorldPosition.ChunkPosition(x, y, -diff.z)));
                    var pos = current + new WorldPosition.ChunkPosition(x, y, diff.z);
                    if (pos.y >= 0)
                        Instance.StartCoroutine(Block.InstantiateChunk(Map.GetChunk(pos), pos));
                }
            }
        }
    }
}