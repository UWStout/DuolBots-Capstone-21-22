using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using Cinemachine;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Handles transitioning from the movement state to the part select state.
    /// </summary>
    public class HandleEndMovementState : MonoBehaviour
    {
        [SerializeField] [Required] private ChassisMoveSelectOnReadyUp
            m_chassisMoveOnReadyUp = null;
        [SerializeField] [Required] private BuildUICameraSystem m_camSys = null;

        private BetterBuildSceneStateManager m_stateMan = null;
        private ReadyUpManager m_readyUpMan = null;

        private BetterBuildSceneStateChangeHandler m_moveStateHandler = null;
        private bool m_isSubbed = false;


        // Domestic Initialization
        private void Awake()
        {
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_chassisMoveOnReadyUp,
                nameof(m_chassisMoveOnReadyUp), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_camSys, nameof(m_camSys),
                this);
            #endregion Asserts
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
            m_moveStateHandler = new BetterBuildSceneStateChangeHandler(
                m_stateMan, null, HandleMoveStateEnd, eBetterBuildSceneState.Movement);
        }

        private void OnDestroy()
        {
            ToggleSubscription(false);

            m_moveStateHandler.ToggleActive(false);
        }


        /// <summary>
        /// Turns off the functionality of the HandleEndMovementState script.
        /// Most importantly, toggles subscriptions to the onSingleBotFlipCrushEnd
        /// event.
        /// </summary>
        public void ToggleEnabled(bool cond)
        {
            if (cond)
            {
                m_chassisMoveOnReadyUp.shouldPlayFlip = false;
            }
            ToggleSubscription(cond);
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
        /// <summary>
        /// Called when a single bot finished its crush animation (no flips
        /// because no one is flipped in the movement select).
        ///
        /// Destroy the bot that finished being crushed.
        /// </summary>
        /// <param name="index">Index of the bot that finished its anim.</param>
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
            GameObject temp_movementRoot = temp_optionList[index].movementBotRoot;
            #region Asserts
            Assert.IsNotNull(temp_movementRoot,
                $"Original chassis at index {index} was already destroyed or was " +
                $"never specified");
            #endregion Asserts
            // Destroy bot at the current index if not chosen one.
            if (m_chassisMoveOnReadyUp.chosenOneIndex != index)
            {
                Destroy(temp_movementRoot);
            }
        }
        /// <summary>
        /// Called when the state of the build ui is changed.
        /// Only functions if the state was changed FROM the movement state.
        /// 
        /// Turns off the chassis/movement stuff and turns on the part select stuff.
        /// </summary>
        private void HandleMoveStateEnd()
        {
            // TODO change to part selection
            // 1. Change camera to part select camera (done)
            // 2. Turn off the UI for chassis/move select (started, not done)
            // 3. Turn on the UI for the part select

            // Change camera
            m_camSys.ChangeToPartSelectCamera();
            PairCamera<CinemachineVirtualCamera> temp_partSelCams
                = m_camSys.partSelectCameras;
            SetSinglePlayerBotRoot(temp_partSelCams.playerZeroCam.gameObject);
            SetSinglePlayerBotRoot(temp_partSelCams.playerOneCam.gameObject);
        }
        /// <summary>
        /// Initializes the <see cref="SlotViewer"/> attached to the
        /// given player camera object to have the chosen bot root
        /// and have its <see cref="DollyTargetCycler"/> controlled by
        /// the corresponding player's input.
        /// </summary>
        private void SetSinglePlayerBotRoot(GameObject playerCamObj)
        {
            // Get the SlotViewer
            SlotViewer temp_slotViewer
                = playerCamObj.GetComponentInParent<SlotViewer>();
            #region Asserts
            CustomDebug.AssertComponentOnOtherIsNotNull(
                temp_slotViewer, playerCamObj, this);
            #endregion Asserts

            // Find the chosen bot root
            SingleChassisMoveOption temp_chosenOption
                = m_chassisMoveOnReadyUp.GetCurrentSelectedOption();
            GameObject temp_chosenMoveRoot = temp_chosenOption.movementBotRoot;
            #region Asserts
            Assert.IsNotNull(temp_chosenMoveRoot, $"{name}'s {GetType().Name} " +
                $"assumed that the movementBotRoot for the chosenOption to not " +
                $"be null, but it was.");
            #endregion Asserts
            // Give the slot viewer the bot root and set it active.
            temp_slotViewer.SetBotRoot(temp_chosenMoveRoot);
            temp_slotViewer.ToggleActive(true);

            // Find the corresponding input to control the current player's cycler
            Input_DollyTargetCycler temp_singlePlayerInput =
                FindDollyCyclerPlayerInput(temp_slotViewer.playerIndex);
            // Set the active part cycler
            temp_singlePlayerInput.dollyTargetCycler = temp_slotViewer.cycler;
        }
        /// <summary>
        /// Simply finds the <see cref="Input_DollyTargetCycler"/> in the
        /// scene with the specified player index
        /// </summary>
        private Input_DollyTargetCycler FindDollyCyclerPlayerInput(byte playerIndex)
        {
            Input_DollyTargetCycler[] temp_playerInputs =
                FindObjectsOfType<Input_DollyTargetCycler>();
            #region Asserts
            Assert.AreNotEqual(0, temp_playerInputs.Length,
                $"{name}'s {GetType().Name} expected to find at least one" +
                $"{nameof(Input_DollyTargetCycler)} in the scene, but none " +
                $"were found.");
            #endregion Asserts

            for (int i = 0; i < temp_playerInputs.Length; ++i)
            {
                Input_DollyTargetCycler temp_singlePlayerInput =
                    temp_playerInputs[i];
                PlayerIndex temp_playerIndexFromInput =
                    temp_singlePlayerInput.GetComponent<PlayerIndex>();
                #region Asserts
                CustomDebug.AssertComponentOnOtherIsNotNull(temp_playerIndexFromInput,
                    temp_singlePlayerInput.gameObject, this);
                #endregion Asserts
                if (temp_playerIndexFromInput.playerIndex == playerIndex)
                {
                    return temp_singlePlayerInput;
                }
            }

            Debug.LogError($"No {nameof(Input_DollyTargetCycler)} was found " +
                $"for player index {playerIndex}");
            return null;
        }
    }
}
