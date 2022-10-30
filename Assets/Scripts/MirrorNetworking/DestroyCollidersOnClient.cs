using UnityEngine;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Destroys colliders that are on this object or children of
    /// this object in its hierarchy for the client.
    /// We most likely don't need colliders on the client since the
    /// server is supposed to do all the physics calculations.
    /// 
    /// This variant is meant to be attached to a
    /// <see cref="NetworkIdentity"/>. See also the variant meant to be
    /// attached to a <see cref="NetworkChildIdentity"/> which is similarly called
    /// <see cref="DestroyCollidersOnClientNetworkChild"/>.
    /// </summary>
    public class DestroyCollidersOnClient : NetworkBehaviour
    {
        public override void OnStartClient()
        {
            base.OnStartClient();

            // Host
            if (isServer) { return; }

            Collider[] temp_allCols = GetComponentsInChildren<Collider>();
            foreach (Collider temp_singleCol in temp_allCols)
            {
                Destroy(temp_singleCol);
            }
        }
    }
}
