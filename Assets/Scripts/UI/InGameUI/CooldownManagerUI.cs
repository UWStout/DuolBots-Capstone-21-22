using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DuolBots;
using DuolBots.Mirror;
// Original Authors - Shelby Vian

/// <summary>
/// Updates UI icons when cooldown is activated.
/// </summary>
public class CooldownManagerUI : MonoBehaviour
{
    private CooldownRemaining m_cooldownLeft;
    [SerializeField] private Image m_cooldownOverlay;
    private List<CustomInputBinding> m_inputs = new List<CustomInputBinding>();
   [SerializeField] private byte m_slot, m_team = 0, m_playerIndex;
    private float m_cooldownMax;
    [SerializeField] private bool m_hasFireAction = false;


    void Start()
    {

        m_playerIndex = gameObject.transform.parent.gameObject.GetComponent<PlayerIndex>().playerIndex;

        m_inputs = (List<CustomInputBinding>)BuildSceneInputData.GetInputBindingsForPlayer(m_team);

        m_slot = GetComponent<SetImageTextures>().partSlot;

        foreach (CustomInputBinding bind in m_inputs)
        {
            if (bind.playerIndex == m_playerIndex && bind.inputType == eInputType.buttonEast && bind.partSlotID == m_slot)
            {
                m_hasFireAction = true;
                break;
            }
        }

        

        m_cooldownOverlay.gameObject.SetActive(false);

        GameObject[] temp_parts = GameObject.FindGameObjectsWithTag("Part");

        foreach (GameObject part in temp_parts)
        {
            if(part.GetComponent<PartSlotIndex>() != null)
            {
                if (part.GetComponent<PartSlotIndex>().slotIndex == m_slot && part.transform.root.GetComponent<NetworkTeamIndex>().teamIndex == m_team)
                {
                    m_cooldownLeft = part.GetComponent<CooldownRemaining>();
                }
            }
        }
    }


    void Update()
    {
        if (m_hasFireAction && m_cooldownLeft != null)
        {
            if (m_cooldownLeft.coolDown < 1)
            {
                m_cooldownOverlay.gameObject.SetActive(true);
                m_cooldownOverlay.fillAmount = m_cooldownLeft.coolDown;

            }
            else if (m_cooldownLeft.coolDown >= 1)
                m_cooldownOverlay.gameObject.SetActive(false);
        }
    }
}
