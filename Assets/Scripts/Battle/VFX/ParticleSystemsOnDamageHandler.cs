using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Handles attached ParticleSystems on damage intake. Used for displaying visual effect for a part that is damaged.
    /// </summary>
    public class ParticleSystemsOnDamageHandler : MonoBehaviour, IParticleSystemPlayer
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        [SerializeField] private List<ParticleSystem> m_pSystems = null;
        private PartHealth m_partHealth = null;
        private PartSOReference m_partSO = null;
        private ePartType m_partType;

        public event Action<IReadOnlyList<ParticleSystem>> onPlayPSystem;

        private void Awake()
        {
            // Domestic Initialization, event subscription, and setup for ParticleSystems.
            CustomDebug.Log($"{name} has {m_pSystems.Count} attached {typeof(ParticleSystem)}'s", IS_DEBUGGING);
            m_partHealth = GetComponentInParent<PartHealth>();
            Assert.IsNotNull(m_partHealth ,$"{name} does not have an attached {nameof(PartHealth)}");

            // Check that the part isn't a movement part, otherwise subscribe to onDamageTaken, and setup ParticleSystems.
            m_partSO = GetComponentInParent<PartSOReference>();
            m_partType = m_partSO.partScriptableObject.partType;

            if (m_partType != ePartType.Movement)
            {
                m_partHealth.onDamageTaken += HandleDamage;

                foreach (ParticleSystem pSystem in m_pSystems)
                {
                    ParticleSystem.MainModule temp_pSModule = pSystem.main;
                    temp_pSModule.loop = false;
                    temp_pSModule.playOnAwake = false;
                }
            }
        
        }
        private void OnDisable()
        {
            // In case the PartHealth does not exist when OnDisable() is called.
            if(m_partHealth != null)
            {
                m_partHealth.onDamageTaken -= HandleDamage;
            }
        }

        private void HandleDamage(float damage)
        {
            if (m_pSystems != null && m_pSystems.Count > 0)
            {
                onPlayPSystem?.Invoke(m_pSystems);
            }
        }
    }
}
