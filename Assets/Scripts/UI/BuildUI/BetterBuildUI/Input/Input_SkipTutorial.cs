using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input_SkipTutorial : MonoBehaviour
{
    [SerializeField] private GameObject[] tutorialPopups = null;

    private void OnSkip(InputValue value)
    {
        if (!value.isPressed) return;

        foreach (GameObject popupSet in tutorialPopups)
        {
            popupSet.SetActive(false);
        }
    }
}
