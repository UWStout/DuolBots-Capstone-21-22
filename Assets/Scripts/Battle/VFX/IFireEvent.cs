using System;
using UnityEngine;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Interface for weapons that can fire. Used for generic detection of a weapon
    /// firing in place of specific WeaponFireControllers.
    /// </summary>
    public interface IFireEvent : IMonoBehaviour
    {
        public event Action onFire;
    }
}
