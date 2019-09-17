#if UNITY_EDITOR
using System;
using Shared.Blocks;
using Shared.Blocks.BlockDataContainer;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.WorldScene.BlockSelection.Button {
    [CustomEditor(typeof(BlockSelectionButton))]
    public class BlockSelectionButtonEditor : Editor {
        private BlockSelectionButton button;
        private SerializedProperty blockType;

        private void OnEnable() {
            button = (BlockSelectionButton) target;
            blockType = serializedObject.FindProperty("blockType");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            base.OnInspectorGUI();
            
            var settings = (BlockDataContainer) EditorGUILayout.ObjectField("Set color from settings",
                null, typeof(BlockDataContainer), false);

            if (settings != null) button.GetComponent<Image>().color = ((BlockType) blockType.enumValueIndex).BlockData().material.color;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif