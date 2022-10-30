using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Local variant of GameOverMonitor. Initializes shared on Start. 
    /// </summary>
    [RequireComponent(typeof(Shared_HealthGameOver))]
    public class Local_HealthGameOver : MonoBehaviour
    {
        private Shared_HealthGameOver m_sharedController = null;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            m_sharedController = GetComponent<Shared_HealthGameOver>();
            Assert.IsNotNull(m_sharedController, $"{name}'s {GetType().Name} " +
                $"requires {nameof(Shared_HealthGameOver)} be attached but none " +
                $"was found.");
        }
        // Subscribe to events
        private void OnEnable()
        {
            m_sharedController.onBotShouldDie += OnBotShouldDie;
        }
        // Unsubscribe from events
        private void OnDisable()
        {
            // Handle if the shared controller is destroyed before this.
            if (m_sharedController != null)
            {
                m_sharedController.onBotShouldDie -= OnBotShouldDie;
            }
        }
        // Called 1st
        private void Start()
        {
            m_sharedController.InitializeRobotHealths();
        }


        /// <summary>
        /// Destroys the bot.
        /// Called by <see cref="Shared_HealthGameOver.onBotShouldDie"/>.
        /// </summary>
        /// <param name="botThatShouldDie">Bot's
        /// <see cref="GameObject"/> to destroy.</param>
        private void OnBotShouldDie(GameObject botThatShouldDie)
        {
            // Destroy the bot
            Destroy(botThatShouldDie);
        }
    }
}
