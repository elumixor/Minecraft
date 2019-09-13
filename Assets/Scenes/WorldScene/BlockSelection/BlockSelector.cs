using System;
using Shared;
using Shared.Blocks;
using Shared.SingletonBehaviour;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Scenes.WorldScene.BlockSelection {
    public class BlockSelector : SingletonBehaviour<BlockSelector> {
        // todo: this should be drag n drop from inspector and serialized lists with colors
        [SerializeField] private BlockType selectedType;
        [SerializeField] private bool isDestroying;

        private void Update() {
            // todo: change cursor if 
            
            // todo: see if actions do not repeat several times when Key is down
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                selectedType = 0;
                Instance.isDestroying = false;
            } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                selectedType = (BlockType) 1;
                Instance.isDestroying = false;
            } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                selectedType = (BlockType) 2;
                Instance.isDestroying = false;
            } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
                selectedType = (BlockType) 3;
                Instance.isDestroying = false;
            } else if (Input.GetKeyDown(KeyCode.Alpha0)) {
                Instance.isDestroying = true;
            }
        }

        public static BlockType SelectedType {
            get => Instance.selectedType;
            set => Instance.selectedType = value;
        }

        public static bool IsDestroying => Instance.isDestroying;
    }
}