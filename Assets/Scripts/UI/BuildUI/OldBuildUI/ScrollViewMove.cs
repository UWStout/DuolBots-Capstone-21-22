using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using static UnityEngine.InputSystem.InputAction;
// Original Author(s) - Eslis Vang

/// <summary>
/// Moves the content in the scrollviewer's viewport.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class ScrollViewMove : MonoBehaviour
{
    // Constants
    private const bool IS_DEBUGGING = true;

    // The row's index.
    [SerializeField] private int m_rowID = 0;
    // The player's index.
    [SerializeField] private int m_playerIndex = -1;
    // The size of a cell.
    [SerializeField] private float CELL_SIZE = 100f;

    [SerializeField] private float PADDING_SIZE = 0f;

    [SerializeField] private int CENTER_SPACING = 1;

    [SerializeField] private bool m_moveScrollView = true;

    // The RectTransform of the object.
    private RectTransform m_rectTransform = null;

    private PartSelectorManager m_partSelector = null;
    // The max offset for the content RectTransform.
    private float m_maxRowOffset = 0f;

    #region UnityMessages
    private void Awake()
    {
        m_rectTransform = this.GetComponent<RectTransform>();

        Assert.IsNotNull(m_rectTransform, $"Player {m_playerIndex + 1}'s {this.name} RectTransform is null.");

        this.GetComponent<GridLayoutGroup>().cellSize.Set(CELL_SIZE, CELL_SIZE);
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Get the width of the viewport.
        float temp_scrollViewHeight = this.transform.parent.parent.GetComponent<RectTransform>().rect.height;
        // Get the width of the viewport's contents.
        float temp_rowMaxHeight = (this.transform.childCount * (CELL_SIZE + PADDING_SIZE) + (2 * CENTER_SPACING * (CELL_SIZE + PADDING_SIZE)));
        // The max offset for the content.
        m_maxRowOffset = temp_rowMaxHeight - (CELL_SIZE + PADDING_SIZE) * (1 + CENTER_SPACING);
        //m_rectTransform.offsetMax = new Vector2(-m_maxRowOffset, m_rectTransform.offsetMax.y);

        m_rectTransform.sizeDelta = new Vector2(m_rectTransform.sizeDelta.x, m_maxRowOffset);

        CustomDebug.Log($"Row: {m_rowID} | Base Height: {temp_scrollViewHeight}", IS_DEBUGGING);
        CustomDebug.Log($"Row: {m_rowID} | Height: {m_maxRowOffset}", IS_DEBUGGING);    

        if (PartSelectorManager.instance != null)
        {
            m_partSelector = PartSelectorManager.instance;
        }

        foreach (Transform child in this.transform)
        {
            foreach (Transform childsChildren in child)
            {
                childsChildren.GetComponent<RectTransform>().sizeDelta = new Vector2(CELL_SIZE, CELL_SIZE);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
    #endregion UnityMessages


    #region HelperFunctions
    public void MoveScrollView(int rowIndex)
    {
        if (m_moveScrollView && m_partSelector != null && m_partSelector.isChassisSelected && m_partSelector.isMovementSelected)
        {
            int temp_activeRow = m_partSelector.GetActiveRow(m_playerIndex);

            CustomDebug.Log($"{this.name} | Row ID {m_rowID} | Current ID {temp_activeRow}", IS_DEBUGGING);

            if (m_rowID == rowIndex)
            {
                int temp_activeColumn = m_partSelector.GetActiveColumn(m_playerIndex);
                //CustomDebug.LogWarning($"Active Column: {temp_activeColumn}");
                m_rectTransform.localPosition = new Vector3(0f, (CELL_SIZE + PADDING_SIZE) * (temp_activeColumn - CENTER_SPACING), 0f);

                if (m_rectTransform.localPosition.y % (CELL_SIZE + PADDING_SIZE) != 0)
                {
                }
            }
        }
    }
    #endregion HelperFunctions
}
