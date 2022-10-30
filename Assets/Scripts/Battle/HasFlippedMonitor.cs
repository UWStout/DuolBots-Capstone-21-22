using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

using NaughtyAttributes;

namespace DuolBots
{
    /// <summary>
    /// Meant to be attached to the bot root.
    /// Re-aligns the bot when it is flipped over.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(HasFlippedMonitorIcons))]
    public class HasFlippedMonitor : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = true;

        [Tooltip("The amount of speed (squared) the attached rigidboby is allowed " +
            "to have and still be considered to be stopped.")]
        [SerializeField] [Min(0.0f)] private float m_velocityNearZeroTolerance = 0.9f; // CHANGE ON THE PREFAB AS NECESSARY
        [Tooltip("Time in seconds the bot must be unmoving and uncontrollable before " +
            "being considered flipped.")]
        [SerializeField] [Min(0.0f)] private float m_timeUntilConsideredFlipped = 1.2f; // CHANGE ON THE PREFAB AS NECESSARY

        [Tooltip("Curve for controlling how the flipping is lerped.")]
        [SerializeField] private AnimationCurve m_flipVelCurve = new AnimationCurve();
        [Tooltip("Scalar for further control on exactly how much to lerp the flip.")]
        [SerializeField] [Min(0.00001f)] private float m_flipVelScalar = 1.0f;
        [Tooltip("Ground/Floor layer to use to place the bot on for flipping")]
        [SerializeField] private LayerMask m_groundLayerMask = 0;

        private GodHandSingleton m_godHandSingleton = null;

        // References
        private Rigidbody m_rb = null;
        private ITeamIndex m_teamIndex = null;
        private SharedController_Movement m_sharedMoveCont = null;
        private MovementPartPlacementManager m_movementPartPlacementMan = null;
        private HasFlippedMonitorIcons m_hfmIcons = null;

        // True if the initializing of a flip still needs to be done
        private bool m_needBeginFlip = true;

        // How long the bot has been unmoving and uncontrollable
        private float m_flippedTime = 0.0f;
        // If the bot is currently flipping
        private bool m_isFlipping = false;

        // Reference to the currently running flip coroutine
        private Coroutine m_activeFlipCoroutine = null;

        public event Action<byte> onBotHasFlipped;
        

