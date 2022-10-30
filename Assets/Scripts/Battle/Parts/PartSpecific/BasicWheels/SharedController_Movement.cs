using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik and Zach Gross

namespace DuolBots
{
    /// <summary>
    /// A helper class with public functions for an attached IController_Movement to access
    /// </summary>
    [RequireComponent(typeof(Specifications_Movement))]
    public class SharedController_Movement : MonoBehaviour, IMovementWeight
    {
        private const bool IS_DEBUGGING = false;
        // Struct that contains some visuals for debugging
        private struct DebuggingTools
        {
            public Vector3 Proj_Before;
            public Vector3 Proj_After;
            public Vector3 Velocity;
            public Vector3 aog_FrontCenter;
            public Vector3 aog_BackCenter;
        }
        private DebuggingTools DG = new DebuggingTools();

        // Currently not used at all
        [SerializeField] private AnimationCurve m_powerCurve;

        private Specifications_Movement m_specs = null;
        [SerializeField] private LayerMask m_groundLayerMask = 1 << 8;

        // Rigidbody to set the velocity and angle
        private Rigidbody m_body = null;

        // The current calculated power
        public float LeftCurrentPower => m_leftCurrentPower;
        private float m_leftCurrentPower = 0.0f;
        public float RightCurrentPower => m_rightCurrentPower;
        private float m_rightCurrentPower = 0.0f;

        public float LeftPowerVisual { get; private set; }
        public float RightPowerVisual { get; private set; }

        // The target power given the last input
        private float m_leftTargetPower = 0.0f;
        private float m_rightTargetPower = 0.0f;

        // Used for flip calculations
        private float m_lastLeftFlipRaw = 0.0f;
        private float m_lastRightFlipRaw = 0.0f;

        // "Constants" to be calculated given m_specifications and the bot's total weight
        private float m_acceleration = 0.0f;
        private float m_rotAccel = 0.0f;
        private float m_friction = 0.0f;

        // Used in oil calculations
        private bool m_isOiled = false;
        private float m_oilMuddle = 1.0f;
        private float m_defaultFriction = 0.0f;
        private float m_oilRecovery = MovementConstants.OIL_RECOVERY_TIME;
        private float m_worstMuddle = -1.0f;
        private float m_worstFriction = -1.0f;

        // Responsible for catching whether each wheel/leg is touching the ground (using triggers)
        private SingleMovement[] m_parts = new SingleMovement[4];

        // How much movement mashing/spamming has happened to flip the bot upright
        public float FlipCharge => m_leftFlip + m_rightFlip;
        private float m_leftFlip = 0.0f;
        private float m_rightFlip = 0.0f;
        private bool m_countFlipCharge = false;

        // Domestic Initialization
        private void Awake()
        {
            m_specs = GetComponent<Specifications_Movement>();

            DG.Proj_Before = Vector3.zero;
            DG.Proj_After = Vector3.zero;
            DG.Velocity = Vector3.zero;
        }

        // Foreign Initialization
        private void Start()
        {
            m_body = GetComponentInParent<Rigidbody>();

            for (int x = 0; x < 4; x++)
            {
                m_parts[x] = m_specs.wheelTransforms[x].GetComponent<SingleMovement>();
                Assert.IsNotNull(m_parts[x], $"SharedController_Movement of {transform.parent.name} cannot find a SingleMovement script on the {(WheelPos)x} part");
            }

            //Assert.IsNotNull(m_body, $"{name} is expecting a Rigidbody as part of BotRoot, did not find it!");
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Specifications_Movement tempspec = GetComponent<Specifications_Movement>();

                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transform.position + new Vector3(3, 0, 0), 2 * transform.forward);

