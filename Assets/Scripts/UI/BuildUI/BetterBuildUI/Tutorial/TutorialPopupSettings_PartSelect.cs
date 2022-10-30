using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NaughtyAttributes;
// Original Authors - Eslis Vang

namespace DuolBots
{
    /// <summary>
    /// Types of inputs in the build scene.
    /// </summary>
    public enum eBetterBuildSceneInput
    {
        Confirm,
        Cycle,
        Unconfirm,
        Attach,
        Move,
        ReadyUp
    }

    public class TutorialPopupSettings_PartSelect : MonoBehaviour
    {
        private const bool IS_DEBUGGING = true;
        // public UnityEvent popupEvents => m_popupEvents;
        // [SerializeField] private UnityEvent m_popupEvents = null;

        #region IsPersistent
        // If the popup should be persistent.
        public bool isPersistent => m_isPersistent;
        [DisableIf(EConditionOperator.And, "CheckIfAutoHide", "m_hideAfterInput")]
        [SerializeField] private bool m_isPersistent = false;

        // The state when the popup should show up.
        public eBetterBuildSceneState onlyShowPersistentDuringState =>
            m_onlyShowPersistentDuringState;
        [Dropdown("GetSceneStates"), ShowIf(EConditionOperator.And,
            "m_isPersistent", "ResetOtherStates"), SerializeField]
        private eBetterBuildSceneState m_onlyShowPersistentDuringState;

        /// <summary>
        /// Sets <paramref name="m_isPersistent"/> to <see langword="false"/> on the
        /// condition that <paramref name="m_hideAfterInput"/> is found to be
        /// <see langword="true"/>.
        /// </summary>
        /// <returns>The state of <paramref name="m_hideAfterInput"/>.</returns>
        private bool CheckIfAutoHide()
        {
            if (!m_hideAfterInput) return false;
            m_isPersistent = false;
            return true;
        }
        #endregion

        #region HideAfterInput
        // If the popup should hide after an input is read.
        public bool hideAfterInput => m_hideAfterInput;
        [DisableIf(EConditionOperator.And, "CheckIfPersistent", "m_isPersistent")]
        [SerializeField] private bool m_hideAfterInput = false;

        // The state when the popup should show up.
        public eBetterBuildSceneState hideAfterInputOnState =>
            m_hideAfterInputOnState;
        [Dropdown("GetSceneStates"), ShowIf(EConditionOperator.And,
            "m_hideAfterInput", "ResetOtherStates")]
        [SerializeField] private eBetterBuildSceneState m_hideAfterInputOnState;

        /// <summary>
        /// Sets <paramref name="m_hideAfterInput"/> to <see langword="false"/> on
        /// the condition that <paramref name="m_isPersistent"/> is found to be
        /// <see langword="true"/>.
        /// </summary>
        /// <returns>The state of <paramref name="m_isPersistent"/>.</returns>
        private bool CheckIfPersistent()
        {
            if (!m_isPersistent) return false;
            m_hideAfterInput = false;
            return true;
        }
        #endregion

        #region OnlyAllowSpecificInputs
        // If the popup should only react to specified inputs.
        public bool onlyAllowSpecificInputs => m_onlyAllowSpecificInputs;
        [SerializeField] private bool m_onlyAllowSpecificInputs = false;

        // The state when the popup should react to specified inputs.
        public eBetterBuildSceneState allowInputInState => m_allowInputInState;
        [SerializeField, ShowIf(EConditionOperator.And, "m_onlyAllowSpecificInputs",
            "ResetOtherStates"), DisableIf("CheckIfPersistentOrAutoHide"),
            Dropdown("GetSceneStates")]
        private eBetterBuildSceneState m_allowInputInState;

        // Conditions for if the state is selected.
        private bool m_chassisState = false;
        private bool m_movementState = false;
        private bool m_partState = false;

        #region ChassisInputs
        // Possible inputs during chassis state.
        [SerializeField, BoxGroup("Chassis Controls"),
            ShowIf(EConditionOperator.And, "CheckState", "m_chassisState",
            "CheckDropDownsAreTheSame"), Label("Confirm Selection")]
        private bool m_confirmSelectionChassis = false;

        [SerializeField, BoxGroup("Chassis Controls"),
            ShowIf(EConditionOperator.And, "CheckState", "m_chassisState",
            "CheckDropDownsAreTheSame"), Label("Cycle")]
        private bool m_cycleChassis = false;

        [SerializeField, BoxGroup("Chassis Controls"),
            ShowIf(EConditionOperator.And, "CheckState", "m_chassisState",
            "CheckDropDownsAreTheSame"), Label("Unconfirm Selection")]
        private bool m_unconfirmSelectionChassis = false;
        #endregion

