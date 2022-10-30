using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnHighlightParts : MonoBehaviour
{
    [SerializeField] private GameObject[] partsList;

    private void Awake()
    {
        partsList = GameObject.FindGameObjectsWithTag("Highlight");
    }

    public void HighlightOff()
    {
        foreach (GameObject part in partsList)
        {
            part.layer = LayerMask.NameToLayer("Default");
        }
        //Debug.Log("Turn Highlight off!");

    }
}
