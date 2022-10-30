using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DuolBots;
//Original Authors - Shelby Vian

/// <summary>
/// Class sorts through the CustomInputBinding list to identify the action names for the active part of each player and change the UI text
/// </summary>
public class PlayerAssignedActions : MonoBehaviour
{
    [SerializeField] private GameObject m_fireIcon, m_axisIcon;
    [SerializeField] private SwapControllerIcons m_swap;
    private List<CustomInputBinding> m_inputs = new List<CustomInputBinding>();
  //  [SerializeField] private List<PartScriptableObject> m_scriptableObject = new List<PartScriptableObject>();
    private byte m_team = 0;
    [SerializeField] private byte player = 0;

    PartDatabase temp_partDatabase;

    void Awake()
    {
        m_inputs = (List<CustomInputBinding>)BuildSceneInputData.GetInputBindingsForPlayer(m_team);

        temp_partDatabase = PartDatabase.instance;
    }

    /// <summary>
    /// Sorts through List to find correct actions
    /// </summary>
    /// <param name="slot"> Slot index of current active part</param>
   public void FindControlsOfActivePart(byte slot)
    {
        m_fireIcon.SetActive(false);
        m_axisIcon.SetActive(false);

        foreach (CustomInputBinding bind in m_inputs)
        {
            if(bind.partSlotID == slot && bind.playerIndex == player)
            {
               // Debug.Log("input type: " + bind.inputType + " for " + bind.playerIndex + " " + temp_partDatabase.GetPartScriptableObject(bind.partUniqueID).actionList[bind.actionIndex].action);
                if(bind.inputType == eInputType.buttonEast)
                {
                    m_fireIcon.GetComponentInChildren<TextMeshProUGUI>().text = temp_partDatabase.GetPartScriptableObject(bind.partUniqueID).actionList[bind.actionIndex].action;
                    m_fireIcon.SetActive(true);
                    
                }
                else
                {
                    m_axisIcon.GetComponentInChildren<TextMeshProUGUI>().text = temp_partDatabase.GetPartScriptableObject(bind.partUniqueID).actionList[bind.actionIndex].action;
                    m_axisIcon.SetActive(true);
                }
            }

        }
       // m_swap.SwapIcons(player);
    }

}
