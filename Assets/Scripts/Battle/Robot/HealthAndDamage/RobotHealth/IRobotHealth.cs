using System;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    public interface IRobotHealth : IMonoBehaviour
    {
        /// <summary>
        /// Event for when this robot's health is reduced.
        /// Parameter is current health.
        /// </summary>
        event Action<float> onHealthChanged;
        /// <summary>
        /// Event for when this robot's health is zero.
        /// </summary>
        event Action onHealthReachedZero;
        /// <summary>
        /// Event for when this robot's health has fallen into
        /// the critical range (means they lose).
        /// </summary>
        event Action<IRobotHealth> onHealthReachedCritical;
        /// <summary>
        /// Current health of the bot.
        /// </summary>
        float currentHealth { get; }
        /// <summary>
        /// Amount of health that is the maximum/starting for this bot.
        /// </summary>
        float maxHealth { get; }
        /// <summary>
        /// Amount of health that is the critical amount for this bot.
        /// A.K.A. when this bot loses.
        /// </summary>
        float criticalHealth { get; }
    }
}
