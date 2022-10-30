using UnityEngine;

using Mirror;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Destroys any left over bots when the active bot scenes are left.
    /// </summary>
    public class BotRootCleaner : NetworkBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField] [Required]
        private BattleStateManager m_battleStateMan = null;

        private BattleStateChangeHandler m_botActiveStateHandler = null;


        // Domestic Initialization
        private void Awake()
        {
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_battleStateMan,
                nameof(m_battleStateMan), this);
            #endregion Asserts
        }
        public override void OnStartServer()
        {
            base.OnStartServer();

            m_botActiveStateHandler = new BattleStateChangeHandler(m_battleStateMan,
                null, HandleChangeFromActiveBotScene, eBattleState.OpeningCinematic,
                eBattleState.Battle, eBattleState.EndingCinematic,
                eBattleState.GameOver);
        }
        public override void OnStopServer()
        {
            m_botActiveStateHandler.ToggleActive(false);
        }


        /// <summary>
        /// Destroy any existing bots.
        /// </summary>
        private void HandleChangeFromActiveBotScene()
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(HandleChangeFromActiveBotScene),
                this, IS_DEBUGGING);
            #endregion Logs

            RobotHelpersSingleton temp_botHelpers = RobotHelpersSingleton.instance;
            GameObject[] temp_allBots = temp_botHelpers.FindAllBotRoots(false);
            foreach (GameObject temp_curBot in temp_allBots)
            {
                NetworkServer.Destroy(temp_curBot);
            }
        }
    }
}
