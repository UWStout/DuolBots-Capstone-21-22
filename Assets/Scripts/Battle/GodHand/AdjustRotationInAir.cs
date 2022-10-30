using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DuolBots
{
    public class AdjustRotationInAir : MonoBehaviour
    {
        private Quaternion m_targetRot;
        private Quaternion m_currentRotation;

        private Vector3 m_targetPos;
        private Vector3 m_currentPos;

        private float timeCount = 0f;
        private float speed = .2f;

        private bool m_active = false;
        // Update is called once per frame
        void Update()
        {
            if (m_active)
            {
                transform.rotation = Quaternion.Lerp(m_currentRotation, m_targetRot, timeCount * speed);
                float y = transform.position.y;
                transform.position = Vector3.Lerp((new Vector3(m_currentPos.x, y, m_currentPos.z)), (new Vector3(m_targetPos.x, y, m_targetPos.z)), timeCount * speed);
                timeCount += Time.deltaTime;
                if (transform.position.y < 20)
                {
                    Rigidbody rb = gameObject.GetComponent<Rigidbody>();
                    rb.freezeRotation = false;
                    //rb.velocity = Vector3.zero;
                    //BattleCameraSystem.instance.ChangeToActiveBattleCameras();
                    m_active = false;
                }
            }
        }

        public void UpdateInfo(Transform Target)
        {
            SetCurrentRotation();
            SetCurrentPosition(Target);
            Activate();
        }

        public void SetCurrentRotation()
        {
            m_currentRotation = transform.rotation;
            m_targetRot = Quaternion.Euler(0, transform.rotation.y, 0);
        }

        public void SetCurrentPosition(Transform Target)
        {
            m_currentPos = transform.position;
            m_targetPos = Target.position;
        }

        public void Activate()
        {
            m_active = true;
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            //BattleCameraSystem.instance.TurnOffAllCameras();
        }
    }
}
