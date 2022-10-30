using System.Collections;
using System.Collections.Generic;
using DuolBots;
using UnityEngine;
using UnityEngine.Assertions;

public class PartSelectTutorialConditionCheck : MonoBehaviour
{
    [SerializeField] private PartSelectionRow m_selectionRow = null;
    [SerializeField] private PartSelectorManager m_partSelector = null;
    [SerializeField] private PartScriptableObject m_camelChassisSO = null;
    [SerializeField] private PartScriptableObject m_camelChassisLegSO = null;
    [SerializeField] private PartScriptableObject m_nelsonTurretSO = null;
    [SerializeField] private int m_frontSlotIndex = 0;
    [SerializeField] private byte m_playerIndex = 0;
    public bool isChassisSelect = false;
    public bool isMovementSelect = false;
    public bool onCamelChassis = false;
    public bool onCamelLeg = false;
    public bool onNelsonTurret = false;
    public bool onFrontSlot = false;

    private void Awake()
    {
        m_partSelector = PartSelectorManager.instance;
        Assert.IsNotNull(m_camelChassisSO, $"{this.name}: Camel chassis SO is null or missing");
        Assert.IsNotNull(m_camelChassisLegSO, $"{this.name}: Camel chassis SO is null or missing");
        Assert.IsNotNull(m_nelsonTurretSO, $"{this.name}: Camel chassis SO is null or missing");
    }

    private void Start()
    {
        m_playerIndex = this.GetComponent<PlayerIndex>().playerIndex;
        m_selectionRow = GameObject.Find($"P{m_playerIndex + 1}ScrollViewManager").GetComponent<PartSelectionRow>();
        Assert.IsNotNull(m_selectionRow, $"{this.name}: Part selection row is null or missing");
    }

    private void Update()
    {
        if (m_selectionRow == null) { return; }
        isChassisSelect = !m_partSelector.isChassisSelected;
        if (!isChassisSelect)
        {
            isMovementSelect = !m_partSelector.isMovementSelected;
        }
        if (m_selectionRow.activeBox.name == m_camelChassisSO.name) { onCamelChassis = true; }
        else { onCamelChassis = false; }
        if (m_selectionRow.activeBox.name == m_camelChassisLegSO.name) { onCamelLeg = true; }
        else { onCamelLeg = false; }
        if (m_selectionRow.activeBox.name == m_nelsonTurretSO.name) { onNelsonTurret = true; }
        else { onNelsonTurret = false; }
        if (m_partSelector.activeSlotIndex == 0) { onFrontSlot = true; }
        else { onFrontSlot = false; }
    }
}
