using UnityEditor;
using UnityEngine;

namespace Shared.SingletonBehaviour {
    /// <summary>
    /// Singleton is not guaranteed to be initialized at OnValidate and Awake
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ExecuteInEditMode]
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T> {
        private static object @lock = new object();
        private static T instance;

        private static void AssignInstance() {
            var instances = FindObjectsOfType<T>();
            if (instances.Length > 1) {
                instance = instances[0];
                for (var i = 1; i < instances.Length; i++) {
                    Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already exists in the scene, removing {instances[i]}");
                    DestroyImmediate(instances[i]);
                }
            } else if (instances.Length == 1)
                instance = instances[0];
            else
                instance = new GameObject($"{typeof(T)} (Singleton)").AddComponent<T>();
        }

        protected virtual void Awake() {
            lock (@lock)
                if (instance == null) {
                    AssignInstance();
                    instance = (T) this;
                    if (EditorApplication.isPlaying) DontDestroyOnLoad(this);
                } else if (instance != this) {
                    Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already exists in the scene, removing {this}");
                    if (EditorApplication.isPlaying) Destroy(this);
                    else DestroyImmediate(this);
                }
        }

        protected static T Instance {
            get {
                lock (@lock) {
                    if (instance == null) AssignInstance();
                    return instance;
                }
            }
        }

        private void OnDestroy() {
            lock (@lock) instance = null;
        }
    }
}