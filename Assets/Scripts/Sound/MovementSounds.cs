using System;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Controls the playing of the movement sounds.
    /// </summary>
    [RequireComponent(typeof(SharedController_Movement))]
    public class MovementSounds : MonoBehaviour, IWwiseEventInvoker
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        [SerializeField, Required]
        private WwiseEventName m_beginMoveEventName = null;
        [SerializeField, Required]
        private WwiseEventName m_endMoveEventName = null;

        private SharedController_Movement m_moveCont = null;
        private bool m_isPlayingSound = false;

        public event Action<WwiseEventName, GameObject> requestInvokeWwiseEvent;


        // Domestic Initialization
        private void Awake()
        {
            m_moveCont = GetComponent<SharedController_Movement>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_moveCont, this);
            #endregion Asserts
        }
        private void Update()
        {
            UpdateMoveSound();
        }


        private void UpdateMoveSound()
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(UpdateMoveSound), this,
                IS_DEBUGGING);
            #endregion Logs
            float temp_leftMag = Mathf.Abs(m_moveCont.LeftPowerVisual);
            float temp_rightMag = Mathf.Abs(m_moveCont.RightPowerVisual);
            float temp_combinedMag = temp_leftMag + temp_rightMag;

            // Start playing sound if moving, stop if not moving.
            ToggleSoundPlaying(temp_combinedMag > 0.0f);
        }
        private void ToggleSoundPlaying(bool playOrStop)
        {
            #region Logs
            CustomDebug.LogForComponent($"{nameof(ToggleSoundPlaying)} " +
                $"{nameof(playOrStop)}={playOrStop}", this,
                IS_DEBUGGING);
            #endregion Logs
            // If the sound is already in the state we are asking to change it to.
            if (m_isPlayingSound == playOrStop) { return; }
            m_isPlayingSound = playOrStop;

            // Want to start playing
            if (playOrStop)
            {
                requestInvokeWwiseEvent?.Invoke(m_beginMoveEventName, gameObject);
            }
            // Want to stop playing
            else
            {
                requestInvokeWwiseEvent?.Invoke(m_endMoveEventName, gameObject);
            }
        }
    }
}
