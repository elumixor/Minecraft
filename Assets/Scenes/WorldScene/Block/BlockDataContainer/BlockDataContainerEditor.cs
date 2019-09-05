using Shared.EnumObjects;
using UnityEditor;

namespace Scenes.WorldScene.Block.BlockDataContainer {
    [CustomEditor(typeof(BlockDataContainer))]
    public class BlockDataContainerEditor : EnumObjectEditor<BlockType, BlockData> { }
}