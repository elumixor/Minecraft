using System.Collections.Generic;
using Shared.GameManagement;
using Shared.Pooling;
using Shared.SpaceWrapping;
using UnityEngine;

namespace Shared.Blocks {
    [RequireComponent(typeof(MeshRenderer))]
    public class Block : MonoBehaviour, IBuildable {
        private BlockType blockType;
        private MeshRenderer meshRenderer;
        private Vector3Int position;
        private UIntPosition chunkPosition;

        private const string PoolTag = "Block";

        private static SortedDictionary<long, SortedDictionary<int, GameObject>> blocks =
            new SortedDictionary<long, SortedDictionary<int, GameObject>>();

        public BlockType BlockType {
            get => blockType;
            set {
                if (value == blockType) return;

                blockType = value;
                meshRenderer.sharedMaterial = blockType.BlockData().material;
            }
        }
        public Vector3Int Position {
            get => position;
            set {
                if (value == position) return;

                position = value;
                if (position.y < 0) position.y = 0;
                UpdatePosition();
            }
        }
        public UIntPosition ChunkPosition {
            get => chunkPosition;
            set {
                if (chunkPosition == value) return;

                chunkPosition = value;
                UpdatePosition();
            }
        }

        // Create block at coordinates at current chunk 
        public static bool Add(BlockType blockType, Vector3Int position, bool replaceExisting = false) =>
            Add(blockType, position, PlayerPosition.CurrentChunk, replaceExisting);

        public static bool Add(BlockType blockType, Vector3Int position, UIntPosition chunkPosition, bool replaceExisting = false) {
            var chunkIndex = chunkPosition.Index;
            if (!blocks.TryGetValue(chunkIndex, out var chunk)) blocks[chunkIndex] = chunk = new SortedDictionary<int, GameObject>();

            var positionIndex = Map.IndexInChunk(position);

            if (replaceExisting) {
                var obj = Pooler.Request(PoolTag);
                obj.GetComponent<Block>().Init(blockType, position, chunkPosition);
                chunk[positionIndex] = obj;
                return true;
            } else {
                if (chunk.ContainsKey(positionIndex)) return false;

                var obj = Pooler.Request(PoolTag);
                obj.GetComponent<Block>().Init(blockType, position, chunkPosition);
                chunk[positionIndex] = obj;
                return true;
            }
        }
        
        public static bool Remove(Block block) {
            var chunkIndex = PlayerPosition.CurrentChunk.Index;
            var positionIndex = Map.IndexInChunk(block.position);

            // todo: this assumes that block is registered in map, thus may throw exception.. which is ok?
            var chunk = blocks[chunkIndex];
            var obj = chunk[positionIndex];
            Pooler.Return(PoolTag, obj);
            chunk.Remove(positionIndex);
            return true;
        }

        public static bool Remove(Vector3Int position) => Remove(position, PlayerPosition.CurrentChunk);

        public static bool Remove(Vector3Int position, UIntPosition chunkPosition) {
            var chunkIndex = chunkPosition.Index;
            if (!blocks.TryGetValue(chunkIndex, out var chunk)) return false;

            var positionIndex = Map.IndexInChunk(position);
            if (!chunk.ContainsKey(positionIndex)) return false;

            var obj = chunk[positionIndex];
            Pooler.Return(PoolTag, obj);
            chunk.Remove(positionIndex);
            return true;
        }

        private void Awake() => meshRenderer = GetComponent<MeshRenderer>();
        private void Reset() => Awake();

        private void Init(BlockType bType, Vector3Int pos, UIntPosition chunkPos) {
            BlockType = bType;
            Position = pos;
            ChunkPosition = chunkPos;
        }

        private void UpdatePosition() =>
            transform.position = (Vector3) Map.GlobalPosition(position, chunkPosition) * Settings.GridUnitWidth;

        public Vector3Int GetBuildPosition(Vector3 hitPoint, Vector3 hitNormal) =>
            position + Vector3Int.RoundToInt(hitNormal);
    }
}