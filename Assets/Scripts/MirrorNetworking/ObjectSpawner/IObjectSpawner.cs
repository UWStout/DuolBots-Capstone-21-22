using System;
using UnityEngine;

using DuolBots.Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// I realized that we were listening for the local controllers to spawn things
    /// a lot in the networked controllers and simply relaying the spawn across the
    /// network. This will keep us from writing a bunch of those.
    ///
    /// Have a local controller implement this and attached a
    /// <see cref="Network_ObjectSpawner"/> to the same object and it will relay
    /// the spawning of the object.
    /// </summary>
    public interface IObjectSpawner : IMonoBehaviour
    {
        event Action<GameObject> onObjectSpawned;
    }
}
