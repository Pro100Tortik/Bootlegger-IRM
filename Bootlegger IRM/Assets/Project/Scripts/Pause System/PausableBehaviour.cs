using UnityEngine;

namespace Bootlegger
{
    public abstract class PausableBehaviour : MonoBehaviour, IPausable
    {
        public bool IsPaused { get; protected set; } = false;
        protected PauseRequest LastPauseRequest { get; private set; }
        protected PauseRequest LastResumeRequest { get; private set; }

        public void Pause(in PauseRequest request)
        {
            LastPauseRequest = request;

            IsPaused = true;
            OnPause(request);
        }

        public void Resume(in PauseRequest request)
        {
            LastResumeRequest = request;

            IsPaused = false;
            OnResume(request);
        }

        protected virtual void OnPause(in PauseRequest request) { }

        protected virtual void OnResume(in PauseRequest request) { }

        protected virtual void Start()
        {
            PauseManager.Instance?.Subscribe(this);
        }

        protected virtual void OnDestroy()
        {
            PauseManager.Instance?.Unsubscribe(this);
        }

        private void Update()
        {
            if (IsPaused) return;

            OnUpdate();
        }

        private void FixedUpdate()
        {
            if (IsPaused) return;

            OnFixedUpdate();
        }

        private void LateUpdate()
        {
            if (IsPaused) return;

            OnLateUpdate();
        }

        protected virtual void OnUpdate() { }
        protected virtual void OnFixedUpdate() { }
        protected virtual void OnLateUpdate() { }
    }
}
