using System;
using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Bomber's implementation of IChargeProjectileSpawn where the projectile grows
    /// in size and increases in damage based on the amount of charge.
    /// </summary>
    [RequireComponent(typeof(SetDamageForSpawnedObject))]
    public class BomberChargeProjectile : MonoBehaviour, ISpawnedChargeProjectile,
        IObjectSpawner
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        [SerializeField, Required] private PartImpactCollider m_impCol = null;
        [SerializeField, Min(1.0f)] private float m_secondsToSpawn = 1.0f;
        [SerializeField, Required]
        private GameObject m_explosiveObjectToSpawn = null;
        [SerializeField, Required] private Transform m_spawnPosition = null;
        [SerializeField, Min(0.0f)] private float m_chargeMultiplier = 0.0f;
        [SerializeField, Min(0.001f)] private float m_sizeScalingMultiplier = 1.0f;

        private SetDamageForSpawnedObject m_damageSetter = null;
        private float m_charge = 0.0f;

        public float charge => m_charge;

        public event Action<GameObject> onObjectSpawned;
        

        // Domestic Initialization
        private void Awake()
        {
            m_damageSetter = GetComponent<SetDamageForSpawnedObject>();
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_impCol, nameof(m_impCol),
                this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_explosiveObjectToSpawn,
                nameof(m_explosiveObjectToSpawn), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_spawnPosition,
                nameof(m_spawnPosition), this);
            CustomDebug.AssertComponentIsNotNull(m_damageSetter, this);
            #endregion Asserts

            Invoke(nameof(SpawnObject), m_secondsToSpawn);
            CustomDebug.Log($"This is when {name} spawned {Time.time}. Going to call " +
                $"{nameof(SpawnObject)} in {m_secondsToSpawn} at " +
                $"{Time.time + m_secondsToSpawn}.", IS_DEBUGGING);
        }
        private void OnDestroy()
        {
            CustomDebug.Log($"This is when {name} was told to die {Time.time}.", IS_DEBUGGING);
        }


        public void SetCharge(float charge)
        {
            m_charge = charge;
            this.transform.localScale = Vector3.one *
                m_sizeScalingMultiplier * m_charge;

            m_damageSetter.damage = m_charge * m_chargeMultiplier;
            #region Logs
            CustomDebug.LogForComponent($"Setting damage to deal to " +
                $"{m_damageSetter.damage} for {m_damageSetter.name}'s " +
                $"{m_damageSetter.GetType().Name}", this, IS_DEBUGGING);
            #endregion Logs
        }


        private void SpawnObject()
        {
            CustomDebug.Log($"Explosion is spawned " +
                $"at time {Time.time}", IS_DEBUGGING);

            GameObject temp_explosionObj =
                Instantiate(m_explosiveObjectToSpawn,
                m_spawnPosition.position, m_spawnPosition.rotation);

            // Pull off the ITeamIndexSetter
            ITeamIndexSetter temp_teamIndexSetter =
                temp_explosionObj.GetComponent<ITeamIndexSetter>();
            #region Asserts
            CustomDebug.AssertIComponentOnOtherIsNotNull(temp_teamIndexSetter,
                temp_explosionObj, this);
            #endregion Asserts
            temp_teamIndexSetter.teamIndex = m_impCol.teamIndex;

            onObjectSpawned?.Invoke(temp_explosionObj);
        }
    }
}
