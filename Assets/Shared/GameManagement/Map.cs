using System;
using JetBrains.Annotations;
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

        /// <summary>
        /// Map storage
        /// </summary>
        public static MapStorage<BlockType> storage = new MapStorage<BlockType>();

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
        public static MapStorage<BlockType>.Chunk GetChunk(WorldPosition.ChunkPosition position,
            bool generateIfMissing = true) =>
            storage.TryGetChunk(position, out var chunk)
                ? chunk
                : generateIfMissing
                    ? GenerateChunk(position)
                    : null;
        /// <summary>
        /// Gets block type at position
        /// </summary>
        /// <param name="position">Position of block</param>
        /// <param name="blockType">Retrieved block type, if block existed at position</param>
        /// <returns>True if block exists, else false</returns>
        public static bool TryGetBlock(WorldPosition position, out BlockType blockType) =>
            storage.TryGetValue(position, out blockType);
        /// <summary>
        /// Sets specific block in map
        /// </summary>
        /// <param name="blockType">Block type</param>
        /// <param name="position">World position of block</param>
        public static void SetBlock(BlockType blockType, WorldPosition position) => storage[position] = blockType;
        /// <summary>
        /// Removes block in map
        /// </summary>
        public static bool Remove(WorldPosition position) => storage.Remove(position);

        #region Private helper methods
        /// <summary>
        /// Generates new chunk at chunk position and saves it to <see cref="storage"/>
        /// </summary>
        /// <param name="position">Chunk position</param>
        /// <returns>Generated chunk</returns>
        private static MapStorage<BlockType>.Chunk GenerateChunk(WorldPosition.ChunkPosition position) {
            Debug.Assert(position.y >= 0, $"Tried to generate chunk at negative y coordinate: {position}");

            switch (position.y) {
                case 1:
                    return storage[position] = TerrainToBlockMap(GenerateHeightMap(position.x, position.z));
                case 0: {
                    var rockChunk = new MapStorage<BlockType>.Chunk();
                    for (var pz = 0u; pz < WorldPosition.ChunkSize; pz++)
                    for (var py = 0u; py < WorldPosition.ChunkSize; py++)
                    for (var px = 0u; px < WorldPosition.ChunkSize; px++)
                        rockChunk[new WorldPosition.LocalPosition(px, py, pz)] = BlockType.Rock;

                    return storage[position] = rockChunk;
                }
                default:
                    // todo: generate clouds? :)
                    // empty chunk, as we are in air
                    return storage[position] = new MapStorage<BlockType>.Chunk();
            }
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
        /// <summary>
        /// Generates <see cref="MapStorage{T}.Chunk"/> of <see cref="BlockType"/>s from height map
        /// </summary>
        /// <param name="points">Rectangular array of height values from 0 to 1</param>
        private static MapStorage<BlockType>.Chunk TerrainToBlockMap(float[,] points) {
            var blockPositions = new MapStorage<BlockType>.Chunk();

            var waterLevel = Instance.waterLevel;
            var groundDepth = Instance.groundDepth;
            var groundUnderwaterDepth = Instance.groundUnderwaterDepth;
            var sandDepth = Instance.sandDepth;

            for (var z = 0u; z < WorldPosition.ChunkSize; z++)
            for (var x = 0u; x < WorldPosition.ChunkSize; x++) {
                void Set(BlockType blockType, int from, int to) {
                    for (var y = (uint) Math.Max(0, from); y < to; y++)
                        blockPositions[new WorldPosition.LocalPosition(x, y, z)] = blockType;
                }

                Debug.Assert(points[x, z] >= 0 && points[x, z] <= 1,
                    $"Point at [{x}, {z}] was out of range (0, 1): {points[x, z]}");

                var height = Mathf.RoundToInt(points[x, z] * WorldPosition.ChunkSize);

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
        #endregion
    }
}