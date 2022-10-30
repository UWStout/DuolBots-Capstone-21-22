using System.Collections;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    public class NetworkChild_PlayParticleSystemsOnDamageDealt :
        NetworkChildBehaviour
    {
        [SerializeField] [Required] private DamageDealer m_damageDealer = null;
        [SerializeField]
        private ParticleSystem[] m_partSystems = new ParticleSystem[1];
        [SerializeField] [Min(0.0f)] private float m_paticlePlayTime = 0.2f;

        private float m_timeToWaitUntil = 0.0f;
        private bool m_isWaiting = false;


        // Domestic Initialization
        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_damageDealer,
                nameof(m_damageDealer), this);
            #endregion Asserts
        }
        public override void OnStartServer()
        {
            base.OnStartServer();

            m_damageDealer.onDamageDealt += OnDamageDealt;
        }
        public override void OnStopServer()
        {
            base.OnStopServer();

            if (m_damageDealer != null)
            {
                m_damageDealer.onDamageDealt -= OnDamageDealt;
            }
        }


        private void OnDamageDealt(float dmgDealt)
        {
            // No damage was actually dealt
            if (dmgDealt <= 0.0f) { return; }

            ServerSendPlayPartSysMessageToClients();
            // Set the time to wait until before stopping the
            // particle systems.
            m_timeToWaitUntil = Time.time + m_paticlePlayTime;
            // If we aren't currently waiting, start waiting.
            if (!m_isWaiting)
            {
                StartCoroutine(StopParticleSystemsAfterWaitCoroutine());
            }
        }
        private IEnumerator StopParticleSystemsAfterWaitCoroutine()
        {
            m_isWaiting = true;

            while (Time.time < m_timeToWaitUntil)
            {
                yield return null;
            }
            ServerSendStopPartSysMessageToClients();

            m_isWaiting = false;
        }


        private void ServerSendPlayPartSysMessageToClients()
        {
            messenger.SendMessageToClient(gameObject,
                nameof(PlayParticleSystemClientMessage));
        }
        private void PlayParticleSystemClientMessage()
        {
            foreach (ParticleSystem temp_pSys in m_partSystems)
            {
                temp_pSys.Play();
            }
        }
        private void ServerSendStopPartSysMessageToClients()
        {
            messenger.SendMessageToClient(gameObject,
                nameof(StopParticleSystemClientMessage));
        }
        private void StopParticleSystemClientMessage()
        {
            foreach (ParticleSystem temp_pSys in m_partSystems)
            {
                temp_pSys.Stop();
            }
        }
    }
}
