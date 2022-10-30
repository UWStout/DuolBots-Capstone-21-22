using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
// Original Authors- Aaron Duffey, Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Script that holds specifications and behavioral variables for ChargeSpawnProjectileFireControllers
    /// </summary>
    public class Specifications_ChargeSpawnProjectileFireController : MonoBehaviour, IPartTrajectorySpecs
    {
        [SerializeField] [Min (0.0f)] private float m_maxCharge = 1.0f;
        public float maxCharge => m_maxCharge;
        [SerializeField] [Min(0.0f)] private float m_minCharge = 0.3f;
        public float minCharge => m_minCharge;
        // Transform holding the position for where to spawn the projectile
        public Transform projectileSpawnPos => m_projectileSpawnPos;
        [SerializeField] [Required] private Transform m_projectileSpawnPos = null;

        // Whether the weapon is allowed to fire without reaching max charge
        [SerializeField] private bool m_allowsPartialCharge = false;
        public bool allowPartialCharge => m_allowsPartialCharge;
        // Whether the weapon will fire automatically when the button is held or fire on button release.
        [SerializeField] private bool m_autoFire = false;
        public bool autoFire => m_autoFire;

        [SerializeField, Required]
        private WwiseEventName m_beginChargeWwiseEventName = null;
        [SerializeField, Required]
        private WwiseEventName m_pauseChargeWwiseEventName = null;
        [SerializeField, Required]
        private WwiseEventName m_resumeChargeWwiseEventName = null;
        [SerializeField, Required]
        private WwiseEventName m_fireWwiseEventName = null;
        [SerializeField] private bool m_hasFullChargeLoopSound = false;
        [SerializeField, ShowIf(nameof(m_hasFullChargeLoopSound)), Required]
        private WwiseEventName m_beginFullChargeSoundLoopEventName = null;
        [SerializeField, ShowIf(nameof(m_hasFullChargeLoopSound)), Required]
        private WwiseEventName m_stopFullChargeSoundLoopEventName = null;

        public WwiseEventName beginChargeWwiseEventName
            => m_beginChargeWwiseEventName;
        public WwiseEventName pauseChargeWwiseEventName
            => m_pauseChargeWwiseEventName;
        public WwiseEventName resumeChargeWwiseEventName
            => m_resumeChargeWwiseEventName;
        public WwiseEventName fireWwiseEventName => m_fireWwiseEventName;
        public bool hasFullChargeLoopSound => m_hasFullChargeLoopSound;
        public WwiseEventName beginFullChargeSoundLoopEventName
            => m_beginFullChargeSoundLoopEventName;
        public WwiseEventName stopFullChargeSoundLoopEventName
            => m_stopFullChargeSoundLoopEventName;
    }
}
