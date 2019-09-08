using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scenes.WorldScene.Block;
using Scenes.WorldScene.BlockSelection;
using Shared;
using UnityEditor;
using UnityEngine;

namespace Scenes.WorldScene.Map {
    public class MapManager : SingletonBehaviour<MapManager> {
        [Serializable]
        public struct BlockPosition {
            public BlockType blockType;

            /// <summary>
            /// Position index, that is i = z * blockSize^2 + y * blockSize + z
            /// </summary>
            [field: SerializeField]
            public int Index { get; private set; }
//
//            [SerializeField] private int x;
//            [SerializeField] private int y;
//            [SerializeField] private int z;
//
//            public int X {
//                get => x;
//                set => x = value;
//            }
//
//            public int Y {
//                get => y;
//                set => y = value;
//            }
//
//            public int Z {
//                get => z;
//                set => z = value;
//            }
//
//            public Vector3Int Position {
//                get => new Vector3Int(x, y, z);
//                set {
//                    x = value.x;
//                    y = value.y;
//                    z = value.z;
//                }
//            }
//
//            public BlockType BlockType {
//                get => blockType;
//                set => blockType = value;
//            }
//
//            public void Deconstruct(out BlockType outBlockType, out Vector3Int outPosition) {
//                outBlockType = blockType;
//                outPosition = Position;
//            }

            public void Deconstruct(out BlockType outBlockType, out int outI) {
                outBlockType = blockType;
                outI = Index;
            }
//
//            public void Deconstruct(out BlockType outBlockType, out int outX, out int outY, out int outZ) {
//                outBlockType = blockType;
//                outX = x;
//                outY = y;
//                outZ = z;
//            }

            /// <summary>
            /// Implicit conversion from value tuple
            /// </summary>
            public static implicit operator BlockPosition((BlockType blockType, int i) valueTuple) =>
                new BlockPosition {blockType = valueTuple.blockType, Index = valueTuple.i};
        }

        private const int BlockSize = 100;
        private const int BlockSizeSquared = BlockSize * BlockSize;

        [SerializeField] private List<BlockPosition> map = new List<BlockPosition>();

        private void Reset() => Awake();

        private static int ToPos(int x, int y, int z) => z * BlockSize * BlockSize + y * BlockSize + x;
        private static int ToPos(Vector3Int pos) => ToPos(pos.x, pos.y, pos.z);
        
        private static (int x, int y, int z) FromPos(int i) {
            var z = i / BlockSizeSquared;
            var y = (i - z * BlockSizeSquared) / BlockSize;
            var x = i - z * BlockSizeSquared - y * BlockSize;
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
                    map[j] = (blockType, i); return;
                }

                if (mapI < i) {
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
        public static void Remove(int x, int y, int z) => Remove(ToPos(x, y, z));

        public static void Remove (Vector3Int position) => Remove(ToPos(position));
    }
}