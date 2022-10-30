using UnityEngine;
using System.Collections.Generic;
// Original Authors - Wyatt Senalik and Cole Woulf

namespace DuolBots
{
    /// <summary>
    /// Class to pass data from the build scene into the battle scene.
    /// </summary>
    public static class BuildSceneInputData
    {
        // Map from team index (byte) to a list of the custom input bindings
        private static Dictionary<byte, BuiltBotInputData> s_inputBindingsData =
            new Dictionary<byte, BuiltBotInputData>();


        /// <summary>
        /// The input bindings specified in the build scene for the specified team
        /// </summary>
        /// <param name="teamIndex">Which team to set the data for.</param>
        public static IReadOnlyList<CustomInputBinding> GetInputBindingsForPlayer(byte teamIndex)
        {
            BuiltBotInputData temp_botInpData = GetBuiltBotInputData(teamIndex);
            if (temp_botInpData == null)
            {
                return null;
            }
            return temp_botInpData.customInputBindings;
        }
        /// <summary>
        /// The bot input data specified in the build scene for the specified team
        /// </summary>
        /// <param name="teamIndex">Which team to set the data for.</param>
        public static BuiltBotInputData GetBuiltBotInputData(byte teamIndex)
        {
            if (!s_inputBindingsData.TryGetValue(teamIndex,
                out BuiltBotInputData temp_inputBindingsData))
            {
                Debug.LogError($"No data for team with index={teamIndex} has been specified");
                return null;
            }
            return temp_inputBindingsData;
        }
        /// <summary>
        /// Sets the data for the given team. Overwrites old data if it exists.
        /// </summary>
        /// <param name="teamIndex">Which team to set the data for.</param>
        /// <param name="inputBindingData">Input data to save for the team.</param>
        public static void SetData(byte teamIndex, List<CustomInputBinding> inputBindingData)
        {
            SetData(teamIndex, new BuiltBotInputData(inputBindingData));
        }
        /// <summary>
        /// Sets the data for the given team. Overwrites old data if it exists.
        /// </summary>
        /// <param name="teamIndex">Which team to set the data for.</param>
        /// <param name="botInputData">Input data to save for the team.</param>
        public static void SetData(byte teamIndex, BuiltBotInputData botInputData)
        {
            if (s_inputBindingsData.ContainsKey(teamIndex))
            {
                // Overwrite data
                s_inputBindingsData[teamIndex] = botInputData;
            }
            else
            {
                // Add new data
                s_inputBindingsData.Add(teamIndex, botInputData);
            }
        }
        /// <summary>
        /// Gets the amount of BuiltBotInputData stored total.
        /// Amount of teams that have BuiltBotInputData.
        /// </summary>
        public static int GetAmountDataStored()
        {
            return s_inputBindingsData.Count;
        }
    }
}
