using UnityEngine;

using NaughtyAttributes;
using TMPro;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Changes the text of the specified TMP to be the slot index
    /// when the player cycles between the slots.
    /// </summary>
    public class SlotIndexDisplay_PartSelect : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = true;

        [SerializeField] [Required]
        private Input_DollyTargetCycler m_targetCycler = null;
        [SerializeField] [Required] private TextMeshProUGUI m_textMesh = null;
        [SerializeField] [Required] private BlinkImage m_leftControl = null;
        [SerializeField] [Required] private BlinkImage m_rightControl = null;

        private BetterBuildSceneStateChangeHandler m_partHandler = null;
        private int m_prevIndex = 0;


        // Domestic Initialization
        private void Awake()
        {
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_targetCycler,
                nameof(m_targetCycler), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_textMesh,
                nameof(m_textMesh), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_leftControl,
                nameof(m_leftControl), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_rightControl,
                nameof(m_rightControl), this);
            #endregion Asserts
        }
        // Foreign Initialization
        private void Start()
        {
            m_partHandler = BetterBuildSceneStateChangeHandler.CreateNew(
                BeginPartStateHandler, EndPartStateHandler,
                eBetterBuildSceneState.Part);
        }
        private void OnDestroy()
        {
            m_partHandler.ToggleActive(false);
        }


        #region PartState
        private void BeginPartStateHandler()
        {
            m_targetCycler.onSelectionIndexChange += OnSelectionChange;
        }
        private void EndPartStateHandler()
        {
            if (m_targetCycler != null)
            {
                m_targetCycler.onSelectionIndexChange -= OnSelectionChange;
            }
        }
        #endregion PartState


        private void OnSelectionChange(int cycleIndex)
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(OnSelectionChange), this,
                IS_DEBUGGING);
            #endregion Logs
            m_textMesh.text = cycleIndex.ToString();

            int temp_rightIndex = m_targetCycler.dollyTargetCycler.
                targetValues.WrapIndex(m_prevIndex - 1);
            int temp_leftIndex = m_targetCycler.dollyTargetCycler.
                targetValues.WrapIndex(m_prevIndex + 1);
            // Went right
            if (temp_rightIndex == cycleIndex)
            {
                m_rightControl.Blink();
            }
            // Went left
            else if (temp_leftIndex == cycleIndex)
            {
                m_leftControl.Blink();
            }
            m_prevIndex = cycleIndex;
        }
    }
}
