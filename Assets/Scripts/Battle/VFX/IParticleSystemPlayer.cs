using System;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Interface for object that needs to play ParticleSystem.
    /// </summary>
    public interface IParticleSystemPlayer: IMonoBehaviour
    {
        public event Action<IReadOnlyList<ParticleSystem>> onPlayPSystem;
    }
}
