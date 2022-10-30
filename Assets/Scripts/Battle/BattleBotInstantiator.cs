using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Handles the instantiation of the bot for the battle scene.
    /// </summary>
    public class BattleBotInstantiator : MonoBehaviour
    {
        public enum eCreateBotExecuteOptions { Awake, Start, Manual }

        // The root of the bot that will house the parts
        [SerializeField] private GameObject m_botRootPrefab = null;
        // Transform holding the location to spawn the bot at
        [SerializeField] private Transform m_defaultSpawnPos = null;
        private Vector3 defaultSpawnPos
        {
            get
            {
                if (m_defaultSpawnPos == null)
                {
                    CustomDebug.LogWarning($"Spawn Position was not specified for {name}");
                    return Vector3.zero;
                }
                return m_defaultSpawnPos.position;
            }
        }

        // When to create the bot. Create it automatically
        // (Start) or manually (not until called).
        [SerializeField] private eCreateBotExecuteOptions m_whenToCreateBot = eCreateBotExecuteOptions.Start;
        private bool IsCreateAutomatic() => m_whenToCreateBot != eCreateBotExecuteOptions.Manual;
        [SerializeField] [ShowIf(nameof(IsCreateAutomatic))] private byte m_automaticTeamIndex = 0;
        [SerializeField] private eMultiplayerOption m_multiplayerOption = eMultiplayerOption.Local;

        // Reference to the bot that is currently being spawned
        private BotUnderConstruction m_currentBot = null;

        private bool m_wasBotCreated = false;


        // Event for when the bot has finished being created.
        public static event Action<byte> onCreateBotFinished;


        // Domestic Initialization
        private void Awake()
        {
            // Confirm a bot root prefab was specified
            Assert.IsNotNull(m_botRootPrefab, $"{GetType().Name} requries that a bot root be specified");

            if (m_whenToCreateBot == eCreateBotExecuteOptions.Awake)
            {
                CreateBot(m_automaticTeamIndex);
            }
        }
        // Foreign Initialization
        private void Start()
        {
            // Confirm the PartDatabase is in the scene
            Assert.IsNotNull(PartDatabase.instance, $"{GetType().Name} requires that the " +
                $"{typeof(PartDatabase).Name} is in the scene.");

            if (m_whenToCreateBot == eCreateBotExecuteOptions.Start)
            {
                CreateBot(m_automaticTeamIndex);
            }
        }


        /// <summary>
        /// Creates the Bot in this order:
        /// Chassis (1) -> Movement (1) -> Utility/Weapon Parts (Variable).
        ///
        /// Pre Conditions - The PartDatabase must have an instance. Data for which chassis, movement part,
        /// and utility/weapon parts were chosen must be in the BuildSceneBotData static class.
        /// Post Conditions - The bot is instantiated with all its parts in the correct spots.
        /// </summary>
        /// <param name="teamIndex">Team to spawn the bot for.</param>
        /// <param name="spawnPos">Optional spawn position. If not specified,
        /// the default spawn position willbe used.</param>
        /// <param name="botRootInstance">Optional existing instance of a bot root
        /// to create the BotUnderConstruction using. Otherwise, will create a new
        /// object to server as the bot root using the bot root prefab.</param>
        public BotUnderConstruction CreateBot(byte teamIndex, Vector3? spawnPos = null,
            GameObject botRootInstance = null)
        {
            if (m_wasBotCreated)
            {
                Debug.LogWarning($"{name}'s {GetType().Name} is trying to create " +
                    $"more than 1 bot. This is probably wrong.");
            }

            // TODO change it from local to something else that determines if we are local or networked
            eMultiplayerOption multiplayerOption = m_multiplayerOption;

            // Look up the parts
            bool temp_wasLoadSuccess = LoadPartsFromDatabase(teamIndex,
                out PartScriptableObject temp_chassisPart,
                out PartScriptableObject temp_movementPart,
                out IReadOnlyList<PartInSlot> temp_partInSlotList);

            // If there is no bot data for
            if (!temp_wasLoadSuccess)
            {
                Debug.LogError($"There was no BotData for team index {teamIndex}");
                return null;
            }

            // Spawn the bot
            CreateChassis(temp_chassisPart, multiplayerOption, spawnPos, botRootInstance);
            CreateMovementPart(temp_movementPart, multiplayerOption);
            CreateSlottedParts(temp_partInSlotList, multiplayerOption);
            // Set the bot's team
            SetBotTeam(teamIndex);
            // Clear references
            BotUnderConstruction temp_constructedBot = m_currentBot;
            ClearReferenceToBot();

            m_wasBotCreated = true;

            // Call the event for the bot finising.
            onCreateBotFinished?.Invoke(teamIndex);

            return temp_constructedBot;
        }
        /// <summary>
        /// Loads data from the BuildSceneBotData static class and the PartDatabase.
        ///
        /// Pre Condtions - The PartDatabase must have an instance. Data for which chassis, movement part,
        /// and utility/weapon parts were chosen must be in the BuildSceneBotData static class.
        /// Post Conditions - Returns data for chassis, movement, and utlity/weapon (slotted) parts.
        /// </summary>
        /// <param name="teamIndex">Index of the team whose bot is being built.</param>
        /// <param name="chassisPart">Which chassis part was chosen.</param>
        /// <param name="movementPart">Which movement part was chosen.</param>
        /// <param name="partInSlotList">Which utility/weapon parts were chosen.</param>
        /// <returns>False if there is no BotData for the specified team.
        /// True if there is BotData for the specified team.</returns>
        private bool LoadPartsFromDatabase(byte teamIndex,
            out PartScriptableObject chassisPart,
            out PartScriptableObject movementPart,
            out IReadOnlyList<PartInSlot> partInSlotList)
        {
            PartDatabase temp_partDatabase = PartDatabase.instance;

            BuiltBotData temp_botData = BuildSceneBotData.GetBotData(teamIndex);

            chassisPart = null;
            movementPart = null;
            partInSlotList = null;
            if (temp_botData == null) { return false; }

            chassisPart = temp_partDatabase.GetPartScriptableObject(temp_botData.chassisID);
            movementPart = temp_partDatabase.GetPartScriptableObject(temp_botData.movementPartID);
            partInSlotList = temp_botData.slottedPartIDList;

            return true;
        }
        /// <summary>
        /// Spawns the given chassis.
        ///
        /// Pre Conditions - Any previous bots instantiated here should be cleared.
        /// Expects that the part passed in is specifically a chassis.
        /// Post Conditions - Starts the creation of a new bot using the bot root specified on the object
        /// and spawns the given chassis.
        /// </summary>
        /// <param name="chassisPart">Chassis to spawn.</param>
        /// <param name="multiplayerOption">Local vs Networked.</param>
        /// <param name="spawnPos">Optional spawn position. If not specified,
        /// the default spawn position willbe used.</param>
        /// <param name="botRootInstance">Optional existing instance of a bot root
        /// to create the BotUnderConstruction using. Otherwise, will create a new
        /// object to server as the bot root using the bot root prefab.</param>
        private void CreateChassis(PartScriptableObject chassisPart,
            eMultiplayerOption multiplayerOption, Vector3? spawnPos = null,
            GameObject botRootInstance = null)
        {
            // If there is still a last bot, it will be cleared
            if (m_currentBot != null)
            {
                Debug.LogWarning($"CreateChassis was called before clearing the last bot information." +
                    $" Lost reference to last bot");
                ClearReferenceToBot();
            }
            // New up the base bot
            CreateNewBaseBot(spawnPos, botRootInstance);

            // Confirm the part really is a chassis
            if (chassisPart.partType != ePartType.Chassis && chassisPart.partType!=ePartType.Other)
            {
                Debug.LogError($"A non-Chassis part ({chassisPart.partName})" +
                    $" was passed to CreateChassis");
                return;
            }

            // Choose the right option based on the multiplayer option
            GameObject temp_prefabToUse = ChoosePartPrefab(chassisPart, multiplayerOption);
            Assert.IsNotNull(m_currentBot, $"Current bot was null when " +
                $"trying to create a new chassis");
            m_currentBot.CreateChassis(temp_prefabToUse);
        }
        /// <summary>
        /// Spawns the given movement part.
        ///
        /// Pre Conditions - There must be a current bot with a chassis created. Given part
        /// must be a movement part.
        /// Post Conditions - Tells the current bot to create a movement part.
        /// </summary>
        /// <param name="movementPart">Movement part to create.</param>
        /// <param name="multiplayerOption">Local vs Networked.</param>
        private void CreateMovementPart(PartScriptableObject movementPart, eMultiplayerOption multiplayerOption)
        {
            // Cannot create a movement part if there is no bot
            if (m_currentBot == null)
            {
                Debug.LogError($"CreateMovementPart was called before a chassis was created.");
                return;
            }

            // Confirm the part really is a chassis
            if (movementPart.partType != ePartType.Movement)
            {
                Debug.LogError($"A non-Movement part ({movementPart.partName})" +
                    $" was passed to CreateMovementPart");
                return;
            }

            // Choose the right option based on multiplayer option
            GameObject temp_prefabToUse = ChoosePartPrefab(movementPart, multiplayerOption);
            Assert.IsNotNull(m_currentBot, $"Current bot was null when " +
                $"trying to create a movement part");
            m_currentBot.CreateMovementPart(temp_prefabToUse, movementPart.partID);
        }
        /// <summary>
        /// Spawns the given utility/weapon parts in their slots.
        ///
        /// Pre Conditions - There must be a current bot with a chassis created. Given parts
        /// must be utility or weapon parts.
        /// Post Conditions - Tells the current bot to create all the given slotted part.
        /// </summary>
        /// <param name="partsInSlotsList">List of utility/weapon parts to create.</param>
        /// <param name="multiplayerOption">Local vs Networked.</param>
        private void CreateSlottedParts(IReadOnlyList<PartInSlot> partsInSlotsList, eMultiplayerOption multiplayerOption)
        {
            // Cannot create slotted parts if there is no bot
            if (m_currentBot == null)
            {
                Debug.LogError($"CreateSlottedParts was called before a chassis was created.");
                return;
            }

            foreach (PartInSlot temp_partInSlot in partsInSlotsList)
            {
                CreateSingleSlottedPart(temp_partInSlot, multiplayerOption);
            }
        }
        /// <summary>
        /// Spawns a single utility/weapon part in its slot.
        ///
        /// Pre Conditions - There must be a current bot with a chassis created. Given part
        /// must be utility or weapon part.
        /// Post Conditions - Tells the current bot to create a slotted part.
        /// </summary>
        /// <param name="partInSlot">Utility/weapon part to create.</param>
        /// <param name="multiplayerOption">Local vs Networked.</param>
        private void CreateSingleSlottedPart(PartInSlot partInSlot, eMultiplayerOption multiplayerOption)
        {
            PartScriptableObject temp_part = PartDatabase.instance.GetPartScriptableObject(partInSlot.partID);

            // Confirm the part really is a slotted part
            if (temp_part.partType != ePartType.Weapon && temp_part.partType != ePartType.Utility)
            {
                Debug.LogError($"A non-Weapon/Utility part ({temp_part.partName})" +
                    $" was passed to CreateSingleSlottedPart");
                return;
            }

            // Choose the right option based on multiplayer option
            GameObject temp_prefabToUse = ChoosePartPrefab(temp_part, multiplayerOption);
            Assert.IsNotNull(m_currentBot, $"Current bot was null when " +
                $"trying to create a slotted part");
            m_currentBot.CreateSlottedPart(temp_prefabToUse, partInSlot.slotIndex);
        }
        /// <summary>
        /// Sets the bot to be a part of the team associated with this battle bot instantiator.
        ///
        /// Pre Conditions - There must be a current bot spawned. The root of that bot must have
        /// a TeamIndex script attached to it.
        /// Post Conditions - The team index of the TeamIndex script is changed to match this
        /// instantiator's team index.
        /// </summary>
        private void SetBotTeam(byte teamIndex)
        {
            Assert.IsNotNull(m_currentBot, $"Cannot set bot's team when there is no bot for {name}'s" +
                $" {nameof(BattleBotInstantiator)}");

            ITeamIndex temp_botTeamIndex = m_currentBot.currentBotRoot.GetComponent<ITeamIndex>();
            Assert.IsNotNull(temp_botTeamIndex, $"Specified bot root for {name}'s {nameof(BattleBotInstantiator)}" +
                $" does not have {nameof(ITeamIndex)} attached");
            temp_botTeamIndex.teamIndex = teamIndex;
        }

        /// <summary>
        /// Sets current bot to null, if it wasn't already.
        /// </summary>
        private void ClearReferenceToBot()
        {
            m_currentBot = null;
        }
        /// <summary>
        /// Creates a fresh bot to begin construction on.
        ///
        /// Pre Conditions - A bot root must be specified in the inspector.
        /// Post Conditions - A new BotUnderConstruction is created.
        /// </summary>
        /// <param name="spawnPos">Optional spawn position. If not specified,
        /// the default spawn position willbe used.</param>
        /// <param name="botRootInstance">Optional existing instance of a bot root
        /// to create the BotUnderConstruction using. Otherwise, will create a new
        /// object to server as the bot root using the bot root prefab.</param>
        private void CreateNewBaseBot(Vector3? spawnPos = null, GameObject botRootInstance = null)
        {
            Assert.IsNotNull(m_botRootPrefab, $"Can't create a new base bot without a bot root.");

            // Spawn at the given spawn position (if any). Otherwise, just use the default.
            Vector3 startPos = spawnPos != null ? spawnPos.Value : defaultSpawnPos;
            if (botRootInstance != null)
            {
                m_currentBot = BotUnderConstruction.ConstructUsingBotRootInstance(botRootInstance, startPos);
            }
            else
            {
                m_currentBot = new BotUnderConstruction(m_botRootPrefab, startPos, $"Bot Root");
            }
        }
        /// <summary>
        /// Helper function to choose which battle prefab to spawn (Local or Network version).
        ///
        /// Pre Conditions - Both local and network prefabs of the PartScriptableObject must be specified.
        /// Post Conditions - Returns either the local or network prefab.
        /// </summary>
        /// <param name="part">Part data holding the prefabs.</param>
        /// <param name="multiplayerOption">Local vs Networked.</param>
        /// <returns>Which battle prefab to spawn.</returns>
        private GameObject ChoosePartPrefab(PartScriptableObject part, eMultiplayerOption multiplayerOption)
        {
            Assert.IsNotNull(part.battleLocalPrefab, $"{GetType().Name}'s ChoosePartPrefab was given a" +
                $" part with no LOCAL prefab specified.");
            Assert.IsNotNull(part.battleNetworkPrefab, $"{GetType().Name}'s ChoosePartPrefab was given a" +
                $" part with no NETWORK prefab specified.");

            // Choose the right option based on the multiplayer option
            switch (multiplayerOption)
            {
                case eMultiplayerOption.Local:
                    return part.battleLocalPrefab;
                case eMultiplayerOption.Networked:
                    return part.battleNetworkPrefab;
                default:
                    Debug.LogError($"Unhandled enum for {typeof(eMultiplayerOption).Name} of {multiplayerOption}");
                    return null;
            }
        }
    }
}
