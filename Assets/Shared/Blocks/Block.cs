using System.Collections;
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

        private static readonly MapStorage<Block> Blocks = new MapStorage<Block>();

        public BlockType BlockType {
            get => blockType;
            private set {
                if (value == blockType) return;

                blockType = value;
                meshRenderer.sharedMaterial = blockType.BlockData().material;
            }
        }
        public WorldPosition Position {
            get => position;
            private set {
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

            var obj = BlockPooler.Request();
            var blockComponent = obj.GetComponent<Block>();
            blockComponent.Init(blockType, position);

            Blocks[position] = blockComponent;
        }
        /// <summary>
        /// Destroys block at position
        /// </summary>
        public static void Destroy(WorldPosition position) {
            Debug.Assert(Blocks.ContainsKey(position),
                $"Tried to destroy block at {position}, but block at position {position} does not exist");

            BlockPooler.Return(Blocks[position].gameObject);
            Blocks.Remove(position);
        }
        /// <summary>
        /// Instantiates <see cref="MapStorage{T}.Chunk"/> of <see cref="Block"/>s
        /// <see cref="WorldPosition.ChunkPosition"/> position
        /// </summary>
        /// <param name="chunk">Chunk of block types</param>
        /// <param name="position">Chunk position</param>
        public static void InstantiateChunk([NotNull] MapStorage<Map.BlockInfo>.Chunk chunk,
            WorldPosition.ChunkPosition position) {
            if (Blocks.ContainsKey(position)) return;

            foreach (var (index, data) in chunk) {
                if (data.adjoiningBlocks < 6) {
                    Instantiate(data.blockType, new WorldPosition(position, new WorldPosition.LocalPosition(index)));
                }
            }
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

            foreach (var pos in chunk.Select(c => new WorldPosition.LocalPosition(c.index)).ToList())
                Destroy(new WorldPosition(position, pos));

            Blocks.Remove(position);
        }

        private void Awake() => meshRenderer = GetComponent<MeshRenderer>();
        private void Reset() => Awake();

        private void Init(BlockType type, WorldPosition pos) {
            gameObject.SetActive(true);
            BlockType = type;
            Position = pos;
        }

        public WorldPosition GetBuildPosition(Vector3 hitPoint, Vector3 hitNormal) =>
            position + (WorldPosition) hitNormal;
    }
}