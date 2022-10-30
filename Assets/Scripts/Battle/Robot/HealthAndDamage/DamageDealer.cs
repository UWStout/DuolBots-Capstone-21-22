using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Helper class for weapons to use when they want to deal damage to a collider that
    /// has the tag PartDamageable.
    /// </summary>
    public class DamageDealer : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        // The amount of damage to deal
        [SerializeField] [Min(0.0f)] private float m_damageToDeal = 1.0f;

        public float damageToDeal { get => m_damageToDeal;  set => m_damageToDeal = value; }


        // Called when this deals damage to a part
        public event Action onDamageDealtNoParam;
        /// <summary>Param: amount of damage dealt.</summary>
        public event Action<float> onDamageDealt;

        /// <summary>
        /// Deals damage to the part attached to the given collider.
        ///
        /// Pre Conditions: Given collider has PartHealth attached to one of its parents.
        /// Post Conditions: Deals damage to the PartHealth.
        /// </summary>
        /// <param name="collider">Collider we impacted with.</param>
        public void DealDamageToPart(Collider collider, byte teamDealingDmg)
        {
            PartHealth temp_partHealth = collider.GetComponentInParent<PartHealth>();

            Assert.IsNotNull(temp_partHealth, $"No {nameof(PartHealth)} attached " +
                $"to {collider.name}, but its getting passed into {nameof(DamageImpactHandler)}'s " +
                $"{nameof(DealDamageToPart)}");

            DealDamageToPart(temp_partHealth, teamDealingDmg);
        }
        public void DealDamageToPart(PartHealth partHealth, byte teamDealingDmg)
        {
            // Cache the value incase one of the callbacks to
            // the event changes the damage.
            float temp_dmgToDeal = m_damageToDeal;
            // Deal damage to the hit part
            partHealth.TakeDamage(temp_dmgToDeal, teamDealingDmg);
            onDamageDealt?.Invoke(temp_dmgToDeal);
            onDamageDealtNoParam?.Invoke();
        }
        /// <summary>
        /// Deals damage to each unique PartHealth associated with the given colliders.
        /// Will not deal damage to a part more than once, even if it hit multiple colliders
        /// for the same part.
        /// Damage is evenly distributed to each part hit such that the amount of damage dealt
        /// to each part is equal to the damage to deal divided by the amount of unique parts hit.
        ///
        /// Pre Conditions - All given colliders have a PartHealth attached to some ancestor.
        /// Post Conditions - Each unique PartHealth will have its health deducted by the damage
        /// to deal divided by the amount of unique parts hit.
        /// 
        /// TODO - Optimize by moving GetComponent to a script in charge of caching
        /// a reference to the PartHealth on the same object as the Collider.
        /// </summary>
        /// <param name="partColliderList">List holding PartDamageable colliders.</param>
        public void DealDamageToParts(IReadOnlyList<Collider> partColliderList,
            byte teamDealingDmg)
        {
            List<PartHealth> temp_foundPartHealths = new List<PartHealth>();

            foreach (Collider temp_singlePartCol in partColliderList)
            {
                // PartHealth should be on the parent of every given collider
                PartHealth temp_curPartHealth = temp_singlePartCol.GetComponentInParent<PartHealth>();
                if (temp_curPartHealth == null)
                {
                    Assert.IsNotNull(temp_curPartHealth, $"No {nameof(PartHealth)} attached " +
                       $"to {temp_singlePartCol.name}, but its getting " +
                       $"passed into {nameof(DamageImpactHandler)}'s " +
                       $"{nameof(DealDamageToParts)}");
                    continue;
                }

                // Don't damage this part if we already damaged it
                if (temp_foundPartHealths.Contains(temp_curPartHealth)) { continue; }

                // This part has not been added to the list yet, so add it
                temp_foundPartHealths.Add(temp_curPartHealth);
            }

            // Can't deal damage to no parts. Avoid dividing by 0.
            if (temp_foundPartHealths.Count == 0) { return; }

            float temp_totalDmg = m_damageToDeal;
            float temp_damageToDeal = temp_totalDmg / temp_foundPartHealths.Count;
            #region Logs
            CustomDebug.LogForComponent($"Trying to deal {temp_totalDmg} " +
                $"damage split across {temp_foundPartHealths.Count} parts. " +
                $"{temp_damageToDeal} a piece", this, IS_DEBUGGING);
            #endregion Logs
            foreach (PartHealth temp_singlePartHealth in temp_foundPartHealths)
            {
                temp_singlePartHealth.TakeDamage(temp_damageToDeal, teamDealingDmg);
            }

            onDamageDealt?.Invoke(temp_totalDmg);
            onDamageDealtNoParam?.Invoke();
        }
    }
}
