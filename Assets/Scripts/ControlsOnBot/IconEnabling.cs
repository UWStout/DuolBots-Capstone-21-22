using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using DuolBots;
public class IconEnabling : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> IconHolders;
    private GameObject NextOn;
    private GameObject CurrentlyOn;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IconHolders != null && IconHolders.Count > 0 && CurrentlyOn.GetComponent<ControlDisplayObject>().CameraObj != null)
        {
            float distance = Mathf.Infinity;
            foreach (GameObject GO in IconHolders)
            {
                float newDistance = (GO.transform.position - CurrentlyOn.GetComponent<ControlDisplayObject>().CameraObj.transform.position).sqrMagnitude;
                if (newDistance < distance)
                {
                    NextOn = GO;
                    distance = newDistance;
                }
            }
            if (CurrentlyOn != NextOn)
            {
                /*
                CurrentlyOn.SetActive(false);
                CurrentlyOn = NextOn;
                CurrentlyOn.SetActive(true);
                */

                foreach (Image i in CurrentlyOn.GetComponentsInChildren<Image>())
                {
                    i.enabled = false;
                }
                CurrentlyOn = NextOn;
                foreach (Image i in CurrentlyOn.GetComponentsInChildren<Image>())
                {
                    i.enabled = true;
                }
            }
        }
    }


    public void UpdateChildrenList(int childIndex)
    {
        IconHolders = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if (child.GetChild(childIndex).childCount > 0)
            {
                IconHolders.Add(child.GetChild(childIndex).gameObject);
            }
            child.GetChild(childIndex).gameObject.GetComponent<ControlDisplayObject>().UpdateChildren();
            //child.GetChild(childIndex).gameObject.SetActive(false);
            foreach (Image i in child.GetChild(childIndex).gameObject.GetComponentsInChildren<Image>())
            {
                i.enabled = false;
            }
        }
        if (IconHolders.Count > 0)
        {
            CurrentlyOn = IconHolders[0];
            //CurrentlyOn.SetActive(true);
            foreach (Image i in CurrentlyOn.GetComponentsInChildren<Image>())
            {
                i.enabled = true;
            }
            NextOn = IconHolders[0];
        }
    }

}
