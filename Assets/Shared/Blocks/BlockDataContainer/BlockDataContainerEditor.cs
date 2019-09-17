#if UNITY_EDITOR
using Shared.EnumObjects;
using UnityEditor;

namespace Shared.Blocks.BlockDataContainer {
    [CustomEditor(typeof(BlockDataContainer))]
    public class BlockDataContainerEditor : EnumObjectEditor<BlockType, BlockData> { }
}
#endif