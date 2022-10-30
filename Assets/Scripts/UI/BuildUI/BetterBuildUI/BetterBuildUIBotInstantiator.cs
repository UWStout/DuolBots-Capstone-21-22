using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    public class BetterBuildUIBotInstantiator : MonoBehaviour
    {
        // The root of the bot that will house the parts
        [SerializeField] [Required] private GameObject m_botRootPrefab = null;
        // Transform holding the location to spawn the bot at
        [SerializeField] private Transform m_defaultSpawnPos = null;
        // Transform that should be the bot's parent
        [SerializeField] private Transform m_parentTrans = null;

        private Vector3 defaultSpawnPos
        {
            get
            {
                if (m_defaultSpawnPos == null)
                {
                    CustomDebug.LogWarning($"Spawn Position was not " +
                        $"specified for {name}");
                    return Vector3.zero;
                }
                return m_defaultSpawnPos.position;
            }
        }

        private PartDatabase m_partDatabase = null;
        public BotUnderConstruction botUnderConstr => m_botUnderConstr;
        private BotUnderConstruction m_botUnderConstr = null;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            // Confirm a bot root prefab was specified
            Assert.IsNotNull(m_botRootPrefab, $"{GetType().Name} " +
                $"requries that a bot root be specified");
        }
        // Called 1st
        // Foreign Initialization
        private void Start()
        {
            m_partDatabase = PartDatabase.instance;
            // Confirm the PartDatabase is in the scene
            Assert.IsNotNull(m_partDatabase, $"{GetType().Name} " +
                $"requires that the " +
                $"{nameof(PartDatabase)} is in the scene.");
        }


        public void CreateChassis(StringID chassisID)
        {
            CreateChassis(chassisID.value);
        }
        public void CreateChassis(string chassisID)
        {
            PartScriptableObject m_chassisPartSO =
                LoadPartOfSpecificType(chassisID, ePartType.Chassis);
            m_botUnderConstr.CreateChassis(m_chassisPartSO.buildUIPrefab);
        }
        public void CreateMovementPart(StringID movementPartID)
        {
            CreateMovementPart(movementPartID.value);
        }
        public void CreateMovementPart(string movementPartID)
        {
            PartScriptableObject m_chassisPartSO =
                LoadPartOfSpecificType(movementPartID, ePartType.Movement);
            m_botUnderConstr.CreateMovementPart(m_chassisPartSO.buildUIPrefab,
                movementPartID);
        }
        public void AddSlottedPart()
        {
            // TODO
        }
        public void CreateBotRoot()
        {
            m_botUnderConstr = new BotUnderConstruction(
                m_botRootPrefab, defaultSpawnPos);
            if (m_parentTrans != null)
            {
                GameObject temp_botRoot = m_botUnderConstr.currentBotRoot;
                temp_botRoot.transform.parent = m_parentTrans;
                // Reset local rotation
                temp_botRoot.transform.localRotation = Quaternion.identity;
            }
        }


        private PartScriptableObject LoadPartOfSpecificType(StringID partID,
            ePartType expectedType)
        {
            return LoadPartOfSpecificType(partID.value, expectedType);
        }
        private PartScriptableObject LoadPartOfSpecificType(string partID,
            ePartType expectedType)
        {
            PartScriptableObject m_partSO =
                m_partDatabase.GetPartScriptableObject(partID);
            Assert.AreEqual(m_partSO.partType, expectedType,
                $"{m_partSO.name} is not {expectedType} " +
                $"and cannot be used as such.");

            return m_partSO;
        }
    }
}
