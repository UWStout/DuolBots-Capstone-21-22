using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik (based on Shelby's original HealthSlider_UI).

namespace DuolBots.Mirror
{
    /// <summary>
    /// Network driver for the <see cref="Shared_HealthSlider_UI"/>.
    /// </summary>
    [RequireComponent(typeof(Shared_HealthSlider_UI))]
    public class Network_HealthSlider_UI : MonoBehaviour
    {
        [SerializeField] [Required]
        private BattleStateManager m_battleStateMan = null;

        private Shared_HealthSlider_UI m_sharedController = null;

        private BattleStateChangeHandler m_battleHandler = null;


        // Domestic Initialization
        private void Awake()
        {
            m_sharedController = GetComponent<Shared_HealthSlider_UI>();
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_battleStateMan,
                nameof(m_battleStateMan), this);
            CustomDebug.AssertComponentIsNotNull(m_sharedController, this);
            #endregion Asserts

            m_battleHandler = new BattleStateChangeHandler(m_battleStateMan,
                HandleBattleBegin, null, eBattleState.Battle);
        }
        private void OnDestroy()
        {
            m_battleHandler.ToggleActive(false);
        }


        private void HandleBattleBegin()
        {
            // In the network variant, we find which bot we have ownership over.
            RobotHelpersSingleton temp_botHelpers = RobotHelpersSingleton.instance;
            GameObject temp_botRoot = temp_botHelpers.FindOwnedBotRootNetwork();
            // Initialize the 
            m_sharedController.Initialize(temp_botRoot);
        }
    }
}
