using UnityEngine;
// Original Authors - Ben Lussman
// Modified by Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Interface for projectile prefab
    /// </summary>
    public interface IPartShowsTrajectory : IMonoBehaviour
    {
        /// <summary>
        /// Initial speed for the projectile prefab.
        /// </summary>
        public float initialSpeed { get; }
        /// <summary>
        /// Gravity the prefab should use.
        /// </summary>
        public bool shouldUseGravity { get; }
    }
}
