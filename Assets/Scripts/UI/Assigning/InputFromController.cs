using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Original Authors - Cole Woulf and Ben Lussman
namespace DuolBots
{
    public class InputFromController : MonoBehaviour
    {
        // the scene manager gameobject in the scene
        private GameObject sceneManager;
        // the player input on the button object
        [SerializeField] PlayerInput m_playerInput;

        // assigns the scene manager object from the scene
        private void Awake()
        {
            sceneManager = GameObject.Find("SceneManager");
        }

        /// <summary>
        /// Changes the action map being used depending on the action Type the part/button has
        /// </summary>
        /// <param name="actionType"></param>
        public void changeActionMap(eActionType actionType)
        {
            m_playerInput.SwitchCurrentActionMap(actionType.ToString());
            Debug.Log(m_playerInput.currentActionMap.ToString());
        }

        #region PlayerInput Messages
        // These each are called by Unity's new PlayerInput's messages.
        // They all just call RobotInputController's OnPlayerInput function, but change the eInputType.
        //
        // Pre Conditions - Assumes that there is a PlayerInput component attached to this GameObject.
        // Assumes active input map is an input map that has actions
        // with the exact same names as the below functions without the 'On' prefix.
        // Post Conditions - Passes the input information along to the RobotInputController
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
        private void OnSelecting(InputValue value) => OnInput(value, eInputType.select);
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
        /// Post Conditions - Passes the input information along to the RobotInputController
        /// using its OnPlayerInput function.
        /// </summary>
        /// <param name="inputType">Type of input.</param>
        private void OnInput(InputValue value, eInputType inputType)
        {
                if (Mathf.Abs(value.Get<float>()) > 0.5f)
                {
                    sceneManager.GetComponent<ControlUIScriptableObjectImplement>().SaveInfoOntoButton(inputType, value);
                    m_playerInput.SwitchCurrentActionMap("PartSelection");
                }
        }
        #endregion PlayerInput Messages
    }
}
