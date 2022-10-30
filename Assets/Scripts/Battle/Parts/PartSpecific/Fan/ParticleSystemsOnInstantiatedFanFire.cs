using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using Mirror;
// Original Author - Aaron Duffey
// Tweaked for Networking by Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Handles VFX from the Fan using the instantiated projectiles. Only starts non-looping
    /// ParticleSystem effects because it will be destroyed after a brief duration.
    /// Does not handle any of the actual collision or Fan effect on objects.
    /// </summary>
    public class ParticleSystemsOnInstantiatedFanFire : NetworkBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;
        private const float BASE_PLAY_RATE = 1.0f;
        private const float MAX_PLAY_RATE = 3.0f;
        private const float PLAY_RATE_RANGE = (MAX_PLAY_RATE - BASE_PLAY_RATE);

        [SerializeField] private List<ParticleSystem> m_pSystems = null;

        private float m_curVFXPlayRate = 1.0f;


        // Domestic Initialization
        private void Awake()
        {
            Assert.IsNotNull(m_pSystems, $"{name} does not have a " +
                $"{m_pSystems.GetType()} but requires one.");
        }


        [Server]
        public void SetPlayBackSpeed(float percentageOfMaCharge)
        {
            SetPlayBackSpeedClientRpc(percentageOfMaCharge);
        }


        [ClientRpc]
        private void SetPlayBackSpeedClientRpc(float percentageOfMaCharge)
        {
            m_curVFXPlayRate = percentageOfMaCharge * PLAY_RATE_RANGE + BASE_PLAY_RATE;
            PlayEffects();
        }
        /// <summary>
        /// Plays the ParticleSystem effects on the instantiated projectile.
        /// Called on after the playback speed is set, done in place of looping VFX.
        /// </summary>
        private void PlayEffects()
        {
            CustomDebug.Log($"PlayEffects called.", IS_DEBUGGING);
            foreach (ParticleSystem pSystem in m_pSystems)
            {
                // Play the ParticleSystem at the current play rate given the charge.
                if (pSystem != null)
                {
                    ParticleSystem.MainModule temp_pModule = pSystem.main;
                    temp_pModule.loop = false;
                    temp_pModule.simulationSpeed = m_curVFXPlayRate;
                    pSystem.Play();
                }
                CustomDebug.Log($"{pSystem}'s simulation speed was set to {m_curVFXPlayRate}", IS_DEBUGGING);
            }
        }
    }
}
