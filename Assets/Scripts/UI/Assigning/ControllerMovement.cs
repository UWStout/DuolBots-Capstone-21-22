using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.Assertions;

// Original Authors - Cole Woulf and Eslis Vang
public class ControllerMovement : MonoBehaviour
{
    // Need to find a way to get actual colomn size
    [SerializeField] private int temp_activeColomnSize = 6;

    // A getter of the active row the player is in
    public int activeRow => temp_activeRow;
    [SerializeField] private int temp_activeRow = 0;

    // Need to find a way to get actual number of buttons
    [SerializeField] private int temp_activeButtonSize = 3;

    // A getter of the active button row in use
    public int activeButtonRow => temp_activeButtonRow;
    [SerializeField] private int temp_activeButtonRow = 0;

    //An array of Assigning Contorl Script
    private AssigningControl[] m_assigningControls = new AssigningControl[2];

    private AssignCamera m_assignCamera;

    private void Start()
    {
        m_assignCamera = FindObjectOfType<AssignCamera>();    
    }

    /// <summary>
    /// finds each player object and assinging them to the Assigning Control array
    /// </summary>
    private void Update()
    {
        if (m_assigningControls[0] == null && GameObject.Find("PlayerObj1"))
        {
            GameObject.Find("PlayerObj1").TryGetComponent<AssigningControl>(out m_assigningControls[0]);
        }
        if (m_assigningControls[1] == null && GameObject.Find("PlayerObj2"))
        {
            GameObject.Find("PlayerObj2").TryGetComponent<AssigningControl>(out m_assigningControls[1]);
        }

        if(m_assignCamera.freeLook != null)
            m_assignCamera.SetCameraTarget(activeRow);

    }

    /// <summary>
    /// When the player moves, calls the move function and depermines what list it is moving through
    /// </summary>
    /// <param name="value"></param>
    public void OnMoveInput(InputValue value)
    {
        if (value.Get<Vector2>() == null) { return; }

        Vector2 temp_moveVector = value.Get<Vector2>();

        if (temp_moveVector != Vector2.zero)
        {
            foreach (AssigningControl control in m_assigningControls)
            {
                if (control.isPartList)
                {
                    MovePlayerPosition(temp_moveVector, ref temp_activeRow, ref temp_activeColomnSize, control.isPartList);
                    break;
                }
                else
                {
                    MovePlayerPosition(temp_moveVector, ref temp_activeButtonRow, ref temp_activeButtonSize, control.isPartList);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Resets the active button row to 0 so it starts at the same spot each time
    /// </summary>
    public void ResetActiveRows()
    {
        foreach (AssigningControl control in m_assigningControls)
        {
            if (control.isPartList)
            {
                temp_activeButtonRow = 0;
                break;
            }
        }
    }

    /// <summary>
    /// moves the player through the vertical list either up or down
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="activeRow"></param>
    /// <param name="activeSize"></param>
    private void MovePlayerPosition(Vector2 direction, ref int activeRow, ref int activeSize, bool downIsCorrect)
    {
        float temp_verticalInput = direction.y;

        if (downIsCorrect)
        {
            // Input Down
            if (temp_verticalInput < 0)
            {
                activeRow = ++activeRow % activeSize;
            }
            // Input Up
            if (temp_verticalInput > 0)
            {
                activeRow--;
                if (activeRow < 0 && activeSize > 0)
                {
                    activeRow = activeSize - 1;
                }
            }
        }
        else
        {
            // Input Down
            if (temp_verticalInput > 0)
            {
                activeRow = ++activeRow % activeSize;
            }
            // Input Up
            if (temp_verticalInput < 0)
            {
                activeRow--;
                if (activeRow < 0 && activeSize > 0)
                {
                    activeRow = activeSize - 1;
                }
            }
            
        }
    }

    public void SetColumnSize(int size)
    {
        temp_activeColomnSize = size;
    }

    public void SetRowSize(int size)
    {
        temp_activeButtonSize = size;
    }

    public void ToggleList()
    {
        m_assigningControls[0].SetIsPartList(!m_assigningControls[0].isPartList);
        m_assigningControls[1].SetIsPartList(!m_assigningControls[1].isPartList);
    }

    public void ToggleList(bool newState)
    {
        m_assigningControls[0].SetIsPartList(newState);
        m_assigningControls[1].SetIsPartList(newState);
    }

    public void FlipInput(int index)
    {
        m_assigningControls[index].gameObject.GetComponent<PlayerInput>().DeactivateInput();
        int temp_index = ++index % 2;
        m_assigningControls[temp_index].gameObject.GetComponent<PlayerInput>().ActivateInput();
    }
}
