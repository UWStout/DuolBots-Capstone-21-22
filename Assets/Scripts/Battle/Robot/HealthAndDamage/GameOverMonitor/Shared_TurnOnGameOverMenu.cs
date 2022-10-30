using UnityEngine;

using NaughtyAttributes;
using TMPro;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    [RequireComponent(typeof(DuolPlayerMenuSetup))]
    public class Shared_TurnOnGameOverMenu : MonoBehaviour
    {
        [SerializeField] [Required] private GameObject m_gameOverUI = null;
        [SerializeField] [Required] private TextMeshProUGUI m_decidingText = null;

        private DuolPlayerMenuSetup m_menuSetup = null;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            m_menuSetup = GetComponent<DuolPlayerMenuSetup>();

            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_gameOverUI,
                nameof(m_gameOverUI), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_decidingText,
                nameof(m_decidingText), this);
            CustomDebug.AssertComponentIsNotNull(m_menuSetup, this);
            #endregion Asserts
        }
        // Called 1st
        // Foreign Initialization
        private void Start()
        {
            m_gameOverUI.SetActive(false);
        }


        public void TurnOnMenu(string gameOverText)
        {
            m_decidingText.text = gameOverText;
            m_gameOverUI.SetActive(true);
            m_menuSetup.SetupMenu();
        }
        public void TurnOffMenu()
        {
            m_decidingText.text = "";
            m_gameOverUI.SetActive(false);
        }
    }
}
