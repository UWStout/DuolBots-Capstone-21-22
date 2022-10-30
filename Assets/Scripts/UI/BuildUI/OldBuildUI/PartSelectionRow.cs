using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using DuolBots;
using Cinemachine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
// Original Author(s) - Eslis Vang

/// <summary>
/// Part selection for a scrollview UI element.
/// </summary>
public class PartSelectionRow : MonoBehaviour
{
    // Constants
    private const float COOLDOWN_TIME = 0.15f;
    private const bool IS_DEBUGGING = false;
    private const int CENTER_ROW_INDEX = 1;

    // Contains the rows which hold the information related to their part types.
    [SerializeField] private List<List<Transform>> m_rowCategories = new List<List<Transform>>();
    // The transform which holds all of weapon part slots.
    [SerializeField] private Transform m_weaponRowTransform = null;
    // The transform which holds all of utility part slots.
    [SerializeField] private Transform m_utilityRowTransform = null;
    // The transform which holds all of movement part slots.
    [SerializeField] private Transform m_movementRowTransform = null;
    // The transform which holds all of chassis parts.
    [SerializeField] private Transform m_chassisSelectionRowTransform = null;
    [SerializeField] private Transform m_confirmRowTransform = null;
    // The base slot prefab which fills each row.
    [SerializeField] private GameObject m_slotObject;
    [SerializeField] private GameObject m_chassisObject;
    // The player's index.
    public int playerIndex => m_playerIndex;
    [SerializeField] private int m_playerIndex = -1;
    // The textbox which shows name of the hovered slot.
    public Text playerInfoTextBox => m_playerInfoTextBox;
    [SerializeField] private Text m_playerInfoTextBox = null;

    [SerializeField] private GenerateViewportBot m_botViewport = null;

    [SerializeField] private GameObject m_confirmPanel = null;

    [SerializeField] private CinemachineVirtualCamera m_virtualCamera = null;
    [SerializeField] private CinemachineFreeLook m_freeLookCamera = null;

    // TODO Change this to determine which team is building
    private byte m_teamIndex = 0;

    // The list holding the transform of each weapon part.
    private List<Transform> m_weaponRow = new List<Transform>();
    // The list holding the transform of each utility part.
    private List<Transform> m_utilityRow = new List<Transform>();
    // The list holding the transform of each movement part.
    private List<Transform> m_movementRow = new List<Transform>();
    // The list holding the transform of each chassis.
    private List<Transform> m_chassisRow = new List<Transform>();
    private List<Transform> m_confirmRow = new List<Transform>();
    // The list of all parts which players can select.
    private List<PartScriptableObject> m_partList = new List<PartScriptableObject>();
    // The slot which the player is actively hovering over.
    public Transform activeBox => m_activeBox;
    [SerializeField] private Transform m_activeBox = null;
    // The slot which the player was previously hovering over.
    [SerializeField] private Transform m_previousBox = null;
    // The slot which the player has selected.
    [SerializeField] private Transform m_selectedBox = null;
    // If the player's next input can be read.
    private bool m_readInput = true;
    // If the player has confirmed their selection.
    private bool m_isBuildSceneConfirmation = false;
    // If both players have confirmed their selection.
    private bool m_isBuildSceneFinished = false;

    public bool isAskingConfirmation => m_isAskingConfirmation;
    private bool m_isAskingConfirmation = false;
    // The indexes of the active column for each row.
    public int[] activeColumnArray => m_activeColumnArray;
    private int[] m_activeColumnArray = { 1, 1, 1, 0 };
    // The active column's index.
    private int m_activeColumn = 1;
    // The active row's index.
    private int m_activeRow = 0;
    // The number of slots the chassis has.
    private int m_chassisSlotAmount = 0;

    private PartSelectorManager m_partSelector = null;

    #region UnityMessages
    private void Awake()
    {
        // If the PartDatabase singleton exists.
        if (PartDatabase.instance != null)
        {
            CustomDebug.Log("Part Database found. Loading all parts.", IS_DEBUGGING);
            // Loads all of the PartScriptableObjects.
            m_partList = PartDatabase.instance.GetAllPartScriptableObjects();

            Assert.IsTrue(m_partList.Count > 0, $"{this.name}: m_partList is empty.");
            // Creates slots for each row. Works if m_partList is not empty.
            PopulateRows();
        }

        Assert.IsNotNull(PartDatabase.instance, "Part Database could not be found.");
        Assert.IsNotNull(m_partList, $"The part list for player {m_playerIndex + 1} is null.");
        Assert.IsNotNull(m_weaponRowTransform, $"The weapon transform for player {m_playerIndex + 1} is null.");
        Assert.IsNotNull(m_utilityRowTransform, $"The utility transform for player {m_playerIndex + 1} is null.");
        Assert.IsNotNull(m_movementRowTransform, $"The movement transform for player {m_playerIndex + 1} is null.");
        Assert.IsNotNull(m_chassisSelectionRowTransform, $"The chassis selection transform for player {m_playerIndex + 1} is null.");
    }

    // Start is called before the first frame update
    void Start()
    {
        m_partSelector = PartSelectorManager.instance;
        Assert.IsNotNull(m_partSelector, $"{this.name}: PartSelectorVariables singleton is null.");

        // Adds all the weapon parts into the weapon row.
        for (int childIndex = 0; childIndex < m_weaponRowTransform.childCount; childIndex++)
        {
            m_weaponRow.Add(m_weaponRowTransform.GetChild(childIndex));
        }
        // Adds all the utility parts into the utility row.
        for (int childIndex = 0; childIndex < m_utilityRowTransform.childCount; childIndex++)
        {
            m_utilityRow.Add(m_utilityRowTransform.GetChild(childIndex));
        }
        // Adds all the movement parts into the movement row.
        for (int childIndex = 0; childIndex < m_movementRowTransform.childCount; childIndex++)
        {
            m_movementRow.Add(m_movementRowTransform.GetChild(childIndex));
        }
        for (int childIndex = 0; childIndex < m_chassisSelectionRowTransform.childCount; childIndex++)
        {
            m_chassisRow.Add(m_chassisSelectionRowTransform.GetChild(childIndex));
        }
        for (int childIndex = 0; childIndex < m_confirmRowTransform.childCount; childIndex++)
        {
            m_confirmRow.Add(m_confirmRowTransform.GetChild(childIndex));
        }

        // Adds all the rows to the row categories list.
        m_rowCategories.Add(m_chassisRow);
        m_rowCategories.Add(m_movementRow);
        m_rowCategories.Add(m_weaponRow);
        m_rowCategories.Add(m_utilityRow);
        m_rowCategories.Add(m_confirmRow);

        Assert.IsNotNull(m_weaponRow, "Weapon transforms returned as null.");
        Assert.IsNotNull(m_utilityRow, "Utility transforms returned as null.");
        Assert.IsNotNull(m_movementRow, "Movement transforms returned as null.");
    }

    private void Update()
    {
        if (m_playerIndex == 0 && m_partSelector.playerTurnIndex == 0 && m_partSelector.playerOneConfirm && m_partSelector.isChassisSelected && m_partSelector.isMovementSelected)
        { m_partSelector.UpdatePlayerTurnIndex(); return; }
        if (m_playerIndex == 1 && m_partSelector.playerTurnIndex == 1 && m_partSelector.playerTwoConfirm && m_partSelector.isChassisSelected && m_partSelector.isMovementSelected)
        { m_partSelector.UpdatePlayerTurnIndex(); return; }

        UpdateActiveBox();
        UpdateCellHighlight(m_playerIndex);

        if (m_partSelector.IsPlayerReady(m_playerIndex))
        {
            m_playerInfoTextBox.color = Color.green;
        }

        if (!m_partSelector.isChassisSelected) { return; }
        if (!m_partSelector.isMovementSelected) { return; }

        UpdateActiveRowVisibility();

        if (m_partSelector.GetActiveRow(m_playerIndex) != 2)
        {
            ScrollViewMove temp_scrollView = this.transform.GetChild(m_partSelector.GetActiveRow(m_playerIndex)).GetComponentInChildren<ScrollViewMove>();
            temp_scrollView.MoveScrollView(m_partSelector.GetActiveRow(m_playerIndex));
        }
    }
    #endregion UnityMessages


    #region UnityEvents
    public void OnMoveInput(InputValue value)
    {
        if (value.Get<Vector2>() == null) { return; }

        Vector2 temp_moveVector = value.Get<Vector2>();
        if (m_readInput && temp_moveVector != Vector2.zero)
        {
            CooldownStart();
            m_partSelector.UpdateCellPosition(m_playerIndex, temp_moveVector);
            UpdateActiveBox();
            UpdateCellHighlight(m_playerIndex);
        }
    }

    public void OnSelection()
    {
        if (!m_partSelector.isChassisSelected || !m_partSelector.isMovementSelected)
        {
            AskConfirmation();
            //DeselectSlots();
            //ToggleSlot();
        }
        else
        {
            if (m_partSelector.playerTurnIndex == m_playerIndex)
            {
                m_selectedBox = m_activeBox;
                SelectedPartToActiveSlot();
            }
        }
    }

    public void SetSelection(int value)
    {
        if (value == 0)
        {
            m_selectedBox = m_activeBox;
            OnConfirm();
        }
        m_isAskingConfirmation = false;
    }

    private void AskConfirmation()
    {
        m_confirmPanel.SetActive(true);
        m_isAskingConfirmation = true;
    }

    public void OnConfirm()
    {
        if (m_partSelector.isChassisSelected && m_partSelector.isMovementSelected)
        {
            m_isBuildSceneConfirmation = !m_isBuildSceneConfirmation;
            m_partSelector.SetPlayerConfirm(m_playerIndex);
            // Disable player's input
            m_readInput = !m_readInput;
            m_partSelector.UpdateCellPosition(m_playerIndex, Vector2.zero);
            UpdateCellHighlight(m_playerIndex);
            if (m_partSelector.playerOneConfirm && m_partSelector.playerTwoConfirm)
            {
                m_isBuildSceneFinished = true;
                CustomDebug.Log(BuildSceneBotData.GetBotData(m_teamIndex).slottedPartIDList.Count, IS_DEBUGGING);
                SetBuildData();
                m_partSelector.LoadNextScene();
            }
        }
        else
        {
            if (m_selectedBox != null && !m_partSelector.isChassisSelected || !m_partSelector.isMovementSelected)
            {
                m_isBuildSceneConfirmation = !m_isBuildSceneConfirmation;
                m_partSelector.UpdatePlayerTurnIndex();
                m_partSelector.SetPlayerConfirm(m_playerIndex);
                /* If both players have confirmed, randomly choose one of chassis that were selected. The player whose chassis wasn't selected becomes the first player to fill a slot.
                 * If both players choose the same chassis, only the first player who gets to fill a slot is randomized.
                 */
                if (m_partSelector.playerOneConfirm && m_partSelector.playerTwoConfirm)
                {
                    m_partSelector.SetBothPlayerConfirmation(false);
                    Random.InitState((int)System.DateTime.Now.Ticks);
                    // Generates a random player index.
                    int temp_randomizedNumber = (int)Random.Range(0f, 99f);
                    CustomDebug.Log($"{this.name}: Randomized number before modulus: {temp_randomizedNumber}", IS_DEBUGGING);
                    temp_randomizedNumber %= 2;
                    CustomDebug.Log($"{this.name}: Randomized number after modulus: {temp_randomizedNumber}", IS_DEBUGGING);
                    if (!m_partSelector.isChassisSelected)
                    {
                        // Sets chassis to the chassis of the corresponding player index.
                        m_partSelector.SetChassisSelection(temp_randomizedNumber);
                        m_chassisSelectionRowTransform.gameObject.SetActive(false);
                        m_movementRowTransform.gameObject.SetActive(true);
                        m_virtualCamera.gameObject.SetActive(false);
                        m_freeLookCamera.gameObject.SetActive(true);
                    }
                    else
                    {
                        m_partSelector.SetMovementSelection(temp_randomizedNumber);
                        m_movementRowTransform.gameObject.SetActive(false);
                    }
                    // Gets the other player's index.
                    int temp_playerTurnIndex = ++temp_randomizedNumber % 2;
                    // Sets the other player's index to be first in turn of selecting parts.
                    m_partSelector.SetPlayerTurnIndex(temp_playerTurnIndex);
                    m_activeBox = null;
                    m_activeColumn = 1;
                    CustomDebug.Log($"{this.name}: Player Turn Index: {temp_playerTurnIndex}", IS_DEBUGGING);
                }
            }
        }
    }

    public void OnCancel()
    {
        if (m_partSelector.playerTurnIndex == m_playerIndex)
        {
            if (m_selectedBox != null)
            {
                DeselectSlots();
                m_selectedBox = null;
            }
            if (m_partSelector.GetActiveRow(m_playerIndex) == 3)
            {
                m_partSelector.UpdateCellPosition(m_playerIndex, Vector2.zero);
                UpdateActiveBox();
                UpdateCellHighlight(m_playerIndex);
            }

            if (m_playerIndex == 0)
                m_partSelector.SetPlayerConfirm(m_playerIndex, false);
            if (m_playerIndex == 1)
                m_partSelector.SetPlayerConfirm(m_playerIndex, false);

            m_confirmPanel.SetActive(false);
            if (m_isAskingConfirmation)
            {
                m_isAskingConfirmation = false;
            }
        }
    }

    public void OnShift(InputValue value)
    {
        if (m_partSelector.playerTurnIndex != m_playerIndex) { return; }

        m_partSelector.MoveSlotPosition(m_playerIndex, (int)value.Get<float>());
    }
    #endregion UnityEvents


    #region HelperFunctions
    public int GetActiveRow()
    {
        return m_activeRow;
    }

    public int GetActiveRowSize(int rowIndex)
    {
        return m_rowCategories[rowIndex].Count;
    }

    public int GetActiveColumn()
    {
        return m_activeColumn;
    }

    public int GetSelectedSlotColumnIndex()
    {
        return m_selectedBox.GetSiblingIndex();
    }

    public void SetBuildConfirm(bool newState)
    {
        m_isBuildSceneConfirmation = newState;
    }

    public void OnChassisSelected()
    {
        m_selectedBox = null;
        m_rowCategories.Remove(m_chassisRow);
        CreateChassisSlots();
        m_partSelector.UpdateCellPosition(m_playerIndex, Vector2.zero);
        UpdateActiveBox();
        // Updates the hover highlight.
        UpdateCellHighlight(m_playerIndex);
        SetBuildData();
    }

    public void OnMovementSelected()
    {
        m_selectedBox = null;
        m_rowCategories.Remove(m_movementRow);
        m_partSelector.UpdateCellPosition(m_playerIndex, Vector2.zero);
        UpdateActiveBox();
        UpdateCellHighlight(m_playerIndex);
        SetBuildData();
    }

    public void UpdateActiveBox()
    {
        if (m_activeBox != null && m_previousBox == null)
        {
            m_previousBox = m_activeBox;
        }
        m_activeRow = m_partSelector.GetActiveRow(m_playerIndex);
        m_activeColumn = m_partSelector.GetActiveColumn(m_playerIndex);
        m_activeBox = m_rowCategories[m_activeRow][m_activeColumn];
    }

    /// <summary>
    /// Updates the previously active slot and new active slot's highlight.
    /// </summary>
    public void UpdateCellHighlight(int playerIndex)
    {
        foreach (Transform box in m_activeBox.parent)
        {
            box.GetChild(2 + playerIndex).gameObject.SetActive(false);
        }
        m_activeBox.GetChild(2 + playerIndex).gameObject.SetActive(true);
        //if (m_previousBox != null && m_previousBox != m_activeBox)
        //{
        //    m_previousBox.GetChild(2 + playerIndex).gameObject.SetActive(false);
        //    m_previousBox = null;
        //}
    }

    /// <summary>
    /// Starts a cooldown timer until the next input could be read.
    /// </summary>
    private void CooldownStart()
    {
        m_readInput = false;
        Invoke("CoolDownEnd", COOLDOWN_TIME);
    }

    /// <summary>
    /// Allows the next input to be read.
    /// </summary>
    private void CoolDownEnd()
    {
        m_readInput = true;
    }

    /// <summary>
    /// Populates each row with slots of the corresponding types.
    /// </summary>
    private void PopulateRows()
    {
        // Creates slots for each scriptable object.
        foreach (PartScriptableObject part in m_partList)
        {
            CustomDebug.Log($"Importing {part.partName} to object lists.", IS_DEBUGGING);
            if (part.partType == ePartType.Weapon)
            {
                InstantiateScriptableObject(m_weaponRowTransform, part);
            }
            else if (part.partType == ePartType.Utility)
            {
                InstantiateScriptableObject(m_utilityRowTransform, part);
            }
            else if (m_playerIndex == 0 && part.partType == ePartType.Movement)
            {
                InstantiateScriptableObject(m_movementRowTransform, part);
            }
            else if (m_playerIndex == 0 && part.partType == ePartType.Chassis)
            {
                // Creates a slot for the chassis selection panel.
                InstantiateScriptableObject(m_chassisSelectionRowTransform, part);
            }
        }
        InstantiateSlotObject(m_confirmRowTransform, "ConfirmButton", Color.green);

        // Creates empty slots if any of the rows are empty.
        if (m_weaponRowTransform.childCount <= 0)
        {
            InstantiateScriptableObject(m_weaponRowTransform, null);
        }
        if (m_utilityRowTransform.childCount <= 0)
        {
            InstantiateScriptableObject(m_utilityRowTransform, null);
        }
        if (m_movementRowTransform.childCount <= 0)
        {
            InstantiateScriptableObject(m_movementRowTransform, null);
        }

        // Creates first and last slots for visual looping.
        GenerateRowLoop(m_weaponRowTransform);
        GenerateRowLoop(m_utilityRowTransform);
        GenerateRowLoop(m_confirmRowTransform);
    }

    /// <summary>
    /// Instantiates an object using information from the given scriptable object. If the scriptable object is null, it will instantiate an empty object instead.
    /// </summary>
    /// <param name="temp_newParentTransform">The parent object of the newly instantiated objects.</param>
    /// <param name="scriptableObject">The scriptable object which data is pulled from</param>
    private void InstantiateScriptableObject(Transform temp_newParentTransform, PartScriptableObject scriptableObject)
    {
        if (scriptableObject == null) { return; }
        GameObject temp_newObject = null;
        if (scriptableObject.partType == ePartType.Chassis)
        {
            // Instantiates a slot object.
            temp_newObject = Instantiate(m_chassisObject, temp_newParentTransform);
        }
        else if (scriptableObject.partType != ePartType.Chassis)
        {
            // Instantiates a slot object.
            temp_newObject = Instantiate(m_slotObject, temp_newParentTransform);
        }
        // Renames object to the part's name.
        temp_newObject.name = scriptableObject.name;
        // Rescales the object to one.
        temp_newObject.transform.localScale = Vector3.one;
        // Sets the image of the slot to the part's image.
        temp_newObject.GetComponent<RawImage>().texture = scriptableObject.partUIData.unlockedSprite;

        RectTransform temp_rectTransform = temp_newObject.GetComponent<RectTransform>();
        // Sets the z of the instantiated object to 0. (Otherwise it may cause some visual issues)
        temp_rectTransform.position = new Vector3(temp_rectTransform.position.x, temp_rectTransform.position.y, 0f);
    }

    private void InstantiateSlotObject(Transform newParentTransform, string objectName)
    {
        if (newParentTransform == null) { return; }

        GameObject temp_newObject = Instantiate(m_slotObject, newParentTransform);

        temp_newObject.name = objectName;

        temp_newObject.transform.localScale = Vector3.one;
        // Sets the color of the slot to be clear.
        temp_newObject.GetComponent<RawImage>().color = Color.white;

        RectTransform temp_rectTransform = temp_newObject.GetComponent<RectTransform>();
        // Sets the z of the instantiated object to 0. (Otherwise it may cause some visual issues)
        temp_rectTransform.position = new Vector3(temp_rectTransform.position.x, temp_rectTransform.position.y, 0f);
    }

    private void InstantiateSlotObject(Transform newParentTransform, string objectName, Color color)
    {
        if (newParentTransform == null) { return; }

        GameObject temp_newObject = Instantiate(m_slotObject, newParentTransform);

        temp_newObject.name = objectName;

        temp_newObject.transform.localScale = Vector3.one;
        // Sets the color of the slot to be clear.
        temp_newObject.GetComponent<RawImage>().color = color;

        RectTransform temp_rectTransform = temp_newObject.GetComponent<RectTransform>();
        // Sets the z of the instantiated object to 0. (Otherwise it may cause some visual issues)
        temp_rectTransform.position = new Vector3(temp_rectTransform.position.x, temp_rectTransform.position.y, 0f);
    }

    /// <summary>
    /// Copies the first and last slots of the row and places them at opposite ends. For visual looping purposes.
    /// </summary>
    /// <param name="temp_parentTransform">The parent object of the newly instantiated objects.</param>
    private void GenerateRowLoop(Transform temp_parentTransform)
    {
        Debug.Log($"Making loop for {temp_parentTransform.name}");
        // Instantiates a slot object.
        Transform temp_firstObject = Instantiate(temp_parentTransform.GetChild(0), temp_parentTransform);
        // Rescales the object to one.
        temp_firstObject.transform.localScale = Vector3.one;
        // Sets the color of the slot the same as the original.
        temp_firstObject.GetComponent<RawImage>().color = temp_parentTransform.GetChild(0).GetComponent<RawImage>().color;
        temp_firstObject.transform.SetAsLastSibling();

        RectTransform temp_firstRectTransform = temp_firstObject.GetComponent<RectTransform>();
        // Sets the z of the instantiated object to 0. (Otherwise it may cause some visual issues)
        temp_firstRectTransform.position = new Vector3(temp_firstRectTransform.position.x, temp_firstRectTransform.position.y, 0f);

        // Instantiates a slot object.
        Transform temp_lastObject = Instantiate(temp_parentTransform.GetChild(temp_parentTransform.childCount - 2), temp_parentTransform);
        // Rescales the object to one.
        temp_lastObject.transform.localScale = Vector3.one;
        // Sets the color of the slot the same as the original.
        temp_lastObject.GetComponent<RawImage>().color = temp_parentTransform.GetChild(temp_parentTransform.childCount - 1).GetComponent<RawImage>().color;
        temp_lastObject.transform.SetAsFirstSibling();

        RectTransform temp_lastRectTransform = temp_lastObject.GetComponent<RectTransform>();
        // Sets the z of the instantiated object to 0. (Otherwise it may cause some visual issues)
        temp_lastRectTransform.position = new Vector3(temp_lastRectTransform.position.x, temp_lastRectTransform.position.y, 0f);
    }

    private void CreateChassisSlots()
    {
        m_chassisSlotAmount = m_partSelector.selectedChassis.modelPrefab.GetComponent<SlotPlacementManager>().GetSlotAmount();
    }

    private void SelectedPartToActiveSlot()
    {
        bool temp_botDataChanged = false;
        PartInSlot temp_partInSlot = null;
        PartScriptableObject temp_partSO = null;

        // Searches through all the parts for the currently selected part.
        foreach (PartScriptableObject part in m_partList)
        {
            if (part.name == m_activeBox.name)
            {
                temp_partSO = part;
                temp_partInSlot = new PartInSlot(temp_partSO.partID, (byte)
                    m_partSelector.activeSlotIndex);
                break;
            }
        }

        if (temp_partSO == null) { return; }
        if (temp_partInSlot == null) { return; }

        // Assigns the movement part if the selected part was a movement type part.
        if (temp_partSO.partType == ePartType.Movement)
        {
            m_partSelector.SetMovementSelection(temp_partSO);
            temp_botDataChanged = true;
        }
        else if (m_partSelector.activeSlotIndex < 0) { }
        // Assigns the selected part to the active slot.
        else
        {
            foreach (PartInSlot slotInfo in m_partSelector.slotData.ToArray())
            {
                if (slotInfo.slotIndex == m_partSelector.activeSlotIndex)
                {
                    m_partSelector.RemoveSlotData(slotInfo);
                    SetBuildData();
                }
            }
            m_partSelector.AddSlotData(temp_partInSlot);
            temp_botDataChanged = true;
        }

        if (temp_botDataChanged)
        {
            SetBuildData();
            m_partSelector.UpdatePlayerTurnIndex();
        }
    }

    private void DeselectSlots()
    {
        if (m_selectedBox != null)
        {
            // Disables the selection highlight for the selected slot.
            m_selectedBox.GetChild(m_playerIndex).gameObject.SetActive(false);

            if (m_selectedBox.GetSiblingIndex() == CENTER_ROW_INDEX || m_selectedBox.GetSiblingIndex() == m_selectedBox.parent.childCount - (1 + CENTER_ROW_INDEX))
            {
                m_selectedBox.parent.GetChild(0).GetChild(m_playerIndex).gameObject.SetActive(false);
                m_selectedBox.parent.GetChild(m_selectedBox.parent.childCount - 1).GetChild(m_playerIndex).gameObject.SetActive(false);
            }
        }
    }

    private void ToggleSlot()
    {
        // Name of the current row's first slot. 
        string temp_firstSlotName = m_rowCategories[m_activeRow][CENTER_ROW_INDEX].name;
        // Name of the current row's last slot.
        string temp_lastSlotName = m_rowCategories[m_activeRow][m_rowCategories[m_activeRow].Count - (1 + CENTER_ROW_INDEX)].name;

        if (m_selectedBox == null && !m_partSelector.isChassisSelected || !m_partSelector.isMovementSelected)
        {
            m_selectedBox = m_activeBox;
        }
        else if (m_selectedBox != null && m_activeBox == m_selectedBox && !m_partSelector.isChassisSelected || !m_partSelector.isMovementSelected)
        {
            m_selectedBox = null;
        }

        CustomDebug.Log($"Slot Selection active?: {m_activeBox.GetChild(0).gameObject.activeSelf}", IS_DEBUGGING);
        if (m_activeRow == 3 && m_selectedBox != null)
        {
            if (m_activeBox == m_rowCategories[m_activeRow][m_rowCategories[m_activeRow].Count - 1])
            {
                foreach (PartScriptableObject part in m_partList)
                {
                    if (part != null && m_selectedBox != null)
                    {
                        if (part.partType == ePartType.Movement && m_selectedBox.name == part.partID)
                        {
                            m_partSelector.SetMovementSelection(part);
                            SelectedPartToActiveSlot();
                            m_selectedBox = null;
                            m_activeRow = 0;
                            m_activeColumn = m_activeColumnArray[m_activeRow];
                            UpdateCellHighlight(m_playerIndex);
                        }
                    }
                }
            }
            else
            {
                if (PartDatabase.instance.GetPartScriptableObject(m_selectedBox.name).partType != ePartType.Movement)
                {
                    SelectedPartToActiveSlot();
                }
            }
            m_selectedBox = null;
            m_partSelector.UpdateCellPosition(m_playerIndex, Vector2.zero);
            UpdateActiveBox();
            UpdateCellHighlight(m_playerIndex);
            SetBuildData();
        }
        else if (!m_activeBox.GetChild(m_playerIndex).gameObject.activeSelf)
        {
            // Enables the selection highlight of the hovered slot.
            m_activeBox.GetChild(m_playerIndex).gameObject.SetActive(true);

            if (m_activeRow != 3 && m_partSelector.isChassisSelected && m_partSelector.isMovementSelected)
            {
                // If the hovered slot is also the first and last slots.
                if (m_activeBox.name == temp_firstSlotName && m_activeBox.name == temp_lastSlotName)
                {
                    // Enables both the first and last slots' selection highlights.
                    m_rowCategories[m_activeRow][0].GetChild(m_playerIndex).gameObject.SetActive(true);

                    m_rowCategories[m_activeRow][m_rowCategories[m_activeRow].Count - 1].GetChild(m_playerIndex).gameObject.SetActive(true);
                }
                // If the hovered slot is only the first slot.
                else if (m_activeBox.name == temp_firstSlotName)
                {
                    // Enables the first slot's selection highlights.
                    m_rowCategories[m_activeRow][m_rowCategories[m_activeRow].Count - 1].GetChild(m_playerIndex).gameObject.SetActive(true);
                }
                // If the hovered slot is only the last slot.
                else if (m_activeBox.name == temp_lastSlotName)
                {
                    // Enables the last slot's selection highlights.
                    m_rowCategories[m_activeRow][0].GetChild(m_playerIndex).gameObject.SetActive(true);
                }
            }

            m_selectedBox = m_activeBox;
        }
        else if (m_activeBox.GetChild(m_playerIndex).gameObject.activeSelf && m_partSelector.isChassisSelected && m_partSelector.isMovementSelected)
        {
            // Disables the selection highlight of the hovered slot.
            m_activeBox.GetChild(m_playerIndex).gameObject.SetActive(false);

            if (m_activeRow != 3)
            {
                // If the hovered slot is also the first and last slots.
                if (m_activeBox.name == temp_firstSlotName && m_activeBox.name == temp_lastSlotName)
                {
                    // Disables both the first and last slots' selection highlights.
                    m_rowCategories[m_activeRow][0].GetChild(m_playerIndex).gameObject.SetActive(false);
                    m_rowCategories[m_activeRow][m_rowCategories[m_activeRow].Count - 1].GetChild(m_playerIndex).gameObject.SetActive(false);
                }
                // If the hovered slot is only the first slot.
                else if (m_activeBox.name == temp_firstSlotName)
                {
                    // Disables the first slot's selection highlights.
                    m_rowCategories[m_activeRow][m_rowCategories[m_activeRow].Count - 1].GetChild(m_playerIndex).gameObject.SetActive(false);
                }
                // If the hovered slot is only the last slot.
                else if (m_activeBox.name == temp_lastSlotName)
                {
                    // Disables the last slot's selection highlights.
                    m_rowCategories[m_activeRow][0].GetChild(m_playerIndex).gameObject.SetActive(false);
                }
            }

            m_selectedBox = null;
        }
    }

