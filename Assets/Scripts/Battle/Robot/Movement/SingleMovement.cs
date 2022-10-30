using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuolBots
{
    public class SingleMovement : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        public bool IsGrounded => m_grounded;
        private bool m_grounded = false;

        public bool IsOiled => m_oiled;
        private bool m_oiled = false;

        [SerializeField] private int m_totalTriggers;
        private int m_triggerContact = 0;

        private void Update()
        {
            CustomDebug.Log($"Number of {transform.parent.name}'s triggers touching ground: {m_triggerContact}", IS_DEBUGGING);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ground"))
            {
                m_triggerContact++;
                if (m_triggerContact >= m_totalTriggers)
                {
                    m_grounded = true;
                }
            }

            if (other.CompareTag("Oil"))
            {
                m_oiled = true;
                StopAllCoroutines();
                CustomDebug.Log($"{transform.parent.name} is now oiled", IS_DEBUGGING);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Ground"))
            {
                m_triggerContact--;
                if (m_triggerContact < 0)
                {
                    Debug.LogWarning($"SingleMovement of {transform.parent.name} is unexpectedly touching less than 0 ground colliders");
                }
                if (m_triggerContact < m_totalTriggers)
                {
                    m_grounded = false;
                }
            }

            if (other.CompareTag("Oil"))
            {
                CustomDebug.Log($"{transform.parent.name} is no longer touching the oil", IS_DEBUGGING);
                StartCoroutine(BeginRemoveOil());
            }
        }

        private IEnumerator BeginRemoveOil()
        {
            yield return new WaitForSeconds(MovementConstants.OIL_RECOVERY_TIME);
            m_oiled = false;
            CustomDebug.Log($"{transform.parent.name} is no longer oiled", IS_DEBUGGING);
        }
    }
}
