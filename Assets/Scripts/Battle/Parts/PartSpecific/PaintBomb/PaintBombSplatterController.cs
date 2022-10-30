using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using NaughtyAttributes;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Controller that spawns a paint splatter object on as a child of a GameObject (which should be Canvas of a bot).
    /// </summary>
    public class PaintBombSplatterController : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;
        private const int MAX_RANDOM_ATTEMPTS = 50;
        private const int MIN_SPLATTERS = 1;
        private const int MAX_SPLATTERS = 100;

        [SerializeField, MinMaxSlider(0.0f, 1920.0f)]
        private Vector2 m_windowWidth = new Vector2(0.0f, 1920.0f);
        [SerializeField, MinMaxSlider(0.0f, 1080.0f)]
        private Vector2 m_windowHeight = new Vector2(0.0f, 1080.0f);
        [SerializeField, Required] private GameObject m_splatter = null;
        [SerializeField, Required] private Transform m_splatterParent = null;
        [SerializeField, MinMaxSlider(MIN_SPLATTERS, MAX_SPLATTERS)]
        private Vector2 m_numSplatters = new Vector2(7, 15);
        [SerializeField] private bool m_isRandomSplatterNum = false;
        [SerializeField] [Min(0.0f)] private float m_allowedSplatterDist = 100.0f;

        private List<Vector3> m_splatterPositions = new List<Vector3>();


        /// <summary>
        /// Spawns either a random number of splatter objects or the serialized value.
        /// </summary>
        public void ApplySplatters()
        {
            int temp_randNum = m_isRandomSplatterNum ? Random.Range(MIN_SPLATTERS, MAX_SPLATTERS) : (int)Random.Range(m_numSplatters.x, m_numSplatters.y);
            CustomDebug.Log($"{name} is applying {temp_randNum} splatters", IS_DEBUGGING);
            for (int i=0; i<temp_randNum; ++i)
            {
                SpawnSplatterObject();
            }
        }


        private Vector2 GenerateValidRandomPosition()
        {
            Vector2 temp_splatterPos = GenerateUnvalidatedRandomPosition();
            int temp_curRandAttempts = 0;
            // Continue generating until the position is valid,
            // or the max random attempts has been reached.
            while (!IsValidPosition(temp_splatterPos) &&
                temp_curRandAttempts++ < MAX_RANDOM_ATTEMPTS)
            {
                temp_splatterPos = GenerateUnvalidatedRandomPosition();
            }
            return temp_splatterPos;
        }
        private Vector2 GenerateUnvalidatedRandomPosition()
        {
            return new Vector2(Random.Range(m_windowWidth.x, m_windowWidth.y),
                Random.Range(m_windowHeight.x, m_windowHeight.y));
        }
        private bool IsValidPosition(Vector3 testSlatterPos)
        {
            float temp_allowedDistSqrd = m_allowedSplatterDist * m_allowedSplatterDist;
            foreach (Vector3 temp_existingPos in m_splatterPositions)
            {
                Vector3 temp_diff = temp_existingPos - testSlatterPos;
                if (temp_diff.sqrMagnitude > temp_allowedDistSqrd)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Spawns a single splatter GameObject with randomized position as a child of the serialized object with a Canvas.
        /// </summary>
        private void SpawnSplatterObject()
        {
            // Select random position
            Vector2 temp_splatterPos = GenerateValidRandomPosition();
            #region Logs
            CustomDebug.Log($"{name} spawned splatter object at {temp_splatterPos}", IS_DEBUGGING);
            #endregion Logs
            m_splatterPositions.Add(temp_splatterPos);
            // Instantiate Splatter prefab
            GameObject temp_splatterObject = Instantiate(m_splatter,
                temp_splatterPos, Quaternion.identity, m_splatterParent);
        }
    }
}
