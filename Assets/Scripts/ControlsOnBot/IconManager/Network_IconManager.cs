using UnityEngine;
using UnityEngine.Assertions;

using Mirror;
// Original Authors - Wyatt Senalik and Ben Lussman

namespace DuolBots.Mirror
{
    /// <summary>
    /// Probably DEPRECATED because we can't used over-bot icons anymore.
    /// 
    /// Network variant of the icon manager.
    /// Initializes the icons when the match begins.
    /// </summary>
    [RequireComponent(typeof(Shared_IconManager))]
    public class Network_IconManager : NetworkBehaviour
    {
        private BattleStateManager m_battleStateMan = null;
        private Shared_IconManager m_sharedController = null;

        private BattleStateChangeHandler m_battleHandler = null;


        // Domestic Initialization
        private void Awake()
        {
            m_sharedController = GetComponent<Shared_IconManager>();
            Assert.IsNotNull(m_sharedController, $"{name}'s {GetType().Name} " +
                $"requires {typeof(Shared_IconManager)} be attached but none was found.");
        }
        // Foreign Initialization
        private void Start()
        {
            m_battleStateMan = BattleStateManager.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(
                m_battleStateMan, this);
            #endregion Asserts

            m_battleHandler = new BattleStateChangeHandler(m_battleStateMan,
                HandleBattleBegin, HandleBattleEnd, eBattleState.Battle);
        }
        private void OnDestroy()
        {
            m_battleHandler.ToggleActive(false);
        }


        /// <summary>
        /// Setup icons.
        /// </summary>
        private void HandleBattleBegin()
        {
            SetupBotIcons();
        }
        private void HandleBattleEnd() { } // Empty. Ideally would destroy icons,
        // but we aren't event using this anymore, so whatever.


        private void SetupBotIcons()
        {
            // Don't setup icons on the enemy team's instance
            if (!hasAuthority) { return; }

            m_sharedController.SetupBotIcons();
        }
    }
}
