using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Helpers;
// Original Authors - Ben Lussman
// Modified by Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    public class ProjectileTrajectory : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField]
        private bool traj = true;
        //[SerializeField]
        //private bool m_HasGravity = true;
        [SerializeField]
        private Vector3 m_Gravity = Physics.gravity;
        [SerializeField]
        private float m_InitVel = -1;
        [SerializeField]
        private GameObject Dot;
        [SerializeField]
        private GameObject End;
        [SerializeField]
        int m_numPoints;
        [SerializeField]
        Transform m_StartingPoint;
        [SerializeField]
        int m_distBetweenPoints=2;
        private DottedLine m_drawDottedLine;
        private float m_scaleDist = .1f;

        private void Start()
        {
            int play = GetComponent<PlayerIndex>().playerIndex;

            m_drawDottedLine = new DottedLine(End, Dot, 13+play);
        }
        private void FixedUpdate()
        {
            if (!traj) { return; }

            UpdateTrajectory();
        }


        public void UpdateInfo(IPartTrajectorySpecs Specs,
            IPartShowsTrajectory Shows)
        {
            #region Asserts
            CustomDebug.AssertIComponentIsNotNull(Specs, this);
            CustomDebug.AssertIComponentIsNotNull(Shows, this);
            #endregion Asserts

            m_StartingPoint = Specs.projectileSpawnPos;
            #region Asserts
            CustomDebug.AssertIsTrueForComponent(m_StartingPoint != null,
                $"{Specs.name} to have a starting point, but none was found", this);
            #endregion Asserts
            m_InitVel = Shows.initialSpeed;
            m_Gravity = Shows.shouldUseGravity ? Physics.gravity : Vector3.zero;
            traj = true;
        }
        public void LineOff()
        {
            traj = false;
            m_drawDottedLine.HideLine();
        }

        // This updates the points and passes them in order to draw lines.
        private void UpdateTrajectory()
        {
            // Make a list of points and add the spawn point to the line.
            List<Vector3> temp_points = new List<Vector3>();
            temp_points.Add(PointOnLine(0));

            // For the maximum number of points.
            for (int i = 1; i < m_numPoints; i++)
            {
                // Add a point to the line at a distanced scaled by a factor.
                temp_points.Add(PointOnLine(i * m_scaleDist));

                // Get the line between this point and the previous one
                Vector3 temp_line = temp_points[i] - temp_points[i - 1];

                // Do a raycast fo the ground.
                RaycastHit temp_hit;
                if (Physics.Raycast(temp_points[i - 1], temp_line, out temp_hit, temp_line.magnitude))
                {
                    // Check if the collision was with the ground
                    if (temp_hit.collider.gameObject.tag == "Ground")
                    {
                        //set the current point to the hit point then leave the loop
                        
                        temp_points[i] = temp_hit.point; // This must be done to display a different object at the end of the line
                        break;
                    }
                }
            }

            // Pass the list of points and the distance between them to another script to be drawn
            m_drawDottedLine.CreateLine(temp_points.ToArray(), m_distBetweenPoints, 0);
            m_drawDottedLine.Point();
        }

        private Vector3 PointOnLine(float time)
        {
            if (m_StartingPoint == null)
            {
                #region Logs
                CustomDebug.LogForComponent($"{nameof(PointOnLine)} was called " +
                    $"when {nameof(m_StartingPoint)} was null", this, IS_DEBUGGING);
                #endregion Logs
                return Vector3.zero;
            }

            return (m_StartingPoint.position + m_InitVel * m_StartingPoint.forward
                * time + .5f * m_Gravity * time * time);
        }
    }
}
