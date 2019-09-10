using System;
using UnityEditor;
using UnityEngine;

namespace Scenes.WorldScene.Map {
    [CustomEditor(typeof(Terrain))]
    public class MapEditor : Editor {
        private Terrain terrain;

        private void OnEnable() {
            terrain = (Terrain) target;
        }

        public override void OnInspectorGUI() {
            foreach (var ((x, y, z), blockType, index) in terrain) {
                GUILayout.Label($"{blockType} at ({x}, {y}, {z}) ({index}) ({Terrain.Get(x, y, z)})");
            }

            base.OnInspectorGUI();
        }
    }
}