using Shared.EnumObjects;
using UnityEngine;

namespace Scenes.WorldScene.Block.BlockDataContainer {
    /// <summary>
    /// Stores and maps block types to <see cref="BlockData"/>
    /// </summary>
    [CreateAssetMenu(fileName = "Block Data", menuName = "Custom/Block Data", order = 0)]
    public class BlockDataContainer : EnumScriptableObject<BlockType, BlockData> {}
}