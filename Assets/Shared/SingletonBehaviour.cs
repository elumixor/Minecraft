using UnityEngine;

namespace Shared {
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T> {
        protected static T Instance { get; private set; }

        protected virtual void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(this);
                throw new System.Exception($"An instance of singleton {name} already exists.");
            }

            Instance = (T) this;
        }
    }
}