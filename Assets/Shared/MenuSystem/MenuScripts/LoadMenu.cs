using System;
using System.IO;
using Shared.GameManagement;
using Shared.MenuSystem.Container;
using UnityEngine;

namespace Shared.MenuSystem.MenuScripts {
    public class LoadMenu : MenuScreen {
        [SerializeField] private MenuButton cancelButton;
        [SerializeField] private RectTransform content;
        [SerializeField] private RectTransform scrollView;

        [SerializeField] private MenuButton optionButton;
        [SerializeField, Range(10, 1000)] private float maxHeight;

        private void OnEnable() {
            content.DestroyAllChildren();

            offset = 0f;

            var rect = optionButton.GetComponent<RectTransform>().rect;

            var itemHeight = rect.height;
            var panelHeight = Game.SavedGames.Count * itemHeight;
            var panelWidth = rect.width;

            var listOffset = 0f;
            foreach (var save in Game.SavedGames) {
                var button = Instantiate(optionButton, content);
                button.Text = Path.GetExtension(save.name) == ".tmp" ? "Last Saved Game" : save.name;
                ButtonDisplayed(button, listOffset, () => {
                    MenuContainer.ActiveMenu = null;
                    Game.Load(save);
                });
                listOffset -= itemHeight;
            }

            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelHeight);
            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelWidth);

            scrollView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Min(panelHeight, maxHeight));
        }

        protected void Start() {
            offset = 0f;
            var rect = optionButton.GetComponent<RectTransform>().rect;

            var itemHeight = rect.height;
            var panelHeight = Game.SavedGames.Count * itemHeight;
            var panelWidth = rect.width;

            var listOffset = 0f;
            foreach (var save in Game.SavedGames) {
                var button = Instantiate(optionButton, content);
                button.Text = Path.GetExtension(save.name) == ".tmp" ? "Last Saved Game" : save.name;
                ButtonDisplayed(button, listOffset, () => {
                    MenuContainer.ActiveMenu = null;
                    Game.Load(save);
                });
                listOffset -= itemHeight;
            }

            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelHeight);
            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelWidth);

            scrollView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Min(panelHeight, maxHeight));

            ButtonDisplayed(cancelButton, offset, () => MenuContainer.ActiveMenu = Game.InGame
                ? MenuContainer.MenuType.Pause
                : MenuContainer.MenuType.Main);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape))
                MenuContainer.ActiveMenu = Game.InGame ? MenuContainer.MenuType.Pause : MenuContainer.MenuType.Main;
        }
    }
}