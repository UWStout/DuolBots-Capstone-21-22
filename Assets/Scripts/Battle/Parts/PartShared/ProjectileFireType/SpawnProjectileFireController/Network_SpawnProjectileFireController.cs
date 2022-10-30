using UnityEngine;
using UnityEngine.Assertions;

using Mirror;
using System;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Networked implmentation of Turret2Axis's IWeaponFireController
    /// </summary>
    [RequireComponent(typeof(Specifications_SpawnProjectileFireController))]
    public class Network_SpawnProjectileFireController : NetworkChildBehaviour,
        IWeaponFireController, IPartShowsTrajectory, IFireEvent, IWwiseEventInvoker
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        // Prefab to spawn as a projectile on turret fire
        [SerializeField] private GameObject m_projectilePrefab = null;
        [SerializeField] private float m_projectileInitSpeed = 40.0f;
        [SerializeField] private bool m_projectileUsesGravity = true;

        private ITeamIndex m_teamIndex = null;
        private CooldownRemaining m_coolDownRemaining = null;
        // How long the current cooldown as left
        private float m_curCoolDown = 0.0f;

        private bool m_isFiring = false;
        public bool isFiring
        {
            set => m_isFiring = value;
            get => m_isFiring;
        }

        // Specifications for variables
        private Specifications_SpawnProjectileFireController
            m_specifications = null;

        // Componenet that calcuates parent velocity
        private TransformVelocityCalculator m_botRootVelocityCalculator = null;

        public event Action onFire;

        #region IPartShowsTrajectory
        public float initialSpeed => m_projectileInitSpeed;
        public bool shouldUseGravity => m_projectileUsesGravity;
        #endregion IPartShowsTrajectory

        #region IWwiseEventInvoker
        public event Action<WwiseEventName, GameObject> requestInvokeWwiseEvent;
        #endregion IWwiseEventInvoker


        public override void OnStartServer()
        {
            base.OnStartServer();

            m_coolDownRemaining = GetComponent<CooldownRemaining>();
            Assert.IsNotNull(m_coolDownRemaining, $"{nameof(m_coolDownRemaining)}" +
                $"was not found on {name} but is required.");
            Assert.IsNotNull(m_projectilePrefab, $"Projectile Prefab not " +
                $"specified for {name}'s {GetType().Name}");
            NetworkIdentity temp_prefabNetIdentity =
                m_projectilePrefab.GetComponent<NetworkIdentity>();
            Assert.IsNotNull(temp_prefabNetIdentity, $"Projectile Prefab did not " +
                $"have a {nameof(NetworkIdentity)} attached.");

            m_specifications =
                GetComponent<Specifications_SpawnProjectileFireController>();
            Assert.IsNotNull(m_specifications,
                $"{typeof(Specifications_Turret2Axis).Name} was not on {name} " +
                $"but is required by {GetType().Name}");

            m_botRootVelocityCalculator =
                GetComponentInParent<TransformVelocityCalculator>();
            Assert.IsNotNull(m_botRootVelocityCalculator,
                $"{typeof(TransformVelocityCalculator).Name} was not on {name} " +
                $"but is required by {GetType().Name}");

            m_teamIndex = GetComponentInParent<ITeamIndex>();
            CustomDebug.AssertIComponentInParentIsNotNull(m_teamIndex, this);
        }
        private void Update()
        {
            // Only server should do stuff
            if (!isServer) { return; }

            SpawnProjectile();
        }


        public void Fire(bool value, eInputType type)
        {
            isFiring = value;
            m_coolDownRemaining.inputType = type;
        }
        public void AlternateFire(bool value, eInputType type)
        { /*This controller does not utilize alternate firing.*/ }


        private void SpawnProjectile()
        {
            if (m_curCoolDown >= 0.0f)
            {
                m_curCoolDown -= Time.deltaTime;
                m_coolDownRemaining.UpdateCoolDown(m_specifications.coolDown, m_curCoolDown);
            }

            else if (isFiring && m_curCoolDown <= 0.0f)
            {
                CustomDebug.Log($"{name} has spawned a projectile", IS_DEBUGGING);
                onFire?.Invoke();
                // Play fire sound
                if (m_specifications.hasSpawnObjSound)
                {
                    requestInvokeWwiseEvent?.Invoke(
                        m_specifications.spawnObjWwiseEventName, gameObject);
                }

                Transform temp_spawnPosTrans = m_specifications.projectileSpawnPos;
                // Instantiate projectile
                GameObject temp_spawnedObject = Instantiate(m_projectilePrefab,
                    temp_spawnPosTrans.position, temp_spawnPosTrans.rotation);
                // Relay spawn over the network. Assumes the projectile prefab
                // has a NetworkIdentity on it.
                NetworkServer.Spawn(temp_spawnedObject);

                // Explicitly look for ITeamIndex
                PartImpactCollider temp_partImpactCollider =
                    temp_spawnedObject.GetComponent<PartImpactCollider>();
                Assert.IsNotNull(temp_partImpactCollider, $"{name}'s {GetType().Name} " +
                    $"expected {temp_spawnedObject.name} to have " +
                    $"{nameof(PartImpactCollider)} attached but none was found");
                temp_partImpactCollider.teamIndex = m_teamIndex.teamIndex;

                // Check if the projectile should inherit its parent's velocity
                if (m_specifications.inheritsParentsVel)
                {
                    if (!temp_spawnedObject.TryGetComponent(out
                        Rigidbody temp_rigidBody))
                    {
                        Debug.LogError($"Spawned projectile, " +
                            $"{temp_rigidBody.name} that is trying to inherit " +
                            $"velocity does not have a rigidbody to inherit " +
                            $"from.");
                        return;
                    }
                    temp_rigidBody.velocity +=
                        m_botRootVelocityCalculator.velocitySinceLastFrame;
                }

                // Reset cooldown
                m_curCoolDown = m_specifications.coolDown;
                m_coolDownRemaining.UpdateCoolDown(m_specifications.coolDown, m_curCoolDown);

                if (!m_specifications.autoFire) { return; }
            }
        }
    }
}
