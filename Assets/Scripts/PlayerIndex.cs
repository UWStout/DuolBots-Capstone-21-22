using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Holds if the player is player one or player two on their team.
    /// </summary>
    public class PlayerIndex : MonoBehaviour
    {
        // TODO: Figure this out some other way and stop serializing
        // Maybe it gets set from the 'join' menu
        [SerializeField] private byte m_playerIndexValue = 0;
        public byte playerIndex
        {
            get => m_playerIndexValue;
            set => m_playerIndexValue = value;
        }
    }
}
