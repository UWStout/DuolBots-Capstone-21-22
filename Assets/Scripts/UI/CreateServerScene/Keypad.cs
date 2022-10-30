using UnityEngine;

using NaughtyAttributes;
using TMPro;
// Original Authors - Wyatt Senalik

namespace DuolBots
{

    public class Keypad : MonoBehaviour
    {
        [SerializeField] [Required] private TMP_InputField m_inputField = null;
        [SerializeField] [Min(0)] private int m_maxCodeLength = 11;


        public void AppendString(string strToAppend)
        {
            m_inputField.text = m_inputField.text + strToAppend;
            m_inputField.text = m_inputField.text.Substring(0,
                Mathf.Min(m_inputField.text.Length, m_maxCodeLength));
        }
        public void Backspace()
        {
            if (m_inputField.text.Length <= 0) { return; }
            m_inputField.text = m_inputField.text.Substring(0, m_inputField.text.Length - 1);
        }
        public void Clear()
        {
            m_inputField.text = string.Empty;
        }
    }
}
