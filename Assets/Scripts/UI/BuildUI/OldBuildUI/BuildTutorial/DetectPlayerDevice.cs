using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DetectPlayerDevice : MonoBehaviour
{
    [SerializeField] [BoxGroup("Input")] private PlayerInput m_playerInput = null;
    [HideInInspector] public string controlScheme => m_controlScheme;
    [SerializeField] [BoxGroup("Input")] [ReadOnly] private string m_controlScheme = "";

    private void Awake()
    {
        m_playerInput = PlayerInput.GetPlayerByIndex(0);
        m_controlScheme = m_playerInput.currentControlScheme;
    }
}
