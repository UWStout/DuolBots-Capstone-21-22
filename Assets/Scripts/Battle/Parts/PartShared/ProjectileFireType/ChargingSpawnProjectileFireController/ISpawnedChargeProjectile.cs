using System;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Interface for projectiles from weapons that have a charge.
    /// </summary>
    public interface ISpawnedChargeProjectile
    {
        public void SetCharge(float charge);
    }
}
