using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scenes.WorldScene.Block;
using Scenes.WorldScene.BlockSelection;
using Shared;
using Shared.SingletonBehaviour;
using UnityEditor;
using UnityEngine;

namespace Scenes.WorldScene.Map {
    public class Terrain : SingletonBehaviour<Terrain>, IEnumerable<((int x, int y, int z) position, BlockType blockType, int index)> {
        /// <summary>
        /// Size of terrain block (x, y, and z size)
        /// </summary>
        private const int ChunkSize = 20;
        
        [SerializeField] private List<BlockPosition> map = new List<BlockPosition>();

        /// <summary>
        /// Current chunk the player is at
        /// </summary>
        public static (int x, int y, int z) CurrentChunkPosition { get; }
        
        /// <summary>
        /// Blocks types and positions in current chunk
        /// </summary>
        public static IEnumerable<BlockPosition> CurrentChunk { get; }

        /// <summary>
        /// Generate terrain chunk at position
        /// </summary>
        public static void GenerateChunk(int x, int y, int z) { }

//        public static void 

        [Serializable]
        public struct BlockPosition {
            public BlockType blockType;

            /// <summary>
            /// Position index, that is i = z * blockSize^2 + y * blockSize + z
            /// </summary>
            [field: SerializeField]
            public int Index { get; private set; }

            /// <summary>
            /// Deconstruct into <see cref="BlockType"/> and block index
            /// </summary>
            public void Deconstruct(out BlockType outBlockType, out int outIndex) {
                outBlockType = blockType;
                outIndex = Index;
            }
            
            /// <summary>
            /// Deconstruct into <see cref="BlockType"/> and position
            /// </summary>
            public void Deconstruct(out BlockType outBlockType, out int x, out int y, out int z) {
                outBlockType = blockType;
                var (x1, y1, z1) = FromPos(Index);
                x = x1;
                y = y1;
                z = z1;
            }

            /// <summary>
            /// Deconstruct into <see cref="BlockType"/>, position and index
            /// </summary>
            public void Deconstruct(out BlockType outBlockType, out int x, out int y, out int z, out int outIndex) {
                outBlockType = blockType;
                outIndex = Index;
                var (x1, y1, z1) = FromPos(outIndex);
                x = x1;
                y = y1;
                z = z1;
            }

            /// <summary>
            /// Implicit conversion from value tuple
            /// </summary>
            public static implicit operator BlockPosition((BlockType blockType, int i) valueTuple) =>
                new BlockPosition {blockType = valueTuple.blockType, Index = valueTuple.i};
        }


        private static int ToPos(int x, int y, int z) => x * (ChunkSize * ChunkSize) + y * ChunkSize + z;
        private static int ToPos(Vector3Int pos) => ToPos(pos.x, pos.y, pos.z);

        private static (int x, int y, int z) FromPos(int i) {
            var x = i / (ChunkSize * ChunkSize);
            var y = (i - x * ChunkSize * ChunkSize) / ChunkSize;
            var z = i - x * ChunkSize * ChunkSize - y * ChunkSize;
            return (x, y, z);
        }

        public static BlockType? Get(int i) {
            foreach (var (blockType, outI) in Instance.map) {
                if (outI == i) return blockType;
                if (outI > i) return null;
            }

            return null;
        }

        public static BlockType? Get(int x, int y, int z) => Get(ToPos(x, y, z));

        public static BlockType? Get(Vector3Int pos) => Get(ToPos(pos));

        public static void Set(BlockType blockType, int i) {
            var map = Instance.map;

            if (map.Count == 0) {
                map.Add((blockType, i));
                return;
            }

            for (var j = 0; j < map.Count; j++) {
                var mapI = map[j].Index;
                if (mapI == i) {
                    map[j] = (blockType, i);
                    return;
                }

                if (mapI > i) {
                    map.Insert(j, (blockType, i));
                    return;
                }
            }

            map.Add((blockType, i));
        }

        public static void Set(BlockType blockType, int x, int y, int z) => Set(blockType, ToPos(x, y, z));

        public static void Set(BlockType blockType, Vector3Int position) => Set(blockType, ToPos(position));


        public static void Remove(int i) {
            var instanceMap = Instance.map;
            for (var index = 0; index < instanceMap.Count; index++) {
                var blockPosition = instanceMap[index];
                if (blockPosition.Index == i) {
                    instanceMap.RemoveAt(index);
                    return;
                }
            }
        }

        // todo: Create block, remove block should be placed here

        public static void Remove(int x, int y, int z) => Remove(ToPos(x, y, z));

        public static void Remove(Vector3Int position) => Remove(ToPos(position));

        public IEnumerator<((int x, int y, int z) position, BlockType blockType, int index)> GetEnumerator() {
            foreach (var (blockType, index) in map)
                yield return (FromPos(index), blockType, index);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [SerializeField, Range(0, ChunkSize)] private int waterLevel;
        [SerializeField, Range(0, ChunkSize)] private int sandDepth;
        [SerializeField, Range(0, ChunkSize)] private int groundDepth;
        [SerializeField, Range(0, ChunkSize)] private int groundUnderwaterDepth;

        public static void ClearMap() {
            Settings.BlocksContainer.DestroyAllChildren();
            Instance.map.Clear();
        }

        public static List<(BlockType Rock, (int x, int z, int y))> GenerateChunk() {
            var points = MapGenerator.Generate(ChunkSize, ChunkSize, 20, 0, 0, 50000);
            var top = Instance.waterLevel + Instance.sandDepth;
            var bottom = Math.Max(0, Instance.waterLevel - Instance.sandDepth);

            var blockPositions = new List<(BlockType Rock, (int x, int z, int y))>();

            var waterLevel = Instance.waterLevel;
            var groundDepth = Instance.groundDepth;
            var groundUnderwaterDepth = Instance.groundUnderwaterDepth;
            var sandDepth = Instance.sandDepth;

            for (var z = 0; z < ChunkSize; z++)
            for (var x = 0; x < ChunkSize; x++) {
                void Set(BlockType blockType, int from, int to) {
                    for (var y = Math.Max(0, from); y < to; y++) blockPositions.Add((blockType, (x, y, z)));
                }

                var height = Mathf.RoundToInt(points[x, z] * ChunkSize);

                if (height > waterLevel + groundDepth) {
                    Set(BlockType.Rock, 0, height - groundDepth);
                    Set(BlockType.Ground, height - groundDepth, height);
                } else if (height > waterLevel + sandDepth) {
                    Set(BlockType.Rock, 0, waterLevel - groundUnderwaterDepth);
                    Set(BlockType.Ground, waterLevel - groundUnderwaterDepth, waterLevel + groundDepth);
                } else if (height > waterLevel) {
                    Set(BlockType.Rock, 0, waterLevel - sandDepth);
                    Set(BlockType.Sand, waterLevel - sandDepth, height - sandDepth);
                } else {
                    // >= waterLevel
                    Set(BlockType.Rock, 0, height - sandDepth);
                    Set(BlockType.Sand, height - sandDepth, height);
                    Set(BlockType.Water, height, waterLevel);
                }
            }

            return blockPositions;
        }
    }
}