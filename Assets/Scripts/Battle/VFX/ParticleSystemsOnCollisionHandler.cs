using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Handles attached ParticleSystems on a projectile after entering a collision.
    /// </summary>
    public class ParticleSystemsOnCollisionHandler : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        // What action will take place on ParticleSystems whose index does not match a serialized play or stop toggle.
        [SerializeField] private PSystemAction m_defaultStartCollisionAction;
        [SerializeField] private PSystemAction m_defaultEndCollisionAction;
        // List of ParticleSystems to affect on collision.
        [SerializeField] private List<ParticleSystem> m_pSystems = null;
        // Lists for what action to perform on the ParticleSystem at the given index at the start and end of collision
        [SerializeField] private List<PSystemAction> m_StartCollisionActions;
        [SerializeField] private List<PSystemAction> m_EndCollisionActions;


        private enum PSystemAction { none, play, stop };
        private void Awake()
        {
            Assert.IsNotNull(m_pSystems, $"{name} does not have any attached {m_pSystems.GetType()}.");
        }

        private void OnCollisionEnter(Collision collision)
        {
            ActionsOverPSystems(m_defaultStartCollisionAction, m_StartCollisionActions, m_pSystems);
        }
        private void OnCollisionExit(Collision collision)
        {
            ActionsOverPSystems(m_defaultEndCollisionAction, m_EndCollisionActions, m_pSystems);
        }

        /// <summary>
        /// Handles actions on a list of ParticleSystems.
        /// </summary>
        /// <param name="defaultAction">What action will be performed if the given index is uninitialized.</param>
        /// <param name="actions">Define how a ParticleSystem will be affected.</param>
        /// <param name="pSystems">The list of ParticleSystems being affected.</param>
        private void ActionsOverPSystems(PSystemAction defaultAction, List<PSystemAction> actions, List<ParticleSystem> pSystems)
        {
            if (m_pSystems != null && m_pSystems.Count > 0)
            {
                int i = 0;
                foreach (ParticleSystem pSystem in pSystems)
                {
                    ++i;
                    if (m_EndCollisionActions.Count < i)
                    { ActionOnPSystem(defaultAction, pSystem); }
                    else
                    { ActionOnPSystem(actions[i - 1], pSystem); }
                }
            }
        }
        /// <summary>
        /// Takes in a PSystemAction and executes its corresponding effect on a ParticleSystem.
        /// </summary>
        /// <param name="action">Defines how the ParticleSystem will be affected.</param>
        /// <param name="pSystem">The ParticleSystem being affected.</param>
        private void ActionOnPSystem(PSystemAction action, ParticleSystem pSystem)
        {
            switch(action)
            {
                case PSystemAction.play:
                    pSystem.Play();
                    break;
                case PSystemAction.stop:
                    pSystem.Stop();
                    break;
            }
        }
    }
}
