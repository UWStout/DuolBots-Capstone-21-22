using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.InputSystem;
// Original Authors - Wyatt Senalik and Eslis Vang

namespace DuolBots
{
    /// <summary>
    /// Gets input for the dolly target cycler and updates
    /// the current selected index.
    /// </summary>
    public class Input_DollyTargetCycler : MonoBehaviour
    {
        public DollyTargetCycler dollyTargetCycler
        {
            get => m_dollyTargetCycler;
            set => SetDollyTargetCycler(value);
        }
        [SerializeField] private DollyTargetCycler m_dollyTargetCycler = null;
        [SerializeField] private PopupStatController m_statController = null;
        [SerializeField] [Range(0.01f, 1.0f)] private float m_cycleDeadzone = 0.1f;
        [SerializeField] private bool m_invertCycle = true;

        [SerializeField] private UnityEvent onUnconfirmSelection = new UnityEvent();

        private BetterBuildSceneStateManager m_stateMan = null;

        public event Action<int> onSelectionIndexChange;


        // Domestic Initialization
        private void Awake()
        {
            if (m_dollyTargetCycler != null)
            {
                m_dollyTargetCycler.onSelectionIndexChange
                    += onSelectionIndexChange;
            }
        }

        // Foreign Initialization
        private void Start()
        {
            m_stateMan = BetterBuildSceneStateManager.instance;
            #region Assert
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_stateMan, this); 
            #endregion
        }

        private void OnDestroy()
        {
            if (m_dollyTargetCycler != null)
            {
                m_dollyTargetCycler.onSelectionIndexChange
                    -= onSelectionIndexChange;
            };
        }


        private void SetDollyTargetCycler(DollyTargetCycler cycler)
        {
            if (m_dollyTargetCycler != null)
            {
                m_dollyTargetCycler.onSelectionIndexChange -= onSelectionIndexChange;
            }
            m_dollyTargetCycler = cycler;
            if (m_dollyTargetCycler == null) { return; }
            m_dollyTargetCycler.onSelectionIndexChange += onSelectionIndexChange;
            m_dollyTargetCycler.currentSelectedIndex = 0;
        }
        #region InputMessages
        private void OnCycle(InputValue value)
        {
            Assert.IsNotNull(m_dollyTargetCycler, $"{name}'s {GetType().Name} " +
                $"needs {nameof(m_dollyTargetCycler)} specified but none was.");

            if (m_dollyTargetCycler.isCurrentlyRotating) { return; }

            float temp_leftRight = value.Get<float>();
            temp_leftRight = m_invertCycle ? -temp_leftRight : temp_leftRight;

            // Left
            if (temp_leftRight < -m_cycleDeadzone)
            {
                int temp_newIndex = m_dollyTargetCycler.currentSelectedIndex - 1;
                temp_newIndex = temp_newIndex < 0 ?
                    m_dollyTargetCycler.amountTargetValues - 1 : temp_newIndex;

                m_dollyTargetCycler.currentSelectedIndex = temp_newIndex;
            }
            // Right
            else if (temp_leftRight > m_cycleDeadzone)
            {
                int temp_newIndex = (m_dollyTargetCycler.currentSelectedIndex + 1)
                    % m_dollyTargetCycler.amountTargetValues;

                m_dollyTargetCycler.currentSelectedIndex = temp_newIndex;
            }
            // Nothing
            else { return; }
            if (m_stateMan.curState ==
                eBetterBuildSceneState.Chassis)
            {
                m_statController.CalculateChassisStats(m_dollyTargetCycler.
                   currentSelectedIndex);
            }
        }
        private void OnUnconfirmSelection(InputValue value)
        {
            if (value.isPressed)
            {
                onUnconfirmSelection?.Invoke();
            }
        }
        #endregion InputMessages
    }
}
