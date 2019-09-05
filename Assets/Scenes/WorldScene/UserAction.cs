using Scenes.WorldScene.BlockSelection;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Scenes.WorldScene {
    // todo: should be a singleton
    public class UserAction : MonoBehaviour {
        [SerializeField] private BlockSelector blockSelector;
        private Camera mainCamera;

        private void Awake() {
            mainCamera = Camera.main;
        }
        private void Update() {
            // check for touch
            if (Input.GetMouseButtonDown(0)) {
                var ray = mainCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                if (Physics.Raycast(ray, out var hit)) {
                    var objectHit = hit.transform;

                    if (objectHit.GetComponent<IBuildable>() is var buildable)
                        buildable.CreateBlock(hit.transform.position + hit.normal, blockSelector.selectedType);
                }
            } else if (Input.GetMouseButtonDown(1)) {
                var ray = mainCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
                if (Physics.Raycast(ray, out var hit)) {
                    var objectHit = hit.transform;

                    if (objectHit.GetComponent<IDestructible>() is MonoBehaviour destructible)
                        Destroy(destructible.gameObject);
                }
            }
        }
    }
}