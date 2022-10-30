using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

// Original Authors - Cole Woulf
namespace DuolBots
{
    public class InputEnumDropdown : MonoBehaviour
    {

        public Dropdown dropdpwn;
        public Text selectedName;

        public void Dropdown_IndexChanged(int index)
        {
            eInputType name = (eInputType)index;
            selectedName.text = name.ToString();

            if (index == 0)
            {
                selectedName.color = Color.red;
            }
            else
            {
                selectedName.color = Color.white;
            }
        }

        void Start()
        {
            PopulateList();
        }

        private void PopulateList()
        {
            string[] enumNames = Enum.GetNames(typeof(eInputType));
            List<string> names = new List<string>(enumNames);

            dropdpwn.AddOptions(names);
        }
    }
}
