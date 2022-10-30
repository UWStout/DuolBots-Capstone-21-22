using Mirror;

namespace DuolBots.Mirror
{
    public class NetworkTeamIndex : NetworkBehaviour, ITeamIndex
    {
        [SyncVar] private byte m_teamIndex = 0;

        public byte teamIndex
        {
            get => m_teamIndex;
            set => m_teamIndex = value;
        }
    }
}
