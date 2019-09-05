using System;
using UnityEngine;

namespace Scenes.WorldScene.Block {
    [Serializable]
    public struct BlockData {
        /// <summary>
        /// Material that is used to render the block
        /// </summary>
        public Material material;

        /// <summary>
        /// Time in seconds, that is needed to destroy the block
        /// </summary>
        [Range(0, 10)]
        public float durability;
    }
}