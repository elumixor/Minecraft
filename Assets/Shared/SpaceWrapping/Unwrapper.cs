using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Shared.SpaceWrapping {
    public abstract class Unwrapper : MonoBehaviour {
        [SerializeField, Range(0.01f, 1f)] private float spawnTime;

        [Pure]
        protected abstract void Unwrap(int i, out int x, out int y, out int z);

        [Pure]
        protected abstract int Wrap(int x, int y, int z);

        private bool shouldStop;
        private bool started;
        private int i;

        private void Start() {
            for (var z = -5; z <= 5; z++)
            for (var y = -5; y <= 5; y++)
            for (var x = -5; x <= 5; x++) {
                var index = Wrap(x, y, z);
                Unwrap(index, out var vx, out var vy, out var vz);
                Debug.Assert(x == vx && y == vy && z == vz,
                    $"{x} {y} {z} produced {index}, which produced {vx} {vy} {vz}");
            }
        }

        private void Unwrap() {
            IEnumerator UnwrapCoroutine() {
                while (!shouldStop) {
                    var t = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
                    t.gameObject.name = $"{i}";
                    Unwrap(i++, out var x, out var y, out var z);
                    t.position = new Vector3(x, y, z);
                    yield return new WaitForSeconds(spawnTime);
                }
            }

            if (!started) {
                started = true;
                StartCoroutine(UnwrapCoroutine());
            } else {
                shouldStop = false;
                StartCoroutine(UnwrapCoroutine());
            }
        }

        private static readonly Dictionary<int, int> SqrtDictionary = new Dictionary<int, int>();
        protected static int Sqrt(int value) {
            if (SqrtDictionary.ContainsKey(value)) return SqrtDictionary[value];

            return SqrtDictionary[value] = Mathf.FloorToInt((float) Math.Sqrt(value));
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.U)) Unwrap();
            else if (Input.GetKeyDown(KeyCode.S)) shouldStop = true;
        }
    }
}