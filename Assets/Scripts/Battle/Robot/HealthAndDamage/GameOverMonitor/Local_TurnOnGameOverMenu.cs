using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    [RequireComponent(typeof(Shared_TurnOnGameOverMenu))]
    public class Local_TurnOnGameOverMenu : MonoBehaviour
    {
        private GameOverMonitor m_goMon = null;
        private Shared_TurnOnGameOverMenu m_sharedController = null;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            m_sharedController = GetComponent<Shared_TurnOnGameOverMenu>();
            CustomDebug.AssertComponentIsNotNull(m_sharedController, this);
        }
        private void Start()
        {
            m_goMon = GameOverMonitor.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_goMon, this);
            #endregion Asserts
            m_goMon.onGameOver.ToggleSubscription(OnGameOver, true);
        }
        private void OnDestroy()
        {
            if (m_goMon != null)
            {
                m_goMon.onGameOver.ToggleSubscription(OnGameOver, false);
            }
        }


        private void OnGameOver(GameOverData gameOverData)
        {
            m_sharedController.TurnOnMenu("Local Game Over!");
        }
    }
}
