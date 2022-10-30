using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSettings : MonoBehaviour
{
    public Toggle partImagesEffect_HealthScale = null;
  //  public Toggle partImagesEffect_Cooldown;

    public Toggle cameraEffect_HealthScale = null;
  //  public Toggle cameraEffect_Cooldown;

    public Toggle duplicateControls = null;

    public List<ListWrapper> settingsToggles = new List<ListWrapper>();

    public List<GameObject> battleUIDisplays = new List<GameObject>();


    private void Awake()
    {
        //set to default on awake, then change to saved settings
        
        partImagesEffect_HealthScale.isOn = (PlayerPrefs.GetInt("partHealthScale") == 1) ? true : false;
     //   partImagesEffect_Cooldown.isOn = (PlayerPrefs.GetInt("partCooldown") == 1) ? true : false;

        cameraEffect_HealthScale.isOn = (PlayerPrefs.GetInt("cameraHealthScale") == 1) ? true : false;
     //   cameraEffect_Cooldown.isOn = (PlayerPrefs.GetInt("cameraCooldown") == 1) ? true : false;

        duplicateControls.isOn = (PlayerPrefs.GetInt("duplicateControls") == 1) ? true : false;
        
    }

    public void SaveCurrentSettings()
    {
        foreach (ListWrapper list in settingsToggles)
        {
            foreach(Toggle toggle in list.toggles)
            {
                if (toggle.isOn)
                {
                    int toggleIndex = list.toggles.IndexOf(toggle);

                    switch (settingsToggles.IndexOf(list))
                    {
                        case 0: //fixed camera location toggle group
                            PlayerPrefs.SetInt("fixedCameraLayout", toggleIndex);
                            break;
                        case 1://part images location toggle group
                            PlayerPrefs.SetInt("partImagesLayout", toggleIndex);
                            break;
                        case 2://controls icon toggle group
                            PlayerPrefs.SetInt("controlIconsLayout", toggleIndex);
                            break;
                        case 3://on bot controls toggle group
                            PlayerPrefs.SetInt("onBotControls", toggleIndex);
                            break;
                    }
                }
               // break;
            }
        }
        
        int currentCameraHealthScaleState = cameraEffect_HealthScale.isOn == true ? 1 : 0;
        PlayerPrefs.SetInt("cameraHealthScale", currentCameraHealthScaleState);

      //  int currentCameraCooldownState = cameraEffect_Cooldown.isOn == true ? 1 : 0;
      //  PlayerPrefs.SetInt("cameraCooldown", currentCameraCooldownState);

        int currentPartHealthScaleState = partImagesEffect_HealthScale.isOn == true ? 1 : 0;
        PlayerPrefs.SetInt("partHealthScale", currentPartHealthScaleState);

       // int currentPartCooldownState = partImagesEffect_Cooldown.isOn == true ? 1 : 0;
      //  PlayerPrefs.SetInt("partCooldown", currentPartCooldownState);

        int currentDuplicateControlsState = duplicateControls.isOn == true ? 1 : 0;
        PlayerPrefs.SetInt("duplicateControls", currentDuplicateControlsState);
        
        PlayerPrefs.Save();

    }

    public void CheckIfChanged()
    {
        // if changed and not saved, have pop up asking to confirm settings
    }

    void Update()
    {
        if (settingsToggles[0].toggles[0].isOn)
        {
            battleUIDisplays[3].SetActive(true);
            battleUIDisplays[4].SetActive(false);
        } else if (settingsToggles[0].toggles[1].isOn)
        {
            battleUIDisplays[3].SetActive(false);
            battleUIDisplays[4].SetActive(true);
        } else if (settingsToggles[0].toggles[2].isOn)
        {
            battleUIDisplays[3].SetActive(false);
            battleUIDisplays[4].SetActive(false);
        }

        if (settingsToggles[1].toggles[0].isOn)
        {
            battleUIDisplays[0].SetActive(true);
            battleUIDisplays[1].SetActive(false);
            battleUIDisplays[2].SetActive(false);
        }
        else if (settingsToggles[1].toggles[1].isOn)
        {
            battleUIDisplays[0].SetActive(false);
            battleUIDisplays[1].SetActive(true);
            battleUIDisplays[2].SetActive(false);
        }
        else if (settingsToggles[1].toggles[2].isOn)
        {
            battleUIDisplays[0].SetActive(false);
            battleUIDisplays[1].SetActive(false);
            battleUIDisplays[2].SetActive(true);
        }
        else if (settingsToggles[1].toggles[3].isOn)
        {
            battleUIDisplays[0].SetActive(false);
            battleUIDisplays[1].SetActive(false);
            battleUIDisplays[2].SetActive(false);

        }

        if (settingsToggles[2].toggles[0].isOn)
        {
            battleUIDisplays[5].SetActive(true);
            battleUIDisplays[6].SetActive(false);
        }
        else if (settingsToggles[2].toggles[1].isOn)
        {
            battleUIDisplays[5].SetActive(false);
            battleUIDisplays[6].SetActive(true);
        }
        else if (settingsToggles[2].toggles[2].isOn)
        {
            battleUIDisplays[5].SetActive(false);
            battleUIDisplays[6].SetActive(false);
        }
        else if (settingsToggles[2].toggles[3].isOn)
        {
            battleUIDisplays[5].SetActive(false);
            battleUIDisplays[6].SetActive(false);
        }

    }

    public void CheckIfOverlaped(int temp)
    {
        if(temp == 1)
        {
            if (settingsToggles[0].toggles[0].isOn && settingsToggles[1].toggles[0].isOn)
            {
                settingsToggles[1].toggles[1].isOn = true;

            }
            else if (settingsToggles[0].toggles[1].isOn && settingsToggles[1].toggles[1].isOn)
            {
                settingsToggles[1].toggles[0].isOn = true;
            }
        } else
        {
            if (settingsToggles[0].toggles[0].isOn && settingsToggles[1].toggles[0].isOn)
            {
                settingsToggles[0].toggles[1].isOn = true;

            }
            else if (settingsToggles[0].toggles[1].isOn && settingsToggles[1].toggles[1].isOn)
            {
                settingsToggles[0].toggles[0].isOn = true;
            }
        }
       
    }
}

[System.Serializable]
public class ListWrapper
{
    public List<Toggle> toggles;
}
