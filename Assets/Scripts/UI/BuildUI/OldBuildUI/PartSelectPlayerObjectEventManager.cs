using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;
using static UnityEngine.InputSystem.InputAction;
using NaughtyAttributes;
// Original Author(s) - Eslis Vang

public class PartSelectPlayerObjectEventManager : MonoBehaviour
{
    private const bool IS_DEBUGGING = false;

    private PartSelectorManager m_partSelector = null;
    private bool m_isPhaseTwo = false;
    [SerializeField] private PartSelectTutorialManager m_tutorialManager = null;
    [SerializeField] private PartSelectTutorialConditionCheck m_conditions = null;
    [SerializeField] private PartSelectionRow m_selectionRow = null;
    [SerializeField] private PlayerInput m_playerInput = null;
    [SerializeField] private ChassisSelectionCinemachineCameraTarget[] m_cameras = null;
    [SerializeField] [ReadOnly] private CinemachineFreeLook m_freeLookCamera = null;
    [SerializeField] [ReadOnly] private CinemachineVirtualCamera m_virtalCamera = null;
    [SerializeField] private ConfirmPanel m_chassisConfirmPanel = null;

    private void Awake()
    {
        m_conditions = GetComponent<PartSelectTutorialConditionCheck>();
        m_tutorialManager = FindObjectOfType<PartSelectTutorialManager>();
        Assert.IsNotNull(m_tutorialManager, $"{this.name}: Tutorial Manager is null");
        m_chassisConfirmPanel = FindObjectOfType<ConfirmPanel>(true);
        Assert.IsNotNull(m_chassisConfirmPanel, $"{this.name}: Chassis confirm panel is missing or null.");
        m_partSelector = PartSelectorManager.instance;
        Assert.IsNotNull(m_partSelector, $"{this.name}: Part Selector is null.");

        m_playerInput = this.GetComponent<PlayerInput>();
        m_selectionRow = GameObject.Find($"P{m_playerInput.playerIndex + 1}ScrollViewManager").GetComponent<PartSelectionRow>();
        m_cameras = FindObjectsOfType<ChassisSelectionCinemachineCameraTarget>(true);
    }

    private void Start()
    {
        foreach (ChassisSelectionCinemachineCameraTarget camObj in m_cameras)
        {
            if (camObj.gameObject.TryGetComponent<CinemachineFreeLook>(out CinemachineFreeLook temp_freeLookCam))
            {
                m_freeLookCamera = temp_freeLookCam;
            }
            if (camObj.gameObject.TryGetComponent<CinemachineVirtualCamera>(out CinemachineVirtualCamera temp_virtalCam))
            {
                m_virtalCamera = temp_virtalCam;
            }
        }
    }

    private void Update()
    {
        if (m_isPhaseTwo) { return; }
        if (m_partSelector.isChassisSelected && m_partSelector.isMovementSelected)
        {
            m_isPhaseTwo = true;
        }
    }

