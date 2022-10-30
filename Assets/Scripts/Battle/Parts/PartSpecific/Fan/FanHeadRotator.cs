using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using NaughtyAttributes;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Handles rotation of the Fan's "head".
    /// </summary>
    public class FanHeadRotator : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        [SerializeField] private FanProjectileFireController m_controller = null;
        [SerializeField] private Transform m_objectToRotate = null;
        // Values to rotate by.
        [SerializeField] [MinMaxSlider(1f, 8f)] private Vector2 m_rotationRange = new Vector2(1f, 8f);
        // The axis of rotation being used
        [SerializeField] [Dropdown(nameof(m_dropdownList))] private byte m_axisOfRotation = 0;

        private DropdownList<byte> m_dropdownList = new NaughtyAttributes.DropdownList<byte>()
        {
            { "X", 0 },
            { "Y", 1 },
            { "Z", 2 }
        };
        private List<bool> m_rotationAxes = new List<bool>() { false, false, false };
        private float m_curRotation = 1f;
        private bool m_isRotating = false;
        private Coroutine m_rotateCorutine = null; private char dum = 'X';

        private void Awake()
        {
            Assert.IsNotNull(m_objectToRotate, $"{name} does not have a fan head " +
                $"{typeof(GameObject)} specified but requires one.");
            // Subscribe to the Fan's charge events
            m_controller.onStartedCharging += StartRotation;
            m_controller.onFinishedCharging += StopRotation;
            // Initialize appropriate axis of rotation based off of Dropwdown serialization
            CustomDebug.Log($"Axis of rotation: {m_axisOfRotation}", IS_DEBUGGING);
            switch(m_axisOfRotation)
            {
                case 0:
                    m_rotationAxes[0] = true;
                    break;
                case 1:
                    m_rotationAxes[1] = true;
                    break;
                case 2:
                    m_rotationAxes[2] = true;
                    break;
                default:
                    // Something has gone seriously wrong.
                    break;
            }
            CustomDebug.Log($"{name} is using the {m_axisOfRotation} axis for rotating {m_objectToRotate}.", IS_DEBUGGING);
            CustomDebug.Log($"The contents of {nameof(m_rotationAxes)} are: {m_rotationAxes[0]}," +
                $" {m_rotationAxes[1]}, { m_rotationAxes[2]}", IS_DEBUGGING);
            CustomDebug.Log($"Wouldn't it be crazy if {m_axisOfRotation} wasn't 'X','Y', or 'Z' but instead all of them?" +
                $"Is it? {m_axisOfRotation == 0 && m_axisOfRotation == 1 && m_axisOfRotation == 2}", IS_DEBUGGING);
        }

        private void OnDisable()
        {
            // In case the object is destroyed before OnDisable() gets called.
            if(m_controller != null)
            {
                m_controller.onStartedCharging -= StartRotation;
                m_controller.onFinishedCharging -= StopRotation;
            }
        }

        public void SetRotationSpeed(float percentageOfCharge)
        {
            m_curRotation = percentageOfCharge * (m_rotationRange.y - m_rotationRange.x) + m_rotationRange.x;
            Mathf.Clamp(m_curRotation, m_rotationRange.x, m_rotationRange.y);
            CustomDebug.Log($"Setting rotation speed on {name} to be {m_curRotation} " +
                $"using percentage of charge: {percentageOfCharge} and a " +
                $"range of rotation speeds of ({m_rotationRange.x}-{m_rotationRange.y})", IS_DEBUGGING);
        }

        private void StartRotation()
        {
            if (m_isRotating) { return; }
            m_isRotating = true;
            m_rotateCorutine = StartCoroutine(Rotate());
        }
        private void StopRotation()
        {
            if (!m_isRotating) { return; }
            m_isRotating = false;
            StopCoroutine(m_rotateCorutine);
        }

        private IEnumerator Rotate()
        {
            // Rotate by the curRotation at the given axis of rotation.
            while(m_isRotating)
            {
                Vector3 temp_rotationAmount = new Vector3(m_rotationAxes[0] ? m_curRotation : 0,
                    m_rotationAxes[1] ? m_curRotation : 0, m_rotationAxes[2] ? m_curRotation : 0);
                m_objectToRotate.Rotate(temp_rotationAmount);
                CustomDebug.Log($"{name} is rotating {m_objectToRotate.name} at " +
                $"{temp_rotationAmount}", IS_DEBUGGING);
                yield return null;
            }
            yield return null;
        }
    }
}
