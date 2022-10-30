using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Original Author - Cole Woulf

/// <summary>
/// This class is for testing the new input system to get familiar with it and use it to go on
/// </summary>
public class TestingInputSystem : MonoBehaviour
{
    private Rigidbody m_sphereRigidbody;
    private PlayerInput m_playerInput;
    private AssigningControls playerAssigningControls;


    #region UnityMessages
    private void Awake()
    {
        m_sphereRigidbody = GetComponent<Rigidbody>();
        m_playerInput = GetComponent<PlayerInput>();

        playerAssigningControls = new AssigningControls();
        playerAssigningControls.PlayerBot.Enable();
        playerAssigningControls.PlayerBot.Jump.performed += Jump;
    }

    /// <summary>
    /// Use this method for movment rather than a Movement method
    /// </summary>
    private void FixedUpdate()
    {
        Vector2 inputVector = playerAssigningControls.PlayerBot.Movement.ReadValue<Vector2>();
        float speed = 1f;
        m_sphereRigidbody.AddForce(new Vector3(inputVector.x, 0, inputVector.y) * speed, ForceMode.Impulse);
    }

    #endregion UnityMessages
    /// <summary>
    /// Testing the New Input System with simple calls and commands
    /// </summary>
    /// <param name="context"></param>
    public void Jump(InputAction.CallbackContext context)
    {
        Debug.Log(context);
        if(context.performed)
        {
            Debug.Log("Jumping" + context.phase);
            m_sphereRigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }
}
