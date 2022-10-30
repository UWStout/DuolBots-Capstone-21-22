using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;
using DuolBots;
using UnityEngine.Events;
using UnityEngine.Assertions;

//Original Author - Shelby Vian
// People who "broke it" - Cole and Ben

/// <summary>
/// Legacy code
/// </summary>
[Obsolete("Somebody wrote this.",false)]
public class ControlUIScriptableObjectImplement : MonoBehaviour
{
    // Constants
    private bool IS_DEBUGGING = false;

    [SerializeField]
    public List<botInfo> m_botPartsList = new List<botInfo>();
    [SerializeField] private GameObject m_ListSpacePrefab, m_InputButtonPrefab, m_DisplayCurrentControlPrefab, m_viewport;//, m_assignPanel;
    [SerializeField] private List<string> m_partsInScene = new List<string>();
    private BuiltBotData m_holdData;
    private int m_partIndex;
    private Color m_colorPlay2 = new Color(0.76f, 0.32f, 0f, 1f), m_colorPlay1 = new Color(0, 0.42f, 0.65f, 1f);
    [SerializeField] private TextMeshProUGUI textPlayer1, textPlayer2;
    public TextMeshProUGUI player1Text => textPlayer1;
    public TextMeshProUGUI player2Text => textPlayer2;

    public byte isPlayer1 => m_isPlayer1;
    private byte m_isPlayer1 = 0;
    [SerializeField] private List<PartScriptableObject> m_scriptableObjectList = new List<PartScriptableObject>();
    int y = 0, index;


    botInfo hold_PartClicked;

    private GameObject inputManager1;
    private GameObject inputManager2;
    private partBindings hold_selectedButtonPrefab;

    private ControllerMovement m_controllerMovement = null;

    private bool m_playerOneConfirmed = false;
    public bool isPlayerOneConfirmed => m_playerOneConfirmed;

    private bool m_playerTwoConfirmed = false;
    public bool isPlayerTwoConfirmed => m_playerTwoConfirmed;

    private List<DefaultControls> playerControls = new List<DefaultControls>() { new DefaultControls(), new DefaultControls() };

