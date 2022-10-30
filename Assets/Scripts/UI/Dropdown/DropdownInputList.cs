using DuolBots;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownInputList : MonoBehaviour
{
    [SerializeField] private Dropdown m_dropdown;
    private byte m_isPlayerOne;
    
    private void Start()
    {
        PopulateList();
        
        //Add listener for when the value of the Dropdown changes, to take action
        m_dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged();
        });
        
    }

    private void DropdownValueChanged()
    {
        m_isPlayerOne = GameObject.Find("SceneManager").GetComponent<ControlUIScriptableObjectImplement>().GetisPlayerOne();
    }

    /// <summary>
    /// Pre-Condition: The list is empty 
    /// Post-Conditon: The list has all the enums from the eInputType Emun
    /// Populates a dropdown list with the Enums from the eInputType enums 
    /// </summary>
    private void PopulateList()
    {
        string[] enumNames = Enum.GetNames(typeof(eInputType));
        List<string> names = new List<string>(enumNames);

        m_dropdown.AddOptions(names);
    }

    public byte GetisPlayerOne()
    {
        // hardcode this needs to fix later
        Debug.Log(m_dropdown.GetComponent<Image>().color.ToString());
        switch (m_dropdown.GetComponent<Image>().color.ToString())
        {
            case ("RGBA(0.760, 0.320, 0.000, 1.000)"):
                return 1;
            case ("RGBA(0.000, 0.420, 0.650, 1.000)"):
                return 0;
        }

        return m_isPlayerOne;
    }
}
