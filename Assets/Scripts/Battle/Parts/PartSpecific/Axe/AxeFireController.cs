using System;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
// Original Authors - Aaron Duffey (Adaptation of Shared_ChargeSpawnProjectileFireController)

namespace DuolBots
{
    /// <summary>
    /// Script that holds functionality for Axe weapon
    /// </summary>
    [RequireComponent(typeof(Specifications_AxeFireController))]
    public class AxeFireController : MonoBehaviour, IWeaponFireController,
        IChargeWeapon, IWwiseEventInvoker
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        // Holds and updates the cooldown of the weapon
        private CooldownRemaining m_coolDownRemaining = null;
        
        // Transforms and GameObjects being manipulated
        private Transform m_axe = null;
        private AxeProjectile m_axeProjectile = null;

        // The ending rotation after an Axe swing
        private float m_endRotation = 120.0f;
        private float m_curRotation = 0.0f;

        // States of the Axe weapon
        private bool m_axeIsFiring = false;
        private bool m_isCoolingDown = false;
        private bool m_isFullyCharged = false;
        private bool m_isCharging = false;

        // State of the charging sound
        private ChargeSoundManager m_chargeSoundMan = null;

        public bool isCharging
        {
            private set => m_isCharging = value;
            get => m_isCharging;
        }
        private float m_curCharge = 0.0f;
        // Specifications holding the behavioral variables for the AxeFireController
        private Specifications_AxeFireController m_specifications = null;
        public Specifications_AxeFireController specifications => m_specifications;

        public event Action onStartedCharging;
        public event Action onFinishedCharging;
        public event Action onFullyChargedStart;
        public event Action onFullyChargedEnd;
        #region IWwiseEventInvoker
        public event Action<WwiseEventName, GameObject> requestInvokeWwiseEvent;
        #endregion IWwiseEventInvoker

        private void Awake()
        {
            // Domestic Initialization
            m_coolDownRemaining = GetComponent<CooldownRemaining>();

            m_specifications = GetComponent<Specifications_AxeFireController>();
            Assert.IsNotNull(m_specifications, $"{nameof(Specifications_AxeFireController)}" +
                $" was not found on {this.name} but is required.");
            // Initialize variables using Specifications_AxeFireController
            if(m_specifications != null)
            {
                m_axe = m_specifications.axe;
                m_axeProjectile = m_specifications.projectile;
                Assert.IsNotNull(m_axeProjectile, $"No projectile {nameof(m_axeProjectile)} is specified but is required.");
            }

            m_chargeSoundMan = new ChargeSoundManager(
                m_specifications.beginChargeAxeWwiseEventName,
                m_specifications.pauseChargeAxeWwiseEventName,
                m_specifications.resumeChargeAxeWwiseEventName,
                gameObject, (WwiseEventName eventName, GameObject obj) =>
                requestInvokeWwiseEvent?.Invoke(eventName, obj));
        }
        private void OnEnable()
        {
            #region Logs
            CustomDebug.LogForComponent($"{nameof(OnEnable)}", this, IS_DEBUGGING);
            #endregion Logs
        }
        private void OnDisable()
        {
            #region Logs
            CustomDebug.LogForComponent($"{nameof(OnDisable)}", this, IS_DEBUGGING);
            #endregion Logs
        }
        private void Start()
        {
            // Foreign Initialization
            m_curCharge = m_specifications.minCharge;
            ResetAxeRotation();

            m_axeProjectile.SetDamageToDeal(m_specifications.damageToDeal);
            m_axeProjectile.ToggleCollider(false);
        }
        private void Update()
        {
            CustomDebug.LogForComponent($"{nameof(Update)}", this, IS_DEBUGGING);
            UpdateCharge();
        }


