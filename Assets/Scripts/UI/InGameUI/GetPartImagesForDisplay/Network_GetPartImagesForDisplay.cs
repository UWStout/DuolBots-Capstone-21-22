using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    [RequireComponent(typeof(Shared_GetPartImagesForDisplay))]
    public class Network_GetPartImagesForDisplay : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField] [Required]
        private BattleStateManager m_battleStateMan = null;
        private Shared_GetPartImagesForDisplay m_sharedController = null;

        private BattleStateChangeHandler m_battleHandler = null;


        // Domestic Intitialization
        private void Awake()
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(Awake), this, IS_DEBUGGING);
            #endregion Logs
            m_sharedController = GetComponent<Shared_GetPartImagesForDisplay>();
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_battleStateMan,
                nameof(m_battleStateMan), this);
            CustomDebug.AssertComponentIsNotNull(m_sharedController, this);
            #endregion Asserts
        }
        // Foreign Initialization
        private void Start()
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(Start), this, IS_DEBUGGING);
            #endregion Logs

            m_battleHandler = new BattleStateChangeHandler(m_battleStateMan,
                HandleBattleBegin, HandleBattleEnd, eBattleState.Battle);
        }
        private void OnDestroy()
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(OnDestroy), this, IS_DEBUGGING);
            #endregion Logs
            m_battleHandler.ToggleActive(false);
        }


        /// <summary>
        /// Initialize the part images. :)
        /// </summary>
        private void HandleBattleBegin()
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(HandleBattleBegin), this,
                IS_DEBUGGING);
            CustomDebug.Log($"Current amount of bot data specified " +
                $"{BuildSceneBotData.GetAllData().Length}", IS_DEBUGGING);
            foreach (BuiltBotDataWithTeamIndex temp_builtData
                in BuildSceneBotData.GetAllData())
            {
                CustomDebug.Log($"Holding bot data for team " +
                    $"{temp_builtData.teamIndex} ", IS_DEBUGGING);
            }
            #endregion Logs
            InitializePartImagesForDisplay();
        }
        /// <summary>
        /// Clears the part images.
        /// </summary>
        private void HandleBattleEnd()
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(HandleBattleEnd), this, IS_DEBUGGING);
            #endregion Logs
            m_sharedController.ClearPartImages();
        }
        private void InitializePartImagesForDisplay()
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(InitializePartImagesForDisplay),
                this, IS_DEBUGGING);
            #endregion Logs
            // In the network variant, we must initialize the part images display
            // for our team index.
            RobotHelpersSingleton temp_robotHelpers
                = RobotHelpersSingleton.instance;
            byte temp_teamIndex = temp_robotHelpers.DetermineMyTeamIndexNetwork();
            m_sharedController.InitializePartImagesForDisplay(temp_teamIndex);
        }
    }
}
