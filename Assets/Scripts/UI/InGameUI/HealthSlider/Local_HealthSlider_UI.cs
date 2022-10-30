using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik (based on Shelby's original HealthSlider_UI).

namespace DuolBots
{
    [RequireComponent(typeof(Shared_HealthSlider_UI))]
    public class Local_HealthSlider_UI : MonoBehaviour
    {
        private Shared_HealthSlider_UI m_sharedController = null;


        private void Awake()
        {
            m_sharedController = GetComponent<Shared_HealthSlider_UI>();
            Assert.IsNotNull(m_sharedController, $"{name}'s {GetType().Name} " +
                $"requires {nameof(Shared_HealthSlider_UI)} but none was found.");
        }
        // Start is called before the first frame update
        private void Start()
        {
            // With the local variant, we initialize on start
            InitializeHealthSlider();
        }


        private void InitializeHealthSlider()
        {
            RobotHelpersSingleton temp_robotHelpers = RobotHelpersSingleton.instance;
            // Get the team with index 0 in local variant
            GameObject temp_botRoot = temp_robotHelpers.FindBotRoot(0);
            m_sharedController.Initialize(temp_botRoot);
        }
    }
}
