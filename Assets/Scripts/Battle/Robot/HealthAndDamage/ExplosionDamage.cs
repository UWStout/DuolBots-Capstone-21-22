using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik and Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Deals damage on Start to all parts in range.
    /// Meant to be spawned in to the game dynamically.
    /// </summary>
    [RequireComponent(typeof(DamageDealer))]
    public class ExplosionDamage : MonoBehaviour, ITeamIndexSetter
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        [SerializeField] [Min(0.0f)] private float m_explosionRadius = 1.0f;
        [SerializeField] private LayerMask m_explosionLayerMask = 1;
        [SerializeField] [Tag] private string m_partTag = "PartDamageable";

        // References
        private DamageDealer m_damageDealer = null;

        public byte teamIndex { get; set; } = byte.MaxValue;


        // Domestic Initialization
        private void Awake()
        {
            m_damageDealer = GetComponent<DamageDealer>();
            Assert.IsNotNull(m_damageDealer, $"{nameof(ExplosionDamage)} " +
                $"requires a {nameof(DamageDealer)} to be attached, but none " +
                $"was found.");
        }
        // Foreign Initialization
        private void Start()
        {
            DealDamageToPartsInRadius();
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, m_explosionRadius);
        }


        /// <summary>
        /// Deals damage to all parts within the explosion radius.
        ///
        /// Pre Conditions - Assumes this is called on Start.
        /// Post Conditions - No variable changes. Deals damage to all
        /// parts within the explosion radius.
        /// </summary>
        private void DealDamageToPartsInRadius()
        {
            // Get all the part healths in range
            IReadOnlyList<Collider> temp_partCollidersHitList =
                GetPartCollidersInRange();

            CustomDebug.Log($"Dealing damage to " +
                $"{temp_partCollidersHitList.Count} parts", IS_DEBUGGING);

            // Deal the damage to them
            m_damageDealer.DealDamageToParts(temp_partCollidersHitList, teamIndex);
        }
        /// <summary>
        /// Finds and returns all PartHealths within the explosion radius.
        ///
        /// Pre Conditions - All Part objects have PartHealth attached to them and
        /// all Part objects have the "PartDamageable" tag.
        /// Post Conditions - No variable changes. Returns a list of PartHealths.
        /// </summary>
        private IReadOnlyList<Collider> GetPartCollidersInRange()
        {
            // Initialize a list of part healths that we are going to build from
            List<Collider> temp_partCollidersHitList = new List<Collider>();

            foreach (Collider temp_singleColliderHit in GetCollidersInRange())
            {
                // Don't do anything if the thing hit wasn't a part
                if (!temp_singleColliderHit.CompareTag(m_partTag)) { continue; }

                // Check that the team index on the object we collided with
                // is noy the same as ours
                ITeamIndex temp_teamIndex = temp_singleColliderHit.
                    GetComponentInParent<ITeamIndex>();
                #region Asserts
                CustomDebug.AssertIComponentOnOtherIsNotNull(
                    temp_teamIndex, temp_singleColliderHit.gameObject, this);
                #endregion Asserts
                #region Logs
                CustomDebug.LogForComponent($"Found object with " +
                    $"{nameof(ITeamIndex)}: {temp_teamIndex.teamIndex}" +
                    $" {name}'s {nameof(ITeamIndex)} is {teamIndex}",
                    this, IS_DEBUGGING);
                #endregion Logs
                if (temp_teamIndex.teamIndex == teamIndex) { continue; }

                // Don't add a duplicate part to the list if it is already there
                if (temp_partCollidersHitList.Contains(temp_singleColliderHit))
                { continue; }

                #region Logs
                CustomDebug.Log($"Part ({temp_singleColliderHit}) is in range",
                    IS_DEBUGGING);
                #endregion Logs
                // The hit collider is a valid part
                temp_partCollidersHitList.Add(temp_singleColliderHit);
            }
            #region Logs
            CustomDebug.Log($"Found {temp_partCollidersHitList.Count} Part " +
                $"Colliders in range", IS_DEBUGGING);
            #endregion Logs           

            return temp_partCollidersHitList;
        }
        /// <summary>
        /// Finds and returns all Colliders within the explosion radius.
        ///
        /// Pre Conditions - Assumes all desired colliders
        /// (for Part objects) are on the specified layermask.
        /// Post Conditions - No variable changes. Returns a
        /// list of all Colliders within the explosion radius.
        /// </summary>
        private IReadOnlyList<Collider> GetCollidersInRange()
        {
            Collider[] temp_hitColliders = Physics.OverlapSphere(transform.position,
                m_explosionRadius, m_explosionLayerMask);
            #region Logs
            CustomDebug.Log($"Found {temp_hitColliders.Length} colliders " +
                $"in range", IS_DEBUGGING);
            #endregion Logs
            // Get the colliders in range
            return temp_hitColliders;
        }
    }
}
