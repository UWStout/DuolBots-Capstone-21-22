using UnityEngine;
// Original Authors - Ben Lussman

namespace DuolBots
{
    /// <summary>
    /// Interface for projectile prefab
    /// </summary>
    public interface IPartTrajectorySpecs : IMonoBehaviour
    {
        /// <summary>
        /// Projectile prefab
        /// </summary>
        public Transform projectileSpawnPos { get; }

    }
}
