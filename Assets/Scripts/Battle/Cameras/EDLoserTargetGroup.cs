using System.Collections.Generic;
using UnityEngine;

using Cinemachine;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    public class EDLoserTargetGroup : MonoBehaviour
    {
        private const bool IS_DEBUGGING = true;

        [SerializeField] [Required] private BattleStateManager m_stateMan = null;
        [SerializeField] [Required] private GameOverMonitor m_gameOverMon = null;
        [SerializeField] [Required]
        private CinemachineTargetGroup m_targetGroup = null;
        [SerializeField] [Min(0.0f)] private float m_targetRadius = 10.0f;
        private float m_usedTargetRadius = 0;
        public float TargetRadius { set { m_targetRadius = value; } }

        private BattleStateChangeHandler m_edHandler = null;
        private RobotHelpersSingleton m_RobotHelpersSingleton = null;

        // Domestic Initialization
        private void Awake()
        {
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_targetGroup,
                nameof(m_targetGroup), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_stateMan,
                nameof(m_stateMan), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_gameOverMon,
                nameof(m_gameOverMon), this);
            #endregion Asserts
            m_RobotHelpersSingleton = RobotHelpersSingleton.instance;
        }
        private void Start()
        {
            m_edHandler = new BattleStateChangeHandler(m_stateMan,
                OnEdBegin, null, eBattleState.EndingCinematic);
        }
        private void OnDestroy()
        {
            m_edHandler.ToggleActive(false);
        }


        private void OnEdBegin()
        {
            SetLoserBotRootsToTarget();
        }
        /// <summary>
        /// TODO - This is setting the wrong bot for someone.
        /// Idk if its the host or client or winner or losing or some combination
        /// of those, but one team always gets it right and the other gets it wrong.
        /// </summary>
        private void SetLoserBotRootsToTarget()
        {
            m_usedTargetRadius = m_targetRadius;
            // Reset to no targets
            m_targetGroup.m_Targets = new CinemachineTargetGroup.Target[0];

            // Find the bots that should be the targets
            RobotHelpersSingleton temp_botHelpers = RobotHelpersSingleton.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(temp_botHelpers,
                this);
            #endregion Asserts
            IReadOnlyList<GameObject> temp_losingBotRoots
                = temp_botHelpers.FindLosingBots(m_gameOverMon.gameOverData.winningTeamIndices);

            // Add the bots to the target list.
            if (m_gameOverMon.gameOverData.winningTeamIndices.Count != 1)
            {
                CustomDebug.LogForComponent($"thinks there is a tie",
                    this, IS_DEBUGGING);

                Vector3 temp_dist = m_RobotHelpersSingleton.FindBotRoot(0).transform.position - m_RobotHelpersSingleton.FindBotRoot(1).transform.position;
                temp_dist.y = 0;

                CinemachineVirtualCamera temp_virtCam =
                    GetComponent<CinemachineVirtualCamera>();
                CustomDebug.AssertComponentOnOtherIsNotNull(temp_virtCam,
                    gameObject, this);
                CinemachineFramingTransposer temp_transposer = temp_virtCam.
                    GetCinemachineComponent<CinemachineFramingTransposer>();
                CustomDebug.AssertComponentOnOtherIsNotNull(temp_transposer,
                    temp_virtCam.gameObject, this);
                m_usedTargetRadius = temp_dist.magnitude;
                temp_transposer.m_TrackedObjectOffset = new Vector3(0,
                    m_usedTargetRadius*m_usedTargetRadius, 0);
            }
            foreach (GameObject temp_curBot in temp_losingBotRoots)
            {
                CustomDebug.LogForComponent($"adding member ({temp_curBot.name}) " +
                    $"to target group", this, IS_DEBUGGING);
                m_targetGroup.AddMember(temp_curBot.transform, 1.0f, m_usedTargetRadius);
            }
        }
    }
}
