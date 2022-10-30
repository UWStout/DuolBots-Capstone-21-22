using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Original Author - Zach Gross
// Obsolete code governing movement

namespace DuolBots
{
    /// <summary>
    /// Basic movement component to control how wheels will work
    /// </summary>
    public class BasicWheels : MonoBehaviour, IPartInput
    {
        private const bool IS_DEBUGGING = false;


        public string uniqueID => m_uniqueID.value;
        [SerializeField] private StringID m_uniqueID = null;

        [SerializeField] private float m_acceleration;
        [SerializeField] private float m_friction;
        [SerializeField] private float m_rotAccel;

        // Eventually, these should be the only necessary exposed variables for movement components
        [SerializeField] private float m_maxWheelPower;
        [SerializeField] private float m_weightSlowdownFactor;

        [SerializeField] private int temp_TotalBotWeight = -1;
        
        private float m_leftCurrentPower = 0.0f;
        private float m_rightCurrentPower = 0.0f;
        private float m_leftTargetPower = 0.0f;
        private float m_rightTargetPower = 0.0f;

        // To assist in determining which point to rotate around
        // TODO: For a robot with 4 wheels, have an empty gameobject for each side, with the wheels for that side as its children
        [SerializeField] private Transform m_leftSideCenter;
        [SerializeField] private Transform m_rightSideCenter;
        private Rigidbody m_body;

        /// <summary>
        /// Domestic initialization
        /// </summary>
        private void Awake()
        {
            m_body = GetComponentInParent<Rigidbody>();

            if (temp_TotalBotWeight > 0 && m_weightSlowdownFactor != 0)
            {
                SetWeight(temp_TotalBotWeight);
            }
        }

        /// <summary>
        /// Takes care of moving the robot body given the power of the wheels
        /// </summary>
        private void Update()
        {
            float dt = Time.deltaTime;

            CalculateWheelPower(dt);

            if (m_leftCurrentPower > 0 && m_rightCurrentPower > 0)
            {
                m_body.velocity = Mathf.Min(m_leftCurrentPower, m_rightCurrentPower) * m_body.transform.forward;
            }
            else if (m_leftCurrentPower < 0 && m_rightCurrentPower < 0)
            {
                m_body.velocity = Mathf.Max(m_leftCurrentPower, m_rightCurrentPower) * m_body.transform.forward;
            }

            float dRot = m_rotAccel * (m_leftCurrentPower - m_rightCurrentPower) / m_maxWheelPower;
            Vector3 rotatePos = RotationPoint();
            m_body.transform.RotateAround(rotatePos, Vector3.up, dRot);
            m_body.velocity = Quaternion.Euler(0, dRot, 0) * m_body.velocity;
        }

        /// <summary>
        /// Changes the power supplied to the specified wheels on this object
        /// 
        /// Pre Conditions: The provided actionIndex is within the range [0,1]
        /// Post Conditions: The designated action occurs
        /// </summary>
        /// <param name="actionIndex">What specific action should this part take</param>
        /// <param name="value">Any information passed with the input</param>
        public void DoPartAction(byte actionIndex, CustomInputData value)
        {
            // All part actions on Basic Wheels must be bound to an analog stick
            if (value.Get<Vector2>() == null)
            {
                Debug.LogError("Attempting to move wheels of " + name + " has failed because InputValue does not contain a Vector2");
                return;
            }

            // Are deadzones set through Unity editor or through code?
            // If they need to be addressed via code, set up a deadzone so that only input of magnitude greater than 0.4(?) is accepted and values are interpolated from there?
            float temp_roundedInput = Mathf.Round(value.Get<Vector2>().y * 10) / 10.0f;

            // case 0: Move left wheel(s)
            // case 1: Move right wheel(s)
            switch (actionIndex)
            {
                case 0:
                    //Debug.Log("Left input: " + temp_roundedInput);
                    m_leftTargetPower = temp_roundedInput * m_maxWheelPower;
                    break;
                case 1:
                    //Debug.Log("Right input: " + temp_roundedInput);
                    m_rightTargetPower = temp_roundedInput * m_maxWheelPower;
                    break;
                default:
                    Debug.Log("Invalid action index for part " + name);
                    break;
            }
        }

        public void SetWeight(int totalBotWeight)
        {
            m_acceleration = MovementConstants.ACCELERATION_CONSTANT / (totalBotWeight * m_weightSlowdownFactor);
            m_rotAccel = MovementConstants.ROTATION_CONSTANT / (totalBotWeight * m_weightSlowdownFactor);
            m_friction = totalBotWeight * m_weightSlowdownFactor / MovementConstants.FRICTION_CONSTANT;
        }

