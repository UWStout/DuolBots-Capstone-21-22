using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Local version of SetBotCenterOfMass. Sets the bot's center of mass
    /// on start only on the server.
    /// </summary>
    [RequireComponent(typeof(SharedSetBotCenterOfMass))]
    public class NetworkSetBotCenterOfMass : ServerNetworkBehaviour
    {
        private Rigidbody m_rigidbody = null;
        private SharedSetBotCenterOfMass m_sharedSetBotCenterOfMass = null;
        private MovementPartColliders m_movementColliders = null;


        public override void OnStartServer()
        {
            base.OnStartServer();

            m_rigidbody = GetComponent<Rigidbody>();
            Assert.IsNotNull(m_rigidbody, $"{nameof(Rigidbody)} is required by " +
                $"{name}'s {GetType().Name} but none was found");

            m_sharedSetBotCenterOfMass = GetComponent<SharedSetBotCenterOfMass>();
            Assert.IsNotNull(m_sharedSetBotCenterOfMass,
                $"{nameof(SharedSetBotCenterOfMass)} is required by " +
                $"{name}'s {GetType().Name} but none was found");

            m_movementColliders = GetComponentInChildren<MovementPartColliders>();
            Assert.IsNotNull(m_movementColliders,
                $"{nameof(MovementPartColliders)} is required by " +
                $"{name}'s {GetType().Name} but none was found");

            m_sharedSetBotCenterOfMass.CalculateAndSetCenterOfMass(m_rigidbody,
                m_movementColliders);
        }
    }
}
