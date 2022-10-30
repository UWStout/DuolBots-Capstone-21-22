using System;
using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Script that holds shared functionality between local and networked variants
    /// of ChargeSpawnProjectileFireController
    /// </summary>
    [RequireComponent(typeof(Specifications_ChargeSpawnProjectileFireController))]
    public class Shared_ChargeSpawnProjectileFireController : MonoBehaviour,
        IWeaponFireController, IChargeWeapon, IPartShowsTrajectory, IObjectSpawner,
        IWwiseEventInvoker
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField] [Required] GameObject m_projectilePrefab = null;
        [SerializeField] [Range(0, 10)] int m_damageToDeal = 1;

        [SerializeField] private float m_projectileInitSpeed = 40.0f;
        [SerializeField] private bool m_projectileUsesGravity = true;

        private CooldownRemaining m_coolDownRemaining = null;
        private bool m_isCharging = false;
        private float m_curCharge = 0.0f;
        private Specifications_ChargeSpawnProjectileFireController
            m_specifications = null;
        private ITeamIndex m_teamIndex = null;
        private bool m_isFullyCharged = false;
        // Controller for the charging sound
        private ChargeSoundManager m_chargeSoundMan = null;

        public bool isCharging
        {
            set => m_isCharging = value;
            get => m_isCharging;
        }
        public float curCharge
        {
            get => m_curCharge;
            private set => SetCurrentCharge(value);
        }
        public Specifications_ChargeSpawnProjectileFireController specifications
            => m_specifications;

        // Events describing whether the weapon is currently charging
        public event Action onStartedCharging;
        public event Action onFinishedCharging;
        // Events for when the weapon reaches and breaks out of being fully charged
        // (firing the weapon after fully charging)
        public event Action onFullyChargedStart;
        public event Action onFullyChargedEnd;
        // Events for instantiation
        public event Action<GameObject> onProjectileSpawned;
        public event Action<GameObject> onObjectSpawned;

        #region IPartShowsTrajectory
        public float initialSpeed => m_projectileInitSpeed;
        public bool shouldUseGravity => m_projectileUsesGravity;
        #endregion IPartShowsTrajectory
        #region IWwiseEventInvoker
        public event Action<WwiseEventName, GameObject> requestInvokeWwiseEvent;
        #endregion IWwiseEventInvoker


        // Domestic Initialization
        private void Awake()
        {
            m_coolDownRemaining = GetComponent<CooldownRemaining>();
            m_specifications = GetComponent<
                Specifications_ChargeSpawnProjectileFireController>();
            m_teamIndex = GetComponentInParent<ITeamIndex>();

            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_coolDownRemaining, this);
            CustomDebug.AssertComponentIsNotNull(m_specifications, this);
            CustomDebug.AssertIComponentInParentIsNotNull(m_teamIndex, this);
            #endregion Asserts

            m_chargeSoundMan = new ChargeSoundManager(
                m_specifications.beginChargeWwiseEventName,
                m_specifications.pauseChargeWwiseEventName,
                m_specifications.resumeChargeWwiseEventName, gameObject,
                (WwiseEventName eventName, GameObject obj) =>
                requestInvokeWwiseEvent?.Invoke(eventName, obj));
        }
        private void Update()
        {
            UpdateCharge();
        }


        public void Fire(bool isPressed, eInputType type)
        {
            isCharging = isPressed;
            m_coolDownRemaining.inputType = type;
            if (isCharging)
            {
                onStartedCharging?.Invoke();
            }
            else
            {
                onFinishedCharging?.Invoke();
            }
        }

        public void AlternateFire(bool value, eInputType type) {
            /*This controller does not utilize alternate firing.*/}

        private void UpdateCharge()
        {
            // If the charge is being held down
            if (isCharging)
            {
                // If the current charge has not reached the max yet, increment it
                if (curCharge < m_specifications.maxCharge)
                {
                    // Start/resume playing charge sound.
                    m_chargeSoundMan.UpdateSound(true);
                    curCharge += Time.deltaTime;
                    return;
                }
                // If we've reached here, charging has finished.
                // Stop playing charge sound since we are fully charged.
                m_chargeSoundMan.UpdateSound(false);
                // Only invoke onFullyChargedStart the first time max charge has
                // been reached
                if (!m_isFullyCharged)
                {
                    // Play the optional fully charged sound
                    if (specifications.hasFullChargeLoopSound)
                    {
                        requestInvokeWwiseEvent?.Invoke(
                            specifications.beginFullChargeSoundLoopEventName,
                            gameObject);
                    }
                    m_isFullyCharged = true;
                    onFullyChargedStart?.Invoke();
                }
                // If autofire is off, wait for the button to be released
                if (!m_specifications.autoFire)
                {
                    return;
                }

                // Autofire is on, call FinishCharging() automatically
                // Reset m_isFullyCharged and invoke onFullyChargedEnd
                onFullyChargedEnd?.Invoke();
                m_isFullyCharged = false;

                FinishCharging();
            }
            // Button is no longer being held down
            // Else if autofire is off and the current charge has reached the
            // max charge,
            else if (curCharge >= m_specifications.maxCharge)
            {
                // Reset m_isFullyCharged and invoke onFullyChargedEnd
                onFullyChargedEnd?.Invoke();
                m_isFullyCharged = false;

                FinishCharging();
            }
            // If the charge is above the minimum charge (but has not reached max)
            // and partial charge is allowed, fire.
            else if (curCharge > specifications.minCharge &&
                m_specifications.allowPartialCharge)
            {
                // Reset m_isFullyCharged and invoke onFullyChargedEnd
                onFullyChargedEnd?.Invoke();
                m_isFullyCharged = false;

                FinishCharging(); 
            }
            // Button was not releaed at max charge so charge will slowly deplete
            else
            {
                // Stop playing charge sound since the button is no
                // longer being held down.
                m_chargeSoundMan.UpdateSound(false);

                // TODO Sounds wont work with this. Consider just cutting this
                // option, only horn uses it I think.
                // UPDATE Had to cut this.
                /*
                curCharge = Mathf.Max(curCharge - Time.deltaTime, 0);
                // Reset sound if we hit bottom of charge.
                if (curCharge <= 0)
                {
                    m_chargeSoundMan.Reset();
                }
                */
            }
        }
        /// <summary>
        /// Invokes the onTurretFinishedCharging event and resets the charge.
        /// </summary>
        private void FinishCharging()
        {
            SpawnProjectile();
            curCharge = 0.0f;
            // Play the fire sound
            requestInvokeWwiseEvent?.Invoke(m_specifications.fireWwiseEventName,
                gameObject);
            // Stop playing the optional fully charged sound
            if (specifications.hasFullChargeLoopSound)
            {
                requestInvokeWwiseEvent?.Invoke(
                    specifications.stopFullChargeSoundLoopEventName,
                    gameObject);
            }
            // Stop playing the sound and reset since we've finished charging
            m_chargeSoundMan.Reset();
        }
        private void SpawnProjectile()
        {
            Transform temp_spawnTrans = specifications.projectileSpawnPos;
            Vector3 temp_spawnPos = temp_spawnTrans.position;
            Quaternion temp_spawnRot = temp_spawnTrans.rotation;
            GameObject temp_spawnedProjectile = Instantiate(m_projectilePrefab,
                temp_spawnPos, temp_spawnRot);

            if (temp_spawnedProjectile.TryGetComponent(out ISpawnedChargeProjectile
                temp_chargeProjectileInterface))
            {
                temp_chargeProjectileInterface.SetCharge(curCharge);
            }

            ITeamIndexSetter temp_partImpactCol =
                temp_spawnedProjectile.GetComponentInChildren<ITeamIndexSetter>();
            if (temp_partImpactCol != null)
            {
                CustomDebug.LogForComponent($"Found an object spawned with " +
                    $"{nameof(ITeamIndexSetter)}: + {temp_spawnedProjectile}",
                    this, IS_DEBUGGING);
                temp_partImpactCol.teamIndex = m_teamIndex.teamIndex;
            }
            else
            {
                CustomDebug.LogWarning($"{name}'s {GetType().Name} " +
                    $"expected {typeof(ITeamIndexSetter)} to be attached to " +
                    $"{temp_spawnedProjectile.name} or its children.");
            }           

            onProjectileSpawned?.Invoke(temp_spawnedProjectile);
            onObjectSpawned?.Invoke(temp_spawnedProjectile);
        }


        /// <summary>
        /// Sets the current charge and updates the cooldown to reflect that.
        /// </summary>
        private void SetCurrentCharge(float newCharge)
        {
            m_curCharge = newCharge;
            m_coolDownRemaining.UpdateCoolDown(m_specifications.maxCharge,
                curCharge);
        }
    }
}
