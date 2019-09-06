using Scenes.WorldScene.Block;
using UnityEditor;
using UnityEngine;

namespace Scenes.WorldScene {
    public interface IBuildable { }

    public static class BuildableExtensions {
        public static void CreateBlock(this IBuildable buildable, Vector3Int position, BlockType selectedType) {
            var instance = (Block.Block)PrefabUtility.InstantiatePrefab(Settings.BlockPrefab);
            var transform = instance.transform;
            transform.parent = Settings.BlocksContainer;
            instance.BlockType = selectedType;
            instance.Position = position;
        }
    }
}