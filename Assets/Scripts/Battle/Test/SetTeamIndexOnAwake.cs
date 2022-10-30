using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// TODO - Kill this.
    /// This is a temporary way to get the 4 local players to have different values.
    /// MUST be added before SetPlayerIndexOnAwake.
    /// </summary>
    [RequireComponent(typeof(PlayerIndex))]
    [RequireComponent(typeof(TeamIndex))]
    [RequireComponent(typeof(SetPlayerIndexOnAwake))]
    public class SetTeamIndexOnAwake : MonoBehaviour
    {
        // Domestic Initialization
        private void Awake()
        {
            ITeamIndex temp_teamIndex = GetComponent<ITeamIndex>();
            Assert.IsNotNull(temp_teamIndex, $"ERROR. {name} ({nameof(SetTeamIndexOnAwake)})" +
                $" does not have a {nameof(ITeamIndex)} attached to it");
            PlayerIndex temp_playerIndex = GetComponent<PlayerIndex>();

            temp_teamIndex.teamIndex = (byte)(temp_playerIndex.playerIndex / 2);
            temp_playerIndex.playerIndex = (byte)(temp_playerIndex.playerIndex % 2);
        }
    }
}
