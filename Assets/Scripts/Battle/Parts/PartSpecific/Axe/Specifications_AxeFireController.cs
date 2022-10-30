using UnityEngine;

using NaughtyAttributes;
// Original Authors - Aaron Duffey

namespace DuolBots
{
    public class Specifications_AxeFireController : MonoBehaviour
    {
        [SerializeField] [Min(0.0f)] private float m_maxCharge = 1.0f;
        public float maxCharge => m_maxCharge;
        [SerializeField] [Min(0.0f)] private float m_minCharge = 0.3f;
        public float minCharge => m_minCharge;
        // Transform holding the position for where to spawn the projectile
        public Transform axe => m_axe;
        [SerializeField] [Required] private Transform m_axe = null;

        public AxeProjectile projectile => m_projectile;
        [SerializeField] [Required] private AxeProjectile m_projectile = null;

        [SerializeField] private float m_damageToDeal = 5.0f;
        [SerializeField] private float m_startingAngle = 30.0f;
        [SerializeField] private float m_targetChargeAngle = -60.0f;
        [SerializeField] [Required] private Transform m_scaleTrans = null;
        [SerializeField] private BetterCurve m_growCurve = null;
        [SerializeField] private BetterCurve m_swingCurve = null;
        [SerializeField] private BetterCurve m_shrinkCurve = null;

        [SerializeField, Required]
        private WwiseEventName m_swingAxeWwiseEventName = null;
        [SerializeField, Required]
        private WwiseEventName m_beginChargeAxeWwiseEventName = null;
        [SerializeField, Required]
        private WwiseEventName m_pauseChargeAxeWwiseEventName = null;
        [SerializeField, Required]
        private WwiseEventName m_resumeChargeAxeWwiseEventName = null;

        public float damageToDeal => m_damageToDeal;
        public float startingAngle => m_startingAngle;
        public float targetChargeAngle => m_targetChargeAngle;
        public Transform scaleTrans => m_scaleTrans;
        public BetterCurve growCurve => m_growCurve;
        public BetterCurve swingCurve => m_swingCurve;
        public BetterCurve shrinkCurve => m_shrinkCurve;

        public WwiseEventName swingAxeWwiseEventName => m_swingAxeWwiseEventName;
        public WwiseEventName beginChargeAxeWwiseEventName =>
            m_beginChargeAxeWwiseEventName;
        public WwiseEventName pauseChargeAxeWwiseEventName =>
            m_pauseChargeAxeWwiseEventName;
        public WwiseEventName resumeChargeAxeWwiseEventName =>
            m_resumeChargeAxeWwiseEventName;
    }
}
