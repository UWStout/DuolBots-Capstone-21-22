using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    public class BattleUIManager : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField] private GameObject[] m_inGameElements = new GameObject[2];
        [SerializeField] private GameObject m_waitingElement = null;


        // Domestic Initialization
        private void Awake()
        {
            ChangeToWaitingUI();
        }


        public void ChangeToInGameUI()
        {
            #region Logs
            CustomDebug.Log(nameof(ChangeToInGameUI), IS_DEBUGGING);
            #endregion Logs
            ToggleInGameElements(true);

            m_waitingElement.SetActive(false);
        }
        public void ChangeToWaitingUI()
        {
            m_waitingElement.SetActive(true);

            ToggleInGameElements(false);
        }
        public void ChangeToEDCinematicUI()
        {
            m_waitingElement.SetActive(false);
            ToggleInGameElements(false);
        }


        private void ToggleInGameElements(bool cond)
        {
            foreach (GameObject temp_gameElement in m_inGameElements)
            {
                temp_gameElement.SetActive(cond);
            }
        }
    }
}
