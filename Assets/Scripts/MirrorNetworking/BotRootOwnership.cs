using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Expected to be attached to the networked BotRoot prefab.
    /// Can be queried to see if this bot belongs to whoever is looking at it.
    /// </summary>
    public class BotRootOwnership : NetworkBehaviour
    {
        // If the local client has authority over this robot.
        public bool isMyBot => m_isMyBot;
        private bool m_isMyBot = false;


        public override void OnStartServer()
        {
            base.OnStartServer();

            if (hasAuthority)
            {
                m_isMyBot = true;
            }
        }
        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            m_isMyBot = true;
        }
    }
}
