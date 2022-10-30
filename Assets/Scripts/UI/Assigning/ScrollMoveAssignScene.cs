using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ScrollMoveAssignScene : MonoBehaviour
{
    // Constants
    private const bool IS_DEBUGGING = false;

    // The row's index.
    [SerializeField] private int m_rowID = 0;
    // The size of a cell.
    [SerializeField] private float CELL_SIZE = 100f;

    [SerializeField] private int CENTER_SPACING = 1;

    // The RectTransform of the object.
    private RectTransform m_rectTransform = null;

    // If the row is active.
    private bool m_scrollBarIsActive = false;
    // The max offset for the content RectTransform.
    private float m_maxRowOffset = 0f;

    private ControllerMovement m_controllerMovement;

    #region UnityMessages
    private void Awake()
    {
        m_rectTransform = this.GetComponent<RectTransform>();
        m_controllerMovement = GameObject.Find("SceneManager").GetComponent<ControllerMovement>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Get the width of the viewport.
        float temp_scrollViewHeight = this.transform.parent.parent.GetComponent<RectTransform>().rect.height;
        // Get the width of the viewport's contents.
        float temp_rowMaxHeight = (this.transform.childCount * CELL_SIZE + (2 * CENTER_SPACING * CELL_SIZE));
        // The max offset for the content.
        m_maxRowOffset = temp_rowMaxHeight - CELL_SIZE * (1 + CENTER_SPACING);
        //m_rectTransform.offsetMax = new Vector2(-m_maxRowOffset, m_rectTransform.offsetMax.y);

        m_rectTransform.sizeDelta = new Vector2(m_rectTransform.sizeDelta.x, m_maxRowOffset);

        CustomDebug.Log($"Row: {m_rowID} | Base Height: {temp_scrollViewHeight}", IS_DEBUGGING);
        CustomDebug.Log($"Row: {m_rowID} | Height: {m_maxRowOffset}", IS_DEBUGGING);

    }

    // Update is called once per frame
    private void Update()
    {
        int temp_activeColumn = m_controllerMovement.activeRow;
        float temp_viewportOffset = (temp_activeColumn - 1) * CELL_SIZE;
        CustomDebug.LogWarning($"Active Column: {temp_activeColumn}");

        m_rectTransform.localPosition = new Vector3(0f, CELL_SIZE * (temp_activeColumn), 0f);
    }
    #endregion UnityMessages
}
