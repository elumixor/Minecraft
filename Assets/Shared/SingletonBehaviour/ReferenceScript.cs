using System;
using UnityEngine;

namespace Shared.SingletonBehaviour {
    public class ReferenceScript : MonoBehaviour {
        private void LogTest(string testName) => Debug.Log($"{testName}: {TestSingleton.Value}");
        private void Reset() => LogTest("Reset");
//        private void OnValidate() => LogTest("OnValidate");
//        private void Awake() => LogTest("Awake");
        private void Start() => LogTest("Start");
    }
}