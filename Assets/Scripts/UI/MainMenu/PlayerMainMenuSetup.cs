using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class PlayerMainMenuSetup : MonoBehaviour
{
    private EventSystem m_eventSystem = null;
    [SerializeField] GameObject m_firstSelectedButton = null;

    private void Awake()
    {
        m_eventSystem = FindObjectOfType<EventSystem>();
    }

    public void SetFirstSelected()
    {
        m_eventSystem.SetSelectedGameObject(m_firstSelectedButton);
        //Debug.Log(m_firstSelectedButton);
    }
}
