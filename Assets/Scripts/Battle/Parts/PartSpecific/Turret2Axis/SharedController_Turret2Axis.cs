using System;
using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Shared functionality for the Local and Network controllers for the Turret2Axis.
    /// </summary>
    [RequireComponent(typeof(Specifications_Turret2Axis))]
    public class SharedController_Turret2Axis : MonoBehaviour, IWwiseEventInvoker
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        // Specifications for variables
        private Specifications_Turret2Axis m_specifications = null;

        private float m_curRotateAngle = 0.0f;
        private float m_curRaiseAngle = 0.0f;

        private bool m_isRotating = false;
        private bool m_isRaising = false;
        private bool m_isPlayingSound = false;

        public event Action<WwiseEventName, GameObject> requestInvokeWwiseEvent;


        // Domestic Initialization
        private void Awake()
        {
            m_specifications = GetComponent<Specifications_Turret2Axis>();
            Assert.IsNotNull(m_specifications,
                $"{typeof(Specifications_Turret2Axis).Name} " +
                $"was not on {name} but is required by {GetType().Name}");


            // Get the starting angles
            Vector3 temp_rotateEulerAngles = AngleHelpers.RestrictAngles(
                m_specifications.rotateTrans.localEulerAngles);
            Vector3 temp_raiseEulerAngles = AngleHelpers.RestrictAngles(
                m_specifications.raiseTrans.localEulerAngles);

            m_curRotateAngle = m_specifications.axisToRotate.GetValueFromEulerAngles(
                temp_rotateEulerAngles);
            m_curRaiseAngle = m_specifications.axisToRaise.GetValueFromEulerAngles(
                temp_raiseEulerAngles);

            CustomDebug.Log($"Starting Rotate Angle {m_curRotateAngle} " +
                $"where actual eulerAngles are {temp_rotateEulerAngles}",
                IS_DEBUGGING);
            CustomDebug.Log($"Starting Raise Angle {m_curRaiseAngle} " +
                $"where actual eulerAngles are {temp_raiseEulerAngles}",
                IS_DEBUGGING);
        }


        /// <summary>
        /// Should be called every frame. Rotates the current based on the given input value.
        /// </summary>
        public void RotateTurret(float rotationInput)
        {
            if (rotationInput == 0.0f)
            {
                m_isRotating = false;
                UpdateSound();
                return;
            }

            rotationInput = m_specifications.invertRotateInput ? -rotationInput : rotationInput;

            CustomDebug.Log($"Rotating the turret with input of {rotationInput}", IS_DEBUGGING);

            eRotationAxis temp_rotAxis = m_specifications.axisToRotate;
            Transform temp_rotateTrans = m_specifications.rotateTrans;
            float temp_rotateSpeed = m_specifications.rotateSpeed;
            float temp_minAngle = m_specifications.minRotateAngle;
            float temp_maxAngle = m_specifications.maxRotateAngle;

            ChangeAngleBasedOnRotationAxis(temp_rotAxis, temp_rotateTrans,
                temp_rotateSpeed, temp_minAngle, temp_maxAngle, rotationInput,
                ref m_curRotateAngle, ref m_isRotating);
        }
        /// <summary>
        /// Should be called every frame. Raises the barrel based on the given input value.
        /// </summary>
        public void RaiseBarrel(float raiseInput)
        {
            if (raiseInput == 0.0f)
            {
                m_isRaising = false;
                UpdateSound();
                return;
            }

            raiseInput = m_specifications.invertRaiseInput ? -raiseInput : raiseInput;

            CustomDebug.Log($"Raising the turret's barrel with input of {raiseInput}",
                IS_DEBUGGING);

            eRotationAxis temp_raiseAxis = m_specifications.axisToRaise;
            Transform temp_raiseTrans = m_specifications.raiseTrans;
            float temp_raiseSpeed = m_specifications.raiseSpeed;
            float temp_minAngle = m_specifications.minRaiseAngle;
            float temp_maxAngle = m_specifications.maxRaiseAngle;

            ChangeAngleBasedOnRotationAxis(temp_raiseAxis, temp_raiseTrans,
                temp_raiseSpeed, temp_minAngle, temp_maxAngle, raiseInput,
                ref m_curRaiseAngle, ref m_isRaising);
        }


        /// <summary>
        /// Changes the rotation of the specified rotateTransform on the given axis
        /// with the given parameters.
        ///
        /// Pre Conditions - rotTrans is not null, min angle is less than max angle and
        /// bot min angle and max angle are between -180.0f and 180.0f.
        /// Post Conditions - Rotates the given transform along the corresponding axis.
        /// </summary>
        /// <param name="rotAxis">Axis to rotate (x, y, or z).</param>
        /// <param name="rotTrans">Transform to change the rotation of.</param>
        /// <param name="rotSpeed">How fast to rotate about the axis.</param>
        /// <param name="minAngle">Minimum angle to clamp the rotation to.</param>
        /// <param name="maxAngle">Maximum angle to clamp the rotation to.</param>
        /// <param name="rotInput">User input axis value.</param>
        private void ChangeAngleBasedOnRotationAxis(eRotationAxis rotAxis,
            Transform rotTrans, float rotSpeed, float minAngle, float maxAngle,
            float rotInput, ref float curAngle, ref bool isRotateOrRaise)
        {
            float temp_changeInAngle = rotInput * rotSpeed * Time.deltaTime;
            CustomDebug.Log($"Changing angle {curAngle} by {temp_changeInAngle}. " +
                $"Current euler angles {rotTrans.localEulerAngles}", IS_DEBUGGING);

            curAngle += temp_changeInAngle;
            bool temp_isFullSwivel = 360.0f <= maxAngle - minAngle;
            // Assume we aren't at an extreme so we are rotating/raising
            isRotateOrRaise = true;
            if (curAngle <= minAngle)
            {
                curAngle = temp_isFullSwivel ? maxAngle : minAngle;
                // If we are at an extreme, if we continue rotating or
                // raising is based on is we can spin full swivel
                isRotateOrRaise = temp_isFullSwivel;
            }
            else if (curAngle >= maxAngle)
            {
                curAngle = temp_isFullSwivel ? minAngle : maxAngle;
                // If we are at an extreme, if we continue rotating or
                // raising is based on is we can spin full swivel
                isRotateOrRaise = temp_isFullSwivel;
            }
            // Update the sound
            UpdateSound();
            //curAngle = Mathf.Clamp(curAngle + temp_changeInAngle, minAngle, maxAngle);
            //AngleHelpers.ClampRestrictAngle(curAngle + temp_changeInAngle, minAngle, maxAngle);

            Vector3 temp_prevEulerAngles = rotTrans.localEulerAngles;
            Vector3 temp_curEulerAngles = rotAxis.ReplaceValueInEulerAngles(
                temp_prevEulerAngles, curAngle);

            rotTrans.localEulerAngles = temp_curEulerAngles;

            #region Logs
            CustomDebug.Log($"Results of change. New curAngle {curAngle}. " +
                $"Non-set euler angles {temp_curEulerAngles}. " +
                $"Euler angles after set {rotTrans.localEulerAngles}",
                IS_DEBUGGING);
            #endregion Logs
        }

        private void UpdateSound()
        {
            // Sounds is already playing, check if we should stop
            if (m_isPlayingSound)
            {
                // If we are rotating or raising, don't stop the sound
                if (m_isRotating || m_isRaising) { return; }

                // If we are not rotating or raising, end the sound
                requestInvokeWwiseEvent?.Invoke(
                    m_specifications.stopStateWwiseEventName,
                    m_specifications.soundObj.gameObject);
                m_isPlayingSound = false;
            }
            // Sound is not playing, check if we should start it
            else
            {
                // If we not rotating or raising, don't start the sound
                if (!m_isRotating && !m_isRaising) { return; }

                // If we are rotating or raising start the sound
                requestInvokeWwiseEvent?.Invoke(
                    m_specifications.beginStateWwiseEventName,
                    m_specifications.soundObj.gameObject);
                m_isPlayingSound = true;
            }
        }
    }
}
