using UnityEngine.InputSystem;
using UnityEngine;

namespace Bootlegger
{
    public class DragManager : MonoBehaviour
    {
        public ReactiveProperty<IDraggable> SelectedDraggable { get; private set; } = new();

        [SerializeField] private Camera playerCamera;
        [SerializeField] private LayerMask draggableLayer;
        [SerializeField] private float dragRange = 3f;
        [SerializeField] private float radius = 0.3f;
        private bool _requestedDrag = false;
        private bool _alreadyDragging = false;

        public void Setup(IInteractor interactor)
        {
            // Setup code if needed
        }

        public void UpdateInput(bool drag)
        {
            _requestedDrag = drag;
        }

        private void FixedUpdate()
        {
            // Обработка отпускания объекта
            if (!_requestedDrag && _alreadyDragging)
            {
                DeselectDraggable();
                _alreadyDragging = false;
                return;
            }

            // Если нет выбранного draggable, пытаемся найти
            if (SelectedDraggable.Value == null)
            {
                DetectDraggables();
            }

            // Обработка начала перетаскивания
            if (_requestedDrag && !_alreadyDragging && SelectedDraggable.Value != null)
            {
                TryStartDragging();
                return;
            }

            // Обновление позиции перетаскиваемого объекта
            if (_alreadyDragging && SelectedDraggable.Value != null)
            {
                UpdateDraggable();
                return;
            }
        }

        private void UpdateDraggable()
        {
            SelectedDraggable.Value.UpdateDrag(Mouse.current.position.ReadValue());
        }

        private void DetectDraggables()
        {
            Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            // Try to find interactions directly
            if (!Physics.Raycast(ray, out var hit, dragRange, draggableLayer, QueryTriggerInteraction.Ignore))
            {
                // Try to find interaction in small radius
                if (!Physics.SphereCast(ray.origin, radius, ray.direction, out hit, dragRange, draggableLayer, QueryTriggerInteraction.Ignore))
                {
                    // Nothing found
                    return;
                }
            }

            SelectDraggable(hit.collider.GetComponent<IDraggable>());
        }

        private void TryStartDragging()
        {
            if (!_requestedDrag || _alreadyDragging || SelectedDraggable.Value == null)
                return;

            _alreadyDragging = true;

            Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            SelectedDraggable.Value.StartDrag(ray.origin + ray.direction * dragRange);
        }

        private void SelectDraggable(IDraggable draggable)
        {
            if (draggable == null)
                return;

            // Если уже есть выбранный объект, снимаем выделение
            if (SelectedDraggable.Value != null)
            {
                if (SelectedDraggable.Value == draggable)
                    return;

                DeselectDraggable();
            }

            SelectedDraggable.Value = draggable;
            SelectedDraggable.Value.Select();
        }

        private void DeselectDraggable()
        {
            if (SelectedDraggable.Value == null)
                return;

            // Если объект перетаскивался, останавливаем перетаскивание
            if (_alreadyDragging)
            {
                SelectedDraggable.Value.StopDrag(Mouse.current.position.ReadValue());
            }

            SelectedDraggable.Value.Deselect();
            SelectedDraggable.Value = null;
        }
    }
}
