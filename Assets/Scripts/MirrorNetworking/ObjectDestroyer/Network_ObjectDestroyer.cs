using UnityEngine;
using UnityEngine.Assertions;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Handles what should be done with <see cref="IObjectDestroyer"/>s
    /// on the network variant (tells the server to destroy it).
    ///
    /// Add this as a parent or the same object as <see cref="IObjectDestroyer"/>s
    /// that this is controlling.
    /// </summary>
    public class Network_ObjectDestroyer : NetworkBehaviour
    {
        private IObjectDestroyer[] m_objectDestroyers = null;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            m_objectDestroyers = GetComponentsInChildren<IObjectDestroyer>();
            Assert.AreNotEqual(0, m_objectDestroyers.Length, $"{name}'s " +
                $"{GetType().Name} expected to find {nameof(IObjectDestroyer)}s " +
                $"attached to its children but none were found.");
        }
        public override void OnStartServer()
        {
            base.OnStartServer();

            foreach (IObjectDestroyer temp_destroyer in m_objectDestroyers)
            {
                temp_destroyer.onShouldDestroyObject += OnShouldDestroyObject;
            }
        }
        public override void OnStopServer()
        {
            base.OnStopServer();

            foreach (IObjectDestroyer temp_destroyer in m_objectDestroyers)
            {
                // Possible that it was destroyed before this.
                if (temp_destroyer == null) { continue; }
                temp_destroyer.onShouldDestroyObject -= OnShouldDestroyObject;
            }
        }


        private void OnShouldDestroyObject(GameObject objToDestroy)
        {
            // Local over network.
            NetworkServer.Destroy(objToDestroy);
        }
    }
}
