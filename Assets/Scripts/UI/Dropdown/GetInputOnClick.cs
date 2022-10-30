using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Original Authors - Cole Woulf
namespace DuolBots
{
    public class GetInputOnClick : MonoBehaviour
    {
        [SerializeField] private Button m_inputButton;
        // have to get once scriptable objects are implemented
        private Text m_partName;
        // text of the input chosen
        [SerializeField] private Text m_inputText;
        private List<string> names;

        /// <summary>
        /// intitually assigns the button to first enum
        /// </summary>
        void Start()
        {
            m_inputText.text = eInputType.buttonEast.ToString();
            string[] enumNames = Enum.GetNames(typeof(eInputType));
            names = new List<string>(enumNames);
        }

        void Update()
        {
            GetButtonPressed();
        }

        public void GetButtonPressed()
        {
            foreach(string temp in names)
            {
                if(Input.GetKeyDown(temp))
                {
                    m_inputText.text = temp;
                }
            }  
        }
    }
}
