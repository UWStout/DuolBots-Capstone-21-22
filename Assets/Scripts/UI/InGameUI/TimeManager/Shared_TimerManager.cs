using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
// Original Authors - ? (I think Shelby or Skyler).
// Adopted by Wyatt to network.

/// <summary>
/// Displays time countdown
/// </summary>
namespace DuolBots
{
    /// <summary>
    /// Controls the timer. Calls GameOverMonitor's GetRobotWinTimeOver
    /// when the timer reaches 0.
    /// </summary>
    public class Shared_TimerManager : MonoBehaviour
    {
        // Time (in seconds) that should be on the timer initially.
        [SerializeField] [Min(0.0f)] private float m_matchTime = 300.0f;
        // References
        // Text for the timer that will be managed by the timer manager.
        [SerializeField] private TextMeshProUGUI m_timerTextMesh = null;

        private float m_timeValue = 0.0f;
        private bool m_startedTimer = false;
        private bool m_hasTimerExpired = false;

        private byte m_prevMinutes = 0;
        private byte m_prevSeconds = 0;

        /// <summary>
        /// Called when the timer changes its text.
        /// Param1 = seconds, Param2 = minutes.
        /// </summary>
        public event Action<byte, byte> onTimerChanged;
        /// <summary>
        /// Called when the timer reaches zero.
        /// </summary>
        public event Action onTimerReachedZero;


        // Called 0th
        // Domestic Initalization
        private void Awake()
        {
            // Intialize the time to start at the match time
            m_timeValue = m_matchTime;
            UpdateTimer();
        }
        // Called every frame
        private void Update()
        {
            // If the timer hasn't been started, don't count yet
            if (!m_startedTimer) { return; }
            // If the timer has run out and that has already been handled,
            // don't keep trying to decrement the timer.
            if (m_hasTimerExpired) { return; }

            // Decrement the timer.
            m_timeValue -= Time.deltaTime;
            // Timer has reached 0.
            if (m_timeValue <= 0)
            {
                m_timeValue = 0;
                // Time has expired and we handled it.
                m_hasTimerExpired = true;
                onTimerReachedZero?.Invoke();
            }

            // TEMP - TEST to stop battle early
            if (Input.GetKeyDown(KeyCode.P))
            {
                m_timeValue = 10;
            }

            UpdateTimer();
        }


        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void StartTimer()
        {
            m_startedTimer = true;
            ShowTimer();
        }
        /// <summary>
        /// Sets the timer's text to be "Time: <paramref name="minutes"/>:
        /// <paramref name="seconds"/>".
        ///
        /// Pre Conditions - Assumes m_timerText is not null. Assumes both variables
        /// are between the range of [0, 59].
        /// Post Conditions - Changes the text of the m_timerText.
        /// </summary>
        public void SetTimerText(byte seconds, byte minutes)
        {
            string temp_secondsStr = seconds < 10 ?
                "0" + seconds : seconds.ToString();
            string temp_minutesStr = minutes < 10 ?
                "0" + minutes : minutes.ToString();

            m_timerTextMesh.text = $"{temp_minutesStr}:{temp_secondsStr}";
        }
        /// <summary>
        /// Resets the timer to be to its starting state.
        /// </summary>
        /// <param name="startTimer">If the timer should automatically
        /// start after this reset.</param>
        public void ResetTimer(bool startTimer = false)
        {
            m_timeValue = m_matchTime;
            m_startedTimer = false;
            m_hasTimerExpired = false;
            m_prevMinutes = 0;
            m_prevSeconds = 0;

            UpdateTimer();

            // If the reset also wants to start the timer back up
            if (startTimer) { StartTimer(); }
        }
        public void ShowTimer()
        {
            ToggleTimerVisibility(true);
        }
        public void HideTimer()
        {
            ToggleTimerVisibility(false);
        }
        public void ToggleTimerVisibility(bool cond)
        {
            m_timerTextMesh.gameObject.SetActive(cond);
        }


        /// <summary>
        /// Updates the timer internally by calculating the current time
        /// and changing the timer based on that time.
        /// </summary>
        private void UpdateTimer()
        {
            GetCurrentTime(out byte temp_seconds, out byte temp_minutes);
            ChangeTimer(temp_seconds, temp_minutes);
        }
        /// <summary>
        /// Calculates the current time based on the total seconds left
        /// in m_timeValue.
        /// </summary>
        /// <param name="seconds">Seconds left on the timer [0-59].</param>
        /// <param name="minutes">Minutes left on the timer.</param>
        private void GetCurrentTime(out byte seconds, out byte minutes)
        {
            minutes = (byte)(m_timeValue / 60);
            seconds = (byte)(m_timeValue % 60);
        }
        /// <summary>
        /// Sets the timer's text only if the given values are different
        /// than the values given last time this was called.
        /// So that the onTimerChange event is not called unnecessarily.
        ///
        /// Pre Conditions - Assumes that SetTimerText has its Pre Conditions
        /// satisfied.
        /// Post Conditions - Potentially sets the timer text and calls onTimerChange.
        /// </summary>
        /// <param name="seconds">Seconds to change to be on the timer.
        /// Expects the range to be [0-59].</param>
        /// <param name="minutes">Minutes to change to be on the timer.
        /// Expects the range to be [0-59].</param>
        private void ChangeTimer(byte seconds, byte minutes)
        {
            // Don't actually change the timer text if the time is the same as last
            // time. This keeps us from calling the change event unnecessarily.
            if (m_prevMinutes == minutes && m_prevSeconds == seconds) { return; }

            SetTimerText(seconds, minutes);

            onTimerChanged?.Invoke(seconds, minutes);

            m_prevMinutes = minutes;
            m_prevSeconds = seconds;
        }
    }
}

