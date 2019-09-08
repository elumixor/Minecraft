using System;
using UnityEditor;
using UnityEngine;

namespace Scenes.WorldScene.Map {
    [CustomEditor(typeof(MapManager))]
    public class MapEditor : Editor {
        private MapManager mapManager;

        private void OnEnable() {
            mapManager = (MapManager) target;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
        }
    }
}