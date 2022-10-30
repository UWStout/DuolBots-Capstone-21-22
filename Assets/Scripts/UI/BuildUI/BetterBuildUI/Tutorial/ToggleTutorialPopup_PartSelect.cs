using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Eslis Vang


namespace DuolBots
{
    /// <summary>
    /// Toggles the tutorial popups based on state.
    /// </summary>
    [RequireComponent(typeof(TutorialPopupSettings_PartSelect))]
    public class ToggleTutorialPopup_PartSelect : MonoBehaviour
    {
        private BetterBuildSceneStateManager m_stateMan = null;
        private BetterBuildSceneStateChangeHandler m_popupHandler = null;
        private TutorialPopupSettings_PartSelect m_popupSettings = null;
        public TutorialPopupSettings_PartSelect popupSettings => m_popupSettings;

        // Domestic Initialization
        private void Awake()
        {
            m_popupSettings = this.GetComponent<TutorialPopupSettings_PartSelect>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_popupSettings, this);
            #endregion
        }

        // Foreign Initialization
        private void Start()
        {
            // Required to disable all popups after the first frame.
            this.gameObject.SetActive(false);

            m_stateMan = BetterBuildSceneStateManager.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_stateMan, this);
            #endregion

            m_popupHandler = new BetterBuildSceneStateChangeHandler(m_stateMan,
                BeginPopupHandler, EndPopupHandler, m_popupSettings.
                onlyShowPersistentDuringState);
        }


        public void HidePopup()
        {
            this.gameObject.SetActive(false);
        }


        private void OnDestroy()
        {
            m_popupHandler.ToggleActive(false);
        }

        private void BeginPopupHandler()
        {
            this.gameObject.SetActive(true);
        }

        private void EndPopupHandler()
        {
            this.gameObject.SetActive(false);
        }
    }
}