                Gizmos.color = new Color(1, 0.5f, 0);
                Gizmos.DrawRay(transform.position + new Vector3(3, 0, 0) - 2 * transform.forward, 2 * transform.forward);

                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position + new Vector3(3.5f, 0, 0), 2 * DG.Proj_Before);

                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position + new Vector3(4, 0, 0), 2 * DG.Proj_After);

                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position + new Vector3(4.5f, 0, 0), 2 * DG.Velocity);

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(DG.aog_FrontCenter, 0.6f);
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(DG.aog_BackCenter, 0.6f);
            }
        }

        public bool IsGrounded()
        {
            for (int x = 0; x < m_parts.Length; x++)
            {
                if (m_parts[x].IsGrounded)
                {
                    return true;
                }
            }
            return false;
        }

        private bool AllGrounded()
        {
            for (int x = 0; x < m_parts.Length; x++)
            {
                if (!m_parts[x].IsGrounded)
                {
                    return false;
                }
            }
            return true;
        }

        private bool HasZeroInput()
        {
            return m_leftTargetPower == 0 && m_rightTargetPower == 0;
        }

        // USED IN NEW IMPLEMENTATION
        private float m_lastLeft = 0;
        private float m_lastRight = 0;
        private Vector3 m_lastDir = Vector3.forward;


        private float m_trueRotAccel;
        private float m_trueFriction;

        /// <summary>
        /// Calculates necessary movement information for this frame and sets the velocity and angle of the Rigidbody
        /// </summary>
        public void Move()
        {
            #region CurrentImplementation
            /** CURRENT IMPLEMENTATION **/

            // If this is not on the server when we are networked.
            if (m_body == null) { return; }

            float dt = Time.deltaTime;

            ResolveOilEffect(dt);
            CalculatePower(dt);

            float[] temp_powerFromParts = new float[4];
            temp_powerFromParts[(int)WheelPos.FL] = m_parts[(int)WheelPos.FL].IsGrounded ? 0.5f * m_leftCurrentPower : 0;
            temp_powerFromParts[(int)WheelPos.FR] = m_parts[(int)WheelPos.FR].IsGrounded ? 0.5f * m_rightCurrentPower : 0;
            temp_powerFromParts[(int)WheelPos.BL] = m_parts[(int)WheelPos.BL].IsGrounded ? 0.5f * m_leftCurrentPower : 0;
            temp_powerFromParts[(int)WheelPos.BR] = m_parts[(int)WheelPos.BR].IsGrounded ? 0.5f * m_rightCurrentPower : 0;

            float temp_finalLeftPower = temp_powerFromParts[(int)WheelPos.FL] + temp_powerFromParts[(int)WheelPos.BL];
            float temp_finalRightPower = temp_powerFromParts[(int)WheelPos.FR] + temp_powerFromParts[(int)WheelPos.BR];

            Vector3 temp_aog = AngleOfGround().normalized;
            float temp_percentOfMax = m_body.velocity.magnitude / m_specs.maxWheelPower;
            Vector3 temp_movementDirection;

            if (temp_percentOfMax <= 0.2f)
            {
                temp_movementDirection = temp_aog;
            }
            else
            {
                float temp_currentVelocityMultiplier = Mathf.Clamp(temp_percentOfMax - 0.2f, 0, 0.6f);
                int directionMultiplier = temp_finalLeftPower < 0 && temp_finalRightPower < 0 ? -1 : 1;
                temp_movementDirection = directionMultiplier * temp_currentVelocityMultiplier * m_body.velocity.normalized + (1 - temp_currentVelocityMultiplier) * temp_aog;
                temp_movementDirection.Normalize();
            }

            if (temp_finalLeftPower > 0 && temp_finalRightPower > 0)
            {
                m_body.velocity = Mathf.Min(temp_finalLeftPower, temp_finalRightPower) * temp_movementDirection;
            }
            else if (temp_finalLeftPower < 0 && temp_finalRightPower < 0)
            {
                m_body.velocity = Mathf.Max(temp_finalLeftPower, temp_finalRightPower) * temp_movementDirection;
            }

            float dRot = m_rotAccel * (temp_finalLeftPower - temp_finalRightPower) / m_specs.maxWheelPower;

            /*Debug.Log($"DRot: {dRot}");
            Debug.Log($"After clamp and muddle: {m_oilMuddle * Mathf.Clamp(dRot, -MovementConstants.MAX_ROTATION_FACTOR * 2 * m_rotAccel * m_specs.maxWheelPower, MovementConstants.MAX_ROTATION_FACTOR * 2 * m_rotAccel * m_specs.maxWheelPower)}");*/

            Vector3 rotatePos = RotationPoint();
            /*m_body.transform.RotateAround(rotatePos, Vector3.up, m_oilMuddle * Mathf.Clamp(dRot, -MovementConstants.MAX_ROTATION_FACTOR * m_specs.maxWheelPower, MovementConstants.MAX_ROTATION_FACTOR * m_specs.maxWheelPower));*/
            m_body.transform.RotateAround(rotatePos, Vector3.up, m_oilMuddle * dRot);

            // If the power is not 0, the visual power should be equal to it
            // If the power is 0 and the other side has a nonzero power, the visual should be set to a nonzero value to make it look like it's moving
            LeftPowerVisual = Mathf.Abs(temp_finalLeftPower) > Mathf.Abs(temp_finalRightPower / 1.5f) ? temp_finalLeftPower : temp_finalRightPower / 1.5f;
            RightPowerVisual = Mathf.Abs(temp_finalRightPower) > Mathf.Abs(temp_finalLeftPower / 1.5f) ? temp_finalRightPower : temp_finalLeftPower / 1.5f;

            #endregion CurrentImplementation

            #region ProposedImplementation
            /** PROPOSED IMPLEMENTATION **/
            // If this is not on the server when we are networked.
            /*
            if (m_body == null) { return; }

            // Mock-up of hitting oil
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                ApplyOil2();
                //ApplyOil();
                Debug.Log("Begin touching oil");
            }
            else if (Input.GetKey(KeyCode.Minus))
            {
                RemoveOil2();
                //RemoveOil();
                Debug.Log("No longer touching oil");
            }

            float dt = Time.deltaTime;

            ResolveOilEffect(dt);
            CalculatePower(dt);

            float[] temp_powerFromParts = new float[4];
            temp_powerFromParts[(int)WheelPos.FL] = m_parts[(int)WheelPos.FL].IsGrounded ? 0.5f * m_leftCurrentPower : 0;
            temp_powerFromParts[(int)WheelPos.FR] = m_parts[(int)WheelPos.FR].IsGrounded ? 0.5f * m_rightCurrentPower : 0;
            temp_powerFromParts[(int)WheelPos.BL] = m_parts[(int)WheelPos.BL].IsGrounded ? 0.5f * m_leftCurrentPower : 0;
            temp_powerFromParts[(int)WheelPos.BR] = m_parts[(int)WheelPos.BR].IsGrounded ? 0.5f * m_rightCurrentPower : 0;

            float temp_finalLeftPower;
            float temp_finalRightPower;

            //const bool DO_POWER_CURVE = false;
            //if (DO_POWER_CURVE)
            //{
            //    float temp_leftSum = temp_powerFromParts[(int)WheelPos.FL] + temp_powerFromParts[(int)WheelPos.BL];
            //    float temp_rightSum = temp_powerFromParts[(int)WheelPos.FR] + temp_powerFromParts[(int)WheelPos.BR];
            //    float temp_leftCurveKey = Mathf.Abs(temp_leftSum) / m_specs.maxWheelPower;
            //    float temp_rightCurveKey = Mathf.Abs(temp_rightSum) / m_specs.maxWheelPower;

            //    temp_finalLeftPower = Mathf.Sign(temp_leftSum) * m_powerCurve.Evaluate(temp_leftCurveKey) * m_specs.maxWheelPower;
            //    temp_finalRightPower = Mathf.Sign(temp_rightSum) * m_powerCurve.Evaluate(temp_rightCurveKey) * m_specs.maxWheelPower;
            //}
            //else
            //{
            temp_finalLeftPower = temp_powerFromParts[(int)WheelPos.FL] + temp_powerFromParts[(int)WheelPos.BL];
            temp_finalRightPower = temp_powerFromParts[(int)WheelPos.FR] + temp_powerFromParts[(int)WheelPos.BR];
            //}

            Vector3 temp_aog = AngleOfGround().normalized;
            float temp_percentOfMax = m_body.velocity.magnitude / m_specs.maxWheelPower;
            Vector3 temp_movementDirection;

            if (temp_percentOfMax <= 0.2f)
            {
                temp_movementDirection = temp_aog;
            }
            else
            {
                float temp_currentVelocityMultiplier = Mathf.Clamp(temp_percentOfMax - 0.2f, 0, 0.6f);
                int directionMultiplier = temp_finalLeftPower < 0 && temp_finalRightPower < 0 ? -1 : 1;
                temp_movementDirection = directionMultiplier * temp_currentVelocityMultiplier * m_body.velocity.normalized + (1 - temp_currentVelocityMultiplier) * temp_aog;
                temp_movementDirection.Normalize();
            }

            Vector3 temp_oldVelocity = CalculateVelocity(m_lastLeft, m_lastRight, m_lastDir);
            //Debug.Log($"Temp Old Velocity: {temp_oldVelocity}");
            //Debug.Log($"RB's Velocity: {m_body.velocity}");
            //Debug.Log($"Difference in Temp_OldVelocity and RB's Velocity: {temp_oldVelocity - m_body.velocity}");

            m_lastLeft = temp_finalLeftPower;
            m_lastRight = temp_finalRightPower;
            m_lastDir = temp_movementDirection;

            Vector3 temp_newVelocity = CalculateVelocity(temp_finalLeftPower, temp_finalRightPower, temp_movementDirection);

            Debug.Log($"Calculated New Velocity: {temp_newVelocity}");

            bool movingBackward = temp_finalLeftPower < 0 && temp_finalRightPower < 0;
            Vector3 forwardOrBack = (movingBackward) ? -transform.forward : transform.forward;

            Vector3 temp_velocityBeforeChange = m_body.velocity;
            Vector3 temp_velocityAfterChange = m_body.velocity + temp_newVelocity;

            Vector3 temp_bv = RoundToPrecision(temp_velocityBeforeChange, 2);
            Vector3 temp_av = RoundToPrecision(temp_velocityAfterChange, 2);

            //Debug.Log($"Before Change: ({temp_bv.x}, {temp_bv.y}, {temp_bv.z})");
            //Debug.Log($"After Change: ({temp_av.x}, {temp_av.y}, {temp_av.z})");

            Vector3 velocity = RoundToPrecision(temp_velocityAfterChange, 2);
            DG.Velocity = velocity;

            Vector3 proj_bv = ReluMatchSign(forwardOrBack, Vector3.Project(temp_bv, transform.forward));
            Vector3 proj_av = ReluMatchSign(forwardOrBack, Vector3.Project(temp_av, transform.forward));


            DG.Proj_Before = proj_bv;
            DG.Proj_After = proj_av;

            // TODO:
            // Figure out some way to cap the player's velocity in such a way that gravity is respected

            // NOTE:
            // It looks like the below check makes gravity work, but doesn't cap as expected when going too fast
            // Need to show the projections (both av and bv) as gizmos and figure out why things aren't working as hoped/expected

            // Only try to cap velocity if the bot is grounded
            if (IsGrounded())
            {
                // If the previous velocity was ok and the new velocity is not, then the change in velocity *caused by the player* is
                // what pushed it over the threshold. Therefore, it should be ok to cap it.
                if (proj_bv.magnitude <= temp_newVelocity.magnitude + 0.001f && proj_av.magnitude > temp_newVelocity.magnitude + 0.001f)
                {
                    m_body.velocity = m_body.velocity.normalized * temp_newVelocity.magnitude;
                    Debug.Log($"RB's rounded velocity: ({velocity.x}, {velocity.y}, {velocity.z})");
                    Debug.Log($"<color=orange>RB's velocity capped to {m_body.velocity}</color>");
                }
                else
                {
                    m_body.AddForce(temp_newVelocity, ForceMode.VelocityChange);
                }
            }


            float dRot = m_oilMuddle * m_rotAccel * (temp_finalLeftPower - temp_finalRightPower) / m_specs.maxWheelPower;

            Vector3 rotatePos = RotationPoint();
            m_body.transform.RotateAround(rotatePos, Vector3.up, dRot);

            // If the power is not 0, the visual power should be equal to it
            // If the power is 0 and the other side has a nonzero power, the visual should be set to a nonzero value to make it look like it's moving
            LeftPowerVisual = Mathf.Abs(temp_finalLeftPower) > Mathf.Abs(temp_finalRightPower / 1.5f) ? temp_finalLeftPower : temp_finalRightPower / 1.5f;
            RightPowerVisual = Mathf.Abs(temp_finalRightPower) > Mathf.Abs(temp_finalLeftPower / 1.5f) ? temp_finalRightPower : temp_finalLeftPower / 1.5f;
            */
            #endregion ProposedImplementation
        }

        /// <summary>
        /// Sets the target power of the left side given the passed data and the max power
        /// </summary>
        /// <param name="newTarget">The value of the input received</param>
        public void SetLeftTarget(float newTarget)
        {
            if (m_countFlipCharge)
            {
                float temp_diff = Mathf.Abs(newTarget - m_lastLeftFlipRaw);
                float temp_add = 0;
                if (temp_diff > 0.2f)
                {
                    temp_add = temp_diff;
                    m_lastLeftFlipRaw = newTarget;
                }

                m_leftFlip = Mathf.Min(m_leftFlip + temp_add, MovementConstants.FLIP_THRESHOLD / 2.0f);
            }
            m_leftTargetPower = newTarget * m_specs.maxWheelPower;
        }

        /// <summary>
        /// Sets the target power of the right side given the passed data and the max power
        /// </summary>
        /// <param name="newTarget">The value of the input receieved</param>
        public void SetRightTarget(float newTarget)
        {
            if (m_countFlipCharge)
            {
                float temp_diff = Mathf.Abs(newTarget - m_lastRightFlipRaw);
                float temp_add = 0;
                if (temp_diff > 0.2f)
                {
                    temp_add = temp_diff;
                    m_lastRightFlipRaw = newTarget;
                }

                m_rightFlip = Mathf.Min(m_rightFlip + temp_add, MovementConstants.FLIP_THRESHOLD / 2.0f);
            }
            m_rightTargetPower = newTarget * m_specs.maxWheelPower;
        }

        /// <summary>
        /// Calculates the acceleration, rotAccel, and friction "constants" for this movement component given the bot's total weight
        /// </summary>
        /// <param name="totalBotWeight">The bot's total weight</param>
        public void SetWeight(int totalBotWeight)
        {
            m_acceleration = MovementConstants.ACCELERATION_CONSTANT / (totalBotWeight * m_specs.weightSlowdownFactor);
            m_rotAccel = MovementConstants.ROTATION_CONSTANT / (Mathf.Sqrt(totalBotWeight) *  m_specs.weightSlowdownFactor);
            m_friction = totalBotWeight * m_specs.weightSlowdownFactor * MovementConstants.FRICTION_CONSTANT;

            CustomDebug.Log($"Weight is being set to {totalBotWeight} for {name}", IS_DEBUGGING);
            CustomDebug.Log("Acceleration: " + m_acceleration, IS_DEBUGGING);
            CustomDebug.Log("Rot Accel: " + m_rotAccel, IS_DEBUGGING);
            CustomDebug.Log("Friction: " + m_friction, IS_DEBUGGING);

            m_trueRotAccel = m_rotAccel;

            m_trueFriction = m_friction;
            m_defaultFriction = m_friction;
        }

        /// <summary>
        /// Tells the movement controller to store and sum up changes in movement
        /// </summary>
        /// <param name="isFlipped">Whether the bot is flipped (and thus requires tracking input)</param>
        public void RequireFlippedInput(bool isFlipped)
        {
            m_countFlipCharge = isFlipped;
            if (!isFlipped)
            {
                m_leftFlip = 0.0f;
                m_rightFlip = 0.0f;
            }
        }

        #region OilCalculations
        /// <summary>
        /// Begins the oil status on the bot's movement
        /// </summary>
        public void ApplyOil()
        {
            m_isOiled = true;
            m_friction = MovementConstants.OIL_MINIMUM_FRICTION * m_defaultFriction;
            m_oilMuddle = MovementConstants.OIL_MINIMUM_MUDDLE;
            m_oilRecovery = 0.0f;
        }

        /// <summary>
        /// Indicates that the bot is no longer "touching" the oil (but the bot still has to go through a recovery period.
        /// </summary>
        public void RemoveOil()
        {
            m_isOiled = false;
        }

        /// <summary>
        /// Progressively increases the amount that oil affects the bot (to a maximum) if affected by oil
        /// Progressively decreases the amount that oil affects the bot (back to normal) if not affected by oil
        /// </summary>
        private void ResolveOilEffect(float dt)
        {
            CheckPartsForOil();

            if (m_isOiled)
            {
                float increase = MovementConstants.OIL_MAXIMUM_MUDDLE * dt * Random.Range(0.15f, 0.25f);
                m_oilMuddle += increase;
                CustomDebug.Log($"Oil muddle increased by {increase}", true);
                m_oilMuddle = Mathf.Clamp(m_oilMuddle, MovementConstants.OIL_MINIMUM_MUDDLE, MovementConstants.OIL_MAXIMUM_MUDDLE);
                CustomDebug.Log($"Oil Muddle is now {m_oilMuddle}", true);
            }
            else
            {
                m_oilRecovery += dt;
                if (m_oilRecovery >= MovementConstants.OIL_RECOVERY_TIME)
                {
                    m_friction = m_defaultFriction;
                    m_oilMuddle = 1.0f;
                    // Reset the "worst" values back to defaults
                    m_worstFriction = -1.0f;
                    m_worstMuddle = -1.0f;
                }
                else
                {
                    if (m_worstFriction == -1.0f && m_worstMuddle == -1.0f)
                    {
                        m_worstMuddle = m_oilMuddle;
                        m_worstFriction = m_friction;
                    }
                    // Lerps friction and oilMuddle back to regular values over the recovery time
                    m_friction = Mathf.Lerp(m_worstFriction, m_defaultFriction, m_oilRecovery / MovementConstants.OIL_RECOVERY_TIME);
                    m_oilMuddle = Mathf.Lerp(m_worstMuddle, 1.0f, m_oilRecovery / MovementConstants.OIL_RECOVERY_TIME);
                    CustomDebug.Log($"Oil Muddle is now {m_oilMuddle}", false);
                }
            }
        }

        private void CheckPartsForOil()
        {
            if (m_isOiled)
            {
                bool hasEffectExpired = true;
                foreach (SingleMovement part in m_parts)
                {
                    if (part.IsOiled)
                    {
                        hasEffectExpired = false;
                        break;
                    }
                }
                if (hasEffectExpired)
                {
                    RemoveOil();
                }

                if (!m_isOiled)
                {
                    Debug.Log("Bot is no longer oiled");
                }
            }
            else
            {
                foreach (SingleMovement part in m_parts)
                {
                    if (part.IsOiled)
                    {
                        ApplyOil();
                        Debug.Log("Bot has become oiled");
                    }
                }
            }
        }
        #endregion Oil

        /// <param name="left">The current left power</param>
        /// <param name="right">The current right power</param>
        /// <param name="dir">The direction the bot is facing</param>
        /// <returns>The desired velocity using the given power values</returns>
        private Vector3 CalculateVelocity(float left, float right, Vector3 dir)
        {
            if (left > 0 && right > 0)
            {
                return Mathf.Min(left, right) * dir; 
            }
            else if (left < 0 && right < 0)
            {
                return Mathf.Max(left, right) * dir;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// Called every update to calculate the power supplied to each side given the most recent target power
        /// </summary>
        private void CalculatePower(float dt)
        {
            // Custom debugging message string
            string dmsg;

            // Make sure at least one part on the left side is grounded (to prevent "revving up" in midair)
            if (m_parts[(int)WheelPos.FL].IsGrounded || m_parts[(int)WheelPos.BL].IsGrounded)
            {
                // Prioritize catch-up
                // Basic requirements for catch-up:
                // The target power of left is in the same direction of the current power of right
                // Neither the target power of left nor the current power of right is 0
                if (
                    Mathf.Sign(m_leftTargetPower) == Mathf.Sign(m_rightCurrentPower) &&
                    (Mathf.Abs(m_leftCurrentPower) < MovementConstants.CATCHUP_FACTOR * Mathf.Abs(m_rightCurrentPower)) &&
                    !(m_leftTargetPower == 0 || m_rightCurrentPower == 0)
                    )
                {
                    // Left is currently moving opposite to right (or left is zero and right is negative), but should change sign and catch up
                    // Left is currently not moving at all and should catch up
                    // Left is currently moving, but slower than right, and should catch up
                    if (
                        Mathf.Sign(m_leftCurrentPower) != Mathf.Sign(m_rightCurrentPower) ||
                        m_leftCurrentPower == 0 ||
                        Mathf.Abs(m_leftCurrentPower) < Mathf.Abs(m_rightCurrentPower)
                        )
                    {
                        float expectedPower = m_leftCurrentPower + Mathf.Sign(m_leftTargetPower) * m_acceleration * dt;
                        if (Mathf.Abs(expectedPower) > Mathf.Abs(m_leftTargetPower) && Mathf.Sign(expectedPower) == Mathf.Sign(m_leftTargetPower))
                        {
                            expectedPower = m_leftTargetPower;
                        }
                        float catchupPower = MovementConstants.CATCHUP_FACTOR * m_rightCurrentPower;

                        // If the catchupPower is greater than the expected power AND catchupPower is less than the target power, use catchupPower instead of expectedPower
                        if (Mathf.Abs(catchupPower) > Mathf.Abs(expectedPower) && Mathf.Abs(catchupPower) <= Mathf.Abs(m_leftTargetPower))
                        {
                            dmsg = $"Left is now {catchupPower} compared to right's {m_rightCurrentPower}";
                            m_leftCurrentPower = catchupPower;
                        }
                        else
                        {
                            dmsg = $"Left is using expectedPower {expectedPower} compared to right's {m_rightCurrentPower}";
                            m_leftCurrentPower = expectedPower;
                        }

                        CustomDebug.Log(dmsg, IS_DEBUGGING);
                    }
                }
                // Left Side Check for Different Signs
                else if (Mathf.Sign(m_leftTargetPower) != Mathf.Sign(m_leftCurrentPower) && !(m_leftTargetPower == 0 || m_leftCurrentPower == 0))
                {
                    // In this case, the left side is both accelerating in the target direction and affected by friction against the current direction, but shouldn't snap to the target

                    dmsg = "Left is accelerating ";
                    dmsg += (Mathf.Sign(m_leftTargetPower) == 1) ? "forward" : "backward";
                    CustomDebug.Log(dmsg, IS_DEBUGGING);
                    dmsg = "Left is affected by ";
                    dmsg += (Mathf.Sign(m_leftCurrentPower) == -1) ? "forward friction" : "backward friction";
                    CustomDebug.Log(dmsg, IS_DEBUGGING);

                    m_leftCurrentPower += Mathf.Sign(m_leftTargetPower) * m_acceleration * dt - Mathf.Sign(m_leftCurrentPower) * m_friction * dt;
                }
                // Left Side Normal Acceleration Check
                else
                {
                    if (Mathf.Abs(m_leftTargetPower) > Mathf.Abs(m_leftCurrentPower))
                    {
                        // In this case, the left side is accelerating in the target direction and should snap to the target if it would exceed the target

                        dmsg = "Left is accelerating ";
                        dmsg += (Mathf.Sign(m_leftTargetPower) == 1) ? "forward" : "backward";
                        CustomDebug.Log(dmsg, IS_DEBUGGING);

                        m_leftCurrentPower += Mathf.Sign(m_leftTargetPower) * m_acceleration * dt;
                        if (Mathf.Abs(m_leftCurrentPower) > Mathf.Abs(m_leftTargetPower) && Mathf.Sign(m_leftCurrentPower) == Mathf.Sign(m_leftTargetPower))
                        {
                            m_leftCurrentPower = m_leftTargetPower;
                        }
                    }
                    // Left Side Friction Check
                    else if (Mathf.Abs(m_leftTargetPower) < Mathf.Abs(m_leftCurrentPower))
                    {
                        // In this case, the left side is affected by friction against the current direction and should snap to the target if it would drop below it

                        dmsg = "Left is affected by ";
                        dmsg += (Mathf.Sign(m_leftCurrentPower) == -1) ? "forward friction" : "backward friction";
                        CustomDebug.Log(dmsg, IS_DEBUGGING);

                        float presign = Mathf.Sign(m_leftCurrentPower);

                        m_leftCurrentPower -= presign * m_friction * dt;
                        if ((Mathf.Abs(m_leftCurrentPower) < Mathf.Abs(m_leftTargetPower) && Mathf.Sign(m_leftCurrentPower) == Mathf.Sign(m_leftTargetPower)) || (m_leftTargetPower == 0 && presign != Mathf.Sign(m_leftCurrentPower)))
                        {
                            m_leftCurrentPower = m_leftTargetPower;
                        }
                    }
                }
            }
            else
            {
                // Even if not grounded, the left side's power should decrease, but not as fast
                dmsg = "Left is in the air and affected by ";
                dmsg += (Mathf.Sign(m_leftCurrentPower) == -1) ? "forward friction" : "backward friction";
                CustomDebug.Log(dmsg, IS_DEBUGGING);

                float presign = Mathf.Sign(m_leftCurrentPower);

                m_leftCurrentPower -= presign * MovementConstants.MIDAIR_FRICTION_FACTOR * m_friction * dt;
                if (presign != Mathf.Sign(m_leftCurrentPower))
                {
                    m_leftCurrentPower = 0;
                }
            }

            // Make sure at least one part on the right side is grounded (to prevent "revving up" in midair)
            if (m_parts[(int)WheelPos.FR].IsGrounded || m_parts[(int)WheelPos.BR].IsGrounded)
            {
                // Prioritize catch-up
                // Basic requirements for catch-up:
                // The target power of right is in the same direction of the current power of left
                // Neither the target power of right nor the current power of left is 0
                if (
                    Mathf.Sign(m_rightTargetPower) == Mathf.Sign(m_leftCurrentPower) &&
                    (Mathf.Abs(m_rightCurrentPower) < MovementConstants.CATCHUP_FACTOR * Mathf.Abs(m_leftCurrentPower)) &&
                    !(m_rightTargetPower == 0 || m_leftCurrentPower == 0)
                    )
                {
                    // right is currently moving opposite to left (or right is zero and left is negative), but should change sign and catch up
                    // right is currently not moving at all and should catch up
                    // right is currently moving, but slower than left, and should catch up
                    if (
                        Mathf.Sign(m_rightCurrentPower) != Mathf.Sign(m_leftCurrentPower) ||
                        m_rightCurrentPower == 0 ||
                        Mathf.Abs(m_rightCurrentPower) < Mathf.Abs(m_leftCurrentPower)
                        )
                    {
                        float expectedPower = m_rightCurrentPower + Mathf.Sign(m_rightTargetPower) * m_acceleration * dt;
                        if (Mathf.Abs(expectedPower) > Mathf.Abs(m_rightTargetPower) && Mathf.Sign(expectedPower) == Mathf.Sign(m_rightTargetPower))
                        {
                            expectedPower = m_rightTargetPower;
                        }
                        float catchupPower = MovementConstants.CATCHUP_FACTOR * m_leftCurrentPower;

                        // If the catchupPower is greater than the expected power AND catchupPower is less than the target power, use catchupPower instead of expectedPower
                        if (Mathf.Abs(catchupPower) > Mathf.Abs(expectedPower) && Mathf.Abs(catchupPower) <= Mathf.Abs(m_rightTargetPower))
                        {
                            dmsg = $"right is now {catchupPower} compared to left's {m_leftCurrentPower}";
                            m_rightCurrentPower = catchupPower;
                        }
                        else
                        {
                            dmsg = $"right is using expectedPower {expectedPower} compared to left's {m_leftCurrentPower}";
                            m_rightCurrentPower = expectedPower;
                        }

                        CustomDebug.Log(dmsg, IS_DEBUGGING);
                    }
                }
                // Right Side Check for Different Signs
                else if (Mathf.Sign(m_rightTargetPower) != Mathf.Sign(m_rightCurrentPower) && !(m_rightTargetPower == 0 || m_rightCurrentPower == 0))
                {
                    // In this case, the right side is both accelerating in the target direction and affected by friction against the current direction, but shouldn't snap to the target

                    dmsg = "Right is accelerating ";
                    dmsg += (Mathf.Sign(m_rightTargetPower) == 1) ? "forward" : "backward";
                    CustomDebug.Log(dmsg, IS_DEBUGGING);
                    dmsg = "Right is affected by ";
                    dmsg += (Mathf.Sign(m_rightCurrentPower) == -1) ? "forward friction" : "backward friction";
                    CustomDebug.Log(dmsg, IS_DEBUGGING);

                    m_rightCurrentPower += Mathf.Sign(m_rightTargetPower) * m_acceleration * dt - Mathf.Sign(m_rightCurrentPower) * m_friction * dt;
                }
                // Right Side Normal Acceleration Check
                else
                {
                    if (Mathf.Abs(m_rightTargetPower) > Mathf.Abs(m_rightCurrentPower))
                    {
                        // In this case, the right side is accelerating in the target direction and should snap to the target if it would exceed the target

                        dmsg = "Right is accelerating ";
                        dmsg += (Mathf.Sign(m_rightTargetPower) == 1) ? "forward" : "backward";
                        CustomDebug.Log(dmsg, IS_DEBUGGING);

                        m_rightCurrentPower += Mathf.Sign(m_rightTargetPower) * m_acceleration * dt;
                        if (Mathf.Abs(m_rightCurrentPower) > Mathf.Abs(m_rightTargetPower) && Mathf.Sign(m_rightCurrentPower) == Mathf.Sign(m_rightTargetPower))
                        {
                            m_rightCurrentPower = m_rightTargetPower;
                        }
                        CustomDebug.Log("Right Current: " + m_rightCurrentPower, IS_DEBUGGING);
                    }
                    // Right Side Friction Check
                    else if (Mathf.Abs(m_rightTargetPower) < Mathf.Abs(m_rightCurrentPower))
                    {
                        // In this case, the right side is affected by friction against the current direction and should snap to the target if it would drop below it

                        dmsg = "Right is affected by ";
                        dmsg += (Mathf.Sign(m_rightCurrentPower) == -1) ? "forward friction" : "backward friction";
                        CustomDebug.Log(dmsg, IS_DEBUGGING);

                        float presign = Mathf.Sign(m_rightCurrentPower);

                        m_rightCurrentPower -= presign * m_friction * dt;
                        if ((Mathf.Abs(m_rightCurrentPower) < Mathf.Abs(m_rightTargetPower) && Mathf.Sign(m_rightCurrentPower) == Mathf.Sign(m_rightTargetPower)) || (m_rightTargetPower == 0 && presign != Mathf.Sign(m_rightCurrentPower)))
                        {
                            m_rightCurrentPower = m_rightTargetPower;
                        }
                    }
                }
            }
            else
            {
                // Even if not grounded, the right side's power should decrease as, but not as fast
                dmsg = "Right is in the air and affected by ";
                dmsg += (Mathf.Sign(m_rightCurrentPower) == -1) ? "forward friction" : "backward friction";
                CustomDebug.Log(dmsg, IS_DEBUGGING);

                float presign = Mathf.Sign(m_rightCurrentPower);

                m_rightCurrentPower -= presign * MovementConstants.MIDAIR_FRICTION_FACTOR * m_friction * dt;
                if (presign != Mathf.Sign(m_rightCurrentPower))
                {
                    m_rightCurrentPower = 0;
                }
            }
        }

        /// <summary>
        /// Determines what point the robot should rotate around while turning
        /// </summary>
        /// <returns>The point the robot will rotate around</returns>
        private Vector3 RotationPoint()
        {
            Vector3 left = m_specs.leftSideCenter;
            Vector3 right = m_specs.rightSideCenter;
            left += 0.5f * (left - right);
            right -= 0.5f * (left - right);
            Vector3 center = Vector3.Lerp(left, right, 0.5f);
            float maxPower;
            float minPower;
            Vector3 minPowerPosition;

            if (Mathf.Abs(m_leftCurrentPower) == Mathf.Abs(m_rightCurrentPower))
            {
                return center;
            }
            else if (Mathf.Abs(m_leftCurrentPower) > Mathf.Abs(m_rightCurrentPower))
            {
                maxPower = Mathf.Abs(m_leftCurrentPower);
                minPower = Mathf.Abs(m_rightCurrentPower);
                minPowerPosition = right;
            }
            else
            {
                maxPower = Mathf.Abs(m_rightCurrentPower);
                minPower = Mathf.Abs(m_leftCurrentPower);
                minPowerPosition = left;
            }
            return Vector3.Lerp(center, minPowerPosition, (maxPower - minPower) / maxPower);
        }

        /// <summary>
        /// Casts down from each wheel to calculate the if there is a difference in elevation at ground level instead of using the transform's forward vector
        /// Checks if the RaycastHits have y-coordinates that lie within a certain threshold (if not, transform.forward is used instead of the angle of ground)
        /// </summary>
        /// <returns></returns>
        private Vector3 AngleOfGround()
        {
            // I was getting this called in the editor and it was causing
            // NPEs - Wyatt
            if (!Application.isPlaying) { return Vector3.zero; }

            Vector3 temp_verticalOffset = new Vector3(0, 1, 0);

            // Changed -transform.up to -Vector3.up
            Physics.Raycast(m_specs.frontLeft.position + temp_verticalOffset, -Vector3.up, out RaycastHit frontLeft, 1000, m_groundLayerMask);
            Physics.Raycast(m_specs.frontRight.position + temp_verticalOffset, -Vector3.up, out RaycastHit frontRight, 1000, m_groundLayerMask);
            Physics.Raycast(m_specs.backLeft.position + temp_verticalOffset, -Vector3.up, out RaycastHit backLeft, 1000, m_groundLayerMask);
            Physics.Raycast(m_specs.backRight.position + temp_verticalOffset, -Vector3.up, out RaycastHit backRight, 1000, m_groundLayerMask);

            Vector3 frontCenter = (frontLeft.point + frontRight.point) / 2;
            DG.aog_FrontCenter = frontCenter;
            Vector3 backCenter = (backLeft.point + backRight.point) / 2;
            DG.aog_BackCenter = backCenter;

            float ghd = Mathf.Abs(frontCenter.y - backCenter.y);
            CustomDebug.Log("Ground height difference: " + ghd, IS_DEBUGGING);

            if (ghd >= MovementConstants.GROUND_HEIGHT_THRESHOLD)
            {
                return transform.forward;
            }
            else
            {
                return frontCenter - backCenter;
            }
        }

        /// <summary>
        /// For each value in the input vector, compares the sign of the input and the sign of the corresponding value from the toMatch
        /// vector. If the signs are the same, keep the value, otherwise, replace it with 0.
        /// </summary>
        /// <param name="toMatch">The vector with the signs to match</param>
        /// <param name="input">The vector with the values to check</param>
        /// <returns></returns>
        private Vector3 ReluMatchSign(Vector3 toMatch, Vector3 input)
        {
            Vector3 constructed = new Vector3();
            constructed.x = (Mathf.Sign(toMatch.x) == Mathf.Sign(input.x)) ? input.x : 0;
            constructed.y = (Mathf.Sign(toMatch.y) == Mathf.Sign(input.y)) ? input.y : 0;
            constructed.z = (Mathf.Sign(toMatch.z) == Mathf.Sign(input.z)) ? input.z : 0;
            return constructed;
        }

        /// <summary>
        /// Rounds each value of the input vector to the given precision and returns it
        /// </summary>
        /// <param name="input"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        private Vector3 RoundToPrecision(Vector3 input, int precision)
        {
            float p = Mathf.Pow(10, precision);
            Vector3 constructed = new Vector3(
                    Mathf.Round(p * input.x) / p,
                    Mathf.Round(p * input.y) / p,
                    Mathf.Round(p * input.z) / p
                );
            return constructed;
        }
    }
}
