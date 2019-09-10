using UnityEngine;

namespace Shared.SingletonBehaviour {
    public class TestSingleton : SingletonBehaviour<TestSingleton> {
        private int value;
        public static int Value => Instance.value;

        protected override void Awake() {
            Debug.Log("Singleton Awake");
            base.Awake();
            value = 5;
        }
    }
}