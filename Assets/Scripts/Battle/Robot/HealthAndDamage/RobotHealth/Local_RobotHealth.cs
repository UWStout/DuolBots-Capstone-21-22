using System;
using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Local robot health for testing.
    /// </summary>
    [RequireComponent(typeof(Shared_RobotHealth))]
    public class Local_RobotHealth : MonoBehaviour, IRobotHealth
    {
        private Shared_RobotHealth m_sharedController = null;

        public event Action<float> onHealthChanged;
        public event Action onHealthReachedZero;
        public event Action<IRobotHealth> onHealthReachedCritical;

        public float currentHealth => m_sharedController.currentHealth;
        public float maxHealth => m_sharedController.maxHealth;
        public float criticalHealth => m_sharedController.criticalHealth;


        // Called 0th
        private void Awake()
        {
            m_sharedController = GetComponent<Shared_RobotHealth>();
            Assert.IsNotNull(m_sharedController, $"{name}'s {GetType().Name} " +
                $"requires {nameof(Shared_RobotHealth)} to be attached but none " +
                $"was found.");

            // Subscribed
            m_sharedController.onHealthChanged += InvokeOnHealthChanged;
            m_sharedController.onHealthReachedZero += InvokeOnHealthReachedZero;
            m_sharedController.onHealthReachedCritical += InvokeOnHealthReachedCritical;
        }
        // Called 1st
        private void Start()
        {
            // This must be in Start because BattleBotInstantiator
            // creates the bot in Start. This will be called after
            // all parts are created.
            m_sharedController.GatherPartHealths();
        }
        private void OnDestroy()
        {
            // Can be null if other object is destroyed first
            if (m_sharedController == null) { return; }
            // Unsubscribe
            m_sharedController.onHealthChanged -= InvokeOnHealthChanged;
            m_sharedController.onHealthReachedZero -= InvokeOnHealthReachedZero;
            m_sharedController.onHealthReachedCritical -= InvokeOnHealthReachedCritical;
        }


        private void InvokeOnHealthChanged(float newHealth)
        {
            onHealthChanged?.Invoke(newHealth);
        }
        private void InvokeOnHealthReachedZero()
        {
            onHealthReachedZero?.Invoke();
        }
        private void InvokeOnHealthReachedCritical()
        {
            onHealthReachedCritical?.Invoke(this);
        }
    }
}
