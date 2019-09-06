using System;
using System.Collections;
using System.Collections.Generic;
using Scenes.WorldScene.Block;
using UnityEditor;
using UnityEngine;

namespace Scenes.WorldScene.Map {
    public class MapManager : MonoBehaviour, IEnumerable<(BlockType? blockType, (int x, int y, int z))> {
        [SerializeField] private BlockType?[,,] currentChunk;
        [SerializeField, Range(10, 100)] private int chunkSize;
        [SerializeField] private Transform blockContainer;
        [SerializeField] private Block.Block blockPrefab;

        /// <summary>
        /// Generates new map chunk
        /// </summary>
        public void GenerateChunk() {
            var rand = new System.Random();
            currentChunk = new BlockType?[chunkSize, chunkSize, chunkSize];
            var values = Enum.GetValues(typeof(BlockType));
            for (var x = 0; x < chunkSize; x++)
            for (var y = 0; y < chunkSize; y++)
            for (var z = 0; z < chunkSize; z++) {
                var value = rand.Next(-1, values.Length);
                currentChunk[x, y, z] = value == -1 ? null : (BlockType?) value;
            }
        }

        public void CreateBlocks() {
            IEnumerator Create() {
                foreach (var (blockType, (x, y, z)) in this) {
                    if (blockType == null) continue;

                    var block = (Block.Block) PrefabUtility.InstantiatePrefab(blockPrefab, blockContainer);
                    block.Position = new Vector3Int(x, y, z);
                    block.BlockType = (BlockType) blockType;
                    yield return null;
                }
            }

            StartCoroutine(Create());
        }

        private BlockType? this[int x, int y, int z] {
            get => currentChunk[x, y, z];
            set => currentChunk[x, y, z] = value;
        }

        public IEnumerator<(BlockType? blockType, (int x, int y, int z))> GetEnumerator() {
            for (var x = 0; x < chunkSize; x++)
            for (var y = 0; y < chunkSize; y++)
            for (var z = 0; z < chunkSize; z++) {
                var value = currentChunk[x, y, z];
                yield return (value, (x, y, z));
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}