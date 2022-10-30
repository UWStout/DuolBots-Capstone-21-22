using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik and Aaron Duffey

namespace DuolBots.Mirror
{
    [RequireComponent(typeof(Shared_ParticleSystemsOnFireHandler))]
    public class Network_ParticleSystemsOnFireHandler : NetworkChildBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        private Shared_ParticleSystemsOnFireHandler m_sharedCont = null;


        // Domestic Initialization
        protected override void Awake()
        {
            base.Awake();

            m_sharedCont = GetComponent<Shared_ParticleSystemsOnFireHandler>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_sharedCont, this);
            #endregion Asserts
        }
        public override void OnStartServer()
        {
            base.OnStartServer();

            m_sharedCont.Initialize();
            // Shared controller will automatically play
            // particle systems on its own after being initialized.
            m_sharedCont.onPlayParticleSystems
                += ServerTellClientsToPlayParticleSystems;
        }
        public override void OnStopServer()
        {
            base.OnStopServer();

            if (m_sharedCont != null)
            {
                m_sharedCont.onPlayParticleSystems
                    -= ServerTellClientsToPlayParticleSystems;
            }
        }


        /// <summary>
        /// The server sends a message to all the clients to play
        /// their particle systems.
        /// </summary>
        private void ServerTellClientsToPlayParticleSystems()
        {
            messenger.SendMessageToClient(gameObject,
                nameof(PlayParticleSystemsOnClientMessage));
        }
        /// <summary>
        /// Message to be called on the client to play their particle systems.
        /// </summary>
        private void PlayParticleSystemsOnClientMessage()
        {
            // Host has already played their particle systems, don't
            // play them twice.
            if (isServer) { return; }

            m_sharedCont.PlayAllParticleSystems();
        }
    }
}
