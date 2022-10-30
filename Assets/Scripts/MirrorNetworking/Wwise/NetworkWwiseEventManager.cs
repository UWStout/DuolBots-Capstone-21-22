using UnityEngine;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Syncs sounds using wwise over the network.
    /// Expected to be attached to a spawned <see cref="NetworkIdentity"/> such as a
    /// weapon projectile.
    /// See <seealso cref="NetworkChildWwiseEventManager"/> for the version to be
    /// attached to <see cref="NetworkChildIdentity"/>.
    /// </summary>
    public class NetworkWwiseEventManager : NetworkBehaviour
    {
        [SerializeField] private bool m_includeInactiveChildren = true;

        private IWwiseEventInvoker[] m_wwiseEventInvokers = null;


        public override void OnStartServer()
        {
            base.OnStartServer();

            // Find all IWwiseEventInvokers in children so that each one does not
            // have to explicitly find this script in its parent or use a
            // GetComponent for it.
            m_wwiseEventInvokers = GetComponentsInChildren
                <IWwiseEventInvoker>(m_includeInactiveChildren);
            // Subscribe to all the request for invokation.
            foreach (IWwiseEventInvoker temp_eventInvoker in m_wwiseEventInvokers)
            {
                temp_eventInvoker.requestInvokeWwiseEvent
                    += OnServerRequestedInvoke;
            }
        }
        public override void OnStopServer()
        {
            base.OnStopServer();

            // Unsubsribe to the requests for invokation.
            foreach (IWwiseEventInvoker temp_eventInvoker in m_wwiseEventInvokers)
            {
                // Possible if this object is destroy after that one,
                // totally okay.
                if (temp_eventInvoker == null) continue;

                temp_eventInvoker.requestInvokeWwiseEvent
                    += OnServerRequestedInvoke;
            }
        }


        /// <summary>
        /// Called when a <see cref="IWwiseEventInvoker"/> on the Server requests to
        /// invoke a Wwise event. Posts the event on the Host (if the Server is a
        /// Host) instantly and then tells the clients to also post the event.
        /// </summary>
        /// <param name="eventName">Wrapped name of the wwise event.</param>
        /// <param name="owningObj">GameObject that the event is to be
        /// posted for. Must be a descendant of this GameObject.</param>
        [Server]
        private void OnServerRequestedInvoke(WwiseEventName eventName,
            GameObject owningObj)
        {
            // Ensure that the given GameObject is a descendant of this GameObject
            // in the Unity Hierarchy.
            TransformChildPath temp_pathToOwningObj
                = new TransformChildPath(transform, owningObj.transform);
            #region Asserts
            CustomDebug.AssertIsTrueForComponent(temp_pathToOwningObj.isValid,
                $"Cannot invoke {eventName} with an owning object of " +
                $"{owningObj}. That owning object is not a child of {name}.",
                this);
            #endregion Asserts

            // Invoke the event on the host (if this is host)
            if (isClient)
            {
                AkSoundEngine.PostEvent(eventName.wwiseEventName, owningObj);
            }
            // Invoke the event on all the clients
            PostEventOnClients(eventName.wwiseEventName, temp_pathToOwningObj);

        }
        /// <summary>
        /// Posts the event for the clients (except the host).
        /// </summary>
        /// <param name="eventName">Name of the wwise event.</param>
        /// <param name="pathToOwner">Path to the tameObject that the event is to be
        /// posted for.</param>
        [ClientRpc]
        private void PostEventOnClients(string eventName,
            TransformChildPath pathToOwner)
        {
            // Don't double invoke on host
            if (isServer) { return; }

            Transform temp_owningObj = pathToOwner.Traverse(transform);
            AkSoundEngine.PostEvent(eventName, temp_owningObj.gameObject);
        }
    }
}
