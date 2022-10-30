using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Original Author - Cole Woulf
public class HighlightParts : MonoBehaviour
{
    [SerializeField] private GameObject m_botPart;

    private AssignControl_PopupManager m_AssignControl_PopupManager;

    /// <summary>
    /// Highlights the corresponding part but only when the button is selected,
    /// so if player is choosing a new rebind in the dropdown, it is deselected
    /// </summary>
    private void Update()
    {


       /* m_AssignControl_PopupManager = GameObject.Find("SceneManager").GetComponent<AssignControl_PopupManager>();

        if(!m_AssignControl_PopupManager.GetifActive())
        {
            HighlightOff();
        }*/
    }

    /// <summary>
    /// Switches the layer to the highlight layer which gives it an outline
    /// </summary>
    public void HighlightOn()
    {
        m_botPart.layer = LayerMask.NameToLayer("Outlined");
    }

    /// <summary>
    /// Switches the layer to the default layer which has no outline
    /// </summary>
    public void HighlightOff()
    {
       m_botPart.layer = LayerMask.NameToLayer("Default");
    }
}
