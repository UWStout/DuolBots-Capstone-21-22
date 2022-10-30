using System;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Cinemachine.Extensions
{
    /// <summary>
    /// Provides events for when a blend begins and when a blend ends.
    /// </summary>
    [RequireComponent(typeof(CinemachineBrain))]
    public class CineBrainBlendMonitor : MonoBehaviour
    {
        // Begin Blend Event
        public event Action onBeginBlendEvent;
        // End Blend Event
        public event Action onEndBlendEvent;

        public bool isBlending => m_brain.IsBlending;

        // Reference to the cinemachine brain that controls the blends
        private CinemachineBrain m_brain = null;


        // Called 0th
        // Domestic initialization
        private void Awake()
        {
            // Set references
            m_brain = GetComponent<CinemachineBrain>();

            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_brain, this);
            #endregion Asserts
        }
        // Called 1st
        // Foreign initialization
        private void Start()
        {
            m_brain.m_CameraActivatedEvent.AddListener(BlendBegan);
        }
        private void OnDestroy()
        {
            m_brain.m_CameraActivatedEvent.RemoveListener(BlendBegan);
        }


        /// <summary>
        /// Called when the cinemachine brian begins a blend event.
        /// </summary>
        /// <param name="cam1">First camera involved in the blend. Unused.</param>
        /// <param name="cam2">Second camera involved in the blend. Unused.</param>
        private void BlendBegan(ICinemachineCamera cam1, ICinemachineCamera cam2)
        {
            // Make sure there is a blend before calling
            if (m_brain.ActiveBlend != null)
            {
                CallBlendBeginEvent();
                DelayCallBlendEnd();
            }
        }
        /// <summary>
        /// Queues a call to the OnBlendEnd event to occur when the active blend has finished.
        /// </summary>
        private void DelayCallBlendEnd()
        { 
            // Cancel any previous invokes to CallBlendEndedEvent that may still be active
            CancelInvoke();

            // Get the time that the blend should end at
            float estimatedBlendEndTime = m_brain.ActiveBlend.Duration;
            // Invoke CallBlendEndedEvent after waiting the active blends duration time
            Invoke(nameof(CallBlendEndEvent), estimatedBlendEndTime);
        }
        /// <summary>
        /// Invokes the OnBlendBegin event.
        /// </summary>
        private void CallBlendBeginEvent()
        {
            onBeginBlendEvent?.Invoke();
        }
        /// <summary>
        /// Invokes the OnBlendEnd event.
        /// </summary>
        private void CallBlendEndEvent()
        {
            onEndBlendEvent?.Invoke();
        }
    }
}
