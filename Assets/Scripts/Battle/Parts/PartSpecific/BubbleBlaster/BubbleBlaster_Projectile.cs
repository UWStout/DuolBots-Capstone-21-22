using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Author - Aaron Duffey
// Modified by Wyatt Senalik (Mar. 31 2022)

namespace DuolBots
{
    /// <summary>
    /// Script attached to the BubbleBlaster's projectiles
    /// that handles movement and collision
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class BubbleBlaster_Projectile : MonoBehaviour
    {
        private bool IS_DEBUGGING = false;


        [SerializeField] [Min(0)] private float m_maxRandomForce = 100;
        [SerializeField]
        [MinMaxSlider(0.0f, 1.0f)]
        private Vector2 m_randomMoveInfluenceRange = new Vector2(0.0f, 1.0f);

        private Rigidbody m_rigidBody = null;


        private void Awake()
        {
            m_rigidBody = GetComponent<Rigidbody>();
            Assert.IsNotNull(m_rigidBody, $"{this.name} could not find an " +
                $"attached {typeof(Rigidbody)} but requires one.");
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            Move();
        }

        /// <summary>
        /// Randomized movement for projectile.
        /// </summary>
        private void Move()
        {
            Vector3 temp_randomDirection = UnityEngine.Random.insideUnitCircle;
            temp_randomDirection = temp_randomDirection.normalized;

            float temp_sample = UnityEngine.Random.Range(
                m_randomMoveInfluenceRange.x, m_randomMoveInfluenceRange.y);
            float temp_forceMag = temp_sample * m_maxRandomForce * Time.deltaTime;

            CustomDebug.Log($"{nameof(temp_sample)}={temp_sample}; " +
                $"{nameof(m_maxRandomForce)}={m_maxRandomForce}; " +
                $"{nameof(Time.deltaTime)}={Time.deltaTime}; " +
                $"{nameof(temp_forceMag)}={temp_forceMag}; " +
                $"{nameof(temp_randomDirection)}={temp_randomDirection}; " +
                temp_forceMag * temp_randomDirection, IS_DEBUGGING);
            m_rigidBody.AddRelativeForce(temp_forceMag * temp_randomDirection);
        }
    }
}
