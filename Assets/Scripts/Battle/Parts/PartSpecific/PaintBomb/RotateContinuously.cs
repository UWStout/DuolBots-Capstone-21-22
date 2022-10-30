using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Rotates given transform continuously (used on PaintBomb projectile)
    /// </summary>
    public class RotateContinuously : MonoBehaviour
    {
        [SerializeField] private Transform m_objectToRotate = null;
        [SerializeField] private Vector3 m_angleToRotateAt = Vector3.zero;

        private bool m_spinIsRunning = false;

        private void Awake()
        {
            Assert.IsNotNull(m_objectToRotate, $"{this.name} does not have a seralized {nameof(m_objectToRotate)} but requires one.");
        }

        // Update is called once per frame
        void Update()
        {
            m_objectToRotate.Rotate(m_angleToRotateAt*Time.deltaTime);
        }


    }
}
