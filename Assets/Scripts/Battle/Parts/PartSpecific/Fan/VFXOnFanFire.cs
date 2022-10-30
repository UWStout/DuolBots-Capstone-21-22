using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.VFX;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Handles VFX for the Fan weapon on charge/fire.
    /// Acts the same as VFXOnChargeHandler but scales the play rate of the VFX to the amount of time charge is held.
    /// </summary>
    [Obsolete("Fan weapon was switched to using ParticleSystems for VFX. \n" +
        "Use ParticleSystemsOnInstantiatedFanFire instead.", false)]
    public class VFXOnFanFire : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;
        private const float BASE_PLAY_RATE = 1.0f;
        private const float MAX_PLAY_RATE = 3.0f;
        private const float CHARGE_BUILDUP = 0.1f;

        [SerializeField] private FanProjectileFireController m_fanProjectileFireController = null;
        [SerializeField] private VisualEffect m_vfx = null;

        // Power of force applied to the affected objects
        private float m_maxFanForce = 50f;
        public float maxFanForce => m_maxFanForce;
        private float m_minFanForce = 20f;
        public float minFanForce => m_minFanForce;
        private float m_curFanForce = 20f;
        private float m_vfxPlayRateMultiplier = 1.0f;

        private void Awake()
        {
            // Check that VisualEffect is not null and that the shared controller is not null and then subscribe to its charging events
            Assert.IsNotNull(m_fanProjectileFireController, $"{name} does not have a " +
                $"{m_fanProjectileFireController.GetType()} but rewuires one.");
            Assert.IsNotNull(m_vfx, $"{name} does not have a " +
                $"{m_vfx.GetType()} but rewuires one.");

            m_fanProjectileFireController.onStartedCharging += StartCharging;
            m_fanProjectileFireController.onFinishedCharging += StopCharging;
        }

        private void OnDisable()
        {
            // In case the object is destroyed before OnDisable() gets called.
            if (m_fanProjectileFireController != null)
            {
                m_fanProjectileFireController.onFinishedCharging -= StartCharging;
                m_fanProjectileFireController.onFinishedCharging -= StopCharging;
            }
        }

        private void StartCharging()
        {
            CustomDebug.Log($"StartCharging called.", IS_DEBUGGING);
            m_vfxPlayRateMultiplier = m_fanProjectileFireController.charge;
            if (m_vfx != null)
            {
                m_vfx.playRate = m_vfxPlayRateMultiplier;
                m_vfx.Play();
            }
        }

        private void StopCharging()
        {
            CustomDebug.Log($"StopCharging called.", IS_DEBUGGING);
            m_vfxPlayRateMultiplier = BASE_PLAY_RATE;
            if (m_vfx != null)
            {
                m_vfx.playRate = BASE_PLAY_RATE;
                m_vfx.Stop();
            }
        }

    }
}
