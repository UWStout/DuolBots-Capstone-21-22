using System;
using UnityEngine;
// Original Author - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Initializes projectile with ITeamIndex and any other necessary information on instantiation
    /// </summary>
    public class SpawnProjectileInitializer : MonoBehaviour
    {
        private ITeamIndex m_teamIndex = null;
        private IObjectSpawner m_ObjectSpawner = null;

        private void Awake()
        {
            m_teamIndex = GetComponentInParent<ITeamIndex>();
            CustomDebug.AssertIComponentIsNotNull(m_teamIndex, this);

            m_ObjectSpawner = GetComponentInParent<IObjectSpawner>();
            CustomDebug.AssertIComponentIsNotNull(m_ObjectSpawner, this);
        }

        private void OnEnable()
        {
            m_ObjectSpawner.onObjectSpawned += OnObjectSpawn;
        }

        private void OnDisable()
        {
            // In case the object gets destroyed before OnDisable() is called.
            if (m_ObjectSpawner != null)
            {
                m_ObjectSpawner.onObjectSpawned -= OnObjectSpawn;
            }
        }

        /// <summary>
        /// Set the team index on the spawned object.
        /// </summary>
        /// <param name="obj">GameObject that was spawned.</param>
        private void OnObjectSpawn(GameObject obj)
        {
            PartImpactCollider temp_damageDealer = obj.GetComponentInChildren<PartImpactCollider>();
            CustomDebug.AssertComponentOnOtherIsNotNull(temp_damageDealer, obj, this);
            if(temp_damageDealer != null)
            {
                temp_damageDealer.teamIndex = m_teamIndex.teamIndex;
            }
        }
    }
}
