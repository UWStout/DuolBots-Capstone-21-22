using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Local variant of the timer manager.
    /// Simply starts the timer on start.
    /// </summary>
    [RequireComponent(typeof(Shared_TimerManager))]
    public class Local_TimerManager : MonoBehaviour
    {
        private Shared_TimerManager m_sharedController = null;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            m_sharedController = GetComponent<Shared_TimerManager>();
            Assert.IsNotNull(m_sharedController, $"{name}'s {GetType().Name} " +
                $"requires {nameof(Shared_TimerManager)} but none was found");
        }
        // Called 1st
        // Foreign Initialization
        private void Start()
        {
            // In the local variant, start the timer right away
            m_sharedController.StartTimer();
        }
    }
}
