using System;
using UnityEditor;
using UnityEngine;

namespace Scenes.WorldScene {
    [CustomEditor(typeof(GeneratedMesh))]
    public class GeneratedMeshEditor : Editor {
        private GeneratedMesh generatedMesh;
        private void OnEnable() {
            generatedMesh = (GeneratedMesh) target;
            Tools.hidden = true;
        }

        private void OnDisable() {
            Tools.hidden = false;
        }

        private void OnSceneGUI() {
//            for (var index = 0; index < generatedMesh.vertices.Length; index++) {
//                var vertex = generatedMesh.vertices[index];
//                var newVertex = Handles.PositionHandle(vertex + generatedMesh.transform.position, Quaternion.identity) - generatedMesh.transform.position;
//                if (newVertex != vertex) {
//                    generatedMesh.vertices[index] = newVertex;
//                    generatedMesh.UpdateMesh();
//                }
//            }
        }
    }
}