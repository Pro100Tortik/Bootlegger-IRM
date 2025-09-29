using System;
using TMPro;
using UnityEngine;

namespace Bootlegger
{
    public class HUDClock : MonoBehaviour
    {
        [SerializeField] private TMP_Text clockText;

        private void Start()
        {
            GameTimeManager.Instance.OnTimeChanged += UpdateClock;

            clockText.text = GameTimeManager.Instance.ToString();
        }

        private void OnDestroy()
        {
            GameTimeManager.Instance.OnTimeChanged -= UpdateClock;
        }

        private void UpdateClock(TimeSpan timeSpan)
        {
            clockText.text = GameTimeManager.Instance.ToString();
        }
    }
}
