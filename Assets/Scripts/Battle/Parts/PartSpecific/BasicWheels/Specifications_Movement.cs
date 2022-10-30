using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Zach Gross and Wyatt Senalik

namespace DuolBots
{
    public class Specifications_Movement : MonoBehaviour
    {
        // The maximum power allowed to be supplied to this movement component
        public float maxWheelPower => m_maxWheelPower;
        [SerializeField] private float m_maxWheelPower = 60;
        // A factor used to determine how much a bot's total weight affects acceleration when using this movement component
        public float weightSlowdownFactor => m_weightSlowdownFactor;
        [SerializeField] [Min(0.1f)] private float m_weightSlowdownFactor = 2.5f;

        // The following transforms are used in determining rotation point
        // Transform marking the center of the left side of this movement component
        public Vector3 leftSideCenter => Vector3.Lerp(frontLeft.position, backLeft.position, 0.5f);
        // Transform marking the center of the right side of this movement component
        public Vector3 rightSideCenter => Vector3.Lerp(frontRight.position, backRight.position, 0.5f);

        // The following transforms are for each part of the entire movement component
        [SerializeField] private Transform[] m_transforms;

        public Transform[] wheelTransforms => m_transforms;
        public Transform frontLeft => m_transforms[(int)WheelPos.FL];
        public Transform frontRight => m_transforms[(int)WheelPos.FR];
        public Transform backLeft => m_transforms[(int)WheelPos.BL];
        public Transform backRight => m_transforms[(int)WheelPos.BR];
    }
}
