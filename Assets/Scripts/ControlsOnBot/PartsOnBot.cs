using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace DuolBots
{
    /// <summary>
    /// Locates all of the parts and stores them for use
    /// </summary>
    public class PartsOnBot
    {
        // Consts
        private const bool IS_DEBUGGING = false;
        private const string BOT_ROOT_TAG = "Robot";

        private GameObject m_Chassis = null;
        private GameObject m_Wheels = null;
        private List<GameObject> m_Slots = null;

        public GameObject Chassis => m_Chassis;
        public GameObject Wheels => m_Wheels;
        public List<GameObject> Slots => m_Slots;
        

        //constructor that sorts the parts based on the enum type
        public PartsOnBot()
        {
            m_Slots = new List<GameObject>();
            GameObject[] parts = GameObject.FindGameObjectsWithTag("Part");

            foreach (GameObject go in parts)
            {
                switch (go.GetComponent<PartSOReference>().partScriptableObject.partType)
                {
                    case ePartType.Chassis:
                        m_Chassis = go;
                        break;
                    case ePartType.Movement:
                        m_Wheels = go;
                        break;
                    default:
                        m_Slots.Add(go);
                        break;
                }
            }
        }

        public PartsOnBot(byte team)
        {
            GameObject[] temp_bots = GameObject.FindGameObjectsWithTag(BOT_ROOT_TAG);
            Assert.AreNotEqual(0, temp_bots.Length, $"Could not find any " +
                $"parts with tag {BOT_ROOT_TAG} in the scene");
            CustomDebug.Log($"Found {temp_bots.Length} bots in the scene",
                IS_DEBUGGING);
            GameObject temp_myBot = null;
            for (int i = 0; i < temp_bots.Length; ++i)
            {
                GameObject temp_curBot = temp_bots[i];
                ITeamIndex temp_curBotTeamIndex = temp_curBot.GetComponent<ITeamIndex>();
                if (temp_curBotTeamIndex.teamIndex == team)
                {
                    temp_myBot = temp_curBot;
                    break;
                }
            }
            Assert.IsNotNull(temp_myBot, $"No bot with my team index {team} " +
                $"was found in the scene");

            m_Slots = new List<GameObject>();
            GameObject[] temp_parts = GameObject.FindGameObjectsWithTag("Part");
            CustomDebug.Log($"Found {temp_parts.Length} parts for team {team}",
                IS_DEBUGGING);
            Assert.AreNotEqual(0, temp_parts.Length, $"No parts were found in the " +
                $"scene for team {team}. At least 1 should be found (Wheels).");

            foreach (GameObject go in temp_parts)
            {
                CustomDebug.Log($"Checking if {go} part is for my team ",
                    IS_DEBUGGING);
                ITeamIndex temp_teamIndex = go.GetComponentInParent<ITeamIndex>();
                if ((temp_teamIndex != null) && (temp_teamIndex.teamIndex == team))
                {
                    CustomDebug.Log($"Found part {go.name} with my team index " +
                        $"of {temp_teamIndex.teamIndex} on " +
                        $"{temp_teamIndex.gameObject.name} with position " +
                        $"{temp_teamIndex.transform.position}", IS_DEBUGGING);
                    switch (go.GetComponent<PartSOReference>().partScriptableObject.partType)
                    {
                        case ePartType.Chassis:
                            m_Chassis = go;
                            break;
                        case ePartType.Movement:
                            m_Wheels = go;
                            break;
                        default:
                            m_Slots.Add(go);
                            break;
                    }
                }
                else
                {
                    CustomDebug.Log($"{go} part was not for my team ",
                        IS_DEBUGGING);
                }
            }
        }
        public List<GameObject> WheelsAndSlots()
        {
            List<GameObject> temp = new List<GameObject>();
            temp.Add(Wheels);
            temp.AddRange(Slots);
            return temp;
        }
    }
}
