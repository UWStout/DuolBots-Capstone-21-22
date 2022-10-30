using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Script that holds specifications and behavior variables for HandShield weapon.
    /// </summary>
    public class Specifications_HandShield : MonoBehaviour
    {
        [SerializeField] private Transform m_handShieldTransform = null;
        public Transform handShieldTransform => m_handShieldTransform;
        [SerializeField] private GameObject m_projectile = null;
        public GameObject projectile => m_projectile;
        [SerializeField] private Transform m_spawnPosition = null;
        public Transform spawnPosition => m_spawnPosition;
        [SerializeField] private float m_duration = 1.25f;
        public float duration => m_duration;
        [SerializeField] private float m_cooldown = 3.0f;
        public float cooldown => m_cooldown;
    }
}
