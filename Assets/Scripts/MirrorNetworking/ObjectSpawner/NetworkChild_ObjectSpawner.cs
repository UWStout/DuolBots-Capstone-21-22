using UnityEngine;
using UnityEngine.Assertions;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Similar to <see cref="Network_ObjectSpawner"/> but works on
    /// NetworkChildIdentities instead of NetworkIdentities.
    /// </summary>
    [DisallowMultipleComponent]
    public class NetworkChild_ObjectSpawner : NetworkChildBehaviour
    {
        private const bool IS_DEBUGGING = false;

        private IObjectSpawner[] m_objectSpawners = null;


        // Called 0th
        // Domestic Initialization
        protected override void Awake()
        {
            base.Awake();

            m_objectSpawners = GetComponentsInChildren<IObjectSpawner>();
            Assert.AreNotEqual(0, m_objectSpawners.Length, $"{name}'s {GetType().Name} expected " +
                $"to find {nameof(IObjectSpawner)}s attached to its children but none were found.");

            CustomDebug.Log($"{name}'s {GetType().Name} spawned at {Time.time}",
                IS_DEBUGGING);
        }
        public override void OnStartServer()
        {
            base.OnStartServer();

            CustomDebug.Log($"{name}'s OnStartServer", IS_DEBUGGING);
            // Subscribe to events
            foreach (IObjectSpawner temp_singleSpawner in m_objectSpawners)
            {
                temp_singleSpawner.onObjectSpawned += SpawnObjectAcrossNetwork;
            }
        }
        public override void OnStopServer()
        {
            base.OnStopServer();

            CustomDebug.Log($"{name}'s OnStopServer", IS_DEBUGGING);
            // Unsubscribe from events
            foreach (IObjectSpawner temp_singleSpawner in m_objectSpawners)
            {
                // Be careful since its very possible the object spawner was destroyed
                // before this component because the object is being destroyed.
                if (temp_singleSpawner == null) { continue; }

                temp_singleSpawner.onObjectSpawned -= SpawnObjectAcrossNetwork;
            }
        }


        [Server]
        private void SpawnObjectAcrossNetwork(GameObject objInstance)
        {
            CustomDebug.Log($"Trying to spawn {objInstance.name} across the network",
                IS_DEBUGGING);
            NetworkServer.Spawn(objInstance);
        }
    }
}
