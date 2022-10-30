using System;
using UnityEngine;
using UnityEngine.VFX;
// Original Authors - Aaron Duffey
// Tweaked for sharing code with networking by Wyatt Senalik.

namespace DuolBots
{
    /// <summary>
    /// Shared controller for VFX displaying during a charge.
    /// </summary>
    public class Shared_VFXOnChargeHandler : MonoBehaviour
    {
        [SerializeField] private Shared_ChargeSpawnProjectileFireController
            m_sharedSpawnProjectileFireController = null;
        [SerializeField] private VisualEffect m_vfx = null;
        private bool m_isSubbed = false;

        public Shared_ChargeSpawnProjectileFireController fireController
            => m_sharedSpawnProjectileFireController;
        public VisualEffect vfx => m_vfx;


        // Domestic Initialization
        private void Awake()
        {
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(
                m_sharedSpawnProjectileFireController,
                nameof(m_sharedSpawnProjectileFireController), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_vfx, nameof(m_vfx), this);
            #endregion Asserts

            // Start the vfx as not charging
            StopCharging();
        }



        public void ToggleSubscription(bool cond, Action startCharging=null,
            Action stopCharging=null)
        {
            if (cond == m_isSubbed) { return; }
            // Possible that the fire controller has been detroyed if
            // we are unsubbing on destroy or something for cleanup.
            if (m_sharedSpawnProjectileFireController == null)
            {
                // Okay if we were trying to unsub
                if (!cond) { return; }
                // Not okay if we were trying to sub
                Debug.LogError($"Tried to subscribe when the fire controller " +
                    $"is null.");
                return;
            }

            // Sub
            if (cond)
            {
                m_sharedSpawnProjectileFireController.onStartedCharging
                    += StartCharging;
                m_sharedSpawnProjectileFireController.onFinishedCharging
                    += StopCharging;

                if (startCharging != null)
                {
                    m_sharedSpawnProjectileFireController.onStartedCharging
                        += startCharging;
                }
                if (stopCharging != null)
                {
                    m_sharedSpawnProjectileFireController.onFinishedCharging
                        += stopCharging;
                }
            }
            // Unsub
            else
            {
                m_sharedSpawnProjectileFireController.onStartedCharging
                   -= StartCharging;
                m_sharedSpawnProjectileFireController.onFinishedCharging
                    -= StopCharging;

                if (startCharging != null)
                {
                    m_sharedSpawnProjectileFireController.onStartedCharging
                        -= startCharging;
                }
                if (stopCharging != null)
                {
                    m_sharedSpawnProjectileFireController.onFinishedCharging
                        -= stopCharging;
                }
            }

            m_isSubbed = cond;
        }
        public void StartCharging()
        {
            if (m_vfx != null)
            {
                m_vfx.Play();
            }
        }
        public void StopCharging()
        {
            if (m_vfx != null)
            {
                m_vfx.Stop();
            }
        }
    }
}
