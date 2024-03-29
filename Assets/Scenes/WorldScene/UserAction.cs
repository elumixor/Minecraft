//using JetBrains.Annotations;

using System.Collections;
using JetBrains.Annotations;
using Shared;
using Shared.Blocks;
using Shared.BlockSelection;
using Shared.GameManagement;
using Shared.MenuSystem.Container;
using Shared.Positioning;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Scenes.WorldScene {
    // todo: should be a singleton
    public class UserAction : MonoBehaviour {
        [SerializeField] private GameObject previewCubeObject;
        [SerializeField] private Image cursor;
        [SerializeField] private Image destroyCursor;
        [SerializeField] private ParticleSystem onDestroyParticleSystem;

        private Camera mainCamera;
        private Vector2 cursorsDifference;
        private (Block block, (float start, float end) time)? destruction;
        private (Renderer renderer, Transform transform, float opacity) previewCube;

        protected void Awake() {
            mainCamera = Camera.main;

            var bigRect = destroyCursor.GetComponent<RectTransform>().rect;
            var smallRect = cursor.GetComponent<RectTransform>().rect;
            var widthDiff = bigRect.width / smallRect.width;
            var heightDiff = bigRect.height / smallRect.height;

            cursorsDifference = new Vector2(widthDiff - 1f, heightDiff - 1f);

            var previewRenderer = previewCubeObject.GetComponent<Renderer>();
            previewCube = (previewRenderer, previewCubeObject.transform, previewRenderer.sharedMaterial.color.a);
        }

        private void DestroySelectedBlock(Block block) {
            Map.Remove(block.Position);
            Block.Destroy(block.Position);
            var particle = Instantiate(onDestroyParticleSystem, block.transform.position, Quaternion.identity);
            particle.GetComponent<ParticleSystemRenderer>().material = block.GetComponent<Renderer>().material;
            destruction = null;
            destroyCursor.SetColorAlpha(0f);
            cursor.transform.localScale = Vector3.one;
        }

        private static void CreateBlockAt(WorldPosition buildPosition) {
            Map.AddBlock(BlockSelector.SelectedType, buildPosition);
            Block.Instantiate(BlockSelector.SelectedType, buildPosition);
        }


        private void Update() {
            if (MenuContainer.MenuDisplayed) return;

            if (Input.GetKeyDown(KeyCode.Escape)) {
                Cursor.lockState = CursorLockMode.Confined;
                MenuContainer.ActiveMenu = MenuContainer.MenuType.Pause;
                Cursor.visible = true;
                return;
            }

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

                if (notDestroying && buildPosition != Player.Position) {
                    previewCube.transform.position = (Vector3) buildPosition * Settings.GridUnitWidth;
                    previewCube.renderer.material.mainTexture = BlockSelector.SelectedType.BlockData().material.mainTexture;
                    previewCube.renderer.enabled = true;
                }

                if (Input.GetMouseButtonDown(0)) {
                    if (notDestroying) {
                        if (buildPosition != Player.Position)
                            CreateBlockAt(buildPosition);
                    } else if (objectHit.GetComponent<Block>() is var block && block != null) {
                        var now = Time.time;
                        destruction = (block, (now, now + block.BlockType.BlockData().durability));
                        destroyCursor.SetColorAlpha(.5f);
                    }
                } else if (Input.GetMouseButtonDown(1) && objectHit.GetComponent<Block>() is var block &&
                           block != null) {
                    var now = Time.time;
                    destruction = (block, (now, now + block.BlockType.BlockData().durability));
                    destroyCursor.SetColorAlpha(.5f);
                }
            } else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) {
                destruction = null;
                destroyCursor.SetColorAlpha(0f);
                cursor.transform.localScale = Vector3.one;
            }
        }
    }
}