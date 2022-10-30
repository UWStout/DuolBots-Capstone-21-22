using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    public class TrajectoryDotAnimation : MonoBehaviour
    {
        // Spacing between the trajectory dots
        [SerializeField] [Min(0.0f)] private float m_spacing = 5.0f;
        // Amount of time the animation should take
        [SerializeField] [Min(0.001f)] private float m_animTime = 1.0f;
        private float m_inverseAnimTime = float.PositiveInfinity;


        // Domestic Initialization
        private void Awake()
        {
            m_inverseAnimTime = 1.0f / m_animTime;
        }
        // Called once every frame
        private void Update()
        {
            // Scale the time so that the animation takes the specified animTime.
            float temp_scaledTime = Time.time * m_inverseAnimTime;
            // Value between 0 and 1
            float t = temp_scaledTime % 1.0f;
            UpdateAnimPosition(t);
        }


        private void UpdateAnimPosition(float t)
        {
            Vector3 temp_startPos = transform.localPosition;
            temp_startPos.z = 0.0f;
            Vector3 temp_targetPos = temp_startPos + new Vector3(0, 0, m_spacing);

            transform.localPosition = Vector3.Lerp(temp_startPos,
                temp_targetPos, t);
        }
    }
}
