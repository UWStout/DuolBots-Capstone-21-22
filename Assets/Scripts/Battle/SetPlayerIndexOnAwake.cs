using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// TODO - Kill this.
    /// This is a temporary way to get the 2 local players to have different values.
    /// </summary>
    [RequireComponent(typeof(PlayerIndex))]
    public class SetPlayerIndexOnAwake : MonoBehaviour
    {
        // What value to give the next player who joins
        public static byte s_nextPlayerIndex = 0;


        // Domestic Initialization
        private void Awake()
        {
            PlayerIndex temp_playerIndex = GetComponent<PlayerIndex>();
            Assert.IsNotNull(temp_playerIndex, $"ERROR. {name} ({GetType().Name})" +
                $" does not have a {typeof(PlayerIndex).Name} attached to it");

            temp_playerIndex.playerIndex = s_nextPlayerIndex;
            Debug.Log($"NextPlayerIndex = {s_nextPlayerIndex}");
            ++s_nextPlayerIndex;
        }
    }
}
