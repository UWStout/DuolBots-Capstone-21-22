using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace DuolBots
{
    [RequireComponent(typeof(Input_DollyTargetCycler))]
    public class SetDollyTargetCyclerForInput : MonoBehaviour
    {
        private Input_DollyTargetCycler m_inputForCycler = null;
        private CameraHelpersSingleton m_cameraHelpersInstance = null;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            CustomDebug.AssertComponentIsNotNull(m_inputForCycler, this);
            m_inputForCycler = GetComponent<Input_DollyTargetCycler>();
            Assert.IsNotNull(m_inputForCycler, $"{name}'s " +
                $"{GetType().Name} requires " +
                $"{nameof(PlayerIndex)} to be attached.");
        }
        // Called 1st
        // Foreign Initialization
        private void Start()
        {
            m_cameraHelpersInstance = CameraHelpersSingleton.instance;
            // TODO Just do this from player join scene.
        }
    }
}
