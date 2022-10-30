using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Author - Wyatt Senalik and Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Handles colliding with a part.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public class PartImpactCollider : MonoBehaviour, ITeamIndexSetter
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        /// <summary>
        /// Params: TeamIndex for team that this part impact collider belongs to.
        /// TeamIndex for enemy team that was hit.
        /// </summary>
        public event Action<byte, byte> onHitEnemy;

        // The tag for parts
        [SerializeField] [Tag] private string m_partTag = "PartDamageable";
        [SerializeField] [Tag]
        private string[] m_ignoreTags = new string[]
        {
            "IgnoreDamagingCollider",
            "Projectile"
        };
        // Whether the projectile can impact any other collider.
        [SerializeField] private bool m_canImpactAnything = false;

        // Whether the projectile can damage the bot that fired it
        public bool canDamageSelf { get; set; } = false;

        public byte teamIndex { get; set; } = byte.MaxValue;
        // List of all the impact handlers on this object
        private List<IImpactHandler> m_impactHandlerList
            = new List<IImpactHandler>();
        // List of parts we are impacting with
        private List<Collider> m_collidersBeingHit = new List<Collider>();


        // Domestic Initialization
        private void Awake()
        {
            Collider temp_collider = GetComponent<Collider>();
            Assert.IsNotNull(temp_collider, $"There is no collider attached " +
                $"to {name}");

            // Find all the impact colliders attached to this object
            FindImpactHandlers();
            CustomDebug.Log($"{name} can damage self ? {canDamageSelf}", IS_DEBUGGING);
        }

        private void OnTriggerEnter(Collider collider)
        {
            CustomDebug.Log("PartImpactCollider OnTriggerEnter", IS_DEBUGGING);
        }
        private void OnTriggerStay(Collider collider)
        {
            CustomDebug.Log($"PartImpactCollider: {this.name} collided with " +
                $"{collider.name}", IS_DEBUGGING);

            // Ignore tag
            foreach (string temp_ignoreTag in m_ignoreTags)
            {
                if (collider.gameObject.CompareTag(temp_ignoreTag)) { return; }
            }

            if (!collider.gameObject.CompareTag(m_partTag))
            {
                // Since the object hit was not a part, do not attempt to do damage
                if (m_canImpactAnything)
                {
                    HandleImpact(collider, false, byte.MaxValue);
                }
                return;
            }
            CustomDebug.Log("Had tag : part", IS_DEBUGGING);

            ITeamIndex temp_hitTeam =
                collider.attachedRigidbody.GetComponent<ITeamIndex>();
            #region Asserts
            Assert.IsNotNull(temp_hitTeam, $"{name}'s {GetType().Name} expects " +
                $"{collider.attachedRigidbody.name} to have a " +
                $"{nameof(ITeamIndex)} attached, but none was found");
            Assert.AreNotEqual(byte.MaxValue, teamIndex, $"{nameof(teamIndex)} " +
                $"was not initialized for {name}'s {GetType()}");
            #endregion Asserts
            // If self damage is disabled and the hit team is us, do nothing.
            if (!canDamageSelf && temp_hitTeam.teamIndex == teamIndex)
            {
                CustomDebug.Log($"Cannot damage self and is TerrainHeightmapSyncControl same as TerrainHeightmapSyncControl team that was hit", IS_DEBUGGING);
                // Since we hit ourselves, do not do damage when handling impact
                if (m_canImpactAnything)
                {
                    CustomDebug.Log($"{name} can impact anything", IS_DEBUGGING);
                    HandleImpact(collider, false, temp_hitTeam.teamIndex);
                }
                #region Logs
                CustomDebug.LogForComponent($"We hit ourself", this, IS_DEBUGGING);
                #endregion Logs
                return;
            }

            // Handle all the impacts for each IImpactHandler
            HandleImpact(collider, true, temp_hitTeam.teamIndex);
        }
        private void OnTriggerExit(Collider other)
        {
            m_collidersBeingHit.Remove(other);
        }


        /// <summary>
        /// If the part impact collider is currently in contact with anything.
        /// </summary>
        public bool IsCurrentlyImpacting()
        {
            return m_collidersBeingHit.Count > 0;
        }


        /// <summary>
        /// Gets all the IImpactHandlers attached to this object.
        /// </summary>
        private void FindImpactHandlers()
        {
            m_impactHandlerList = new List<IImpactHandler>(
                GetComponents<IImpactHandler>());
            // Sort by priority
            m_impactHandlerList.Sort((IImpactHandler x, IImpactHandler y) =>
            {
               return x.priority.CompareTo(y.priority);
            });
            #region Logs
            CustomDebug.LogForComponent($"Found {m_impactHandlerList.Count} " +
                $"{nameof(IImpactHandler)}s", this, IS_DEBUGGING);
            #endregion Logs
        }
        /// <summary>
        /// Calls HandleImpact for each IImpactHandler attached to this object.
        /// </summary>
        /// <param name="collider">Collider we collided with.</param>
        private void HandleImpact(Collider collider, bool didHitEnemy, byte teamEnemyIndex)
        {
            if (!m_collidersBeingHit.Contains(collider))
            {
                m_collidersBeingHit.Add(collider);
            }

            if (didHitEnemy)
            {
                onHitEnemy?.Invoke(teamIndex, teamEnemyIndex);
            }

            #region Logs
            CustomDebug.LogForComponent($"{nameof(HandleImpact)} for " +
                $"{m_impactHandlerList.Count} {nameof(IImpactHandler)}s",
                this, IS_DEBUGGING);
            #endregion Logs
            foreach (IImpactHandler temp_impactHandler in m_impactHandlerList)
            {
                #region Logs
                CustomDebug.LogForComponent($"{nameof(HandleImpact)} for " +
                    $"{temp_impactHandler.priority} priority impact handler " +
                    $"({temp_impactHandler.GetType().Name}). Was this an enemy?" +
                    $"{didHitEnemy}", this, IS_DEBUGGING);
                #endregion Logs
                temp_impactHandler.HandleImpact(collider, didHitEnemy, teamEnemyIndex);
            }
        }
    }
}
