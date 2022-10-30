using System;
using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Identity for a NetworkChild GameObject.
    /// Manages the conneciton between the <see cref="NetworkChildManager"/>
    /// and all the <see cref="NetworkChildBehaviour"/> attached to this
    /// behaviour.
    /// </summary>
    public class NetworkChildIdentity : MonoBehaviour
    {
        public string guid => m_guid;
        [SerializeField] [ReadOnly] private string m_guid
            = Guid.NewGuid().ToString();

        public NetworkChildManager manager { get; private set; } = null;
        public NetworkMessenger messenger { get; private set; } = null;
        public uint childID { get; private set; }

        private NetworkChildBehaviour[] m_netChildBehviours =
            new NetworkChildBehaviour[0];


        /// <summary>
        /// Returns true in server context if this game object has been spawned.
        /// </summary>
        public bool isServer => manager != null ? manager.isServer : false;
        /// <summary>
        /// Returns true in client context if this game
        /// object has been spawned by the server.
        /// </summary>
        public bool isClient => manager != null ? manager.isClient : false;
        /// <summary>
        /// Returns true on the client if this game object
        /// represents the player created for this client.
        /// </summary>
        public bool isLocalPlayer => manager != null ? manager.isLocalPlayer : false;
        /// <summary>
        /// Returns true on the client if this client has
        /// authority over this game object. It is meaningless in server context.
        /// </summary>
        public bool hasAuthority => manager != null ? manager.hasAuthority : false;


        /// <summary>
        /// Called on server when a game object spawns on the server,
        /// or when the server is started for game objects in the Scene.
        /// </summary>
        public virtual void OnStartServer()
        {
            foreach (NetworkChildBehaviour temp_behaviour in m_netChildBehviours)
            {
                temp_behaviour.OnStartServer();
            }
        }
        /// <summary>
        /// Called on server when a game object is destroyed on the server,
        /// or when the server is stopped for game objects in the Scene.
        /// </summary>
        public virtual void OnStopServer()
        {
            foreach (NetworkChildBehaviour temp_behaviour in m_netChildBehviours)
            {
                temp_behaviour.OnStopServer();
            }
        }
        /// <summary>
        /// Called on clients when the game object spawns on the client,
        /// or when the client connects to a server for game objects in the Scene.
        /// </summary>
        public virtual void OnStartClient()
        {
            foreach (NetworkChildBehaviour temp_behaviour in m_netChildBehviours)
            {
                temp_behaviour.OnStartClient();
            }
        }
        /// <summary>
        /// Called on clients when the server destroys the game object.
        /// </summary>
        public virtual void OnStopClient()
        {
            foreach (NetworkChildBehaviour temp_behaviour in m_netChildBehviours)
            {
                temp_behaviour.OnStopClient();
            }
        }
        /// <summary>
        /// Called on clients after OnStartClient for the player game
        /// object on the local client
        /// </summary>
        public virtual void OnStartLocalPlayer()
        {
            foreach (NetworkChildBehaviour temp_behaviour in m_netChildBehviours)
            {
                temp_behaviour.OnStartLocalPlayer();
            }
        }
        /// <summary>
        /// Called on clients before OnStopClient for the player
        /// game object on the local client
        /// </summary>
        public virtual void OnStopLocalPlayer()
        {
            foreach (NetworkChildBehaviour temp_behaviour in m_netChildBehviours)
            {
                temp_behaviour.OnStopLocalPlayer();
            }
        }
        /// <summary>
        /// Called on owner client when assigned authority by the server.
        /// hasAuthority will be true for such objects in client context.
        /// </summary>
        public virtual void OnStartAuthority()
        {
            foreach (NetworkChildBehaviour temp_behaviour in m_netChildBehviours)
            {
                temp_behaviour.OnStartAuthority();
            }
        }
        /// <summary>
        /// Called on owner client when authority is removed by the server.
        /// </summary>
        public virtual void OnStopAuthority()
        {
            foreach (NetworkChildBehaviour temp_behaviour in m_netChildBehviours)
            {
                temp_behaviour.OnStopAuthority();
            }
        }


        // Called 0th
        private void Awake()
        {
            m_netChildBehviours = GetComponents<NetworkChildBehaviour>();
        }
        private void OnDestroy()
        {
            // Call the OnStops when applicable
            if (isServer)
            {
                OnStopServer();
            }
            if (isClient)
            {
                if (isLocalPlayer)
                {
                    OnStopLocalPlayer();
                }
                if (hasAuthority)
                {
                    OnStopAuthority();
                }
                OnStopClient();
            }
        }


        /// <summary>
        /// Initializes the <see cref="NetworkChildIdentity"/ from the
        /// <see cref="NetworkChildManager"/> with a unique id and a
        /// reference to its manager.
        ///
        /// Pre Conditions - Assumes that <see cref="NetworkChildManager"/>
        /// is the only script that calls this and that it is only called once.
        /// Post Conditions - childID and myManager are set.
        /// </summary>
        public void Initialize(uint id, NetworkMessenger childMessenger,
            NetworkChildManager netChildMan)
        {
            Assert.IsNull(manager, $"{name}'s {nameof(NetworkChildIdentity)} " +
                $"should have its Initialize function called by anything but the " +
                $"{nameof(NetworkChildManager)}");

            childID = id;
            messenger = childMessenger;
            manager = netChildMan;
        }
    }
}