    void Start()
    {
        m_controllerMovement = GetComponent<ControllerMovement>();

        // Get built bot data in scene -currently only for team index 0
        m_holdData = BuildSceneBotData.GetBotData(0);

        m_partsInScene.Add(m_holdData.chassisID);
        m_partsInScene.Add(m_holdData.movementPartID);

        foreach (PartInSlot temp_stringID in m_holdData.slottedPartIDList)
        {
            m_partsInScene.Add(temp_stringID.partID);
        }

        // Search dictionary for PartScriptableObject
        PartDatabase temp_partDatabase = PartDatabase.instance;

        foreach (string partID in m_partsInScene)
        {
            m_scriptableObjectList.Add(temp_partDatabase.GetPartScriptableObject(partID));
        }

        
        foreach (PartScriptableObject test in m_scriptableObjectList)
        {
            // Part data
            botInfo newPart = new botInfo();
            newPart.botPart = test;
            newPart.partId = test.partID;
            newPart.partModel = test.modelPrefab;
            newPart.numOfBindings = test.actionList.Count;
            newPart.partSprite = test.partUIData.unlockedSprite;
            newPart.currentControlPrefabImages = new List<Image>();

            if (newPart.numOfBindings > 0)
            {
                // Instantiate list item prefab
                m_ListSpacePrefab.transform.position = new Vector3(-1, (-100) - (190 * y), 0);
                newPart.scrollListSpace = Instantiate(m_ListSpacePrefab, m_ListSpacePrefab.transform.position, m_ListSpacePrefab.transform.rotation);
                newPart.scrollListSpace.transform.SetParent(m_viewport.transform.Find("Content"), false);
                y++;

                newPart.button = newPart.scrollListSpace.GetComponentInChildren<Button>();

                newPart.button.onClick.AddListener(delegate
                {
                    ActiveBindings(newPart);
                });

                // Set part images
                RawImage imageHold = newPart.scrollListSpace.GetComponentInChildren<RawImage>();
                imageHold.GetComponent<RawImage>().texture = newPart.partSprite;

                // Instantiate new bindings list for the bot part
                newPart.bindings = new List<partBindings>();

                // Instantiate bindings struct list
                for (int i = 0; i < newPart.numOfBindings; i++)
                {
                    partBindings newBind = new partBindings();

                    // Set part id and action name for this binding
                    newBind.partId = newPart.partId;
                    newBind.actionName = newPart.botPart.actionList[i].getname();

                    #region prefab position math
                    RectTransform temp_rt = (RectTransform)newPart.button.transform.Find("ContentHolder");
                    float temp_displayHeight = temp_rt.rect.height / newPart.numOfBindings;
                    // Debug.Log("display height " + newPart.numOfBindings);

                    //TO DO: make better

                    switch (newPart.numOfBindings)
                    {
                        case 1:
                            m_DisplayCurrentControlPrefab.transform.position = new Vector3(82.5f, 0, 0);
                            m_InputButtonPrefab.transform.position = new Vector3(100f, 0, 0);
                            break;
                        case 2:
                            if (i == 0)
                            {
                                m_DisplayCurrentControlPrefab.transform.position = new Vector3(82.5f, temp_displayHeight / 3, 0);
                                m_InputButtonPrefab.transform.position = new Vector3(100f, temp_displayHeight / 3, 0);
                            }
                            else
                            {
                                m_DisplayCurrentControlPrefab.transform.position = new Vector3(82.5f, -1 * temp_displayHeight / 3, 0);
                                m_InputButtonPrefab.transform.position = new Vector3(100f, -1 * temp_displayHeight / 3, 0);
                            }
                            break;
                        case 3:
                            if (i == 0)
                            {
                                m_DisplayCurrentControlPrefab.transform.position = new Vector3(82.5f, -1 * temp_displayHeight, 0);
                                m_InputButtonPrefab.transform.position = new Vector3(100f, -1 * temp_displayHeight, 0);
                            }
                            else if (i != 0 && i % 2 == 0)
                            {
                                //even number, not zero
                                m_DisplayCurrentControlPrefab.transform.position = new Vector3(82.5f, temp_displayHeight, 0);
                                m_InputButtonPrefab.transform.position = new Vector3(100f, temp_displayHeight, 0);
                            }
                            else
                            {
                                m_DisplayCurrentControlPrefab.transform.position = new Vector3(82.5f, 0, 0);
                                m_InputButtonPrefab.transform.position = new Vector3(100f, 0, 0);
                            }
                            break;
                    }

                    #endregion

                    // Instantiate current control text display prefabs
                    var temp = Instantiate(m_DisplayCurrentControlPrefab, m_DisplayCurrentControlPrefab.transform.position, m_DisplayCurrentControlPrefab.transform.rotation);
                    temp.transform.SetParent(newPart.button.transform.Find("ContentHolder"), false);
                    newPart.currentControlPrefabImages.Add(temp.GetComponentInChildren<Image>());

                    // Instantiates the button that can change
                    newBind.buttonPrefab = Instantiate(m_InputButtonPrefab, m_InputButtonPrefab.transform.position, m_InputButtonPrefab.transform.rotation);
                    newBind.buttonPrefab.GetComponent<InputButtonInfo>().Setaction(test.actionList[i].actionType);
                    newBind.buttonPrefab.transform.SetParent(newPart.button.transform.Find("ButtonPlace"), false);

                    // Sets part id and part index to specific button that was instatiated
                    newBind.buttonPrefab.GetComponent<InputButtonInfo>().SetPartID(newPart.partId);
                    newBind.buttonPrefab.GetComponent<InputButtonInfo>().SetPartIndex((byte)i);
                    
                    newPart.currentControlPrefabImages.Add(newBind.buttonPrefab.GetComponentInChildren<Image>());

                    newBind.assignedPlayer = m_isPlayer1;
                    int index4 = m_botPartsList.FindIndex(X => X.partId == newPart.partId);

                    if (index4>=0 && PlayerPrefs.GetInt("duplicateControls") == 1)
                    {
                        // If the string id is the same and the slot index is different, set same controls
                        newBind.buttonPrefab.GetComponent<InputButtonInfo>().SetisPlayerOne(m_botPartsList[index4].bindings[i].buttonPrefab.GetComponent<InputButtonInfo>().GetisPlayerOne());
                        newBind.buttonPrefab.GetComponent<InputButtonInfo>().Setinput(m_botPartsList[index4].bindings[i].buttonPrefab.GetComponent<InputButtonInfo>().GetInput());
                    }
                    else
                    {
                        newBind.buttonPrefab.GetComponent<InputButtonInfo>().SetisPlayerOne(m_isPlayer1);
                        newBind.buttonPrefab.GetComponent<InputButtonInfo>().Setinput(playerControls[(int)m_isPlayer1].UseInput(newBind.buttonPrefab.GetComponent<InputButtonInfo>().Getaction()));
                    }

                    ChangePlayerByte();

                    // Get text from scriptable objects for action list text
                    Text[] tempText = newPart.scrollListSpace.GetComponentsInChildren<Text>();
                    tempText[i * 2].text = newBind.actionName;

                    // Do last
                    newPart.bindings.Add(newBind);
                }

                m_botPartsList.Add(newPart);
            }
        }

        if (m_isPlayer1 == 0)
        {
            textPlayer2.GetComponent<TextMeshProUGUI>().color = Color.gray;
            textPlayer1.GetComponent<TextMeshProUGUI>().color = m_colorPlay1;
        }
        else if (m_isPlayer1 == 1)
        {
            textPlayer1.GetComponent<TextMeshProUGUI>().color = Color.gray;
            textPlayer2.GetComponent<TextMeshProUGUI>().color = m_colorPlay2;
        }
        int slotIndex = 0;
        for (int i = 0; i < m_botPartsList.Count; i++)
        {
            CustomDebug.Log(GameObject.Find($"Slot_{slotIndex}") + "------------------------", IS_DEBUGGING);
            slotIndex++;
        }
    }


