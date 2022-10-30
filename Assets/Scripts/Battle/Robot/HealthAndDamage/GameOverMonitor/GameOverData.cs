using System.Collections.Generic;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    public enum eGameOverCause { Default, Health, Time, Disconnect }

    public class GameOverData
    {
        private eGameOverCause m_cause = eGameOverCause.Default;
        private byte[] m_winningTeamIndices = new byte[0];

        public eGameOverCause cause => m_cause;
        public IReadOnlyList<byte> winningTeamIndices => m_winningTeamIndices;


        public GameOverData()
        {
            m_cause = eGameOverCause.Default;
            m_winningTeamIndices = new byte[0];
        }
        /// <summary>
        /// When one or more teams have won.
        /// </summary>
        /// <param name="gameOverCause">What caused the game to end.</param>
        /// <param name="indicesOfWinningTeam">Which teams have won.</param>
        public GameOverData(eGameOverCause gameOverCause,
            params byte[] indicesOfWinningTeam)
        {
            m_cause = gameOverCause;
            m_winningTeamIndices = indicesOfWinningTeam;
        }
    }
}