        public void Fire(bool isPressed, eInputType type)
        {
            #region Logs
            CustomDebug.LogForComponent($"{nameof(Fire)}: {nameof(isPressed)}=" +
                $"{isPressed}, {nameof(type)}={type}", this, IS_DEBUGGING);
            #endregion Logs
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
        public void AlternateFire(bool value, eInputType type) { /*This controller does not utilize alternate firing.*/}


        private void UpdateCharge()
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(UpdateCharge), this, IS_DEBUGGING);
            #endregion Logs
            // Can't do anything is on cooldown, or the axe is currently swinging
            if (m_isCoolingDown || m_axeIsFiring) { return; }
            // If the charge is being held down
            if (isCharging)
            {
                #region Logs
                CustomDebug.LogForComponent($"isCharging", this, IS_DEBUGGING);
                #endregion Logs
                if (m_curRotation > m_specifications.targetChargeAngle)
                {
                    // Normalize the charge to be between 0 and 1 where 0 is
                    // not at all charged and 1 is fully charged.
                    float temp_nCharge = GetNormalizedCharge();
                    // Base the rotation on the amount of charge so that
                    // when you are fully charged, your rotation is fully
                    // at the top.
                    float temp_angleDiff = m_specifications.targetChargeAngle
                        - m_specifications.startingAngle;
                    float temp_curRot = temp_angleDiff * temp_nCharge +
                        m_specifications.startingAngle;

                    AlterAxeRotation(temp_curRot);
                }

                // If the current charge has not reached the max yet, end here.
                if (m_curCharge < m_specifications.maxCharge)
                {
                    // Resume playing charge sound
                    m_chargeSoundMan.UpdateSound(true);

                    m_curCharge += Time.deltaTime;
                    return;
                }
                else if(!m_isFullyCharged)
                {
                    m_isFullyCharged = true;
                    onFullyChargedStart?.Invoke();

                    // Stop playing charge sound
                    // It is fully charged now, so reset that the charge sound
                    // was started.
                    m_chargeSoundMan.Reset();
                }
            }
            // Button is no longer being held down
            else 
            {
                #region Logs
                CustomDebug.LogForComponent($"Not charging", this, IS_DEBUGGING);
                #endregion Logs
                // Stop playing charge sound
                m_chargeSoundMan.UpdateSound(false);

                // Else if the current charge has reached the max charge
                if (m_curCharge >= m_specifications.maxCharge)
                {
                    m_isFullyCharged = false;
                    onFullyChargedEnd?.Invoke();
                    FinishCharging();

                    // It is fully charged now, so reset that the charge sound
                    // was started.
                    m_chargeSoundMan.Reset();
                }
            }
        }
        /// <summary>
        /// Invokes the onTurretFinishedCharging event and resets the charge.
        /// </summary>
        private void FinishCharging()
        {
            m_axeIsFiring = true;
            // Reset charge and snap to the target rotation (where the axe will start its swing from).
            m_curCharge = m_specifications.minCharge;
            m_curRotation = m_specifications.targetChargeAngle;
            // Turn the damaging collider on for the Axe projectile
            m_axeProjectile.ToggleCollider(true);
            StartGrow();
        }
        /// <summary>
        /// Starts scaling up the Axe weapon before it swings to attack.
        /// </summary>
        private void StartGrow()
        {
            StartCoroutine(ChangeScaleCoroutine(m_specifications.growCurve,
                StartSwing));
        }
        private IEnumerator ChangeScaleCoroutine(BetterCurve animCurve,
             Action onFinished=null)
        {
            Transform temp_scaleTrans = m_specifications.scaleTrans;

            float t = 0.0f;
            float temp_endTime = animCurve.GetEndTime();
            float temp_curVal;
            Vector3 temp_newScale;
            // While the duration of the grow time has not been reached, evaluate
            // the BetterCurve at the given time and changes the scale to that value.
            while (t < temp_endTime)
            {
                temp_curVal = animCurve.Evaluate(t);
                temp_newScale = new Vector3(temp_curVal, temp_curVal, temp_curVal);
                temp_scaleTrans.localScale = temp_newScale;

                yield return null;
                t += Time.deltaTime;
            }
            // Growth has finished, snap to the final scale.
            temp_curVal = animCurve.Evaluate(temp_endTime);
            temp_newScale = new Vector3(temp_curVal, temp_curVal, temp_curVal);
            temp_scaleTrans.localScale = temp_newScale;

            onFinished?.Invoke();
        }
        private void StartSwing()
        {
            requestInvokeWwiseEvent?.Invoke(m_specifications.swingAxeWwiseEventName,
                gameObject);
            StartCoroutine(SwingCoroutine());
        }
        private IEnumerator SwingCoroutine()
        {
            BetterCurve temp_swingCurve = m_specifications.swingCurve;

            float t = 0.0f;
            float temp_endTime = temp_swingCurve.GetEndTime();
            // Set the rotation to the evaluated value of the given duration on the BetterCurve.
            while (t < temp_endTime)
            {
                AlterAxeRotation(temp_swingCurve.Evaluate(t));

                yield return null;
                t += Time.deltaTime;
            }
            AlterAxeRotation(temp_swingCurve.Evaluate(temp_endTime));

            StartShrink();
        }
        private void StartShrink()
        {
            StartCoroutine(ChangeScaleCoroutine(m_specifications.shrinkCurve,
                EndSwing));
        }
        private void EndSwing()
        {
            m_axeIsFiring = false;

            ResetAxeRotation();

            m_curCharge = m_specifications.minCharge;

            // Turning off the damaging collider after the swing finishes.
            m_axeProjectile.ToggleCollider(false);
        }
        private void ResetAxeRotation()
        {
            AlterAxeRotation(m_specifications.startingAngle);
        }
        private void AlterAxeRotation(float newEulerAngle)
        {
            Vector3 temp_newAngles = m_axe.localEulerAngles;
            temp_newAngles.z = newEulerAngle;
            m_axe.localEulerAngles = temp_newAngles;

            m_curRotation = newEulerAngle;
        }
        /// <summary>
        /// Returns the charge after normalizing between 0 and 1.
        /// 0 is not at all charged. 1 is fully charged.
        /// </summary>
        private float GetNormalizedCharge()
        {
            return (m_curCharge - m_specifications.minCharge) /
                (m_specifications.maxCharge - m_specifications.minCharge);
        }
    }
}
