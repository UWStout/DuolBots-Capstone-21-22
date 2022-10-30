using System.Collections.Generic;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    public class ChosenPartsManager_PartSelect : SingletonMonoBehaviour<
        ChosenPartsManager_PartSelect>
    {
        // Constants
        private const bool IS_DEBUGGING = true;

        private BetterBuildSceneStateChangeHandler m_partHandler = null;
        private string m_chassisSelection = null;
        private string m_movementSelection = null;
        private Dictionary<byte, string> m_slottedParts
            = new Dictionary<byte, string>();

        public List<PartInSlot> slottedParts => ConvertSlottedPartsToList();

        // Foreign Initialization
        private void Start()
        {
            m_partHandler = BetterBuildSceneStateChangeHandler.CreateNew(
                StartPartHandler, EndPartHandler, eBetterBuildSceneState.Part);
        }


        public void SetChassis(string chassisID)
        {
            m_chassisSelection = chassisID;
        }
        public void SetMovementPart(string movementID)
        {
            m_movementSelection = movementID;
        }
        public void SetSlottedPart(byte slotIndex, string partID)
        {
            if (m_slottedParts.ContainsKey(slotIndex))
            {
                m_slottedParts.Remove(slotIndex);
            }
            m_slottedParts.Add(slotIndex, partID);
        }


        #region PartState
        private void StartPartHandler() { }
        private void EndPartHandler()
        {
            SendDataToBattle();
        }
        #endregion PartState

        private void SendDataToBattle()
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(SendDataToBattle), this,
                IS_DEBUGGING);
            #endregion Logs
            BuildSceneBotData.SetData(0, m_chassisSelection, m_movementSelection,
                ConvertSlottedPartsToList());
        }
        private List<PartInSlot> ConvertSlottedPartsToList()
        {
            List<PartInSlot> temp_partList = new List<PartInSlot>(
                m_slottedParts.Count);

            foreach (KeyValuePair<byte, string> temp_kvp in m_slottedParts)
            {
                PartInSlot temp_partInSlot = new PartInSlot(temp_kvp.Value,
                    temp_kvp.Key);
                temp_partList.Add(temp_partInSlot);
            }

            return temp_partList;
        }
    }
}