    void Update()
    {
        inputManager1 = GameObject.Find("PlayerObj1");
        inputManager2 = GameObject.Find("PlayerObj2");
    }



    public void ActiveBindings(botInfo partClicked)
    {
        hold_PartClicked = partClicked;

        foreach (partBindings bind in partClicked.bindings)
        {
            CustomDebug.Log($"index of partClickedBind: partClicked.bindings.IndexOf(bind)", IS_DEBUGGING);
            int temp_bindIndex = partClicked.bindings.IndexOf(bind);

            bind.buttonPrefab.GetComponent<Button>().onClick.AddListener(delegate
            {
                ButtonValueChanged(bind, temp_bindIndex);
            });
        }
    }

    public void DeactiveBindings()
    {
        foreach (partBindings bind in hold_PartClicked.bindings)
        {
            int temp_bindIndex = hold_PartClicked.bindings.IndexOf(bind);

            bind.buttonPrefab.GetComponent<Button>().onClick.RemoveListener(delegate
            {
                ButtonValueChanged(bind, temp_bindIndex);
            });
        }
    }

    public bool CheckSplitControls(botInfo part, int index)
    {
        CustomDebug.Log($"Index of Bind: index", IS_DEBUGGING);
        var temp = part.bindings[index];
        byte temp_OtherPlayer = temp.assignedPlayer;
        temp.assignedPlayer = m_isPlayer1;
        part.bindings[index] = temp;

        switch (part.numOfBindings)
        {
            case 2:
                if (part.bindings[0].assignedPlayer == part.bindings[1].assignedPlayer)
                {

                    if (index == 0)
                    {
                        
                        var temp_swapBindPlayer = part.bindings[1];
                        temp_swapBindPlayer.assignedPlayer = temp_OtherPlayer;
                        part.bindings[1] = temp_swapBindPlayer;
                    }
                    else if (index == 1)
                    {
                        
                        var temp_swapBindPlayer = part.bindings[0];
                        temp_swapBindPlayer.assignedPlayer = temp_OtherPlayer;
                        part.bindings[0] = temp_swapBindPlayer;
                    }

                    return true;
                }
                else

                    return true;
            case 3:
                if (part.bindings[0].assignedPlayer == part.bindings[1].assignedPlayer && part.bindings[1].assignedPlayer == part.bindings[2].assignedPlayer)
                {
                    temp.assignedPlayer = temp_OtherPlayer;
                    part.bindings[index] = temp;
                    return false;
                }
                else
                    return true;
        }
        return true;
    }

