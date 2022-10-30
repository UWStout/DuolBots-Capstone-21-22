using UnityEngine;
// Original Authors - Wyatt Senalik and Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Interface for handling what to do when PartImpactCollider collide with a part.
    /// </summary>
    public interface IImpactHandler
    {
        /// <summary>
        /// Called when PartImpactCollider collides with a part (OnTriggerStay).
        /// </summary>
        public void HandleImpact(Collider collider, bool didHitEnemy, byte enemyTeamIndex);

        public int priority { get; }
    }
}
