using System;
using Scenes.WorldScene.Block;
using Scenes.WorldScene.Block.BlockDataContainer;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Scenes.WorldScene.BlockSelection {
    public class BlockSelector : MonoBehaviour {
        // todo: this should be drag n drop from inspector and serialized lists with colors
        [SerializeField] private BlockDataContainer blockDataContainer;

        /// <summary>
        /// Instantiate selector, create cubes
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void Awake() {
//            throw new NotImplementedException();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                selectedType = 0;
            } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                selectedType = (BlockType) 1;
            } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                selectedType = (BlockType) 2;
            } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
                selectedType = (BlockType) 3;
            }
        }

        public BlockType selectedType;
    }
}