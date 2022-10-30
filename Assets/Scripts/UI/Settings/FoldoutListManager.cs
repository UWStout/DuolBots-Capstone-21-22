using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class FoldoutListManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> groups = new List<GameObject>();
    [SerializeField] private List<GameObject> foldouts = new List<GameObject>();

    void Update()
    {
        if (foldouts[0].activeSelf)
        {
            groups[1].transform.localPosition = new Vector3(-461f, 41f, 0f);
            groups[2].transform.localPosition = new Vector3(-461f, -53f, 0f);
            if (foldouts[1].activeSelf)
            {
                groups[2].transform.localPosition = new Vector3(-461f, -270f, 0f);
            }
        }
        else
        {
            groups[1].transform.localPosition = new Vector3(-461f, 259f, 0f);

            if (foldouts[1].activeSelf)
                groups[2].transform.localPosition = new Vector3(-461f, -53f, 0f);
            else
                groups[2].transform.localPosition = new Vector3(-461f, 165f, 0f);
        }
    }
    
}
