using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

using NaughtyAttributes;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Controls what happens once both players ready up (held by the
    /// <see cref="ReadyUpManager"/> for the
    /// <see cref="eBetterBuildSceneState.Chassis"/> portion of the BuildBot scene.
    /// Handles the transition between <see cref="eBetterBuildSceneState.Chassis"/>
    /// and <see cref="eBetterBuildSceneState.Movement"/>
    /// </summary>
    public class ChassisMoveSelectOnReadyUp : MonoBehaviour
    {
        private const bool IS_DEBUGGING = true;

        [SerializeField] private string m_chassisMoveInputMap = "ChassisAndMovement";
        [SerializeField] private string m_emptyInputMap = "None";
        [SerializeField, NaughtyAttributes.Tag] private string m_playerTag = "Player";

        // Must have all player's selectors in the correct order.
        [SerializeField]
        private DollyTargetCycler[] m_playerCyclers = new DollyTargetCycler[2];

        // Data for each option for selecting a chassis
        [SerializeField]
        private SingleChassisMoveOption[]
            m_optionList = new SingleChassisMoveOption[3];

        [SerializeField] private SelectChosenOneAnimation m_previewAnimation = null;

        [SerializeField, ReadOnly] private GameObject[] m_playerObj = new GameObject[2];
        [SerializeField] private LockedSelectionIndex[] m_lockedSelIndex =
            new LockedSelectionIndex[2];
        [SerializeField] private InputMapStack[] m_inputStack = new InputMapStack[2];

        private BetterBuildSceneStateManager m_stateMan = null;
        private ChosenPartsManager_PartSelect m_chosenPartsMan = null;
        private ReadyUpManager m_readyUpMan = null;

        private BetterBuildSceneStateChangeHandler m_chassisMoveHandler = null;
        // Index of the chosen chassis.
        private int m_chosenOneIndex = -1;
        // If we have subbed to events.
        private bool m_isSubbed = false;
        // List of callbacks so that we may unsubscribe from onflipcrushend.
        private Action<int>[] m_callbacks = new Action<int>[3];
        private int m_amountFlipCrushFin = 0;

        public IReadOnlyList<SingleChassisMoveOption> optionList => m_optionList;
        public int chosenOneIndex => m_chosenOneIndex;
        public bool shouldPlayFlip { get; set; } = true;

        /// <summary>
        /// Param - int: index of the bot whose flipcrush animation
        /// has ended.
        /// </summary>
        public event Action<int> onSingleBotFlipCrushEnd;


        // Called 1st
        // Foreign Initialization
        private void Start()
        {
            m_playerObj = GameObject.FindGameObjectsWithTag(m_playerTag);
            for (int i = 0; i < m_playerObj.Length; i++)
            {
                m_lockedSelIndex[i] = m_playerObj[i].
                    GetComponent<LockedSelectionIndex>();
                m_inputStack[i] = m_playerObj[i].
                    GetComponent<InputMapStack>();
            }

            m_readyUpMan = ReadyUpManager.instance;
            m_stateMan = BetterBuildSceneStateManager.instance;
            m_chosenPartsMan = ChosenPartsManager_PartSelect.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_readyUpMan, this);
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_stateMan, this);
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_chosenPartsMan,
                this);
            #endregion Asserts

            m_chassisMoveHandler = new BetterBuildSceneStateChangeHandler(
                m_stateMan, StartChassisMoveHandler, EndChassisMoveHandler,
                eBetterBuildSceneState.Chassis, eBetterBuildSceneState.Movement);

            // Initialize indices for chassis animators
            for (int i = 0; i < m_optionList.Length; ++i)
            {
                m_optionList[i].animCont.selectionIndex = i;
            }
        }

        private void OnDestroy()
        {
            m_chassisMoveHandler.ToggleActive(false);
        }



        public SingleChassisMoveOption GetCurrentSelectedOption()
        {
            #region Asserts
            CustomDebug.AssertIndexIsInRange(chosenOneIndex, m_optionList, this);
            #endregion Asserts
            return m_optionList[chosenOneIndex];
        }


        #region ChassisMovement StateHandler
        private void StartChassisMoveHandler()
        {
            ToggleSubscription(true);
        }
        private void EndChassisMoveHandler()
        {
            ToggleSubscription(false);
        }
        #endregion ChassisMovement StateHandler

        /// <summary>
        /// Subscribes or unsubscribes from events we are interested in.
        /// Will do nothing if the condition matches the current subscription state.
        /// </summary>
        /// <param name="cond">True - subscribe. False - unsubscribe.</param>
        private void ToggleSubscription(bool cond)
        {
            if (cond == m_isSubbed) { return; }
            m_isSubbed = cond;

            // Subscribe
            if (cond)
            {
                m_readyUpMan.onAllPlayersReady += OnAllPlayersReady;
                m_callbacks = new Action<int>[m_optionList.Length];
                for (int i = 0; i < m_optionList.Length; ++i)
                {
                    Action<int> temp_callback = OnFlipCrushEnd;
                    m_optionList[i].animCont.onFlipCrushEnd += temp_callback;
                    m_callbacks[i] = temp_callback;
                }
            }
            // Unsubscribe
            else
            {
                if (m_readyUpMan != null)
                {
                    m_readyUpMan.onAllPlayersReady -= OnAllPlayersReady;
                }
                for (int i = 0; i < m_callbacks.Length; ++i)
                {
                    Action<int> temp_callback = m_callbacks[i];
                    m_optionList[i].animCont.onFlipCrushEnd -= temp_callback;
                }
            }
        }
        /// <summary>
        /// Called when both players have readied up from
        /// <see cref="ReadyUpManager.onAllPlayersReady"/>.
        ///
        /// Starts playing transition animation from
        /// Chassis Select -> Movement Select.
        /// </summary>
        private void OnAllPlayersReady()
        {
            foreach (InputMapStack inputMapStack in m_inputStack)
            {
                inputMapStack.SwitchInputMap(m_emptyInputMap);
            }
            foreach (GameObject player in m_playerObj)
            {
                PopupStatController temp_statController =
                    player.GetComponentInChildren<PopupStatController>(true);
                if (!temp_statController.gameObject.activeSelf) { continue; }
                temp_statController.gameObject.SetActive(false);
                player.GetComponent<InputMapStack>().PopInputMap("UI");
            }
            // Decide the true chosen one from the potentially two chosen ones.
            m_chosenOneIndex = DetermineTrueChosenOne(out bool temp_didAgree,
                out int temp_botIndex);
            #region Asserts
            Assert.IsFalse(m_chosenOneIndex < 0,
                $"index of true chosen one is negative");
            Assert.IsFalse(m_chosenOneIndex >= m_optionList.Length,
                $"index of true chosen one is greater than specified animators");
            #endregion Asserts

            SendDataToChosenPartsManager(m_chosenOneIndex);

            // If there was not consensus, play the randomly choose animation.
            if (!temp_didAgree)
            {
                // TODO - Play randomly choose "animation"
                m_previewAnimation.Play(temp_botIndex);
                // The script made to do this will have a function that takes
                // in the trueChosenIndex as a param. Will start with calling
                // StartTransitionAnimationsAfterWaitCoroutine.
                // Replace line below with call to the aforementioned function.
                StartTransitionAnimationsAfterWait(1.5f);
            }
            // Skip waiting for animation.
            else
            {
                StartTransitionAnimationsAfterWait(0);
            }
        }
        /// <summary>
        /// Determines which chassis is the one that will be continued with.
        /// Since both players can potentially choose different chassis, we need
        /// to randomly pick one if they are different.
        /// </summary>
        /// <param name="areAllSame">If the players chose the same chassis.
        /// False if the players chose different chassis.</param>
        /// <returns>Index of the randomly chosen (between player choices)
        /// chassis.</returns>
        private int DetermineTrueChosenOne(out bool areAllSame,
            out int botIndex)
        {
            Assert.AreNotEqual(0, m_playerCyclers.Length, $"No player cyclers " +
                $"were specified in {name}'s {GetType().Name}");

            areAllSame = true;

            if (m_lockedSelIndex[0].selectionIndex != m_lockedSelIndex[1].
                selectionIndex)
            {
                areAllSame = false;
            }

            int temp_trueChosenPosition = UnityEngine.Random.Range(0,
                m_playerCyclers.Length);
            botIndex = temp_trueChosenPosition;
            return m_lockedSelIndex[temp_trueChosenPosition].selectionIndex;
        }
        private void SendDataToChosenPartsManager(int chosenIndex)
        {
            switch (m_stateMan.curState)
            {
                case eBetterBuildSceneState.Chassis:
                    {
                        m_chosenPartsMan.SetChassis(
                            m_optionList[chosenIndex].chassisID);
                        break;
                    }
                case eBetterBuildSceneState.Movement:
                    {
                        m_chosenPartsMan.SetMovementPart(
                            m_optionList[chosenIndex].movementID);
                        break;
                    }
                default:
                    CustomDebug.UnhandledEnum(m_stateMan.curState, this);
                    break;
            }
        }
        /// <summary>
        /// Starts a coroutine that will wait the specified time before starting
        /// the transition animations to allow for the small animation that plays
        /// if both players chose different chassis.
        /// </summary>
        /// <param name="waitTime">Time to wait before playing transition animation.
        /// Should be either 0 or the amount of time the small
        /// "random choose animation."</param>
        private void StartTransitionAnimationsAfterWait(float waitTime)
        {
            StartCoroutine(
                TransitionAnimationsAfterWaitCoroutine(waitTime));
        }
        private IEnumerator TransitionAnimationsAfterWaitCoroutine(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            StartTransitionAnimations();
        }
        /// <summary>
        /// Starts the transition animations for going from
        /// Chassis Selection -> Movement Selection.
        /// </summary>
        private void StartTransitionAnimations()
        {
            // Crush the non-chosen ones.
            for (int i = 0; i < m_optionList.Length; ++i)
            {
                // Chosen one doesn't get crushed
                if (i == m_chosenOneIndex) { continue; }

                m_optionList[i].animCont.PlayCrushAnimation();
            }
            // Flip the chosen one.
            if (shouldPlayFlip)
            {
                m_optionList[m_chosenOneIndex].animCont.PlayFlipAnimation();
            }

            // When the bots have been fully crushed/ are offscreen (flipped),
            // then spawn the movement options and play their intro animations.
            // This will be handled via subscription to the onFlipCrushEnd event.
        }
        /// <summary>
        /// Called by <see cref="BetterBuildBotAnimatorController.onFlipCrushEnd"/>
        /// when the crush/flip animation has reached the point where the new bots
        /// should be spawned.
        /// </summary>
        /// <param name="index">Index for which chassis finished their flip/crush
        /// animation.</param>
        private void OnFlipCrushEnd(int index)
        {
            onSingleBotFlipCrushEnd?.Invoke(index);

            int temp_animsToWaitFor = shouldPlayFlip ? m_optionList.Length :
                m_optionList.Length - 1;

            // After uncrush, advance state, if all have finished.
            if (++m_amountFlipCrushFin >= temp_animsToWaitFor)
            {
                StartCoroutine(WaitBeforeAdvanceCoroutine());
                m_amountFlipCrushFin = 0;
            }
        }
        private IEnumerator WaitBeforeAdvanceCoroutine()
        {
            // Gross hack to fix destroy needing a frame to actually destroy things
            yield return null;
            yield return null;
            BetterBuildSceneStateManager.instance.AdvanceState();

            foreach (InputMapStack inputMapStack in m_inputStack)
            {
                inputMapStack.PopInputMap(m_emptyInputMap);
            }
        }
    }
}
