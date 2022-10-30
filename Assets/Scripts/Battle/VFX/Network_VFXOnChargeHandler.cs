using UnityEngine;
using UnityEngine.VFX;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik and Aaron Duffey

namespace DuolBots.Mirror
{
    /// <summary>
    /// Network version of having a VFX play over the network
    /// when a weapon starts charging.
    /// </summary>
    [RequireComponent(typeof(Shared_OnChargeHandler))]
    public class Network_VFXOnChargeHandler : NetworkChildBehaviour
    {
        [SerializeField] [Required] private VisualEffect m_vfx = null;

        private Shared_OnChargeHandler m_sharedHandler = null;
        private bool m_isSubbed = false;


        // Domestic Initialization
        protected override void Awake()
        {
            base.Awake();

            m_sharedHandler = GetComponent<Shared_OnChargeHandler>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_sharedHandler, this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_vfx, nameof(m_vfx), this);
            #endregion Asserts

            // Start inactive
            StopCharging();
        }
        public override void OnStartServer()
        {
            base.OnStartServer();

            // Subscribe
            m_sharedHandler.ToggleSubscription(true, StartChargingServer,
                StopChargingServer);
        }
        public override void OnStopServer()
        {
            base.OnStopServer();

            // Unsubscribe
            m_sharedHandler.ToggleSubscription(false, StartChargingServer,
                StopChargingServer);
        }


        private void StartChargingServer()
        {
            messenger.SendMessageToClient(gameObject, nameof(StartChargingClient));
        }
        private void StopChargingServer()
        {
            messenger.SendMessageToClient(gameObject, nameof(StopChargingClient));
        }
        private void StartChargingClient()
        {
            StartCharging();
        }
        private void StopChargingClient()
        {
            StopCharging();
        }
        private void StartCharging()
        {
            if (m_vfx != null)
            {
                m_vfx.Play();
            }
        }
        private void StopCharging()
        {
            if (m_vfx != null)
            {
                m_vfx.Stop();
            }
        }
    }
}