        #region MovementInputs
        // Possible inputs during movement state.
        [SerializeField, BoxGroup("Movement Controls"),
            ShowIf(EConditionOperator.And, "CheckState", "m_movementState",
            "CheckDropDownsAreTheSame"), Label("Confirm Selection")]
        private bool m_confirmSelectionMovement = false;

        [SerializeField, BoxGroup("Movement Controls"),
            ShowIf(EConditionOperator.And, "CheckState", "m_movementState",
            "CheckDropDownsAreTheSame"), Label("Cycle")]
        private bool m_cycleMovement = false;

        [SerializeField, BoxGroup("Movement Controls"),
            ShowIf(EConditionOperator.And, "CheckState", "m_movementState",
            "CheckDropDownsAreTheSame"), Label("Unconfirm Selection")]
        private bool m_unconfirmSelectionMovement = false;
        #endregion

        #region PartInputs
        // Possible inputs during part selection state.
        [SerializeField, BoxGroup("Part Controls"),
            ShowIf(EConditionOperator.And, "CheckState", "m_partState",
            "CheckDropDownsAreTheSame"), Label("Attach Part")]
        private bool m_attachPart = false;

        [SerializeField, BoxGroup("Part Controls"),
            ShowIf(EConditionOperator.And, "CheckState", "m_partState",
            "CheckDropDownsAreTheSame"), Label("Cycle")]
        private bool m_cyclePart = false;

        [SerializeField, BoxGroup("Part Controls"),
            ShowIf(EConditionOperator.And, "CheckState", "m_partState",
            "CheckDropDownsAreTheSame"), Label("Move")]
        private bool m_movePart = false;

        [SerializeField, BoxGroup("Part Controls"),
            ShowIf(EConditionOperator.And, "CheckState", "m_partState",
            "CheckDropDownsAreTheSame"), Label("Ready Up")]
        private bool m_readyUpPart = false;
        #endregion PartInputs

