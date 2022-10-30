using UnityEngine;
// Original Authors - Wyatt Senalik and Zach Gross

namespace DuolBots
{
    /// <summary>
    /// Controls a general movement component when not on a server
    /// </summary>
    [RequireComponent(typeof(SharedController_Movement))]
    public class LocalController_Movement : MonoBehaviour, IController_Movement
    {
        private SharedController_Movement m_sharedController = null;

        // Domestic Initialization
        private void Awake()
        {
            m_sharedController = GetComponent<SharedController_Movement>();
        }

        private void Update()
        {
            m_sharedController.Move();
        }

        // Passes the newTarget on to m_sharedController
        public void SetLeftTarget(float newTarget)
        {
            m_sharedController.SetLeftTarget(newTarget);
        }

        // Passes the newTarget on to m_sharedController
        public void SetRightTarget(float newTarget)
        {
            m_sharedController.SetRightTarget(newTarget);
        }
    }
}
