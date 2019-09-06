using System;
using UnityEditor;
using UnityEngine;

namespace Scenes.WorldScene.Map {
    [CustomEditor(typeof(MapManager))]
    public class MapManagerEditor : Editor {
        private MapManager mapManager;

        private void OnEnable() {
            mapManager = (MapManager) target;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate Chunk")) mapManager.GenerateChunk();
            if (GUILayout.Button("Create Blocks")) mapManager.CreateBlocks();
        }
    }
}