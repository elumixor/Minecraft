using Shared.GameManagement;
using Shared.MenuSystem.Container;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Shared.MenuSystem.MenuScripts {
    public class PauseMenu : MenuScreen {
        [SerializeField] private MenuButton saveButton;
        [SerializeField] private MenuButton loadButton;
        [SerializeField] private MenuButton exitMenuButton;
        [SerializeField] private MenuButton exitGameButton;
        [SerializeField] private MenuButton cancelButton;

        private void Start() {
            offset = 0f;

            var saves = Game.SavedGames;
            var hasSaves = saves.Count > 0;

            ButtonDisplayed(saveButton, offset, () => MenuContainer.ActiveMenu = MenuContainer.MenuType.SaveGame);

            if (hasSaves)
                ButtonDisplayed(loadButton, offset, () => MenuContainer.ActiveMenu = MenuContainer.MenuType.LoadGame);
            else ButtonHidden(loadButton);

            ButtonDisplayed(exitMenuButton, offset, () => {
                MenuContainer.ActiveMenu = MenuContainer.MenuType.Main;
                Game.ExitToMenu();
            });

            ButtonDisplayed(exitGameButton, offset, Game.ExitGame);

            ButtonDisplayed(cancelButton, offset, () => {
                Cursor.lockState = CursorLockMode.Locked;
                MenuContainer.ActiveMenu = null;
            });
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                MenuContainer.ActiveMenu = null;
            }
        }
    }
}