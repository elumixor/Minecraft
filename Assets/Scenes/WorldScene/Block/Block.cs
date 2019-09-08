using System;
using Scenes.WorldScene.Map;
using UnityEditor;
using UnityEngine;

namespace Scenes.WorldScene.Block {
    [RequireComponent(typeof(MeshRenderer))]
    public class Block : MonoBehaviour, IBuildable {
        [SerializeField] private BlockType blockType;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Vector3Int position;

        public BlockType BlockType {
            get => blockType;
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

        private void Awake() {
            Reset();
        }

        private void Start() {
            MapManager.Set(blockType, position);
        }

        private void OnDestroy() {
            MapManager.Remove(position);
        }

        private void Reset() => meshRenderer = GetComponent<MeshRenderer>();

        public Vector3Int GetBuildPosition(Vector3 hitPoint, Vector3 hitNormal) =>
            position + Vector3Int.RoundToInt(hitNormal);
    }
}