using UnityEngine;

namespace Bootlegger
{
    public class Player : SaveableBehaviour, IInteractor
    {
        [Header("Player Systems")]
        [SerializeField] private HeadRotator headRotator;
        [SerializeField] private MovementController movementController;
        [SerializeField] private PlayerInteractor interactor;
        [SerializeField] private Inventory inventory;

        private void Awake()
        {
            interactor.Setup(this);
        }

        protected override void OnUpdate()
        {
            CameraInput cameraInput = new()
            {
                LookInput = InputManager.GetLookAxis() * 0.13f
            };
            headRotator.UpdateInput(cameraInput);

            ControllerInput controllerInput = new()
            {
                MoveInput = InputManager.GetMoveAxis(),
                Crouch = InputManager.GetKey(InputType.Hold, ActionKey.Crouch),
                Jump = InputManager.GetKey(InputType.Hold, ActionKey.Jump),
                Run = InputManager.GetKey(InputType.Hold, ActionKey.Run)
            };
            movementController.UpdateInput(controllerInput);

            InteractorInput interactorInput = new()
            {
                Interact = InputManager.GetKey(InputType.Hold, ActionKey.Interact),
                Throw = InputManager.GetKey(InputType.Hold, ActionKey.Interact)
            };
            interactor.UpdateInput(interactorInput);

            //InventoryInput inventoryInput = new()
            //{
            //    Slot1 = InputManager.GetKeyDown(ActionKey.Slot1),
            //    Slot2 = InputManager.GetKeyDown(ActionKey.Slot2),
            //    Slot3 = InputManager.GetKeyDown(ActionKey.Slot3),
            //    Slot4 = InputManager.GetKeyDown(ActionKey.Slot4),
            //    Slot5 = InputManager.GetKeyDown(ActionKey.Slot5),
            //    Slot6 = InputManager.GetKeyDown(ActionKey.Slot6),
            //    Slot7 = InputManager.GetKeyDown(ActionKey.Slot7),
            //    Slot8 = InputManager.GetKeyDown(ActionKey.Slot8),
            //    Slot9 = InputManager.GetKeyDown(ActionKey.Slot9),
            //};
            //inventory.UpdateInput(inventoryInput);
        }
    }
}
