using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// This script gets the last current input and displays the icons that the last input device was
/// </summary>
public class DetectingDevices : MonoBehaviour
{
    [SerializeField] private PlayerInput m_playerInput = null;

    // Determing Xbox or PS
    [Header("Xbox")]
    // Xbox Gamepad
    [SerializeField] private Sprite m_xboxNorth = null;
    [SerializeField] private Sprite m_xboxSouth = null;
    [SerializeField] private Sprite m_xboxEast = null;
    [SerializeField] private Sprite m_xboxWest = null;
    [Header("PS")]
    // Playstation
    [SerializeField] private Sprite m_psNorth = null;
    [SerializeField] private Sprite m_psSouth = null;
    [SerializeField] private Sprite m_psEast = null;
    [SerializeField] private Sprite m_psWest = null;

    [Header("Main Menu Sprites")]
    [SerializeField] private GameObject m_playSouth = null;
    [SerializeField] private GameObject m_settingsNorth = null;
    [SerializeField] private GameObject m_creditsWest = null;
    [SerializeField] private GameObject m_exitEast = null;

    [Header("Credits Menu Sprites")]
    [SerializeField] private GameObject m_creditsBack = null;

    [Header("Settings Menu Sprites")]
    [SerializeField] private GameObject m_fullscreen = null;
    [SerializeField] private GameObject m_apply = null;
    [SerializeField] private GameObject m_back = null;

    [Header("Exit Menu Sprites")]
    [SerializeField] private GameObject m_quit = null;
    [SerializeField] private GameObject m_cancel = null;

    void Start()
    {
        XboxControls();
    }

    private void Update()
    {
        ControllerIconsToUse();
    }

    /// <summary>
    /// Decides what controller icons to use depending on the last input device used
    /// </summary>
    private void ControllerIconsToUse()
    {
        if (m_playerInput.devices.Count <= 0) { return; }
        //Debug.Log(m_playerInput.devices[0]);
        if (m_playerInput.devices[0].ToString() == "XInputControllerWindows:/XInputControllerWindows"
            || m_playerInput.devices[0].ToString() == "Keyboard:/Keyboard")
        {
            XboxControls();
        }
        else
        {
            PSControls();
        }
    }


    private void XboxControls()
    {
        // Main Menu
        m_settingsNorth.GetComponent<Image>().sprite = m_xboxNorth;
        m_playSouth.GetComponent<Image>().sprite = m_xboxSouth;
        m_exitEast.GetComponent<Image>().sprite = m_xboxEast;
        m_creditsWest.GetComponent<Image>().sprite = m_xboxWest;
        // Credits
        m_creditsBack.GetComponent<Image>().sprite = m_xboxEast;
        // Settings
        m_fullscreen.GetComponent<Image>().sprite = m_xboxNorth;
        m_apply.GetComponent<Image>().sprite = m_xboxSouth;
        m_back.GetComponent<Image>().sprite = m_xboxEast;
        // Quit
        m_quit.GetComponent<Image>().sprite = m_xboxSouth;
        m_cancel.GetComponent<Image>().sprite = m_xboxEast;
    }

    private void PSControls()
    {
        // Main Menu
        m_settingsNorth.GetComponent<Image>().sprite = m_psNorth;
        m_playSouth.GetComponent<Image>().sprite = m_psSouth;
        m_exitEast.GetComponent<Image>().sprite = m_psEast;
        m_creditsWest.GetComponent<Image>().sprite = m_psWest;
        // Credits
        m_creditsBack.GetComponent<Image>().sprite = m_psEast;
        // Settings
        m_fullscreen.GetComponent<Image>().sprite = m_psNorth;
        m_apply.GetComponent<Image>().sprite = m_psSouth;
        m_back.GetComponent<Image>().sprite = m_psEast;
        // Quit
        m_quit.GetComponent<Image>().sprite = m_psSouth;
        m_cancel.GetComponent<Image>().sprite = m_psEast;
    }
}
