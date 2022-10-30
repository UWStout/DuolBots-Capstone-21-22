using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Assistance class for instantiating a bot.
    /// Manges spawning the different kinds of parts for a bot.
    /// </summary>
    public class BotUnderConstruction
    {
        // Bot's root object
        private readonly GameObject m_currentBotRoot = null;
        // Chassis object
        private GameObject m_chassisObj = null;
        private SlotPlacementManager m_slotManager = null;
        private MovementPartPlacementManager m_movementPartManager = null;
        // Movement part object
        private GameObject m_movementObj = null;

        private Vector3 m_spawnPos = Vector3.zero;
        private Vector3 spawnPos
        {
            get => m_spawnPos;
            set
            {
                m_spawnPos = value;
                // When spawn position is changed, update the position
                UpdatePosition();
            }
        }
        // Don't change this variable directly, only use the property
        private Vector3 m_movementOffset = Vector3.zero;
        private Vector3 movementOffset
        {
            get => m_movementOffset;
            set
            {
                m_movementOffset = value;
                // Update the movement offset in the movement part manager
                m_movementPartManager.movePosOffset = m_movementOffset;
                // When movement part offset is changed, update the position
                UpdatePosition();
            }
        }

        public GameObject currentBotRoot => m_currentBotRoot;
        public GameObject currentChassis => m_chassisObj;
        public GameObject currentMovementPart => m_movementObj;
        public IReadOnlyDictionary<int, GameObject> currentSlottedParts => m_slotManager.GetSlottedParts();


        /// <summary>
        /// Instantiates the root of the bot.
        /// </summary>
        /// <param name="botRootPrefab">Root for the bot.</param>
        public BotUnderConstruction(GameObject botRootPrefab) : this(botRootPrefab, Vector3.zero) { }
        /// <summary>
        /// Instantiates the root of the bot at the specified position.
        /// </summary>
        /// <param name="botRootPrefab">Root for the bot.</param>
        /// <param name="startPos">Position to spawn the root at.</param>
        public BotUnderConstruction(GameObject botRootPrefab, Vector3 startPos, string name = "")
        {
            name = name == "" ? botRootPrefab.name : name;

            m_currentBotRoot = Object.Instantiate(botRootPrefab, startPos, Quaternion.identity);
            m_currentBotRoot.name = name;
            // Must be done after the root is instantiated.
            // Property calls a functio that requires m_currentBotRoot to not be null.
            spawnPos = startPos;
        }
        /// <summary>
        /// Constructs a BotUnderConstruction without instantiating any bot root.
        /// Instead it uses the given instance as the bot root.
        /// </summary>
        /// <param name="startPos">Position to have the bot positioned at.</param>
        /// <param name="botRootInstance">Already spawned instance of the bot root.</param>
        private BotUnderConstruction(Vector3 startPos, GameObject botRootInstance)
        {
            m_currentBotRoot = botRootInstance;
            // Must be done after the root is instantiated.
            // Property calls a functio that requires m_currentBotRoot to not be null.
            spawnPos = startPos;
        }

        /// <summary>
        /// Constructs a BotUnderConstruction without instantiating any bot root.
        /// Instead it uses the given instance as the bot root.
        /// </summary>
        /// <param name="botRootInstance">Already spawned instance of the bot root.</param>
        /// <param name="startPos">Position to have the bot positioned at.</param>
        /// <returns>Returns the constructed BotUnderConstruction.</returns>
        public static BotUnderConstruction ConstructUsingBotRootInstance(GameObject botRootInstance, Vector3 startPos)
        {
            return new BotUnderConstruction(startPos, botRootInstance);
        }


        /// <summary>
        /// Creates a chassis from the given prefab.
        ///
        /// Pre Conditions - The bot root must be crated first (should occur in constructor).
        /// Given chassis prefab must have a SlotManager and
        /// MovementPartManager attached to it.
        /// Post Conditions - Instantiates the chassis prefab as a child of the bot root.
        /// </summary>
        /// <param name="chassisPrefab">Prefab of the chassis to spawn.</param>
        public void CreateChassis(GameObject chassisPrefab)
        {
            Assert.IsNotNull(m_currentBotRoot, $"Root cannot be null when trying to create a chassis");

            m_chassisObj = Object.Instantiate(chassisPrefab, m_currentBotRoot.transform);

            m_slotManager = m_chassisObj.GetComponent<SlotPlacementManager>();
            Assert.IsNotNull(m_slotManager, $"Given chassis ({m_chassisObj.name}) did not" +
                $" have required {typeof(SlotPlacementManager).Name} attached to it.");

            m_movementPartManager = m_chassisObj.GetComponent<MovementPartPlacementManager>();
            Assert.IsNotNull(m_movementPartManager, $"Given chassis ({m_chassisObj.name}) did not" +
                $" have required {typeof(MovementPartPlacementManager).Name} attached to it.");
        }
        /// <summary>
        /// Creates a movement part from the given prefab.
        ///
        /// Pre Conditions - A chassis must have been created before this call. The
        /// given movement part prefab must have MovementPartModels attached to it.
        /// The given movement part's ID must have been used to specify attach positions
        /// on the chassis's MovementPartManager.
        /// Post Conditions - Instantiates the movement prefab to be children of the chassis.
        /// The parts should also be in the correct positions.
        /// </summary>
        /// <param name="movementPartPrefab">Prefab of the movement part.</param>
        /// <param name="partID">Movement part's ID.</param>
        public void CreateMovementPart(GameObject movementPartPrefab, string partID)
        {
            Assert.IsNotNull(m_chassisObj, $"Movement Part cannot be created before a Chassis");

            // Create the movement objet
            m_movementObj = Object.Instantiate(movementPartPrefab, m_chassisObj.transform);

            // Pull of the movement part's models
            MovementPartModels temp_movementPart = m_movementObj.GetComponent<MovementPartModels>();
            Assert.IsNotNull(temp_movementPart, $"Given movement part ({m_movementObj.name}) " +
                $"did not have required {typeof(MovementPartModels).Name} attached to it.");

            // Move the models to be at the pre-defined location on the model
            MovementPartSpecification temp_movementPartSpec =
                m_movementPartManager.GetMovementPartSpecification(partID);
            temp_movementPart.MoveModels(temp_movementPartSpec.placementLocations);

            // Activates the correct set of stabilizing colliders for Squeek
            m_movementPartManager.SetStabilizers(partID);

            // We no longer do this here, PartSlotIndex now initializes itself.
            //// Set the slot index of the movement part
            //PartSlotIndex temp_movePartSlotIndex =
            //    m_movementObj.GetComponent<PartSlotIndex>();
            //Assert.IsNotNull(temp_movePartSlotIndex, $"{GetType().Name} expected " +
            //    $"{m_movementObj.name} to have {nameof(PartSlotIndex)} " +
            //    $"but none was found");
            //temp_movePartSlotIndex.slotIndex = PartSlotIndex.MOVEMENT_PART_SLOT_ID;

            // Apply the placement offset to the chassis
            movementOffset = temp_movementPartSpec.chassisPlacementOffset;
        }
        /// <summary>
        /// Creates a slotted part from the given prefab and places it at the specified slot index.
        ///
        /// Pre Conditions - A chassis must have been created before this call. The slotted part
        /// must have a position offset built into it to account for the slot. It should be such that
        /// the part begins at (0, 0, 0) and extends forward in the z direction.
        /// Post Conditions - Instantiates the slotted prefab to be a child of the chassis
        /// at the location of the slot with the given slot index.
        /// </summary>
        /// <param name="slottedPartPrefab">Prefab of the slotted part.</param>
        /// <param name="slotIndex"></param>
        public GameObject CreateSlottedPart(GameObject slottedPartPrefab, byte slotIndex)
        {
            Assert.IsNotNull(m_chassisObj, $"Slotted Part cannot be created before a Chassis");
            Assert.IsNotNull(m_slotManager, $"SlotManager is missing. Cannot create slotted part");

            GameObject temp_partInstance = m_slotManager.AddPartToSlot(slottedPartPrefab, slotIndex);
            return temp_partInstance;
        }

        /// <summary>
        /// Destroys the current chassis.
        ///
        /// Pre Conditions - Chassis must exist.
        /// Post Conditions - Chassis is destroyed and references are thrown away.
        /// </summary>
        public void DestroyChassis()
        {
            if (m_chassisObj == null)
            {
                Debug.LogWarning("Trying to destroy a chassis that doesn't exist");
                return;
            }

            Object.Destroy(m_chassisObj);
            m_slotManager = null;
            m_movementPartManager = null;
        }
        /// <summary>
        /// Destroys the current movement part.
        ///
        /// Pre Conditions - Movement part must exist (Chassis must exist).
        /// Post Conditions - Movement part is destroyed.
        /// </summary>
        public void DestroyMovementPart()
        {
            if (m_movementObj == null)
            {
                Debug.LogWarning("Trying to destroy a Movement Part and there is no movement part");
                return;
            }

            Object.Destroy(m_movementObj);
            movementOffset = Vector3.zero;
        }
        /// <summary>
        /// Destroys the part at the specified.
        ///
        /// Pre Conditions - Chassis cannot be destroyed yet.
        /// Post Conditions - Destroys the part at the given slot index.
        /// </summary>
        /// <param name="slotIndex">Index to delete the part from.</param>
        /// <returns>True - if the part exists and is deleted. False - if the part
        /// did not exist and there was nothing deleted.</returns>
        public bool DestroySlottedPart(int slotIndex)
        {
            Assert.IsNotNull(m_chassisObj, $"Cannot delete a part without a Chassis");
            Assert.IsNotNull(m_slotManager, $"SlotManager is missing. Cannot delete slotted part");

            return m_slotManager.RemovePartFromSlot(slotIndex);
        }


        /// <summary>
        /// Updates the position of the bot root.
        ///
        /// Pre Conditions - m_currentBotRoot must exist.
        /// Post Conditions - The bot root is moved to be in a position that reflects
        /// both the spawn position and the movement part offset.
        /// </summary>
        private void UpdatePosition()
        {
            m_currentBotRoot.transform.position = spawnPos + movementOffset;
        }
    }
}
