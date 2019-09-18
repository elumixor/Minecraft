using System.IO;
using System.Linq;
using Shared.GameManagement;
using Shared.MenuSystem.Container;
using UnityEngine;
using UnityEngine.UI;

namespace Shared.MenuSystem.MenuScripts {
    public class SaveMenu : MenuScreen {
        [SerializeField] private MenuButton cancelButton;
        [SerializeField] private MenuButton saveButton;

        [SerializeField] private RectTransform content;
        [SerializeField] private RectTransform scrollView;

        [SerializeField] private MenuButton optionButton;
        [SerializeField, Range(10, 1000)] private float maxHeight;

        [SerializeField] private Text placeholder;
        [SerializeField] private InputField input;


        private void OnEnable() {
            content.DestroyAllChildren();

            var rect = optionButton.GetComponent<RectTransform>().rect;

            var itemHeight = rect.height;
            var panelHeight = (Game.SavedGames.Count - 1) * itemHeight;
            var panelWidth = rect.width;

            var listOffset = 0f;
            var saves = Game.SavedGames.Where(e => Path.GetExtension(e.name) != ".tmp").ToList();
            foreach (var (saveName, _) in saves) {
                var button = Instantiate(optionButton, content);
                button.Text = saveName;
                ButtonDisplayed(button, listOffset, () => input.text = saveName);
                listOffset -= itemHeight;
            }

            Debug.Assert(Game.CurrentSave != default, "No current save when opening save menu");

            placeholder.text = Path.GetExtension(Game.CurrentSave.name) == ".tmp"
                ? "Last Saved game"
                : Game.CurrentSave.name;

            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelHeight);
            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelWidth);

            scrollView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Min(panelHeight, maxHeight));
        }

        protected void Start() {
            offset = 0f;

            var rect = optionButton.GetComponent<RectTransform>().rect;

            var itemHeight = rect.height;
            var panelHeight = (Game.SavedGames.Count - 1) * itemHeight;
            var panelWidth = rect.width;

            var listOffset = 0f;
            var saves = Game.SavedGames.Where(e => Path.GetExtension(e.name) != ".tmp").ToList();
            foreach (var (saveName, _) in saves) {
                var button = Instantiate(optionButton, content);
                button.Text = saveName;
                ButtonDisplayed(button, listOffset, () => input.text = saveName);
                listOffset -= itemHeight;
            }

            placeholder.text = Path.GetExtension(Game.CurrentSave.name) == ".tmp"
                ? "Last Saved game"
                : Game.CurrentSave.name;

            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, panelHeight);
            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelWidth);

            scrollView.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Min(panelHeight, maxHeight));

            ButtonDisplayed(saveButton, offset, Save);
            ButtonDisplayed(cancelButton, offset, () => MenuContainer.ActiveMenu = MenuContainer.MenuType.Pause);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape))
                MenuContainer.ActiveMenu = MenuContainer.MenuType.Pause;
            else if (Input.GetKeyDown(KeyCode.Return))
                Save();
        }

        private void Save() {
            if (!string.IsNullOrEmpty(input.text)) Game.SetCurrentSaveName(input.text);
            Game.Save();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            MenuContainer.ActiveMenu = null;
        }
    }
}