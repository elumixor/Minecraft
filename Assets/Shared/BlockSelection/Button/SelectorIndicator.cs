using Shared.Blocks;
using UnityEngine;

namespace Shared.BlockSelection.Button {
    public class SelectorIndicator : MonoBehaviour {
        private static readonly int SelectedHash = Animator.StringToHash("Selected");

        [SerializeField] private Animator animator;
        [SerializeField] private BlockType blockType;
        [SerializeField] private KeyCode hotkey;

        public KeyCode Hotkey => hotkey;

        private void Awake() => animator = GetComponent<Animator>();
        private void Reset() => Awake();

        public BlockType Select() {
            animator.SetBool(SelectedHash, true);
            return blockType;
        }

        public void Deselect() {
            animator.SetBool(SelectedHash, false);
        }
    }
}