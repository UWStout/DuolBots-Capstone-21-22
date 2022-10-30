using System;
using UnityEngine;
// Original Authors - Aaron Duffey, Wyatt Senalik, and Skyler Grabowsky

namespace DuolBots
{
    [RequireComponent(typeof(Shared_OnChargeHandler))]
    public class Shared_HornChargeHandler : MonoBehaviour
    {
        [SerializeField] private ParticleSystem m_partSys = null;

        private Shared_OnChargeHandler m_sharedHandler = null;

        public ParticleSystem partSys => m_partSys;


        // Domestic Initialization
        private void Awake()
        {
            m_sharedHandler = GetComponent<Shared_OnChargeHandler>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_sharedHandler, this);
            #endregion Asserts
        }
        // Foreign Initialization
        private void Start()
        {
            // Start off not charging
            StopCharging();
        }


        public void ToggleSubscription(bool cond, Action onStartCharge,
            Action onStopCharge)
        {
            m_sharedHandler.ToggleSubscription(cond, onStartCharge, onStopCharge);
        }
        public void StartCharging()
        {
            if (m_partSys == null)
            {
                Debug.LogError($"Tried to start charging with no particle system");
                return;
            }

            m_partSys.Play();
        }
        public void StopCharging()
        {
            // Okay if we try to stop playing after some things have been
            // destroyed.
            if (m_partSys == null) { return; }

            m_partSys.Stop();
        }
    }
}
