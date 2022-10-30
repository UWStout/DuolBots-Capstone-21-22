using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    // 3 different stages of tutorial
    public bool stage1 = false;
    public bool stage2 = false;

    [SerializeField] public GameObject m_activePanel => m_popUps2[m_popUpIndex];
    [SerializeField] private GameObject m_firstPopUp;
    [SerializeField] private GameObject[] m_popUps2;
    private int m_popUpIndex = 0;

    private ControllerMovement m_control;
    private ControlUIScriptableObjectImplement m_implement;
    private AssigningControl m_assigningControl1;
    private AssigningControl m_assigningControl2;

    private void Start()
    {
        m_control = GameObject.Find("SceneManager").GetComponent<ControllerMovement>();
        m_implement = GameObject.Find("SceneManager").GetComponent<ControlUIScriptableObjectImplement>();
        m_assigningControl1 = GameObject.Find("PlayerObj1").GetComponent<AssigningControl>();
        m_assigningControl2 = GameObject.Find("PlayerObj2").GetComponent<AssigningControl>();
        //SceneManager.LoadScene();
        Debug.Log("Loading Tutorial Scene");
        AssignStage(SingletonTutorialStateManager.instance.tutorialIndex);
    }
    /// <summary>
    /// first stage assign scene:
    /// allow them to only ready up. So just show them the assign scene 
    /// </summary>
    private void Update()
    {
        if (stage1)
        {
            Stage1AssignScene();
        }
        else if(stage2)
        { 
            Stage2AssignScene();
        }
        /*
        else
        {
            m_firstPopUp.SetActive(false);
            for (int i = 0; i < m_popUps2.Length; i++)
            {
                m_popUps2[i].SetActive(false);
            }
        }*/
    }

    public void AssignStage(int newStage)
    {
        Debug.Log($"Trying to assign tutorial stage at index: {newStage}");
        if (newStage != 0 && newStage != 1)
        {
            stage1 = false;
            stage2 = false;
            m_assigningControl1.SetSceneToLoad(m_assigningControl1.battleScene);
            m_assigningControl2.SetSceneToLoad(m_assigningControl2.battleScene);
            return;
        }

        else if (newStage == 0) { stage1 = true; stage2 = false; }
        else if (newStage == 1) { stage1 = false; stage2 = true; }
        if (newStage == 0 || newStage == 1)
        {
            m_assigningControl1.SetSceneToLoad(m_assigningControl1.tutorialScene);
            m_assigningControl2.SetSceneToLoad(m_assigningControl2.tutorialScene);
        }
        Debug.Log($"Stage 1: {stage1} | Stage 2: {stage2}");
    }

    /// <summary>
    /// first stage assign scene:
    /// allow them to only ready up. So just show them the assign scene 
    /// </summary>

    private void Stage1AssignScene()
    {
        m_firstPopUp.SetActive(true);
        // if player presses space or start, ready that player up but disable all other input
        m_assigningControl1.TutorialPhase1Assign();
        m_assigningControl2.TutorialPhase1Assign();
      
    }

    private void Stage2AssignScene()
    {
        //m_assigningControl1.TutorialPhase2Assign();
        //m_assigningControl2.TutorialPhase2Assign();
        m_firstPopUp.SetActive(false);
        if (m_popUpIndex == 0) // intro
        {
            m_popUps2[m_popUpIndex].SetActive(true);
            if (m_control.activeRow == 1)
            {
                m_popUpIndex++;
            }
        }
        else if (m_popUpIndex == 1)  // first player change control 1
        {
            m_popUps2[m_popUpIndex - 1].SetActive(false);
            m_popUps2[m_popUpIndex].SetActive(true);
            if (m_control.activeButtonRow == 2 && m_implement.m_botPartsList[m_control.activeRow].partId == "Turret 2 Axis")
            {
                m_popUpIndex++;
            }
        }
        else if (m_popUpIndex == 2)  // first player change control 2
        {
            m_popUps2[m_popUpIndex - 1].SetActive(false);
            m_popUps2[m_popUpIndex].SetActive(true);
            // not sure if this will work
            if (m_implement.isPlayer1 == 0)
            {
                m_popUpIndex++;
            }
        }
        else if (m_popUpIndex == 3)  // second player change control 
        {
            m_popUps2[m_popUpIndex - 1].SetActive(false);
            m_popUps2[m_popUpIndex].SetActive(true);
            if (m_control.activeRow == 1)
            {
                m_popUpIndex++;
            }
        }
        else if (m_popUpIndex == 4)  // second player change control 1
        {
            m_popUps2[m_popUpIndex - 1].SetActive(false);
            m_popUps2[m_popUpIndex].SetActive(true);
            if (m_control.activeButtonRow == 1 && m_implement.m_botPartsList[m_control.activeRow].partId == "Turret 2 Axis")
            {
                m_popUpIndex++;
            }
        }
        else if (m_popUpIndex == 5)  // second player change control 2
        {
            m_popUps2[m_popUpIndex - 1].SetActive(false);
            m_popUps2[m_popUpIndex].SetActive(true);
            if (m_implement.isPlayer1 == 1)
            {
                m_popUpIndex++;
            }
        }
        else if (m_popUpIndex == 6)  // second player change control 3
        {
            m_popUps2[m_popUpIndex - 1].SetActive(false);
            m_popUps2[m_popUpIndex].SetActive(true);
            // timer or click through
            StartCoroutine("increase");
        }
        else if(m_popUpIndex == 7)
        {
            m_popUps2[m_popUpIndex - 1].SetActive(false);
            m_popUps2[m_popUpIndex].SetActive(true);
            // either both press start or get ready up button
            // probably switch to tutorial phase 1 action map so they cant do anything else 
        }
    }

    IEnumerator increase()
    {
        //Disable input until time is up
        m_assigningControl1.DeactivatePlayerInput();
        m_assigningControl2.DeactivatePlayerInput(); 
        yield return new WaitForSeconds(10f);
        m_popUpIndex++;
        m_assigningControl1.ActivatePlayerInput();
        m_assigningControl2.ActivatePlayerInput();
    }
}
