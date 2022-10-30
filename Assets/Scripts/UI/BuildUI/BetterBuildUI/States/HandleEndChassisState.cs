using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Handles ending the chassis selection state.
    /// </summary>
    public class HandleEndChassisState : MonoBehaviour
    {
        [SerializeField] [Required] private ChassisMoveSelectOnReadyUp
            m_chassisMoveOnReadyUp = null;
        [SerializeField] private float m_dropOffset = 20;
        [SerializeField] [Required] private HandleEndMovementState
            m_handleEndMoveState = null;

        private BetterBuildSceneStateManager m_stateMan = null;
        private ReadyUpManager m_readyUpMan = null;

        private BetterBuildSceneStateChangeHandler m_chassisStateHandler = null;
        private bool m_isSubbed = false;


        // Sub to events
        private void OnEnable()
        {
            ToggleSubscription(true);
        }
        // Unsub from events
        private void OnDisable()
        {
            ToggleSubscription(false);
        }
        // Foreign Initialization
        private void Start()
        {
            m_stateMan = BetterBuildSceneStateManager.instance;
            m_readyUpMan = ReadyUpManager.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_stateMan, this);
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_readyUpMan, this);
            #endregion Asserts

            m_chassisStateHandler = new BetterBuildSceneStateChangeHandler(
                m_stateMan, null, HandleChassisStateEnd,
                eBetterBuildSceneState.Chassis);
        }
        private void OnDestroy()
        {
            m_chassisStateHandler.ToggleActive(false);
        }


        private void HandleChassisStateEnd()
        {
            // TODO get these from a better location?
            // Unlock the preview images
            PreviewImageController[] temp_previewImgContList
                = FindObjectsOfType<PreviewImageController>();

            for (int i = 0; i < temp_previewImgContList.Length; ++i)
            {
                PreviewImageController temp_imgCont = temp_previewImgContList[i];
                temp_imgCont.SetIsSilhouette(true);
                temp_imgCont.SetImageState(eImageState.Movement);
                temp_imgCont.LockImage(false);
                temp_imgCont.UpdateImageToCurrentSelection();
            }

            // Turn off the ChassisSelector specific stuff (this)
            ToggleSubscription(false);
            // Turn on the Movement specific stuff
            m_handleEndMoveState.ToggleEnabled(true);
        }
        private void ToggleSubscription(bool cond)
        {
            if (cond == m_isSubbed) { return; }
            m_isSubbed = cond;

            if (cond)
            {
                m_chassisMoveOnReadyUp.onSingleBotFlipCrushEnd
                    += OnSingleBotFlipCrushEnd;
            }
            else
            {
                if (m_chassisMoveOnReadyUp != null)
                {
                    m_chassisMoveOnReadyUp.onSingleBotFlipCrushEnd
                        -= OnSingleBotFlipCrushEnd;
                }
            }
        }

        private void OnSingleBotFlipCrushEnd(int index)
        {
            IReadOnlyList<SingleChassisMoveOption> temp_optionList
                = m_chassisMoveOnReadyUp.optionList;

            #region Asserts
            Assert.IsTrue(index >= 0 && index < temp_optionList.Count,
                $"Index for {name}'s {GetType().Name}'s " +
                $"{nameof(OnSingleBotFlipCrushEnd)} is out of range for " +
                $"{nameof(temp_optionList)}. Expected to be in range [0, " +
                $"{temp_optionList.Count - 1}]");
            #endregion Asserts
            GameObject temp_chassisRoot = temp_optionList[index].chassisBotRoot;
            #region Asserts
            Assert.IsNotNull(temp_chassisRoot,
                $"Original chassis at index {index} was already destroyed or was " +
                $"never specified");
            #endregion Asserts
            // Destroy bot at the current index
            Destroy(temp_chassisRoot);

            // Instantiate new bot
            GameObject temp_moveOpt = SpawnMovementOption(index);
            temp_optionList[index].movementBotRoot = temp_moveOpt;

            // Drop from sky if chosen one
            if (index == m_chassisMoveOnReadyUp.chosenOneIndex)
            {
                GameObject temp_moveBotRoot = temp_optionList[index].
                    instantiator.botUnderConstr.currentBotRoot;
                Vector3 temp_pos = temp_moveBotRoot.transform.position;
                temp_pos.y += m_dropOffset;
                temp_moveBotRoot.transform.position = temp_pos;
            }
            // If not chosen one, start crushed
            else
            {
                temp_moveOpt.transform.localScale = new Vector3(1, 0, 1);
            }
        }
        /// <summary>
        /// Spawns a new bot that uses the chosen chassis but has movement
        /// parts corresponding to the index and the serialized movementIDs.
        /// </summary>
        /// <param name="index">Index corresponding to the position of the selection
        /// that a movement part bot should be created for.</param>
        /// <returns>The current bot root of the newly created movement part.
        /// </returns>
        private GameObject SpawnMovementOption(int index)
        {
            IReadOnlyList<SingleChassisMoveOption> temp_optionList
                = m_chassisMoveOnReadyUp.optionList;

            #region Asserts
            Assert.IsTrue(index >= 0 && index < temp_optionList.Count,
               $"Index for {name}'s {GetType().Name}'s " +
               $"{nameof(SpawnMovementOption)} is out of range for " +
               $"{nameof(temp_optionList)}. Expected to be in range [0, " +
               $"{temp_optionList.Count - 1}]");
            #endregion Asserts
            SingleChassisMoveOption temp_chosenOption
                = temp_optionList[m_chassisMoveOnReadyUp.chosenOneIndex];
            SingleChassisMoveOption temp_moveOption
                = temp_optionList[index];

            // Figure out chosen chassis
            string temp_chosenChassisID = temp_chosenOption.chassisID;

            BetterBuildUIBotInstantiator temp_botInst = temp_moveOption.instantiator;
            #region Asserts
            Assert.IsNotNull(temp_botInst,
                $"{nameof(BetterBuildUIBotInstantiator)} at {index} was " +
                $"not specified");
            #endregion Asserts
            // Step 2: Create the new movement part bot
            temp_botInst.CreateBotRoot();
            temp_botInst.CreateChassis(temp_chosenChassisID);
            // Figure out which movement part THIS
            // chassis needs to have.
            temp_botInst.CreateMovementPart(temp_moveOption.movementID);

            return temp_botInst.botUnderConstr.currentBotRoot;
        }
    }
}
