using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Destroys the specified component on clients on OnStartClient.
    /// </summary>
    public class ServerOnlyComponentNetworkChild : NetworkChildBehaviour
    {
        [Tooltip("Component to Destroy on non-host clients")]
        [SerializeField] private Component m_serverOnlyComponent = null;


        public override void OnStartClient()
        {
            base.OnStartClient();

            // If Host
            if (isServer) { return; }

            // Get rid of the specified component.
            Destroy(m_serverOnlyComponent);
            // No need to keep this component around either.
            Destroy(this);
        }
    }
}
