using JetBrains.Annotations;
using Shared.SingletonBehaviour;
using UnityEngine;

namespace Shared.MenuSystem.Container {
    public class MenuContainer : SingletonBehaviour<MenuContainer> {
        public enum MenuType {
            Main,
            LoadGame,
            SaveGame,
            Pause
        }

        [SerializeField] private MenuScreen[] menuScreens;
        [SerializeField] private MenuButton menuButton;

        [CanBeNull, SerializeField] private MenuScreen current;

        public static MenuButton MenuButton => Instance.menuButton;
        public static MenuType? ActiveMenu {
            set {
                if (Instance.current != null) Instance.current.gameObject.SetActive(false);
                if (value != null) {
                    var newMenu = value.Value.ArrayValueIn(Instance.menuScreens);
                    if (Instance.current != newMenu) {
                        Instance.current = newMenu;
                        newMenu.gameObject.SetActive(true);
                    }
                } else Instance.current = null;
            }
        }

        public static bool MenuDisplayed => Instance.current != null;
    }
}