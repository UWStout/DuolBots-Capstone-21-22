using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    public class RoomKeyInputField : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_inputBox = null;
        [SerializeField] private TextMeshProUGUI m_inputText = null;

        [SerializeField] private string m_defaultInputPhrase = "Input Text";


        public void SanitizeInputText(string updatedText)
        {
            if (updatedText != "")
            {
                m_inputBox.text = "";
            }
            else
            {
                m_inputBox.text = m_defaultInputPhrase;
            }
        }
    }
}
