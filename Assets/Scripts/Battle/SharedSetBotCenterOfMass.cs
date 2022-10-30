using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Shared functionality for both the local and network version of
    /// SetBotCenterOfMass.
    /// </summary>
    public class SharedSetBotCenterOfMass : MonoBehaviour
    {
        /// <summary>
        /// Calculates the center of mass of the robot such that the center of mass is
        /// equally between all the movement part colliders.
        ///
        /// Pre Conditions - There is at least one collider attached to at least one of the
        /// specified Transforms.
        /// Post Conditions - The center of mass of the bot's Rigidbody is updated.
        /// </summary>
        public void CalculateAndSetCenterOfMass(Rigidbody rb,
            MovementPartColliders movementColliders)
        {
            Vector3 temp_worldCenterOfMass = Vector3.zero;
            IReadOnlyList<Transform> temp_moveColliderTransList =
                movementColliders.GetColliderTransforms();
            foreach (Transform temp_singleColliderTrans in temp_moveColliderTransList)
            {
                temp_worldCenterOfMass += temp_singleColliderTrans.position;
            }
            Assert.AreNotEqual(0, temp_moveColliderTransList.Count,
                $"No colliders were found by {GetType().Name}");
            temp_worldCenterOfMass /= temp_moveColliderTransList.Count;

            rb.centerOfMass = temp_worldCenterOfMass - rb.transform.position;
        }
    }
}
