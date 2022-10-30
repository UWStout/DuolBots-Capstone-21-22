using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

using NaughtyAttributes;

using DuolBots.Mirror;
// Original Authors - Wyatt Senalik, Aaron Duffey, and Zachary Gross

namespace DuolBots
{
    /// <summary>
    /// Passes along input calls from PlayerInput to IRobotInputController.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerIndex))]
    public class PlayerRobotInputController : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;
        private static readonly List<eInputType> DEBUGGING_INPUT_TYPE_LIST
            = new List<eInputType>()
        {
            eInputType.buttonEast,
            eInputType.buttonNorth,
            eInputType.buttonSouth,
            eInputType.buttonWest,
            eInputType.dPad_Down,
            eInputType.dPad_Left,
            eInputType.dPad_Right,
            eInputType.dPad_Up,
            eInputType.leftStickPress,
            eInputType.rightStickPress,
            eInputType.select,
            eInputType.start,
            //eInputType.leftStick_X,
            //eInputType.leftStick_Y,
            //eInputType.rightStick_Y,
            //eInputType.rightStick_Y
            eInputType.leftShoulder,
            eInputType.rightShoulder
        };
        private const float CHANGE_SLOT_DEADZONE = 0.15f;

        // Tag that is on the root of all robots
        [SerializeField] [Tag] private string m_robotTag = "Robot";
        // When to look for the robots in the scene
        private enum eFindRobotAtTime { OnBotCreate, OnStart, OnStateMan };
        [SerializeField]
        private eFindRobotAtTime m_findRobotAtTime
            = eFindRobotAtTime.OnBotCreate;

        // References
        // Singleton helper
        private RobotHelpersSingleton m_robotHelpers = null;
        // Singleton State Manager
        private BattleStateManager m_battleStateMan = null;
        // This player's team's robot input controller
        private IRobotInputController m_robotInputController = null;
        // This player's team's robot's slot placement manager
        private SlotPlacementManager m_slotPlaceMan = null;
        // If this player is player one or two
        private PlayerIndex m_playerIndex = null;
        // Which team this player is on
        private ITeamIndex m_teamIndex = null;

        private BattleStateChangeHandler m_battleHandler = null;
        private bool m_isAcceptingInput = false;
        private byte m_curSelectedSlot = 0;
        // Indicies of slots contianing parts
        private IReadOnlyList<int> m_usedSlotIndicies = null;
        // Current index of the used slot indicies list
        private int m_curSlotIndiciesListIndex = 0;
        private IEnumerable<CustomInputBinding> m_inputs = null;

        // Slot that is currently selected
        public byte curSelectedSlot
        {
            get => m_curSelectedSlot;
            private set
            {
                m_curSelectedSlot = value;
                CustomDebug.Log($"Currently Selected Slot updated to " +
                    $"{m_curSelectedSlot}", IS_DEBUGGING);
                onSlotSelectedChanged?.Invoke(m_curSelectedSlot);
            }
        }
        /// <summary>
        /// Index that iterates through the slots from 0 to (num of parts)
        /// attached no matter which slots actually have parts.
        /// Example: Whale bot has parts only in slots 2 and 4. Slot 2's index is now
        /// 0 and Slot 4's index is now 1.
        /// </summary>
        public int curSlotIndiciesListIndex => m_curSlotIndiciesListIndex;

        public event Action<byte> onSlotSelectedChanged;
        /// <summary>
        /// Parameter is the <see cref="m_curSlotIndiciesListIndex"/>.
        /// Iterates through the slots from 0 to (num of parts) attached no matter
        /// which slots actually have parts.
        /// Example: Whale bot has parts only in slots 2 and 4. Slot 2's index is now
        /// 0 and Slot 4's index is now 1.
        /// </summary>
        public event Action<byte> onIndexIterated;


        // Domestic Initialization
        private void Awake()
        {
            m_playerIndex = GetComponent<PlayerIndex>();
            m_teamIndex = GetComponent<ITeamIndex>();
            // We can do this in Awake instead of Start because
            // RobotHelpersSingleton is a dynamic singleton that will create itself.
            m_robotHelpers = RobotHelpersSingleton.instance;

            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_playerIndex, this);
            CustomDebug.AssertIComponentIsNotNull(m_teamIndex, this);

            CustomDebug.AssertDynamicSingletonMonoBehaviourPersistantIsNotNull(
                m_robotHelpers, this);

