// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Holds data for the currently active lobby.
    /// </summary>
    public static class CurrentLobbyData
    {
        /// <summary>
        /// Lobby data for the current lobby. Default is Private.
        /// </summary>
        public static eLobbyType currentLobbyType { get; set; } = eLobbyType.Private;
    }
}
