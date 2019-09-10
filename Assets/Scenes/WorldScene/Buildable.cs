using Scenes.WorldScene.Block;
using Scenes.WorldScene.BlockSelection;
using Scenes.WorldScene.Map;
using UnityEditor;
using UnityEngine;
using Terrain = Scenes.WorldScene.Map.Terrain;

namespace Scenes.WorldScene {
    public interface IBuildable {
        Vector3Int GetBuildPosition(Vector3 hitPoint, Vector3 hitNormal);
    }

    public static class BuildableExtensions {
        public static void CreateBlock(this Vector3Int blockLocation) => blockLocation.CreateBlock(BlockSelector.SelectedType); 

        public static void CreateBlock(this Vector3Int blockLocation, BlockType selectedType) {
            var instance = (Block.Block) PrefabUtility.InstantiatePrefab(Settings.BlockPrefab);
            var transform = instance.transform;
            transform.parent = Settings.BlocksContainer;
            instance.BlockType = selectedType;
            instance.Position = blockLocation;
            Terrain.Set(selectedType, blockLocation);
        }
    }
}