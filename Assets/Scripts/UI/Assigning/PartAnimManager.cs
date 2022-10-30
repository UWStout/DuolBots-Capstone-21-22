using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace DuolBots
{
    public class PartAnimManager : MonoBehaviour
    {

        private GameObject part;
        private int m_slotIndex = -1;
        private int m_partIndex = -1;
        private CustomInputData m_customInput;
        private PartsOnBot m_partsOnBot;
        private GameObject currentPart;

        // this is just getting the part slot index, not the actual action list index
        public void UpdateAnimationManager(int _slotindex, int _partindex)
        {
            if (m_slotIndex != -1 && m_partIndex != -1)
            {
                m_customInput = new CustomInputData(0f, false);
                MovingParts();
            }

            m_slotIndex = _slotindex;
            m_partIndex = _partindex;
            m_customInput = new CustomInputData(0.5f, true);
            m_partsOnBot = new PartsOnBot();
        }

        private void Update()
        {
            if(currentPart == null) { return; }
            MovingParts();
        }
        private void MovingParts()
        {
            currentPart = m_partsOnBot.WheelsAndSlots()[m_slotIndex];

            if (currentPart != null)
            {
                /*
                var temp = currentPart.GetComponentInChildren<IPartInput>();
                if (temp != null)
                {
                    temp.DoPartAction((byte)m_partIndex, m_customInput);
                    MoveSpecs();
                }
                */
                
                if (currentPart.GetComponent<Input_Turret2Axis>() != null)
                {
                    currentPart.GetComponent<Input_Turret2Axis>().DoPartAction((byte)m_partIndex, m_customInput);
                    MoveSpecs();
                }
                else if (currentPart.GetComponentInChildren<Input_Movement>() != null)
                {
                    currentPart.GetComponentInChildren<Input_Movement>().DoPartAction((byte)m_partIndex, m_customInput);
                    MoveWheels();
                }
                else
                {
                    Debug.Log("There are no controllers for this part");
                }

            }
        }

        private void MoveWheels()
        {
            var currentPart = m_partsOnBot.WheelsAndSlots()[m_slotIndex];

            if(currentPart != null)
            {
                m_customInput.SetData(10f);
            }
        }

        private void MoveSpecs()
        {
            var currentPart = m_partsOnBot.WheelsAndSlots()[m_slotIndex];

            if(currentPart != null)
            {
                var specs = currentPart.GetComponent<Specifications_Turret2Axis>();
                var MaxRaise = AngleHelpers.ClampAnglesAlongRotationAxis(specs.raiseTrans.localEulerAngles, eRotationAxis.z, specs.minRaiseAngle, specs.maxRaiseAngle);
                if (specs.maxRaiseAngle == MaxRaise.z)
                {
                    //m_customInput = new CustomInputData(1f, true);
                    m_customInput.SetData(0.5f);
                    //Debug.Log("Moving in positive direction");
                }
                else if(specs.minRaiseAngle == MaxRaise.z)
                {
                    //m_customInput = new CustomInputData(-1f, true);
                    m_customInput.SetData(-0.5f);
                    //Debug.Log("Moving in negative direction");
                }

                //Debug.Log("CustomInputData: "+ m_customInput.Get());
                //Debug.Log("Current Raise Angle is: " + MaxRaise.z);
            }
        }
    }
}