        /// <summary>
        /// Called every update to calculate the power supplied to each wheel given the most recent target power
        /// </summary>
        private void CalculateWheelPower(float dt)
        {
            CustomDebug.Log("Calculating Wheel Power with dt = " + dt, IS_DEBUGGING);
            CustomDebug.Log("Left Current: " + m_leftCurrentPower + "\nLeft Target: " + m_leftTargetPower, IS_DEBUGGING);
            CustomDebug.Log("Right Current: " + m_rightCurrentPower + "\nRight Target: " + m_rightTargetPower, IS_DEBUGGING);
            string dmsg;

            // Left Wheel Check for Different Signs
            if (Mathf.Sign(m_leftTargetPower) != Mathf.Sign(m_leftCurrentPower) && !(m_leftTargetPower == 0 || m_leftCurrentPower == 0))
            {
                // In this case, the left wheel is both accelerating in the target direction and affected by friction against the current direction, but shouldn't snap to the target

                dmsg = "Left is accelerating ";
                dmsg += (Mathf.Sign(m_leftTargetPower) == 1) ? "forward" : "backward";
                CustomDebug.Log(dmsg, IS_DEBUGGING);
                dmsg = "Left is affected by ";
                dmsg += (Mathf.Sign(m_leftCurrentPower) == -1) ? "forward friction" : "backward friction";
                CustomDebug.Log(dmsg, IS_DEBUGGING);

                m_leftCurrentPower += Mathf.Sign(m_leftTargetPower) * m_acceleration * dt - Mathf.Sign(m_leftCurrentPower) * m_friction * dt;
            }
            else
            {
                // Left Wheel Normal Acceleration Check
                if (Mathf.Abs(m_leftTargetPower) > Mathf.Abs(m_leftCurrentPower))
                {
                    // In this case, the left wheel is accelerating in the target direction and should snap to the target if it would exceed the target

                    dmsg = "Left is accelerating ";
                    dmsg += (Mathf.Sign(m_leftTargetPower) == 1) ? "forward" : "backward";
                    CustomDebug.Log(dmsg, IS_DEBUGGING);

                    m_leftCurrentPower += Mathf.Sign(m_leftTargetPower) * m_acceleration * dt;
                    if (Mathf.Abs(m_leftCurrentPower) > Mathf.Abs(m_leftTargetPower) && Mathf.Sign(m_leftCurrentPower) == Mathf.Sign(m_leftTargetPower))
                    {
                        m_leftCurrentPower = m_leftTargetPower;
                    }
                }
                // Left Wheel Friction Check
                else if (Mathf.Abs(m_leftTargetPower) < Mathf.Abs(m_leftCurrentPower))
                {
                    // In this case, the left wheel is affected by friction against the current direction and should snap to the target if it would drop below it

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

            // Right Wheel Check for Different Signs
            if (Mathf.Sign(m_rightTargetPower) != Mathf.Sign(m_rightCurrentPower) && !(m_rightTargetPower == 0 || m_rightCurrentPower == 0))
            {
                // In this case, the right wheel is both accelerating in the target direction and affected by friction against the current direction, but shouldn't snap to the target

                dmsg = "Right is accelerating ";
                dmsg += (Mathf.Sign(m_rightTargetPower) == 1) ? "forward" : "backward";
                CustomDebug.Log(dmsg, IS_DEBUGGING);
                dmsg = "Right is affected by ";
                dmsg += (Mathf.Sign(m_rightCurrentPower) == -1) ? "forward friction" : "backward friction";
                CustomDebug.Log(dmsg, IS_DEBUGGING);

                m_rightCurrentPower += Mathf.Sign(m_rightTargetPower) * m_acceleration * dt - Mathf.Sign(m_rightCurrentPower) * m_friction * dt;
            }
            else
            {
                // Right Wheel Normal Acceleration Check
                if (Mathf.Abs(m_rightTargetPower) > Mathf.Abs(m_rightCurrentPower))
                {
                    // In this case, the right wheel is accelerating in the target direction and should snap to the target if it would exceed the target

                    dmsg = "Right is accelerating ";
                    dmsg += (Mathf.Sign(m_rightTargetPower) == 1) ? "forward" : "backward";
                    CustomDebug.Log(dmsg, IS_DEBUGGING);

                    m_rightCurrentPower += Mathf.Sign(m_rightTargetPower) * m_acceleration * dt;
                    if (Mathf.Abs(m_rightCurrentPower) > Mathf.Abs(m_rightTargetPower) && Mathf.Sign(m_rightCurrentPower) == Mathf.Sign(m_rightTargetPower))
                    {
                        m_rightCurrentPower = m_rightTargetPower;
                    }
                }
                // Right Wheel Friction Check
                else if (Mathf.Abs(m_rightTargetPower) < Mathf.Abs(m_rightCurrentPower))
                {
                    // In this case, the right wheel is affected by friction against the current direction and should snap to the target if it would drop below it

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

        /// <summary>
        /// Determines what point the robot should rotate around while turning
        /// </summary>
        /// <returns>The point the robot will rotate around</returns>
        private Vector3 RotationPoint()
        {
            Vector3 center = Vector3.Lerp(m_leftSideCenter.position, m_rightSideCenter.position, 0.5f);
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
                minPowerPosition = m_rightSideCenter.position;
            }
            else
            {
                maxPower = Mathf.Abs(m_rightCurrentPower);
                minPower = Mathf.Abs(m_leftCurrentPower);
                minPowerPosition = m_leftSideCenter.position;
            }
            return Vector3.Lerp(center, minPowerPosition, (maxPower + minPower) / maxPower);
        }
    }
}
