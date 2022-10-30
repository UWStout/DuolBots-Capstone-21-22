using UnityEngine;
// Original Author - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Applies the given force to the attached rigidbody on awake.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class ApplyForceForwardOnAwake : MonoBehaviour
    {
        // Magnitude of the force to apply to the rigidbody on awake.
        [SerializeField] private float m_forceMagnitude = 500.0f;
        public float ForceMagnitude => m_forceMagnitude;


        // Domestic Initialization
        private void Awake()
        {
            Rigidbody temp_rigidbody = GetComponent<Rigidbody>();
            temp_rigidbody.AddForce(transform.forward * m_forceMagnitude);
        }
    }
}
