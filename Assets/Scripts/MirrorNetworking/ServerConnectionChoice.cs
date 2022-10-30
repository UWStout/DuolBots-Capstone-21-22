// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    public enum ConnectionChoice { Join, Host }

    /// <summary>
    /// Static class to hold which connetion option the player selected.
    /// </summary>
    public static class ServerConnectionChoice
    {
        private static ConnectionChoice s_connectionChosen = ConnectionChoice.Join;

        public static ConnectionChoice GetConnectionChoice() => s_connectionChosen;
        public static void SetConnectionChoice(ConnectionChoice connectionChosen) => s_connectionChosen = connectionChosen; 
    }
}
