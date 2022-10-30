using System;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Handles PartcileSystems when a weapon fires.
    /// </summary>
    public class Shared_ParticleSystemsOnFireHandler : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        // List of ParticleSystems to affect on collision.
        [SerializeField] private List<ParticleSystem> m_pSystems
            = new List<ParticleSystem>();
        // Lists for which ParticleSystems to turn on or off on collision
        // '(System affected is determined by m_pSystems order of initalization).
        [SerializeField] private List<bool> m_playOnFireToggles
            = new List<bool>();
        // Used to listen for weapon firing events.
        private IFireEvent m_firingListener = null;

        public event Action onPlayParticleSystems;


        // Domestic Initialization
        private void Awake()
        {
            m_firingListener = GetComponent<IFireEvent>();
            #region Asserts
            CustomDebug.AssertIComponentIsNotNull(m_firingListener, this);
            #endregion

            foreach(ParticleSystem pSystem in m_pSystems)
            {
                pSystem.Stop();
                // This works because ParticleSystem.MainModule is an interface
                // that is not an independent object
                // - "There's some magic going on here - Wyatt"
                ParticleSystem.MainModule temp_pSModule = pSystem.main;
                temp_pSModule.loop = false;
            }
        }
        private void OnDestroy()
        {
            // Just in case the object has already been destroyed before
            // OnDestroy() is called.
            if (m_firingListener != null)
            {
                m_firingListener.onFire -= PlayAllParticleSystems;
            }
        }


        public void Initialize()
        {
            m_firingListener.onFire += PlayAllParticleSystems;
        }
        public void PlayAllParticleSystems()
        {
            HandlePSystemsOnFire();
            onPlayParticleSystems?.Invoke();
        }


        private void HandlePSystemsOnFire()
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(HandlePSystemsOnFire), this,
                IS_DEBUGGING);
            #endregion Logs
            if (m_pSystems != null && m_pSystems.Count > 0)
            {
                #region Logs
                CustomDebug.LogForComponent($"Going to try and play " +
                    $"{m_pSystems.Count} particle systems", this, IS_DEBUGGING);
                #endregion Logs
                for (int i = 0; i < m_playOnFireToggles.Count; ++i)
                {
                    if (m_playOnFireToggles[i])
                    {
                        CustomDebug.AssertIndexIsInRange(i, m_pSystems, this);
                        if (!m_pSystems[i].isPlaying)
                        {
                            #region Logs
                            CustomDebug.LogForComponent($"{i}th particle system " +
                                $"will be played", this, IS_DEBUGGING);
                            #endregion Logs
                            m_pSystems[i].Play();
                        }
                        else
                        {
                            #region Logs
                            CustomDebug.LogForComponent($"{i}th particle system " +
                                $"is already playing", this, IS_DEBUGGING);
                            #endregion Logs
                        }
                    }
                }
            }
        }
    }
}
