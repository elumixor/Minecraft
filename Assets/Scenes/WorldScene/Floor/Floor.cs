using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Scenes.WorldScene.Floor {
    [ExecuteInEditMode]
    public class Floor : MonoBehaviour, IBuildable {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Vector3 offset;

        private void Update() {
            if (cameraTransform == null) return;

            var tr = transform;

            var pos = tr.position;
            var cameraPos = cameraTransform.position + offset;
            pos.x = cameraPos.x;
            pos.z = cameraPos.z;
            tr.position = pos;

            var rot = tr.rotation.eulerAngles;
            var cameraRot = cameraTransform.rotation.eulerAngles;
            rot.y = cameraRot.y;
            tr.rotation = Quaternion.Euler(rot);
        }

        public Vector3Int GetBuildPosition(Vector3 hitPoint, Vector3 hitNormal) {
            var p = Vector3Int.RoundToInt(hitPoint);
            p.y = 0;
            return p;
        }
    }
}