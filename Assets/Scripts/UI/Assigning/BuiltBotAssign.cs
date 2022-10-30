using DuolBots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Original Authors - Cole Woulf
public class BuiltBotAssign : MonoBehaviour
{
    // a variable that constructs the bot on screen
    private BotUnderConstruction m_UnderConstruction;
    // an empty root needed for assign screen for the bot
    [SerializeField] private GameObject m_empty;
    // needed to set the position of the bot on scene
    [SerializeField] private Transform m_Position;

    private List<GameObject> m_partList;
    // TODO Make it actually be the right teamIndex
    private byte m_teamIndex = 0;


    /// <summary>
    /// Initually sets our root to an empty gameobject because we do not need anything
    /// in the UI Assign Scene from their root
    /// </summary>
    private void Awake()
    {
        m_UnderConstruction = new BotUnderConstruction(m_empty, m_Position.position);
        // need to set the number of slots
    }
    /// <summary>
    /// Builds the bot that was created in the Building Bot Scene
    /// </summary>
    private void Start()
    {
        // Look up the parts
        LoadPartsFromDatabase(out PartScriptableObject temp_chassisPart,
            out PartScriptableObject temp_movementPart, out IReadOnlyList<PartInSlot> temp_partInSlotList);

        BuiltBot();
    }
    /// <summary>
    /// Rotates the bot
    /// </summary>
    private void Update()
    {
        // can delete later, just giving it slight rotation for now to spin it
        //Assert.IsNotNull(m_UnderConstruction.currentBotRoot, $"bot is null");
        //m_UnderConstruction.currentBotRoot.transform.Rotate(0, -rotateSpeed * Time.deltaTime, 0);
    }


    /// <summary>
    /// Loads data from the BuildSceneBotData static class and the PartDatabase.
    ///
    /// Pre Condtions - The PartDatabase must have an instance. Data for which chassis, movement part,
    /// and utility/weapon parts were chosen must be in the BuildSceneBotData static class.
    /// Post Conditions - Returns data for chassis, movement, and utlity/weapon (slotted) parts.
    /// </summary>
    /// <param name="chassisPart">Which chassis part was chosen.</param>
    /// <param name="movementPart">Which movement part was chosen.</param>
    /// <param name="partInSlotList">Which utility/weapon parts were chosen.</param>
    private void LoadPartsFromDatabase(out PartScriptableObject chassisPart, out PartScriptableObject movementPart,
        out IReadOnlyList<PartInSlot> partInSlotList)
    {
        PartDatabase temp_partDatabase = PartDatabase.instance;

        BuiltBotData temp_botData = BuildSceneBotData.GetBotData(m_teamIndex);
        chassisPart = temp_partDatabase.GetPartScriptableObject(temp_botData.chassisID);
        movementPart = temp_partDatabase.GetPartScriptableObject(temp_botData.movementPartID);
        partInSlotList = temp_botData.slottedPartIDList;
    }
    /// <summary>
    /// Creates the chassis, the movment, as well as the number of specified weapons
    /// on the chassis slots and displays it on screen
    /// </summary>
    private void BuiltBot()
    {
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
        m_UnderConstruction.CreateChassis(m_chassis.battleLocalPrefab);

        // Movement
        m_movement = temp_partDatabase.GetPartScriptableObject(temp_botData.movementPartID);
        Assert.IsNotNull(m_movement, $"Wheels are null");
        m_UnderConstruction.CreateMovementPart(m_movement.battleLocalPrefab, m_movement.partID);

        // Weapons / Utilities
        m_partInSlot = temp_botData.slottedPartIDList;

        Assert.IsNotNull(m_partInSlot, $"Parts are null");
        foreach (PartInSlot temp_partInSlot in m_partInSlot)
        {
            m_partInSlotPrefab = temp_partDatabase.GetPartScriptableObject(temp_partInSlot.partID);
            m_UnderConstruction.CreateSlottedPart(m_partInSlotPrefab.battleLocalPrefab, temp_partInSlot.slotIndex);
        }
    }
}
