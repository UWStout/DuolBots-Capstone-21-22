using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Script that holds the specifications and behavioral variables for the PaintBomb projectile
    /// </summary>
    public class Specifications_PaintBombFireController : MonoBehaviour
    {
        // The projectile being instantiated and fired
        [SerializeField] private GameObject m_projectilePrefab = null;
        public GameObject projectilePrefab => m_projectilePrefab;
        // The overlay Canvas that the projectile uses to affect the 
        [SerializeField] private GameObject m_splatterCanvasPrefab = null;
        public GameObject splatterCanvas => m_splatterCanvasPrefab;

        // Transform holding the position for where to spawn the projectile
        public Transform projectileSpawnPos => m_projectileSpawnPos;
        [SerializeField] [Required] private Transform m_projectileSpawnPos = null;

        // Whether the projectile will inherit its parent's velocity.
        [SerializeField] private bool m_inheritsParentsVel = true;
        public bool inheritsParentsVel => m_inheritsParentsVel;

        [SerializeField] private bool m_autoFire = false;
        public bool autoFire => m_autoFire;

        [SerializeField] private float m_coolDown = 0.0f;
        public float coolDown => m_coolDown;

    }
}
