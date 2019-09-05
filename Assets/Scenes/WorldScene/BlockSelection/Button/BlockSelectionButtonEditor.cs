using System;
using Scenes.WorldScene.Block;
using Scenes.WorldScene.Block.BlockDataContainer;
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

//            EditorGUILayout.PropertyField(blockType);

            var settings = (BlockDataContainer)EditorGUILayout.ObjectField("Set color from settings",
                null, typeof(BlockDataContainer), false);

            if (settings != null)
                button.GetComponent<Image>().color = settings[(BlockType) blockType.enumValueIndex].material.color;

            serializedObject.ApplyModifiedProperties();
        }
    }
}