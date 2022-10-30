using System;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    public interface IObjectDestroyer
    {
        /// <summary>
        /// Should be called instead of GameObject.Destroy or NetworkServer.Destroy.
        /// </summary>
        event Action<GameObject> onShouldDestroyObject;
    }
}
