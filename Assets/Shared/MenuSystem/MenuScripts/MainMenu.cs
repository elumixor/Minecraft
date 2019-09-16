using Shared.GameManagement;
using Shared.MenuSystem.Container;
using UnityEngine;

namespace Shared.MenuSystem.MenuScripts {
    public class MainMenu : MenuScreen {
        [SerializeField] private MenuButton continueButton;
        [SerializeField] private MenuButton newGameButton;
        [SerializeField] private MenuButton loadButton;
        [SerializeField] private MenuButton exitButton;

        protected void Start() {
            offset = 0f;

            // Create menu buttons for options
            var saves = Game.SavedGames;
            var hasSaves = saves.Count > 0;

            if (hasSaves) ButtonDisplayed(continueButton, 0f, () => {
                MenuContainer.ActiveMenu = null;
                Game.Load(saves[0]);
            });
            else ButtonHidden(continueButton);

            ButtonDisplayed(newGameButton, offset, () => {
                MenuContainer.ActiveMenu=  null;
                Game.New();
            });

            if (hasSaves)
                ButtonDisplayed(loadButton, offset,
                    () => MenuContainer.ActiveMenu =MenuContainer.MenuType.LoadGame);
            else ButtonHidden(loadButton);

            ButtonDisplayed(exitButton, offset, Game.ExitGame);
        }
    }
}