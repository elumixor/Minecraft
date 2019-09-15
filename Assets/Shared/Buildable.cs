using Shared.Positioning;
using UnityEngine;
//using Scenes.WorldScene.BlockSelection;

namespace Shared {
    public interface IBuildable {
        WorldPosition GetBuildPosition(Vector3 hitPoint, Vector3 hitNormal);
    }

//    public static class BuildableExtensions {
//        public static void CreateBlock(this Vector3Int blockLocation) => blockLocation.CreateBlock(BlockSelector.SelectedType); 
//
//        public static void CreateBlock(this Vector3Int blockLocation, BlockType selectedType) {
//            var instance = (Block.Block) PrefabUtility.InstantiatePrefab(Settings.BlockPrefab);
//            var transform = instance.transform;
//            transform.parent = Settings.BlocksContainer;
//            instance.BlockType = selectedType;
//            instance.Position = blockLocation;
//            Map.Set(selectedType, blockLocation);
//        }
//    }
}