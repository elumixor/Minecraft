using System;
using System.IO;
using JetBrains.Annotations;
using Shared.Blocks;
using Shared.Positioning;
using Shared.SingletonBehaviour;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shared.GameManagement {
    public class Game : SingletonBehaviour<Game> {
        [SerializeField] private SceneField mainMenuScene;
        [SerializeField] private SceneField worldScene;
        [SerializeField] private int seed;

        /// <summary>
        /// Action, to  be executed after scene has loaded
        /// </summary>
        [CanBeNull] private static Action onLoaded;

        /// <summary>
        /// Saves game state to a binary file
        /// </summary>
        /// <param name="fileName">Name of saved data file</param>
        public static void Save(string fileName) {
            // todo: maybe write file version
            // todo: maybe write chunk size as well and make it variable
            // todo: maybe CONVERT block positions from previous version into current

            // todo: warn if file exists -> replace/cancel

            using (var writer = new BinaryWriter(File.Open(fileName, FileMode.Create))) {
                var ((cx, cy, cz), (px, py, pz)) = Player.Position;

                writer.Write(cx);
                writer.Write(cy);
                writer.Write(cz);

                writer.Write(px);
                writer.Write(py);
                writer.Write(pz);

                var (zx, zy) = Map.zero;

                writer.Write(zx);
                writer.Write(zy);

                var storage = Map.storage;
                var count = storage.Count;

                writer.Write(count);

                foreach (var (index, chunk) in storage) {
                    writer.Write(index);
                    writer.Write(chunk.Count);

                    foreach (var (blockIndex, blockType) in chunk) {
                        writer.Write(blockIndex);
                        writer.Write((int) blockType);
                    }
                }
            }
        }
        /// <summary>
        /// Loads game state from file, creates map, blocks and places player at saved position
        /// </summary>
        /// <param name="fileName">Path to saved data file</param>
        /// <exception cref="FileLoadException">If could not correctly load save file</exception>
        public static void Load(string fileName) {
            using (var reader = new BinaryReader(File.Open(fileName, FileMode.Create))) {
                var cx = reader.ReadInt32();
                var cy = reader.ReadInt32();
                var cz = reader.ReadInt32();

                var px = reader.ReadUInt32();
                var py = reader.ReadUInt32();
                var pz = reader.ReadUInt32();

                Player.Position = new WorldPosition(
                    new WorldPosition.ChunkPosition(cx, cy, cz),
                    new WorldPosition.LocalPosition(px, py, pz));

                var zx = reader.ReadInt32();
                var zy = reader.ReadInt32();

                Map.zero = (zx, zy);

                var storageCount = reader.ReadInt32();
                var storage = new MapStorage<BlockType>();

                for (var i = 0; i < storageCount; i++) {
                    var chunkIndex = reader.ReadUInt64();
                    var blocksCount = reader.ReadInt32();

                    var chunk = storage[new WorldPosition.ChunkPosition(chunkIndex)] =
                        new MapStorage<BlockType>.Chunk();

                    for (var j = 0; j < blocksCount; j++) {
                        var blockIndex = reader.ReadUInt32();
                        var blockType = (BlockType) reader.ReadInt32();

                        chunk[new WorldPosition.LocalPosition(blockIndex)] = blockType;
                    }
                }
            }
        }
        /// <summary>
        /// Begins new game
        /// </summary>
        public static void New() {
            SceneManager.LoadScene(Instance.worldScene);
            onLoaded = () => {
                Map.zero = (Instance.seed, Instance.seed);

                for (var x = -1; x <= 1; x++)
                for (var y = 0; y <= 2; y++)
                for (var z = -1; z <= 1; z++) {
                    var chunkPosition = new WorldPosition.ChunkPosition(x, y, z);
                    Block.InstantiateChunk(Map.GetChunk(chunkPosition), chunkPosition);
                }

                for (var chunkY = 1;; chunkY++) {
                    var chunkPosition = new WorldPosition.ChunkPosition(0, chunkY, 0);
                    var chunk = Map.GetChunk(chunkPosition);

                    for (var z = 0u; z < WorldPosition.ChunkSize; z++)
                    for (var x = 0u; x < WorldPosition.ChunkSize; x++)
                    for (var y = 0u; y < WorldPosition.ChunkSize; y++) {
                        var localPosition = new WorldPosition.LocalPosition(x, y, z);
                        if (!chunk.ContainsKey(localPosition)) {
                            Player.Position = new WorldPosition(chunkPosition, localPosition);
                            onLoaded = null;
                            return;
                        }
                    }
                }
            };
        }

        #region Private helper methods
        /// <summary>
        /// Subscribes to <see cref="SceneManager.sceneLoaded"/> event
        /// </summary>
        protected override void Awake() {
            base.Awake();
            SceneManager.sceneLoaded += (scene, mode) => onLoaded?.Invoke();
        }
        #endregion
    }
}