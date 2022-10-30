using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Script that holds specifications and behavioral variables for MeleeFireControllers.
    /// </summary>
    public class Specifications_MeleeProjectile : MonoBehaviour
    {
        // Whether the projectile will inherit its parent's velocity.
        [SerializeField] private bool m_inheritsParentsVel = true;
        public bool inheritsParentsVel => m_inheritsParentsVel;
        // Whether the weapon is allowed to fire without reaching max charge
        [SerializeField] private bool m_allowsPartialCharge = false;
        private bool m_autoFire = false;
        public bool autoFire => m_autoFire;
        // How long the weapon must wait between firing.
        [SerializeField] private float m_coolDown = 1.0f;
        public float coolDown => m_coolDown;
    }
}
