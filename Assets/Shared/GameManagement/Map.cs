using System;
using Shared.Blocks;
using Shared.Positioning;
using Shared.SingletonBehaviour;
using UnityEngine;

namespace Shared.GameManagement {
    /// <summary>
    /// Manages map data - stores, generates and modifies block types at specific coordinates
    /// </summary>
    public class Map : SingletonBehaviour<Map> {
        /// <summary>
        /// Map zero. Used to correctly offset generated chunks
        /// </summary>
        public static (int x, int y) zero;

        public struct BlockInfo {
            public BlockType blockType;
            public byte adjoiningBlocks;

            public BlockInfo(BlockType blockType, byte adjoiningBlocks) {
                this.blockType = blockType;
                this.adjoiningBlocks = adjoiningBlocks;
            }

            public BlockInfo(BlockType blockType) {
                this.blockType = blockType;
                adjoiningBlocks = 0;
            }
        }

        /// <summary>
        /// Map storage that maps <see cref="WorldPosition"/> to <see cref="BlockType"/> and flag for if the block
        /// should be displayed (if contains adjusting blocks at all aligning positions)
        /// </summary>
        public static MapStorage<BlockInfo> storage = new MapStorage<BlockInfo>();

        [SerializeField, Range(0, WorldPosition.ChunkSize)] private int waterLevel = 10;
        [SerializeField, Range(0, WorldPosition.ChunkSize)] private int sandDepth = 2;
        [SerializeField, Range(0, WorldPosition.ChunkSize)] private int groundDepth = 3;
        [SerializeField, Range(0, WorldPosition.ChunkSize)] private int groundUnderwaterDepth = 1;

        [SerializeField, Range(1, 5)] private int octaves = 2;
        [SerializeField, Range(0.0001f, 100f)] private float scale = 10f;
        [SerializeField, Range(0.0001f, 1)] private float persistance = 0.5f;
        [SerializeField, Range(0.0001f, 5)] private float lacunarity = 2f;

        /// <summary>
        /// Gets chunk of blocks in map.
        /// </summary>
        /// <remarks>
        /// If generateIfMissing is set to true will generate new chunk, if no chunk was
        /// previously generated and will return generated chunk.
        /// </remarks>
        /// <param name="position">Chunk position</param>
        /// <param name="generateIfMissing">Generate new chunk if no chunk was found</param>
        /// <returns>Chunk of block types at chunk position or null if no chunk was generated
        /// and generateIfMissing is set to false</returns>
        public static MapStorage<BlockInfo>.Chunk GetChunk(WorldPosition.ChunkPosition position,
            bool generateIfMissing = true) {
            return storage.TryGetChunk(position, out var chunk)
                ? chunk
                : generateIfMissing
                    ? GenerateChunk(position)
                    : null;
        }
        /// <summary>
        /// Gets block type at position
        /// </summary>
        /// <param name="position">Position of block</param>
        /// <param name="blockType">Retrieved block type, if block existed at position</param>
        /// <returns>True if block exists, else false</returns>
        public static bool TryGetBlock(WorldPosition position, out BlockInfo blockType) =>
            storage.TryGetValue(position, out blockType);
        /// <summary>
        /// Sets specific block in map
        /// </summary>
        /// <param name="blockType">Block type</param>
        /// <param name="position">World position of block</param>
        public static void AddBlock(BlockType blockType, WorldPosition position) {
            Debug.Assert(!storage.ContainsKey(position),
                $"Tried to add block at {position}, but block at {position} already exists");


            byte adjoining = 0;

            foreach (var adjoiningVector in WorldPosition.AdjoiningVectors) {
                var adjoiningPosition = position + adjoiningVector;

                if (adjoiningPosition.chunkPosition == position.chunkPosition) {
                    if (!storage.TryGetValue(adjoiningPosition, out var block)) continue;

                    if (++block.adjoiningBlocks == 6)
                        Block.Destroy(adjoiningPosition);

                    storage[adjoiningPosition] = block;
                    adjoining++;
                }
            }

            storage[position] = new BlockInfo(blockType, adjoining);

            Debug.Assert(adjoining < 6, $"Created block had more 6 or more adjoining blocks");
        }
        /// <summary>
        /// Removes block in map
        /// </summary>
        public static bool Remove(WorldPosition position) {
            foreach (var adjoiningVector in WorldPosition.AdjoiningVectors) {
                var adjoiningPosition = position + adjoiningVector;

                if (adjoiningPosition.chunkPosition == position.chunkPosition) {
                    if (!storage.TryGetValue(adjoiningPosition, out var block)) continue;

                    if (block.adjoiningBlocks-- == 6)
                        Block.Instantiate(block.blockType, adjoiningPosition);

                    storage[adjoiningPosition] = block;
                }
            }

            return storage.Remove(position);
        }

