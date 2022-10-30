using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Author - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Sets the rotation of an object to the specified Rigidbody's velocity
    /// </summary>
    public class SetRotationToVelocity : MonoBehaviour
    {
        [SerializeField] private Rigidbody m_rgbd = null;

        private void Awake()
        {
            CustomDebug.AssertComponentIsNotNull(m_rgbd, this);
        }

        // Update is called once per frame
        void Update()
        {
            transform.forward = m_rgbd.velocity.normalized;
        }
    }
}
