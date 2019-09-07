using System;
using UnityEditor;
using UnityEngine;

namespace Scenes.WorldScene.Block {
    [CustomEditor(typeof(Block))]
    public class BlockEditor : Editor {
        private Block block;
        private SerializedProperty position;
        private SerializedProperty blockType;
        private MeshRenderer meshRenderer;

        private void OnEnable() {
            block = (Block) target;
            position = serializedObject.FindProperty("position");
            blockType = serializedObject.FindProperty("blockType");
            meshRenderer = block.GetComponent<MeshRenderer>();
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            using (var changed = new EditorGUI.ChangeCheckScope()) {
                EditorGUILayout.PropertyField(blockType);

                if (changed.changed) {
                    meshRenderer.sharedMaterial = ((BlockType) blockType.enumValueIndex).BlockData().material;
                }
            }

            using (var changed = new EditorGUI.ChangeCheckScope()) {
                EditorGUILayout.PropertyField(position);

                if (changed.changed) {
                    var pos = position.vector3IntValue;
                    block.transform.position = new Vector3(pos.x, pos.y, pos.z) * Settings.GridUnitWidth;
                }
            }


            serializedObject.ApplyModifiedProperties();
        }
    }
}