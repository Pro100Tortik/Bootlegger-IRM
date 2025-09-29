using UnityEngine;

namespace Bootlegger
{
    public class Crosshair : PausableBehaviour
    {
        [SerializeField] private PlayerInteractor interactor;
        [SerializeField] private RectTransform crosshair;
        [SerializeField] private Transform head;
        [SerializeField] private bool showRealPosition = true;
        [SerializeField] private float selectionMultiplier = 1.6f;
        [SerializeField] private float speed = 3f;
        private bool _selectedAnything = false;
        private Vector2 _initialSize;

        private void Awake()
        {
            _initialSize = crosshair.sizeDelta;
        }

        private void OnEnable()
        {
            interactor.SelectedInteractable.ValueChanged += UpdateCrosshairSize;
        }

        private void OnDisable()
        {
            interactor.SelectedInteractable.ValueChanged -= UpdateCrosshairSize;
        }

        private void UpdateCrosshairSize()
        {
            if (interactor.SelectedInteractable.Value == null)
            {
                // Reset crosshair
                _selectedAnything = false;
                return;
            }

            if (!interactor.SelectedInteractable.Value.CanInteract)
                return;

            _selectedAnything = true;
        }

        protected override void OnUpdate()
        {
            if (showRealPosition)
            {
                crosshair.position = Camera.main.WorldToScreenPoint(head.position + head.forward);
            }
            else
            {
                crosshair.anchoredPosition = Vector2.zero;
            }
        }

        protected override void OnFixedUpdate()
        {
            if (!_selectedAnything)
            {
                crosshair.sizeDelta = Vector2.Lerp(crosshair.sizeDelta, _initialSize, Time.fixedDeltaTime * speed);
            }
            else
            {
                crosshair.sizeDelta = Vector2.Lerp(crosshair.sizeDelta, _initialSize * selectionMultiplier, Time.fixedDeltaTime * speed);
            }
        }
    }
}
