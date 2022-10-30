using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

using Cinemachine;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik and Eslis Vang

namespace DuolBots
{
    /// <summary>
    /// Updates a virtual camera driven by a dolly body.
    /// </summary>
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class DollyTargetCycler : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;


        [SerializeField] private float[] m_targetValues =
        { 10.0f, 10.0f + (20.0f / 3.0f), 10.0f - (20.0f / 3.0f) };
        public float[] targetValues
        {
            get => m_targetValues;
            set => m_targetValues = value;
        }

        [SerializeField][CurveRange(0.0f, 0.0f, 1.0f, 1.0f, EColor.Blue)]
        private AnimationCurve m_speedCurve = null;

        [SerializeField][Min(0.0f)] private float m_timeToTake = 1.0f;

        public int amountTargetValues => m_targetValues.Length;

        public int currentSelectedIndex
        {
            get => m_currentSelectedIndex;
            set => SetSelectedIndex(value);
        }
        [SerializeField, ReadOnly] private int m_currentSelectedIndex = 0;

        private CinemachineVirtualCamera m_dollyCamera = null;
        private CinemachineTrackedDolly m_trackedDolly = null;
        public CinemachineSmoothPath dollyPath
        {
            get => m_dollyPath;
            set
            {
                m_dollyPath = value;
                m_trackedDolly.m_Path = m_dollyPath;
                // Select the index of the new dolly path
                SetSelectedIndex(m_currentSelectedIndex);
            }
        }
        private CinemachineSmoothPath m_dollyPath = null;

        public bool isCurrentlyRotating => m_isCurrentRotateToNextPosActive;
        private bool m_isCurrentRotateToNextPosActive = false;
        private Coroutine m_currentRotateToNextPos = null;

        /// <summary>
        /// Events that gets called when the current selected index changes.
        /// Param - New selected index.
        /// </summary>
        public event Action<int> onSelectionIndexChange;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            m_dollyCamera = GetComponent<CinemachineVirtualCamera>();
            CustomDebug.AssertComponentIsNotNull(m_dollyCamera, this);
            // Virtual Camera must be driven by a dolly
            m_trackedDolly = m_dollyCamera.
                GetCinemachineComponent<CinemachineTrackedDolly>();
            Assert.IsNotNull(m_trackedDolly, $"{name}'s " +
                $"{GetType().Name} requires {nameof(CinemachineTrackedDolly)} " +
                $"be the Body of {nameof(m_dollyCamera)} but none was found.");
            // Dolly must use a smooth path
            m_dollyPath = m_trackedDolly.m_Path as CinemachineSmoothPath;
            /* This is okay now since it is possible that we set the dollypath later
            Assert.IsNotNull(dollyPath, $"{name}'s {GetType().Name} " +
                $"requires {m_trackedDolly.name}'s " +
                $"{nameof(CinemachineTrackedDolly)} use a " +
                $"{nameof(CinemachineSmoothPath)} but it was not.");
            */
        }


        /// <summary>
        /// Updates the current selected index to the new index.
        /// </summary>
        /// <param name="newIndex"></param>
        private void SetSelectedIndex(int newIndex)
        {
            if (newIndex < 0)
            {
                Debug.LogError("Invalid index. Less than 0.");
                return;
            }
            if (newIndex >= m_targetValues.Length)
            {
                Debug.LogError("Invalid index. Greater than the amount of " +
                    $"specified target values ({m_targetValues.Length}).");
                return;
            }

            UpdateCurrentSelectedIndex(newIndex);
        }
        private void UpdateCurrentSelectedIndex(int newIndex)
        {
            int temp_prev = m_currentSelectedIndex;
            m_currentSelectedIndex = newIndex;

            // We do this whenever now because we need the event
            // to be invoked when this has its dolly path set (for SlotViewer).
            //if (temp_prev != m_currentSelectedIndex)
            //{
                onSelectionIndexChange?.Invoke(m_currentSelectedIndex);
            //}

            StartRotateToNextPositionCoroutine(m_targetValues[newIndex]);
        }
        private void StartRotateToNextPositionCoroutine(float pathPos)
        {
            if (m_isCurrentRotateToNextPosActive)
            {
                return;
            }

            // We didn't used to do this, but something is making this be null
            // and I don't know what, but the game still runs in editor, so idk.
            if (m_dollyPath == null) { return; }
            m_currentRotateToNextPos = StartCoroutine(
                RotateToNextPositionCoroutine(pathPos));
        }


        private IEnumerator RotateToNextPositionCoroutine(float targetPathPos)
        {
            #region Asserts
            Assert.IsNotNull(m_dollyPath, $"{nameof(m_dollyPath)} has not yet " +
                $"been initialized.");
            #endregion Asserts

            m_isCurrentRotateToNextPosActive = true;

            // Get starting data
            float temp_startPathPos = m_trackedDolly.m_PathPosition;
            int temp_amountPoints = m_dollyPath.m_Waypoints.Length;
            // Figure out which direction is the one with least difference
            // Normal (direct from numbers)
            float temp_normalDiff = targetPathPos - temp_startPathPos;
            // Loop around forward
            float temp_forwardsDiff = temp_normalDiff + temp_amountPoints;
            // Loop around backward
            float temp_backwardsDiff = temp_normalDiff - temp_amountPoints;

            // Find minimum difference of the differences
            float temp_smallestDiff = MathHelpers.MinIgnoreSign(temp_normalDiff,
                temp_forwardsDiff, temp_backwardsDiff);

            CustomDebug.Log($"Start: {temp_startPathPos}. " +
                $"Target: {targetPathPos}. Small Diff: {temp_smallestDiff}",
                IS_DEBUGGING);

            float temp_timer = 0.0f;
            float t = 0.0f;
            while (t < 1.0f)
            {
                // Percent finished.
                float temp_evalutaion = m_speedCurve.Evaluate(temp_timer);

                m_trackedDolly.m_PathPosition = temp_startPathPos +
                    temp_evalutaion * temp_smallestDiff;

                CustomDebug.Log($"Eval: {temp_evalutaion}. " +
                    $"Pos: {m_trackedDolly.m_PathPosition}.",
                    IS_DEBUGGING);

                yield return null;

                temp_timer += Time.deltaTime;
                float temp_avoidZero = Mathf.Max(m_timeToTake, Mathf.Epsilon);
                t = temp_timer / temp_avoidZero;

            }
            m_trackedDolly.m_PathPosition = targetPathPos;

            m_isCurrentRotateToNextPosActive = false;
        }
    }
}
