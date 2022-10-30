using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// When the end state is reached, this will reset all the catchup
    /// events subscribed to the event resetter.
    /// </summary>
    public class BattleResetCatchupEvents : MonoBehaviour
    {
        [SerializeField] [Required]
        private BattleStateManager m_battleStateMan = null;
        [SerializeField] [Required]
        private CatchupEventResetter m_eventResetter = null;

        private BattleStateChangeHandler m_endHandler = null;


        // Domestic Initialization
        private void Awake()
        {
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_battleStateMan,
                nameof(m_battleStateMan), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_eventResetter,
                nameof(m_eventResetter), this);
            #endregion Asserts

            m_endHandler = new BattleStateChangeHandler(m_battleStateMan,
                HandleEndBegin, null, eBattleState.End);
        }
        private void OnDestroy()
        {
            m_endHandler.ToggleActive(false);
        }


        /// <summary>
        /// When battle reaches its end state, reset all catchup events
        /// in case we are going to some other state after the end state.
        /// </summary>
        private void HandleEndBegin()
        {
            m_eventResetter.ResetAllCatchupEvents();
        }
    }
}
