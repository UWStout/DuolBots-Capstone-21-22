using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Script to control the turret part when not on a server.
    /// </summary>
    [RequireComponent(typeof(Specifications_Turret2Axis))]
    [RequireComponent(typeof(SharedController_Turret2Axis))]
    public class LocalController_Turret2Axis : MonoBehaviour, IController_Turret2Axis
    {
        // Specifications for variables
        private Specifications_Turret2Axis m_specifications = null;
        // Shared controller that will rotate/raise the turret transforms
        private SharedController_Turret2Axis m_sharedController = null;

        // Current input values
        private float m_curRotateInp = 0.0f;
        private float m_curRaiseInp = 0.0f;


        // Domestic Initialization
        private void Awake()
        {
            m_specifications = GetComponent<Specifications_Turret2Axis>();
            Assert.IsNotNull(m_specifications, $"{typeof(LocalController_Turret2Axis).Name} was not on" +
                $" {name} but is required by {typeof(Specifications_Turret2Axis).Name}");

            m_sharedController = GetComponent<SharedController_Turret2Axis>();
            Assert.IsNotNull(m_sharedController, $"{typeof(SharedController_Turret2Axis).Name} was not on" +
                $" {name} but is required by {typeof(Specifications_Turret2Axis).Name}");
        }

        // Called once every frame
        private void Update()
        {
            RotateTurret();
            RaiseBarrel();
        }

        /// <summary>
        /// Changes the current rotation input by the given amount.
        /// This change will be applied on Update.
        /// </summary>
        /// <param name="rotInpChangeAmount">Amount to change the rotation input by.</param>
        public void ChangeRotationInput(float rotInpChangeAmount)
        {
            m_curRotateInp += rotInpChangeAmount;
        }
        /// <summary>
        /// Changes the current raise input by the given amount.
        /// This change will be applied on Update.
        /// </summary>
        /// <param name="raiseInpChangeAmount">Amount to change the raise input by.</param>
        public void ChangeRaiseInput(float raiseInpChangeAmount)
        {
            m_curRaiseInp += raiseInpChangeAmount;
        }

        /// <summary>
        /// Called on Update. Rotates the current based on current input.
        /// </summary>
        private void RotateTurret()
        {
            m_sharedController.RotateTurret(m_curRotateInp);
        }
        /// <summary>
        /// Called on Update. Raises the barrel based on the current input.
        /// </summary>
        private void RaiseBarrel()
        {
            m_sharedController.RaiseBarrel(m_curRaiseInp);
        }
    }
}
