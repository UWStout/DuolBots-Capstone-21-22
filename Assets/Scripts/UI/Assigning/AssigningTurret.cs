using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace DuolBots
{
    public class AssigningTurret : MonoBehaviour
    {

        public bool yAxis = false;
        public bool xAxis = false;
        public bool button = false;

        private IController_Turret2Axis m_turretController = null;
        private IWeaponFireController m_weaponFireController = null;
        private Input_Turret2Axis turret;

        // Current input values
        private float m_lastRotInpVal = 0.0f;
        private float m_curRotInpVal = 360f;
        private float m_lastRaiseInpVal = 180;
        private float m_curRaiseInpVal = 0f;

        private float speedAcc = 0.1f;
        public float Xspeed = 0.0f;
        public float Yspeed = 0.0f;

        // Domestic Initialization
        private void Awake()
        {
            m_turretController = GetComponent<IController_Turret2Axis>();
            m_weaponFireController = GetComponent<IWeaponFireController>();
        }

        private void Update()
        {
            
            if(button)
            {
                //StartCoroutine(FireTheTurret());
            }

            // Movees the Y Axis Rotation
            if(yAxis)
            {
                //StartCoroutine(RaiseTheTurret());
            }

            if(xAxis)
            {
                //RotateTurret();
            }
           


            // Moves the X Axis rotation
            /*if (xAxis)
            {
                Xspeed = 0.001f;
            }
            else
            {
                if(Xspeed > 0)
                {
                    Xspeed -= 0.0005f * Time.deltaTime;
                }
                Xspeed = 0;
            }
            m_turretController.ChangeRotationInput(Xspeed);
            */

        }

       /* IEnumerator Barrel()
        {
            Yspeed = 0.001f;
            yield return new WaitForSeconds(1f);
            Yspeed = -0.001f;
        }

        private void RotateTurret()
        {
            *//*
            if (speed <= .1)
            {
                speed += speedAcc * Time.deltaTime;
            }
            else
            {
                speed = 0.1f;
            }
            /*else
            {
                speed -= speedAcc * Time.deltaTime;
            }*//*
            Xspeed = speedAcc;
            if(Xspeed > 0.1f)
            {
                Xspeed = 0.1f;
            }
            m_turretController.ChangeRotationInput(Xspeed*Time.deltaTime);         
        }

        IEnumerator RotateTheTurretInput()
        {
            xAxis = false;
            RotateTurret();
            yield return new WaitForSeconds(2f);
            xAxis = true;
        }


        private void RaiseBarrel()
        {
            //m_turretController.ChangeRaiseInput(Random.Range(m_curRaiseInpVal, m_lastRaiseInpVal));
            //m_turretController.ChangeRaiseInput(m_curRaiseInpVal - m_lastRaiseInpVal);
            m_turretController.ChangeRaiseInput(speedAcc * Time.deltaTime);
            
        }
        IEnumerator RaiseTheTurret()
        {
            m_turretController.ChangeRaiseInput(speedAcc * Time.deltaTime);
            yield return new WaitForSeconds(1f);
            m_turretController.ChangeRaiseInput(-speedAcc * Time.deltaTime);
        }

        private void FireTurret()
        {
            m_weaponFireController.Fire(true);
        }

        IEnumerator FireTheTurret()
        {
            button = false;
            FireTurret();
            yield return new WaitForSeconds(2f);
            button = true;
        }*/
    }
}
