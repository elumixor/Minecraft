using System;
using Scenes.WorldScene.Block.BlockDataContainer;
using UnityEditor;
using UnityEngine;

namespace Scenes.WorldScene.BlockSelection {
    [CustomEditor(typeof(BlockSelector))]
    public class BlockSelectorEditor : Editor {
        private BlockSelector blockSelector;
//        private SerializeField smallSize

        private void OnEnable() {
            blockSelector = (BlockSelector) target;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            GUI.enabled = !EditorApplication.isPlaying;
//            if (GUILayout.Button("Create buttons")) {
//                // Destroy all children
//                while (blockSelector.transform.childCount > 0) DestroyImmediate(blockSelector.transform.GetChild(0));
//
//                var settings = (BlockDataContainer) typeof(BlockSelector).GetField("blockDataContainer").GetValue(blockSelector);
//                foreach (var (material) in settings) {
//
//                    material.color
//                }
//            }

            GUI.enabled = true;
        }
    }
}