//using JetBrains.Annotations;

using JetBrains.Annotations;
using Scenes.WorldScene.Block;
using Scenes.WorldScene.BlockSelection;
using Shared;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Scenes.WorldScene {
    // todo: should be a singleton
    public class UserAction : MonoBehaviour {
        [SerializeField] private BlockSelector blockSelector;
        private Camera mainCamera;

        private void Awake() {
            mainCamera = Camera.main;
        }

        [SerializeField] private Image cursor;
        [SerializeField] private Image destroyCursor;

        [CanBeNull] private IDestructible selectedDestructible;
        private float destructionEndTime;
        private float destructionStartTime;

        [SerializeField] private GameObject previewCube;

        private void Update() {
            if (selectedDestructible != null) {
                var now = Time.time;
                var bigRect = destroyCursor.GetComponent<RectTransform>().rect;
                var smallRect = cursor.GetComponent<RectTransform>().rect;
                var widthDiff = bigRect.width / smallRect.width;
                var heightDiff = bigRect.height / smallRect.height;

                cursor.transform.localScale = (now - destructionStartTime) / (destructionEndTime - destructionStartTime)
                                              * (new Vector2(widthDiff, heightDiff) - Vector2.one) + Vector2.one;
                if (now >= destructionEndTime) {
                    selectedDestructible.Destroy();
                    selectedDestructible = null;
                    destroyCursor.SetColorAlpha(0f);
                    cursor.transform.localScale = Vector3.one;
                }
            }

            previewCube.GetComponent<Renderer>().enabled = false;

            var ray1 = mainCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            if (Physics.Raycast(ray1, out var hit1)) {
                var objectHit = hit1.transform;

                if (selectedDestructible == null) {
                    Debug.Log(objectHit);
                    if (objectHit.GetComponent<Block.Block>() != null) {
                        previewCube.transform.position = hit1.transform.position + hit1.normal;
                        previewCube.GetComponent<Renderer>().enabled = true;
                        var opacity = previewCube.GetComponent<Renderer>().sharedMaterial.color.a;
                        previewCube.GetComponent<Renderer>().sharedMaterial.color =
                            blockSelector.selectedType.BlockData().material.color.SetAlpha(opacity);
                    } else if (objectHit.GetComponent<Floor.Floor>() != null) {
                        var hitPoint = Vector3Int.RoundToInt(hit1.point);
                        hitPoint.y = 0;
                        previewCube.transform.position = hitPoint;
                        previewCube.GetComponent<Renderer>().enabled = true;
                        var opacity = previewCube.GetComponent<Renderer>().sharedMaterial.color.a;
                        previewCube.GetComponent<Renderer>().sharedMaterial.color =
                            blockSelector.selectedType.BlockData().material.color.SetAlpha(opacity);
                    }
                }
            }

            if (Input.GetMouseButtonDown(0) && selectedDestructible == null) {
                var ray = mainCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                if (Physics.Raycast(ray, out var hit)) {
                    var objectHit = hit.transform;
                    var block = objectHit.GetComponent<Block.Block>();
                    if (block != null)
                        block.CreateBlock(block.Position + Vector3Int.RoundToInt(hit.normal),
                            blockSelector.selectedType);
                    else {
                        var floor = objectHit.GetComponent<Floor.Floor>();
                        if (floor != null)
                            floor.CreateBlock(Vector3Int.RoundToInt(hit.point), blockSelector.selectedType);
                    }

//                    var buildable = objectHit.GetComponent<IBuildable>();
//                    Debug.Log(hit.normal);
                }
            } else if (Input.GetMouseButtonDown(1)) {
                var ray = mainCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                if (Physics.Raycast(ray, out var hit)) {
                    var objectHit = hit.transform;

                    selectedDestructible = objectHit.GetComponent<IDestructible>();
                    if (selectedDestructible != null) {
                        var durability = selectedDestructible.Durability;
                        if (durability < 1e-5f) {
                            selectedDestructible.Destroy();
                            selectedDestructible = null;
                        } else {
                            destructionStartTime = Time.time;
                            destructionEndTime = destructionStartTime + selectedDestructible.Durability;
                            destroyCursor.SetColorAlpha(.5f);
                        }
                    }
                }
            } else if (Input.GetMouseButtonUp(1)) {
                if (selectedDestructible != null) {
                    selectedDestructible = null;
                    destroyCursor.SetColorAlpha(0f);
                    cursor.transform.localScale = Vector3.one;
                }
            }
        }
    }
}