using System.Collections;
using System.Collections.Generic;
using Shared.EnumObjects;
using UnityEngine;

namespace Scenes.WorldScene.Block.BlockDataContainer {
    /// <summary>
    /// Stores and maps block types to <see cref="BlockData"/>
    /// </summary>
    [CreateAssetMenu(fileName = "Block Data", menuName = "Custom/Block Data", order = 0)]
    public class BlockDataContainer : EnumScriptableObject<BlockType, BlockData>, IEnumerable<BlockData> {
        public IEnumerator<BlockData> GetEnumerator() => ((IEnumerable<BlockData>) values).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}