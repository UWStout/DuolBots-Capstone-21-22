using System;
using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Origianl Author - Aaron Duffey
// Modified by Wyatt Senalik (Mar. 31 2022)

namespace DuolBots
{
    /// <summary>
    /// Script that handles spawning projectiles for the BubbleBlaster
    /// (randomly spawns projectiles in a set of Transforms and ranomizes
    /// their movement).
    ///
    /// This is both the local and network version (network version just also
    /// needs the Network_ObjectSpawner and NetworkTransforms).
    /// </summary>
    [RequireComponent(typeof(Specifications_SpawnProjectileFireController))]
    public class Local_BubbleBlasterFireController : MonoBehaviour, IWeaponFireController,
        IObjectSpawner, IPartShowsTrajectory, IWwiseEventInvoker
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField] private Transform[] m_spawnPositions = null;
        private Transform m_lastUsedSpawnPosition = null;
        [SerializeField] [Required]
        private GameObject m_bubbleProjectilePrefab = null;
        [SerializeField] [Required] private Transform m_barrelToSpin = null;
        [SerializeField] private float m_barrelRotSpeed = 2.0f;

        [SerializeField] private float m_projectileInitSpeed = 40.0f;
        [SerializeField] private bool m_projectileUsesGravity = false;

        private CooldownRemaining m_coolDownRemaining = null;
        private Specifications_SpawnProjectileFireController m_specifications = null;
        private ITeamIndex m_teamIndex = null;
        private bool m_isFiring = false;
        private float m_curCoolDown = 0.0f;
        private Vector3 m_barrelEulers = Vector3.zero;

        public event Action<GameObject> onObjectSpawned;

        #region IPartShowsTrajectory
        public float initialSpeed => m_projectileInitSpeed;
        public bool shouldUseGravity => m_projectileUsesGravity;
        #endregion IPartShowsTrajectory
        #region IWwiseEventInvoker
        public event Action<WwiseEventName, GameObject> requestInvokeWwiseEvent;
        #endregion IWwiseEventInvoker


        private void Awake()
        {
            m_coolDownRemaining = GetComponent<CooldownRemaining>();
            // Check for null componenets
            Assert.IsNotNull(m_coolDownRemaining, $"{name} does not have an attached {nameof(m_coolDownRemaining)} but requires one.");
            Assert.IsNotNull(m_spawnPositions, $"{this.name} does not have a spawn position specified but requires one.");
            Assert.IsNotNull(m_bubbleProjectilePrefab, $"{this.name} does not have a projectile prefab {typeof(GameObject)} specified but requires one.");
            m_specifications = GetComponent<Specifications_SpawnProjectileFireController>();
            Assert.IsNotNull(m_specifications, $"{this.name} does not have a {typeof(Specifications_SpawnProjectileFireController)} but requires one.");

            m_teamIndex = GetComponentInParent<ITeamIndex>();
            CustomDebug.AssertIComponentIsNotNull(m_teamIndex, this);

            CustomDebug.AssertSerializeFieldIsNotNull(m_barrelToSpin,
                nameof(m_barrelToSpin), this);
            m_barrelEulers = m_barrelToSpin.localEulerAngles;
        }

        private void Update()
        {
            if (m_specifications.autoFire)
            {
                SpawnProjectile();
            }
            SpinBarrel();
        }

        public void Fire(bool value, eInputType type)
        {
            m_isFiring = value;
            if(!m_specifications.autoFire && m_isFiring)
            {
                SpawnProjectile();
            }
        }

        public void AlternateFire(bool value, eInputType type) { /*This controller does not utilize alternate firing.*/}

        private void SpawnProjectile()
        {
            if (!m_isFiring) { return; }

            // Check if current cooldown has not reached 0
            if (m_curCoolDown > 0.0f)
            {
                m_curCoolDown -= Time.deltaTime;
                m_coolDownRemaining.UpdateCoolDown(m_specifications.coolDown, m_curCoolDown);
                return;
            }

            // Randomize spawn position
            int temp_randPos = UnityEngine.Random.Range(0, m_spawnPositions.Length - 1);
            if (m_lastUsedSpawnPosition != null)
            {
                while (m_lastUsedSpawnPosition == m_spawnPositions[temp_randPos])
                {
                    temp_randPos = UnityEngine.Random.Range(0, m_spawnPositions.Length - 1);
                }
            }
            Transform temp_randSpawnPos = m_spawnPositions[temp_randPos];
            m_lastUsedSpawnPosition = temp_randSpawnPos;

            // Instantiate BubbleBlaster projectile
            GameObject temp_projectile = Instantiate(m_bubbleProjectilePrefab,
                temp_randSpawnPos.position, temp_randSpawnPos.rotation);
            onObjectSpawned?.Invoke(temp_projectile);

            // Set the part impact collider's team index
            PartImpactCollider temp_partImpactCol =
                temp_projectile.GetComponentInChildren<PartImpactCollider>();
            temp_partImpactCol.teamIndex = m_teamIndex.teamIndex;


            // Reset current cooldown
            m_curCoolDown = m_specifications.coolDown;
            m_coolDownRemaining.UpdateCoolDown(m_specifications.coolDown, m_curCoolDown);
        }
        private void SpinBarrel()
        {
            if (!m_isFiring) { return; }

            // Rotate the barrel
            m_barrelEulers.y += m_barrelRotSpeed * Time.deltaTime;
            m_barrelEulers = AngleHelpers.ClampAnglesAlongRotationAxis(
                m_barrelEulers, eRotationAxis.y, -180.0f, 180.0f);
            m_barrelToSpin.localEulerAngles = m_barrelEulers;
        }
    }
}
