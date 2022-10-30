using System;
using UnityEngine;
using UnityEngine.Assertions;
using NaughtyAttributes;

using DuolBots.Mirror;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Script that controls the firing of Fan weapon.
    /// </summary>
    [RequireComponent(typeof(Specifications_FanProjectileFireController))]
    public class FanProjectileFireController : NetworkChildBehaviour, IWeaponFireController,
        IObjectSpawner, IWwiseEventInvoker
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        [SerializeField] private Collider m_areaOfEffect = null;
        [SerializeField] private FanHeadRotator m_fanRotator = null;
        [SerializeField] private Specifications_FanProjectileFireController m_specifications = null;
        [SerializeField] private FanProjectile m_projectile = null;
        // Projectile that holds the VFX for the Fan's wind (NOTE: this projectile gets instantiated!)
        [SerializeField] private GameObject m_vfxProjectile = null;
        [SerializeField] private float m_instantiationDelay = 0.7f;

        private float m_curDelay = 0.0f;
        private eInputType m_inputType = eInputType.buttonEast;
        // Charge variables
        private bool m_isCharging = false;
        // Whether the Fan is currently above the minimum charge and is currently firing
        private bool m_isFiring = false;
        private float m_charge;
        public float charge { get => m_charge; set => m_charge = value; }
        private float m_chargeRate = 0.1f;
        // Multiplier for the charge that is used to calculate force at a given charge
        private float m_forceChargeMultiplier = 1f;
        // Power of force applied to the affected objects
        private float m_maxFanForce = 50f;
        public float maxFanForce { get { return m_maxFanForce; } set { m_maxFanForce = value; } }
        private float m_minFanForce = 20f;
        public float minFanForce { get { return m_minFanForce; } set { m_minFanForce = value; } }
        private float m_curFanForce = 0f;

        private bool m_isPlayingFanSound = false;

        // Charging events
        public event Action onStartedCharging;
        public event Action onFinishedCharging;

        #region IObjectSpawner
        public event Action<GameObject> onObjectSpawned;
        #endregion IObjectSpawner
        #region IWwiseEventInvoker
        public event Action<WwiseEventName, GameObject> requestInvokeWwiseEvent;
        #endregion IWwiseEventInvoker


        protected override void Awake()
        {
            base.Awake();

            #region Asserts
            Assert.IsNotNull(m_areaOfEffect, $"{name} does not have a " +
                $"{typeof(Collider)} specified but requires one.");
            Assert.IsNotNull(m_fanRotator, $"{name} does not have a fan head " +
                $"{typeof(FanHeadRotator)} specified but requires one.");
            Assert.IsNotNull(m_specifications, $"{name} does not have a " +
                 $"{typeof(Specifications_FanProjectileFireController)} specified but requires one.");
            Assert.IsNotNull(m_areaOfEffect, $"{name} does not have a " +
                $"{typeof(FanProjectile)} specified but requires one.");
            Assert.IsNotNull(m_vfxProjectile, $"{name} does not have a " +
                $"VFX projectile prefab {typeof(GameObject)} but requires one.");
            #endregion

            m_curDelay = m_instantiationDelay;
        }
        // Foreign Initialization
        private void Start()
        {
            if (m_specifications != null)
            {
                m_minFanForce = m_specifications.minFanForce;
                m_maxFanForce = m_specifications.maxFanForce;
                m_chargeRate = m_specifications.chargeRate;
                m_forceChargeMultiplier = m_maxFanForce / m_chargeRate;
            }
        }
        // Update is called once per frame
        private void Update()
        {
            if (!isServer) { return; }

            UpdateCharge(m_isCharging);
            UpdateForce(m_charge);
            UpdateRotationSpeed(m_charge);
            m_areaOfEffect.enabled = m_isCharging;
        }


        public void Fire(bool value, eInputType type)
        {
            m_isCharging = value;
            m_inputType = type;

            if (value)
            {
                BeginFanSound();
            }
            else
            {
                StopFanSound();
            }
        }
        public void AlternateFire(bool value, eInputType type) { /*This controller does not utilize alternate firing.*/}

        #region UpdateFunctions
        private void UpdateCharge(bool value)
        {
            // Fan should only firing if it is above the minimum charge
            m_isFiring = m_charge > m_specifications.minCharge;

            if (m_curDelay > 0.0f) { m_curDelay -= Time.deltaTime; }
            else
            {
                // Instantiate VFX Projectile even if fire is not being held until charge depletes below minimum.
                if (m_isFiring)
                {
                    SpawnVFXProjectile();
                    m_curDelay = m_instantiationDelay;
                }
            }

            if (value)
            {
                // Charge is incremented if it below the maximum charge, on StartedCharging is called
                // when the minimum charge has been reached.
                if (m_charge < m_specifications.maxCharge)
                {
                    m_charge += m_chargeRate;

                    if(m_isFiring && m_charge > m_specifications.minCharge)
                    {
                        m_isFiring = true;
                        onStartedCharging?.Invoke();
                    }
                }
                else
                {
                    m_charge = m_specifications.maxCharge;
                }
            }
            else
            {
                // Deplete remaining charge if the fire button is not held.
                if(m_charge > 0f)
                {
                    m_charge -= m_chargeRate;
                    // Finish charging if the charge reached 0 after depleting.
                    if (m_charge <= 0f)
                    {
                        onFinishedCharging?.Invoke();
                    }
                }
                onFinishedCharging?.Invoke();
            }
        }
        private void UpdateForce(float charge)
        {
            // Set the FanProjectile's force equal to the charge * the given force charge multiplier.
            m_curFanForce = charge * m_forceChargeMultiplier;
            Mathf.Clamp(m_curFanForce, m_minFanForce, m_maxFanForce);
            m_projectile.fanForce = m_curFanForce;
            CustomDebug.Log($"{name}'s force is {m_curFanForce}, the charge was {charge} " +
                $"and the force multiplier was {m_forceChargeMultiplier}", IS_DEBUGGING);
        }

        /// <summary>
        /// Updates the rotation speed of the FanHeadRotator.
        /// Pre-Condition: Assumes max charge on specifications is a positive value.
        /// Post-Condition: Sets the rotation speed to the current percentage of charge.
        /// </summary>
        /// <param name="charge">Current charge of FanProjectileFireController</param>
        private void UpdateRotationSpeed(float charge)
        {
            m_fanRotator.SetRotationSpeed(charge / m_specifications.maxCharge);
        }

        #endregion
        private void SpawnVFXProjectile()
        {
            GameObject temp_vfxProjectile = Instantiate(m_vfxProjectile,
                m_specifications.projectileSpawnPos.position,
                m_specifications.projectileSpawnPos.rotation, null);

            onObjectSpawned?.Invoke(temp_vfxProjectile);

            ParticleSystemsOnInstantiatedFanFire temp_vfxHandler =
                temp_vfxProjectile.GetComponent<ParticleSystemsOnInstantiatedFanFire>();
            #region Asserts
            Assert.IsNotNull(temp_vfxHandler, $"{temp_vfxProjectile.name} " +
                $"did not have an attached " +
                $"{temp_vfxHandler.GetType()} on instantiation but requires one.");
            #endregion Asserts
            temp_vfxHandler.SetPlayBackSpeed(m_charge / m_specifications.maxCharge);
        }
        private void BeginFanSound()
        {
            if (m_isPlayingFanSound) { return; }
            requestInvokeWwiseEvent?.Invoke(m_specifications.beginFanEventName,
                gameObject);
            m_isPlayingFanSound = true;
        }
        private void StopFanSound()
        {
            if (!m_isPlayingFanSound) { return; }
            requestInvokeWwiseEvent?.Invoke(m_specifications.stopFanEvetnName,
                gameObject);
            m_isPlayingFanSound = false;
        }
    }
}
