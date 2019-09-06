using System;
using UnityEditor;
using UnityEngine;

namespace Scenes.WorldScene.Block {
    [CustomEditor(typeof(Block))]
    public class BlockEditor : Editor {
        private Block block;
        private SerializedProperty position;

//        private void OnEnable() {
//            block = (Block) target;
//            position = serializedObject.FindProperty("position");
//        }
//
//        public override void OnInspectorGUI() {
////            base.OnInspectorGUI();
//            serializedObject.Update();
//            using (var changed = new EditorGUI.ChangeCheckScope()) {
//                EditorGUILayout.PropertyField(position);
//
//                if (changed.changed) {
//                    var pos = position.vector3IntValue;
//                    block.transform.position = new Vector3(pos.x, pos.y, pos.z) * Settings.GridUnitWidth;
//                }
//            }
//
//            serializedObject.ApplyModifiedProperties();
//        }
    }
}