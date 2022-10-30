using DuolBots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using NaughtyAttributes;

// Original Authors - Cole Woulf and Eslis Vang
public class AssigningControl : MonoBehaviour
{
    [SerializeField] private PlayerInput m_playerInput = null;
    [SerializeField] private Color m_highlightColor = Color.blue;
    [SerializeField] private Color m_defaultColor = Color.cyan;
    // If the player has confirmed their selection.
    private ControllerMovement m_controllerMovement = null;
    private ControlUIScriptableObjectImplement m_control;
    private PartAnimManager m_manager;
    [SerializeField] private GameObject m_scrollView = null;

    // Present from Wyatt :+1:
    [SerializeField] [Scene] private string m_sceneToLoad = "Battle_TwoPCs";
    public string tutorialScene => m_tutorialScene;
    [SerializeField] [Scene] private string m_tutorialScene = "BattleTutorial";
    public string battleScene => m_battleScene;
    [SerializeField] [Scene] private string m_battleScene = "Battle_TwoPCs";
    [SerializeField] private TutorialManager m_tutorialManager = null;

    public byte m_playerIndex = 2;

    public bool isMouseVisible = true;
    public bool isPartList => m_isPartList;
    private bool m_isPartList = true;

    private AssignCamera m_assignCamera;

    private void Awake()
    {
        m_tutorialManager = FindObjectOfType<TutorialManager>();
        m_playerInput = this.GetComponent<PlayerInput>();
        m_controllerMovement = GameObject.Find("SceneManager").GetComponent<ControllerMovement>();
        m_control = GameObject.Find("SceneManager").GetComponent<ControlUIScriptableObjectImplement>();
        m_assignCamera = FindObjectOfType<AssignCamera>();

        if (GameObject.Find("PlayerObj1"))
        {
            this.name = "PlayerObj2";
            m_playerIndex = 1;
        }
        else
        {
            this.name = "PlayerObj1";
            m_playerIndex = 0;
        }
        m_scrollView = GameObject.Find("Content");
    }

    private void Start()
    {
        if (this.name == "PlayerObj1" && m_control.isPlayer1 != 0)
        {
            transform.gameObject.GetComponent<PlayerInput>().DeactivateInput();
        }
        if (this.name == "PlayerObj2" && m_control.isPlayer1 != 1)
        {
            transform.gameObject.GetComponent<PlayerInput>().DeactivateInput();
        }
        m_controllerMovement.SetColumnSize(m_scrollView.transform.childCount);
        m_manager = GameObject.Find("SceneManager").GetComponent<PartAnimManager>();
    }

    public void OnMove(InputValue value)
    {
        Debug.Log($"P{m_playerInput.playerIndex + 1} Moved");
        if (m_isPartList)
        {
            m_control.m_botPartsList[m_controllerMovement.activeRow].scrollListSpace.transform.GetChild(0).GetComponent<Image>().color = m_defaultColor;
        }
        else
        {
            GameObject temp = m_control.m_botPartsList[m_controllerMovement.activeRow].bindings[m_controllerMovement.activeButtonRow].buttonPrefab;
            temp.GetComponent<Image>().color = temp.GetComponent<Button>().colors.disabledColor;
        }
        m_controllerMovement.OnMoveInput(value);
        if (m_isPartList)
        {
            m_control.m_botPartsList[m_controllerMovement.activeRow].scrollListSpace.transform.GetChild(0).GetComponent<Image>().color = m_highlightColor;
        }
        else
        {
            GameObject temp = m_control.m_botPartsList[m_controllerMovement.activeRow].bindings[m_controllerMovement.activeButtonRow].buttonPrefab;
            temp.GetComponent<Image>().color = temp.GetComponent<Button>().colors.highlightedColor;
            m_manager.UpdateAnimationManager(m_controllerMovement.activeRow,m_controllerMovement.activeButtonRow);
        }
    }

