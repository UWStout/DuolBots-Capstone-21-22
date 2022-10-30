using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
namespace DuolBots {
    public class DestroyObjectsAfterDamage : NetworkBehaviour
    {
        // Part Health script for this object
        private PartHealth m_PartHealth = null;

        /// <summary>
        /// This is the Start life cycle function for networked objects
        /// </summary>
        public override void OnStartServer()
        {
            // Call the Super of this function.
            base.OnStartServer();

            // Get the Part health script on this object and verify that it has one.
            m_PartHealth = GetComponent<PartHealth>();
            CustomDebug.AssertComponentIsNotNull(m_PartHealth, this);

            // Subsrcibe to the event for health changing.
            m_PartHealth.onHealthChanged += CheckForNoHealth;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            if (m_PartHealth != null)
            {
                m_PartHealth.onHealthChanged += CheckForNoHealth;
            }

        }

        private void CheckForNoHealth(float curHealth)
        {
            if (curHealth <= 0)
            {
                NetworkServer.Destroy(gameObject);
            }
        }

        
    }
}
