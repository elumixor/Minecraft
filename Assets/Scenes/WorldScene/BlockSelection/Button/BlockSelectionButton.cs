using Scenes.WorldScene.Block;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scenes.WorldScene.BlockSelection.Button {
    [RequireComponent(typeof(Image))]
    public class BlockSelectionButton : MonoBehaviour, IPointerClickHandler {
       [SerializeField] private BlockType blockType;
       public BlockSelector selector;

       public void OnPointerClick(PointerEventData eventData) => selector.selectedType = blockType;
    }
}