using System;
using Scenes.WorldScene.Block.BlockDataContainer;
using UnityEngine;

namespace Scenes.WorldScene {
//    [ExecuteInEditMode]
    public class Settings : MonoBehaviour {
        [SerializeField] private Block.Block blockPrefab;
        [SerializeField] private BlockDataContainer blockDataContainer;
        [SerializeField] private Transform blocksContainer;


        private static Settings instance;
        private static Settings Instance => instance == null ? instance = FindObjectOfType<Settings>() : instance;
        public static Block.Block BlockPrefab => Instance.blockPrefab;
        public static BlockDataContainer BlockDataContainer => Instance.blockDataContainer;

        public static Transform BlocksContainer => Instance.blocksContainer;
    }
}