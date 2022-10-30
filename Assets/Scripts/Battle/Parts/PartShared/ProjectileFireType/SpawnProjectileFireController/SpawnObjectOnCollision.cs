using System;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Helper script for spawning objects on projectile collision
    /// </summary>
    public class SpawnObjectOnCollision : MonoBehaviour, IObjectSpawner
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        [SerializeField] [Required] private GameObject m_objectToSpawn = null;

        public event Action<GameObject> onObjectSpawned;

        private void Awake()
        {
            Debug.Assert(m_objectToSpawn, $"{nameof(m_objectToSpawn)} was not specificed for {name}'s {GetType().Name}");
        }

        private void OnCollisionEnter(Collision collision)
        {
            GameObject temp_spawnedObj = Instantiate(m_objectToSpawn, transform.position, Quaternion.identity);
            onObjectSpawned?.Invoke(temp_spawnedObj);
            CustomDebug.Log($"On Collision Enter", IS_DEBUGGING);
        }
    }
}
