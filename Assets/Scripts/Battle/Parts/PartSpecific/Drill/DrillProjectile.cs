using UnityEngine;
// Original Author - Aaron Duffey
// Tweaked by Wyatt Senalik to use PartImpactCollider instead of
// OnTriggerAxis directly.

namespace DuolBots
{
    /// <summary>
    /// Script that handles the behavior of the Drill weapon's "projectile"
    /// ( a GameObject with a trigger Collider and DamageDealer).
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PartImpactCollider))]
    [RequireComponent(typeof(DamageDealer))]
    [RequireComponent(typeof(Collider))]
    public class DrillProjectile : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        [SerializeField] [Min(0.0f)] private float m_spinDamage = 0.05f;
        [SerializeField] [Min(0.0f)] private float m_jabDamage = 2.0f;

        private PartImpactCollider m_impactCol = null;
        private DamageDealer m_damageDealer = null;
        private Collider m_triggerCollider = null;
#if UNITY_EDITOR
        private MeshRenderer m_mr = null;
#endif
        private bool m_didSpinAlreadyHit = false;
        private bool m_didJabAlreadyHit = false;

        public PartImpactCollider impactCollider => m_impactCol;


        // Domestic initialization
        private void Awake()
        {
            m_impactCol = GetComponent<PartImpactCollider>();
            m_damageDealer = GetComponent<DamageDealer>();
            m_triggerCollider = GetComponent<Collider>();
#if UNITY_EDITOR
            if (IS_DEBUGGING)
            {
                m_mr = GetComponent<MeshRenderer>();
                CustomDebug.AssertComponentIsNotNull(m_mr, this);
                Material temp_copyMat = new Material(m_mr.material);
                m_mr.material = temp_copyMat;
            }
#endif
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_damageDealer, this);
            CustomDebug.AssertComponentIsNotNull(m_triggerCollider, this);
            #endregion Asserts
        }
        private void OnEnable()
        {
            m_damageDealer.onDamageDealtNoParam += OnDamageDealt;
        }
        private void OnDisable()
        {
            if (m_damageDealer != null)
            {
                m_damageDealer.onDamageDealtNoParam -= OnDamageDealt;
            }
        }
        // Foreign Initialization
        private void Start()
        {
            PartImpactCollider temp_impactCollider
                = GetComponent<PartImpactCollider>();
            ITeamIndex temp_teamIndex = GetComponentInParent<ITeamIndex>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(temp_impactCollider, this);
            CustomDebug.AssertIComponentInParentIsNotNull(temp_teamIndex, this);
            #endregion Asserts
            temp_impactCollider.teamIndex = temp_teamIndex.teamIndex;

            ResetDamageAndHit();
            m_triggerCollider.isTrigger = true;
        }


        /// <summary>
        /// Allows the spin to deal damage (true), or
        /// stop allowing the spin to deal damage (false).
        /// </summary>
        public void ResetSpinHit(bool shouldSpinHit)
        {
            m_didSpinAlreadyHit = !shouldSpinHit;
            UpdateDamageDealer();
        }
        /// <summary>
        /// Allows the jab to deal damage (true), or
        /// stop allowing the jab to deal damage (false).
        /// </summary>
        public void ResetJabHit(bool shouldJabHit)
        {
            m_didJabAlreadyHit = !shouldJabHit;
            UpdateDamageDealer();
        }


        /// <summary>
        /// Called by the damage dealer when it deals damage to a part.
        /// Sets already hit to true and changes the damage to deal to 0
        /// temporarily.
        /// </summary>
        private void OnDamageDealt()
        {
            ResetDamageAndHit();
        }
        private void ResetDamageAndHit()
        {
            m_didSpinAlreadyHit = true;
            m_didJabAlreadyHit = true;
            UpdateDamageDealer();
        }
        private void UpdateDamageDealer()
        {
            // If already hit, then damage should be 0,
            // otherwise it should be the damage to deal since we haven't hit yet.
            m_damageDealer.damageToDeal = DetermineDamage();

#if UNITY_EDITOR
            UpdateDebugMesh();
#endif
        }
        private float DetermineDamage()
        {
            // If we didn't hit with jab already, next we should deal its dmg
            if (!m_didJabAlreadyHit) { return m_jabDamage; }
            // If we didn't hit with spin already, next we should deal its dmg
            if (!m_didSpinAlreadyHit) { return m_spinDamage; }
            // We hit with jab and spin already, so we don't deal damage this time.
            return 0.0f;
        }

#if UNITY_EDITOR
        private void UpdateDebugMesh()
        {
            if (!IS_DEBUGGING) { return; }

            m_mr.enabled = m_damageDealer.damageToDeal > 0.0f;
            m_mr.material.color = DetermineMatColor();
        }
        private Color DetermineMatColor()
        {
            if (!m_didJabAlreadyHit) { return Color.blue; }
            if (!m_didSpinAlreadyHit) { return Color.red; }
            return Color.black;
        }
#endif
    }
}
