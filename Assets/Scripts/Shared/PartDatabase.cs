using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Loads all the parts from the Resources/Parts folder and stores them.
    /// 
    /// Pre Conditions - All PartScriptableObjects MUST be in the folder Resources/Parts.
    /// They cannot be in subfolders of Resources/Parts. They must be simply in that folder.
    /// Post Conditions - Will hold be populated with the paths to all of the parts with
    /// the uniqueID as the key.
    /// </summary>
    public class PartDatabase : DynamicSingletonMonoBehaviourPersistant<PartDatabase>
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        // Name of the folder in the resources folder where all the parts are located
        [SerializeField] private string m_pathToPartsFolderInResources = "Parts";

        // Part's unique ID -> part
        private Dictionary<string, PartScriptableObject>
            m_partIDToPartScriptableObjectDictionary = new Dictionary<string, PartScriptableObject>();
        // If the parts are all loaded into the dictionary. If this is false, the parts
        // should be loaded when a part is tried to be gotten.
        private bool m_isLoaded = false;


        // Domestic Initialization
        protected override void Awake()
        {
            // Call the singleton's awake.
            base.Awake();

            // To save performance, load parts on awake.
            LoadParts();
        }


        /// <summary>
        /// Loads the scriptable object with the given ID from the resources folder.
        /// Cache a reference for reuse instead of calling this multiple times when possible.
        ///
        /// Pre Conditions - Assumes a part with the given part ID has been loaded into
        /// the dictionary already with a valid path to the part.
        /// Post Conditions - Loads the part from the Resources folder and returns it.
        /// </summary>
        /// <param name="partID">Unique ID for the part.</param>
        public PartScriptableObject GetPartScriptableObject(string partID)
        {
            // Load the parts if they are not loaded
            if (!m_isLoaded)
            {
                LoadParts();
            }

            if (!m_partIDToPartScriptableObjectDictionary.TryGetValue(partID, out PartScriptableObject temp_part))
            {
                // If the given partID was not in the dictionary, error out
                Debug.LogError($"{typeof(PartDatabase).Name} does not recognize the partID ({partID})");
                return null;
            }
            return temp_part;
        }
        /// <summary>
        /// Loads the scriptable object with the given ID from the resources folder.
        /// Cache a reference for reuse instead of calling this multiple times when possible.
        ///
        /// Pre Conditions - Assumes a part with the given part ID has been loaded into
        /// the dictionary already with a valid path to the part.
        /// Post Conditions - Loads the part from the Resources folder and returns it.
        /// </summary>
        /// <param name="partID">Unique ID for the part.</param>
        public PartScriptableObject GetPartScriptableObject(StringID partID)
        {
            return GetPartScriptableObject(partID.value);
        }
        /// <summary>
        /// Unloads the part scriptable objects from being held.
        /// </summary>
        public void UnloadParts()
        {
            m_partIDToPartScriptableObjectDictionary.Clear();
            m_isLoaded = false;
        }
        /// <summary>
        /// Gets a list of all the parts being held.
        /// </summary>
        public List<PartScriptableObject> GetAllPartScriptableObjects()
        {
            // Load the parts if they are not loaded
            if (!m_isLoaded)
            {
                LoadParts();
            }

            List<PartScriptableObject> temp_allParts =
                new List<PartScriptableObject>(m_partIDToPartScriptableObjectDictionary.Count);

            foreach (KeyValuePair<string, PartScriptableObject> temp_pair in
                m_partIDToPartScriptableObjectDictionary)
            {
                temp_allParts.Add(temp_pair.Value);
            }

            return temp_allParts;
        }
        /// <summary>
        /// Gets a list of all the slotted parts being held.
        /// </summary>
        /// <returns></returns>
        public List<PartScriptableObject> GetSlottedPartScriptableObjects(
            IReadOnlyCollection<PartScriptableObject> excludedParts = null)
        {
            // Load the parts if they are not loaded
            if (!m_isLoaded)
            {
                LoadParts();
            }
            // If no excluded parts, new up an empty list
            if (excludedParts == null)
            { excludedParts = new List<PartScriptableObject>(); }

            List<PartScriptableObject> temp_slottedParts
                = new List<PartScriptableObject>();

            foreach (KeyValuePair<string, PartScriptableObject> temp_pair in
                m_partIDToPartScriptableObjectDictionary)
            {
                if (!temp_pair.Value.partType.IsSlottedPart()) { continue; }
                if (excludedParts.Contains(temp_pair.Value)) { continue; }

                temp_slottedParts.Add(temp_pair.Value);
            }

            return temp_slottedParts;
        }


        /// <summary>
        /// Loads all the parts from the Resources folder.
        /// </summary>
        private void LoadParts()
        {
            m_partIDToPartScriptableObjectDictionary.Clear();

            // Load all the parts
            PartScriptableObject[] temp_partScriptableObjects =
                Resources.LoadAll<PartScriptableObject>(m_pathToPartsFolderInResources);

            CustomDebug.Log($"Loaded x={temp_partScriptableObjects.Length} parts from " +
                $"Resources/{m_pathToPartsFolderInResources}.", IS_DEBUGGING);

            // Add each part to the Part Holder
            foreach (PartScriptableObject temp_singlePartSO in temp_partScriptableObjects)
            {
                CustomDebug.Log($"Storing {temp_singlePartSO.partName}", IS_DEBUGGING);

                AddPartPathToDictionary(temp_singlePartSO.partID, temp_singlePartSO);
            }

            m_isLoaded = true;
        }
        /// <summary>
        /// Adds a part with the given ID to the dictionary.
        ///
        /// Pre Conditions - Assumes the part with the given ID has not already
        /// added to the dictionary.
        /// Post Conditions - The part is added to the dictionary.
        /// </summary>
        /// <param name="partID">Unique ID for the part.</param>
        /// <param name="part">Part to put in the dictionary.</param>
        private void AddPartPathToDictionary(string partID, PartScriptableObject part)
        {
            // If we already contain the key, we have a problem.
            if (m_partIDToPartScriptableObjectDictionary.ContainsKey(partID))
            {
                Debug.LogError($"{typeof(PartDatabase).Name} already contains a path for partID ({partID})");
                return;
            }

            m_partIDToPartScriptableObjectDictionary.Add(partID, part);
            CustomDebug.Log($"Added part with id ({partID}) to the dictionary with path (" +
                $"{part})", IS_DEBUGGING);
        }
    }
}
