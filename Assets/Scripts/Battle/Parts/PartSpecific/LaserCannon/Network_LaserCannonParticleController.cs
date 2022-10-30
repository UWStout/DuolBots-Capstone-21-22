using UnityEngine;
using UnityEngine.Assertions;

using Mirror;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Network version of <see cref="LaserCannonParticleController"/>.
    /// </summary>
    public class Network_LaserCannonParticleController : NetworkChildBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField] [Min(0.01f)]
        private float m_particleSpeedMultiplier = 1.0f;
        [SerializeField] [Required]
        private ParticleSystem m_particleSystem = null;

        private Shared_ChargeSpawnProjectileFireController
            m_sharedChargeSpawnProjectileFireController = null;

        // What the current state of the laser particle effect system is.
        // True = playing. False = stopped. Starts playing for some reason.
        // If this is set to false instead, you will see the particle system
        // on game start, which is undesired.
        private bool m_curState = true;


        // Called on both Client and Server
        protected override void Awake()
        {
            base.Awake();

            Assert.IsNotNull(m_particleSystem, $"{typeof(ParticleSystem).Name} " +
                $"was not found on {this.name} but is required.");
        }
        public override void OnStartServer()
        {
            base.OnStartServer();

            m_sharedChargeSpawnProjectileFireController = GetComponent
                <Shared_ChargeSpawnProjectileFireController>();
            Assert.IsNotNull(m_sharedChargeSpawnProjectileFireController,
                $"{typeof(Shared_ChargeSpawnProjectileFireController).Name} " +
                $"was not found on {this.name} but is required.");
        }
        private void Update()
        {
            // Only server should run update
            if (!isServer) { return; }

            // If the charge button is being held, start the particle system
            if (m_sharedChargeSpawnProjectileFireController.isCharging)
            {
                // Set particle system's charge
                ChangeParticleSystemCharge();
                // Play particle system
                OnParticleSystemStateChange(true);
            }
            // If the charge button is not being held
            else
            {
                // Stop playing particle system
                OnParticleSystemStateChange(false);
            }
        }


        /// <summary>
        /// Sends out messages to the clients to play (newState=true) or
        /// stop (newState=false) the particle system.
        /// </summary>
        [Server]
        private void OnParticleSystemStateChange(bool newState)
        {
            CustomDebug.Log($"{name}'s " +
                $"{nameof(OnParticleSystemStateChange)} " +
                $"{nameof(newState)} is {newState}", IS_DEBUGGING);

            // Don't make a server call if the new state
            // is the same as the old state.
            if (newState == m_curState) { return; }
            m_curState = newState;

            // Send the message with the packed data
            messenger.SendMessageToClient(gameObject,
                nameof(OnParticleSystemStateChangeClient), newState);
        }
        [Client]
        private void OnParticleSystemStateChangeClient(bool newState)
        {
            CustomDebug.Log($"{name}'s " +
                $"{nameof(OnParticleSystemStateChangeClient)} " +
                $"{nameof(newState)} is {newState}", IS_DEBUGGING);

            // If particle system was played on server, play on client
            if (newState)
            {
                if (m_particleSystem.isPlaying) { return; }
                CustomDebug.Log($"Playing particle system", IS_DEBUGGING);
                m_particleSystem.Play();
            }
            // If particle system was stopped on server, stop on client
            else
            {
                if (m_particleSystem.isStopped) { return; }
                CustomDebug.Log($"Stopping particle system", IS_DEBUGGING);
                m_particleSystem.Stop();
            }
        }

        /// <summary>
        /// Sends out messages to the clients to change the charge
        /// of their particle systems to be based on the current charge
        /// of the laser.
        /// </summary>
        [Server]
        private void ChangeParticleSystemCharge()
        {
            float temp_charge = m_sharedChargeSpawnProjectileFireController
                .curCharge;

            messenger.SendMessageToClient(gameObject,
                nameof(ChangeParticleSystemChargeClient), temp_charge);
        }
        [Client]
        private void ChangeParticleSystemChargeClient(float charge)
        {
            // Set the speed of the particle system to correspond to the
            // current charge of the laser.
            ParticleSystem.MainModule temp_main = m_particleSystem.main;
            temp_main.startSpeed = charge * m_particleSpeedMultiplier;
        }
    }
}
