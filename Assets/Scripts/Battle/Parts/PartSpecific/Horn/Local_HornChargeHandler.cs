using UnityEngine;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    [RequireComponent(typeof(Shared_HornChargeHandler))]
    public class Local_HornChargeHandler : MonoBehaviour
    {
        private Shared_HornChargeHandler m_sharedHandler = null;


        // Domestic Initialization
        private void Awake()
        {
            m_sharedHandler = GetComponent<Shared_HornChargeHandler>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_sharedHandler, this);
            #endregion Asserts
        }
        private void OnEnable()
        {
            m_sharedHandler.ToggleSubscription(true, StartCharging, StopCharging);
        }
        private void OnDisable()
        {
            m_sharedHandler.ToggleSubscription(false, StartCharging, StopCharging);
        }


        private void StartCharging() => m_sharedHandler.StartCharging();
        private void StopCharging() => m_sharedHandler.StopCharging();
    }
}
