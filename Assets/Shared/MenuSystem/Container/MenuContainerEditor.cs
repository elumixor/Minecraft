#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Shared.MenuSystem.Container {
    [CustomEditor(typeof(MenuContainer))]
    public class MenuContainerEditor : Editor {
        private MenuContainer menuContainer;
        private SerializedProperty menuScreens;
        private SerializedProperty current;
        private SerializedProperty menuButton;

        private int length;
        private string[] enumNames;

        private void OnEnable() {
            menuContainer = (MenuContainer) target;
            menuScreens = serializedObject.FindProperty("menuScreens");
            current = serializedObject.FindProperty("current");
            menuButton = serializedObject.FindProperty("menuButton");
            enumNames = Enum.GetNames(typeof(MenuContainer.MenuType));
            length = menuScreens.arraySize = enumNames.Length;
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(menuButton, true);

            EditorGUILayout.PropertyField(current, true);

            for (var i = 0; i < length; i++) {
                var oldValue = menuScreens.GetArrayElementAtIndex(i).objectReferenceValue;
                var newValue = EditorGUILayout.ObjectField(enumNames[i], oldValue,
                    typeof(MenuScreen), true);

                if (newValue != oldValue) {
                    menuScreens.GetArrayElementAtIndex(i).objectReferenceValue = newValue;
                }
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif