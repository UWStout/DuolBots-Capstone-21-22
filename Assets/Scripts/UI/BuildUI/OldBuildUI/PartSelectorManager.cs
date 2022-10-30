using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using DuolBots;
using NaughtyAttributes;
using UnityEngine.SceneManagement;
// Original Author(s) - Eslis Vang

/// <summary>
/// Manages the data for each player and the parts for the bot being built.
/// </summary>
public class PartSelectorManager : SingletonMonoBehaviourPersistant<PartSelectorManager>
{
    private const bool IS_DEBUGGING = true;
    private const int CELL_PADDING_SIZE = 1;

    public PartSelectionRow activeSelectionRow => m_selectionRow[playerTurnIndex];
    [SerializeField] private List<PartSelectionRow> m_selectionRow = new List<PartSelectionRow>();
    [SerializeField] private PartSelectionSinglePartTypeSelection m_chassisSelection = null;
    [SerializeField] private PartSelectionSinglePartTypeSelection m_movementSelection = null;
    public PartScriptableObject selectedChassis => m_selectedChassis;
    [SerializeField] private PartScriptableObject m_selectedChassis = null;
    public PartScriptableObject selectedMovement => m_selectedMovement;
    [SerializeField] private PartScriptableObject m_selectedMovement = null;
    [SerializeField] private Text m_playerTurnTextBox = null;
    [SerializeField] private GenerateViewportBot m_botViewport = null;
    [SerializeField] private GameObject m_botObject = null;
    public List<PartInSlot> slotData => m_slotData;
    [SerializeField] private List<PartInSlot> m_slotData = new List<PartInSlot>();
    [SerializeField] private ChassisSelectionCinemachineCameraTarget m_cinemachineManager = null;
    [SerializeField] [Scene] private string m_nextScene = "NewAssignScene";
    [SerializeField] private GameObject m_chassisModels = null;
    [SerializeField] private PartSelectTutorialManager m_tutorialManager = null;

    public bool isChassisSelected => m_isChassisSelected;
    private bool m_isChassisSelected = false;
    public bool isMovementSelected => m_isMovementSelected;
    private bool m_isMovementSelected = false;
    public bool playerOneConfirm => m_playerOneConfirm;
    private bool m_playerOneConfirm = false;
    public bool playerTwoConfirm => m_playerTwoConfirm;
    private bool m_playerTwoConfirm = false;
    public int playerTurnIndex => m_playerTurnIndex;
    private int m_playerTurnIndex = 0;
    public int activeSlotIndex => m_activeSlot;
    private int m_activeSlot = -1;
    private int m_playerOneActiveRow = 0;
    private int m_playerOneActiveColumn = CELL_PADDING_SIZE;
    private int m_playerTwoActiveRow = 0;
    private int m_playerTwoActiveColumn = CELL_PADDING_SIZE;
    private LinkedList<int> m_playerOneColumnArray = new LinkedList<int>();
    private LinkedList<int> m_playerTwoColumnArray = new LinkedList<int>();

    #region UnityMessages
    // Domestic Initialization
    protected override void Awake()
    {
        // Call the singleton's awake.
        base.Awake();
    }

    private void Start()
    {
        m_tutorialManager = FindObjectOfType<PartSelectTutorialManager>();
        LoadSelectionRows();
        for (int count = 0; count < 3; count++)
        {
            m_playerOneColumnArray.AddFirst(1);
            m_playerTwoColumnArray.AddFirst(1);
        }
        m_playerOneColumnArray.AddLast(0);
        m_playerTwoColumnArray.AddLast(0);
    }

    // Update is called once per frame
    void Update()
    {
        m_playerTurnTextBox.text = $"Player {playerTurnIndex + 1}'s Turn";

        if (!m_isChassisSelected) { return; }
        if (m_botViewport.botObject == null) { return; }

        if (m_botObject == null)
            m_botObject = m_botViewport.botObject;
    }
    #endregion UnityMessages


    #region UnityEvents
    #endregion UnityEvents


    #region HelperMethods
    #region PublicFunctions
    public int GetActiveRow(int playerIndex)
    {
        Assert.IsFalse(playerIndex != 0 && playerIndex != 1, $"Player index is invalid when getting active row.");
        if (playerIndex != 0 && playerIndex != 1) { return -1; }

        if (playerIndex == 0)
        {
            return m_playerOneActiveRow;
        }
        else
        {
            return m_playerTwoActiveRow;
        }
    }

    public int GetActiveColumn(int playerIndex)
    {
        Assert.IsFalse(playerIndex != 0 && playerIndex != 1, $"Player index is invalid when getting active column.");
        if (playerIndex != 0 && playerIndex != 1) { return -1; }

        if (playerIndex == 0)
        {
            return m_playerOneActiveColumn;
        }
        else
        {
            return m_playerTwoActiveColumn;
        }
    }

    public PartScriptableObject GetChassisDataAtIndex(int index)
    {
        return m_chassisSelection.GetNodeAtIndex(index);
    }

    public void SetChassisSelection(int playerIndex)
    {
        Assert.IsFalse(playerIndex != 0 && playerIndex != 1, $"Player index is invalid when setting chassis selection.");
        if (playerIndex != 0 && playerIndex != 1) { return; }
        if (selectedChassis != null) { return; }

        m_selectedChassis = m_chassisSelection.GetNodeAtIndex(m_selectionRow[playerIndex].GetSelectedSlotColumnIndex());
        m_isChassisSelected = true;

        m_selectionRow[playerIndex].OnChassisSelected();
        m_selectionRow[(playerIndex + 1) % 2].OnChassisSelected();

        m_playerOneActiveColumn = 1;
        m_playerTwoActiveColumn = 1;
        m_selectionRow[playerIndex].UpdateCellHighlight(playerIndex);
        m_selectionRow[(playerIndex + 1) % 2].UpdateCellHighlight((playerIndex + 1) % 2);
        if (m_tutorialManager.isTutorialActive)
        {
            m_tutorialManager.NextPanel();
        }

        m_chassisModels.SetActive(false);

        CustomDebug.Log($"P{playerIndex + 1} has changed the selected chassis.", IS_DEBUGGING);

        Assert.IsNotNull(m_selectedChassis, $"Player {playerIndex + 1} selected a chassis but it was null.");
    }

    public void SetMovementSelection(int playerIndex)
    {
        m_selectedMovement = m_movementSelection.GetNodeAtIndex(m_selectionRow[playerIndex].GetSelectedSlotColumnIndex());
        m_isMovementSelected = true;

        m_selectionRow[playerIndex].OnMovementSelected();
        m_selectionRow[(playerIndex + 1) % 2].OnMovementSelected();

        foreach (PartSelectionRow playerRowObj in m_selectionRow)
        {
            Transform temp_rows = playerRowObj.transform;
            foreach (Transform childRows in temp_rows)
            {
                childRows.gameObject.SetActive(true);
            }
        }
        if (m_tutorialManager.isTutorialActive)
        {
            m_tutorialManager.NextPanel();
        }

        m_cinemachineManager.gameObject.SetActive(true);
    }

    public void SetMovementSelection(PartScriptableObject movementPart)
    {
        m_selectedMovement = movementPart;
        m_isMovementSelected = true;
    }

    public void SetPlayerConfirm(int playerIndex)
    {
        Assert.IsFalse(playerIndex != 0 && playerIndex != 1, $"Player index is invalid when trying to update player confirm status.");
        if (playerIndex != 0 && playerIndex != 1) { return; }

        if (playerIndex == 0)
        {
            m_playerOneConfirm = !m_playerOneConfirm;
            if (m_playerOneConfirm)
                m_selectionRow[playerIndex].playerInfoTextBox.color = Color.green;
            else
                m_selectionRow[playerIndex].playerInfoTextBox.color = Color.white;
        }
        else if (playerIndex == 1)
        {
            m_playerTwoConfirm = !m_playerTwoConfirm;
            if (m_playerTwoConfirm)
                m_selectionRow[playerIndex].playerInfoTextBox.color = Color.green;
            else
                m_selectionRow[playerIndex].playerInfoTextBox.color = Color.white;
        }
    }

    public void SetPlayerConfirm(int playerIndex, bool value)
    {
        Assert.IsFalse(playerIndex != 0 && playerIndex != 1, $"Player index is invalid when trying to update player confirm status.");
        if (playerIndex != 0 && playerIndex != 1) { return; }

        if (playerIndex == 0)
        {
            m_playerOneConfirm = value;
            if (value)
                m_selectionRow[playerIndex].playerInfoTextBox.color = Color.green;
            else
                m_selectionRow[playerIndex].playerInfoTextBox.color = Color.white;
        }
        else if (playerIndex == 1)
        {
            m_playerTwoConfirm = value;
            if (value)
                m_selectionRow[playerIndex].playerInfoTextBox.color = Color.green;
            else
                m_selectionRow[playerIndex].playerInfoTextBox.color = Color.white;
        }
    }

    public LinkedListNode<int> GetColumnNodeFromRowIndex(int playerIndex, int index)
    {
        Assert.IsFalse(playerIndex != 0 && playerIndex != 1, $"Player index is invalid when trying to get column node from row {index}.");
        if (playerIndex != 0 && playerIndex != 1) { return null; }

        LinkedListNode<int> node;
        if (playerIndex == 0)
        {
            node = m_playerOneColumnArray.First;
        }
        else
        {
            node = m_playerTwoColumnArray.First;
        }
        for (int i = 0; i < index; i++)
        {
            if ((node = node.Next) == null)
            {
                return null;
            }
        }
        return node;
    }

    public void UpdateCellPosition(int playerIndex, Vector2 direction)
    {
        Assert.IsFalse(playerIndex != 0 && playerIndex != 1, $"Player index is invalid when updating cell position.");
        if (playerIndex != 0 && playerIndex != 1) { return; }

        if (m_isChassisSelected && m_isMovementSelected && direction == Vector2.zero)
        {
            if (playerIndex == 0)
            {
                m_playerOneActiveColumn = GetColumnNodeFromRowIndex(playerIndex, 0).Value;
                m_playerOneActiveRow = 0;
            }
            else
            {
                m_playerTwoActiveColumn = GetColumnNodeFromRowIndex(playerIndex, 0).Value;
                m_playerTwoActiveRow = 0;
            }
        }

        if (!m_isChassisSelected || !m_isMovementSelected)
        {
            MovePlayerPosition(playerIndex, direction, false, false);
            m_cinemachineManager.SetNewCameraTarget(-1, m_playerTurnIndex);
        }
        else
        {
            MovePlayerPosition(playerIndex, direction, true, false);
        }
    }

    public void SetPlayerTurnIndex(int newPlayerTurnIndex)
    {
        m_playerTurnIndex = newPlayerTurnIndex;
        m_playerTurnTextBox.text = $"Player {playerTurnIndex + 1}'s Turn";
    }

    public void UpdatePlayerTurnIndex()
    {
        m_playerTurnIndex = ++m_playerTurnIndex % 2;
        m_playerTurnTextBox.text = $"Player {playerTurnIndex + 1}'s Turn";
    }

    public void SetBothPlayerConfirmation(bool newConfirmationState)
    {
        m_playerOneConfirm = newConfirmationState;
        m_playerTwoConfirm = newConfirmationState;
        m_selectionRow[0].SetBuildConfirm(newConfirmationState);
        m_selectionRow[1].SetBuildConfirm(newConfirmationState);

        if (m_playerOneConfirm)
            m_selectionRow[0].playerInfoTextBox.color = Color.green;
        else
            m_selectionRow[0].playerInfoTextBox.color = Color.white;
        if (m_playerTwoConfirm)
            m_selectionRow[1].playerInfoTextBox.color = Color.green;
        else
            m_selectionRow[1].playerInfoTextBox.color = Color.white;
    }

    public void AddSlotData(PartInSlot slot)
    {
        m_slotData.Add(slot);
    }

    public void RemoveSlotData(PartInSlot slot)
    {
        m_slotData.Remove(slot);
    }

    public bool IsPlayerReady(int playerIndex)
    {
        if (playerIndex == 0)
        {
            return playerOneConfirm;
        }
        else if (playerIndex == 1)
        {
            return playerTwoConfirm;
        }
        else
        {
            CustomDebug.Log("Player index out of bounds when checking if player is ready.", IS_DEBUGGING);
            return false;
        }
    }

    public void MoveSlotPosition(int playerIndex, int direction)
    {
        if (!m_isChassisSelected) { return; }
        if (!m_isMovementSelected) { return; }
        if (playerTurnIndex != playerIndex) { return; }
        if (Mathf.Abs(direction) != 1) { return; }

        SlotPlacementManager temp_slotManager = m_selectedChassis.modelPrefab.GetComponent<SlotPlacementManager>();
        int temp_slotAmount = temp_slotManager.GetSlotAmount();

        m_activeSlot = Mathf.Clamp(m_activeSlot + direction, -2, temp_slotAmount);
        if (m_activeSlot == -2)
        {
            m_activeSlot = temp_slotAmount - 1;
        }
        else if (m_activeSlot == temp_slotAmount)
        {
            m_activeSlot = -1;
        }

        m_cinemachineManager.SetNewCameraTarget(m_activeSlot + 1, playerIndex);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(m_nextScene);
        Destroy(this);
    }
    #endregion PublicFunctions


    #region PrivateFunctions
    private void LoadSelectionRows()
    {
        if (m_selectionRow.Count <= 0)
        {
            m_selectionRow.Add(GameObject.Find("P1ScrollViewManager").GetComponent<PartSelectionRow>());
            m_selectionRow.Add(GameObject.Find("P2ScrollViewManager").GetComponent<PartSelectionRow>());
        }
        Assert.IsNotNull(m_selectionRow[0], $"{this.name}: Selection row for P1 is null.");
        Assert.IsNotNull(m_selectionRow[1], $"{this.name}: Selection row for P2 is null.");
    }

