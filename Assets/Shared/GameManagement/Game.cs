using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Shared.Blocks;
using Shared.SingletonBehaviour;
using Shared.SpaceWrapping;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shared.GameManagement {
    public class Game : SingletonBehaviour<Game> {
        [SerializeField] private SceneField mainMenuScene;
        [SerializeField] private SceneField worldScene;
        [SerializeField] private int seed;

        [CanBeNull] private static Action onLoaded;

        // Public API
        /// <summary>
        /// Saves game state to a binary file
        /// </summary>
        /// <param name="fileName">Name of saved data file</param>
        public static void Save(string fileName) {
            // todo: warn if file exists -> replace/cancel
            using (var writer = new BinaryWriter(File.Open(fileName, FileMode.Create))) WriteFile(writer);
        }

        /// <summary>
        /// Loads game state from file, creates map, blocks and places player at saved position
        /// </summary>
        /// <param name="fileName">Path to saved data file</param>
        /// <exception cref="FileLoadException">If could not correctly load save file</exception>
        public static void Load(string fileName) {
//            try {
            using (var reader = new BinaryReader(File.Open(fileName, FileMode.Create))) {
                ReadFile(reader, out var map, out var zero, out var playerPosition);
                SceneManager.LoadScene(Instance.worldScene);
                Map.CreateFrom(map);
                Map.Zero = zero;
                PlayerPosition.GlobalPosition = playerPosition;
            }

//            } catch (Exception e) {
//                throw new FileLoadException($"Could not load save file at {fileName}", e);
//            }
        }

        public static void New() {
            SceneManager.LoadScene(Instance.worldScene);
            onLoaded = () => {
                Map.Zero = (Instance.seed, Instance.seed);

                // todo: make this vertically correct, e.g. y should be non-negative
                for (var x = -1; x <= 1; x++)
                for (var y = -1; y <= 1; y++)
                for (var z = -1; z <= 1; z++) {
                    var pos = new UIntPosition(x,y,z);
                    var mapChunk = Map.GenerateChunk(pos);
                    Map.InstantiateChunk(mapChunk, pos);
                }

                // todo: same for this
                PlayerPosition.GlobalPosition = (0, 0, 0);

                onLoaded = null;
            };
        }

        /// <summary>
        /// Subscribes to <see cref="SceneManager.sceneLoaded"/> event
        /// </summary>
        protected override void Awake() {
            base.Awake();
            SceneManager.sceneLoaded += (scene, mode) => onLoaded?.Invoke();
        }

        /// <summary>
        /// Write game data to a file
        /// </summary>
        private static void WriteFile(BinaryWriter writer) {
            // todo: write file version
            // todo: maybe write chunk size as well and make it variable
            // todo: maybe CONVERT block positions from previous version into current

            var (px, py, pz) = PlayerPosition.GlobalPosition;
            writer.Write(px);
            writer.Write(py);
            writer.Write(pz);

            var (zx, zy) = Map.Zero;
            writer.Write(zx);
            writer.Write(zy);

            var storage = Map.Storage;
            var count = storage.Count;
            writer.Write(count);

            foreach (var kvp in storage) {
                writer.Write(kvp.Key); // chunk position index

                var chunk = kvp.Value;
                var blocksCount = chunk.Count;
                writer.Write(blocksCount);

                foreach (var blockPosition in chunk) {
                    writer.Write(blockPosition.Key); // block position index
                    writer.Write((int) blockPosition.Value); // block type
                }
            }
        }

        /// <summary>
        /// Read game data from file
        /// </summary>
        private static void ReadFile(BinaryReader reader,
            out MapStorage map, out (int x, int y) zero, out (int x, int y, int z) playerPosition) {
            var px = reader.ReadInt32();
            var py = reader.ReadInt32();
            var pz = reader.ReadInt32();

            playerPosition = (px, py, pz);

            var zx = reader.ReadInt32();
            var zy = reader.ReadInt32();

            zero = (zx, zy);

            var count = reader.ReadInt32();

            map = new MapStorage();

            for (var i = 0; i < count; i++) {
                var chunkIndex = reader.ReadInt64();
                var blocksCount = reader.ReadInt32();

                var chunk = map[chunkIndex] = new SortedDictionary<int, BlockType>();

                for (var j = 0; j < blocksCount; j++) {
                    var blockIndex = reader.ReadInt32();
                    var blockType = (BlockType) reader.ReadInt32();

                    chunk[blockIndex] = blockType;
                }
            }
        }
    }
}