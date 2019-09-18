using System;
using System.Collections.Generic;
using System.Linq;
using Scenes.WorldScene.BlockSelection.Button;
using Shared;
using Shared.Blocks;
using Shared.SingletonBehaviour;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Scenes.WorldScene.BlockSelection {
    public class BlockSelector : SingletonBehaviour<BlockSelector> {
        // todo: this should be drag n drop from inspector and serialized lists with colors
        [SerializeField] private List<SelectorIndicator> selectors;
        [SerializeField] private SelectorIndicator destroySelector;
        [SerializeField] private bool isDestroying;

        private void Start() {
#if UNITY_EDITOR
            if (Application.isPlaying && selectors != null && selectors.Count > 0)
#endif
                SelectedType = selectors.First().Select();
        }

        private void Update() {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif

            foreach (var selector in selectors.Where(selector => Input.GetKeyDown(selector.Hotkey))) {
                SelectedType = selector.Select();
                destroySelector.Deselect();
                isDestroying = false;
                foreach (var selector2 in selectors.Where(selector2 => selector2 != selector)) selector2.Deselect();

                break;
            }
#if UNITY_EDITOR
            if (destroySelector == null) return;
#endif

            if (Input.GetKeyDown(destroySelector.Hotkey)) {
                destroySelector.Select();
                foreach (var selector in selectors) selector.Deselect();
                isDestroying = true;
            }
        }

        public static BlockType SelectedType { get; private set; }
        public static bool IsDestroying => Instance.isDestroying;
    }
}