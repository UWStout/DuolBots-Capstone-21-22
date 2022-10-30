using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using NaughtyAttributes;
// Original authors - Aaron Duffey, Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Specifications for spawning a projectile.
    /// </summary>
    public class Specifications_SpawnProjectileFireController : MonoBehaviour,
        IPartTrajectorySpecs
    {
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

        [SerializeField] private bool m_hasSpawnObjSound = false;
        [SerializeField, ShowIf(nameof(m_hasSpawnObjSound)), Required]
        private WwiseEventName m_spawnObjWwiseEventName = null;

        public bool hasSpawnObjSound => m_hasSpawnObjSound;
        public WwiseEventName spawnObjWwiseEventName => m_spawnObjWwiseEventName;
    }
}
