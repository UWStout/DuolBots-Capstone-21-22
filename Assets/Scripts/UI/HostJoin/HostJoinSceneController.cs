using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    public class HostJoinSceneController : MonoBehaviour
    {
        public static bool wasDisconnected { get; set; } = false;

        [SerializeField] [Required]
        private DuolPlayerMenuSetup m_defaultMenuSetup = null;
        [SerializeField] [Required]
        private DuolPlayerMenuSetup m_disonnectedMenuSetup = null;
        [SerializeField] [Required] private GameObject m_disconnectedPopup = null;
        [SerializeField] [Tag] private string m_playerInpTag = "PlayerInput";
        [SerializeField] [Scene] private string m_mainMenuSceneName = "NewMainMenu";
        [SerializeField] [Required] private GameObject m_battleNetMan = null;


        // Domestic Initialization
        private void Awake()
        {
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_defaultMenuSetup,
                nameof(m_defaultMenuSetup), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_disonnectedMenuSetup,
                nameof(m_disonnectedMenuSetup), this);
            #endregion Asserts
        }
        // Foreign Initialization
        private void Start()
        {
            // If we the player was disconnected before coming to this scene,
            // then display the popup.
            if (wasDisconnected)
            {
                m_disconnectedPopup.SetActive(true);
                m_disonnectedMenuSetup.SetupMenu();
                // Reset the bool.
                wasDisconnected = false;
            }
            // We weren't disconnected, so just setup the menu normally.
            else
            {
                m_defaultMenuSetup.SetupMenu();
            }
        }


        public void GoToMainMenu()
        {
            // Destroy the player objects since new ones will be created in
            // the player join section of the main menu.
            GameObject[] temp_playerObjs = GameObject.
                FindGameObjectsWithTag(m_playerInpTag);
            foreach (GameObject temp_obj in temp_playerObjs)
            {
                Destroy(temp_obj);
            }
            // Destroy the battle network manager since its don't destroy on load.
            Destroy(m_battleNetMan);
            // Load the main menu scene.
            SceneLoader temp_sceneLoader = SceneLoader.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(temp_sceneLoader,
                this);
            #endregion Asserts
            temp_sceneLoader.LoadScene(m_mainMenuSceneName);
        }
    }
}
