using Mirror;

using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Syncs sounds using wwise over the network.
    /// Expected to be attached to a spawned <see cref="NetworkChildIdentity"/> such
    /// as a weapon projectile.
    /// See <seealso cref="NetworkWwiseEventManager"/> for the version to be
    /// attached to <see cref="NetworkIdentity"/>.
    /// </summary>
    public class NetworkChildWwiseEventManager : NetworkChildBehaviour
    {
        private const bool IS_DEUBBING = true;

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
                #region Logs
                CustomDebug.LogForComponent($"Posting AKSound event " +
                    $"{eventName.wwiseEventName} from {eventName.name}" +
                    $"({eventName.GetType().Name})", this, IS_DEUBBING);
                #endregion Logs
                AkSoundEngine.PostEvent(eventName.wwiseEventName, owningObj);
            }
            // Send a message to play the event on all the clients.
            WwiseEventClientData temp_clientData = new WwiseEventClientData(
                eventName.wwiseEventName, temp_pathToOwningObj);
            messenger.SendMessageToClient(gameObject,
                nameof(PostEventOnClientMessage), temp_clientData);
        }
        /// <summary>
        /// Posts the event for the clients (except the host).
        /// </summary>
        /// <param name="clientData">Data wrapping the eventName and
        /// pathToOwner.</param>
        private void PostEventOnClientMessage(WwiseEventClientData clientData)
        {
            // Don't double invoke on host
            if (isServer) { return; }

            #region Logs
            CustomDebug.LogForComponent($"Posting AKSound event " +
                $"{clientData.eventName}", this, IS_DEUBBING);
            #endregion Logs

            Transform temp_owningObj = clientData.pathToOwner.Traverse(transform);
            AkSoundEngine.PostEvent(clientData.eventName,
                temp_owningObj.gameObject);
        }
    }
}
