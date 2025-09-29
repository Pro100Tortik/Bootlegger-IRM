using UnityEngine;
using System;

namespace Bootlegger
{
    public class GameTimeManager : Singleton<GameTimeManager>, ISaveable
    {
        public event Action<TimeSpan> OnTimeChanged;
        public event Action OnDayEnd;

        [Header("Time Settings")]
        [SerializeField] private float timeScale = 1f;
        [SerializeField] private int startHour = 7;
        [SerializeField] private int endHour = 4;
        [SerializeField] private bool isTimeRunning = true;

        private float _currentTimeInMinutes;

        public TimeSpan CurrentTime
        {
            get
            {
                int hours = (int)(_currentTimeInMinutes / 60) % 24;
                int minutes = (int)(_currentTimeInMinutes % 60);
                return new TimeSpan(hours, minutes, 0);
            }
        }

        public bool IsTimeRunning => isTimeRunning;

        private void Start()
        {
            _currentTimeInMinutes = startHour * 60;
        }

        private void FixedUpdate()
        {
            if (!isTimeRunning) return;

            _currentTimeInMinutes += Time.fixedDeltaTime * timeScale;

            if (ShouldStopAtFourAM())
            {
                StopTime();
            }

            OnTimeChanged?.Invoke(CurrentTime);
            //UpdateSunRotation();
        }

        private bool ShouldStopAtFourAM()
        {
            int currentHour = (int)(_currentTimeInMinutes / 60) % 24;

            return currentHour >= endHour && currentHour < startHour;
        }

        private void StopTime()
        {
            isTimeRunning = false;

            _currentTimeInMinutes = endHour * 60;
            OnTimeChanged?.Invoke(CurrentTime);
            OnDayEnd?.Invoke();
        }

        public void StartNewDay()
        {
            _currentTimeInMinutes = startHour * 60;
            isTimeRunning = true;
        }

        public void ToggleTime()
        {
            isTimeRunning = !isTimeRunning;
        }

        private void UpdateSunRotation()
        {
            if (RenderSettings.sun != null)
            {
                float adjustedTime = _currentTimeInMinutes;

                if (adjustedTime < startHour * 60)
                {
                    adjustedTime += 24 * 60;
                }

                float dayProgress = (adjustedTime - startHour * 60) / ((24 + endHour - startHour) * 60f);
                float sunAngle = Mathf.Lerp(0, 360, dayProgress);

                RenderSettings.sun.transform.rotation = Quaternion.Euler(new Vector3(sunAngle - 90, 170, 0));
            }
        }

        public override string ToString() => CurrentTime.ToString(@"hh\:mm");

        [ContextMenu("Start New Day")]
        private void StartNewDayContext() => StartNewDay();
    }
}
