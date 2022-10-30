using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Like a NetworkBehaviour, but only meant to exist on the client.
    /// Will destroy itself on server-side (but not on host).
    /// </summary>
    public class ClientNetworkBehaviour : NetworkBehaviour
    {
        // Stub out all the server functions with sealed so that the
        // behaviour that extends this does not have the option of extending them.
        public sealed override void OnStartServer()
        {
            base.OnStartServer();
            // When something that is not the client has this
            // (a server or host), destroy it.
            if (!isClient)
            {
                Destroy(this);
            }
        }
        public sealed override void OnStopServer() => base.OnStopServer();
    }
}
