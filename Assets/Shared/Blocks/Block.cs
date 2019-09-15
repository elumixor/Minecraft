using System.Linq;
using JetBrains.Annotations;
using Shared.GameManagement;
using Shared.Pooling;
using Shared.Positioning;
using UnityEngine;

namespace Shared.Blocks {
    [RequireComponent(typeof(MeshRenderer))]
    public class Block : MonoBehaviour, IBuildable {
        private BlockType blockType;
        private MeshRenderer meshRenderer;
        private WorldPosition position;

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

        private static readonly MapStorage<Block> Blocks = new MapStorage<Block>();

        public BlockType BlockType {
            get => blockType;
            set {
                if (value == blockType) return;

                blockType = value;
                meshRenderer.sharedMaterial = blockType.BlockData().material;
            }
        }
        public WorldPosition Position {
            get => position;
            set {
                if (position == value) return;

                position = value;
                transform.position = (Vector3) position * Settings.GridUnitWidth;
            }
        }
        /// <summary>
        /// Instantiates <see cref="Block"/> of given <see cref="BlockType"/> at <see cref="WorldPosition"/> position
        /// </summary>
        public static void Instantiate(BlockType blockType, WorldPosition position) {
            Debug.Assert(!Blocks.ContainsKey(position),
                $"Tried to instantiate block at {position}, but block at position {position} already exists");

            var obj = Pooler.Request(PoolTag);
            var blockComponent = obj.GetComponent<Block>();
            blockComponent.Init(blockType, position);

            Blocks[position] = blockComponent;

            foreach (var vector in WorldPosition.AdjoiningVectors)
                if (Blocks.TryGetValue(position + vector, out var block)) {
                    blockComponent.AddAdjoiningBlock();
                    block.AddAdjoiningBlock();
                }
        }
        /// <summary>
        /// Destroys block at position
        /// </summary>
        public static void Destroy(WorldPosition position) {
            Debug.Assert(Blocks.ContainsKey(position),
                $"Tried to destroy block at {position}, but block at position {position} does not exist");

            var block = Blocks[position];

            foreach (var vector in WorldPosition.AdjoiningVectors)
                if (Blocks.TryGetValue(position + vector, out var adjoiningBlock)) {
                    block.RemoveAdjoiningBlock();
                    adjoiningBlock.RemoveAdjoiningBlock();
                }

            Pooler.Return(PoolTag, block.gameObject);
            Blocks.Remove(position);
        }
        /// <summary>
        /// Instantiates <see cref="MapStorage{T}.Chunk"/> of <see cref="Block"/>s
        /// <see cref="WorldPosition.ChunkPosition"/> position
        /// </summary>
        /// <param name="chunk">Chunk of block types</param>
        /// <param name="position">Chunk position</param>
        public static void InstantiateChunk([NotNull] MapStorage<BlockType>.Chunk chunk,
            WorldPosition.ChunkPosition position) {
            foreach (var (index, data) in chunk)
                Instantiate(data, new WorldPosition(position, new WorldPosition.LocalPosition(index)));
        }
        /// <summary>
        /// Destroys all blocks of chunk at coordinates
        /// </summary>
        /// <param name="position">Chunk position</param>
        public static void DestroyChunk(WorldPosition.ChunkPosition position) {
            if (!Blocks.TryGetChunk(position, out var chunk)) {
                Debug.LogWarning($"Tried to delete chunk non-existent chunk at {position}");
                return;
            }

            // todo: check index transformation

            // todo: delete chunk at position

            var positions = chunk.Select(c => new WorldPosition.LocalPosition(c.index)).ToList();
            foreach (var pos in positions) Destroy(new WorldPosition(position, pos));

            Blocks.Remove(position);
        }

        private void Awake() => meshRenderer = GetComponent<MeshRenderer>();
        private void Reset() => Awake();

        private void Init(BlockType type, WorldPosition pos) {
            adjoiningBlocksCount = 0;
            gameObject.SetActive(true);
            BlockType = type;
            Position = pos;
        }

        public WorldPosition GetBuildPosition(Vector3 hitPoint, Vector3 hitNormal) =>
            position + (WorldPosition) hitNormal;
    }
}