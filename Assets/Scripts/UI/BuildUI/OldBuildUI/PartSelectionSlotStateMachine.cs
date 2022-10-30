using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartSelectionSlotStateMachine : MonoBehaviour
{
    [SerializeField] private GameObject[] m_selectionHighlights = new GameObject[2];
    [SerializeField] private GameObject[] m_hoverHighlights = new GameObject[2];

    public void ToggleHoverHiglight(int playerIndex)
    {
        m_selectionHighlights[playerIndex].SetActive(!m_selectionHighlights[playerIndex].activeSelf);
    }

    public void SetHoverLight(int playerIndex, bool newState)
    {
        m_selectionHighlights[playerIndex].SetActive(newState);
    }

    public void ToggleSelectionHighlight(int playerIndex)
    {
        m_hoverHighlights[playerIndex].SetActive(!m_hoverHighlights[playerIndex].activeSelf);
    }

    public void SetSelectionHighlight(int playerIndex, bool newState)
    {
        m_hoverHighlights[playerIndex].SetActive(newState);
    }
}
