using UnityEngine;
using UnityEngine.Assertions;

using Cinemachine;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    [RequireComponent(typeof(CinemachineFreeLook))]
    public class CinemachineFreeLookCameraController : MonoBehaviour
    {
        private CinemachineFreeLook m_freeLookCamera = null;


        // Domestic Initialization
        private void Awake()
        {
            m_freeLookCamera = GetComponent<CinemachineFreeLook>();

            CustomDebug.AssertComponentIsNotNull(m_freeLookCamera, this);
        }


        public void MoveCamera(Vector2 movement)
        {
            // Can't move the camera if its not on yet
            if (!gameObject.activeInHierarchy) { return; }

            m_freeLookCamera.m_XAxis.m_InputAxisValue = movement.x;
            m_freeLookCamera.m_YAxis.m_InputAxisValue = movement.y;
        }
    }
}
