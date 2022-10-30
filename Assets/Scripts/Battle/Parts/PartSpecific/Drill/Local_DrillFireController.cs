using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
// Original Author - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Drill's implementation of IWeaponFireController which does not fire a
    /// projectile but rather handles rotation and enables a damage hitbox
    /// </summary>
    [RequireComponent(typeof(DrillSoundManager))]
    [DisallowMultipleComponent]
    public class Local_DrillFireController : MonoBehaviour, IWeaponFireController
    {
        // Constants
        private const bool IS_DEBUGGING = false;
        private const float ROTATION_SPEED = 3f;

        private Specifications_DrillFireController m_specifications = null;
        private CooldownRemaining m_jabCoolDown = null;
        // Positions being manipulated by controller
        private Transform m_drillTransform = null;
        // Projectile and specifications containing behavioral variables
        private DrillProjectile m_drillProjectile = null;
        // Reference to the sound manager
        private DrillSoundManager m_soundMan = null;

        // Spinning variables
        private Coroutine m_spinCorout = null;
        private bool m_drillIsSpinning = false;

        // Jabbing variables
        private float m_maxJabCD = 5.0f;
        private float m_jabDuration = 3.0f;
        private bool m_drillIsJabbing = false;

        


        // Domestic Initialization
        private void Awake()
        {
            // Check for null componenets
            m_specifications = GetComponent<Specifications_DrillFireController>();
            m_jabCoolDown = GetComponent<CooldownRemaining>();
            m_soundMan = GetComponent<DrillSoundManager>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_specifications, this);
            CustomDebug.AssertComponentIsNotNull(m_jabCoolDown, this);
            CustomDebug.AssertComponentIsNotNull(m_soundMan, this);
            #endregion Asserts

            m_drillProjectile = m_specifications.drillProjectile;
            m_drillTransform = m_specifications.turretTransform;

            m_jabDuration = m_specifications.jabDuration;

            m_maxJabCD = m_specifications.jabDelay;

            #region Asserts
            Assert.IsNotNull(m_drillProjectile, $"{this.name} does not have a " +
                $"{nameof(m_drillProjectile)} but requires one.");
            Assert.IsNotNull(m_drillTransform, $"{this.name} does not have a " +
                $"{nameof(m_drillTransform)} but requires one.");
            #endregion Asserts
        }


        #region IWeaponFireController
        // Spin
        public void Fire(bool isPressed, eInputType type)
        {
            // Start spinning if pressed, stop spinning if released
            if (isPressed)
            {
                StartSpin();
            }
            else
            {
                StopSpin();
            }
        }
        // Jab
        public void AlternateFire(bool value, eInputType type)
        {
            #region Logs
            CustomDebug.LogForComponent($"{nameof(AlternateFire)}", this,
                IS_DEBUGGING);
            #endregion Logs
            m_jabCoolDown.inputType = type;
            if (value)
            {
                StartJab();
            }
        }
        #endregion IWeaponFireController


        private void StartSpin()
        {
            // Don't spin if already spinning
            if (m_drillIsSpinning) { return; }

            m_drillIsSpinning = true;
            m_spinCorout = StartCoroutine(DrillSpinning());
        }
        private void StopSpin()
        {
            // If the drill isn't spinning, there is nothing to stop
            if (!m_drillIsSpinning) { return; }

            m_drillIsSpinning = false;
            StopCoroutine(m_spinCorout);
            // Spin ended, don't allow the spin to deal damage anymore.
            m_drillProjectile.ResetSpinHit(false);
            // Update drill spin to be no longer spinning
            m_soundMan.UpdateSpinSound(false);
        }
        private IEnumerator DrillSpinning()
        {
            float temp_timer = 0.0f;
            // This Coroutine only ends when StopCoroutine is called.
            while (true)
            {
                // Rotate the drill
                m_drillTransform.Rotate(0, ROTATION_SPEED, 0);

                // Reset the drill if we've reached damage reset time.
                if (temp_timer >= m_specifications.dmgResetTime)
                {
                    m_drillProjectile.ResetSpinHit(true);
                    temp_timer = 0.0f;
                }

                // Update drill sound
                m_soundMan.UpdateSpinSound(true);

                yield return null;

                temp_timer += Time.deltaTime;
            }
        }

        /// <summary>
        /// Starts jabbing if the drill is not already jabbing and if it
        /// is not on cooldown.
        /// </summary>
        private void StartJab()
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(StartJab), this, IS_DEBUGGING);
            #endregion Logs
            // Don't jab if already jabbing or if the cooldown is not up
            if (m_drillIsJabbing) { return; }

            m_drillIsJabbing = true;

            // Allow the jab to hit
            m_drillProjectile.ResetJabHit(true);
            // Put the drill on cooldown (current value can't be max value,
            // but idk why).
            m_jabCoolDown.UpdateCoolDown(1.0f, 0.999f);
            // Play jab extend sound
            m_soundMan.BeginJabSound();

            StartJabExtend();
        }
        /// <summary>
        /// Start extending the drill for the jab.
        /// Calls <see cref="StartJabReset"/> after it has
        /// fully extended.
        /// </summary>
        private void StartJabExtend()
        {
            // Extend the drill and afterwords, start moving it back.
            StartCoroutine(MoveDrill(m_specifications.jabCurve, 0.0f,
                StartJabReset));
        }
        /// <summary>
        /// Starts moving the drill back to its default position.
        /// Waits the <see cref="m_jabDuration"/> before moving back.
        /// Calls <see cref="FinishJabbing"/> after its done.
        /// </summary>
        private void StartJabReset()
        {
            // Wait for a bit and then move the drill back
            StartCoroutine(MoveDrill(m_specifications.resetCurve, m_jabDuration,
                FinishJabbing));
        }
        /// <summary>
        /// Finishes jabbing by turning off the damaging collider and putting the
        /// drill on cooldown.
        /// </summary>
        private void FinishJabbing()
        {
            // Stop allowing the jab to deal damage
            m_drillProjectile.ResetJabHit(false);
            // Stop playing sound effect
            m_soundMan.StopJabSound();

            StartCoroutine(CountDownJabCoolDownCoroutine());
        }
        /// <summary>
        /// Coroutine to count down the jab's cooldown.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CountDownJabCoolDownCoroutine()
        {
            // Put the jab on cooldown
            float temp_curJabCD = m_maxJabCD;
            while (temp_curJabCD > 0.0f)
            {
                temp_curJabCD -= Time.deltaTime;
                m_jabCoolDown.UpdateCoolDown(m_maxJabCD, temp_curJabCD);
                yield return null;
            }
            temp_curJabCD = 0.0f;
            m_jabCoolDown.UpdateCoolDown(m_maxJabCD, temp_curJabCD);

            // Allow the drill to jab again
            m_drillIsJabbing = false;
        }
        /// <summary>
        /// Moves the drill along its local y position
        /// based on the given animation curve.
        /// </summary>
        /// <param name="animCurve">Curve that determines how the
        /// drill should be moved.</param>
        /// <param name="delayTime">Amount of time in seconds to wait before
        /// moving the drill.</param>
        /// <param name="onFinish">Action to call after finished moving
        /// the drill.</param>
        private IEnumerator MoveDrill(BetterCurve animCurve, float delayTime = 0.0f,
            Action onFinish = null)
        {
            yield return new WaitForSeconds(delayTime);

            // Extend the drill
            float t = 0.0f;
            float temp_endTime = animCurve.GetEndTime();
            float temp_curVal;
            while (t < temp_endTime)
            {
                temp_curVal = animCurve.Evaluate(t);
                Vector3 temp_localPos = m_drillTransform.localPosition;
                temp_localPos.y = temp_curVal;
                m_drillTransform.localPosition = temp_localPos;

                t += Time.deltaTime;
                yield return null;
            }

            onFinish?.Invoke();
        }
    }
}
