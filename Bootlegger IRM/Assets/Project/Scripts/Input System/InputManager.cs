using UnityEngine;

namespace Bootlegger
{
    public static class InputManager
    {
        private const string INPUT_READER_PATH = "Game Resources/InputReaderSO";

        private static InputReaderSO InputReader
        {
            get
            {
                if (_inputReader == null)
                {
                    _inputReader = Resources.Load<InputReaderSO>(INPUT_READER_PATH);
                    _inputReader.Initialize();

                    Debug.Log(_inputReader == null ? $"Failed loading input reader." : "Loaded input reader.");
                }

                return _inputReader;
            }
        }
        private static InputReaderSO _inputReader;

        public static bool GetKeyDown(ActionKey actionKey) => InputReader.GetKeyDown(actionKey);
        public static bool GetKeyUp(ActionKey actionKey) => InputReader.GetKeyUp(actionKey);
        public static bool GetKey(InputType inputType, ActionKey actionKey) => InputReader.GetKeyByType(inputType, actionKey);
        public static Vector2 GetLookAxis() => InputReader.GetLookAxis();
        public static Vector2 GetMoveAxis() => InputReader.GetMoveAxis();
    }
}
