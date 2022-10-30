using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuolBots
{
    [RequireComponent(typeof(PlayerRobotInputController))]
    [RequireComponent(typeof(ProjectileTrajectory))]
    public class TrajectoryManager : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField]
        private List<TrajectoryScripts> m_slots = new List<TrajectoryScripts>();

        private BattleStateManager m_battleStateMan = null;

        private ITeamIndex m_teamIndex = null;
        private PlayerRobotInputController m_playerInpCont = null;
        private ProjectileTrajectory m_trajectory = null;

        private SlotPlacementManager m_slotPlaceMan = null;

        private BattleStateChangeHandler m_battleHandler = null;


        // Domestic Initialization
        private void Awake()
        {
            m_playerInpCont = GetComponent<PlayerRobotInputController>();
            m_trajectory = GetComponent<ProjectileTrajectory>();
            m_teamIndex = GetComponent<ITeamIndex>();

            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_playerInpCont, this);
            CustomDebug.AssertComponentIsNotNull(m_trajectory, this);
            CustomDebug.AssertIComponentIsNotNull(m_teamIndex, this);
            #endregion Asserts
        }
        private void Start()
        {
            m_battleStateMan = BattleStateManager.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_battleStateMan, this);
            #endregion Asserts
            m_battleHandler = new BattleStateChangeHandler(m_battleStateMan,
                HandleBattleBegin, HandleBattleEnd, eBattleState.Battle);
        }
        private void OnDestroy()
        {
            if (m_battleHandler == null) { return; }
            m_battleHandler.ToggleActive(false);
        }


        private void HandleBattleBegin()
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(HandleBattleBegin), this,
                IS_DEBUGGING);
            #endregion Logs

            m_playerInpCont.onSlotSelectedChanged += UpdateLine;

            RobotHelpersSingleton temp_robotHelpers = RobotHelpersSingleton.instance;
            #region Asserts
            CustomDebug.AssertDynamicSingletonMonoBehaviourPersistantIsNotNull(temp_robotHelpers,
                this);
            #endregion Asserts
            GameObject temp_myBot = temp_robotHelpers.FindBotRoot(m_teamIndex.teamIndex);
            m_slotPlaceMan = temp_myBot.GetComponentInChildren<SlotPlacementManager>();
            #region Asserts
            CustomDebug.AssertComponentInChildrenOnOtherIsNotNull(m_slotPlaceMan,
                temp_myBot, this);
            #endregion Asserts

            m_slots.Clear();
            int temp_slotAm = m_slotPlaceMan.GetSlotAmount();
            for (int i = 0; i < temp_slotAm; ++i)
            {
                if (!m_slotPlaceMan.GetSlotTransform(i,
                    out Transform temp_slotTrans))
                {
                    #region Logs
                    CustomDebug.LogForComponent($"found no part in slot {i}", this,
                        IS_DEBUGGING);
                    #endregion Logs
                    m_slots.Add(new TrajectoryScripts());
                    continue;
                }

                TrajectoryScripts temp_TS = new TrajectoryScripts();
                temp_TS.m_Specs = temp_slotTrans.GetComponentInChildren
                    <IPartTrajectorySpecs>();
                temp_TS.m_Traj = temp_slotTrans.GetComponentInChildren
                    <IPartShowsTrajectory>();

                m_slots.Add(temp_TS);
            }

            UpdateLine(m_playerInpCont.curSelectedSlot);
        }

        private void HandleBattleEnd()
        {
            m_trajectory.LineOff();
            m_slots.Clear();
            m_playerInpCont.onSlotSelectedChanged -= UpdateLine;
        }

        private void UpdateLine(byte slotIndex)
        {
            #region Logs
            CustomDebug.LogForComponent($"{nameof(UpdateLine)} with " +
                $"{nameof(slotIndex)}={slotIndex}", this, IS_DEBUGGING);
            #endregion Logs
            #region Asserts
            CustomDebug.AssertIndexIsInRange(slotIndex, m_slots, this);
            #endregion Asserts

            if (m_slots[slotIndex].m_Specs == null || m_slots[slotIndex].m_Traj == null)
            {
                #region Logs
                CustomDebug.LogForComponent($"turning off trajectory line",
                    this, IS_DEBUGGING);
                #endregion Logs
                m_trajectory.LineOff();
                return;
            }

            #region Logs
            CustomDebug.LogForComponent($"Updating trajecotry info",
                this, IS_DEBUGGING);
            #endregion Logs
            m_trajectory.UpdateInfo(m_slots[slotIndex].m_Specs, m_slots[slotIndex].m_Traj);
        }

        private struct TrajectoryScripts
        {
            public IPartTrajectorySpecs m_Specs;
            public IPartShowsTrajectory m_Traj;
        }

    }
}
