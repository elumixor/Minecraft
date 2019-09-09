using Scenes.WorldScene.Map;
using UnityEngine;

namespace Scenes.WorldScene {
    public class PlayerCamera : MonoBehaviour {
        [SerializeField] private Transform playerCamera;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float translateSpeed = 10f;

        private float pitch;
        private float yaw;

        private void Awake() {
            var angles = transform.eulerAngles;
            pitch = angles.x;
            yaw = angles.y;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void LateUpdate() {
            pitch -= rotationSpeed * Input.GetAxis("Mouse Y");
            yaw += rotationSpeed * Input.GetAxis("Mouse X");

            // Clamp pitch:
            pitch = Mathf.Clamp(pitch, -90f, 90f);

            // Wrap yaw:
            while (yaw < 0f) yaw += 360f;
            while (yaw >= 360f) yaw -= 360f;

            // Set orientation:
            playerCamera.eulerAngles = new Vector3(pitch, yaw, 0f);

            var transform1 = transform;

            var oldRotation = transform1.eulerAngles;

            transform1.eulerAngles = playerCamera.eulerAngles;

            if (Input.GetKey(KeyCode.W))
                transform1.Translate(new Vector3(0, 0, translateSpeed * Time.deltaTime));
            if (Input.GetKey(KeyCode.S))
                transform1.Translate(new Vector3(0, 0, -translateSpeed * Time.deltaTime));
            if (Input.GetKey(KeyCode.A))
                transform1.Translate(new Vector3(-translateSpeed * Time.deltaTime, 0, 0));
            if (Input.GetKey(KeyCode.D))
                transform1.Translate(new Vector3(translateSpeed * Time.deltaTime, 0, 0));

            if (Input.GetKey(KeyCode.Z)) {
                var transformPosition = transform1.position;
                transformPosition.y += translateSpeed * Time.deltaTime;
                transform1.position = transformPosition;
            }

            if (Input.GetKey(KeyCode.X)) {
                var transformPosition = transform.position;
                transformPosition.y -= translateSpeed * Time.deltaTime;
                transform1.position = transformPosition;
            }

            transform1.eulerAngles = oldRotation;

            var position = transform1.position;
            var playerGridPosition = Vector3Int.RoundToInt(position);
            var playerPosition = (Vector3) playerGridPosition * Settings.GridUnitWidth;
            var diff = position - playerPosition;

            // todo: camera still passes through blocks if moving diagonally

            if (diff.x > 0 && MapManager.Get(playerGridPosition + Vector3Int.right).HasValue
                || diff.x < 0 && MapManager.Get(playerGridPosition - Vector3Int.right).HasValue) {
                position.x = playerPosition.x;
            }

            if (diff.y > 0 && MapManager.Get(playerGridPosition + Vector3Int.up).HasValue
                || diff.y < 0 && MapManager.Get(playerGridPosition - Vector3Int.up).HasValue) {
                position.y = playerPosition.y;
            }

            if (diff.z > 0 && MapManager.Get(playerGridPosition + new Vector3Int(0, 0, 1)).HasValue
                || diff.z < 0 && MapManager.Get(playerGridPosition - new Vector3Int(0, 0, 1)).HasValue) {
                position.z = playerPosition.z;
            }

            if (diff.y < 0 && playerGridPosition.y == 0) {
                position.y = 0;
            }

            transform1.position = position;
        }
    }
}