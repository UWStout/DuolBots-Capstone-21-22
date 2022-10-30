using System;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    public class DrillSoundManager : MonoBehaviour, IWwiseEventInvoker
    {
        public enum eSpinSound { None, Air, Mat }

        [SerializeField, Required] private PartImpactCollider m_partImpCol = null;
        [SerializeField, Required]
        private WwiseEventName m_beginSpinAirEventName = null;
        [SerializeField, Required]
        private WwiseEventName m_endSpinAirEventName = null;
        [SerializeField, Required]
        private WwiseEventName m_beginSpinMatEventName = null;
        [SerializeField, Required]
        private WwiseEventName m_endSpinMatEventName = null;

        [SerializeField, Required]
        private WwiseEventName m_beginJabSoundName = null;
        [SerializeField, Required]
        private WwiseEventName m_endJabSoundName = null;

        // Current state of the spin sound
        private eSpinSound m_activeSpinSound = eSpinSound.None;

        public event Action<WwiseEventName, GameObject> requestInvokeWwiseEvent;


        /// <summary>
        /// Starts playing the jab sound.
        /// </summary>
        public void BeginJabSound()
        {
            requestInvokeWwiseEvent?.Invoke(m_beginJabSoundName, gameObject);
        }
        // Stops playing the jab sound.
        public void StopJabSound()
        {
            requestInvokeWwiseEvent?.Invoke(m_endJabSoundName, gameObject);
        }
        /// <summary>
        /// Updates the drill spinning sound to either be playing
        /// in air, or in material. Or turns off the currently playing sound.
        /// </summary>
        public void UpdateSpinSound(bool isSpinning)
        {
            // Stop spin sound if playing
            if (!isSpinning)
            {
                StopSpinSound();
                return;
            }

            bool temp_isContacting = m_partImpCol.IsCurrentlyImpacting();

            switch (m_activeSpinSound)
            {
                // No sound is currently playing.
                case eSpinSound.None:
                    UpdateSpinSoundFromNone(temp_isContacting);
                    break;
                // Air sound is currently playing
                case eSpinSound.Air:
                    UpdateSpinSoundFromAir(temp_isContacting);
                    break;
                // Mat sound is currently playing
                case eSpinSound.Mat:
                    UpdateSpinSoundFromMat(temp_isContacting);
                    break;
                default:
                    CustomDebug.UnhandledEnum(m_activeSpinSound, this);
                    break;
            }
        }


        /// <summary>
        /// Updates the sound when the assumption is the
        /// <see cref="m_activeSpinSound"/> is <see cref="eSpinSound.None"/>.
        /// </summary>
        private void UpdateSpinSoundFromNone(bool isContacting)
        {
            // Play the spinning in material.
            if (isContacting)
            {
                StartMatSpinSound();
                return;
            }
            // Play the spining in air.
            StartAirSpinSound();
        }
        /// <summary>
        /// Updates the sound when the assumption is the
        /// <see cref="m_activeSpinSound"/> is <see cref="eSpinSound.Air"/>.
        /// </summary>
        private void UpdateSpinSoundFromAir(bool isContacting)
        {
            // Drill is now hitting something, play material sound
            if (isContacting)
            {
                // Stop air sound
                StopSpinSound();
                // Begin mat
                StartMatSpinSound();
                return;
            }
            // Otherwise, just keep playing the air sound (do nothing)
        }
        /// <summary>
        /// Updates the sound when the assumption is the
        /// <see cref="m_activeSpinSound"/> is <see cref="eSpinSound.Mat"/>.
        /// </summary>
        private void UpdateSpinSoundFromMat(bool isContacting)
        {
            // Drill is no longer hitting something, stop playing
            // the material sound and instead play the air sound
            if (!isContacting)
            {
                // Stop mat sound
                StopSpinSound();
                // Begin air
                StartAirSpinSound();
            }
            // Otherwise, just keep playing the material sound (do nothing)
        }
        /// <summary>
        /// Start playing air sound and update the active spin sound.
        /// </summary>
        private void StartAirSpinSound()
        {
            requestInvokeWwiseEvent?.Invoke(m_beginSpinAirEventName, gameObject);
            m_activeSpinSound = eSpinSound.Air;
        }
        /// <summary>
        /// Start playing material sound and update the active spin sound.
        /// </summary>
        private void StartMatSpinSound()
        {
            requestInvokeWwiseEvent?.Invoke(m_beginSpinMatEventName, gameObject);
            m_activeSpinSound = eSpinSound.Mat;
        }
        /// <summary>
        /// Stops currently playing drill sound, whether its the air sound or
        /// the material sound.
        /// </summary>
        private void StopSpinSound()
        {
            WwiseEventName temp_stopEventName;
            switch (m_activeSpinSound)
            {
                // Nothing to stop
                case eSpinSound.None:
                    return;
                case eSpinSound.Air:
                    temp_stopEventName = m_endSpinAirEventName;
                    break;
                case eSpinSound.Mat:
                    temp_stopEventName = m_endSpinMatEventName;
                    break;
                default:
                    CustomDebug.UnhandledEnum(m_activeSpinSound, this);
                    return;
            }
            requestInvokeWwiseEvent?.Invoke(temp_stopEventName, gameObject);
            m_activeSpinSound = eSpinSound.None;
        }
    }
}
