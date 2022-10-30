using System;
using UnityEngine;
// Original Author - Wyatt Senalik and Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Spawns an object when the PartImpactCollider collides with a part.
    /// </summary>
    [RequireComponent(typeof(PartImpactCollider))]
    public class SpawnObjectImpactHandler : MonoBehaviour, IImpactHandler,
        IObjectSpawner
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        [SerializeField] private int m_priority = -1;
        // Explosion to spawn on impact
        [SerializeField] private GameObject m_prefabToSpawn = null;
        // The max amount we can spawn if we hit multiple objects
        [SerializeField] [Min(1)] private int m_maxAmountAllowedToSpawn = 1;

        // The current amount of objects we have spawned
        private int m_currentAmountSpawned = 0;

        private PartImpactCollider m_partImpactCollider = null;

        public event Action<GameObject> onObjectSpawned;

        private void Awake()
        {
            m_partImpactCollider = GetComponent<PartImpactCollider>();
        }

        #region IImpactHandler
        public int priority => m_priority;

        public void HandleImpact(Collider collider, bool didImpactEnemy, byte enemyTeamIndex)
        {
            // Only spawn the next object if we have not exceeded the amount
            // we are allowed to spawn.
            if (++m_currentAmountSpawned <= m_maxAmountAllowedToSpawn)
            {
                SpawnObject(collider);
            }
        }
        #endregion IImpactHandler


        /// <summary>
        /// Spawns an object at this objects position when the PartImpactCollider
        /// collides with a part.
        /// Intended to only be called by PartImpactCollider's onPartTriggerStay
        /// event.
        ///
        /// Pre Conditions: None.
        /// Post Conditions: Spawns an object at this object's position.
        /// </summary>
        /// <param name="collider">Collider we impacted with.</param>
        private void SpawnObject(Collider collider)
        {
            CustomDebug.Log($"Creating object from {name} colliding with " +
                $"{collider.name}", IS_DEBUGGING);
            GameObject temp_spawnedObj = Instantiate(m_prefabToSpawn,
                transform.position, Quaternion.identity);

            ITeamIndexSetter temp_teamIndex = temp_spawnedObj.GetComponent<ITeamIndexSetter>();
            if(temp_teamIndex != null)
            {
                CustomDebug.LogForComponent($"Found an object spawned with {nameof(ITeamIndexSetter)}: + {temp_spawnedObj}", this, IS_DEBUGGING);
                temp_teamIndex.teamIndex = m_partImpactCollider.teamIndex;
            }
            else
            {
                CustomDebug.LogForComponent($"Did not find an object spawned with attached {nameof(ITeamIndexSetter)}",
                    this, IS_DEBUGGING);
            }
            

            onObjectSpawned?.Invoke(temp_spawnedObj);
        }
    }
}
