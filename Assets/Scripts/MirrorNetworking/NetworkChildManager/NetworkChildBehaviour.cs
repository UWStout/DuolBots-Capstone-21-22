using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Base class for MonoBehaviours that want to know about network stuff, but
    /// have their object dynamically created as descendants of a NetworkIdentity.
    /// </summary>
    [RequireComponent(typeof(NetworkChildIdentity))]
    public abstract class NetworkChildBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Returns the NetworkChildIdentity of this object
        /// </summary>
        public NetworkChildIdentity netChildIdentity { get; private set; }
        /// <summary>
        /// Returns true in server context if this game object has been spawned.
        /// </summary>
        public bool isServer => netChildIdentity.isServer;
        /// <summary>
        /// Returns true in client context if this game
        /// object has been spawned by the server.
        /// </summary>
        public bool isClient => netChildIdentity.isClient;
        /// <summary>
        /// Returns true on the client if this game object
        /// represents the player created for this client.
        /// </summary>
        public bool isLocalPlayer => netChildIdentity.isLocalPlayer;
        /// <summary>
        /// Returns true on the client if this client has
        /// authority over this game object. It is meaningless in server context.
        /// </summary>
        public bool hasAuthority => netChildIdentity.hasAuthority;
        /// <summary>
        /// The Manager that spawned this NetworkChildIdentity.
        /// </summary>
        public NetworkChildManager manager => netChildIdentity.manager;
        /// <summary>
        /// Messenger that this Behaviour can use to call ClientRpcs.
        /// </summary>
        public NetworkMessenger messenger => netChildIdentity.messenger;
        /// <summary>
        /// Unique child ID for this NetworkIdentity.
        /// </summary>
        public uint childID => netChildIdentity.childID;


        /// <summary>
        /// Called on server when a game object spawns on the server,
        /// or when the server is started for game objects in the Scene.
        /// </summary>
        public virtual void OnStartServer() { }
        /// <summary>
        /// Called on server when a game object is destroyed on the server,
        /// or when the server is stopped for game objects in the Scene.
        /// </summary>
        public virtual void OnStopServer() { }
        /// <summary>
        /// Called on clients when the game object spawns on the client,
        /// or when the client connects to a server for game objects in the Scene.
        /// </summary>
        public virtual void OnStartClient() { }
        /// <summary>
        /// Called on clients when the server destroys the game object.
        /// </summary>
        public virtual void OnStopClient() { }
        /// <summary>
        /// Called on clients after OnStartClient for the player game
        /// object on the local client
        /// </summary>
        public virtual void OnStartLocalPlayer() { }
        /// <summary>
        /// Called on clients before OnStopClient for the player
        /// game object on the local client
        /// </summary>
        public virtual void OnStopLocalPlayer() { }
        /// <summary>
        /// Called on owner client when assigned authority by the server.
        /// hasAuthority will be true for such objects in client context.
        /// </summary>
        public virtual void OnStartAuthority() { }
        /// <summary>
        /// Called on owner client when authority is removed by the server.
        /// </summary>
        public virtual void OnStopAuthority() { }


        // Called 0th
        protected virtual void Awake()
        {
            netChildIdentity = GetComponent<NetworkChildIdentity>();
            Assert.IsNotNull($"No {nameof(NetworkChildIdentity)} was attached to " +
                $"{name} but is required by {GetType().Name}");
        }
    }
}
