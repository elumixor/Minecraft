using UnityEditor;
using UnityEngine;

namespace Shared.SingletonBehaviour {
    /// <summary>
    /// Singleton is not guaranteed to be initialized at OnValidate and Awake
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ExecuteInEditMode]
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T> {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Lock = new object();
        private static T instance;

        private static void AssignInstance() {
            var instances = FindObjectsOfType<T>();
            if (instances.Length > 1) {
                if (instance == null) {
                    for (var i = 1; i < instances.Length; i++) {
                        Debug.LogWarning(
                            $"[Singleton] Instance '{typeof(T)}' already exists in the scene, removing {instances[i]}");
                        DestroyImmediate(instances[i].gameObject);
                    }

                    instance = instances[0];
                } else {

                    foreach (var i in instances) {
                        if (i != instance) {
                            Debug.LogWarning(
                                $"[Singleton] Instance '{typeof(T)}' already exists in the scene, removing {i}");
                            DestroyImmediate(i.gameObject);
                        }
                    }
                }
            } else if (instances.Length == 1)
                instance = instances[0];
            else
                instance = new GameObject($"{typeof(T)} (Singleton)").AddComponent<T>();

            // todo: #if UNITY_EDITOR... conditional check
            if (Application.isPlaying) {
                DontDestroyOnLoad(instance);
                DontDestroyOnLoad(instance.gameObject);
            }
        }

        // todo: Replace existing singleton behaviour in scene
        protected virtual void Awake() {
            lock (Lock) AssignInstance();
        }

        protected static T Instance {
            get {
                lock (Lock) {
                    if (instance == null) AssignInstance();
                    return instance;
                }
            }
        }

        private void OnDestroy() {
            lock (Lock) if (instance == this) instance = null;
        }
    }
}