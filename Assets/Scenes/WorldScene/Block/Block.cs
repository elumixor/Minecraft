using System;
using Shared;
using UnityEngine;

namespace Scenes.WorldScene.Block {
    [RequireComponent(typeof(MeshRenderer))]
    public class Block : MonoBehaviour, IBuildable, IDestructible {
        [SerializeField] private BlockType blockType;

        // todo: this should be unmodifiable from inspector
        [SerializeField] private BlockData blockData;

        [SerializeField] private MeshRenderer meshRenderer;


        public BlockData BlockData => blockData;
        public BlockType BlockType {
            get => blockType;
            set {
                if (value != blockType) {
                    blockType = value;
                    blockData = Settings.BlockDataContainer[blockType];
                    meshRenderer.sharedMaterial = blockData.material;
                }
            }
        }

        private void Awake() => meshRenderer = GetComponent<MeshRenderer>();

        private void Reset() => meshRenderer = GetComponent<MeshRenderer>();

        // todo: this should only run if blockType has changed
        /// <summary>
        /// Update block material when block type has changed in editor
        /// </summary>
        private void OnValidate() {
            BlockType = blockType;
        }
    }
}