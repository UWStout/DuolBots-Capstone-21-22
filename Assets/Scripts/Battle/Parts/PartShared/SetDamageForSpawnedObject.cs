using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik and Aaron Duffey

namespace DuolBots
{
    public class SetDamageForSpawnedObject : MonoBehaviour
    {
        [SerializeField, Min(0.0f)] private float m_damage = 0.0f;
        private IObjectSpawner[] m_objSpawners = new IObjectSpawner[0];

        public float damage
        {
            get => m_damage;
            set => m_damage = value;
        }


        // Domestic Initialization
        private void Awake()
        {
            m_objSpawners = GetComponentsInChildren<IObjectSpawner>(true);
            #region Asserts
            CustomDebug.AssertIndexIsInRange(0, m_objSpawners, this);
            #endregion Asserts
        }
        private void OnEnable()
        {
            foreach (IObjectSpawner temp_spawner in m_objSpawners)
            {
                temp_spawner.onObjectSpawned += TryToSetDamage;
            }
        }
        private void OnDisable()
        {
            foreach (IObjectSpawner temp_spawner in m_objSpawners)
            {
                if (temp_spawner == null) { continue; }
                temp_spawner.onObjectSpawned -= TryToSetDamage;
            }
        }


        private void TryToSetDamage(GameObject spawnedObj)
        {
            DamageDealer temp_dmgDealer = spawnedObj.
                GetComponentInChildren<DamageDealer>();
            if (temp_dmgDealer == null)
            {
                CustomDebug.LogWarning($"Did not find {nameof(DamageDealer)} " +
                    $"attached to {spawnedObj.name}");
                return;
            }

            temp_dmgDealer.damageToDeal = m_damage;
        }
    }
}
