using Shared.Blocks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scenes.WorldScene.BlockSelection.Button {
    [RequireComponent(typeof(Image))]
    public class BlockSelectionButton : MonoBehaviour {
       [SerializeField] private BlockType blockType;
//       public void OnPointerClick(PointerEventData eventData) => BlockSelector.SelectedType = blockType;
    }
}