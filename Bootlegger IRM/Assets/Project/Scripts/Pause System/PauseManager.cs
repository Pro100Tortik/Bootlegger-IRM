using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

namespace Bootlegger
{
    public class PauseManager : Singleton<PauseManager>
    {
        public bool IsPaused { get; private set; } = false;

        private readonly List<IPausable> _pausables = new();
        private Priority _currentPriority;
        private float _prepauseTimeScale = 1.0f;

        protected override void Awake()
        {
            base.Awake();

            SceneLoader.SceneLoaded += () => ResumeGame(new PauseRequest { IsSceneChanged = true });
        }

        private void OnDestroy()
        {
            SceneLoader.SceneLoaded -= () => ResumeGame(new PauseRequest { IsSceneChanged = true });
        }

        public void Subscribe(IPausable pausable)
        {
            if (!_pausables.Contains(pausable))
            {
                _pausables?.Add(pausable);
            }
        }

        public void Unsubscribe(IPausable pausable)
        {
            if (_pausables.Contains(pausable))
            {
                _pausables?.Remove(pausable);
            }
        }

        public void PauseGame(in PauseRequest request)
        {
            if (IsPaused && request.Priority < _currentPriority)
                return;

            _currentPriority = request.Priority;

            IsPaused = true;

            _prepauseTimeScale = Time.timeScale;
            Time.timeScale = 0f;

            foreach (var pausable in _pausables)
            {
                pausable.Pause(request);
            }
        }

        public void ResumeGame(in PauseRequest request)
        {
            if (!IsPaused)
                return;

            if (request.Priority < _currentPriority)
                return;

            _currentPriority = Priority.None;

            IsPaused = false;

            Time.timeScale = _prepauseTimeScale;

            foreach (var pausable in _pausables)
            {
                pausable?.Resume(request);
            }
        }
    }
}
