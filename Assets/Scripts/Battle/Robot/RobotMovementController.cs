using UnityEngine;
using UnityEngine.Assertions;
// Original Author - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// The controller that causes movement due to player input.
    /// This should be called from all movement parts.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class RobotMovementController : MonoBehaviour
    {
        // References
        private Rigidbody m_rigidbody = null;


        // Domestic Initialization
        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();

            Assert.IsNotNull(m_rigidbody, $"ERROR. Rigidbody in {name} is null!");
        }
    }
}
