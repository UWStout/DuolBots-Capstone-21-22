using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Shows appropriate element depending on the lobby type.
    /// </summary>
    public class BattleWaitingScreenManager : MonoBehaviour
    {
        [SerializeField] private GameObject m_privateGameElement = null;
        [SerializeField] private GameObject m_queueElement = null;


        // Domestic Initializtion
        private void Awake()
        {
            // Change which text element is displayed depending on the lobby type
            switch (CurrentLobbyData.currentLobbyType)
            {
                case eLobbyType.Private:
                    m_privateGameElement.SetActive(true);
                    m_queueElement.SetActive(false);
                    break;
                case eLobbyType.Queue:
                    m_queueElement.SetActive(true);
                    m_privateGameElement.SetActive(false);
                    break;
                default:
                    CustomDebug.UnhandledEnum(CurrentLobbyData.currentLobbyType,
                        this);
                    break;
            }
        }
    }
}
