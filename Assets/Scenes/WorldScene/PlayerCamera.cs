using System;
using UnityEngine;

namespace Scenes.WorldScene {
    public class PlayerCamera : MonoBehaviour {
        private float pitch;
        private float yaw;

        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float translateSpeed = 10f;

        private void Awake() {
            var angles = transform.eulerAngles;
            pitch = angles.x;
            yaw = angles.y;
            Cursor.lockState = CursorLockMode.Locked;
        }


        private void LateUpdate() {
            var transform1 = transform;
            pitch -= rotationSpeed * Input.GetAxis("Mouse Y");
            yaw += rotationSpeed * Input.GetAxis("Mouse X");

            // Clamp pitch:
            pitch = Mathf.Clamp(pitch, -90f, 90f);

            // Wrap yaw:
            while (yaw < 0f) {
                yaw += 360f;
            }

            while (yaw >= 360f) {
                yaw -= 360f;
            }

            // Set orientation:
            transform1.eulerAngles = new Vector3(pitch, yaw, 0f);

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
        }
    }
}