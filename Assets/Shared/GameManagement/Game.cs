using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        // todo: custom property drawer for directory? if this is possible
        [SerializeField] private string savedGamesFolder;
        [SerializeField] private string savedGamesExtension;

        public static (string name, string filePath) CurrentSave { get; private set; }

        private static (string name, string filePath) Temporary => ("_.tmp", Path.Combine(
            Application.persistentDataPath,
            Instance.savedGamesFolder, $"_.tmp.{Instance.savedGamesExtension}"));

        public static void SetCurrentSaveName(string saveName) {
            CurrentSave = (saveName, Path.Combine(Application.persistentDataPath,
                Instance.savedGamesFolder, $"{saveName}.{Instance.savedGamesExtension}"));
        }

        private static List<(string name, string filePath)> savedGames;
        public static List<(string name, string filePath)> SavedGames {
            get {
                if (savedGames != null) return savedGames;

                var combined = Path.Combine(Application.persistentDataPath, Instance.savedGamesFolder);
                if (!Directory.Exists(combined)) {
                    Directory.CreateDirectory(combined);
                    return savedGames = new List<(string name, string filePath)>();
                }


                return savedGames =
                    new DirectoryInfo(combined).GetFiles($"*.{Instance.savedGamesExtension}")
                        .OrderByDescending(file => file.LastWriteTime)
                        .Select(file => (Path.GetFileNameWithoutExtension(file.Name), file.FullName))
                        .ToList();
            }
        }

        public static bool InGame => SceneManager.GetActiveScene().name == Instance.worldScene.SceneName;

        /// <summary>
        /// Action, to  be executed after scene has loaded
        /// </summary>
        [CanBeNull] private static Action onLoaded;

        /// <summary>
        /// Saves game state to a binary file
        /// </summary>
        public static void Save() {
            var fileName = CurrentSave.filePath;
            var dir = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            if (CurrentSave.name != Temporary.name && File.Exists(Temporary.filePath)) {
                savedGames.Remove(CurrentSave);
                File.Delete(Temporary.filePath);
            }

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
                        writer.Write((int) blockType.blockType);
                        writer.Write(blockType.adjoiningBlocks);
                    }
                }
            }

            if (CurrentSave.name != Temporary.name && File.Exists(Temporary.filePath) &&
                !savedGames.Contains(CurrentSave)) savedGames.Add(CurrentSave);
        }
        /// <summary>
        /// Loads game state from file, creates map, blocks and places player at saved position
        /// </summary>
        /// <param name="saveFile">Name and path to saved data file</param>
        /// <exception cref="FileLoadException">If could not correctly load save file</exception>
        public static void Load((string name, string path) saveFile) {
            if (InGame) {
                Save();
                Cleanup();
            }
            CurrentSave = saveFile;

            SceneManager.LoadScene(Instance.worldScene);

            onLoaded = () => {
                Cursor.visible = false;
                using (var reader = new BinaryReader(File.Open(saveFile.path, FileMode.Open))) {
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
                    var storage = new MapStorage<Map.BlockInfo>();

                    for (var i = 0; i < storageCount; i++) {
                        var chunkIndex = reader.ReadUInt64();
                        var blocksCount = reader.ReadInt32();

                        var chunk = storage[new WorldPosition.ChunkPosition(chunkIndex)] =
                            new MapStorage<Map.BlockInfo>.Chunk();

                        for (var j = 0; j < blocksCount; j++) {
                            var blockIndex = reader.ReadUInt32();
                            var blockType = (BlockType) reader.ReadInt32();
                            var adjoiningBlocks = reader.ReadByte();

                            chunk[new WorldPosition.LocalPosition(blockIndex)] =
                                new Map.BlockInfo(blockType, adjoiningBlocks);
                        }
                    }

                    Map.storage = storage;

                    for (var x = -1; x <= 1; x++)
                    for (var y = -1; y <= 1; y++)
                    for (var z = -1; z <= 1; z++) {
                        var chunkPosition = new WorldPosition.ChunkPosition(x, y, z) + Player.Position.chunkPosition;
                        if (chunkPosition.y >= 0) Block.InstantiateChunk(Map.GetChunk(chunkPosition), chunkPosition);
                    }
                }

                onLoaded = null;
            };
        }
        /// <summary>
        /// Begins new game
        /// </summary>
        public static void New() {
            SceneManager.LoadScene(Instance.worldScene);

            CurrentSave = Temporary;

            onLoaded = () => {
                Cursor.visible = false;
                Map.storage = new MapStorage<Map.BlockInfo>();
                Map.zero = (Instance.seed, Instance.seed);

                for (var x = -1; x <= 1; x++)
                for (var y = 0; y <= 2; y++)
                for (var z = -1; z <= 1; z++) {
                    var chunkPosition = new WorldPosition.ChunkPosition(x, y, z);
                    if (chunkPosition.y >= 0) Block.InstantiateChunk(Map.GetChunk(chunkPosition), chunkPosition);
                }

                for (var chunkY = 1;; chunkY++) {
                    var chunkPosition = new WorldPosition.ChunkPosition(0, chunkY, 0);
                    var chunk = Map.GetChunk(chunkPosition);

                    for (var z = WorldPosition.ChunkSize / 2; z < WorldPosition.ChunkSize; z++)
                    for (var x = WorldPosition.ChunkSize / 2; x < WorldPosition.ChunkSize; x++)
                    for (var y = 0u; y < WorldPosition.ChunkSize; y++) {
                        var localPosition = new WorldPosition.LocalPosition(x, y, z);
                        if (!chunk.ContainsKey(localPosition)) {
                            Player.Position = new WorldPosition(chunkPosition, localPosition);
                            Save();
                            onLoaded = null;
                            return;
                        }
                    }
                }
            };
        }
        /// <summary>
        /// Exits game
        /// </summary>
        public static void ExitGame() {
            Save();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }
        /// <summary>
        /// Exits to main menu
        /// </summary>
        public static void ExitToMenu() {
            Save();
            Cleanup();
            Cursor.visible = true;
            SceneManager.LoadScene(Instance.mainMenuScene);
        }

        #region Private helper methods
        /// <summary>
        /// Cleanup all blocks
        /// </summary>
        private static void Cleanup() {
            var pos = Player.Position.chunkPosition;
            for (var z = -1; z <= 1; z++)
            for (var y = -1; y <= 1; y++)
            for (var x = -1; x <= 1; x++)
                if (pos.y + y >= 0)
                    Block.DestroyChunk(pos + new WorldPosition.ChunkPosition(x, y, z));
        }
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