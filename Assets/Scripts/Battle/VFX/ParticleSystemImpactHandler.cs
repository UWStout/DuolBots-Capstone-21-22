using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Aaron Duffey and Wyatt Senalik
namespace DuolBots
{
    /// <summary>
    /// Handles attached ParticleSystems when the ImpactHandler collides with a part.
    /// </summary>
    [RequireComponent(typeof(PartImpactCollider))]
    public class ParticleSystemImpactHandler : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        [SerializeField] private int m_priority = 0;

        [SerializeField] private List<ParticleSystem> m_pSystems = null;

        private void Awake()
        {
            foreach (ParticleSystem pSystem in m_pSystems)
            {
                pSystem.Stop();
            }
        }
        #region IImpactHandler
        public int priority => m_priority;

        public void HandleImpact(Collider collider, bool didImpactEnemy, byte enemyTeamIndex)
        {
            CustomDebug.LogForComponent(nameof(HandleImpact), this, IS_DEBUGGING);
            foreach(ParticleSystem pSystem in m_pSystems)
            {
                pSystem.Play();
            }
        }
        #endregion IImpactHandler
    }
}
