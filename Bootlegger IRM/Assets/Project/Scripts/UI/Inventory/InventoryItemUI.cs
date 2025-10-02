using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Bootlegger
{
    public class InventoryItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [field: SerializeField] public ItemSO Item { get; private set; }

        [SerializeField] private Image itemImage;
        [SerializeField] private TMP_Text itemCountText;
        private Transform _parentAfterDrag;
        private RectTransform _rect;

        private void Awake()
        {
            _rect = transform as RectTransform;

            UpdateGraphics();
        }

        public void InitializeItem(ItemSO item)
        {
            Item = item;
            UpdateGraphics();
        }

        public void SetParent(Transform transform)
        {
            _parentAfterDrag = transform;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            itemImage.raycastTarget = false;
            _parentAfterDrag = transform.parent;

            // Parent is the slot we in, but its parent is Grid
            transform.SetParent(transform.parent.parent);
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Mouse.current.position.ReadValue();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            itemImage.raycastTarget = true;
            transform.SetParent(_parentAfterDrag);

            _rect.anchoredPosition = Vector2.zero;

            UpdateGraphics();
        }

        private void UpdateGraphics()
        {
            if (Item == null)
            {
                itemImage.enabled = false;
                itemCountText.text = string.Empty;
                return;
            }

            itemImage.sprite = Item.Icon;
            itemImage.enabled = true;
            itemCountText.text = $"{(Item.CanStack ? 2 : 1)}";
        }
    }
}
