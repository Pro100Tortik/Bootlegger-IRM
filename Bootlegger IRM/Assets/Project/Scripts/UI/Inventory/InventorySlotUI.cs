using UnityEngine.EventSystems;
using UnityEngine;

namespace Bootlegger
{
    public class InventorySlotUI : MonoBehaviour, IDropHandler
    {
        private InventoryItemUI _currentItem;

        private void Awake()
        {
            _currentItem = GetComponentInChildren<InventoryItemUI>();
        }

        public void OnDrop(PointerEventData eventData)
        {
            // Swap or try stacking
            if (_currentItem.Item != null)
                return;

            InventoryItemUI item = eventData.pointerDrag.GetComponent<InventoryItemUI>();
            item.SetParent(transform);
        }
    }
}