    public void OnSelect()
    {
        // once selecting the part list, go into button list
        Debug.Log($"P{m_playerInput.playerIndex + 1} Selected");
        if (!m_isPartList)
        {
            if(m_tutorialManager.stage1)
            {
                if (m_controllerMovement.activeButtonRow == 2 && m_control.isPlayer1 == 1)
                    m_control.OnSelection(m_controllerMovement.activeRow, m_controllerMovement.activeButtonRow);
                else if(m_controllerMovement.activeButtonRow == 1 && m_control.isPlayer1 == 0)
                {
                    m_control.OnSelection(m_controllerMovement.activeRow, m_controllerMovement.activeButtonRow);
                }
            }
            else
            {
                m_control.OnSelection(m_controllerMovement.activeRow, m_controllerMovement.activeButtonRow);
            }   
        }
        else
        {
            if (m_tutorialManager.stage2)
            {
                if (m_controllerMovement.activeRow == 1 && (m_control.isPlayer1 == 1 || m_control.isPlayer1 == 0))
                {
                    m_control.ActiveBindings(m_control.m_botPartsList[m_controllerMovement.activeRow]);
                    m_controllerMovement.SetRowSize(m_control.m_botPartsList[m_controllerMovement.activeRow].bindings.Count);
                }
            }
            else
            {
                m_control.ActiveBindings(m_control.m_botPartsList[m_controllerMovement.activeRow]);
                m_controllerMovement.SetRowSize(m_control.m_botPartsList[m_controllerMovement.activeRow].bindings.Count);
            }
        }
        m_controllerMovement.ToggleList();
        m_controllerMovement.ResetActiveRows();

    }
        
    public void OnConfirm()
    {
        Debug.Log($"P{m_playerInput.playerIndex + 1} Confirm");
        if (m_playerInput.playerIndex == 0)
        {
            if (!m_control.isPlayerOneConfirmed)
            {
                m_control.ChangeActivePlayer();
            }
            m_control.SetPlayerConfirm(m_playerInput.playerIndex);
            m_control.SetPlayer1TextColor();
        }

        if (m_playerInput.playerIndex == 1)
        {
            if (!m_control.isPlayerTwoConfirmed)
            {
                m_control.ChangeActivePlayer();
            }
            m_control.SetPlayerConfirm(m_playerInput.playerIndex);
            m_control.SetPlayer2TextColor();
        }

        if (m_control.isPlayerOneConfirmed && m_control.isPlayerTwoConfirmed)
        {
            GameObject.Find("SceneManager").GetComponent<PassingInput>().SubmitBinding();
            SceneLoader.instance.LoadScene(m_sceneToLoad);
            //GameObject.Find("SceneManager").GetComponent<LoadNextScene>().LoadScene(m_sceneToLoad);
        }
    }

    public void OnLook(InputValue value)
    {
        m_assignCamera.UpdateAxisValue(value);
    }

    public void SetIsPartList(bool value)
    {
        m_isPartList = value;
    }

    public void OnCancel()
    {
        Debug.Log($"P{m_playerInput.playerIndex + 1} Cancel");
        m_controllerMovement.ToggleList(true);
        GameObject temp = m_control.m_botPartsList[m_controllerMovement.activeRow].bindings[m_controllerMovement.activeButtonRow].buttonPrefab;
        temp.GetComponent<Image>().color = temp.GetComponent<Button>().colors.disabledColor;
    }

    public void OnConfirmTutorial()
    {
        if (m_playerInput.playerIndex == 0)
        {
            if (!m_control.isPlayerOneConfirmed)
            {
                m_control.ChangeActivePlayer();
            }
            m_control.SetPlayerConfirm(m_playerInput.playerIndex);
            m_control.SetPlayer1TextColor();
        }

        if (m_playerInput.playerIndex == 1)
        {
            if (!m_control.isPlayerTwoConfirmed)
            {
                m_control.ChangeActivePlayer();
            }
            m_control.SetPlayerConfirm(m_playerInput.playerIndex);
            m_control.SetPlayer2TextColor();
        }

        if (m_control.isPlayerOneConfirmed && m_control.isPlayerTwoConfirmed)
        {
            Debug.Log("Loading Tutorial Scene");
            GameObject.Find("SceneManager").GetComponent<PassingInput>().SubmitBinding();
            SceneLoader.instance.LoadScene("BattleTutorial");
        }
    }

    public void TutorialPhase1Assign()
    {
        Debug.Log("Phase 1");
        m_playerInput.SwitchCurrentActionMap("TutorialConfirm");
        Debug.Log(m_playerInput.currentActionMap);
    }

    public void TutorialPhase2Assign()
    {
        Debug.Log("Phase 2");
        // might need this but not sure
        //m_playerInput.SwitchCurrentActionMap("TutorialConfirm1");
        m_playerInput.SwitchCurrentActionMap("PartSelection");
    }

    public void DeactivatePlayerInput()
    {
        m_playerInput.DeactivateInput();
    }

    public void ActivatePlayerInput()
    {
        m_playerInput.ActivateInput();
        m_playerInput.SwitchCurrentActionMap("TutorialConfirm");
        //m_playerInput.SwitchCurrentActionMap("PartSelection");
    }

    public void SetSceneToLoad(string sceneName)
    {
        m_sceneToLoad = sceneName;
    }
}

