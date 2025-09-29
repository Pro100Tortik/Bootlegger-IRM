using UnityEngine;

namespace Bootlegger
{
    public class PlayerInteractor : MonoBehaviour
    {
        public ReactiveProperty<IInteractable> SelectedInteractable { get; private set; } = new();

        [Header("References")]
        [SerializeField] private Transform playerCamera;

        [Header("Interactor Settings")]
        [SerializeField] private float interactionRange = 3.0f;
        [SerializeField] private float interactionRadius = 0.2f;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private LayerMask blockingLayer;

        [Header("Grab Settings")]
        [SerializeField] private float carryRange = 2.5f;
        [SerializeField] private float maxCarryRange = 4.0f;
        [SerializeField] private float throwForce = 7f;

        private bool _requestedInteraction = false;
        private bool _requestedThrow = false;
        private bool _alreadyInteracted = false;
        private bool _alreadyThrowed = false;
        private IInteractor _owner;
        private IGrabbable _grabbable;

        public void Setup(IInteractor interactor)
        {
            _owner = interactor;
        }

        public void UpdateInput(InteractorInput input)
        {
            _requestedInteraction = input.Interact;
            _requestedThrow = input.Throw;
        }

        private void FixedUpdate()
        {
            if (_alreadyInteracted && !_requestedInteraction)
                _alreadyInteracted = false;

            if (_alreadyThrowed && !_requestedThrow)
                _alreadyThrowed = false;

            if (_grabbable != null)
            {
                UpdateGrabbable();
                return;
            }

            DetectInteractables();
            TryInteraction();
        }

        private void DetectInteractables()
        {
            bool directHit = false;

            // Try to find interactions directly
            if (!Physics.Raycast(playerCamera.position, playerCamera.forward, out var hit, interactionRange, interactableLayer | blockingLayer, QueryTriggerInteraction.Ignore))
            {
                // Try to find interaction in small radius
                if (!Physics.SphereCast(playerCamera.position, interactionRadius, playerCamera.forward, out hit, interactionRange, interactableLayer, QueryTriggerInteraction.Ignore))
                {
                    // Nothing found
                    DeselectInteraction();
                    return;
                }
            }
            else
            {
                directHit = IsInteractable(hit, out _);
                if (!directHit)
                {
                    if (!Physics.SphereCast(playerCamera.position, interactionRadius, playerCamera.forward, out hit, interactionRange, interactableLayer, QueryTriggerInteraction.Ignore))
                    {
                        // Nothing found
                        DeselectInteraction();
                        return;
                    }
                }
            }

            if (!directHit)
            {
                if (Physics.Linecast(playerCamera.transform.position, hit.point, blockingLayer, QueryTriggerInteraction.Ignore))
                {
                    DeselectInteraction();
                    return;
                }
            }

            IsInteractable(hit, out var interactable);

            SelectInteractable(interactable);
        }

        private bool IsInteractable(RaycastHit hit, out IInteractable interactable) => hit.collider.TryGetComponent(out interactable);

        private void SelectInteractable(IInteractable interactable)
        {
            if (interactable == null)
            {
                DeselectInteraction();
                return;
            }

            if (SelectedInteractable.Value != null)
            {
                if (SelectedInteractable.Value == interactable)
                    return;

                DeselectInteraction();
            }

            SelectedInteractable.Value = interactable;

            SelectedInteractable.Value.Select();
        }

        private void DeselectInteraction()
        {
            if (SelectedInteractable.Value == null)
                return;

            SelectedInteractable.Value.Deselect();
            SelectedInteractable.Value = null;
        }

        private void UpdateGrabbable()
        {
            // Out of reach, drop
            if (Vector3.Distance(_grabbable.Transform.position, playerCamera.position) > maxCarryRange)
            {
                _grabbable.Throw(Vector3.zero);
                _grabbable = null;
                return;
            }

            _grabbable.UpdatePosition(playerCamera.position + playerCamera.forward * carryRange);
            TryThrowing();
        }

        private void TryThrowing()
        {
            if (_grabbable == null)
                return;

            if (!_requestedThrow || _alreadyInteracted)
                return;

            _alreadyThrowed = true;

            _grabbable.Throw(playerCamera.forward * throwForce);
            _grabbable.Select();
            _grabbable = null;
        }

        private void TryInteraction()
        {
            if (!_requestedInteraction || _alreadyInteracted || _alreadyThrowed)
                return;

            if (SelectedInteractable.Value == null)
                return;

            if (!SelectedInteractable.Value.CanInteract)
                return;

            _alreadyInteracted = true;

            if (SelectedInteractable.Value is IGrabbable grabbable)
            {
                _grabbable = grabbable;
                _grabbable.Grab();
                return;
            }

            SelectedInteractable.Value.Interact(_owner);
        }
    }
}
