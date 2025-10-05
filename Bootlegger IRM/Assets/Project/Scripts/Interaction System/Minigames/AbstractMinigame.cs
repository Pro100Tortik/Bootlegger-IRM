using Unity.Cinemachine;
using UnityEngine;

namespace Bootlegger
{
    /// <summary>
    /// Used for cooking mini-games and some other 'immersive' moments, like picking locks
    /// </summary>
    public abstract class AbstractMinigame : InteractionBase
    {
        [SerializeField] protected CinemachineCamera minigameCamera;
        [SerializeField] protected Transform minigamePosition;
        private bool _playing = false;
        private IInteractor _interactor;

        private void Awake()
        {
            minigameCamera.enabled = false;
        }

        protected override void OnInteract(IInteractor interactor)
        {
            _interactor = interactor;

            _interactor.MinigameStarted(minigamePosition.position, minigameCamera.transform.rotation);

            StartMinigame();
        }

        protected virtual void StartMinigame()
        {
            minigameCamera.enabled = true;

            // Update camera
            minigameCamera.Priority = 2;
            minigameCamera.Prioritize();

            GameManager.Instance.ChangeGameMode(GameMode.Minigame);

            _playing = true;
            // Spawn items if needed or something
        }

        protected override void OnUpdate()
        {
            if (!_playing)
                return;

            if (InputManager.GetKeyDown(ActionKey.Cancel))
                QuitMinigame();
        }

        public virtual void FinishMinigame()
        {
            QuitMinigame();
        }

        public void QuitMinigame()
        {
            _playing = false;

            minigameCamera.enabled = false;

            _interactor.MinigameStopped(minigamePosition.position, minigameCamera.transform.rotation);

            GameManager.Instance.ChangeGameMode(GameMode.Exploration);
        }
    }
}
