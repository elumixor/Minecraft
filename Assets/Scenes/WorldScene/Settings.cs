using System;
using Scenes.WorldScene.Block.BlockDataContainer;
using UnityEngine;

namespace Scenes.WorldScene {
//    [ExecuteInEditMode]
    public class Settings : MonoBehaviour {
        [SerializeField] private Block.Block blockPrefab;
        [SerializeField] private BlockDataContainer blockDataContainer;
        [SerializeField] private Transform blocksContainer;

        [SerializeField] private Transform floor;
        [SerializeField, Range(0, 50)] private float gridUnitWidth;

        private static Settings instance;
        private static Settings Instance => instance == null ? instance = FindObjectOfType<Settings>() : instance;
        public static Block.Block BlockPrefab => Instance.blockPrefab;
        public static BlockDataContainer BlockDataContainer => Instance.blockDataContainer;
        public static Transform BlocksContainer => Instance.blocksContainer;

        private void OnValidate() {
            if (floor == null) return;

            var tr = floor.transform;
            var transformPosition = tr.position;
            transformPosition.y = -gridUnitWidth * .5f - tr.localScale.y *.5f;
            tr.position = transformPosition;
        }
    }
}