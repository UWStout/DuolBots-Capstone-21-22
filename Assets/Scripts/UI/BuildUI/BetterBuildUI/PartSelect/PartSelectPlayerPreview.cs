using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    [RequireComponent(typeof(PartSelectPlayerSelection))]
    public class PartSelectPlayerPreview : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = true;

        [SerializeField][Tag] private string m_robotTag = "Robot";
        [SerializeField]
        [Required]
        private Input_DollyTargetCycler m_dollyTargetCycInp = null;
        [SerializeField] private GameObject[] m_nameDisplays = null;

        private ChosenPartsManager_PartSelect m_chosenPartsMan = null;
        private PartSelectPlayerSelection m_playerSel = null;
        private PlayerIndex m_playerIndex = null;
        private SlotPlacementManager m_slotPlaceMan = null;
        private SlotViewPlacement m_slotViewPlacement = null;
        private PartPreview m_prevSelPartPreview = null;

        private BetterBuildSceneStateChangeHandler m_partHandler = null;


        // Domestic Initialization
        private void Awake()
        {
            m_playerSel = GetComponent<PartSelectPlayerSelection>();
            m_playerIndex = GetComponentInParent<PlayerIndex>();
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_dollyTargetCycInp,
                nameof(m_dollyTargetCycInp), this);
            CustomDebug.AssertComponentIsNotNull(m_playerSel, this);
            CustomDebug.AssertComponentInParentIsNotNull(m_playerIndex, this);
            #endregion Asserts
        }
        // Foreign Initialization
        private void Start()
        {
            m_chosenPartsMan = ChosenPartsManager_PartSelect.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_chosenPartsMan,
                this);
            #endregion Asserts

            m_partHandler = BetterBuildSceneStateChangeHandler.CreateNew(
                BeginPartHandler, EndPartHandler, eBetterBuildSceneState.Part);
        }
        // Subscribe
        private void OnEnable()
        {
            m_playerSel.onSelectedIndexChanged += UpdatePreviewAfterPartSwap;
        }
        // Unsubscribe
        private void OnDisable()
        {
            if (m_playerSel != null)
            {
                m_playerSel.onSelectedIndexChanged -= UpdatePreviewAfterPartSwap;
            }
        }
        private void OnDestroy()
        {
            m_partHandler.ToggleActive(false);
        }


        public void AttachCurrentPart()
        {
            if (TryToGetExistingAttachedPart(out bool temp_isCurSelPart,
                out PartSelectAttachedPart temp_existingAttachedPart))
            {
                // No need to attach the current part if its already attached
                if (temp_isCurSelPart) { return; }

                // Need to delete the existing, so we can add ours
                Destroy(temp_existingAttachedPart.gameObject);
            }

            // Current part is not attached, so we will need to add it.
            CreateNewAttachedPart();

            // Save the slotted part selection
            PartScriptableObject temp_selPartSO =
                m_playerSel.GetCurrentlySelectedPartSO();
            byte temp_slotIndex = GetCurrentlySelectedSlotIndex();
            m_chosenPartsMan.SetSlottedPart(temp_slotIndex, temp_selPartSO.partID);

            UpdatePreview();
        }
        /// <summary>
        /// Gets the slot index being selected currently.
        /// </summary>
        public byte GetCurrentlySelectedSlotIndex()
        {
            int temp_viewIndex = m_dollyTargetCycInp.dollyTargetCycler.
                currentSelectedIndex;
            return (byte)m_slotViewPlacement.
                GetSlotIndexFromViewIndex(temp_viewIndex);
        }


        #region PartStateHandlers
        private void BeginPartHandler()
        {
            // Get the slot placement manager
            GameObject temp_robot = GameObject.FindWithTag(m_robotTag);
            #region Asserts
            CustomDebug.AssertIsTrueForComponent(temp_robot != null,
                $"to find a {nameof(GameObject)} with the tag {m_robotTag}", this);
            #endregion Asserts
            #region Logs
            CustomDebug.LogForComponent($"Found robot " +
                $"{temp_robot.transform.parent}", this, IS_DEBUGGING);
            #endregion Logs
            m_slotPlaceMan = temp_robot.
                GetComponentInChildren<SlotPlacementManager>();
            m_slotViewPlacement = temp_robot.
                GetComponentInChildren<SlotViewPlacement>();
            #region Asserts
            CustomDebug.AssertComponentInChildrenOnOtherIsNotNull(m_slotPlaceMan,
                temp_robot, this);
            CustomDebug.AssertComponentInChildrenOnOtherIsNotNull(
                m_slotViewPlacement, temp_robot, this);
            #endregion Asserts

            m_dollyTargetCycInp.dollyTargetCycler.onSelectionIndexChange
                += UpdatePreviewAfterSlotSwap;

            // Update right away as well
            UpdatePreview();
        }
        private void EndPartHandler()
        {
            if (m_dollyTargetCycInp != null &&
                m_dollyTargetCycInp.dollyTargetCycler != null)
            {
                m_dollyTargetCycInp.dollyTargetCycler.onSelectionIndexChange
                    -= UpdatePreviewAfterSlotSwap;
            }
        }
        #endregion PartStateHandlers

        #region Preview
        private void UpdatePreviewAfterPartSwap(int partIndex)
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(UpdatePreviewAfterPartSwap),
                this, IS_DEBUGGING);
            #endregion Logs
            UpdatePreview();
        }
        private void UpdatePreviewAfterSlotSwap(int slotIndex)
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(UpdatePreviewAfterSlotSwap),
                this, IS_DEBUGGING);
            #endregion Logs
            UpdatePreview();
        }
        private void UpdatePreview()
        {
            byte temp_playerIndex = m_playerIndex.playerIndex;
            // Turn off old preview
            DeactivatePartPreview();

            // Don't turn on the preview if there the part we are trying to preview
            // is already attached.
            if (TryToGetExistingAttachedPart(out bool temp_isCurSelPart, out _))
            {
                if (temp_isCurSelPart) { return; }
            }

            // Turn on existing preview (one other player is viewing)
            // for current part if there is one
            if (TryToGetExistingPartPreview(out PartPreview temp_existingPreview))
            {
                // The existing preview is the one the other player is also viewing,
                // so just toggle it active for this player as well.
                #region Logs
                CustomDebug.LogForComponent($"Turning on " +
                    $"{temp_existingPreview.name} for player " +
                    $"{temp_playerIndex}", this, IS_DEBUGGING);
                #endregion Logs
                ActivatePartPreview(temp_existingPreview);
                return;
            }
            // If we reach here, either the other player was not viewing this slot
            // or the part the other player is viewing in this slot is not for the
            // part we are trying to preview.
            // Either way, we need to instantiate a new part preview.
            PartPreview temp_newPartPrev = CreateNewPartPreview();
            #region Logs
            CustomDebug.LogForComponent($"Turning on " +
                $"{temp_newPartPrev.name} for player " +
                $"{temp_playerIndex}", this, IS_DEBUGGING);
            #endregion Logs
            // Activate the preview
            ActivatePartPreview(temp_newPartPrev);
        }
        /// <summary>
        /// Tries to pull an existing <see cref="PartPreview"/> off of the currently
        /// selected slot children. If the found part preview is for the part we
        /// currently have selected, returns true.
        ///
        /// If there is no existing <see cref="PartPreview"/> or if there is but
        /// it is not for the part that is currently selected, returns false.
        /// </summary>
        /// <param name="partPreview">Existing <see cref="PartPreview"/></param>
        private bool TryToGetExistingPartPreview(out PartPreview partPreview)
        {
            // Get which part we are trying to enable
            PartScriptableObject temp_selPartSO =
                m_playerSel.GetCurrentlySelectedPartSO();
            // Slot the part is already or will be attached to
            Transform temp_slotTrans = GetCurrentlySelectedSlotTransform();
            // Try to get the preview for the other player's part, since they
            // might be the same part.
            partPreview = temp_slotTrans.GetComponentInChildren<PartPreview>();

            // If there is no part preview, there is no existing part preview
            if (partPreview == null) { return false; }

            // Pull off the part SO reference
            PartSOReference temp_existingPartSORef = partPreview.
                    GetComponent<PartSOReference>();
            #region Asserts
            CustomDebug.AssertComponentOnOtherIsNotNull(temp_existingPartSORef,
                partPreview.gameObject, this);
            #endregion Asserts
            // If the preview is for the part we currently have selected
            return temp_existingPartSORef.partScriptableObject.partID ==
                    temp_selPartSO.partID;
        }
        /// <summary>
        /// Creates a new part preview at the currently selected slot
        /// for the part that is currently selected.
        /// </summary>
        /// <returns>PartPreview on the created GameObject.</returns>
        private PartPreview CreateNewPartPreview()
        {
            PartScriptableObject temp_selPartSO =
                m_playerSel.GetCurrentlySelectedPartSO();
            Transform temp_slotTrans = GetCurrentlySelectedSlotTransform();
            GameObject temp_createdPartPrevObject = Instantiate(
                temp_selPartSO.buildUIPreviewPrefab, temp_slotTrans);
            PartPreview temp_newPartPrev =
                temp_createdPartPrevObject.GetComponent<PartPreview>();
            #region Asserts
            CustomDebug.AssertComponentOnOtherIsNotNull(temp_newPartPrev,
                temp_createdPartPrevObject, this);
            #endregion Asserts
            return temp_newPartPrev;
        }
        /// <summary>
        /// Activates the given part preview and sets it as the
        /// previous selected part preview.
        /// </summary>
        /// <param name="partPreview">Preview to activate.</param>
        private void ActivatePartPreview(PartPreview partPreview)
        {
            partPreview.ToggleActive(m_playerIndex.playerIndex, true);
            m_prevSelPartPreview = partPreview;
        }
        /// <summary>
        /// Deactivates the previous part preview if there is one.
        /// </summary>
        private void DeactivatePartPreview()
        {
            // If there was no previous (if its the first one)
            if (m_prevSelPartPreview == null) { return; }
            #region Logs
            CustomDebug.LogForComponent($"Turning off " +
                $"{m_prevSelPartPreview.name} for player " +
                $"{m_playerIndex.playerIndex}", this, IS_DEBUGGING);
            #endregion Logs
            m_prevSelPartPreview.ToggleActive(m_playerIndex.playerIndex, false);
            m_prevSelPartPreview = null;
        }
        #endregion Preview

        #region AttachPart
        /// <summary>
        /// Tries to find an existing <see cref="PartSelectAttachedPart"/> attached
        /// to the currently attached slot.
        /// </summary>
        /// <param name="isCurSelPart">True if that attached part is the part that
        /// is currently being selected.</param>
        /// <param name="existingAttachedPart">The currently attached part.</param>
        /// <returns>True if there is an existing attached part attached to the
        /// current slot.</returns>
        private bool TryToGetExistingAttachedPart(out bool isCurSelPart,
            out PartSelectAttachedPart existingAttachedPart)
        {
            isCurSelPart = false;

            // Get which part we are trying to attach
            PartScriptableObject temp_selPartSO =
                m_playerSel.GetCurrentlySelectedPartSO();
            // Slot the part will be attached to
            Transform temp_slotTrans = GetCurrentlySelectedSlotTransform();

            // Check if there is already a part attached to the current slot
            existingAttachedPart = temp_slotTrans.
                GetComponentInChildren<PartSelectAttachedPart>();
            // No attached part.
            if (existingAttachedPart == null) { return false; }

            // There is an attached part, now check if it is the for the
            // part we are trying to attach.
            PartSOReference temp_soRef = existingAttachedPart.
                GetComponent<PartSOReference>();
            #region Asserts
            CustomDebug.AssertComponentOnOtherIsNotNull(temp_soRef,
                existingAttachedPart.gameObject, this);
            #endregion Asserts
            // Not the one we are trying to attach, but there is one attached
            if (temp_soRef.partScriptableObject.partID != temp_selPartSO.partID)
            { return true; }

            // Part we are trying to attach already is attached and ours
            isCurSelPart = true;
            return true;
        }
        /// <summary>
        /// Attaches a newly instantiated part to the currently selected slot.
        /// </summary>
        /// <returns><see cref="PartSelectAttachedPart"/> attached to the newly
        /// instantiated part.</returns>
        private PartSelectAttachedPart CreateNewAttachedPart()
        {
            // Get which part we are trying to attach
            PartScriptableObject temp_selPartSO =
                m_playerSel.GetCurrentlySelectedPartSO();
            // Slot the part will be attached to
            Transform temp_slotTrans = GetCurrentlySelectedSlotTransform();

            GameObject temp_createdPartObject = Instantiate(
                temp_selPartSO.buildUIPrefab, temp_slotTrans);
            PartSelectAttachedPart temp_attachedPart = temp_createdPartObject.
                GetComponent<PartSelectAttachedPart>();
            #region Asserts
            CustomDebug.AssertComponentOnOtherIsNotNull(temp_attachedPart,
                temp_createdPartObject, this);
            #endregion Asserts
            return temp_attachedPart;
        }
        #endregion AttachPart

        /// <summary>
        /// Gets the Transform of the currently selected slot.
        /// </summary>
        private Transform GetCurrentlySelectedSlotTransform()
        {
            int temp_slotIndex = GetCurrentlySelectedSlotIndex();
            return m_slotPlaceMan.GetSlotTransform(temp_slotIndex);
        }
    }
}
