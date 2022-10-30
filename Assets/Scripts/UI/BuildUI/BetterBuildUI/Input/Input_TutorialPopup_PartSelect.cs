using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DuolBots
{
    public class Input_TutorialPopup_PartSelect : MonoBehaviour
    {
        private const bool IS_DEBUGGING = true;

        ToggleTutorialPopup_PartSelect[] m_tutorialPopups = null;
        BetterBuildSceneStateManager m_stateMan = null;

        private void Awake()
        {
            m_tutorialPopups = FindObjectsOfType<ToggleTutorialPopup_PartSelect>();
        }

        private void Start()
        {
            m_stateMan = BetterBuildSceneStateManager.instance;
        }

        private void OnAny()
        {
            foreach (ToggleTutorialPopup_PartSelect popup in m_tutorialPopups)
            {
                // Continue in the case that:
                // If popup is persistent.
                if (popup.popupSettings.isPersistent) continue;
                // If popup is only looking for specific inputs.
                if (popup.popupSettings.onlyAllowSpecificInputs) continue;
                // If popup is not supposed to hide after input.
                if (!popup.popupSettings.hideAfterInput) continue;
                // If current state doesn't match when the popup should hide.
                if (popup.popupSettings.hideAfterInputOnState != m_stateMan.
                    curState) continue;
                popup.HidePopup();
            }
        }

        // May not even use this due to over-engineering.
        #region Inputs
        private void OnConfirmSelection()
        {
            CheckInput(eBetterBuildSceneInput.Confirm, InputConfirmSelection);
        }

        private void InputConfirmSelection()
        {
            // Do confirm selection.
        }

        private void OnCycle()
        {
            CheckInput(eBetterBuildSceneInput.Cycle, InputCycle);
        }

        private void InputCycle()
        {

        }

        private void OnUnconfirmSelection()
        {
            CheckInput(eBetterBuildSceneInput.Unconfirm, InputUnconfirmSelection);
        }

        private void InputUnconfirmSelection()
        {

        }

        private void OnAttach()
        {
            CheckInput(eBetterBuildSceneInput.Attach, InputAttach);
        }

        private void InputAttach()
        {

        }

        private void OnMove()
        {
            CheckInput(eBetterBuildSceneInput.Move, InputMove);
        }

        private void InputMove()
        {

        }

        private void OnReadyUp()
        {
            CheckInput(eBetterBuildSceneInput.ReadyUp, InputReadyUp);
        }

        private void InputReadyUp()
        {

        }
        #endregion

        private void CheckInput(eBetterBuildSceneInput inputType, Action inputFunction)
        {
            foreach (ToggleTutorialPopup_PartSelect popup in m_tutorialPopups)
            {
                // Continue in the case that:
                // The popup is not active.
                if (!popup.gameObject.activeSelf) continue;
                // The popup is not looking for specific inputs.
                if (!popup.popupSettings.onlyAllowSpecificInputs) continue;
                // If the input has not been enabled.
                if (!popup.popupSettings.QueryAllowedInput(inputType)) continue;
                #region Log
                CustomDebug.Log($"<color=green>{m_stateMan.curState}</color>: {Enum.GetName(typeof(eBetterBuildSceneInput), inputType)}", IS_DEBUGGING);
                #endregion
                // if (popup.popupSettings.popupEvents != null)
                //     popup.popupSettings.popupEvents.Invoke();
                // If the popup should not hide after input.
                if (!popup.popupSettings.hideAfterInput) continue;
                popup.HidePopup();
            }
        }
    }
}
