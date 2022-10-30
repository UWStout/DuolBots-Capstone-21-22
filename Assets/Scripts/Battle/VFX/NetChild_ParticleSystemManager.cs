using System;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Network manager for playing ParticleSystems.
    /// </summary>
    public class NetChild_ParticleSystemManager : NetworkChildBehaviour
    {
        private IParticleSystemPlayer[] m_pSystemPlayers =
            new IParticleSystemPlayer[1];
        
        public override void OnStartServer()
        {
            base.OnStartServer();

            // Subscribe to event of each IParticleSystemPlayer
            m_pSystemPlayers = GetComponentsInChildren<IParticleSystemPlayer>(true);
            CustomDebug.AssertIndexIsInRange(0, m_pSystemPlayers, this);
            
            for(int i=0; i<m_pSystemPlayers.Length; ++i)
            {
                IParticleSystemPlayer temp_player = m_pSystemPlayers[i];
                temp_player.onPlayPSystem += PlayPSystemServer;
            }

        }
        public override void OnStopServer()
        {
            base.OnStopServer();

            // Unsubscribe to event of each IParticleSystemPlayer
            foreach (IParticleSystemPlayer iPSysPlayer in m_pSystemPlayers)
            {
                // In case the object was destroyed before this gets called.
                if(iPSysPlayer != null)
                iPSysPlayer.onPlayPSystem -= PlayPSystemServer;

            }
        }

        private void PlayPSystemServer(IReadOnlyList<ParticleSystem> pSystems)
        {
            TransformChildPath[] temp_pathsToPSystems =
                new TransformChildPath[pSystems.Count];
            for (int i = 0; i < pSystems.Count; ++i)
            {
                ParticleSystem temp_pSys = pSystems[i];
                temp_pathsToPSystems[i] = new TransformChildPath(transform,
                    temp_pSys.transform);

                if (temp_pSys.isPlaying) { continue; }
                temp_pSys.Play();
            }

            messenger.SendMessageToClient(gameObject,
                nameof(PlayPSystemOnClientMessage), temp_pathsToPSystems);
        }


        private void PlayPSystemOnClientMessage(
            TransformChildPath[] pathsToPSystems)
        {
            if (isServer) { return; }
            foreach(TransformChildPath path in pathsToPSystems)
            {
                Transform temp_particleTrans = path.Traverse(transform);
                ParticleSystem temp_pSystem = temp_particleTrans.
                    GetComponent<ParticleSystem>();
                #region Asserts
                CustomDebug.AssertComponentOnOtherIsNotNull(temp_pSystem,
                    temp_particleTrans.gameObject, this);
                #endregion Asserts
                if (!temp_pSystem.isPlaying)
                {
                    temp_pSystem.Play();
                }
            }
        }
    }
}
