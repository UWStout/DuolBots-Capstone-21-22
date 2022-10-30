using System;
using UnityEngine;
// Original Authors - Aaron Duffey
// Tweaked for sharing code with networking by Wyatt Senalik.

namespace DuolBots
{
    /// <summary>
    /// Shared controller for VFX displaying during a charge.
    /// </summary>
    public class Shared_OnChargeHandler : MonoBehaviour
    {
        [SerializeField] private Shared_ChargeSpawnProjectileFireController
            m_sharedSpawnProjectileFireController = null;
        private bool m_isSubbed = false;

        public Shared_ChargeSpawnProjectileFireController fireController
            => m_sharedSpawnProjectileFireController;


        // Domestic Initialization
        private void Awake()
        {
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(
                m_sharedSpawnProjectileFireController,
                nameof(m_sharedSpawnProjectileFireController), this);
            #endregion Asserts
        }



        public void ToggleSubscription(bool cond, Action startCharging,
            Action stopCharging)
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
    }
}
