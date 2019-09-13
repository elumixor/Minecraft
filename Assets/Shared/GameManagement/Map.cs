using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Shared.Blocks;
using Shared.SingletonBehaviour;
using Shared.SpaceWrapping;
using UnityEngine;

namespace Shared.GameManagement {
    public class Map : SingletonBehaviour<Map> {
        /// <summary>
        /// Size of terrain block (x, y, and z size)
        /// </summary>
        public const int ChunkSize = 20;

        /// <summary>
        /// Coordinate zero at global coordinates
        /// <remarks>
        ///    Setter does not preserve existing blocks and their positions. It should be 
        /// </remarks>
        /// </summary>
        public static (int x, int y) Zero { get; set; }

        /// <summary>
        /// Map storage
        /// </summary>
        public static MapStorage Storage { get; private set; } = new MapStorage();

        [SerializeField, Range(0, ChunkSize)] private int waterLevel = 10;
        [SerializeField, Range(0, ChunkSize)] private int sandDepth = 2;
        [SerializeField, Range(0, ChunkSize)] private int groundDepth = 3;
        [SerializeField, Range(0, ChunkSize)] private int groundUnderwaterDepth = 1;

        [SerializeField, Range(1, 5)] private int octaves = 2;
        [SerializeField, Range(0.0001f, 100f)] private float scale = 10f;
        [SerializeField, Range(0.0001f, 1)] private float persistance = 0.5f;
        [SerializeField, Range(0.0001f, 5)] private float lacunarity = 2f;

        // Public API

        public static SortedDictionary<int, BlockType> GetChunk(UIntPosition position, bool generateIfMissing = true) {
            var index = position.Index;
            if (Storage.TryGetValue(index, out var chunk)) return chunk;
            
            return !generateIfMissing ? null : GenerateChunk(position);
        }

        // Generate new chunk at xyz coordinate in chunk coordinates
        public static SortedDictionary<int, BlockType> GenerateChunk(UIntPosition position) {
            var offsetX = (Zero.x + position.x) * ChunkSize;
            var offsetY = (Zero.y + position.y) * ChunkSize;

            var points = new float[ChunkSize, ChunkSize];

            var minHeight = float.MaxValue;
            var maxHeight = float.MinValue;

            for (var y = 0; y < ChunkSize; y++)
            for (var x = 0; x < ChunkSize; x++) {
                var amplitude = 1f;
                var frequency = 1f;
                var noiseHeight = 0f;

                for (var i = 0; i < Instance.octaves; i++) {
                    var xValue = (x + (float) offsetX) / Instance.scale * frequency;
                    var yValue = (y + (float) offsetY) / Instance.scale * frequency;
                    noiseHeight += (Mathf.PerlinNoise(xValue, yValue) * 2 - 1) * amplitude;

                    amplitude *= Instance.persistance;
                    frequency *= Instance.lacunarity;
                }

                if (noiseHeight > maxHeight) maxHeight = noiseHeight;
                if (noiseHeight < minHeight) minHeight = noiseHeight;

                points[x, y] = noiseHeight;
            }

            for (var y = 0; y < ChunkSize; y++)
            for (var x = 0; x < ChunkSize; x++) {
                points[x, y] = Mathf.InverseLerp(minHeight, maxHeight, points[x, y]);
            }

            var perlinPoints = points;

            // todo: check if chunk exists and conditionally overwrite
            return Storage[position.Index] = TerrainToBlockMap(perlinPoints);
        }

        public static void CreateFrom(MapStorage mapData) => Storage = mapData;

        // Get block at current chunk
        [Pure, CanBeNull]
        public static BlockType? GetBlock(Vector3Int position) => GetBlock(position, PlayerPosition.CurrentChunk);

        // Get block at specific chunk
        [Pure, CanBeNull]
        public static BlockType? GetBlock(Vector3Int position, UIntPosition chunkPosition) =>
            Storage.TryGetValue(chunkPosition.Index, out var chunk)
                ? chunk.TryGetValue(IndexInChunk(position), out var blockType) ? (BlockType?) blockType : null
                : null;

        // Set or remove block at current chunk
        public static void SetBlock(BlockType blockType, Vector3Int position) =>
            SetBlock(blockType, position, PlayerPosition.CurrentChunk);

        // Set or remove block at specific chunk
        public static void SetBlock(BlockType blockType, Vector3Int position, UIntPosition chunkPosition) =>
            Storage[chunkPosition.Index][IndexInChunk(position)] = blockType;

        public static void Remove(Vector3Int position) => Remove(position, PlayerPosition.CurrentChunk);

        public static void Remove(Vector3Int position, UIntPosition chunkPosition) =>
            Storage[chunkPosition.Index].Remove(IndexInChunk(position));

        public static int IndexInChunk(int x, int y, int z) => x * ChunkSize * ChunkSize + y * ChunkSize + z;
        public static int IndexInChunk(Vector3Int position) => IndexInChunk(position.x, position.y, position.z);

        private static Vector3Int FromChunkIndex(int i) {
            var x = i / (ChunkSize * ChunkSize);
            var y = (i - x * ChunkSize * ChunkSize) / ChunkSize;
            var z = i - x * ChunkSize * ChunkSize - y * ChunkSize;
            return new Vector3Int(x, y, z);
        }

        public static UIntPosition GlobalPosition(Vector3Int position) => GlobalPosition(position, PlayerPosition.CurrentChunk);

        public static UIntPosition GlobalPosition(Vector3Int position, UIntPosition chunkPosition) =>
            chunkPosition * ChunkSize + (UIntPosition) position;

        public static void InstantiateChunk(UIntPosition chunkPosition) =>
            InstantiateChunk(Storage[chunkPosition.Index], chunkPosition);

        public static void InstantiateChunk(SortedDictionary<int, BlockType> chunk, UIntPosition chunkPosition) {
            foreach (var kvp in chunk) Block.Add(kvp.Value, FromChunkIndex(kvp.Key), chunkPosition);
        }

        public static void DestroyChunk(UIntPosition chunkPosition) =>
            DestroyChunk(Storage[chunkPosition.Index], chunkPosition);

        public static void DestroyChunk(SortedDictionary<int, BlockType> chunk, UIntPosition chunkPosition) {
            foreach (var kvp in chunk) Block.Remove(FromChunkIndex(kvp.Key), chunkPosition);
        }

        private static SortedDictionary<int, BlockType> TerrainToBlockMap(float[,] points) {
            var blockPositions = new SortedDictionary<int, BlockType>();

            var waterLevel = Instance.waterLevel;
            var groundDepth = Instance.groundDepth;
            var groundUnderwaterDepth = Instance.groundUnderwaterDepth;
            var sandDepth = Instance.sandDepth;

            for (var z = 0; z < ChunkSize; z++)
            for (var x = 0; x < ChunkSize; x++) {
                void Set(BlockType blockType, int from, int to) {
                    for (var y = Math.Max(0, from); y < to; y++) blockPositions[IndexInChunk(x, y, z)] = blockType;
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