    /// <summary>
    /// Stores the index of the dropdown changed, changes the dropdown color to the active player color
    /// </summary>
    /// <param name="change"> partBindings struct item to change color </param>
    /// <param name="bot"> the corresponding bot part for the button</param>
    public void ButtonValueChanged(partBindings change, int index)
    {
        hold_selectedButtonPrefab = change;
        // hold_selectedButton = bot;
        CustomDebug.Log($"index of change: {index}", IS_DEBUGGING);
        if (CheckSplitControls(hold_PartClicked, index))
        {
            // checks which player is active and gets their input
            if (m_isPlayer1 == 0)
                inputManager1.GetComponent<InputFromController>().changeActionMap(change.buttonPrefab.GetComponent<InputButtonInfo>().Getaction());
            if (m_isPlayer1 == 1)
                inputManager2.GetComponent<InputFromController>().changeActionMap(change.buttonPrefab.GetComponent<InputButtonInfo>().Getaction());
        }
        else
        {
            CustomDebug.Log($"Player {m_isPlayer1} has too many assigned controls", IS_DEBUGGING);
            DeactiveBindings();
        }
    }

    public void OnSelection(int activeRow, int activeButtonRow)
    {
        ButtonValueChanged(m_botPartsList[activeRow].bindings[activeButtonRow], activeButtonRow);
    }

    /// <summary>
    /// Changes the m_isPlayer1 byte variable between 0 and 1
    /// </summary>
    private void ChangePlayerByte()
    {
        if (m_isPlayer1 == 0)
        {
            m_isPlayer1 = 1;
        }
        else
        {
            m_isPlayer1 = 0;
        }
    }

    /// <summary>
    /// Called once player clickes 'confirm' on assign panel. Changes the button highlight to the current active player. Calls DropdownValueChanged
    /// </summary>
    public void ChangeActivePlayer()
    {
        if ((m_playerOneConfirmed && m_isPlayer1 == 1) || (m_playerTwoConfirmed && m_isPlayer1 == 0))
        {
            CustomDebug.Log("Is Conformed P1" + m_playerOneConfirmed, IS_DEBUGGING);
            CustomDebug.Log("Is Conformed P2" + m_playerTwoConfirmed, IS_DEBUGGING);
            return;
        }
        else
        {
            CustomDebug.Log("Is NOT Conformed P1" + m_playerOneConfirmed, IS_DEBUGGING);
            CustomDebug.Log("Is NOT Conformed P2" + m_playerTwoConfirmed, IS_DEBUGGING);
        }
        switch (m_isPlayer1)
        {
            case 0:
                textPlayer1.GetComponent<TextMeshProUGUI>().color = Color.gray;
                textPlayer2.GetComponent<TextMeshProUGUI>().color = m_colorPlay2;
                break;
            case 1:
                textPlayer2.GetComponent<TextMeshProUGUI>().color = Color.gray;
                textPlayer1.GetComponent<TextMeshProUGUI>().color = m_colorPlay1;
                break;
        }

        CustomDebug.Log($"Player is deactivated: {m_isPlayer1}", IS_DEBUGGING);

        m_controllerMovement.FlipInput(m_isPlayer1);
        ChangePlayerByte();
    }

