using System;
using System.Collections.Generic;
using Shared.SingletonBehaviour;
using UnityEngine;

namespace Shared.Pooling {
    public class Pooler : SingletonBehaviour<Pooler> {
        [Serializable]
        private struct Pool {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        [SerializeField] private List<Pool> pools;
        private Dictionary<string, (GameObject prefab, Queue<GameObject> queue)> poolsDictionary;

        private static Transform objectsContainer;
        
        // Public API

        /// <summary>
        /// Request an object from the pool
        /// </summary>
        /// <returns></returns>
        public static GameObject Request(string tag) {
            if (!Instance.poolsDictionary.TryGetValue(tag, out var kvp)) {
                Debug.LogWarning($"Pool with tag '{tag}' does not exist.");
                return null;
            }

            var gameObject = kvp.queue.Count == 0 ? ExpandQueue(kvp.queue, kvp.prefab) : kvp.queue.Dequeue();
            foreach (var handler in gameObject.GetComponents<IPoolRequestHandler>()) handler.OnPoolRequested();
            gameObject.SetActive(true);
            return gameObject;
        }

        /// <summary>
        /// Return an object to the pool
        /// </summary>
        public static void Return(string tag, GameObject gameObject) {
            if (!Instance.poolsDictionary.TryGetValue(tag, out var kvp)) {
                Debug.LogWarning($"Pool with tag '{tag}' does not exist.");
                return;
            }
            
            foreach (var handler in gameObject.GetComponents<IPoolReturnHandler>()) handler.OnPoolReturned();
            gameObject.SetActive(false);
            kvp.queue.Enqueue(gameObject);
        }

        // todo: maybe instantiate as coroutine 

        /// <summary>
        /// Populate pools on awake
        /// </summary>
        protected override void Awake() {
            base.Awake();

            objectsContainer = new GameObject("Pool objects container").transform;
            DontDestroyOnLoad(objectsContainer);

            poolsDictionary = new Dictionary<string, (GameObject prefab, Queue<GameObject> queue)>(pools.Count);

            foreach (var pool in pools) {
                var queue = new Queue<GameObject>(pool.size);
                poolsDictionary[pool.tag] = (pool.prefab, queue);
                for (var i = 0; i < pool.size; i++) ExpandQueue(queue, pool.prefab);
            }
        }

        private static GameObject ExpandQueue(Queue<GameObject> queue, GameObject prefab) {
            var gameObject = Instantiate(prefab, objectsContainer, true);
            gameObject.SetActive(false);
            queue.Enqueue(gameObject);
            return gameObject;
        }
    }
}