    private void MovePlayerPosition(int playerIndex, Vector2 direction, bool invertControls, bool invertHorizontal)
    {
        int temp_activeColumn = GetActiveColumn(playerIndex);
        int temp_activeRow = GetActiveRow(playerIndex);
        int temp_activeRowSize = GetActiveRowSize(playerIndex, temp_activeRow);
        float temp_horizontalInput = 0f;
        float temp_verticalInput = 0f;

        if (invertControls && invertHorizontal)
        {
            temp_horizontalInput = -direction.y;
            temp_verticalInput = direction.x;
        }
        else if (invertControls)
        {
            temp_horizontalInput = -direction.y;
            temp_verticalInput = -direction.x;
        }
        else if (invertHorizontal)
        {
            temp_horizontalInput = -direction.x;
            temp_verticalInput = direction.y;
        }
        else
        {
            temp_horizontalInput = direction.x;
            temp_verticalInput = direction.y;
        }

        CustomDebug.Log($"H: {temp_horizontalInput} V: {temp_verticalInput} | Controls inverted: {invertControls}", IS_DEBUGGING);

        if (temp_horizontalInput != 0)
        {
            // Input Right
            if (temp_horizontalInput > 0)
            {
                /*
                 * Cases:
                 *  If chassis isn't selected or if player is in the chassis slot row, we want to loop from 0 to row count.
                 *  If in any other row, loop from 1 to row count - 1.
                 */
                if (!m_isChassisSelected || !m_isMovementSelected)
                {
                    temp_activeColumn = ++temp_activeColumn % temp_activeRowSize;
                }
                else if (m_isChassisSelected && m_isMovementSelected)
                {
                    temp_activeColumn = Mathf.Clamp(++temp_activeColumn % (temp_activeRowSize - 1), CELL_PADDING_SIZE, temp_activeRowSize - (1 + CELL_PADDING_SIZE));
                }
            }
            // Input Left
            else if (temp_horizontalInput < 0)
            {
                /*
                 * Cases:
                 *  If chassis isn't selected or if player is in the chassis slot row, we want to loop from 0 to row count.
                 *  If in any other row, loop from 1 to row count - 1.
                 */
                if (!m_isChassisSelected || !m_isMovementSelected)
                {
                    if (temp_activeColumn == 0)
                    {
                        temp_activeColumn = temp_activeRowSize - 1;
                    }
                    else
                    {
                        temp_activeColumn = Mathf.Clamp(--temp_activeColumn, 0, temp_activeRowSize - 1);
                    }
                }
                else if (m_isChassisSelected && m_isMovementSelected)
                {
                    if (temp_activeColumn == CELL_PADDING_SIZE)
                    {
                        temp_activeColumn = temp_activeRowSize - (1 + CELL_PADDING_SIZE);
                    }
                    else
                    {
                        temp_activeColumn = Mathf.Clamp(--temp_activeColumn, CELL_PADDING_SIZE, temp_activeRowSize - (1 + CELL_PADDING_SIZE));
                    }
                }
            }

            CustomDebug.Log($"{this.name}: Active column: {temp_activeColumn}", IS_DEBUGGING);
            if (playerIndex == 0)
            {
                m_playerOneActiveColumn = temp_activeColumn;
            }
            else if (playerIndex == 1)
            {
                m_playerTwoActiveColumn = temp_activeColumn;
            }
            if (m_isChassisSelected && m_isMovementSelected)
            {
                GetColumnNodeFromRowIndex(playerIndex, temp_activeRow).Value = temp_activeColumn;
            }
        }
        else if (m_isChassisSelected && m_isMovementSelected)
        {
            // Input Down
            if (temp_verticalInput < 0)
            {
                /*
                 * Cases:
                 *  Row movement is only allowed after a chassis is selected.
                 *  If at the bottom of the rows, do not move further.
                 *  The bottom row can only be accessed if it is the player's turn to fill a slot.
                 */
                if (temp_activeRow == 3) { return; }
                else
                {
                    temp_activeRow = ++temp_activeRow % 3;
                }
            }
            // Input Up
            if (temp_verticalInput > 0)
            {
                /*
                 * Cases:
                 *  Row movement is only allowed after a chassis is selected.
                 *  If at the top of the rows, do not move further.
                 *  The next row can only be accessed if it is the player's turn to fill a slot.
                 */
                if (temp_activeRow == 3) { return; }
                else
                {
                    temp_activeRow--;
                    if (temp_activeRow < 0)
                    {
                        temp_activeRow = 2;
                    }
                }
            }

            CustomDebug.Log($"{this.name}: Active row: {temp_activeRow}", IS_DEBUGGING);
            if (playerIndex == 0)
            {
                LinkedListNode<int> temp_columnNode = GetColumnNodeFromRowIndex(playerIndex, m_playerOneActiveRow);
                if (m_playerOneActiveRow != 3)
                {
                    temp_columnNode.Value = GetActiveColumn(playerIndex);
                }
                else
                {
                    temp_columnNode.Value = 0;
                }
                m_playerOneActiveColumn = GetColumnNodeFromRowIndex(playerIndex, temp_activeRow).Value;
                m_playerOneActiveRow = temp_activeRow;
            }
            else if (playerIndex == 1)
            {
                LinkedListNode<int> temp_columnNode = GetColumnNodeFromRowIndex(playerIndex, m_playerTwoActiveRow);
                if (m_playerTwoActiveRow != 3)
                {
                    temp_columnNode.Value = GetActiveColumn(playerIndex);
                }
                else
                {
                    temp_columnNode.Value = 0;
                }
                m_playerTwoActiveColumn = GetColumnNodeFromRowIndex(playerIndex, temp_activeRow).Value;
                m_playerTwoActiveRow = temp_activeRow;
            }
        }
    }

    private int GetActiveRowSize(int playerIndex, int rowIndex)
    {
        return m_selectionRow[playerIndex].GetActiveRowSize(rowIndex);
    }
    #endregion PrivateFunctions
    #endregion HelperMethods
}
