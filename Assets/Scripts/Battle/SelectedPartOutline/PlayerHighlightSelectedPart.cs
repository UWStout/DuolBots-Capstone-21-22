using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// When the player changes slots, this hears that change and
    /// tells the corresponding part to highlight.
    /// </summary>
    [RequireComponent(typeof(PlayerRobotInputController))]
    [RequireComponent(typeof(PlayerIndex))]
    public class PlayerHighlightSelectedPart : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        // When to initialize
        private enum eInitializeTime { OnAwake, OnMatchBegin };
        [SerializeField] private eInitializeTime m_initTime = eInitializeTime.OnAwake;

        private BattleStateManager m_battleStateMan = null;
        private PlayerRobotInputController m_playerInpCont = null;
        private ITeamIndex m_teamIndex = null;
        private PlayerIndex m_playerIndex = null;
        private SlotPlacementManager m_slotPlacementMan = null;

        private BattleStateChangeHandler m_battleHandler = null;
        private PartHighlight m_prevHiglightedPart = null;
        private bool m_isSubbed = false;



        // Domestic Initialization
        private void Awake()
        {
            m_playerInpCont = GetComponent<PlayerRobotInputController>();
            m_teamIndex = GetComponent<ITeamIndex>();
            m_playerIndex = GetComponent<PlayerIndex>();

            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_playerInpCont, this);
            CustomDebug.AssertIComponentIsNotNull(m_teamIndex, this);
            CustomDebug.AssertComponentIsNotNull(m_playerIndex, this);
            #endregion Asserts

            if (m_initTime == eInitializeTime.OnAwake)
            {
                HandleBattleBegin();
            }
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

            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            m_battleHandler.ToggleActive(false);

            ToggleSubscriptions(false);
        }


        /// <summary>
        /// Finds the bot that corresponds to this player's team and
        /// initilaizes the scripts variables based on that.
        /// </summary>
        private void HandleBattleBegin()
        {
            #region Debugs
            CustomDebug.Log($"{name}'s {GetType()}'s {nameof(HandleBattleBegin)}",
                IS_DEBUGGING);
            #endregion Debugs

            GameObject temp_myBot = RobotHelpersSingleton.instance.FindBotRoot(
                m_teamIndex.teamIndex);
            m_slotPlacementMan = temp_myBot.GetComponentInChildren
                <SlotPlacementManager>();
            #region Asserts
            CustomDebug.AssertComponentInChildrenOnOtherIsNotNull(
                m_slotPlacementMan, temp_myBot, this);
            #endregion Assets
            
            // Highlight the currently selected slot by default
            ChangeSlotHighlight(m_playerInpCont.curSelectedSlot);
        }
        /// <summary>
        /// Unhighlight the currently highlighted part.
        /// </summary>
        private void HandleBattleEnd()
        {
            #region Asserts
            CustomDebug.AssertIsTrueForComponent(m_prevHiglightedPart != null,
                $"a part to be highlighted before the battle ended.",
                this);
            #endregion Asserts
            m_prevHiglightedPart.DeactiveHighlight(m_playerIndex.playerIndex);
        }
        /// <summary>
        /// Subscribes (true) or unsubscribes (false) to/from interested events.
        /// </summary>
        /// <param name="cond">Sub (true) or unsub (false)</param>
        private void ToggleSubscriptions(bool cond)
        {
            if (cond == m_isSubbed) { return; }

            // Sub
            if (cond)
            {
                m_playerInpCont.onSlotSelectedChanged += ChangeSlotHighlight;
            }
            // Unsub
            else
            {
                if (m_playerInpCont != null)
                {
                    m_playerInpCont.onSlotSelectedChanged -= ChangeSlotHighlight;
                }
            }

            m_isSubbed = cond;
        }
        /// <summary>
        /// Changes which slot has a highlight on it.
        /// </summary>
        /// <param name="newSlotIndex">Index of the new slot that should
        /// be highlighted now.</param>
        private void ChangeSlotHighlight(byte newSlotIndex)
        {
            

            #region Logs
            CustomDebug.Log($"{nameof(ChangeSlotHighlight)} to {newSlotIndex}",
                IS_DEBUGGING);
            #endregion Logs
            if (m_slotPlacementMan == null)
            {
                #region Logs
                Debug.LogWarning($"{name}'s {GetType().Name}'s " +
                    $"{nameof(ChangeSlotHighlight)} is being called before " +
                    $"{nameof(m_slotPlacementMan)} is initialized");
                #endregion Logs
                return;
            }
            // No slotted parts.
            if (m_slotPlacementMan.GetAmountSlotsThatHaveChildren() == 0) { return; }

            Transform temp_slotToHighlightTrans
                = m_slotPlacementMan.GetSlotTransform(newSlotIndex);
            PartHighlight temp_slotToHighlight
                = temp_slotToHighlightTrans.GetComponentInChildren<PartHighlight>();

            #region Asserts
            CustomDebug.AssertComponentInChildrenOnOtherIsNotNull(
                temp_slotToHighlight, temp_slotToHighlightTrans.gameObject, this);
            #endregion Asserts

            // Active new highlight, deactive previous highlight,
            // and replace old with new.
            temp_slotToHighlight.ActivateHighlight(m_playerIndex.playerIndex);
            // If null on first instance
            if (m_prevHiglightedPart != null)
            {
                m_prevHiglightedPart.DeactiveHighlight(m_playerIndex.playerIndex);
            }
            m_prevHiglightedPart = temp_slotToHighlight;
        }
    }
}
