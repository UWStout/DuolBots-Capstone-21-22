using UnityEngine;
using UnityEngine.Assertions;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Network variant of the timer manager.
    /// Starts the timer when the match begins.
    /// </summary>
    [RequireComponent(typeof(Shared_TimerManager))]
    public class Network_TimerManager : NetworkBehaviour
    {
        private const bool IS_DEBUGGING = true;

        private BattleStateManager m_battleStateMan = null;
        private Shared_TimerManager m_sharedController = null;

        private BattleStateChangeHandler m_battleHandler = null;


        // Domestic Initialization
        private void Awake()
        {
            m_sharedController = GetComponent<Shared_TimerManager>();
            #region Asserts
            Assert.IsNotNull(m_sharedController, $"{name}'s {GetType().Name} " +
                $"requires {nameof(Shared_TimerManager)} but none was found");
            #endregion Asserts
        }
        public override void OnStartServer()
        {
            base.OnStartServer();

            m_battleStateMan = BattleStateManager.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_battleStateMan,
                this);
            #endregion Asserts

            m_battleHandler = new BattleStateChangeHandler(m_battleStateMan,
                HandleBattleBegin, HandleBattleEnd, eBattleState.Battle);
        }
        public override void OnStopServer()
        {
            base.OnStopServer();

            m_battleHandler?.ToggleActive(false);
        }


        /// <summary>
        /// Starts the timer once the battle has begun.
        /// </summary>
        [Server]
        private void HandleBattleBegin()
        {
            #region Logs
            CustomDebug.Log($"{name}'s {GetType().Name}'s " +
                $"<color=green>{nameof(HandleBattleBegin)}</color>", IS_DEBUGGING);
            #endregion Logs
            m_sharedController.StartTimer();

            ToggleTimerVisibilityClientRpc(true);
            m_sharedController.onTimerChanged += UpdateClientTimerClientRpc;
        }
        /// <summary>
        /// Resets the timer after a battle ends.
        /// </summary>
        [Server]
        private void HandleBattleEnd()
        {
            #region Logs
            CustomDebug.Log($"{name}'s {GetType().Name}'s " +
                $"<color=red>{nameof(HandleBattleEnd)}</color>", IS_DEBUGGING);
            #endregion Logs
            m_sharedController.ResetTimer();
            m_sharedController.HideTimer();

            ToggleTimerVisibilityClientRpc(false);
            m_sharedController.onTimerChanged -= UpdateClientTimerClientRpc;
        }

        [ClientRpc]
        private void UpdateClientTimerClientRpc(byte seconds, byte minutes)
        {
            // Don't handle twice for host
            if (isServer) { return; }

            m_sharedController.SetTimerText(seconds, minutes);
        }
        [ClientRpc]
        private void ToggleTimerVisibilityClientRpc(bool cond)
        {
            m_sharedController.ToggleTimerVisibility(cond);
        }
    }
}