            // PlayerInput should be attached since this uses Input Messages
            PlayerInput temp_playerInput = GetComponent<PlayerInput>();
            CustomDebug.AssertComponentIsNotNull(temp_playerInput, this);
            #endregion Asserts
        }
        // Foreign Initialization
        private void Start()
        {
            if (m_findRobotAtTime == eFindRobotAtTime.OnStateMan)
            {
                m_battleStateMan = BattleStateManager.instance;
                #region Asserts
                CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_battleStateMan,
                    this);
                #endregion Asserts
                m_battleHandler = new BattleStateChangeHandler(m_battleStateMan,
                    HandleBattleBegin, HandleBattleEnd, eBattleState.Battle);
            }

            if (m_findRobotAtTime == eFindRobotAtTime.OnStart)
            {
                FindBotOnSameTeamLocal(m_teamIndex.teamIndex);
            }

            m_inputs = BuildSceneInputData.GetInputBindingsForPlayer(0).Where(X => X.playerIndex == m_playerIndex.playerIndex);
        }
        // Called when the component is activated.
        private void OnEnable()
        {
            if (m_findRobotAtTime == eFindRobotAtTime.OnBotCreate)
            {
                BattleBotInstantiator.onCreateBotFinished
                    += FindBotOnSameTeamLocal;
            }
        }
        // Called when the component is de-activated.
        private void OnDisable()
        {
            if (m_findRobotAtTime == eFindRobotAtTime.OnBotCreate)
            {
                BattleBotInstantiator.onCreateBotFinished
                    -= FindBotOnSameTeamLocal;
            }
        }


        /// <summary>
        /// Get a slot transform from the selection index.
        /// </summary>
        /// <param name="selectionIndex">Index in <see cref="m_usedSlotIndicies"/>.
        /// Is NOT the index of the slot.</param>
        /// <returns></returns>
        public Transform GetSlotTransformFromSelection(int selectionIndex)
        {
            #region Asserts
            CustomDebug.AssertIndexIsInRange(selectionIndex,
                m_usedSlotIndicies, this);
            #endregion Asserts

            int temp_partIndex = m_usedSlotIndicies[selectionIndex];
            return m_slotPlaceMan.GetSlotTransform(temp_partIndex);
        }


        /// <summary>
        /// Called when battle state is changed to.
        /// Finds bot and initializes references to various scripts on the bot.
        /// Starts accepting input.
        /// </summary>
        private void HandleBattleBegin()
        {
            FindBotOnSameTeamNetwork();
            m_isAcceptingInput = true;
        }
        /// <summary>
        /// Called when battle state is changed from.
        /// Stops accepting input.
        /// </summary>
        private void HandleBattleEnd()
        {
            // No longer accepting input
            ReleaseControlsOnCurrentPart(m_curSelectedSlot);
            ReleaseControlsOnCurrentPart(PartSlotIndex.MOVEMENT_PART_SLOT_ID);
            m_isAcceptingInput = false;
        }

        /// <summary>
        /// Called by <see cref="NetworkBotInstantiator"/>'s
        /// onAllBotsInstantiated event.
        /// 
        /// Finds the robot on the same team as this player and caches its
        /// IRobotInputController.
        /// </summary>
        private void FindBotOnSameTeamNetwork()
        {
            #region Logs
            CustomDebug.Log($"Player {m_playerIndex.playerIndex} " +
                $"{nameof(FindBotOnSameTeamNetwork)}", IS_DEBUGGING);
            #endregion Logs
            FindBotOnSameTeamLocal(m_teamIndex.teamIndex);
        }
        /// <summary>
        /// Called by <see cref="BattleBotInstantiator.onCreateBotFinished"/>,
        /// on Start if <see cref="m_findRobotAtTime"/> is set to
        /// <see cref="eFindRobotAtTime.OnStart"/>, and by
        /// <see cref="FindBotOnSameTeamNetwork"/> if
        /// <see cref="m_findRobotAtTime"/> is set to
        /// <see cref="eFindRobotAtTime.OnAllBotsCreated"/>.
        /// 
        /// Finds the robot on the same team as this player and caches its
        /// IRobotInputController.
        /// </summary>
        private void FindBotOnSameTeamLocal(byte botTeamIndex)
        {
            if (botTeamIndex != m_teamIndex.teamIndex) { return; }

            #region Asserts
            Assert.IsNotNull(m_robotHelpers, $"{nameof(m_robotHelpers)} has not " +
               $"yet been assigned, but {name}'s {GetType().Name} needs to use " +
               $"it.");
            #endregion Asserts
            GameObject temp_myRobot = m_robotHelpers.FindBotRoot(botTeamIndex);
            m_robotInputController
                = temp_myRobot.GetComponent<IRobotInputController>();
            m_slotPlaceMan
                = temp_myRobot.GetComponentInChildren<SlotPlacementManager>();

            #region Asserts
            CustomDebug.AssertIComponentOnOtherIsNotNull(m_robotInputController,
                temp_myRobot, this);
            CustomDebug.AssertComponentInChildrenOnOtherIsNotNull(m_slotPlaceMan,
                temp_myRobot, this);
            #endregion Asserts

            m_usedSlotIndicies = m_slotPlaceMan.GetUsedSlotsIndices();
            // Start the player on a slot (player 1 gets the lowest,
            // player 2 gets the highest).
            if (m_usedSlotIndicies.Count > 0)
            {
                int temp_index = m_playerIndex.playerIndex == 0 ?
                    0 : m_usedSlotIndicies.Count - 1;

                SetCurSelectSlotFromSlotIndiciesList(temp_index);
            }
        }
        /// <summary>
        /// Updates the currently selected slot index to be the index specified
        /// at the given position in the used slot indicies list.
        /// </summary>
        /// <param name="indiciesListPos">Pos (index) in the indicies list of the
        /// value that the current slot index should be.</param>
        private void SetCurSelectSlotFromSlotIndiciesList(int indiciesListPos)
        {
            #region Asserts
            CustomDebug.AssertIndexIsInRange(indiciesListPos,
                m_usedSlotIndicies, this);
            #endregion Asserts

            // Update the currently selected slot index to reflect
            m_curSlotIndiciesListIndex = indiciesListPos;
            curSelectedSlot = (byte)m_usedSlotIndicies[indiciesListPos];
            onIndexIterated?.Invoke((byte)indiciesListPos);
        }

        #region PlayerInput Messages

        #region GenericControls
        // These each are called by Unity's new PlayerInput's messages.
        // They all just call IRobotInputController's OnPlayerInput function, but change the eInputType.
        //
        // Pre Conditions - Assumes that there is a PlayerInput component attached to this GameObject.
        // Assumes active input map is an input map that has actions
        // with the exact same names as the below functions without the 'On' prefix.
        // Post Conditions - Passes the input information along to the IRobotInputController
        // using the OnInput function.
        private void OnButtonEast(InputValue value) => OnInput(value, eInputType.buttonEast);
        private void OnButtonWest(InputValue value) => OnInput(value, eInputType.buttonWest);
        private void OnButtonSouth(InputValue value) => OnInput(value, eInputType.buttonSouth);
        private void OnButtonNorth(InputValue value) => OnInput(value, eInputType.buttonNorth);
        private void OnDPad(InputValue value) => OnInput(value, eInputType.dPad);
        private void OnDPadDown(InputValue value) => OnInput(value, eInputType.dPad_Down);
        private void OnDPadUp(InputValue value) => OnInput(value, eInputType.dPad_Up);
        private void OnDPadLeft(InputValue value) => OnInput(value, eInputType.dPad_Left);
        private void OnDPadRight(InputValue value) => OnInput(value, eInputType.dPad_Right);
        private void OnDPadX(InputValue value) => OnInput(value, eInputType.dPad_X);
        private void OnDPadY(InputValue value) => OnInput(value, eInputType.dPad_Y);
        private void OnLeftShoulder(InputValue value) => OnInput(value, eInputType.leftShoulder);
        private void OnRightShoulder(InputValue value) => OnInput(value, eInputType.rightShoulder);
        private void OnLeftTrigger(InputValue value) => OnInput(value, eInputType.leftTrigger);
        private void OnRightTrigger(InputValue value) => OnInput(value, eInputType.rightTrigger);
        private void OnLeftStick(InputValue value) => OnInput(value, eInputType.leftStick);
        private void OnLeftStickDown(InputValue value) => OnInput(value, eInputType.leftStick_Down);
        private void OnLeftStickUp(InputValue value) => OnInput(value, eInputType.leftStick_Up);
        private void OnLeftStickRight(InputValue value) => OnInput(value, eInputType.leftStick_Right);
        private void OnLeftStickLeft(InputValue value) => OnInput(value, eInputType.leftStick_Left);
        private void OnLeftStickX(InputValue value) => OnInput(value, eInputType.leftStick_X);
        private void OnLeftStickY(InputValue value) => OnInput(value, eInputType.leftStick_Y);
        private void OnLeftStickPress(InputValue value) => OnInput(value, eInputType.leftStickPress);
        private void OnRightStick(InputValue value) => OnInput(value, eInputType.rightStick);
        private void OnRightStickDown(InputValue value) => OnInput(value, eInputType.rightStick_Down);
        private void OnRightStickUp(InputValue value) => OnInput(value, eInputType.rightStick_Up);
        private void OnRightStickRight(InputValue value) => OnInput(value, eInputType.rightStick_Right);
        private void OnRightStickLeft(InputValue value) => OnInput(value, eInputType.rightStick_Left);
        private void OnRightStickX(InputValue value) => OnInput(value, eInputType.rightStick_X);
        private void OnRightStickY(InputValue value) => OnInput(value, eInputType.rightStick_Y);
        private void OnRightStickPress(InputValue value) => OnInput(value, eInputType.rightStickPress);
        private void OnSelect(InputValue value) => OnInput(value, eInputType.select);
        private void OnStart(InputValue value) => OnInput(value, eInputType.start);
        private void OnTriggerAxis(InputValue value) => OnInput(value, eInputType.triggerAxis);
        private void OnShoulderAxis(InputValue value) => OnInput(value, eInputType.shoulderAxis);
        private void OnButtons(InputValue value) => OnInput(value, eInputType.buttons);
        private void OnButtonsX(InputValue value) => OnInput(value, eInputType.buttons_X);
        private void OnButtonsY(InputValue value) => OnInput(value, eInputType.buttons_Y);

        /// <summary>
        /// Calls OnPlayerInput for the given input type.
        ///
        /// Pre Conditions - Assumes m_robotInputController is not null. Assumes
        /// m_amPlayerOne accurately reflects if the current player is player one
        /// or player two.
        /// Post Conditions - Passes the input information along to the IRobotInputController
        /// using its OnPlayerInput function.
        /// </summary>
        /// <param name="inputType">Type of input.</param>
        private void OnInput(InputValue value, eInputType inputType)
        {
            // Not accepting input
            if (!m_isAcceptingInput) { return; }

            #region Logs
            CustomDebug.Log($"{inputType} Input. Get Data: {value.Get()} for " +
                $"team {m_teamIndex.teamIndex}.",
                IS_DEBUGGING && (DEBUGGING_INPUT_TYPE_LIST.Contains(inputType)));
            #endregion Logs

            if (m_robotInputController == null) { return; }

            m_robotInputController.OnPlayerInput(m_playerIndex.playerIndex,
                inputType, curSelectedSlot, new CustomInputData(value, inputType));
        }
        #endregion GenericControls

        private void OnChangeSlot(InputValue value)
        {
            // Not accepting input
            if (!m_isAcceptingInput) { return; }

            // Expecting an axis [-1, 1]
            float temp_axisData = value.Get<float>();

            // Change slot to the left
            if (temp_axisData < -CHANGE_SLOT_DEADZONE)
            {
                CycleCurSlotIndex(false);
            }
            // Change slot to the right
            else if (temp_axisData > CHANGE_SLOT_DEADZONE)
            {
                CycleCurSlotIndex(true);
            }
        }
        #endregion PlayerInput Messages

        /// <summary>
        /// Cycle the currently selected slot to the left or right.
        /// </summary>
        /// <param name="toLeftOrRight">True - Cycle Right (positive).
        /// False - Cycle Left (negative).</param>
        private void CycleCurSlotIndex(bool toLeftOrRight)
        {

            ReleaseControlsOnCurrentPart(m_curSelectedSlot);
            // Determine if we are going left (false) or right (true)
            int temp_inc = toLeftOrRight ? -1 : 1;
         
            // Unwrapped index
            int temp_incIndex = m_curSlotIndiciesListIndex + temp_inc;
 
            // Wrap the index if it went out of bounds
            if (temp_incIndex < 0) { temp_incIndex = m_usedSlotIndicies.Count - 1; }
            if (temp_incIndex >= m_usedSlotIndicies.Count) { temp_incIndex = 0; }

            // Save that wrapped index
            m_curSlotIndiciesListIndex = temp_incIndex;
       


            SetCurSelectSlotFromSlotIndiciesList(m_curSlotIndiciesListIndex);
        }
        /// <summary>
        /// This passes a fake release input to all of this players controls on the currnet index.
        /// </summary>
        private void ReleaseControlsOnCurrentPart(byte CurSlot)
        {
            foreach (CustomInputBinding CIB in m_inputs)
            {
                if (CIB.partSlotID == CurSlot)
                {
                    m_robotInputController.OnPlayerInput(m_playerIndex.playerIndex, CIB.inputType, CurSlot, new CustomInputData(0.0f, false, CIB.inputType));
                }
            }
        }
    }
}
