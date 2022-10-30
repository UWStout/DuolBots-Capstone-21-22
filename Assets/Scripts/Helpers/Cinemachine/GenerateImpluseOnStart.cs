using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Cinemachine.Extensions
{
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public class GenerateImpluseOnStart : MonoBehaviour
    {
        private CinemachineImpulseSource m_impulseSource = null;


        // Domestic Initialization
        private void Awake()
        {
            m_impulseSource = GetComponent<CinemachineImpulseSource>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_impulseSource, this);
            #endregion Asserts
        }
        // Foreigin Initialization
        private void Start()
        {
            m_impulseSource.GenerateImpulse();
        }
    }
}
