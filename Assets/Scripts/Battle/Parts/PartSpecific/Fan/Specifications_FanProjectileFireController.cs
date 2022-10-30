using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Specifications and behavioral variables for the FanProjectileFireController
    /// </summary>
    public class Specifications_FanProjectileFireController : MonoBehaviour
    {
        // The range of power the force for the Fan weapon.
        [SerializeField] [Min(0.0f)] private float m_minFanForce = 10.0f;
        public float minFanForce => m_minFanForce;
        [SerializeField] private float m_maxFanForce = 50.0f;
        public float maxFanForce => m_maxFanForce;
        [SerializeField] [Min(1.0f)] private float m_maxCharge = 1.0f;
        public float maxCharge => m_maxCharge;
        [SerializeField] [Min(0.0f)] private float m_minCharge = 0.3f;
        public float minCharge => m_minCharge;
        [SerializeField] private float m_chargeRate = 0.1f;
        public float chargeRate => m_chargeRate;
        // Transform holding the position for where to spawn the projectile
        public Transform projectileSpawnPos => m_projectileSpawnPos;
        [SerializeField] [Required] private Transform m_projectileSpawnPos = null;

        // Whether the projectile will inherit its parent's velocity.
        [SerializeField] private bool m_inheritsParentsVel = true;
        public bool inheritsParentsVel => m_inheritsParentsVel;
        // Whether the weapon is allowed to fire without reaching max charge
        [SerializeField] private bool m_allowsPartialCharge = false;
        public bool allowPartialCharge => m_allowsPartialCharge;
        // Whether the weapon will fire automatically when the button is held or fire on button release.
        [SerializeField] private bool m_autoFire = false;
        public bool autoFire => m_autoFire;

        [SerializeField, Required] private WwiseEventName m_beginFanEventName = null;
        [SerializeField, Required] private WwiseEventName m_stopFanEventName = null;

        public WwiseEventName beginFanEventName => m_beginFanEventName;
        public WwiseEventName stopFanEvetnName => m_stopFanEventName;
    }
}
