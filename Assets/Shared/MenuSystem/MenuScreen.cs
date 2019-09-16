using Shared.MenuSystem.Container;
using UnityEngine;
using UnityEngine.Events;

namespace Shared.MenuSystem {
    public abstract class MenuScreen : MonoBehaviour {
        protected float offset;

        protected static void ButtonDisplayed(MenuButton button, float off, UnityAction onClick) {
            button.Button.onClick.AddListener(onClick);
            var position = button.GetComponent<RectTransform>().position;
            position.y += off;
            button.GetComponent<RectTransform>().position = position;
        }

        protected void ButtonHidden(MenuButton button) {
            button.gameObject.SetActive(false);
            offset += button.GetComponent<RectTransform>().rect.height;
        }
    }
}