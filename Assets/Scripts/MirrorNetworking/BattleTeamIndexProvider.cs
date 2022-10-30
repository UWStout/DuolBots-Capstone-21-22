using Mirror;
using NaughtyAttributes;
// Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    public class BattleTeamIndexProvider : NetworkBehaviour
    {
        public static BattleTeamIndexProvider instance { get; private set; }

        [SyncVar] [ReadOnly] private byte m_nextAvailableIndex = 0;


        // This breaks the 1st Commandment of Mirror Networking,
        // but I will allow it for singleton initialization.
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                CustomDebug.LogWarning($"Multiple instances of " +
                    $"{GetType().Name} singleton exist");
            }
        }

        public byte RequestNewTeamIndex()
        {
            // Return the current index and then increment.
            return m_nextAvailableIndex++;
        }
    }
}