        /// <summary>
        /// Checks if <paramref name="m_isPersistent"/> or
        /// <paramref name="m_hideAfterInput"/> are <see langword="true"/>.
        /// </summary>
        /// <returns><see langword="true"/> if either
        /// <paramref name="m_isPersistent"/> or <paramref name="m_hideAfterInput"/>
        /// are <see langword="true"/>.</returns>
        private bool CheckIfPersistentOrAutoHide()
        {
            if (m_isPersistent)
            {
                m_allowInputInState = m_onlyShowPersistentDuringState;
                return true;
            }
            if (m_hideAfterInput)
            {
                m_allowInputInState = m_hideAfterInputOnState;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks what state is selected to allow inputs.
        /// </summary>
        /// <returns>The state of <paramref name="m_onlyAllowSpecificInputs"/>.
        /// </returns>
        private bool CheckState()
        {
            m_chassisState = false;
            m_movementState = false;
            m_partState = false;

            switch (m_allowInputInState)
            {
                case eBetterBuildSceneState.Chassis:
                    m_chassisState = true;
                    break;
                case eBetterBuildSceneState.Movement:
                    m_movementState = true;
                    break;
                case eBetterBuildSceneState.Part:
                    m_partState = true;
                    break;
            }
            if (m_onlyAllowSpecificInputs)
            {
                return true;
            }
            return false;
        }

        private bool ResetOtherStates()
        {
            if (m_allowInputInState == eBetterBuildSceneState.Chassis)
            {
                m_confirmSelectionMovement = false;
                m_cycleMovement = false;
                m_unconfirmSelectionMovement = false;
                m_attachPart = false;
                m_cyclePart = false;
                m_movePart = false;
                m_readyUpPart = false;
            }
            if (m_allowInputInState == eBetterBuildSceneState.Movement)
            {
                m_confirmSelectionChassis = false;
                m_cycleChassis = false;
                m_unconfirmSelectionChassis = false;
                m_attachPart = false;
                m_cyclePart = false;
                m_movePart = false;
                m_readyUpPart = false;
            }
            if (m_allowInputInState == eBetterBuildSceneState.Part)
            {
                m_confirmSelectionChassis = false;
                m_cycleChassis = false;
                m_unconfirmSelectionChassis = false;
                m_confirmSelectionMovement = false;
                m_cycleMovement = false;
                m_unconfirmSelectionMovement = false;
            }
            if (m_allowInputInState == eBetterBuildSceneState.Assign)
            {
                m_confirmSelectionChassis = false;
                m_cycleChassis = false;
                m_unconfirmSelectionChassis = false;
                m_confirmSelectionMovement = false;
                m_cycleMovement = false;
                m_unconfirmSelectionMovement = false;
                m_attachPart = false;
                m_cyclePart = false;
                m_movePart = false;
                m_readyUpPart = false;
            }
            if (m_allowInputInState == eBetterBuildSceneState.End)
            {
                m_confirmSelectionChassis = false;
                m_cycleChassis = false;
                m_unconfirmSelectionChassis = false;
                m_confirmSelectionMovement = false;
                m_cycleMovement = false;
                m_unconfirmSelectionMovement = false;
                m_attachPart = false;
                m_cyclePart = false;
                m_movePart = false;
                m_readyUpPart = false;
            }

            return true;
        }

        #endregion


        #region DropdownHelpers
        /// <summary>
        /// Gets a dropdown list of all <see cref="eBetterBuildSceneState"/>.
        /// </summary>
        /// <returns>All possible states in <see cref="eBetterBuildSceneState"/>.
        /// </returns>
        private DropdownList<eBetterBuildSceneState> GetSceneStates()
        {
            return new DropdownList<eBetterBuildSceneState>
            {
                { "Chassis", eBetterBuildSceneState.Chassis },
                { "Post Chassis", eBetterBuildSceneState.PostChassis },
                { "Movement", eBetterBuildSceneState.Movement },
                { "Post Movement", eBetterBuildSceneState.PostMovement },
                { "Part", eBetterBuildSceneState.Part },
                { "Assign", eBetterBuildSceneState.Assign },
                { "End", eBetterBuildSceneState.End }
            };
        }

        /// <summary>
        /// Checks if the dropdowns are the same.
        /// </summary>
        /// <returns><see langword="true"/> if the dropdowns are the same.
        /// </returns>
        private bool CheckDropDownsAreTheSame()
        {
            if (m_isPersistent && m_onlyShowPersistentDuringState ==
                m_allowInputInState) return true;
            if (m_hideAfterInput && m_hideAfterInputOnState ==
                m_allowInputInState) return true;
            if (!m_isPersistent && !m_hideAfterInput) return true;
            return false;
        }

        /// <summary>
        /// Checks if the dropdowns are not the same.
        /// </summary>
        /// <returns><see langword="true"/> if the dropdowns are not the same.
        /// </returns>
        private bool DropDownsAreNotTheSame()
        {
            if (!m_onlyAllowSpecificInputs) return false;
            if (m_onlyShowPersistentDuringState == m_allowInputInState)
                return false;
            if (m_hideAfterInputOnState == m_allowInputInState) return false;
            return true;
        }
        #endregion

        /// <summary>
        /// Queries if specified input type is allowed.
        /// </summary>
        /// <param name="inputType">The type of input being queried for.</param>
        /// <returns>The state of the <paramref name="inputType"/>
        /// from the corresponding game state.</returns>
        public bool QueryAllowedInput(eBetterBuildSceneInput inputType)
        {
            if (!CheckState()) return false;
            if (m_chassisState)
            {
                switch (inputType)
                {
                    case eBetterBuildSceneInput.Confirm:
                        if (m_confirmSelectionChassis) return true;
                        break;
                    case eBetterBuildSceneInput.Cycle:
                        if (m_cycleChassis) return true;
                        break;
                    case eBetterBuildSceneInput.Unconfirm:
                        if (m_unconfirmSelectionChassis) return true;
                        break;
                }
            }
            if (m_movementState)
            {
                switch (inputType)
                {
                    case eBetterBuildSceneInput.Confirm:
                        if (m_confirmSelectionMovement) return true;
                        break;
                    case eBetterBuildSceneInput.Cycle:
                        if (m_cycleMovement) return true;
                        break;
                    case eBetterBuildSceneInput.Unconfirm:
                        if (m_unconfirmSelectionMovement) return true;
                        break;
                }
            }
            if (m_partState)
            {
                switch (inputType)
                {
                    case eBetterBuildSceneInput.Attach:
                        if (m_attachPart) return true;
                        break;
                    case eBetterBuildSceneInput.Cycle:
                        if (m_cyclePart) return true;
                        break;
                    case eBetterBuildSceneInput.Move:
                        if (m_movePart) return true;
                        break;
                    case eBetterBuildSceneInput.ReadyUp:
                        if (m_readyUpPart) return true;
                        break;
                }
            }
            #region Logs
            CustomDebug.Log($"Query for {inputType.GetType()}.{Enum.GetName(typeof(eBetterBuildSceneInput), inputType)} was not found in the given", IS_DEBUGGING); 
            #endregion
            return false;
        } 
    }
}
