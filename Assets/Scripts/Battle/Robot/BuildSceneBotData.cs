using System;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Data holder for the build scene to send data over to the battle scene.
    /// </summary>
    public static class BuildSceneBotData
    {
        // Data for built bots for each team that has built
        private static Dictionary<byte, BuiltBotData> s_botDataPerTeam = new Dictionary<byte, BuiltBotData>();


        /// <summary>
        /// Sets data for a built bot for the specified team.
        /// </summary>
        /// <param name="teamIndex">Team the build data corresponds to.</param>
        /// <param name="botData">Data for the built bot (Chassis's PartID, Movement Part's PartID,
        /// List of the Weapon/Utility Part's PartID and which slot they are in).</param>
        public static void SetData(byte teamIndex, BuiltBotData botData)
        {
            if (s_botDataPerTeam.ContainsKey(teamIndex))
            {
                s_botDataPerTeam[teamIndex] = botData;
            }
            else
            {
                s_botDataPerTeam.Add(teamIndex, botData);
            }
        }
        /// <summary>
        /// Sets data for a built bot for the specified team.
        /// </summary>
        /// <param name="teamIndex">Team the build data corresponds to.</param>
        /// <param name="chassisID">Chassis's PartID.</param>
        /// <param name="movementID">Movement Part's PartID.</param>
        /// <param name="slottedIDList">List of the Weapon/Utility Part's PartID and which slot they are in.</param>
        public static void SetData(byte teamIndex, string chassisID,
            string movementID, List<PartInSlot> slottedIDList)
        {
            SetData(teamIndex, new BuiltBotData(chassisID, movementID, slottedIDList));
        }
        /// <summary>
        /// Sets data for a built bot for the specified team.
        /// </summary>
        /// <param name="dataWithIndex">Bot data with the team index.</param>
        public static void SetData(BuiltBotDataWithTeamIndex dataWithIndex)
        {
            byte temp_teamIndex = dataWithIndex.teamIndex;
            BuiltBotData temp_botData = dataWithIndex.botData;
            SetData(temp_teamIndex, temp_botData);
        }
        /// <summary>
        /// Returns the BuiltBotData for the specified team.
        ///
        /// Pre Conditions - Data for the given teamIndex must be set.
        /// Post Conditions - Returns the BuiltBotData associated with the specified team.
        /// </summary>
        /// <param name="teamIndex">Team identifier to get the built bot data for.</param>
        public static BuiltBotData GetBotData(byte teamIndex)
        {
            if (!s_botDataPerTeam.TryGetValue(teamIndex, out BuiltBotData temp_botData))
            {
                Debug.LogError($"No BotData was provided for teamIndex={teamIndex}");
                return null;
            }
            return temp_botData;
        }
        /// <summary>
        /// Returns the BuiltBotData (chassisID, movementPartID, slottedPartIDList)
        /// for the specified team.
        ///
        /// Pre Conditions - Data for the given teamIndex must be set.
        /// Post Conditions - Returns the BuiltBotData associated with the specified team.
        /// </summary>
        /// <param name="teamIndex">Team identifier to get the built bot data for.</param>
        /// <param name="chassisID">out param - Chassis's PartID.</param>
        /// <param name="movementPartID">out param - MovementPart's PartID.</param>
        /// <param name="slottedPartIDList">out param - List of PartInSlot (Weapon/Utility part's
        /// PartID and which slot it should be in).</param>
        public static void GetBotData(byte teamIndex, out string chassisID,
            out string movementPartID, out IReadOnlyList<PartInSlot> slottedPartIDList)
        {
            BuiltBotData temp_botData = GetBotData(teamIndex);
            chassisID = temp_botData.chassisID;
            movementPartID = temp_botData.movementPartID;
            slottedPartIDList = temp_botData.slottedPartIDList;
        }
        /// <summary>
        /// Gets the amount of BotData that has been specified total.
        /// Amount of teams who have BotData.
        /// </summary>
        public static int GetAmountBotDataStored()
        {
            return s_botDataPerTeam.Count;
        }
        /// <summary>
        /// Gets all the data stored for built bots and what team
        /// they belong to.
        /// </summary>
        /// <returns></returns>
        public static BuiltBotDataWithTeamIndex[] GetAllData()
        {
            List<BuiltBotDataWithTeamIndex> temp_allData =
                new List<BuiltBotDataWithTeamIndex>(s_botDataPerTeam.Count);
            foreach (KeyValuePair<byte, BuiltBotData> temp_kvp in s_botDataPerTeam)
            {
                byte temp_teamIndex = temp_kvp.Key;
                BuiltBotData temp_botData = temp_kvp.Value;
                BuiltBotDataWithTeamIndex temp_dataWithIndex =
                    new BuiltBotDataWithTeamIndex(temp_botData, temp_teamIndex);
                temp_allData.Add(temp_dataWithIndex);
            }
            return temp_allData.ToArray();
        }
    }


    /// <summary>
    /// Data for the built bot
    /// </summary>
    public class BuiltBotData
    {
        public string chassisID => m_chassisID;
        private string m_chassisID = "";
        public string movementPartID => m_movementPartID;
        private string m_movementPartID = "";
        public IReadOnlyList<PartInSlot> slottedPartIDList => m_slottedPartIDList;
        private List<PartInSlot> m_slottedPartIDList = new List<PartInSlot>();


        public BuiltBotData(string chassisPartStringID, string movementPartStringID,
            List<PartInSlot> listOfSlottedPartIDs)
        {
            m_chassisID = chassisPartStringID;
            m_movementPartID = movementPartStringID;
            m_slottedPartIDList = listOfSlottedPartIDs;
        }
    }

    /// <summary>
    /// Class to tie a part to which slot it was placed in.
    /// </summary>
    [Serializable]
    public class PartInSlot
    {
        private string m_partID = "";
        private byte m_slotNum = byte.MaxValue;

        public string partID => m_partID;
        public byte slotIndex => m_slotNum;


        public PartInSlot(string id, byte slot)
        {
            m_partID = id;
            m_slotNum = slot;
        }
    }

    /// <summary>
    /// Data for the built bot and the team index.
    /// </summary>
    public class BuiltBotDataWithTeamIndex
    {
        public BuiltBotData botData => m_botData;
        private BuiltBotData m_botData = null;
        public byte teamIndex => m_teamIndex;
        private byte m_teamIndex = byte.MaxValue;


        public BuiltBotDataWithTeamIndex()
        {
            m_botData = null;
            m_teamIndex = byte.MaxValue;
        }
        public BuiltBotDataWithTeamIndex(BuiltBotData data, byte index)
        {
            m_botData = data;
            m_teamIndex = index;
        }
    }
}
