using UnityEngine;

using Mirror;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Network variant of GameOverMonitor. Initializes shared on Start only on server. 
    /// </summary>
    [RequireComponent(typeof(Shared_HealthGameOver))]
    public class Network_HealthGameOver : NetworkBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = true;

        [SerializeField] [Required]
        private BattleStateManager m_battleStateMan = null;

        private Shared_HealthGameOver m_sharedController = null;

        private BattleStateChangeHandler m_battleHandler = null;


        // Domestic Initialization
        // Called on both server and client
        private void Awake()
        {
            m_sharedController = GetComponent<Shared_HealthGameOver>();
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_battleStateMan,
                nameof(m_battleStateMan), this);
            CustomDebug.AssertComponentIsNotNull(m_sharedController, this);
            #endregion Asserts
        }
        public override void OnStartServer()
        {
            base.OnStartServer();
            #region Logs
            CustomDebug.Log($"{name}'s {GetType().Name}'s " +
                    $"{nameof(OnStartServer)}.", IS_DEBUGGING);
            #endregion Logs
            m_battleHandler = new BattleStateChangeHandler(m_battleStateMan,
                HandleBattleStart, null, eBattleState.Battle);
        }
        public override void OnStopServer()
        {
            base.OnStopServer();
            #region Logs
            CustomDebug.Log($"{name}'s {GetType().Name}'s " +
                    $"{nameof(OnStopServer)}.", IS_DEBUGGING);
            #endregion Logs
            m_battleHandler.ToggleActive(false);
        }


        /// <summary>
        /// Initializes the shared controller and subs to the onBotShouldDie event.
        /// </summary>
        [Server]
        private void HandleBattleStart()
        {
            m_sharedController.InitializeRobotHealths();
        }
    }
}
