using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Listens to the specified <see cref="DollyTargetCycler"/> and
    /// moves the camera's lookAt transform to look at the slot
    /// that the target cycler is focused on.
    /// </summary>
    public class SlotViewer : MonoBehaviour
    {
        private const bool IS_DEBUGGING = true;

        [SerializeField] [Required] private PlayerIndex m_playerIndex = null;
        [SerializeField] [Required] private DollyTargetCycler m_cycler = null;
        [SerializeField] [Required] private Transform m_lookAtTrans = null;
        [SerializeField] [Min(0.0f)] private float m_viewOffset = 3.0f;

        private GameObject m_botRoot = null;

        private SlotPlacementManager m_slotPlacementManager = null;
        private SlotViewPlacement m_slotViewPlacement = null;

        public byte playerIndex => m_playerIndex.playerIndex;
        public DollyTargetCycler cycler => m_cycler;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            Assert.IsNotNull(m_lookAtTrans, $"{GetType().Name}'s {name} " +
                $"requires that {nameof(m_lookAtTrans)} be specified.");

            // Starts inactive by default
            ToggleActive(false);
        }
        // Subscribe
        private void OnEnable()
        {
            cycler.onSelectionIndexChange += OnSelectionIndexChange;
        }
        // Unsubscribe
        private void OnDisable()
        {
            if (cycler != null)
            {
                cycler.onSelectionIndexChange -= OnSelectionIndexChange;
            }
        }


        /// <summary>
        /// Sets the bot root to view the slots for.
        /// Must be called before ToggleActive(true) is called.
        /// </summary>
        /// <param name="botRoot"></param>
        public void SetBotRoot(GameObject botRoot)
        {
            m_botRoot = botRoot;
            m_slotPlacementManager = m_botRoot.GetComponentInChildren
                <SlotPlacementManager>();
            m_slotViewPlacement = m_botRoot.GetComponentInChildren
                <SlotViewPlacement>();

            #region Asserts
            CustomDebug.AssertComponentOnOtherIsNotNull(m_slotPlacementManager,
                m_botRoot, this);
            CustomDebug.AssertComponentOnOtherIsNotNull(m_slotViewPlacement,
                m_botRoot, this);
            #endregion Asserts

            // Set the target cycler's target values
            cycler.targetValues = m_slotViewPlacement.targetValues;
            // Set the target cycler to the slot view placement's dolly path.
            cycler.dollyPath = m_slotViewPlacement.GetDollyPathForPlayer(
                m_playerIndex.playerIndex);
        }
        public void ToggleActive(bool cond)
        {
            cycler.gameObject.SetActive(cond);
        }


        /// <summary>
        /// Called when the player goes to the next
        /// spot on the dolly.
        /// </summary>
        /// <param name="newIndex">New index that the player has navigated
        /// to using the dolly cycler.</param>
        private void OnSelectionIndexChange(int newIndex)
        {
            ViewSlot(newIndex);
        }
        /// <summary>
        /// Has the camera's lookAt transform move to look at the
        /// part in the slot with the given index.
        /// </summary>
        private void ViewSlot(int viewIndex)
        {
            ConfirmBotRootHasBeenSet();

            int temp_maxSlots = m_slotPlacementManager.GetSlotAmount();
            #region Asserts
            CustomDebug.AssertIndexIsInRange(viewIndex,
                m_slotViewPlacement.slotOrder, this);
            #endregion Asserts
            int temp_indexOfSlotToView = m_slotViewPlacement.
                GetSlotIndexFromViewIndex(viewIndex);
            #region Asserts
            CustomDebug.AssertIndexIsInRange(temp_indexOfSlotToView, 0,
                temp_maxSlots, this);
            #endregion Asserts
            Transform temp_transToView = m_slotPlacementManager.
                GetSlotTransform(temp_indexOfSlotToView);

            CustomDebug.Log($"Viewing {temp_transToView.name} with index " +
                $"{viewIndex}", IS_DEBUGGING);
            SetCameraToFocusOnSlotTransform(temp_transToView);
        }
        private void ConfirmBotRootHasBeenSet()
        {
            Assert.IsNotNull(m_botRoot, $"No bot root has been set yet");
        }
        /// <summary>
        /// Has the camera's lookAt transform move to look at the
        /// specified transform.
        /// </summary>
        private void SetCameraToFocusOnSlotTransform(Transform slotTransform)
        {
            Vector3 temp_offset = slotTransform.forward * m_viewOffset;
            m_lookAtTrans.position = slotTransform.position + temp_offset;
        }
    }
}
