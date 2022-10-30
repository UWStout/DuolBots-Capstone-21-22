using Mirror;
using System;
using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Restricts calling RobotHealth's GatherParts function to the server.
    /// Assumes RobotHealth is set to manual timing.
    /// </summary>
    [RequireComponent(typeof(Shared_RobotHealth))]
    public class Network_RobotHealth : NetworkBehaviour, IRobotHealth
    {
        private Shared_RobotHealth m_sharedController = null;

        public event Action<float> onHealthChanged;
        public event Action onHealthReachedZero;
        public event Action<IRobotHealth> onHealthReachedCritical;

        public float currentHealth => m_currentHealth;
        public float maxHealth => m_maxHealth;
        public float criticalHealth => m_criticalHealth;

        // Cache these locally so that we can sync them over the network
        // instead of just using Shared_RobotHealth's values which are
        // always zero on the server.
        [SyncVar] private float m_currentHealth = 0.0f;
        [SyncVar] private float m_maxHealth = 0.0f;
        [SyncVar] private float m_criticalHealth = 0.0f;


        public override void OnStartServer()
        {
            base.OnStartServer();

            m_sharedController = GetComponent<Shared_RobotHealth>();
            Assert.IsNotNull(m_sharedController, $"{name}'s {GetType().Name} requires " +
                $"a {nameof(Shared_RobotHealth)} be attached but none was found.");

            m_sharedController.GatherPartHealths();

            m_currentHealth = m_sharedController.currentHealth;
            m_maxHealth = m_sharedController.maxHealth;
            m_criticalHealth = m_sharedController.criticalHealth;

            m_sharedController.onHealthChanged += OnHealthChanged;
            m_sharedController.onHealthReachedZero += OnHealthReachedZero;
            m_sharedController.onHealthReachedCritical += OnHealthReachedCritical;
        }
        public override void OnStopServer()
        {
            base.OnStopServer();

            // Can be null if other object is destroyed first
            if (m_sharedController == null) { return; }
            m_sharedController.onHealthChanged -= OnHealthChanged;
            m_sharedController.onHealthReachedZero -= OnHealthReachedZero;
            m_sharedController.onHealthReachedCritical -= OnHealthReachedCritical;
        }


        private void OnHealthChanged(float newHealth)
        {
            m_currentHealth = newHealth;
            onHealthChanged?.Invoke(newHealth);
            // Invoke event on non-host
            OnHealthChangedClientRpc(newHealth);
        }
        private void OnHealthReachedZero()
        {
            onHealthReachedZero?.Invoke();
            // Invoke event on non-host
            OnHealthReachedZeroClientRpc();
        }
        private void OnHealthReachedCritical()
        {
            onHealthReachedCritical?.Invoke(this);
            // Invoke event on non-host
            OnHealthReachedCriticalClientRpc();
        }

        [ClientRpc]
        private void OnHealthChangedClientRpc(float newHealth)
        {
            // Protect against calling twice on host
            if (isServer) { return; }

            onHealthChanged?.Invoke(newHealth);
        }
        [ClientRpc]
        private void OnHealthReachedZeroClientRpc()
        {
            // Protect against calling twice on host
            if (isServer) { return; }

            onHealthReachedZero?.Invoke();
        }
        [ClientRpc]
        private void OnHealthReachedCriticalClientRpc()
        {
            // Protect against calling twice on host
            if (isServer) { return; }

            onHealthReachedCritical?.Invoke(this);
        }
    }
}
