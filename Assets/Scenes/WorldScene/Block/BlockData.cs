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
        [Range(0.1f, 2f)]
        public float durability;

        public void Deconstruct(out Material outMaterial) => outMaterial = material;

        public void Deconstruct(out Material outMaterial, out float outDurability) {
            outMaterial = material;
            outDurability = durability;
        }
    }
}