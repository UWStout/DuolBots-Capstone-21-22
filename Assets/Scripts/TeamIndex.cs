using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Holds a value representing which team.
    /// </summary>
    public class TeamIndex : MonoBehaviour, ITeamIndex
    {
        // TODO Get this from somewhere else
        // Index for which team this is
        [SerializeField] private byte m_teamIndexValue = 0;

        /// <summary>
        /// Index for which team this is.
        /// </summary>
        public byte teamIndex
        {
            get => m_teamIndexValue;
            set => m_teamIndexValue = value;
        }
    }
}
