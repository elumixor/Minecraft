using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Shared.MenuSystem {
    public class MenuButton : MonoBehaviour {
        [SerializeField] private Button button;
        [SerializeField] private TMPro.TextMeshProUGUI text;

        [SerializeField] private string label;

        public UnityEvent OnClick => button.onClick;

        public string Text {
            get => text.text;
            set => text.text = value;
        }

        private void OnValidate() {
            text.text = label;
        }

        public Button Button => button;
    }
}