using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using NaughtyAttributes;
using DuolBots;
using TMPro;

public class PartInfoPanel : MonoBehaviour
{
    private const bool IS_DEBUGGING = true;

    [SerializeField] [ReadOnly] private PartSelectorManager m_partSelector = null;
    [SerializeField] private bool m_updateStatBarOne = false;
    [SerializeField] private bool m_updateStatBarTwo = false;
    [SerializeField] private bool m_updateStatBarThree = false;
    [SerializeField] [BoxGroup("Text")] private TextMeshPro[] m_statSubTitles = null;
    [SerializeField] [BoxGroup("Text")] private TextMeshPro m_statTitle = null;
    [SerializeField] [BoxGroup("Stats")] private PartScriptableObject m_part = null;
    [SerializeField] [BoxGroup("Stats")] private Image[] m_statBars = null;
    [SerializeField] [BoxGroup("Stats")] private float m_fillSpeed = 1f;
    [SerializeField] [BoxGroup("Stats")] [ReadOnly] private float[] m_TargetValues = null;
    [SerializeField] private PartDatabase m_partDatabase = null;

    private void Awake()
    {
        if (PartSelectorManager.instance == null) { CustomDebug.Log($"{this.name}: Part Selector is null or missing", IS_DEBUGGING); return; }
        m_partSelector = PartSelectorManager.instance;
        m_partDatabase = PartDatabase.instance;
        Assert.IsNotNull(m_partDatabase, $"{this.name}: Part Database singleton is null or missing.");

        m_TargetValues = new float[] { 0f, 0f, 0f };
        if (m_statBars == null) { return; }
        if (m_statBars.Length != 3) { return; }
        for (int i = 0; i < m_TargetValues.Length; i++) { m_statBars[i].fillAmount = m_TargetValues[i]; }
    }

    private void Update()
    {
        if (m_partSelector == null) { return; }
        m_part = m_partDatabase.GetPartScriptableObject(m_partSelector.activeSelectionRow.activeBox.name);
        //UpdateInfoPanel();
    }

    public void SetInfoPanel(PartScriptableObject part)
    {
        if (part == null) { return; }
        if (!m_partSelector.isChassisSelected)
        {
            m_TargetValues[0] = part.health;
            m_TargetValues[1] = part.weight;
            m_TargetValues[2] = (float)part.modelPrefab.GetComponent<SlotPlacementManager>().GetSlotAmount();
        }
        else if (!m_partSelector.isMovementSelected)
        {
            m_TargetValues[0] = part.health;
            m_TargetValues[1] = part.weight;
            m_TargetValues[2] = part.movementSpeed;
        }
    }

    public void UpdateInfoPanel()
    {
        if (m_updateStatBarOne && m_statBars[0].fillAmount <= m_TargetValues[0])
        {
            m_statBars[0].fillAmount += Mathf.Lerp(0f, m_TargetValues[0], Time.deltaTime * m_fillSpeed);
        }
        else
        {
            m_statBars[0].fillAmount += Mathf.Lerp(0f, m_TargetValues[0], Time.deltaTime * m_fillSpeed);
        }

        if (m_updateStatBarTwo && m_statBars[1].fillAmount <= m_TargetValues[1])
        {
            m_statBars[1].fillAmount += Mathf.Lerp(0f, m_TargetValues[1], Time.deltaTime * m_fillSpeed);
        }
        else
        {
            m_statBars[1].fillAmount -= Mathf.Lerp(0f, m_TargetValues[1], Time.deltaTime * m_fillSpeed);
        }

        if (!m_partSelector.isChassisSelected) { return; }

        if (m_updateStatBarThree && m_statBars[2].fillAmount <= m_TargetValues[2])
        {
            m_statBars[2].fillAmount += Mathf.Lerp(0f, m_TargetValues[2], Time.deltaTime * m_fillSpeed);
        }
        else
        {
            m_statBars[2].fillAmount -= Mathf.Lerp(0f, m_TargetValues[2], Time.deltaTime * m_fillSpeed);
        }

        if (m_updateStatBarOne && m_updateStatBarTwo && m_updateStatBarThree)
        {

        }
    }

    public void ResetInfoPanel()
    {
        m_statTitle.text = "";
        foreach (TextMeshPro textBox in m_statSubTitles) { textBox.text = ""; }
        foreach (Image statBar in m_statBars) { statBar.fillAmount = 0.0f; }
    }
}
