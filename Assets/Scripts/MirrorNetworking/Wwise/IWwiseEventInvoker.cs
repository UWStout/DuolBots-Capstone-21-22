using System;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Interface for <see cref="DuolBots.Mirror.NetworkWwiseEventManager"/> to find
    /// and listen to for relaying the event evokation across the network.
    /// </summary>
    public interface IWwiseEventInvoker
    {
        public event Action<WwiseEventName, GameObject> requestInvokeWwiseEvent;
    }
}
