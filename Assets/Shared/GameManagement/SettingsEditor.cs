using System.Collections.Generic;
#if UNITY_EDITOR
using System;
using Shared.Blocks;
using UnityEditor;
using UnityEngine;

namespace Shared.GameManagement {
    [CustomEditor(typeof(Settings))]
    public class SettingsEditor : Editor {
        private Settings settings;
        private SerializedProperty blockData;
        private SerializedProperty playerConstructable;


        private int length;
        private string[] enumNames;


        private void OnEnable() {
            settings = (Settings) target;
            blockData = serializedObject.FindProperty("blockData");
            playerConstructable = serializedObject.FindProperty("playerConstructable");

            enumNames = Enum.GetNames(typeof(BlockType));
            length = blockData.arraySize = enumNames.Length;
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            var selectedCount = playerConstructable.arraySize;
            var selected = new List<int>(playerConstructable.arraySize);
            for (var i = 0; i < selectedCount; i++)
                selected.Add(playerConstructable.GetArrayElementAtIndex(i).intValue);

            for (var i = 0; i < length; i++) {
                using (new GUILayout.HorizontalScope()) {
                    EditorGUILayout.PropertyField(blockData.GetArrayElementAtIndex(i), 
                        new GUIContent(enumNames[i]), true);

                    var contained = selected.Contains(i);
                    var contains = EditorGUILayout.Toggle("Player Constructable", contained);

                    if (contained && !contains) selected.Remove(i);
                    else if (contains && !contained) selected.Add(i);
                }
            }

            playerConstructable.arraySize = selected.Count;
            for (var i = 0; i < selected.Count; i++)
                playerConstructable.GetArrayElementAtIndex(i).intValue = selected[i];

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif