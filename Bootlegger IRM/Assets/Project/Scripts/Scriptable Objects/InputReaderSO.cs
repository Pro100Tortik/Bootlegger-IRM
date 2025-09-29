using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Bootlegger
{
    [CreateAssetMenu(fileName = "InputReaderSO", menuName = "Scriptable Objects/Create Input Reader")]
    public class InputReaderSO : ScriptableObject
    {
        [Header("Input Settings")]
        [SerializeField] private InputActionAsset inputAsset;

        [Header("Action Map Names")]
        [SerializeField] private string gameplayMapName = "Walk";
        [SerializeField] private string menuMapName = "Menu";

        private InputActionMap gameplayMap;
        private InputActionMap menuMap;

        private Dictionary<ActionKey, InputAction> _actionMap;
        private Dictionary<ActionKey, bool> toggleState;

        public void Initialize()
        {
            if (inputAsset == null) return;

            gameplayMap = inputAsset.FindActionMap(gameplayMapName, true);
            menuMap = inputAsset.FindActionMap(menuMapName, true);

            SwitchToGameplay();

            SetupActionMap();

            SceneManager.sceneLoaded += (scene, loadMode) => ResetToggles();

            toggleState = new Dictionary<ActionKey, bool>();
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
            gameplayMap?.Disable();
            menuMap?.Disable();

            SceneManager.sceneLoaded -= (scene, loadMode) => ResetToggles();
        }

        private void ResetToggles()
        {
            toggleState.Clear();
        }

        private void SetupActionMap()
        {
            _actionMap = new Dictionary<ActionKey, InputAction>
            {
                // Gameplay actions
                { ActionKey.Crouch, gameplayMap.FindAction("Crouch") },
                { ActionKey.Jump, gameplayMap.FindAction("Jump") },
                { ActionKey.Run, gameplayMap.FindAction("Run") },
                { ActionKey.Interact, gameplayMap.FindAction("Interact") },

                // Menu actions
                //{ ActionKey.Pause, menuMap.FindAction("Pause") },
                //{ ActionKey.MenuUp, menuMap.FindAction("MenuUp") },
                //{ ActionKey.MenuDown, menuMap.FindAction("MenuDown") },
                //{ ActionKey.MenuLeft, menuMap.FindAction("MenuLeft") },
                //{ ActionKey.MenuRight, menuMap.FindAction("MenuRight") },
                //{ ActionKey.MenuConfirm, menuMap.FindAction("MenuConfirm") },
                //{ ActionKey.MenuCancel, menuMap.FindAction("MenuCancel") },
                //{ ActionKey.MenuConsole, menuMap.FindAction("MenuConsole") },
                //{ ActionKey.MenuNext, menuMap.FindAction("MenuNext") }
            };
        }

        public void SwitchToGameplay()
        {
            //Utils.EnableCursor = false;

            menuMap.Disable();
            gameplayMap.Enable();
        }

        public void SwitchToMenu()
        {
            //Utils.EnableCursor = true;

            gameplayMap.Disable();
            menuMap.Enable();
        }

        public InputAction GetAction(ActionKey key)
        {
            if (_actionMap == null)
                SetupActionMap();

            return _actionMap.TryGetValue(key, out var action) ? action : null;
        }

        public bool GetKeyByType(InputType type, ActionKey key) => type switch
        {
            InputType.Hold => GetKeyHold(key),
            InputType.Toggle => GetKeyToggle(key),
            _ => false,
        };

        private bool GetKeyToggle(ActionKey key)
        {
            var action = GetAction(key);
            if (action == null) return false;

            // Переключаем состояние, если кнопка была нажата
            if (action.WasPressedThisFrame())
            {
                if (!toggleState.ContainsKey(key))
                    toggleState[key] = false;

                toggleState[key] = !toggleState[key];
            }

            return toggleState.TryGetValue(key, out var state) && state;
        }

        public bool GetKeyDown(ActionKey key) => GetAction(key)?.WasPressedThisFrame() ?? false;
        public bool GetKeyHold(ActionKey key) => GetAction(key)?.IsPressed() ?? false;
        public bool GetKeyUp(ActionKey key) => GetAction(key)?.WasReleasedThisFrame() ?? false;

        public Vector2 GetMoveAxis()
        {
            var action = gameplayMap.FindAction("MoveAxis");
            return Vector2.ClampMagnitude(action?.ReadValue<Vector2>() ?? Vector2.zero, 1f);
        }

        public Vector2 GetLookAxis()
        {
            var action = gameplayMap.FindAction("LookAxis");
            return action?.ReadValue<Vector2>() ?? Vector2.zero;
        }
    }
}
