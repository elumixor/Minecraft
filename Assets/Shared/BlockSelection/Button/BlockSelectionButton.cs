using Shared.Blocks;
using UnityEngine;
using UnityEngine.UI;

namespace Shared.BlockSelection.Button {
    [RequireComponent(typeof(Image))]
    public class BlockSelectionButton : MonoBehaviour {
       [SerializeField] private BlockType blockType;
//       public void OnPointerClick(PointerEventData eventData) => BlockSelector.SelectedType = blockType;
    }
}