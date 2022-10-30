using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PartAssignPlayerInputManager : MonoBehaviour
{
    private ControlUIScriptableObjectImplement m_control = null;
    private ControllerMovement m_controllerMovement = null;
    [SerializeField] private Color m_highlightColor = Color.blue;

    // Start is called before the first frame update
    void Start()
    {
        m_controllerMovement = GameObject.Find("SceneManager").GetComponent<ControllerMovement>();
        m_control = GameObject.Find("SceneManager").GetComponent<ControlUIScriptableObjectImplement>();
    }

    public void OnPlayerJoined(PlayerInput player)
    {
        Debug.Log("Player has joined");
        m_control.m_botPartsList[m_controllerMovement.activeRow].scrollListSpace.transform.GetChild(0).GetComponent<Image>().color = m_highlightColor;
    }
}
