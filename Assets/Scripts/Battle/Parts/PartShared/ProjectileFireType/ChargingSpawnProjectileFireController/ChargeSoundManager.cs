using System;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Manages the playing of a charging sound with begin, resume,
    /// and pause functionality.
    /// </summary>
    public class ChargeSoundManager
    {
        // Wwise event names for the events.
        private WwiseEventName m_beginChargeWwiseEventName = null;
        private WwiseEventName m_pauseChargeWwiseEventName = null;
        private WwiseEventName m_resumeChargeWwiseEventName = null;
        // GameObject that the sound should be played for.
        private GameObject m_gameObject = null;
        // Action ot be invoked when we want to request an invoke for a wwise event.
        private Action<WwiseEventName, GameObject> m_requestInvokeWwiseEvent = null;

        // State of the charging sound
        private bool m_isChargeSoundPlaying = false;
        private bool m_wasChargeSoundStarted = false;


        /// <summary></summary>
        /// <param name="beginChargeWwiseEventName">Event to invoke to begin the
        /// charge sound.</param>
        /// <param name="pauseChargeWwiseEventName">Event to invoke to pause the
        /// charge sound.</param>
        /// <param name="resumeChargeWwiseEventName">Event to invoke to resume the
        /// paused charge sound.</param>
        /// <param name="gameObject">GameObject the sound should play on.</param>
        /// <param name="requestInvokeWwiseEvent">Reference to the action to call when
        /// we want to play a wwise event.</param>
        public ChargeSoundManager(WwiseEventName beginChargeWwiseEventName,
            WwiseEventName pauseChargeWwiseEventName,
            WwiseEventName resumeChargeWwiseEventName, GameObject gameObject,
            Action<WwiseEventName, GameObject> requestInvokeWwiseEvent)
        {
            m_beginChargeWwiseEventName = beginChargeWwiseEventName;
            m_pauseChargeWwiseEventName = pauseChargeWwiseEventName;
            m_resumeChargeWwiseEventName = resumeChargeWwiseEventName;
            m_gameObject = gameObject;
            m_requestInvokeWwiseEvent = requestInvokeWwiseEvent;
        }

        /// <summary>
        /// Updates the sound to either play/resume or pause it.
        ///
        /// If given true - will play if it hasn't started or resume
        /// if it has been paused.
        /// If given false - will stop playing if the sound is currenyly playing.
        /// </summary>
        public void UpdateSound(bool playOrPause)
        {
            // If what we are updating it to is the current state.
            if (m_isChargeSoundPlaying == playOrPause) { return; }
            m_isChargeSoundPlaying = playOrPause;

            // Want to play
            if (playOrPause)
            {
                // Charge sound was already started
                if (m_wasChargeSoundStarted)
                {
                    m_requestInvokeWwiseEvent?.Invoke(m_resumeChargeWwiseEventName,
                        m_gameObject);
                }
                // Charge sound needs to begin
                else
                {
                    m_requestInvokeWwiseEvent?.Invoke(m_beginChargeWwiseEventName,
                        m_gameObject);
                    m_wasChargeSoundStarted = true;
                }
            }
            // Want to pause
            else
            {
                m_requestInvokeWwiseEvent?.Invoke(m_pauseChargeWwiseEventName,
                    m_gameObject);
            }
        }
        /// <summary>
        /// Resets the sound to not be marked as playing or started.
        /// Also stops the sound if it is playing.
        /// </summary>
        public void Reset()
        {
            UpdateSound(false);

            m_isChargeSoundPlaying = false;
            m_wasChargeSoundStarted = false;
        }
    }
}
