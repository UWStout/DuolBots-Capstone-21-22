using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Sets the bot's center of mass based on the movement colliders so that we
    /// no longer trip while trying to move.
    /// </summary>
    public class MovementPartColliders : MonoBehaviour
    {
        // Transforms that have colliders (ground interacting) for the movement parts attached to them.
        [SerializeField] private List<Transform> m_movementColliders = new List<Transform>();


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            Assert.AreNotEqual(0, m_movementColliders.Count, $"At least one collider must be specified " +
                $"for {GetType().Name}'s {nameof(m_movementColliders)}");
        }


        /// <summary>
        /// Gets all the colliders off the serialized movement collider transforms
        /// and returns them in a list.
        /// </summary>
        public IReadOnlyList<Collider> GetColliders()
        {
            List<Collider> temp_colliderList = new List<Collider>();
            foreach (Transform temp_curColParent in m_movementColliders)
            {
                Collider[] temp_curColliders = temp_curColParent.GetComponents<Collider>();
                temp_colliderList.AddRange(temp_curColliders);
            }
            Assert.AreNotEqual(0, temp_colliderList.Count,
                $"No colliders were found by {GetType().Name}");

            return temp_colliderList;
        }
        public IReadOnlyList<Transform> GetColliderTransforms() => m_movementColliders;
    }
}