    public void CancelChanges()
    {
        DeactiveBindings();
    }


    // Ben Lussman
    /// <summary>
    /// Returns a list of dropdowns based on which part you select
    /// </summary>
    /// <param name="index"></param>
    /// <returns>list of dropdown prefabs</returns>
    public List<GameObject> GetAllDropdownsByPart(int index)
    {
        List<GameObject> dropdowns = new List<GameObject>();

        foreach (partBindings pb in m_botPartsList[index].bindings)
        {
            dropdowns.Add(pb.buttonPrefab);
        }

        return dropdowns;
    }

    public void SaveInfoOntoButton(eInputType inputType, InputValue value)
    {
        // assuming that there is 2 bindings
        if (hold_PartClicked.bindings.Count == 2)
        {
            switch (hold_PartClicked.bindings.FindIndex(x => x.buttonPrefab = hold_selectedButtonPrefab.buttonPrefab))
            {
                case 0:
                    hold_PartClicked.bindings[1].buttonPrefab.GetComponent<InputButtonInfo>().SetisPlayerOne((byte)((isPlayer1 + 1) % 2));
                    break;
                case 1:
                    hold_PartClicked.bindings[0].buttonPrefab.GetComponent<InputButtonInfo>().SetisPlayerOne((byte)((isPlayer1 + 1) % 2));
                    break;
            }
        }

        hold_selectedButtonPrefab.buttonPrefab.GetComponent<InputButtonInfo>().SetisPlayerOne(m_isPlayer1);
        hold_selectedButtonPrefab.buttonPrefab.GetComponent<InputButtonInfo>().Setinput(inputType);

        DeactiveBindings();
        ChangeActivePlayer();
    }

    /// <summary>
    /// Get function returns m_isPlayer1 variable
    /// </summary>
    /// <returns> 0 or 1 </returns>
    public byte GetisPlayerOne()
    {
        return m_isPlayer1;
    }

    public void SetPlayerConfirm(int playerIndex)
    {
        Assert.IsFalse(playerIndex != 0 && playerIndex != 1, $"Player index is invalid when trying to update player confirm status.");
        if (playerIndex != 0 && playerIndex != 1) { return; }

        if (playerIndex == 0)
        {
            m_playerOneConfirmed = !m_playerOneConfirmed;
        }
        else if (playerIndex == 1)
        {
            m_playerTwoConfirmed = !m_playerTwoConfirmed;
        }
    }

    /// <summary>
    /// Sets the player 2 text to lime green
    /// </summary>
    public void SetPlayer1TextColor()
    {
        textPlayer1.color = Color.green;
    }

    /// <summary>
    /// Sets the player 1 text to lime green
    /// </summary>
    public void SetPlayer2TextColor()
    {
        textPlayer2.color = Color.green;
    }

    public Color GetPlayer1Color()
    {
        return m_colorPlay1;
    }

    public Color GetPlayer2Color()
    {
        return m_colorPlay2;
    }
}

[System.Serializable]
public struct botInfo
{
    public PartScriptableObject botPart;
    public string partId;
    public int numOfBindings;
    public GameObject scrollListSpace;
    public Button button;
    public Texture2D partSprite;
    public GameObject partModel;
    public List<Image> currentControlPrefabImages;
    [SerializeField]
    public List<partBindings> bindings;

    public GameObject ObjectInScene;
}

[System.Serializable]
public struct partBindings
{
    public string partId;
    public string actionName;
    [SerializeField]
    public GameObject buttonPrefab;
    public byte assignedPlayer;
}

