using UnityEngine;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace DuolBots.Mirror
{
    [RequireComponent(typeof(Shared_HornChargeHandler))]
    public class Network_HornChargeHandler : NetworkChildBehaviour
    {
        private Shared_HornChargeHandler m_sharedHandler = null;


        // Domestic Initialization
        protected override void Awake()
        {
            base.Awake();

            m_sharedHandler = GetComponent<Shared_HornChargeHandler>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_sharedHandler, this);
            #endregion Asserts
        }
        public override void OnStartServer()
        {
            base.OnStartServer();

            m_sharedHandler.ToggleSubscription(true, OnStartCharge, OnStopCharge);
        }
        public override void OnStopServer()
        {
            base.OnStopServer();

            m_sharedHandler.ToggleSubscription(false, OnStartCharge, OnStopCharge);
        }


        private void OnStartCharge()
        {
            messenger.SendMessageToClient(gameObject,
                nameof(OnStartHornChargeClient));
        }
        private void OnStopCharge()
        {
            messenger.SendMessageToClient(gameObject,
                nameof(OnStopHornChargeClient));
        }
        private void OnStartHornChargeClient()
        {
            m_sharedHandler.StartCharging();
        }
        private void OnStopHornChargeClient()
        {
            m_sharedHandler.StopCharging();
        }
    }
}
