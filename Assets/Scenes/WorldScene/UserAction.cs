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
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit)) {
                    var objectHit = hit.transform;

                    var newBlockPosition = hit.transform.position + hit.normal;
//                    var newBlockRotation = hit.transform.rotation;

                    if (objectHit.GetComponent<IBuildable>() is var buildable)
                        buildable.CreateBlock(newBlockPosition, blockSelector.selectedType);

                    // Do something with the object that was hit by the raycast.
                }
            }
        }
    }
}