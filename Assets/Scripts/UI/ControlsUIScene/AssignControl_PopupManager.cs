using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Original Author - Shelby Vian

/// <summary>
/// Toggles control assign panel
/// </summary>
public class AssignControl_PopupManager : MonoBehaviour
{
    [SerializeField]
    private GameObject AssignScreen;


    void Start()
    {
        AssignScreen_Deactive();
    }

    /// <summary>
    /// Activates control assign panel
    /// </summary>
    public void AssignScreen_Active()
    {
        AssignScreen.SetActive(true);
    }

    /// <summary>
    /// Deactivates control assign panel
    /// </summary>
    public void AssignScreen_Deactive()
    {
        //AssignScreen.SetActive(false);
    }

    public bool GetifActive()
    {
        return AssignScreen.activeSelf;
    }
}
