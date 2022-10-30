using UnityEngine;
using UnityEngine.Assertions;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// I realized that we were listening for the local controllers to spawn things
    /// a lot in the networked controllers and simply relaying the spawn across the
    /// network. This will keep us from writing a bunch of those.
    ///
    /// Have a local controller implement <see cref="IObjectSpawner"/> and
    /// attached this to a parent object and it will relay the spawning of the object.
    /// </summary>
    public class Network_ObjectSpawner : NetworkBehaviour
    {
        private const bool IS_DEBUGGING = false;

        private IObjectSpawner[] m_objectSpawners = null;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            m_objectSpawners = GetComponentsInChildren<IObjectSpawner>();
            Assert.AreNotEqual(0, m_objectSpawners.Length, $"{name}'s {GetType().Name} expected " +
                $"to find {nameof(IObjectSpawner)}s attached to its children but none were found.");

            CustomDebug.LogForComponent($"Found {m_objectSpawners.Length} {nameof(IObjectSpawner)}s.",
                this, IS_DEBUGGING);
            foreach (IObjectSpawner objSpawner in m_objectSpawners)
            {
                CustomDebug.LogForComponent($"Found object spawner {objSpawner.GetType().Name}",
                    this, IS_DEBUGGING);
            }

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
            CustomDebug.LogForComponent($"Trying to spawn {objInstance.name} across the network",
                this, IS_DEBUGGING);
            NetworkServer.Spawn(objInstance);
        }
    }
}
