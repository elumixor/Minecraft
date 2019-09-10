
using System;
using UnityEditor;
using UnityEngine;

namespace Shared.SingletonBehaviour {
    [CustomEditor(typeof(ReferenceEditMode))]
    public class ReferenceEditor : Editor {
        private ReferenceEditMode reference;

        private void OnEnable() => reference = (ReferenceEditMode) target;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (GUILayout.Button("Test")) {
                reference.LogTest("Editor");
            }
        }
    }
}