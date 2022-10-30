using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Allows for spawning objects over the network without them needing
    /// to be a NetworkIdentity. Instead, they can be a NetworkChildIdentity
    /// which allows us to spawn objects as children of a NetworkIdentity,
    /// which you otherwise cannot do.
    /// </summary>
    [RequireComponent(typeof(NetworkMessenger))]
    public class NetworkChildManager : NetworkBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        // Invoked when a child spawns/is destroyed.
        // Called right after spawn & initialization.
        public Action<NetworkChildIdentity> onChildSpawn;
        // Called right before being destroyed.
        public Action<NetworkChildIdentity> onChildDestroy;

        // GameObjets that are registed to be spawned as descendants of this
        // child manager. Must be non-null, prefabs, that have a
        // NetworkChildIdentity attached to them.
        [SerializeField]
        private GameObject[] m_registeredChildren = new GameObject[0];

        // Dictionary to map the guid of the NetworkChildIdentity to an index
        // in the m_registeredChildren array for faster lookup.
        private Dictionary<string, int> m_registeredChildrenIndexByGuid
            = new Dictionary<string, int>();
        // Dictionaries for converting between the child IDs and
        // the GameObjects for those children.
        private Dictionary<NetworkChildIdentity, uint> m_childIDByGameObject
            = new Dictionary<NetworkChildIdentity, uint>();
        private Dictionary<uint, NetworkChildIdentity> m_gameObjectByChildID
            = new Dictionary<uint, NetworkChildIdentity>();

        // "Generated" next child identifier.
        private uint m_curChildID = 0;

        // If start has been called for the different Mirror Start functions
        // for each server and client.
        private bool m_wasStartServerCalled = false;
        private bool m_wasStartLocalPlayerCalled = false;
        private bool m_wasStartClientCalled = false;
        private bool m_wasStartAuthorityCalled = false;

        // Helps initialize network child identities. Allows for
        // NetworkChildBehaviours to send messages as ClientRpcs.
        private NetworkMessenger m_networkMessenger = null;


        // Domestic Initialization
        private void Awake()
        {
            m_networkMessenger = GetComponent<NetworkMessenger>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_networkMessenger, this);
            #endregion Asserts

            // Sort the registered children by their NetworkChildIdentity's Guid.
            m_registeredChildrenIndexByGuid = new Dictionary<string,int>(
                m_registeredChildren.Length);
            for (int i = 0; i < m_registeredChildren.Length; ++i)
            {
                GameObject temp_child = m_registeredChildren[i];

                NetworkChildIdentity temp_netChildId = temp_child.
                    GetComponent<NetworkChildIdentity>();
                #region Asserts
                CustomDebug.AssertComponentOnOtherIsNotNull(temp_netChildId,
                    temp_child, this);
                #endregion Asserts
                m_registeredChildrenIndexByGuid.Add(temp_netChildId.guid, i);
            }
        }
        /// <summary>
        /// Called by Mirror when the server is started.
        /// </summary>
        public override void OnStartServer()
        {
            base.OnStartServer();
            // Relay OnStartServer to all spawned NetworkChildIdentities.
            ExecuteOnAllChildren((NetworkChildIdentity temp_childIdentity) =>
                temp_childIdentity.OnStartServer());
            // Set the flag that NetworkChildIdentity has already been called so
            // that when we spawn NetworkChildIdentities later, we know to call
            // OnStartServer right away.
            m_wasStartServerCalled = true;
        }
        /// <summary>
        /// Called by Mirror when the server is stopped.
        /// Is called during OnDestroy (server) as well.
        /// </summary>
        public override void OnStopServer()
        {
            base.OnStopServer();
            // Relay OnStopServer to all spawned NetworkChildIdentities.
            ExecuteOnAllChildren((NetworkChildIdentity temp_childIdentity) =>
                temp_childIdentity.OnStopServer());
        }
        /// <summary>
        /// Called by Mirror when the client is started.
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();
            #region Logs
            CustomDebug.Log($"{GetType().Name}'s {nameof(OnStartClient)}",
                IS_DEBUGGING);
            #endregion Logs
            // Relay OnStartClient to all spawned NetworkChildIdentities.
            ExecuteOnAllChildren((NetworkChildIdentity temp_childIdentity) =>
                temp_childIdentity.OnStartClient());
            // Set the flag that NetworkChildIdentity has already been called so
            // that when we spawn NetworkChildIdentities later, we know to call
            // OnStartClient right away.
            m_wasStartClientCalled = true;
        }
        /// <summary>
        /// Called by Mirror when the client disconnects from the server.
        /// Is called during OnDestroy (client) as well.
        /// </summary>
        public override void OnStopClient()
        {
            base.OnStopClient();
            // There is no OnStopLocalPlayer built into Mirror, so check here
            // to relay our own.
            if (isLocalPlayer)
            {
                ExecuteOnAllChildren((NetworkChildIdentity temp_childIdentity) =>
                    temp_childIdentity.OnStopLocalPlayer());
            }
            // Relay OnStopClient to all spawned NetworkChildIdentities
            ExecuteOnAllChildren((NetworkChildIdentity temp_childIdentity) =>
                    temp_childIdentity.OnStopClient());
        }
        /// <summary>
        /// Called by Mirror when the client is spawned as a local client (aka when
        /// they spawn as the player object for the connecting client).
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            // Relay OnStartLocalPlayer to all spawned NetworkChildIdentities
            ExecuteOnAllChildren((NetworkChildIdentity temp_childIdentity) =>
                temp_childIdentity.OnStartLocalPlayer());
            // Set the flag that NetworkChildIdentity has already been called so
            // that when we spawn NetworkChildIdentities later, we know to call
            // OnStartLocalPlayer right away.
            m_wasStartLocalPlayerCalled = true;
        }
        /// <summary>
        /// Called by Mirror when this object is spawned with authority on a client
        /// to make ServerRpc calls.
        /// </summary>
        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            // Relay OnStartAuthority to all spawned NetworkChildIdentities
            ExecuteOnAllChildren((NetworkChildIdentity temp_childIdentity) =>
                    temp_childIdentity.OnStartAuthority());
            // Set the flag that NetworkChildIdentity has already been called so
            // that when we spawn NetworkChildIdentities later, we know to call
            // OnStartAuthority right away.
            m_wasStartAuthorityCalled = true;
        }
        /// <summary>
        /// Called by Mirror when the client disconnects from the server on an
        /// object was given authority for that client.
        /// Is called during OnDestroy (client w/ authority) as well.
        /// </summary>
        public override void OnStopAuthority()
        {
            base.OnStopAuthority();
            // Relay OnStopAuthority to all spawned NetworkChildIdentities
            ExecuteOnAllChildren((NetworkChildIdentity temp_childIdentity) =>
                   temp_childIdentity.OnStopAuthority());
        }


        /// <summary>
        /// Spawns the specified already spawned GameObject on the clients.
        /// </summary>
        /// <param name="childInstance">GameObject whose spawn to relay.
        /// Must be a descendant of this GameObject.
        /// Its prefab must be registered to this NetworkChildManager.</param>
        [Server]
        public void Spawn(GameObject childInstance)
        {
            // If the child has no parent, default its parent to be this object.
            Transform temp_parent = childInstance.transform.parent != null ?
                childInstance.transform.parent : transform;
            Spawn(childInstance, childInstance.transform.position,
                childInstance.transform.rotation, temp_parent);
        }
        /// <summary>
        /// Spawns the specified prefab with the given position and rotation
        /// as a child of the specified parent on the server and on the clients.
        /// </summary>
        /// <param name="childInstance">GameObject whose spawn to relay.
        /// Must be a descendant of this GameObject.
        /// Its prefab must be registered to this NetworkChildManager.</param>
        [Server]
        public void Spawn(GameObject childInstance, Vector3 position,
            Quaternion rotation, Transform parent)
        {
            if (!ValidateSpawn(childInstance, parent,
                out int temp_registrationIndex,
                out TransformChildPath temp_childPath))
            {
                // No need to print out error, that is handled already if this
                // is false
                return;
            }
            // Get childID
            uint temp_childID = GenerateNewChildID();
            // Initialize on server
            childInstance.transform.position = position;
            childInstance.transform.rotation = rotation;
            childInstance.transform.parent = parent;
            InitializeNetworkChild(childInstance, temp_childID);
            // Spawn on clients
            SpawnAtPositionClientRpc(temp_registrationIndex, position,
                rotation, temp_childPath, temp_childID);
        }
        /// <summary>
        /// Destroys the given GameObject with a <see cref="NetworkChildIdentity"/>
        /// on the server and clients. Must be called from the server.
        ///
        /// Pre Conditions - Assumes the given GameObject has a
        /// <see cref="NetworkChildIdentity"/> attached. Assumes that the
        /// dictionaries contains the the <see cref="NetworkChildIdentity"/>
        /// on the object with its key.
        /// Post Conditions - Destroys the given GameObject on the server and
        /// clients as well as removing the from the dictionaries.
        /// </summary>
        /// <param name="childObject">GameObject to destroy.</param>
        [Server]
        public void Destroy(GameObject childObject)
        {
            NetworkChildIdentity temp_netChildIdentity =
                childObject.GetComponent<NetworkChildIdentity>();
            #region Asserts
            CustomDebug.AssertComponentOnOtherIsNotNull(temp_netChildIdentity,
                childObject, this);
            #endregion Asserts

            if (!m_childIDByGameObject.TryGetValue(temp_netChildIdentity,
                out uint temp_childID))
            {
                Debug.LogError($"The specified object is not a NetworkChild of " +
                    $"{name}'s {GetType().Name}. Could not destroy.");
                return;
            }

            // Destroy on Server
            DestroyHelper(temp_netChildIdentity);
            // Destroy on Clients
            DestroyClientRpc(temp_childID);
        }


        /// <summary>
        /// Spawns the registered prefab on the clients at the given index with
        /// the specified position and rotation as a child of the Transform at
        /// the end of the child path.
        /// </summary>
        /// <param name="childID">Unique ID for the child generated by
        /// the server,</param>
        [ClientRpc]
        private void SpawnAtPositionClientRpc(int registrationIndex,
            Vector3 position, Quaternion rotation, TransformChildPath childPath,
            uint childID)
        {
            // Don't spawn twice on host
            if (isServer) { return; }

            SpawnObjectHelper(registrationIndex, position,
                rotation, childPath, childID);
        }
        /// <summary>
        /// Destroys the existing <see cref="NetworkChildIdentity"/>
        /// with the specified childId on the client.
        /// </summary>
        [ClientRpc]
        private void DestroyClientRpc(uint childId)
        {
            // Don't destroy twice on host
            if (isServer) { return; }

            if (!m_gameObjectByChildID.TryGetValue(childId,
                out NetworkChildIdentity temp_childIdentity))
            {
                Debug.LogError($"No {nameof(NetworkChildIdentity)} " +
                    $"with childId={childId} exists.");
                return;
            }

            DestroyHelper(temp_childIdentity);
        }

        /// <summary>
        /// Helper for the SpawnClientRpcs.
        /// Finds the GameObject with the given registration index and spawns it
        /// as a child of the transform that lies at the end of the childPath.
        ///
        /// Pre Conditions - Assumes that the registration index is in range.
        /// Assumes the prefab at that location is valid. Also assumes the
        /// childPath is not null and is a valid path.
        /// </summary>
        /// <param name="registrationIndex">Index of the prefab to spawn.</param>
        /// <param name="position">Position to spawn the prefab at.</param>
        /// <param name="rotation">Rotatio to spawn the prefab with.</param>
        /// <param name="childPath">Path to the transform that will be the
        /// parent of the spawned GameObject.</param>
        /// <param name="childID">Unique ID for the child generated by
        /// the server,</param>
        [Client]
        private GameObject SpawnObjectHelper(int registrationIndex,
            Vector3 position, Quaternion rotation, TransformChildPath childPath,
            uint childID)
        {
            #region Asserts
            CustomDebug.AssertIndexIsInRange(registrationIndex,
                m_registeredChildren, this);
            #endregion Asserts
            GameObject temp_prefab = m_registeredChildren[registrationIndex];

            #region Asserts
            CustomDebug.AssertIsTrueForComponent(childPath != null, $"specified " +
                $"path not to be null", this);
            #endregion Asserts
            Transform temp_childParent = childPath.Traverse(transform);

            GameObject temp_spawnedObj = Instantiate(temp_prefab,
                position, rotation, temp_childParent);
            // No longer reset local
            //temp_spawnedObj.transform.ResetLocal();
            InitializeNetworkChild(temp_spawnedObj, childID);
            return temp_spawnedObj;
        }

        /// <summary>
        /// Generates a new, unique id for a NetworkChildIdentity.
        /// </summary>
        [Server]
        private uint GenerateNewChildID()
        {
            return m_curChildID++;
        }
        /// <summary>
        /// Helper function for constructors. Validates the given
        /// prefab and returns if it fits the valid criteria.
        /// Also determines the registration index and creates the child path.
        /// </summary>
        /// <param name="childInstance">Child prefab to validate and check
        /// the registration index for.</param>
        /// <param name="parent">Parent that we want to spawn the prefab
        /// as a child of.</param>
        /// <param name="registrationIndex">Index of the prefab to spawn
        /// in the registration index.</param>
        /// <param name="childPath">Path to the given parent transform in
        /// a form that can be passed over the network.</param>
        /// <returns>If the child is a valid prefab to spawn.</returns>
        [Server]
        private bool ValidateSpawn(GameObject childInstance, Transform parent,
            out int registrationIndex, out TransformChildPath childPath)
        {
            registrationIndex = -1;
            childPath = null;
            // Confirm the specified GameObject is valid.
            if (!ConfirmValidSpawnInstance(childInstance,
                out GameObject temp_prefab)) { return false; }

            #region Logs
            CustomDebug.Log($"Found prefab ({temp_prefab}) for instance object " +
                $"({childInstance})", IS_DEBUGGING);
            #endregion Logs

            #region Asserts
            CustomDebug.AssertIsTrueForComponent(temp_prefab != null,
                $"Could find no SceneObject", this);
            #endregion Asserts
            NetworkChildIdentity temp_childID = childInstance.
                GetComponent<NetworkChildIdentity>();
            #region Asserts
            CustomDebug.AssertComponentOnOtherIsNotNull(temp_childID,
                childInstance, this);
            #endregion Asserts
            if (!m_registeredChildrenIndexByGuid.TryGetValue(temp_childID.guid,
                out registrationIndex))
            {
                Debug.LogError($"The specified prefab ({temp_prefab.name}) " +
                    $"has not been registered");
                return false;
            }

            // Get the child path
            childPath = new TransformChildPath(transform, parent);
            return true;
        }

        /// <summary>
        /// Checks if the given child GameObject is valid to spawn.
        /// Is invalid if it is not an instance (prefab),
        /// it has no NetworkChildIdentity, or it has not been registered.
        /// </summary>
        private bool ConfirmValidSpawnInstance(GameObject childInstance,
            out GameObject prefab)
        {
            prefab = null;

            // Confirm that the specified GameObject is not null.
            if (childInstance == null)
            {
                Debug.LogError($"{childInstance.name} cannot be spawned. " +
                    $"It is null.");
                return false;
            }
            // Confirm that the specified GameObject is truly an instance.
            if (Utils.IsPrefab(childInstance))
            {
                Debug.LogError($"{childInstance.name} cannot be spawned. " +
                    $"It is not a GameObject.");
                return false;
            }
            // Confirm that the specified Instance has a NetworkChildIdentity.
            if (!childInstance.TryGetComponent(out NetworkChildIdentity
                temp_netChildIdentity))
            {
                Debug.LogError($"{childInstance.name} cannot be spawned. " +
                    $"It has no {nameof(NetworkChildIdentity)}.");
                return false;
            }
            #region Logs
            CustomDebug.Log($"Finding prefab for instanced GameObject " +
                $"({childInstance.name} with GUID " +
                $"{temp_netChildIdentity.guid})", IS_DEBUGGING);
            #endregion Logs
            prefab = FindRegisteredPrefabWithGuid(temp_netChildIdentity.guid);
            // Confirm that the specified Prefab is in the list of registered
            // children.
            if (prefab == null)
            {
                Debug.LogError($"{childInstance.name} cannot be spawned. " +
                    $"It was not specified as a registered child.");
                return false;
            }

            return true;
        }
        /// <summary>
        /// Initializes the <see cref="NetworkChildIdentity"/> attached to
        /// the given spawned GameObject.
        /// 
        /// Pre Conditions - Assumes that GameObject has a
        /// <see cref="NetworkChildIdentity"/> attached to it.
        /// Assumes that it hasn't already been initialized.
        /// Post Conditions - Intializes that <see cref="NetworkChildIdentity"/>.
        /// </summary>
        /// <param name="spawnedNetChild">GameObject that was just spawned
        /// and has a <see cref="NetworkChildIdentity"/> attached.</param>
        /// <param name="childID">Unique ID generated by the server for
        /// this <see cref="NetworkChildIdentity"/>.</param>
        private void InitializeNetworkChild(GameObject spawnedNetChild,
            uint childID)
        {
            // Don't need to check if this is null because
            // it is done in ConfirmValidSpawnPrefab.
            NetworkChildIdentity temp_netChildIdentity =
                spawnedNetChild.GetComponent<NetworkChildIdentity>();
            #region Asserts
            Assert.IsNotNull(temp_netChildIdentity, $"{spawnedNetChild.name} " +
                $"was expected to have {nameof(NetworkChildIdentity)} but none " +
                $"was found");
            #endregion Asserts
            temp_netChildIdentity.Initialize(childID, m_networkMessenger, this);

            // Add the net child to the dicitonaries.
            m_gameObjectByChildID.Add(childID, temp_netChildIdentity);
            m_childIDByGameObject.Add(temp_netChildIdentity, childID);

            // Potentially call the various start functions if their window has
            // already passed.
            CallMissedStartFunctions(temp_netChildIdentity);

            #region Logs
            CustomDebug.Log($"Initializing {temp_netChildIdentity.name}",
                IS_DEBUGGING);
            #endregion Logs

            onChildSpawn?.Invoke(temp_netChildIdentity);
        }
        /// <summary>
        /// Executes the specified function on all instantiated
        /// <see cref="NetworkChildIdentity"/> controlled by this
        /// <see cref="NetworkChildManager"/>.
        /// </summary>
        /// <param name="funcToExec">Function that will be executed on
        /// each NetworkChildIdentity.</param>
        private void ExecuteOnAllChildren(Action<NetworkChildIdentity> funcToExec)
        {
            #region Logs
            CustomDebug.Log($"{GetType().Name}'s {nameof(ExecuteOnAllChildren)}. " +
                $"{nameof(m_gameObjectByChildID)} Count = " +
                $"{m_gameObjectByChildID.Count}. {nameof(m_childIDByGameObject)} " +
                $"Count = {m_childIDByGameObject.Count}",
                IS_DEBUGGING);
            #endregion Logs
            foreach (KeyValuePair<uint, NetworkChildIdentity>
                temp_kvp in m_gameObjectByChildID)
            {
                NetworkChildIdentity temp_curChild = temp_kvp.Value;
                funcToExec.Invoke(temp_curChild);
            }
        }
        /// <summary>
        /// Removes the <see cref="NetworkChildIdentity"/> from both dictionaries
        /// and then Destroys it.
        /// </summary>
        /// <param name="childIdentity"><see cref="NetworkChildIdentity"/> to
        /// clean up and Destroy.</param>
        private void DestroyHelper(NetworkChildIdentity childIdentity)
        {
            // Remove from dictionaries
            m_gameObjectByChildID.Remove(childIdentity.childID);
            m_childIDByGameObject.Remove(childIdentity);

            onChildDestroy?.Invoke(childIdentity);
            // Destroy the object
            Destroy(childIdentity.gameObject);
        }
        /// <summary>
        /// Finds a prefab in the registered prefabs dictionary with the
        /// specified guid.
        /// </summary>
        private GameObject FindRegisteredPrefabWithGuid(string guid)
        {
            if (!m_registeredChildrenIndexByGuid.TryGetValue(guid,
                out int temp_childRegIndex))
            {
                Debug.LogError($"No Prefab is registered with the guid {guid}");
            }
            #region Asserts
            CustomDebug.AssertIndexIsInRange(temp_childRegIndex,
                m_registeredChildren, this);
            #endregion Asserts
            return m_registeredChildren[temp_childRegIndex];
        }
        /// <summary>
        /// Calls any start calls that were missed because this was spawned after
        /// start.
        /// Order is: OnStartServer, OnStartClient,
        /// OnStartLocalPlayer, OnStartAuthority
        /// </summary>
        /// <param name="spawnedChild">Spawned <see cref="NetworkChildIdentity"/>
        /// that needs its start functions called.</param>
        private void CallMissedStartFunctions(NetworkChildIdentity spawnedChild)
        {
            // Server starts
            if (isServer)
            {
                if (m_wasStartServerCalled)
                {
                    #region Logs
                    CustomDebug.Log($"Calling " +
                        $"{nameof(spawnedChild.OnStartServer)} for " +
                        $"{spawnedChild.name}", IS_DEBUGGING);
                    #endregion Logs
                    spawnedChild.OnStartServer();
                }
            }
            // Client starts
            if (isClient)
            {
                if (m_wasStartClientCalled)
                {
                    #region Logs
                    CustomDebug.Log($"Calling " +
                        $"{nameof(spawnedChild.OnStartClient)} for " +
                        $"{spawnedChild.name}", IS_DEBUGGING);
                    #endregion Logs
                    spawnedChild.OnStartClient();
                }
                if (m_wasStartLocalPlayerCalled && isLocalPlayer)
                {
                    #region Logs
                    CustomDebug.Log($"Calling " +
                        $"{nameof(spawnedChild.OnStartLocalPlayer)} for " +
                        $"{spawnedChild.name}", IS_DEBUGGING);
                    #endregion Logs
                    spawnedChild.OnStartLocalPlayer();
                }
                if (m_wasStartAuthorityCalled && hasAuthority)
                {
                    #region Logs
                    CustomDebug.Log($"Calling " +
                        $"{nameof(spawnedChild.OnStartAuthority)} for " +
                        $"{spawnedChild.name}", IS_DEBUGGING);
                    #endregion Logs
                    spawnedChild.OnStartAuthority();
                }
            }
        }
    }
}
