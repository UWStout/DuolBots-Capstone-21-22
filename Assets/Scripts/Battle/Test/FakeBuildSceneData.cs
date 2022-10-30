using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots.Test
{
    /// <summary>
    /// Puts some fake data needed from the build
    /// scene into the BuildSceneInputData.
    /// </summary>
    [RequireComponent(typeof(TeamIndex))]
    public class FakeBuildSceneData : MonoBehaviour
    {
        [SerializeField] private FakeBuildSceneBotData m_buildData =
            new FakeBuildSceneBotData();
        [SerializeField] private List<CustomInputBindingWithStringID>
            m_inputBindingData =
            new List<CustomInputBindingWithStringID>();

        private ITeamIndex m_teamIndex = null;


        private void Awake()
        {
            m_teamIndex = GetComponent<ITeamIndex>();
            Assert.IsNotNull(m_teamIndex,
                $"{nameof(FakeBuildSceneBotData)} requires a " +
                $"{nameof(ITeamIndex)} attached to it, but " +
                $"there was none.");

            SetBotData();
            SetInputData();
        }


        private void SetBotData()
        {
            BuiltBotData temp_botData = new BuiltBotData(
                m_buildData.chassisPartID, m_buildData.movementPartID,
                m_buildData.slottedPartIDList);
            BuildSceneBotData.SetData(m_teamIndex.teamIndex, temp_botData);
        }
        private void SetInputData()
        {
            List<CustomInputBinding> temp_customInputBindings
                = new List<CustomInputBinding>();
            foreach (CustomInputBindingWithStringID temp_binding
                in m_inputBindingData)
            {
                temp_customInputBindings.Add(
                    temp_binding.ConvertToCustomInputBinding());
            }

            BuildSceneInputData.SetData(m_teamIndex.teamIndex,
                temp_customInputBindings);        
        }
    }

    [Serializable]
    class CustomInputBindingWithStringID
    {
        /// <summary>
        /// Which player is controlling the part.
        /// True - Player 1. False - Player 2.
        /// </summary>
        public byte playerIndex => m_playerIndex;
        [SerializeField] private byte m_playerIndex = 0;
        /// <summary>
        /// Which action on the part the input is binded to.
        /// </summary>
        public byte actionIndex => m_actionIndex;
        [SerializeField] private byte m_actionIndex = 0;
        /// <summary>
        /// The input that is binded to cause the action.
        /// </summary>
        public eInputType inputType => m_inputType;
        [SerializeField] private eInputType m_inputType =
            eInputType.buttonEast;
        /// <summary>
        /// Instance identifier for the part based on slot index.
        /// </summary>
        public byte partSlotID => m_partSlotID;
        [SerializeField] private byte m_partSlotID = 0;
        /// <summary>
        /// Instance identifier for the part based on slot index.
        /// </summary>
        public string partUniqueID => m_partUniqueID.value;
        [SerializeField] private StringID m_partUniqueID = null;


        public CustomInputBinding ConvertToCustomInputBinding()
        {
            return new CustomInputBinding(m_playerIndex, m_actionIndex,
                m_inputType, m_partSlotID, m_partUniqueID.value);
        }
    }


    /// <summary>
    /// Serialized data for which parts we should instantiate for testing.
    /// </summary>
    [Serializable]
    class FakeBuildSceneBotData
    {
        // Which parts where selected as the chassis and movement
        // are handled separately because
        // there must be one and only one of each selected.
        [SerializeField] private StringID m_chassisPartID = null;
        [SerializeField] private StringID m_movementPartID = null;
        // There can be variable amount of weapon and utility
        // parts selected, so we hold those in a list.
        [SerializeField] private List<FakePartInSlot> m_slottedPartIDList
            = new List<FakePartInSlot>();

        public string chassisPartID => m_chassisPartID.value;
        public string movementPartID => m_movementPartID.value;
        public List<PartInSlot> slottedPartIDList
        {
            get
            {
                List<PartInSlot> temp_createdParts =
                    new List<PartInSlot>(m_slottedPartIDList.Count);
                foreach (FakePartInSlot temp_fakePart
                    in m_slottedPartIDList)
                {
                    temp_createdParts.Add(
                        temp_fakePart.ConvertToPartInSlot());
                }
                return temp_createdParts;
            }
        }
    }

    /// <summary>
    /// Serialized version of a PartInSlot for test data.
    /// Class to tie a part to which slot it was placed in.
    /// </summary>
    [Serializable]
    class FakePartInSlot
    {
        [SerializeField] private StringID m_partID = null;
        [SerializeField] private byte m_slotNum = byte.MaxValue;


        public PartInSlot ConvertToPartInSlot()
        {
            return new PartInSlot(m_partID.value, m_slotNum);
        }
    }
}
