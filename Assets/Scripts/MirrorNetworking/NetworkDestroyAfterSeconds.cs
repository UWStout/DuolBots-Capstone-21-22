using UnityEngine;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// <see cref="DestroyAfterSeconds"/> but for <see cref="NetworkIdentity"/>s.
    /// </summary>
    public class NetworkDestroyAfterSeconds : NetworkBehaviour
    {
        // After how many seconds do we destroy this object
        [SerializeField] private float m_secondsToLive = 10.0f;


        public override void OnStartServer()
        {
            base.OnStartServer();

            Invoke(nameof(DestroySelf), m_secondsToLive);
        }


        [Server]
        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
