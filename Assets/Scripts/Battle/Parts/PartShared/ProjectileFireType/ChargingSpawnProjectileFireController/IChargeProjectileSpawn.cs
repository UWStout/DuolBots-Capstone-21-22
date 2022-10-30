using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Interface for projectiles from weapons that have a charge.
    /// </summary>
    public interface IChargeProjectileSpawn
    {
        void SetCharge(float charge);
    }
}
