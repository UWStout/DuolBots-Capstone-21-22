using UnityEngine;
using UnityEngine.Assertions;
// Original Author - Wyatt Senalik and Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Deals damage to parts when the PartImpactCollider collides with a part.
    /// </summary>
    [RequireComponent(typeof(PartImpactCollider))]
    [RequireComponent(typeof(DamageDealer))]
    public class DamageImpactHandler : MonoBehaviour, IImpactHandler
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        [SerializeField] private int m_priority = 0;
        private PartImpactCollider m_partImpCol = null;
        // The thing that will deal damage
        private DamageDealer m_damageDealer = null;


        // Called 0th
        private void Awake()
        {
            m_partImpCol = GetComponent<PartImpactCollider>();
            m_damageDealer = GetComponent<DamageDealer>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_partImpCol, this);
            CustomDebug.AssertComponentIsNotNull(m_damageDealer, this);
            #endregion Asserts
        }


        #region IImpactHandler
        public int priority => m_priority;

        public void HandleImpact(Collider collider, bool didImpactEnemy, byte enemyTeamIndex)
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(HandleImpact), this, IS_DEBUGGING);
            #endregion Logs
            // If we didn't impact an enemy, don't try to deal damage.
            if (!didImpactEnemy) { return; }
            #region Logs
            CustomDebug.LogForComponent($"Trying to deal damage to {collider.name}",
                this, IS_DEBUGGING);
            #endregion Logs
            m_damageDealer.DealDamageToPart(collider, m_partImpCol.teamIndex);
        }
        #endregion IImpactHandler
    }
}