        #region Private helper methods
        /// <summary>
        /// Generates new chunk at chunk position and saves it to <see cref="storage"/>
        /// </summary>
        /// <param name="position">Chunk position</param>
        /// <returns>Generated chunk</returns>
        private static MapStorage<BlockInfo>.Chunk GenerateChunk(WorldPosition.ChunkPosition position) {
            Debug.Assert(position.y >= 0, $"Tried to generate chunk at negative y coordinate: {position}");

            switch (position.y) {
                case 1:
                    return storage[position] = TerrainToBlockMap(GenerateHeightMap(position.x, position.z));
                case 0:
                    return storage[position] = RockChunk();
                default:
                    // todo: generate clouds? :)
                    // empty chunk, as we are in air
                    return storage[position] = new MapStorage<BlockInfo>.Chunk();
            }
        }
        /// <summary>
        /// Generates chunk of only rock blocks
        /// </summary>
        /// <returns></returns>
        private static MapStorage<BlockInfo>.Chunk RockChunk() {
            Debug.Assert(WorldPosition.ChunkSize > 0, "Chunk size should be greater than 0");
            var rockChunk = new MapStorage<BlockInfo>.Chunk();

            for (var pz = 0u; pz < WorldPosition.ChunkSize; pz++)
            for (var py = 0u; py < WorldPosition.ChunkSize; py++)
            for (var px = 0u; px < WorldPosition.ChunkSize; px++)
                rockChunk[new WorldPosition.LocalPosition(px, py, pz)] = new BlockInfo(BlockType.Rock,
                    (byte) ((px > 0 ? 1 : 0)
                            + (px < WorldPosition.ChunkSize - 1 ? 1 : 0)
                            + (py > 0 ? 1 : 0)
                            + (py < WorldPosition.ChunkSize - 1 ? 1 : 0)
                            + (pz > 0 ? 1 : 0)
                            + (pz < WorldPosition.ChunkSize - 1 ? 1 : 0)));

            return rockChunk;
        }
        /// <summary>
        /// Generates <see cref="MapStorage{T}.Chunk"/> of <see cref="BlockType"/>s from height map
        /// </summary>
        /// <param name="points">Rectangular array of height values from 0 to 1</param>
        private static MapStorage<BlockInfo>.Chunk TerrainToBlockMap(float[,] points) {
            var blockPositions =
                new (BlockType blockType, byte adjustingCount)[WorldPosition.ChunkSize, WorldPosition.ChunkSize][];

            var waterLevel = Instance.waterLevel;
            var groundDepth = Instance.groundDepth;
            var groundUnderwaterDepth = Instance.groundUnderwaterDepth;
            var sandDepth = Instance.sandDepth;

            for (var z = 0u; z < WorldPosition.ChunkSize; z++)
            for (var x = 0u; x < WorldPosition.ChunkSize; x++) {
                Debug.Assert(points[x, z] >= 0 && points[x, z] <= 1,
                    $"Point at [{x}, {z}] was out of range [0, 1]: {points[x, z]}");

                var height = Mathf.RoundToInt(points[x, z] * WorldPosition.ChunkSize);
                var column = blockPositions[x, z] =
                    new (BlockType blockType, byte adjustingCount)[Math.Max(height, waterLevel)];

                var previousX = x > 0 ? blockPositions[x - 1, z] : null;
                var previousZ = z > 0 ? blockPositions[x, z - 1] : null;

                void Set(BlockType blockType, int from, int to) {
                    var start = Math.Max(0, from);
                    for (var y = start; y < to; y++) {
                        column[y] = (blockType, 0);
                        if (y > 0) {
                            column[y - 1].adjustingCount++;
                            column[y].adjustingCount++;
                        }

                        if (previousX != null && y < previousX.Length) {
                            previousX[y].adjustingCount++;
                            column[y].adjustingCount++;
                        }

                        if (previousZ != null && y < previousZ.Length) {
                            previousZ[y].adjustingCount++;
                            column[y].adjustingCount++;
                        }
                    }
                }

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

            var chunk = new MapStorage<BlockInfo>.Chunk();

            for (var z = 0u; z < WorldPosition.ChunkSize; z++)
            for (var x = 0u; x < WorldPosition.ChunkSize; x++) {
                var height = blockPositions[x, z].Length;
                for (var y = 0u; y < height; y++) {
                    var (blockType, adjustingCount) = blockPositions[x, z][y];
                    chunk[new WorldPosition.LocalPosition(x, y, z)] = new BlockInfo(blockType, adjustingCount);
                }
            }

            return chunk;
        }
        /// <summary>
        /// Generates height map using perlin noise algorithm
        /// </summary>
        /// <param name="xOffset">X offset</param>
        /// <param name="yOffset">Y Offset</param>
        private static float[,] GenerateHeightMap(float xOffset, float yOffset) {
            var offsetX = (zero.x + xOffset) * WorldPosition.ChunkSize;
            var offsetY = (zero.y + yOffset) * WorldPosition.ChunkSize;

            var points = new float[WorldPosition.ChunkSize, WorldPosition.ChunkSize];

            var minHeight = float.MaxValue;
            var maxHeight = float.MinValue;

            for (var y = 0; y < WorldPosition.ChunkSize; y++)
            for (var x = 0; x < WorldPosition.ChunkSize; x++) {
                var amplitude = 1f;
                var frequency = 1f;
                var noiseHeight = 0f;

                for (var i = 0; i < Instance.octaves; i++) {
                    var xValue = (x + offsetX) / Instance.scale * frequency;
                    var yValue = (y + offsetY) / Instance.scale * frequency;
                    noiseHeight += (Mathf.PerlinNoise(xValue, yValue) * 2 - 1) * amplitude;

                    amplitude *= Instance.persistance;
                    frequency *= Instance.lacunarity;
                }

                if (noiseHeight > maxHeight) maxHeight = noiseHeight;
                if (noiseHeight < minHeight) minHeight = noiseHeight;

                points[x, y] = noiseHeight;
            }

            for (var y = 0; y < WorldPosition.ChunkSize; y++)
            for (var x = 0; x < WorldPosition.ChunkSize; x++) {
                points[x, y] = Mathf.InverseLerp(minHeight, maxHeight, points[x, y]);
            }

            return points;
        }
        #endregion
    }
}