    public void OnMove(InputValue value)
    {
        if (m_tutorialManager.isTutorialActive)
        {
            // Checks these only if the tutorial is active
            // If there is an active tutorial panel and if the panel allows the player to move
            if (m_tutorialManager.m_activePanel.activeSelf && !m_tutorialManager.m_activePanel.GetComponent<TutorialPanelSettings>().allowMove) { return; }
            // If the player is choosing a chassis during the tutorial, freeze them if they hover over the camel chassis
            if (m_conditions.isChassisSelect && m_conditions.onCamelChassis && !m_selectionRow.isAskingConfirmation) { return; }
            // If the player is choosing a movement part during the tutorial, freeze them if they hover over the camel legs
            if (m_conditions.isMovementSelect && m_conditions.onCamelLeg && !m_selectionRow.isAskingConfirmation) { return; }
            if (SingletonTutorialStateManager.instance.tutorialIndex == 1 && m_isPhaseTwo && m_conditions.onNelsonTurret && m_partSelector.playerTurnIndex == m_selectionRow.playerIndex && m_tutorialManager.m_activePanel.GetComponent<TutorialPanelSettings>().allowMove) { m_tutorialManager.NextPanel(); return; }
        }
        // If it isn't the player's turn and it's still in phase one (chassis or movement selection), freeze them
        if (m_partSelector.playerTurnIndex != m_selectionRow.playerIndex && !m_isPhaseTwo) { return; }
        // If a confirmation panel is open traverse the panel instead
        if (m_selectionRow.isAskingConfirmation)
        {
            m_chassisConfirmPanel.Move(value);
            return;
        }
        // If players are choosing parts (chassis or movement selection)
        if (!m_isPhaseTwo)
        {
            CustomDebug.Log($"P{m_playerInput.playerIndex + 1} Moved", IS_DEBUGGING);
            m_selectionRow.OnMoveInput(value);
        }
        // If players are choosing parts (not chassis or movement selection)
        if (m_isPhaseTwo)
        {
            m_selectionRow.OnMoveInput(value);
            return;
        }

        m_virtalCamera.GetComponent<ChassisSelectionCinemachineCameraTarget>().SetNewCameraTarget(m_partSelector.GetActiveColumn(m_partSelector.playerTurnIndex), m_partSelector.playerTurnIndex);
    }

    public void OnSelect()
    {
        if (m_tutorialManager.isTutorialActive)
        {
            if (SingletonTutorialStateManager.instance.tutorialIndex == 1 && m_isPhaseTwo && m_conditions.onNelsonTurret && m_conditions.onFrontSlot && m_partSelector.playerTurnIndex == m_selectionRow.playerIndex && m_tutorialManager.m_activePanel.GetComponent<TutorialPanelSettings>().allowSelect) { m_selectionRow.OnSelection(); m_tutorialManager.NextPanel(); return; }
            // Checks these only if the tutorial is active
            // If there is an active tutorial panel and if the panel allows the player to select
            if (m_tutorialManager.m_activePanel.activeSelf && !m_tutorialManager.m_activePanel.GetComponent<TutorialPanelSettings>().allowSelect)
            {
                m_tutorialManager.OnSelect();
                return;
            }
            // If the player is selecting the chassis and are not over the camel chassis, disable selection
            if (m_conditions.isChassisSelect && !m_conditions.onCamelChassis) { return; }
            // If the player is selecting movement and are not over the camel legs, disable selection
            if (m_conditions.isMovementSelect && !m_conditions.onCamelLeg) { return; }
        }
        // If a confirmation panel is open and if the player's index is the same as the current player's turn
        if (m_selectionRow.isAskingConfirmation && m_partSelector.playerTurnIndex == m_selectionRow.playerIndex && m_chassisConfirmPanel.gameObject.activeSelf)
        {
            ChassisSelectionCinemachineCameraTarget temp_vCam = m_virtalCamera.GetComponent<ChassisSelectionCinemachineCameraTarget>();
            if (!temp_vCam.CheckIfValidVirtualCam()) { return; }
            int temp_value = m_chassisConfirmPanel.Select();
            m_selectionRow.SetSelection(temp_value);

            if (temp_value == 0)
            {
                temp_vCam.ResetVirtualCamera();
            }
            return;
        }
        if (!m_isPhaseTwo && m_partSelector.playerTurnIndex == m_selectionRow.playerIndex)
        {
            CustomDebug.Log($"P{m_playerInput.playerIndex + 1} Selected", IS_DEBUGGING);
            m_selectionRow.OnSelection();
        }
        if (m_isPhaseTwo)
        {
            m_selectionRow.OnSelection();
        }
    }

