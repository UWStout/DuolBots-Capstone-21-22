using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Toggles in game options menu - does not pause game
/// </summary>
public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject m_OptionsMenu;
    
    void Awake()
    {
        m_OptionsMenu.SetActive(false);
    }

    public void ActiveOptionsMenu()
    {
        //time still runs
        m_OptionsMenu.SetActive(true);
    }


    public void DeactiveOptionsMenu()
    {
        m_OptionsMenu.SetActive(false);
    }

}
