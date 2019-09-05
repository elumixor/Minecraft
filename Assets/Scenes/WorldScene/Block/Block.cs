using System;
using Shared;
using UnityEngine;

namespace Scenes.WorldScene.Block {
    [RequireComponent(typeof(MeshRenderer))]
    public class Block : MonoBehaviour, IBuildable, IDestructible {
        [SerializeField] private BlockType blockType;
        [SerializeField] private BlockData blockData;

        public BlockData BlockData => blockData;
        public BlockType BlockType => blockType;

        /// <summary>
        /// Update block material when block type has changed in editor
        /// </summary>
        private void OnValidate() {
            blockData = Resources.Load<BlockDataContainer.BlockDataContainer>("Block Data")[blockType];
            GetComponent<MeshRenderer>().sharedMaterial = blockData.material;
        }
    }
}