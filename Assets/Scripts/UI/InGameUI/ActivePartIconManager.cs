using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Original Authors - Shelby Vian
// Tweaked by Wyatt and Aaron

namespace DuolBots
{
    public class ActivePartIconManager : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        private BattleStateManager m_battleStateMan = null;
        private InstantiateLayoutPrefabs m_instLayoutPrefs = null;

        private BattleStateChangeHandler m_battleHandler = null;

        private Shared_GetPartImagesForDisplay m_partImagesForDisplay = null;
        private Dictionary<byte, GameObject> m_partIconsDict =
            new Dictionary<byte, GameObject>();
        public byte curSlot = 0;

        private PartHealthColorScale m_setOverlay = null;

        private PlayerRobotInputController m_playerInpCont = null;
        private ITeamIndex m_teamIndex = null;
        private PlayerIndex m_playerIndex = null;
        private SlotPlacementManager m_slotPlacementMan = null;

        private FullChargeAnimator[] m_chargeAnimator = new FullChargeAnimator[2];

        private bool m_isSubbed = false;


        // Domestic Initialization
        private void Awake()
        {
            m_playerInpCont = GetComponent<PlayerRobotInputController>();
            m_teamIndex = GetComponent<ITeamIndex>();
            m_playerIndex = GetComponent<PlayerIndex>();

            GameObject temp = GameObject.Find("NetworkBattleLogic/BattleViewManager/BattleUIManager (Canvas)/InGameUI");

            if (temp == null)
            {
                Debug.LogError("No InGameUI object found");
            }
            else
                m_chargeAnimator = temp.GetComponentsInChildren<FullChargeAnimator>();

            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_playerInpCont, this);
            CustomDebug.AssertIComponentIsNotNull(m_teamIndex, this);
            CustomDebug.AssertComponentIsNotNull(m_playerIndex, this);
            #endregion Asserts
        }
        // Foreign Initialization
        private void Start()
        {
            m_battleStateMan = BattleStateManager.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_battleStateMan,
                this);
            #endregion Asserts
            m_battleHandler = new BattleStateChangeHandler(m_battleStateMan,
                HandleBattleStart, HandleBattleEnd, eBattleState.Battle);
        }
        private void OnDestroy()
        {
            m_battleHandler.ToggleActive(false);
        }



        private void HandleBattleStart()
        {
            m_instLayoutPrefs = InstantiateLayoutPrefabs.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_instLayoutPrefs,
                this);
            #endregion Asserts

            ToggleSubscriptions(true);
        }
        private void HandleBattleEnd()
        {
            ToggleSubscriptions(false);
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
                m_playerInpCont.onSlotSelectedChanged
                    += UpdateCurrentActiveSlot;
                m_instLayoutPrefs.onLayoutInstantiated.ToggleSubscription(
                    Initialize, true);
            }
            // Unsub
            else
            {
                if (m_playerInpCont != null)
                {
                    m_playerInpCont.onSlotSelectedChanged
                        -= UpdateCurrentActiveSlot;
                }
                if (m_instLayoutPrefs != null)
                {
                    m_instLayoutPrefs.onLayoutInstantiated.ToggleSubscription(
                        Initialize, false);
                }
            }

            m_isSubbed = cond;
        }
        /// <summary>
        /// Finds the bot that corresponds to this player's team and
        /// initilaizes the scripts variables based on that.
        /// </summary>
        private void Initialize(IReadOnlyList<GameObject> spawnedIconObjs)
        {
            GameObject temp_myBot = RobotHelpersSingleton.instance.FindBotRoot(
                m_teamIndex.teamIndex);
            m_slotPlacementMan = temp_myBot.GetComponentInChildren
                <SlotPlacementManager>();

            CustomDebug.Log($"{name}'s {GetType()}'s {nameof(Initialize)} for " +
                $"player {m_playerIndex.playerIndex} found " +
                $"{spawnedIconObjs.Count} PartImageUIs", IS_DEBUGGING);

            m_partIconsDict.Clear();

            foreach (GameObject temp_iconObj in spawnedIconObjs)
            {
                PlayerIndex temp_iconPlayerIndex
                    = temp_iconObj.transform.parent.GetComponent<PlayerIndex>();
                SetImageTextures temp_imgTex = temp_iconObj.
                    GetComponentInChildren<SetImageTextures>();

                #region Asserts
                CustomDebug.AssertComponentOnOtherIsNotNull(temp_iconPlayerIndex,
                    temp_iconObj.transform.parent.gameObject, this);
                CustomDebug.AssertComponentInChildrenOnOtherIsNotNull(
                    temp_imgTex, temp_iconObj, this);
                #endregion Asserts

                #region Logs
                CustomDebug.Log($"{temp_iconObj.name} has player index " +
                    $"{temp_iconPlayerIndex.playerIndex}", IS_DEBUGGING);
                #endregion Logs

                if (temp_iconPlayerIndex.playerIndex == m_playerIndex.playerIndex)
                {
                    m_partIconsDict.Add(temp_imgTex.partSlot, temp_iconObj);
                }
            }


            SetSizes(m_playerInpCont.curSelectedSlot, true);
            curSlot = m_playerInpCont.curSelectedSlot;

            #region Asserts
            CustomDebug.AssertComponentInChildrenOnOtherIsNotNull(
                m_slotPlacementMan, temp_myBot, this);
            #endregion Assets
        }
        private void UpdateCurrentActiveSlot(byte index)
        {
            SetSizes(curSlot, false);//shrink current icon
            SetSizes(index, true);
            curSlot = index;
        }

        /// <summary>
        /// Changes the size of the icon for a given slot. Both increases and decreases icon size.
        /// </summary>
        /// <param name="slotIndex">the slot being changed</param>
        /// <param name="active">bool to set whether the icon size is increased or decreased</param>
        private void SetSizes(byte slotIndex, bool active)
        {
            if (!m_partIconsDict.TryGetValue(slotIndex, out GameObject temp_icon))
            {
                Debug.LogError($"{name}'s {GetType().Name}'s {nameof(SetSizes)} " +
                    $"slot index {slotIndex} not in dictionary");
                return;
            }
            m_setOverlay = temp_icon.GetComponent<PartHealthColorScale>();

            PlayerAssignedActions temp_assignedActions;

            if (m_playerIndex.playerIndex == 0)
                temp_assignedActions = GameObject.Find("Player1ControlsDisplay").
                    GetComponent<PlayerAssignedActions>();
            else
                temp_assignedActions = GameObject.Find("Player2ControlsDisplay").
                    GetComponent<PlayerAssignedActions>();

            temp_assignedActions.FindControlsOfActivePart(
                temp_icon.GetComponentInChildren<SetImageTextures>().partSlot);


            if (active && m_chargeAnimator[m_playerIndex.playerIndex] != null)
                m_chargeAnimator[m_playerIndex.playerIndex].SetActivePart(slotIndex, m_teamIndex.teamIndex, m_playerIndex.playerIndex);


            Image[] temp_icons = temp_icon.GetComponentsInChildren<Image>();
            foreach (Image hold in temp_icons)
            {
                if (hold.name != "Fill" && hold.name != "InactivePartOverlay")
                {
                    if (active)
                    {
                        m_setOverlay.SetInactiveOverlay(false);
                        hold.gameObject.GetComponent<RectTransform>().sizeDelta +=
                            new Vector2(20f, 20f);
                    }
                    else
                    {
                        m_setOverlay.SetInactiveOverlay(true);
                        hold.gameObject.GetComponent<RectTransform>().sizeDelta +=
                            new Vector2(-20f, -20f);
                    }
                }

            }
        }
    }
}
