using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class TutorialPanelSettings : MonoBehaviour
{
    public bool persistentPanel => m_persistentPanel;
    [SerializeField] private bool m_persistentPanel = false;
    public bool allowInput => m_allowInput;
    [SerializeField] private bool m_allowInput = false;
    public bool allowMove => m_move;
    [SerializeField] [ShowIf("m_allowInput")] private bool m_move = false;
    public bool allowSelect => m_select;
    [SerializeField] [ShowIf("m_allowInput")] private bool m_select = false;
    public bool allowConfirm => m_confirm;
    [SerializeField] [ShowIf("m_allowInput")] private bool m_confirm = false;
    public bool allowCancel => m_cancel;
    [SerializeField] [ShowIf("m_allowInput")] private bool m_cancel = false;
    public bool allowShift => m_shift;
    [SerializeField] [ShowIf("m_allowInput")] private bool m_shift = false;
    public bool allowLook => m_look;
    [SerializeField] [ShowIf("m_allowInput")] private bool m_look = false;
}
