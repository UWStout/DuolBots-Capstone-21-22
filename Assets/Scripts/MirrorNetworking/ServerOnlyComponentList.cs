using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Destroys the specified component on clients on OnStartClient.
    /// </summary>
    public class ServerOnlyComponentList : NetworkBehaviour
    {
        private bool IS_DEBUGGING = false;

        [Tooltip("Components to Destroy on non-host clients")]
        [SerializeField] private Component[] m_serverOnlyComponentList = null;


        public override void OnStartClient()
        {
            base.OnStartClient();

            CustomDebug.Log($"{nameof(ServerOnlyComponentListNetworkChild)}'s " +
                $"{nameof(OnStartClient)} {(isServer ? "Server" : "Client")}",
                IS_DEBUGGING);
            // If Host
            if (isServer) { return; }

            // Get rid of the specified components.
            foreach (Component temp_comp in m_serverOnlyComponentList)
            {
                Destroy(temp_comp);
            }
            // No need to keep this component around either.
            Destroy(this);
        }
    }
}
