using System;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Helper to put on a chassis for creating the movement part.
    /// Allows you to serialize all combinations of placing specific movement parts.
    /// Every MovementPart's placement should be specified on every Chassis.
    /// </summary>
    public class MovementPartPlacementManager : MonoBehaviour
    {
        // Dictionary for where to place the movement part for the part with the specified ID
        // This MUST line up with the corresponding MovementPart's Transform list exactly.
        // The parent Transform in this list at position x, must be the specific parent for
        // the Transform in the MovementPart's m_modelTransformList at index x.
        [SerializeField] private GenericDictionary<StringID, MovementPartSpecification>
            m_serializedMovementPlacementMap = new GenericDictionary<StringID, MovementPartSpecification>();
        // Dictionary that will reflect the serialzied dictionary, but with strings instead of StringIDs
        private Dictionary<string, MovementPartSpecification> m_movementPlacementMap =
            new Dictionary<string, MovementPartSpecification>();

        public Vector3 movePosOffset
        {
            get => m_movePosOffset;
            // Should only be called from BotUnderConstruction
            set => m_movePosOffset = value;
        }
        private Vector3 m_movePosOffset = Vector3.zero;


        // Domestic Initialization
        private void Awake()
        {
            // Transfer the serialized dictionary to the actual placement map
            foreach (KeyValuePair<StringID, MovementPartSpecification> temp_kvp
                in m_serializedMovementPlacementMap)
            {
                m_movementPlacementMap.Add(temp_kvp.Key.value, temp_kvp.Value);
            }
            // No need to hold the serialized one during runtime
            m_serializedMovementPlacementMap.Clear();
        }


        /// <summary>
        /// Gets the list of parent transforms for the specified partID.
        /// 
        /// Pre Conditions - The given partID must have been serialized in the serialized dictionary.
        /// Post Conditions - Returns the List of Transforms that corresponds to the given partID.
        /// </summary>
        /// <param name="partID">ID for the movement part we want the model parents for.</param>
        /// <returns></returns>
        public IReadOnlyList<Transform> GetMovementPartModelParentTransforms(string partID)
        {
            MovementPartSpecification temp_movementPartSpecs = GetMovementPartSpecification(partID);

            return temp_movementPartSpecs.placementLocations;
        }
        /// <summary>
        /// Gets the movement part placement specification for the given part ID.
        /// 
        /// Pre Conditions - The given partID must have been serialized in the serialized dictionary.
        /// Post Conditions - Returns the MovementPartSpecification that corresponds to the given partID.
        /// </summary>
        /// <param name="partID">ID for the movement part we want the placement specifications for.</param>
        /// <returns></returns>
        public MovementPartSpecification GetMovementPartSpecification(string partID)
        {
            if (!m_movementPlacementMap.TryGetValue(partID,
                out MovementPartSpecification temp_movementPartSpecs))
            {
                Debug.LogError($"Given part ID {partID} was not specified for {name}'s " +
                    $"{typeof(MovementPartPlacementManager)}");
                return null;
            }

            return temp_movementPartSpecs;
        }

        /// <summary>
        /// Used only for Squeek.
        /// Activates the GameObject that holds the stabilizing colliders corresponding to the given partID.
        /// </summary>
        public void SetStabilizers(string partID)
        {
            MovementPartSpecification currentPart = GetMovementPartSpecification(partID);
            if (currentPart != null)
            {
                currentPart.SetStabilizer(true);
            }
        }
    }


    /// <summary>
    /// Specifications of where to place a individual movement parts and
    /// how to orient the chassis based on it.
    /// </summary>
    [Serializable]
    public class MovementPartSpecification
    {
        // Where to place the individual models for the movement part
        [SerializeField] private List<Transform> m_placementLocations = new List<Transform>();
        // Offset for the chassis
        [SerializeField] private Vector3 m_chassisPlacementOffset = Vector3.zero;
        // Offset for the movement stabilizer collider
        [SerializeField] private Transform m_moveStabilizerColOffsetTrans = null;

        public IReadOnlyList<Transform> placementLocations => m_placementLocations;
        public Vector3 chassisPlacementOffset => m_chassisPlacementOffset;

        public void SetStabilizer(bool cond)
        {
            m_moveStabilizerColOffsetTrans.gameObject.SetActive(cond);
        }
    }
}
