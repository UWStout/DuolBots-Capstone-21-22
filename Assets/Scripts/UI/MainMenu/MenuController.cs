using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DuolBots
{
    public class MenuController : MonoBehaviour
    {
        // Main Menu Panels
        [Header("Main Menu Panels")]
        [SerializeField] private GameObject MainMenu = null;
        [SerializeField] private GameObject CreditsMenu = null;
        [SerializeField] private GameObject SettingsMenu = null;
        [SerializeField] private GameObject PlayerJoinMenu = null;
        [SerializeField] private GameObject QuitConfirmingMenu = null;

        private PlayerMenuControls m_controls = null;
        [SerializeField] private PlayerInput m_playerInput = null;

        private GameObject playerObject;
        [SerializeField] GameObject m_playerInputManager = null;
        private MenuStack m_menuStack = null;

        [SerializeField] private BackgroundVideoManager m_videoBlur = null;
        [SerializeField] private WindowManager m_windowManager = null;

        private PlayingSounds m_playingSounds = null;

        private void Awake()
        {
            m_controls = new PlayerMenuControls();
            playerObject = this.gameObject;
            m_playerInputManager.SetActive(false);
            m_menuStack = FindObjectOfType<MenuStack>();

            // Making sure on awake right menu controls are loaded
            m_controls.Credits.Disable();
            m_controls.PlayerJoin.Disable();
            m_controls.Settings.Disable();
            m_controls.QuitConfirm.Disable();

            // Credits Menu Button Functions
            m_controls.Credits.CreditsBack.performed += ctx => CreditsBack();
            //m_controls.Credits.CreditsUpPanel.performed += ctx => CreditsUpPanel();
            //m_controls.Credits.CreditsDownPanel.performed += ctx => CreditsDownPanel();

            // Settings Menu Button Fuctions
            //m_controls.Settings.LeftRes.performed += ctx => LeftResButton();
            //m_controls.Settings.RightRes.performed += ctx => RightResButton();
            /*
            m_controls.Settings.SettingsBackMainMenu.performed += ctx => SettingsBackToMain();
            
            m_controls.Settings.Apply.performed += ctx => ApplyButton();
            m_controls.Settings.FullscreenToggle.performed += ctx => ToggleFullscreen();
            */

            // Quit Confirming
            //m_controls.QuitConfirm.Quittng.performed += ctx => Quitting();
            //m_controls.QuitConfirm.DontQuit.performed += ctx => DontQuit();

            m_playingSounds = FindObjectOfType<PlayingSounds>();
            if(m_playingSounds == null) { return; }

        }
        private void OnEnable()
        {
            m_controls.Enable();
        }

        private void OnDisable()
        {
            m_controls.Disable();
        }



        /// <summary>
        /// Main Menu Functions
        /// TODO: still need blurred translations to each different menu with blurred out vid
        /// </summary>
        public void Play()
        {
            m_menuStack.OpenMenu(PlayerJoinMenu);
            m_videoBlur.SubMenuMaterialStats();
            m_controls.Credits.Disable();
            m_controls.PlayerJoin.Disable();
            m_controls.Settings.Disable();
            //m_controls.QuitConfirm.Disable();
            m_playerInputManager.SetActive(true);
            m_playerInputManager.GetComponent<PlayerInputManager>().EnableJoining();
            m_playerInput.gameObject.SetActive(false);
            m_playingSounds.SelectSound();
        }

        public void Settings()
        {
            m_menuStack.OpenMenu(SettingsMenu);
            m_videoBlur.SubMenuMaterialStats();
            m_controls.Credits.Disable();
            m_controls.PlayerJoin.Disable();
            //m_controls.Settings.Enable();
            m_controls.QuitConfirm.Disable();
            m_playingSounds.SelectSound();
        }

        public void Credits()
        {
            m_menuStack.OpenMenu(CreditsMenu);
            m_videoBlur.SubMenuMaterialStats();
            m_controls.Credits.Enable();
            m_controls.PlayerJoin.Disable();
            m_controls.Settings.Disable();
            m_controls.QuitConfirm.Disable();
            m_playingSounds.SelectSound();
        }

        public void Quit()
        {
            m_menuStack.OpenMenu(QuitConfirmingMenu);
            m_videoBlur.SubMenuMaterialStats();
            //m_controls.QuitConfirm.Enable();
            m_playingSounds.SelectSound();
        }

        public void Quitting()
        {
            m_playingSounds.SelectSound();
            Debug.Log("Quitting This Awesome Game");
            Application.Quit();

        }

        public void DontQuit()
        {
            m_menuStack.CloseCurrentMenu();
            m_videoBlur.MainMenuMaterialStats();
            //m_controls.QuitConfirm.Disable();
            m_playingSounds.SelectSound();
        }


        public void CreditsBack()
        {
            m_menuStack.CloseCurrentMenu();
            m_videoBlur.MainMenuMaterialStats();
            m_controls.Credits.Disable();
            m_controls.PlayerJoin.Disable();
            m_controls.Settings.Disable();
            m_controls.QuitConfirm.Disable();
            m_playingSounds.SelectSound();

        }

        public void ApplyButton()
        {
            m_windowManager.ApplyChanges();
            m_playingSounds.SelectSound();

        }

        void LeftResButton()
        {
            m_windowManager.SetPreviousResolution();
            m_playingSounds.MoveMenuSound();

        }

        void RightResButton()
        {
            m_windowManager.SetNextResolution();
            m_playingSounds.MoveMenuSound();
        }

        public void ToggleFullscreen()
        {
            m_windowManager.SwitchScreen();
            m_windowManager.SetToggle();
            m_playingSounds.SelectSound();

        }
        public void SettingsBackToMain()
        {
            m_menuStack.CloseCurrentMenu();
            m_videoBlur.MainMenuMaterialStats();
            m_controls.Settings.Disable();
            m_playingSounds.SelectSound();
        }

        public void BackToMainMenu()
        {
            m_videoBlur.MainMenuMaterialStats();
            m_playerInputManager.SetActive(false);
            m_playerInput.gameObject.SetActive(true);
            m_playingSounds.SelectSound();
        }
    }
}
