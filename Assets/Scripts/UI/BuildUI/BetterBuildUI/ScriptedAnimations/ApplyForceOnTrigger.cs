using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// When this enters a collider as a trigger,
    /// it applies the specified force.
    /// </summary>
    public class ApplyForceOnTrigger : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;
        HashSet<Rigidbody> m_List = new HashSet<Rigidbody>();

        [SerializeField] private Vector3 m_flipForce =
            new Vector3(0.0f, 20.0f, 0.0f);


        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("----------------------------------------------------------------------------------------------------------------------------------");
            CustomDebug.Log($"Collided with {other.attachedRigidbody.name}'s " +
                $"{other.name}", IS_DEBUGGING);
            
            if (!m_List.Contains(other.attachedRigidbody)){
                Debug.Log("Force Applied");
                m_List.Add(other.attachedRigidbody);
                other.attachedRigidbody.AddForce(m_flipForce);
                StartCoroutine(ClearList());
            }
        }

        private IEnumerator ClearList()
        {
            yield return new WaitForSeconds(0.33f);
            m_List = new HashSet<Rigidbody>();
        }
    }
}
