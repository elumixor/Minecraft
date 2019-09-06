using UnityEngine;

namespace Scenes.WorldScene.Block {
    [RequireComponent(typeof(MeshRenderer))]
    public class Block : MonoBehaviour, IBuildable, IDestructible {
        [SerializeField] private BlockType blockType;
        [SerializeField] private MeshRenderer meshRenderer;

        public BlockType BlockType {
            get => blockType;
            set {
                if (value != blockType) {
                    blockType = value;
                    meshRenderer.sharedMaterial = blockType.BlockData().material;
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
        public float Durability => blockType.BlockData().durability;
        public GameObject GameObject => gameObject;
    }
}