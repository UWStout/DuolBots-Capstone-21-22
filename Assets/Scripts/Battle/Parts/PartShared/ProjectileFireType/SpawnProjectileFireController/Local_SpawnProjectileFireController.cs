using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik, Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Local version of SpawnProjectileFireController that implements
    /// IWeaponFireController.
    /// </summary>
    [RequireComponent(typeof(Specifications_Turret2Axis))]
    public class Local_SpawnProjectileFireController : MonoBehaviour,
        IWeaponFireController, IPartShowsTrajectory
    {
        // Prefab to spawn as a projectile on turret fire
        [SerializeField] private GameObject m_projectilePrefab = null;
        [SerializeField] private float m_projectileInitSpeed = 40.0f;
        [SerializeField] private bool m_projectileUsesGravity = true;

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

        private ITeamIndex m_teamIndex = null;

        #region IPartShowsTrajectory
        public float initialSpeed => m_projectileInitSpeed;
        public bool shouldUseGravity => m_projectileUsesGravity;
        #endregion IPartShowsTrajectory


        // Domestic initialization
        private void Awake()
        {
            m_coolDownRemaining = GetComponent<CooldownRemaining>();
            Assert.IsNotNull($"{name} does not have a {nameof(m_coolDownRemaining)} but requires one.");

            Assert.IsNotNull(m_projectilePrefab, $"Projectile Prefab not " +
                $"specified for {name}'s {GetType().Name}");

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
            CustomDebug.AssertIComponentIsNotNull(m_teamIndex, this);
        }

        private void Update()
        {
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
            if (m_curCoolDown >= 0.0f) {
                m_curCoolDown -= Time.deltaTime;
                m_coolDownRemaining.UpdateCoolDown(m_specifications.coolDown, m_curCoolDown);
            }
            else if (isFiring && m_curCoolDown <= 0.0f)
            {
                // Instantiate projectile
                GameObject temp_spawnedObject =
                    Instantiate(m_projectilePrefab,
                    m_specifications.projectileSpawnPos.position,
                    m_specifications.projectileSpawnPos.rotation);

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
                PartImpactCollider temp_partImpactCollider =
                    temp_spawnedObject.GetComponent<PartImpactCollider>();
                Assert.IsNotNull(temp_partImpactCollider, $"{name}'s {GetType().Name} " +
                    $"expected {temp_spawnedObject.name} to have " +
                    $"{nameof(PartImpactCollider)} attached but none was found");
                temp_partImpactCollider.teamIndex = m_teamIndex.teamIndex;

                // Reset cooldown
                m_curCoolDown = m_specifications.coolDown;
                m_coolDownRemaining.UpdateCoolDown(m_specifications.coolDown, m_curCoolDown);


                if (!m_specifications.autoFire) { return; }
            }
        }
    }
}