    public void OnConfirm()
    {
        if (m_tutorialManager.isTutorialActive)
        {
            // If there is an active tutorial panel and if the panel allows the player to confirm
            if (m_tutorialManager.m_activePanel.activeSelf && !m_tutorialManager.m_activePanel.GetComponent<TutorialPanelSettings>().allowConfirm) { return; }
            // If the players press confirm on the introduction of the tutorial, end the tutorial
            if (m_tutorialManager.m_activePanel.activeSelf && m_tutorialManager.m_activePanel.GetComponent<TutorialPanelSettings>().allowConfirm && m_tutorialManager.m_activePanel.transform.GetSiblingIndex() == 0) { m_tutorialManager.EndTutorial(); return; }
        }
        if (!m_isPhaseTwo && m_partSelector.playerTurnIndex == m_selectionRow.playerIndex)
        {
            CustomDebug.Log($"P{m_playerInput.playerIndex + 1} Confirm", IS_DEBUGGING);
            m_selectionRow.OnConfirm();
        }
        if (m_isPhaseTwo)
        {
            m_selectionRow.OnConfirm();
        }
    }

    public void OnCancel()
    {
        if (m_tutorialManager.isTutorialActive)
            // If there is an active tutorial panel and if the panel allows the player to cancel
            if (m_tutorialManager.m_activePanel.activeSelf && !m_tutorialManager.m_activePanel.GetComponent<TutorialPanelSettings>().allowCancel) { return; }
        if (m_selectionRow.playerIndex == m_partSelector.playerTurnIndex && m_selectionRow.isAskingConfirmation)
        {
            m_chassisConfirmPanel.Cancel();
        }
        CustomDebug.Log($"P{m_playerInput.playerIndex + 1} Cancel", IS_DEBUGGING);
        m_selectionRow.OnCancel();
    }

    public void OnShift(InputValue value)
    {
        if (m_tutorialManager.isTutorialActive)
        {
            if (SingletonTutorialStateManager.instance.tutorialIndex == 1 && m_isPhaseTwo && !m_conditions.onFrontSlot && m_partSelector.playerTurnIndex == m_selectionRow.playerIndex && m_tutorialManager.m_activePanel.GetComponent<TutorialPanelSettings>().allowShift) { m_selectionRow.OnShift(value); }
            if (SingletonTutorialStateManager.instance.tutorialIndex == 1 && m_isPhaseTwo && m_conditions.onFrontSlot && m_partSelector.playerTurnIndex == m_selectionRow.playerIndex && m_tutorialManager.m_activePanel.GetComponent<TutorialPanelSettings>().allowShift) { m_tutorialManager.NextPanel(); return; }
            if (SingletonTutorialStateManager.instance.tutorialIndex == 1 && m_isPhaseTwo && !m_conditions.onFrontSlot && m_partSelector.playerTurnIndex == m_selectionRow.playerIndex && m_tutorialManager.m_activePanel.GetComponent<TutorialPanelSettings>().allowShift) { return; }
            // If there is an active tutorial panel and if the panel allows the player to shift
            if (m_tutorialManager.m_activePanel.activeSelf && !m_tutorialManager.m_activePanel.GetComponent<TutorialPanelSettings>().allowShift) { return; }
        }
        if (!m_isPhaseTwo) { return; }
        CustomDebug.Log($"P{m_playerInput.playerIndex + 1} Shift", IS_DEBUGGING);
        m_selectionRow.OnShift(value);
    }

    public void OnLook(InputValue value)
    {
        if (m_tutorialManager.isTutorialActive)
            // If there is an active tutorial panel and if the panel allows the player to look
            if (m_tutorialManager.m_activePanel.activeSelf && !m_tutorialManager.m_activePanel.GetComponent<TutorialPanelSettings>().allowLook) { return; }
        if (!m_isPhaseTwo) { return; }
        CustomDebug.Log($"P{m_playerInput.playerIndex + 1} Look", IS_DEBUGGING);
        m_freeLookCamera.GetComponent<ChassisSelectionCinemachineCameraTarget>().UpdateAxisValue(value);
    }
}
