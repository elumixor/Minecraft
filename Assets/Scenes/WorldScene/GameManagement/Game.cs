using System;
using Shared.SingletonBehaviour;
using UnityEngine;

namespace Scenes.WorldScene.GameManagement {
    public class Game : SingletonBehaviour<Game> {
        public static void Save(string filePath) => throw new NotImplementedException();
        public static void Load(string filePath) => throw new NotImplementedException();
        public static void Load(int id) => throw new NotImplementedException();
        public static void New(int seed) => throw new NotImplementedException();
    }
}