    private void UpdateTextBox()
    {

    }

    private void SetBuildData()
    {
        if (m_isBuildSceneFinished)
        {
            CustomDebug.LogWarning($"{BuildSceneBotData.GetBotData(m_teamIndex).slottedPartIDList.Count}");
        }
        else
        {
            if (m_partSelector.slotData.Count > 0 && m_partSelector.selectedMovement != null && m_partSelector.selectedChassis != null)
            {
                BuildSceneBotData.SetData(m_teamIndex, new BuiltBotData(m_partSelector.selectedChassis.partID,
                    m_partSelector.selectedMovement.partID, m_partSelector.slotData));
            }
            else if (m_partSelector.selectedMovement != null)
            {
                BuildSceneBotData.SetData(m_teamIndex, new BuiltBotData(m_partSelector.selectedChassis.partID,
                    m_partSelector.selectedMovement.partID, new List<PartInSlot>()));
            }
            else if (m_partSelector.selectedChassis != null)
            {
                BuildSceneBotData.SetData(m_teamIndex, new BuiltBotData(m_partSelector.selectedChassis.partID,
                    null, new List<PartInSlot>()));
            }
        }
        m_botViewport.BuildBot();
    }

    private void UpdateActiveRowVisibility()
    {
        // Swap between active rows. Disables inactive rows to avoid overlapping when selecting parts.
        m_weaponRowTransform.parent.parent.gameObject.SetActive(false);
        m_utilityRowTransform.parent.parent.gameObject.SetActive(false);
        m_confirmRowTransform.parent.parent.gameObject.SetActive(false);

        switch (m_activeRow)
        {
            case 0:
                m_weaponRowTransform.parent.parent.gameObject.SetActive(true);
                break;
            case 1:
                m_utilityRowTransform.parent.parent.gameObject.SetActive(true);
                break;
            case 2:
                m_confirmRowTransform.parent.parent.gameObject.SetActive(true);
                break;
            case 3:
                if (m_selectedBox != null)
                {
                    m_selectedBox.parent.parent.parent.gameObject.SetActive(true);
                }
                break;
            default:
                break;
        }
        foreach (List<Transform> row in m_rowCategories)
        {
            int temp_index = 0;
            foreach (Transform child in row)
            {
                if (child == row[temp_index++])
                {
                    child.gameObject.SetActive(true);
                }
                else
                    child.gameObject.SetActive(false);
            }
        }
    }
    #endregion HelperFunctions
}
