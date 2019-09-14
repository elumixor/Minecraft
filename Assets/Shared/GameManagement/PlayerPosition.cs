using Shared.SingletonBehaviour;
using Shared.SpaceWrapping;
using UnityEngine;

namespace Shared.GameManagement {
    public class PlayerPosition : SingletonBehaviour<PlayerPosition> {
        // position in chunk
        public static Vector3Int Position { get; set; }

        private static UIntPosition currentChunk;

        // position of chunk itself
        public static UIntPosition CurrentChunk {
            get => currentChunk;
            set {
                if (currentChunk != value) {
                    var diff = value - currentChunk;
                    if (diff.x != 0) {
                        for (var z = -1; z <= 1; z++)
                        for (var y = -1; y <= 1; y++) {
                            Map.DestroyChunk(currentChunk + new UIntPosition(-diff.x, y, z));
                            var pos = value + new UIntPosition(diff.x, y, z);
                            if (pos.y >= 0)
                                Map.InstantiateChunk(Map.GetChunk(pos), pos);
                        }
                    } else if (diff.y != 0) {
                        for (var z = -1; z <= 1; z++)
                        for (var x = -1; x <= 1; x++) {
                            Map.DestroyChunk(currentChunk + new UIntPosition(x, -diff.y, z));
                            var pos = value + new UIntPosition(x, diff.y, z);
                            if (pos.y >= 0)
                                Map.InstantiateChunk(Map.GetChunk(pos), pos);
                        }
                    } else if (diff.z != 0) {
                        for (var y = -1; y <= 1; y++)
                        for (var x = -1; x <= 1; x++) {
                            Map.DestroyChunk(currentChunk + new UIntPosition(x, y, -diff.z));
                            var pos = value + new UIntPosition(x, y, diff.z);
                            if (pos.y >= 0)
                                Map.InstantiateChunk(Map.GetChunk(pos), pos);
                        }
                    }

                    currentChunk = value;
                }
            }
        }

        // global position wrt Position and ChunkPosition
        public static UIntPosition GlobalPosition {
            get => Map.ToGlobalPosition(Position, currentChunk);
            set {
                CurrentChunk = UIntPosition.Floor((Vector3) value / Map.ChunkSize);
                Position = value - currentChunk * Map.ChunkSize;
            }
        }
    }
}