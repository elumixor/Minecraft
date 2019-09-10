//using JetBrains.Annotations;

using System.Collections;
using JetBrains.Annotations;
using Scenes.WorldScene.Block;
using Scenes.WorldScene.BlockSelection;
using Scenes.WorldScene.Map;
using Shared;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Scenes.WorldScene {
    // todo: should be a singleton
    public class UserAction : SingletonBehaviour<UserAction> {
        [SerializeField] private GameObject previewCubeObject;
        [SerializeField] private Image cursor;
        [SerializeField] private Image destroyCursor;

        private Camera mainCamera;
        private Vector2 cursorsDifference;
        private (Block.Block block, (float start, float end) time)? destruction;
        private (Renderer renderer, Transform transform, float opacity) previewCube;

        protected override void Awake() {
            mainCamera = Camera.main;

            var bigRect = destroyCursor.GetComponent<RectTransform>().rect;
            var smallRect = cursor.GetComponent<RectTransform>().rect;
            var widthDiff = bigRect.width / smallRect.width;
            var heightDiff = bigRect.height / smallRect.height;

            cursorsDifference = new Vector2(widthDiff - 1f, heightDiff - 1f);

            var previewRenderer = previewCubeObject.GetComponent<Renderer>();
            previewCube = (previewRenderer, previewCubeObject.transform, previewRenderer.sharedMaterial.color.a);
        }

        private void DestroySelectedBlock(Block.Block block) {
            Destroy(block.gameObject);
            destruction = null;
            destroyCursor.SetColorAlpha(0f);
            cursor.transform.localScale = Vector3.one;
            MapManager.Remove(block.Position);
        }


        private void Update() {
            var destroyingStarted = destruction != null;

            // if destroying started, update timer and conditionally destroy block
            if (destroyingStarted) {
                var now = Time.time;
                var (block, (start, end)) = destruction.Value;
                cursor.transform.localScale = (now - start) / (end - start) * cursorsDifference + Vector2.one;
                if (now >= end) DestroySelectedBlock(block);
            }

            previewCube.renderer.enabled = false;

            if (!Physics.Raycast(mainCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0)), out var hit)) return;

            var objectHit = hit.transform;
            var notDestroying = !BlockSelector.IsDestroying;

            if (!destroyingStarted) {
                if (!(objectHit.GetComponent<IBuildable>() is var buildable) || buildable == null) return;

                var buildPosition = buildable.GetBuildPosition(hit.point, hit.normal);

                if (notDestroying) {
                    previewCube.transform.position = buildPosition;
                    previewCube.renderer.material.color =
                        BlockSelector.SelectedType.BlockData().material.color.SetAlpha(previewCube.opacity);
                    previewCube.renderer.enabled = true;
                }

                if (Input.GetMouseButtonDown(0)) {
                    if (notDestroying) Settings.CreateBlockInstance(BlockSelector.SelectedType, buildPosition);
                    else if (objectHit.GetComponent<Block.Block>() is var block && block != null) {
                        var now = Time.time;
                        destruction = (block, (now, now + block.BlockType.BlockData().durability));
                        destroyCursor.SetColorAlpha(.5f);
                    }
                }
            } else if (Input.GetMouseButtonUp(0)) {
                destruction = null;
                destroyCursor.SetColorAlpha(0f);
                cursor.transform.localScale = Vector3.one;
            }

            if (Input.GetKeyDown(KeyCode.G)) {
                MapManager.ClearMap();

//                IEnumerator Create() {
                    var blockLocations = MapManager.GenerateChunk();
                    foreach (var (blockType, (x, y, z)) in blockLocations) {
                        Settings.CreateBlockInstance(blockType, x, y, z);
//                        yield return null;
                    }
//                }

//                StartCoroutine(Create());
            }
        }
    }
}