        // Called 0th
        private void Awake()
        {
            m_rb = GetComponent<Rigidbody>();
            m_teamIndex = GetComponent<ITeamIndex>();
            m_hfmIcons = GetComponent<HasFlippedMonitorIcons>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_rb, this);
            CustomDebug.AssertIComponentIsNotNull(m_teamIndex, this);
            CustomDebug.AssertComponentIsNotNull(m_hfmIcons, this);
            #endregion Asserts
        }
        // Called 1st
        private void Start()
        {
            // Getting part references MUST be done in Start. If it is done in Awake, it will be
            // done before the part exists (potentially).
            m_sharedMoveCont = GetComponentInChildren<SharedController_Movement>();
            Assert.IsNotNull(m_sharedMoveCont, $"{nameof(HasFlippedMonitor)} on {name} requires a" +
                $" {nameof(SharedController_Movement)} attached to one of its children," +
                $" but it was not found.");

            m_movementPartPlacementMan = GetComponentInChildren<MovementPartPlacementManager>();
            Assert.IsNotNull(m_movementPartPlacementMan, $"{nameof(HasFlippedMonitor)} on {name}" +
                $" requires a {nameof(MovementPartPlacementManager)} attached to one of " +
                $"its children, but it was not found.");

            

            m_godHandSingleton = GodHandSingleton.Instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_godHandSingleton,
                this);
            #endregion Asserts
        }
        // Called once every frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Backslash))
            {
                Debug.Log($"Is Grounded: {IsBotGrounded()}");
                Debug.Log($"Threshold: {m_velocityNearZeroTolerance}");
                Debug.Log($"Current Velocity: {m_rb.velocity.magnitude}");
                Debug.Log($"Current Flip Charge: {m_sharedMoveCont.FlipCharge}");
            }
            // TODO - REMOVE.
            // TEMP so that if people get stuck at sgx, we can flip them.
            if (Input.GetKeyDown(KeyCode.O))
            {
                FlipBot();
            }
            // Kinda sucks this has to be in update, but otherwise we need to have events built into
            // the Rigidbody for when the bot's velocity reaches close to 0 AND and event
            // for the SharedController_Movement for when all 4 move parts are not grounded.
            HandleBotFlippedUpdate();
        }


        /// <summary>
        /// Checks if the bot is flipped and flips the bot accordingly.
        /// </summary>
        private void HandleBotFlippedUpdate()
        {
            // Bot hasn't flipped
            if (!IsBotFlipped())
            {
                m_sharedMoveCont.RequireFlippedInput(false);
                m_needBeginFlip = true;
                m_hfmIcons.TogglePromptsActive(false, m_teamIndex.teamIndex);
                return;
            }

            // Only do this once
            if (m_needBeginFlip)
            {
                // Tell SharedController_Movement that movement spam is needed
                m_sharedMoveCont.RequireFlippedInput(true);
                CustomDebug.Log($"Bot {name} has flipped. BotStopped={IsBotStopped()}." +
                        $" BotGrounded={IsBotGrounded()}", IS_DEBUGGING);

                m_hfmIcons.TogglePromptsActive(true, m_teamIndex.teamIndex);
                m_needBeginFlip = false;
            }

            if (m_sharedMoveCont.FlipCharge >= MovementConstants.FLIP_THRESHOLD)
            {
                // Bot has flipped
                FlipBot();
            }
        }

        #region Checking if flipped
        /// <summary>
        /// Checks if the bot should be considered flipped.
        /// Bot is considered flipped if it is not grounded (uncontrollable),
        /// has stopped moving, and has been in that condition for long enough.
        ///
        /// Pre Conditions - SharedMovementController (IsBotGrounded) and
        /// Rigidbody (IsBotStopped) are not null.
        /// Pos Conditions - Returns true if the bot is considered flipped. False if its
        /// not considered flipped.
        /// </summary>
        private bool IsBotFlipped()
        {
            // The bot is not flipped at all if the bot is grounded or it is moving.
            if (IsBotGrounded() || !IsBotStopped())
            {
                m_flippedTime = 0.0f;
                return false;
            }
            // If the bot hasn't been "flipped" long enough
            m_flippedTime += Time.deltaTime;
            if (m_flippedTime < m_timeUntilConsideredFlipped)
            {
                return false;
            }

            // Bot has made it past all checks testing if it hasn't flipped
            return true;
        }
        /// <summary>
        /// Checks if the bot is not currently moving.
        ///
        /// Pre Conditions - SharedMovementController is not null.
        /// Post Conditions - Returns if the bot is grounded.
        /// </summary>
        private bool IsBotStopped()
        {
            Assert.IsNotNull(m_rb, $"{nameof(HasFlippedMonitor)}'s {nameof(IsBotStopped)} function" +
                $" requires variable {nameof(m_rb)} ({m_rb.GetType().Name}) to be NotNull.");

            return m_rb.velocity.magnitude < m_velocityNearZeroTolerance;
        }
        /// <summary>
        /// Checks if the bot is currently controllable (grounded).
        ///
        /// Pre Conditions - Rigidbody is not null.
        /// Post Conditions - Returns if the bot is grounded.
        /// </summary>
        private bool IsBotGrounded()
        {
            Assert.IsNotNull(m_sharedMoveCont, $"{nameof(HasFlippedMonitor)}'s {nameof(IsBotGrounded)}" +
                $" function requires variable {nameof(m_sharedMoveCont)} " +
                $"({m_sharedMoveCont.GetType().Name}) to be NotNull.");

            return m_sharedMoveCont.IsGrounded();
        }
        #endregion Checking if flipped


        #region Executing the flip
        /// <summary>
        /// Starts flipping the bot if it is not already flipping.
        /// If it is already flipping, does nothing.
        ///
        /// Pre Conditions - Rigidbody is not null.
        /// Post Conditions - Starts flipping the bot if it is not flipping already. Also
        /// disables physics for the bot.
        /// </summary>
        private void FlipBot()
        {
            Assert.IsNotNull(m_rb, $"{nameof(HasFlippedMonitor)}'s {nameof(FlipBot)} function" +
                $" requires variable {nameof(m_rb)} ({m_rb.GetType().Name}) to be NotNull.");

            // Don't start flipping the bot if it is already flipping
            if (m_isFlipping) { return; }

            m_godHandSingleton.SpawnPlayGodHand(gameObject, m_teamIndex.teamIndex);

            // Set flipping flag
            m_isFlipping = true;

            // Start the actual coroutine to flip the bot
            StartFlipCoroutine();
        }

        /// <summary>
        /// Called when the bot has finished flipping.
        ///
        /// Pre Conditions - Rigidbody is not null.
        /// Post Conditions - Re-enables physics for the bot.
        /// </summary>
        private void EndFlipBot()
        {
            Assert.IsNotNull(m_rb, $"{nameof(HasFlippedMonitor)}'s {nameof(EndFlipBot)} function" +
                $" requires variable {nameof(m_rb)} ({m_rb.GetType().Name}) to be NotNull.");

            // Reset the flipping flag
            m_isFlipping = false;
            m_needBeginFlip = true;
        }
        /// <summary>
        /// Starts the coroutine to flip the bot.
        /// If another flip coroutine is currently running, that coroutine is stopped
        /// and a new one is started.
        ///
        /// Pre Conditions - None.
        /// Post Conditions - Re-enables physics for the bot.
        /// </summary>
        private void StartFlipCoroutine()
        {
            m_activeFlipCoroutine = StartCoroutine(FlipCoroutine());

            // Tell the movement controller it doesn't need to track flip "charge" anymore
            m_sharedMoveCont.RequireFlippedInput(false);

            m_hfmIcons.TogglePromptsActive(false, m_teamIndex.teamIndex);
        }
        /// <summary>
        /// Coroutine to flip the bot.
        /// Moves the position and rotation to correct the bot's placement.
        /// </summary>
        private IEnumerator FlipCoroutine()
        {
            yield return new WaitForSeconds(1.5f);
            EndFlipBot();
        }
        /// <summary>
        /// Calculate the target position we want to flip to.
        /// Position is based on the floor below the bot currently and the movement part offset.
        ///
        /// Pre Conditions - MovementPartPlacementManager is not null. The ground layermask accurately
        /// reflects everything that the bot can sit on.
        /// Post Conditions - Returns the calculated target position.
        /// </summary>
        private Vector3 CalculateFlipTargetPosition()
        {
            Assert.IsNotNull(m_movementPartPlacementMan, $"{nameof(HasFlippedMonitor)} on {name}" +
                $" requires a {nameof(MovementPartPlacementManager)} attached to one of " +
                $"its children, but it was not found.");

            // Cast down a ray to the ground
            if (!Physics.Raycast(transform.position, Vector3.down,
                out RaycastHit temp_hitInfo, float.PositiveInfinity, m_groundLayerMask))
            {
                // Means we are trying to flip the bot but are not above the ground.
                Debug.LogError($"No ground could be found, but we are trying to flip the bot ({name})");
                return transform.position;
            }
            Vector3 temp_targetPos = temp_hitInfo.point + m_movementPartPlacementMan.movePosOffset;

            return temp_targetPos;
        }
        private Quaternion CalculateFlipTargetRotation()
        {
            // TODO make the rotation align with the floor
            return Quaternion.identity;
        }
        #endregion Executing the flip

        
    }
}
