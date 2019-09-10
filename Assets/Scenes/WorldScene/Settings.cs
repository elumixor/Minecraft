using System;
using Scenes.WorldScene.Block;
using Scenes.WorldScene.Block.BlockDataContainer;
using Shared;
using UnityEditor;
using UnityEngine;

namespace Scenes.WorldScene {
    public class Settings : SingletonBehaviour<Settings> {
        [SerializeField] private Block.Block blockPrefab;
        [SerializeField] private BlockDataContainer blockDataContainer;
        [SerializeField] private Transform blocksContainer;
        [SerializeField] private Transform floor;
        [SerializeField, Range(0.1f, 10f)] private float gridUnitWidth;
        public static Block.Block BlockPrefab => Instance.blockPrefab;
        public static BlockDataContainer BlockDataContainer => Instance.blockDataContainer;
        public static Transform BlocksContainer => Instance.blocksContainer;
        public static float GridUnitWidth => Instance.gridUnitWidth;

        public static Block.Block CreateBlockInstance(BlockType blockType = default, Vector3Int position = default) {
            var instance = (Block.Block) PrefabUtility.InstantiatePrefab(BlockPrefab);
            var transform = instance.transform;
            transform.parent = BlocksContainer;
            instance.BlockType = blockType;
            instance.Position = position;
            return instance;
        }

        private void OnValidate() {
            if (floor == null) return;

            var tr = floor.transform;
            var transformPosition = tr.position;
            transformPosition.y = -gridUnitWidth * .5f;
            tr.position = transformPosition;
        }

        public static void CreateBlockInstance(BlockType blockType, int x, int y, int z) =>
            CreateBlockInstance(blockType, new Vector3Int(x, y, z));
    }
}