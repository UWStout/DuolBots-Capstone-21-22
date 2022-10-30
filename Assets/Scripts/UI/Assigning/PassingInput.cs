using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

// Original Authors - Cole Woulf and Ben Lussman
// Recently edited by Wyatt Senalik
namespace DuolBots
{
    /// <summary>
    /// Stores the inputs from the dropdown of inputs
    /// </summary>
    public class PassingInput : MonoBehaviour
    {
        private List<GameObject> m_input;
        // the scene manager of the assing scene
        [SerializeField] private GameObject sceneManager;
        // TODO Fix this to be able to determine which team we are currently assigning input for
        private byte m_teamIndex = 0;
        // determines whether it is player one or player two assigning the controls
        private byte m_playerIndex = 0;
        // a list of custom input bindings that we will be passing once filled
        private List<CustomInputBinding> m_customInputBindings;

        private void Update()
        {
            m_playerIndex = (byte)(FindObjectOfType<ControlUIScriptableObjectImplement>().GetisPlayerOne());
        }

        /// <summary>
        /// Pre-Condition: There are dropdowns and valid part IDs
        /// Post-Condition: Correct data (is player one/two, action index, input type, and part id) for each part is sent to BuildSceneInputData
        /// </summary>
        public void SubmitBinding()
        {
            m_customInputBindings = new List<CustomInputBinding>();

            ControlUIScriptableObjectImplement temp_control =
                sceneManager.GetComponent<ControlUIScriptableObjectImplement>();

            BuiltBotData temp_botData = BuildSceneBotData.GetBotData(m_teamIndex);

            // This for loop with the nested foreach, allows for the movement part to send
            // its input control and who is controlling it to the battle team
            for (int i = 0; i < temp_control.m_botPartsList.Count; i++)
            {
                if (temp_control.m_botPartsList[i].partId.Equals(temp_botData.movementPartID))
                {
                    m_input = temp_control.GetAllDropdownsByPart(i);
                    foreach (GameObject temp_inputObj in m_input)
                    {
                        InputButtonInfo temp_butInp =
                            temp_inputObj.GetComponent<InputButtonInfo>();
                        m_customInputBindings.Add(new CustomInputBinding(
                            temp_butInp.GetisPlayerOne(),
                            (byte)m_input.IndexOf(temp_inputObj),
                            temp_butInp.Getinput(),
                            PartSlotIndex.MOVEMENT_PART_SLOT_ID,
                            temp_botData.movementPartID));
                    }
                    break;
                }
            }

            // this foreach loop with the nested foreach allows for every slotted part
            // on the bot to send its input and who is contorlling it to the battle team
            for (byte temp_partSlotIndex = 0; temp_partSlotIndex <
                temp_botData.slottedPartIDList.Count; ++temp_partSlotIndex)
            {
                PartInSlot temp_partInSlot = temp_botData.slottedPartIDList[temp_partSlotIndex];
                for (int i = 0; i < temp_control.m_botPartsList.Count; i++)
                {
                    if (temp_control.m_botPartsList[i].partId.Equals(temp_partInSlot.partID))
                    {
                        m_input = temp_control.GetAllDropdownsByPart(i);
                        foreach (GameObject temp_inputObj in m_input)
                        {
                            InputButtonInfo temp_butInp =
                                temp_inputObj.GetComponent<InputButtonInfo>();
                            m_customInputBindings.Add(new CustomInputBinding(
                                temp_butInp.GetisPlayerOne(),
                                (byte)m_input.IndexOf(temp_inputObj),
                                temp_butInp.Getinput(),
                                temp_partSlotIndex,
                                temp_partInSlot.partID));
                        }
                        break;
                    }
                }
            }

            // TODO This is a hack. It just sets the data for the first team currently
            // PLEASE FIX - Sincerely Wyatt
            BuildSceneInputData.SetData(0, m_customInputBindings);
            Debug.Log("Sent Input Info to Battle Team");
        }
    }
}
