using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Shared {
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

//        /// <summary>
//        /// Access singleton instance through this propriety.
//        /// </summary>
//        protected static T Instance {
//            get {
//                if (shuttingDown) {
//                    Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null.");
//                    return null;
//                }
//
//                lock (@lock) {
//                    if (instance == null) {
//                        // Search for existing instance.
//                        instance = FindObjectOfType<T>();
//
//                        // Create new instance if one doesn't already exist.
//                        if (instance == null) {
//                            // Need to create a new GameObject to attach the singleton to.
//                            var singletonObject = new GameObject();
//                            instance = singletonObject.AddComponent<T>();
//                            singletonObject.name = $"{typeof(T)} (Singleton)";
//
//                            // Make instance persistent.
//                            DontDestroyOnLoad(singletonObject);
//                        }
//                    }
//
//                    return instance;
//                }
//            }
//        }

//        private void OnApplicationQuit() => shuttingDown = true;

        private void OnDestroy() {
            lock (@lock) instance = null;
        }
    }
}