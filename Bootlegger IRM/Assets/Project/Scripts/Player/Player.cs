using Unity.Cinemachine;
using UnityEngine;

namespace Bootlegger
{
    public class Player : SaveableBehaviour, IInteractor
    {
        [Header("Player Systems (Exploration)")]
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private CinemachineCamera playerCamera;
        [SerializeField] private MovementController movementController;
        [SerializeField] private HeadRotator headRotator;
        [SerializeField] private PlayerInteractor interactor;
        [SerializeField] private Inventory inventory;

        [Header("Player Systems (Minigames)")]
        [SerializeField] private DragManager dragManager;
        private GameManager _gameManager;

        private void Awake()
        {
            interactor.Setup(this);
            dragManager.Setup(this);

            GameManager.EnableCursor = false;
        }

        protected override void Start()
        {
            base.Start();
            _gameManager = GameManager.Instance;
        }

        protected override void OnUpdate()
        {
            switch (_gameManager.CurrentGameMode)
            {
                case GameMode.Exploration:
                    UpdateExplorationSystems(true);
                    break;

                case GameMode.Minigame:
                    UpdateMinigameSystems();
                    UpdateExplorationSystems(false);
                    break;

                case GameMode.Trading:
                    break;
            }
        }

        // Movement, interactions
        private void UpdateExplorationSystems(bool enabled)
        {
            CameraInput cameraInput = new()
            {
                LookInput = InputManager.GetLookAxis() * 0.13f
            };
            headRotator.UpdateInput(enabled ? cameraInput : default);

            ControllerInput controllerInput = new()
            {
                MoveInput = InputManager.GetMoveAxis(),
                Crouch = InputManager.GetKey(InputType.Hold, ActionKey.Crouch),
                Jump = InputManager.GetKey(InputType.Hold, ActionKey.Jump),
                Run = InputManager.GetKey(InputType.Hold, ActionKey.Run)
            };
            movementController.UpdateInput(enabled ? controllerInput : default);

            InteractorInput interactorInput = new()
            {
                Interact = InputManager.GetKey(InputType.Hold, ActionKey.Interact),
                Throw = InputManager.GetKey(InputType.Hold, ActionKey.PrimaryAttack)
            };
            interactor.UpdateInput(enabled ? interactorInput : default);

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

        // Some interactions
        private void UpdateMinigameSystems()
        {
            dragManager.UpdateInput(InputManager.GetKey(InputType.Hold, ActionKey.PrimaryAttack));
        }

        public void MinigameStarted(Vector3 position, Quaternion rotation)
        {
            GameManager.EnableCursor = true;

            //playerCamera.enabled = false;
        }

        public void MinigameStopped(Vector3 position, Quaternion rotation)
        {
            GameManager.EnableCursor = false;

            playerCamera.enabled = true;

            movementController.SetPosition(position, true);
            headRotator.SetLookDirection(rotation.eulerAngles);
        }
    }
}
