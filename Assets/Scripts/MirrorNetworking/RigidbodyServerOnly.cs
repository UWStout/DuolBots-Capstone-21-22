using UnityEngine;
using UnityEngine.Assertions;

using Mirror;

namespace DuolBots.Mirror
{
    /// <summary>
    /// Destroys the Rigidbody on non-host clients.
    /// Use for when all physics will be done on the server and
    /// its okay for only the transform information to be given
    /// to the client.
    /// </summary>
    [RequireComponent(typeof(NetworkTransform))]
    public class RigidbodyServerOnly : NetworkBehaviour
    {
        public override void OnStartClient()
        {
            base.OnStartClient();

            // If Host
            if (isServer) { return; }

            // Get rid of the Rigidbody if we are not on the server.
            // The NetworkTransform will relay transform information.
            Rigidbody temp_rb = GetComponent<Rigidbody>();
            Assert.IsNotNull(temp_rb, $"{name}'s {GetType().Name} requires " +
                $"a {nameof(Rigidbody)} but none was found.");
            Destroy(temp_rb);
        }
    }
}
