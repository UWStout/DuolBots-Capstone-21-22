using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Like a NetworkBehaviour, but only meant to exist on the server.
    /// Will destroy itself on client-side (but not on host).
    /// </summary>
    public class ServerNetworkBehaviour : NetworkBehaviour
    {
        // Stub out all the client functions with sealed so that the
        // behaviour that extends this does not have the option of extending them.
        public sealed override void OnStartAuthority() => base.OnStartAuthority();
        public sealed override void OnStopAuthority() => base.OnStopAuthority();
        public sealed override void OnStartClient()
        {
            // When something that is not the server has this
            // (a non-host client), destroy it.
            if (!isServer)
            {
                Destroy(this);
            }
        }
        public sealed override void OnStopClient() => base.OnStopClient();
        public sealed override void OnStartLocalPlayer() => base.OnStartLocalPlayer();
    }
}
