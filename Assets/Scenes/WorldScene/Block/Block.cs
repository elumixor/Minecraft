using UnityEditor;
using UnityEngine;

namespace Scenes.WorldScene.Block {
    [RequireComponent(typeof(MeshRenderer))]
    public class Block : MonoBehaviour, IBuildable, IDestructible {
        [SerializeField] private BlockType blockType;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Vector3Int position;

        public BlockType BlockType {
            set {
                if (value != blockType) {
                    blockType = value;
                    meshRenderer.sharedMaterial = blockType.BlockData().material;
                }
            }
        }

        public Vector3Int Position {
            get => position;
            set {
                position = value;
                if (position.y < 0) position.y = 0;
                transform.position = new Vector3(position.x, position.y, position.z) * Settings.GridUnitWidth;
            }
        }

        private void Awake() => meshRenderer = GetComponent<MeshRenderer>();

        private void Reset() => meshRenderer = GetComponent<MeshRenderer>();

        // todo: this should only run if blockType has changed
        /// <summary>
        /// Update block material when block type has changed in editor
        /// </summary>
        private void OnValidate() {
            Debug.Log("on validate");
            meshRenderer.sharedMaterial = blockType.BlockData().material;

            if (position.y < 0) position.y = 0;
            transform.position = new Vector3(position.x, position.y, position.z) * Settings.GridUnitWidth;
        }
        public float Durability => blockType.BlockData().durability;
        public GameObject GameObject => gameObject;
    }
}