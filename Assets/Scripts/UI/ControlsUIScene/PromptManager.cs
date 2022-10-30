using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PromptManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_promptSpace;
    public string[] m_prompts;
    

    public void Start()
    {
        m_promptSpace.text = "Select a bot part to change its controls.";
    }

    public void NewPrompt(int index = 0)
    {
        m_promptSpace.text = m_prompts[index];
    }

}
