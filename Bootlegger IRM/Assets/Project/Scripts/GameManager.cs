using UnityEngine;

namespace Bootlegger
{
    public class GameManager : Singleton<GameManager>
    {
        public static bool EnableCursor
        {
            get => Cursor.visible;
            set
            {
                Cursor.visible = value;
                Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }

        [field: SerializeField] public GameMode CurrentGameMode { get; private set; } = GameMode.Exploration;

        public void ChangeGameMode(GameMode gameMode)
        {
            CurrentGameMode = gameMode;
        }
    }
}
