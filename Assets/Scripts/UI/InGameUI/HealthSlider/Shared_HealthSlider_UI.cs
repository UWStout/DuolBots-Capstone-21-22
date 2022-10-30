using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using NaughtyAttributes;
// Original Authors - Shelby Vian

namespace DuolBots
{
    /// <summary>
    /// Health bar for the robot.
    /// </summary>
    public class Shared_HealthSlider_UI : MonoBehaviour
    {
        // Visual bar for the health
        [SerializeField] private Slider m_healthSlider = null;
        // Tag for robot
        [SerializeField] [Tag] private string m_robotTag = "Robot";

        // Reference to the RobotHealth script attached to the robot this health slider
        // is associated with.
        private IRobotHealth m_health = null;

        private bool m_isInitialized = false;


        // Called when the component or gameobject is destroyed
        private void OnDestroy()
        {
            // Only do destroy logic, if we are initialized.
            if (!m_isInitialized) { return; }

            // Its possible that RobotHealth has been destroyed, say if we are
            // scene transitioning and RobotHealth is destroyed before this
            if (m_health != null)
            {
                m_health.onHealthChanged -= SetCurrentHealth;
            }
        }


        public void Initialize(GameObject robotObject)
        {
            m_health = robotObject.GetComponent<IRobotHealth>();
            Assert.IsNotNull(m_health, $"There was not {typeof(IRobotHealth).Name} attached to" +
                $" the found robot ({robotObject.name}) with tag={m_robotTag}");

            // Set max health
            SetMaxHealth(m_health.maxHealth);

            // Set up callback so that SetCurrentHealth is called whenever health is changed
            m_health.onHealthChanged += SetCurrentHealth;

            m_isInitialized = true;
        }


        /// <summary>
        /// Initializes the slider by setting its maximum value to the robot's maximum health.
        /// Also sets the health bar to be full.
        ///
        /// Pre Conditions - Given maxHealth must be greater than 0.
        /// Post Conditions - Sets the health slider's max value to the given value and sets
        /// its full percentage to 100.
        /// </summary>
        /// <param name="maxHealth">Max health for the associated robot.</param>
        private void SetMaxHealth(float maxHealth)
        {
            m_healthSlider.maxValue = maxHealth;
            m_healthSlider.value = m_healthSlider.maxValue;
        }
        /// <summary>
        /// Updates the slider's current value to be the current health value.
        /// Driven by the RobotHealth's onHealthChanged event.
        ///
        /// Pre Conditions - Given currentHealth must be >= 0 and <= the value last past into
        /// maximum health.
        /// Post Conditions - Health slider is updated to be currentHealth/maxHealth * 100 % full.
        /// </summary>
        /// <param name="currentHealth">Current health of the robot this is associated with.</param>
        private void SetCurrentHealth(float currentHealth)
        {
            m_healthSlider.value = currentHealth;
        }
    }
}

