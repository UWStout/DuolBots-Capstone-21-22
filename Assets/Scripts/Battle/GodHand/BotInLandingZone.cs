using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Ben Lussman
// Tweaked by Wyatt Senalik and Aaron

namespace DuolBots
{
    public class BotInLandingZone : MonoBehaviour
    {
        [SerializeField, Tag] private string m_robotTag = "Robot";
        private int m_collidersInTrigger = 0;

        private void OnTriggerEnter(Collider other)
        {
            Rigidbody temp_otherRb = other.attachedRigidbody;
            if (temp_otherRb == null) { return; }
            if (!temp_otherRb.CompareTag(m_robotTag)) { return; }

            ++m_collidersInTrigger;
        }

        private void OnTriggerExit(Collider other)
        {
            Rigidbody temp_otherRb = other.attachedRigidbody;
            if (temp_otherRb == null) { return; }
            if (!temp_otherRb.CompareTag(m_robotTag)) { return; }

            --m_collidersInTrigger;
        }

        public bool AreBotsInArea()
        {
            return m_collidersInTrigger > 0;
        }
    }
}
