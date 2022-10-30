using DuolBots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GenerateViewportBot : MonoBehaviour
{
    // a variable that constructs the bot on screen
    private BotUnderConstruction m_UnderConstruction;
    // an empty root needed for assign screen for the bot
    [SerializeField] private GameObject m_empty;
    // needed to set the position of the bot on scene
    [SerializeField] private Transform m_Position;
    [SerializeField] private const float ROTATE_SPEED = 25f;
    public GameObject botObject => m_viewportObject;
    [SerializeField] private GameObject m_viewportObject = null;

    // TODO Make it actually be the right teamIndex
    private byte m_teamIndex = 0;


    /// <summary>
    /// Initually sets our root to an empty gameobject because we do not need anything
    /// in the UI Assign Scene from their root
    /// </summary>
    private void Awake()
    {
        m_UnderConstruction = new BotUnderConstruction(m_empty, m_Position.position, "BotObject");

    }

    /// <summary>
    /// Rotates the bot
    /// </summary>
    private void Update()
    {
        if (m_viewportObject == null)
        {
            m_viewportObject = GameObject.Find("BotObject");
        }
        else
        {
            m_viewportObject.transform.localScale = Vector3.one * 0.5f;
            //m_UnderConstruction.currentBotRoot.transform.Rotate(0, -ROTATE_SPEED * Time.deltaTime, 0);
        }
    }

    /// <summary>
    /// Creates the chassis, the movment, as well as the number of specified weapons
    /// on the chassis slots and displays it on screen
    /// </summary>
    public void BuildBot()
    {
        if (m_viewportObject != null)
        {
            foreach (Transform child in m_viewportObject.transform)
            {
                Destroy(child.gameObject);
            }
            //m_viewportObject = null;
        }

        // will assign the chassis from PartScriotableObject data from the Part Database
        PartScriptableObject m_chassis = null;
        // will assign the movement from PartScriotableObject data from the Part Database
        PartScriptableObject m_movement = null;
        // will assign the weapons/utilities in the correct weapon slots from
        IReadOnlyList<PartInSlot> m_partInSlot = null;
        PartScriptableObject m_partInSlotPrefab = null;

        PartDatabase temp_partDatabase = PartDatabase.instance;

        BuiltBotData temp_botData = BuildSceneBotData.GetBotData(m_teamIndex);

        // Chassis
        m_chassis = temp_partDatabase.GetPartScriptableObject(temp_botData.chassisID);
        Assert.IsNotNull(m_chassis, $"Chassis are null");
        m_UnderConstruction.CreateChassis(m_chassis.modelPrefab);

        // Movement
        if (temp_botData.movementPartID != "" && temp_botData.movementPartID != null)
        {
            m_movement = temp_partDatabase.GetPartScriptableObject(temp_botData.movementPartID);
        Assert.IsNotNull(m_movement, $"Wheels are null");
            m_UnderConstruction.CreateMovementPart(m_movement.modelPrefab, m_movement.partID);
            m_UnderConstruction.currentBotRoot.transform.position = m_Position.position;
        }
        // Weapons / Utilities
        m_partInSlot = temp_botData.slottedPartIDList;

        Assert.IsNotNull(m_partInSlot, $"Parts are null");
        foreach (PartInSlot temp_partInSlot in m_partInSlot)
        {
            m_partInSlotPrefab = temp_partDatabase.GetPartScriptableObject(temp_partInSlot.partID);
            m_UnderConstruction.CreateSlottedPart(m_partInSlotPrefab.modelPrefab, temp_partInSlot.slotIndex);
        }
    }
}
