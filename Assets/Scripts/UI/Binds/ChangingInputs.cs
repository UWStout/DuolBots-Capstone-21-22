using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
// Original Author - Cole Woulf

public class ChangingInputs : MonoBehaviour
{
    private PlayerInput m_playerInput;
    //private Menu_InputActions m_Menu_InputActions;

    private void Awake()
    {
        m_playerInput = GetComponent<PlayerInput>();
        //m_Menu_InputActions = new Menu_InputActions();
    }

    private void Update()
    {
        
    }

    public void ChangeInput()
    {
        //m_Menu_InputActions.Disable();
        //m_Menu_InputActions.TempBattleControls
    }
}
