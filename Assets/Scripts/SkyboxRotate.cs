using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - ?
// Edited by Wyatt Senalik (Tried to make it so it
// wouldn't actually edit the material and want us to push it).

namespace DuolBots
{
    public class SkyboxRotate : MonoBehaviour
    {
        [SerializeField] private float m_rotationSpeed = 0.4f;

        private Material m_workingCopyMat = null;


        private void Awake()
        {
            m_workingCopyMat = new Material(RenderSettings.skybox);
            RenderSettings.skybox = m_workingCopyMat;
        }
        private void Update()
        {
            m_workingCopyMat.SetFloat("_Rotation", Time.time * m_rotationSpeed);
        }
    }
}
