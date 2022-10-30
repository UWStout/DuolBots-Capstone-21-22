using UnityEngine;
using UnityEngine.Assertions;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots
{

    /// <summary>
    /// Holds serialized fields for a Turret2Axis part.
    /// </summary>
    public class Specifications_Turret2Axis : MonoBehaviour
    {
        // Transform to rotate for rotating the turret
        public Transform rotateTrans => m_rotateTrans;
        [SerializeField] [Required] private Transform m_rotateTrans = null;
        // Transform to rotate for raising the turret
        public Transform raiseTrans => m_raiseTrans;
        [SerializeField] [Required]  private Transform m_raiseTrans = null;


        // Rotation and raising specifications
        public bool invertRotateInput => m_invertRotateInput;
        [SerializeField] private bool m_invertRotateInput = true;
        public float rotateSpeed => m_rotateSpeed;
        [SerializeField] [Min(0.0f)] private float m_rotateSpeed = 140.0f;
        public float minRotateAngle => m_minRotateAngle;
        [SerializeField] [Range(-180.0f, 180.0f)] private float m_minRotateAngle = -180.0f;
        public float maxRotateAngle => m_maxRotateAngle;
        [SerializeField] [Range(-180.0f, 180.0f)] private float m_maxRotateAngle = 180.0f;
        public eRotationAxis axisToRotate => m_axisToRotate;
        [SerializeField] private eRotationAxis m_axisToRotate = eRotationAxis.y;

        public bool invertRaiseInput => m_invertRaiseInput;
        [SerializeField] private bool m_invertRaiseInput = true;
        public float raiseSpeed => m_raiseSpeed;
        [SerializeField] [Min(0.0f)] private float m_raiseSpeed = 80.0f;
        public float minRaiseAngle => m_minRaiseAngle;
        [SerializeField] [Range(-180.0f, 180.0f)] private float m_minRaiseAngle = 0.0f;
        public float maxRaiseAngle => m_maxRaiseAngle;
        [SerializeField] [Range(-180.0f, 180.0f)] private float m_maxRaiseAngle = 180.0f;
        public eRotationAxis axisToRaise => m_axisToRaise;
        [SerializeField] private eRotationAxis m_axisToRaise = eRotationAxis.z;


        public WwiseEventName beginStateWwiseEventName =>
            m_beginStateWwiseEventName;
        [SerializeField] [Required]
        private WwiseEventName m_beginStateWwiseEventName = null;
        public WwiseEventName stopStateWwiseEventName => m_stopStateWwiseEventName;
        [SerializeField] [Required]
        private WwiseEventName m_stopStateWwiseEventName = null;
        public AkGameObj soundObj => m_soundObject;
        [SerializeField] [Required] private AkGameObj m_soundObject = null;



        // Domestic Initialization
        private void Awake()
        {
            // Not null assertions
            Assert.IsNotNull(m_rotateTrans, CreateIsNotNullMessage("rotateTrans"));
            Assert.IsNotNull(m_raiseTrans, CreateIsNotNullMessage("raiseTrans"));
        }

        /// <summary>
        /// Creates a string to print if a serialized field is null.
        /// </summary>
        private string CreateIsNotNullMessage(string nameOfNullVariable)
        {
            return $"{name}'s {typeof(Specifications_Turret2Axis)} did not have {nameOfNullVariable} specified";
        }
    }
}
