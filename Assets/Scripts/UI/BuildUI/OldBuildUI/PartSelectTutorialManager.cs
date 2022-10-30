using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using DuolBots; // Gross

public class PartSelectTutorialManager : MonoBehaviour
{
    [SerializeField] [ReadOnly] private SingletonTutorialStateManager m_tutorialManager = null;
    public bool isTutorialActive => m_isTutorialActive;
    [SerializeField] private bool m_isTutorialActive = false;
    [SerializeField] private Transform m_tutorialRoot = null;
    [SerializeField] public GameObject m_activePanel => m_tutorialPanels[m_panelIndex];
    [SerializeField] private List<GameObject> m_tutorialPanels = null;
    [SerializeField] private int m_panelIndex = 0;
    [SerializeField] private int m_tutorialIndex = 0;

    private void Start()
    {
        CustomDebug.LogWarning("Calling start");
        m_tutorialManager = SingletonTutorialStateManager.instance;
        if (m_tutorialManager == null) { m_isTutorialActive = false; return; }
        m_isTutorialActive = m_tutorialManager.isTutorialActive;
        m_tutorialIndex = m_tutorialManager.tutorialIndex;
        if (!m_isTutorialActive) { return; }
        StartTutorial();
    }

    public void SetTutorialStatus(bool state)
    {
        m_isTutorialActive = state;
    }

    public void StartTutorial()
    {
        if (!m_isTutorialActive) { Debug.Log($"Tutorial is off."); return; }
        if (m_tutorialManager != null)
            m_tutorialIndex = m_tutorialManager.tutorialIndex;
        if (m_tutorialIndex != 0 && m_tutorialIndex != 1) { Debug.Log($"Phase index is not a possible index: {m_tutorialIndex}"); return; }
        if (m_tutorialPanels.Count > 0) { ResetInfo(); }
        Transform temp_panelHolder = m_tutorialRoot.GetChild(m_tutorialIndex);
        if (temp_panelHolder == null) { Debug.Log("Panel holder is null."); return; }
        if (temp_panelHolder.childCount <= 0) { Debug.Log($"{temp_panelHolder.name} has no children."); return; }
        GetAllTutorialPanels(temp_panelHolder);
        m_tutorialPanels[m_panelIndex].SetActive(true);
    }

    public void EndTutorial()
    {
        m_isTutorialActive = false;
        ResetInfo();
        m_tutorialManager.EndTutorial();
    }

    public void OnSelect()
    {
        if (!m_tutorialPanels[m_panelIndex].GetComponent<TutorialPanelSettings>().persistentPanel)
        {
            m_tutorialPanels[m_panelIndex].SetActive(false);
            NextPanel();
        }
    }

    private void GetAllTutorialPanels(Transform panelHolder)
    {
        Debug.Log(panelHolder.name);
        foreach (Transform panel in panelHolder)
        {
            m_tutorialPanels.Add(panel.gameObject);
        }
    }

    [Button(enabledMode: EButtonEnableMode.Editor)]
    public void NextPanel()
    {
        if (m_tutorialPanels.Count > m_panelIndex + 1)
        {
            m_tutorialPanels[m_panelIndex].SetActive(false);
            m_tutorialPanels[++m_panelIndex].SetActive(true);
        }
    }

    [Button(enabledMode: EButtonEnableMode.Editor)]
    private void GetManagerInfoAtPhase()
    {
        if (m_tutorialPanels.Count <= 0)
        {
            StartTutorial();
        }
    }

    [Button(enabledMode: EButtonEnableMode.Editor)]
    private void ResetInfo()
    {
        foreach (Transform panel in m_tutorialRoot.GetChild(m_tutorialIndex))
        {
            panel.gameObject.SetActive(false);
        }
        m_tutorialPanels.Clear();
        m_panelIndex = 0;
    }
}
