using System.Collections.Generic;
using Shared.GameManagement;
using Shared.Pooling;
using Shared.SpaceWrapping;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Shared.Blocks {
    [RequireComponent(typeof(MeshRenderer))]
    public class Block : MonoBehaviour, IBuildable {
        private BlockType blockType;
        private MeshRenderer meshRenderer;
        private Vector3Int position;
        private UIntPosition chunkPosition;

        private int adjoiningBlocksCount;
        private void AddAdjoiningBlock() {
            adjoiningBlocksCount++;
            if (adjoiningBlocksCount == 6) gameObject.SetActive(false);
        }

        private void RemoveAdjoiningBlock() {
            adjoiningBlocksCount--;
            if (adjoiningBlocksCount < 6) gameObject.SetActive(true);
        }

        private const string PoolTag = "Block";

        private static readonly MapStorage<Block> BlocksStorage = new MapStorage<Block>();

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

        public static bool Add(BlockType blockType, Vector3Int position, UIntPosition chunkPosition,
            bool replaceExisting = false) {
            var chunkIndex = chunkPosition.Index;
            if (!BlocksStorage.TryGetValue(chunkIndex, out var chunk))
                BlocksStorage[chunkIndex] = chunk = new SortedDictionary<int, Block>();

            var positionIndex = Map.IndexInChunk(position);

            if (!replaceExisting && chunk.ContainsKey(positionIndex)) return false;

            var obj = Pooler.Request(PoolTag);
            var blockComponent = obj.GetComponent<Block>();
            blockComponent.Init(blockType, position, chunkPosition);
            chunk[positionIndex] = blockComponent;

            var globalPosition = Map.ToGlobalPosition(position, chunkPosition);
            foreach (var vector in UIntPosition.AdjoiningVectors) {
                var (checkedPosition, checkedChunk) = Map.FromGlobalPosition(globalPosition + vector);

                if (BlocksStorage.TryGetValue(checkedChunk.Index, Map.IndexInChunk(checkedPosition), out var block)) {
                    blockComponent.AddAdjoiningBlock();
                    block.AddAdjoiningBlock();
                }
            }

            return true;
        }

        public static bool Remove(Block block) {
            var positionIndex = Map.IndexInChunk(block.position);

            var globalPosition = Map.ToGlobalPosition(block.position, block.chunkPosition);

            foreach (var vector in UIntPosition.AdjoiningVectors) {
                var (checkedPosition, checkedChunk) = Map.FromGlobalPosition(globalPosition + vector);

                if (BlocksStorage.TryGetValue(checkedChunk.Index, Map.IndexInChunk(checkedPosition),
                    out var adjoiningBlock)) {
                    block.RemoveAdjoiningBlock();
                    adjoiningBlock.RemoveAdjoiningBlock();
                }
            }

            // todo: this assumes that block is registered in map, thus may throw exception.. which is ok?
            var chunk = BlocksStorage[block.chunkPosition.Index];
            var obj = chunk[positionIndex];
            Pooler.Return(PoolTag, obj.gameObject);
            chunk.Remove(positionIndex);
            return true;
        }

        public static bool Remove(Vector3Int position) => Remove(position, PlayerPosition.CurrentChunk);

        public static bool Remove(Vector3Int position, UIntPosition chunkPosition) =>
            !BlocksStorage.TryGetValue(chunkPosition.Index, Map.IndexInChunk(position), out var block) || Remove(block);

        private void Awake() => meshRenderer = GetComponent<MeshRenderer>();
        private void Reset() => Awake();

        private void Init(BlockType bType, Vector3Int pos, UIntPosition chunkPos) {
            adjoiningBlocksCount = 0;
            gameObject.SetActive(true);
            BlockType = bType;
            Position = pos;
            ChunkPosition = chunkPos;
        }

        private void UpdatePosition() =>
            transform.position = (Vector3) Map.ToGlobalPosition(position, chunkPosition) * Settings.GridUnitWidth;

        public Vector3Int GetBuildPosition(Vector3 hitPoint, Vector3 hitNormal) =>
            position + Vector3Int.RoundToInt(hitNormal);
    }
}