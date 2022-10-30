using System;
using System.Collections;
using UnityEngine;

using Cinemachine;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    public class BattleCameraSystem : SingletonMonoBehaviour<BattleCameraSystem>
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField] private Camera m_p0Cam = null;
        [SerializeField] private Camera m_p1Cam = null;

        [SerializeField] private CinemachineVirtualCamera m_waitingCamera = null;
        [SerializeField] private PairCamera<CinemachineFreeLook>
            m_activeBattleCameras = null;
        [SerializeField] private CinemachineVirtualCamera m_explosionCamera = null;

        [SerializeField] [Min(0.0f)]
        private float m_splitTransitionSpeed = 1.0f;

        private Coroutine m_splitTransitionCorout = null;
        private bool m_isSplitTransitionCoroutActive = false;

        public PairCamera<CinemachineFreeLook> activeBattleCameras
            => m_activeBattleCameras;

        /// <summary>
        /// Invoked when the transition to split/single screen finishes.
        /// </summary>
        public event Action onScreenTransitionFinished;


        public void ChangeToWaitingCamera()
        {
            TurnOffAllCameras();
            m_waitingCamera.gameObject.SetActive(true);
            TransitionToSingleScreen();
        }
        public void ChangeToActiveBattleCameras()
        {
            TurnOffAllCameras();
            m_activeBattleCameras.ToggleCameras(true);
            TransitionToSplitScreen();
        }
        public void ChangeToEDCinematicCamera()
        {
            TurnOffAllCameras();
            m_explosionCamera.gameObject.SetActive(true);
            TransitionToSingleScreen();
        }
        public void ChangeToEndCinematicCamera()
        {
            TurnOffAllCameras();
        }


        public void TurnOffAllCameras()
        {
            m_waitingCamera.gameObject.SetActive(false);
            m_activeBattleCameras.ToggleCameras(false);
            m_explosionCamera.gameObject.SetActive(false);
        }
        private void TransitionToSplitScreen()
        {
            // Stop the coroutine if one is active
            if (m_isSplitTransitionCoroutActive)
            {
                StopCoroutine(m_splitTransitionCorout);
            }
            m_splitTransitionCorout = StartCoroutine(
                TransitionToSplitScreenCoroutine());
        }
        private void TransitionToSingleScreen()
        {
            // Stop the coroutine if one is active
            if (m_isSplitTransitionCoroutActive)
            {
                StopCoroutine(m_splitTransitionCorout);
            }
            m_splitTransitionCorout = StartCoroutine(
                TransitionToSingleScreenCoroutine());
        }
        private IEnumerator TransitionToSplitScreenCoroutine()
        {
            float temp_p0CurWidth = m_p0Cam.rect.width;

            // Everything will be calculated from the
            // current width of player 1's camera.
            while (temp_p0CurWidth > 0.5f)
            {
                // Change the hinging value
                float temp_changeAmount = m_splitTransitionSpeed * Time.deltaTime;
                temp_p0CurWidth -= temp_changeAmount;

                CustomDebug.Log($"Changing p0 width to {temp_p0CurWidth}",
                    IS_DEBUGGING);

                // Update cameras
                SetCameraRectValues(temp_p0CurWidth);

                yield return null;
            }
            // Split screen p0 cam should take up half the screen
            SetCameraRectValues(0.5f);

            onScreenTransitionFinished?.Invoke();

            yield return null;
        }
        private IEnumerator TransitionToSingleScreenCoroutine()
        {
            float temp_p0CurWidth = m_p0Cam.rect.width;

            // Everything will be calculated from the
            // current width of player 1's camera.
            while (temp_p0CurWidth < 1.0f)
            {
                // Change the hinging value
                float temp_changeAmount = m_splitTransitionSpeed * Time.deltaTime;
                temp_p0CurWidth += temp_changeAmount;

                // Update cameras
                SetCameraRectValues(temp_p0CurWidth);

                yield return null;
            }
            // Split screen p0 cam should take up the entire screen
            SetCameraRectValues(1.0f);

            onScreenTransitionFinished?.Invoke();

            yield return null;
        }
        private void SetCameraRectValues(float p0Width)
        {
            Rect temp_p0Rect = m_p0Cam.rect;
            temp_p0Rect.width = p0Width;

            Rect temp_p1Rect = m_p1Cam.rect;
            // We want to always take up the entirety of the screen, so
            // the sum of the two players' camera widths should always be 1.
            temp_p1Rect.width = 1 - temp_p0Rect.width;
            // Player 0's rect starts at 0 always, so the width
            // of Player 0's rect is also the x pos of Player 1's.
            temp_p1Rect.x = temp_p0Rect.width;

            // Update the values of the cameras
            m_p0Cam.rect = temp_p0Rect;
            m_p1Cam.rect = temp_p1Rect;
        }
    }
}
