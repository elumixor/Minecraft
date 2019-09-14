using System;
using Shared;
using Shared.GameManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes.MainMenu {
    public class MenuButtonsHandle : MonoBehaviour {
        private void Start() {
            Game.New();
        }

        public void NewGame() => Game.New();
        public void Load() => throw new NotImplementedException();
    }
}