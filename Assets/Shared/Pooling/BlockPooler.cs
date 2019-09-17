using System;
using System.Collections.Generic;
using Shared.GameManagement;
using Shared.SingletonBehaviour;
using UnityEngine;

namespace Shared.Pooling {
    public class BlockPooler : SingletonBehaviour<BlockPooler> {
        [SerializeField] private GameObject prefab;
        [SerializeField, Range(0, 10000)] private int size;

        private static Queue<GameObject> queue = new Queue<GameObject>();

        // Public API

        /// <summary>
        /// Request an object from the pool
        /// </summary>
        /// <returns></returns>
        public static GameObject Request() {
            var gameObject = queue.Count == 0 ? ExpandQueue() : queue.Dequeue();
            gameObject.SetActive(true);
            return gameObject;
        }

        /// <summary>
        /// Return an object to the pool
        /// </summary>
        public static void Return(GameObject gameObject) {
            gameObject.SetActive(false);
            queue.Enqueue(gameObject);
        }

        // todo: maybe instantiate as coroutine 

        /// <summary>
        /// Populate pools on awake
        /// </summary>
        protected override void Awake() {
            base.Awake();

            // check for null because other instance of singleton may be present in the scene
            if (!Application.isPlaying || this == null) return;

            transform.DestroyAllChildren();
            queue = new Queue<GameObject>(size);
            for (var i = 0; i < size; i++) queue.Enqueue(ExpandQueue());
        }

        private static GameObject ExpandQueue() {
            var gameObject = Instantiate(Instance.prefab, Instance.transform, true);
            gameObject.SetActive(false);
            return gameObject;
        }
    }
}