using UnityEngine;

namespace DuolBots
{
    public class TeamSpawnPoints : MonoBehaviour
    {
        [SerializeField] private Transform[] m_spawnLocations = null;


        public Transform GetSpawnLocation(byte teamIndex)
        {
            if (teamIndex >= m_spawnLocations.Length)
            {
                Debug.LogError($"Specified teamIndex of {teamIndex} is out " +
                    $"of bounds.");
                return null;
            }

            return m_spawnLocations[teamIndex];
        }
    }
}
