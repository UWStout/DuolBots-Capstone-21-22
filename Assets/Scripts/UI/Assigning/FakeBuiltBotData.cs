using DuolBots;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Original Authors - Cole Woulf
namespace DuolBots.Test
{
    public class FakeBuiltBotData : MonoBehaviour
    {
        //causing stack overflow error
        [SerializeField] private FakeBuiltData m_botData  = new FakeBuiltData();

        //[SerializeField] private ScriptableObject s_chassisPartID;
        //[SerializeField] private ScriptableObject s_movementPartID;
        // There can be variable amount of weapon and utility parts selected, so we hold those in a list.
        private List<PartInSlot> m_slottedPartIDList = new List<PartInSlot>();

        // TODO fix?
        private byte m_teamIndex = 0;


        // Start is called before the first frame update
        private void Awake()
        {
            SetBuildBotData();
        }

        private void SetBuildBotData()
        {
            BuildSceneBotData.SetData(m_teamIndex,
                m_botData.chassisPartID, m_botData.movementPartID, m_botData.slottedPartIDList);
        }

    }


    /// <summary>
    /// Serialized data for which parts we should instantiate for testing.
    /// </summary>
    [Serializable]
    class FakeBuiltData
    {
        // Which parts where selected as the chassis and movement are handled separately because
        // there must be one and only one of each selected.
        [SerializeField] private StringID m_chassisPartID = null;
        [SerializeField] private StringID m_movementPartID = null;
        // There can be variable amount of weapon and utility parts selected, so we hold those in a list.
        [SerializeField] private List<FakePartInSlot> m_slottedPartIDList = new List<FakePartInSlot>();

        public string chassisPartID => m_chassisPartID.value;
        public string movementPartID => m_movementPartID.value;
        public List<PartInSlot> slottedPartIDList
        {
            get
            {
                List<PartInSlot> temp_createdParts = new List<PartInSlot>(m_slottedPartIDList.Count);
                foreach (FakePartInSlot temp_fakePart in m_slottedPartIDList)
                {
                    temp_createdParts.Add(temp_fakePart.ConvertToPartInSlot());
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
    class FakePartInSlot2
    {
        [SerializeField] private StringID m_partID = null;
        [SerializeField] private byte m_slotNum = byte.MaxValue;


        public PartInSlot ConvertToPartInSlot()
        {
            return new PartInSlot(m_partID.value, m_slotNum);
        }
    }
}
