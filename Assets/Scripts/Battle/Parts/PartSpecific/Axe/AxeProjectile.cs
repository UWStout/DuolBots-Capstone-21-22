using UnityEngine;

using NaughtyAttributes;
// Original Author - Aaron Duffey
// Tweaked by Wyatt Senalik and Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Script that handles the behavior of the Axe weapon's "projectile"
    /// ( a GameObject with a trigger Collider and DamageDealer).
    /// </summary>
    [RequireComponent(typeof(DamageDealer))]
    [RequireComponent(typeof(Collider))]
    public class AxeProjectile : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        [SerializeField, Tag] private string m_partTag = "Part";
        [SerializeField, Tag] private string m_partDamageableTag = "PartDamageable";
        private DamageDealer m_damageDealer = null;
        private Collider m_triggerCollider = null;
        private ITeamIndex m_teamIndex = null;

        // Whether the collider is allowed to deal damage
        private bool m_canDealDamage = true;


        // Domestic initialization
        private void Awake()
        {
            m_damageDealer = GetComponent<DamageDealer>();
            m_triggerCollider = GetComponent<Collider>();
            m_teamIndex = GetComponentInParent<ITeamIndex>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_damageDealer, this);
            CustomDebug.AssertComponentIsNotNull(m_triggerCollider, this);
            CustomDebug.AssertIComponentInParentIsNotNull(m_teamIndex, this);
            #endregion Asserts
            if (!m_triggerCollider.isTrigger)
            {
                m_triggerCollider.isTrigger = true;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            CustomDebug.Log($"{name} was triggered by {other.name}", IS_DEBUGGING);

            // Can't deal damage
            if (!m_canDealDamage) { return; }
            // Didn't hit the right stuff
            if (!other.CompareTag(m_partTag) &&
                !other.CompareTag(m_partDamageableTag))
            { return; }

            #region Logs
            CustomDebug.Log($"{name} is dealing " +
                $"{m_damageDealer.damageToDeal} damage to {other.name}",
                IS_DEBUGGING);
            #endregion Logs
            m_damageDealer.DealDamageToPart(other, m_teamIndex.teamIndex);
            m_canDealDamage = false;
        }

        public void SetDamageToDeal(float damage)
        {
            #region Asserts
            CustomDebug.AssertIsTrueForComponent(m_damageDealer != null,
                $"{nameof(SetDamageToDeal)} not be called before DamageDealer " +
                $"has been initialized.", this);
            #endregion Asserts
            m_damageDealer.damageToDeal = damage;
        }
        public void ToggleCollider(bool isActive)
        {
            m_triggerCollider.enabled = isActive;
            m_canDealDamage = isActive;
        }
    }
}
