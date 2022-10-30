using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
// Original Authors - Zach Gross and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// PartInput for general movement components
    /// </summary>
    public class Input_Movement : MonoBehaviour, IPartInput
    {
        private const bool IS_DEBUGGING = false;

        // Unique string id for the movement component
        public string uniqueID => m_uniqueID.value;
        [SerializeField] StringID m_uniqueID = null;

        private IController_Movement m_movementController = null;

        // Domestic Initializaton
        private void Awake()
        {
            m_movementController = GetComponent<IController_Movement>();
        }

        /// <summary>
        /// Implementation from IPartInput
        /// Called when input assigned to this part is received
        /// Pre Conditions:
        ///     actionIndex is either 0 or 1
        /// Post Conditions:
        ///     Calls the designated helper method given the action index
        /// </summary>
        /// <param name="actionIndex"></param>
        /// <param name="value"></param>
        public void DoPartAction(byte actionIndex, CustomInputData value)
        {
            switch (actionIndex)
            {
                case 0:
                    LeftInput(value);
                    break;
                case 1:
                    RightInput(value);
                    break;
                default:
                    Debug.LogError("Invalid action index for part " + name);
                    break;
            }
        }

        /// <summary>
        /// Receives data passed with the input for the left side and rounds it to the nearest 0.1 before passing it to the correct controller
        /// Pre Conditions:
        ///     The attached m_movementController is not null
        ///     Called from DoPartAction when the designated input is received
        /// Post Conditions:
        ///     Passes the rounded data to m_movementController
        /// </summary>
        /// <param name="value">The value passed with the input</param>
        private void LeftInput(CustomInputData value)
        {
            float temp_roundedInput = Mathf.Round(value.Get<float>() * 10) / 10.0f;
            m_movementController.SetLeftTarget(temp_roundedInput);
        }

        /// <summary>
        /// Receives data passed with the input for the right side and rounds it to the nearest 0.1 before passing it to the correct controller
        /// Pre Conditions:
        ///     The attached m_movementController is not null
        ///     Called from DoPartAction when the designated input is received
        /// Post Conditions:
        ///     Passes the rounded data to m_movementController
        /// </summary>
        /// <param name="value">The value passed with the input</param>
        private void RightInput(CustomInputData value)
        {
            float temp_roundedInput = Mathf.Round(value.Get<float>() * 10) / 10.0f;
            m_movementController.SetRightTarget(temp_roundedInput);
        }
    }
}
