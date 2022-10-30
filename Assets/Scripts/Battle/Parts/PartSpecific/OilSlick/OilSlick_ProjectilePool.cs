using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using NaughtyAttributes;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Script that controls the OilSlick pool spawned by OilSlick projectiles after impacting the ground.
    /// </summary>
    public class OilSlick_ProjectilePool : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;
        private const string PART = "Part";
        // Ground check constants
        private const float GC_LENGTH = 0.1f;
        private const float GC_OFFSET = .01f;
        
        [SerializeField] private List<Transform> m_groundCheckers = null;

        private GameObject m_oilSlickPool = null;
        private Collider m_oilSlickCollider = null;
        private float m_duration = 10f;

        private void Awake()
        {
            Assert.IsNotNull($"{this.name} could not find an attached {typeof(GameObject)} OilSlick pool but requires one.");
            Assert.IsNotNull($"{this.name} could not find an attached {typeof(Collider)} but requires one.");
        }

        private void Update()
        {
            m_duration -= Time.deltaTime;
            if (m_duration <= 0.0f) { Destroy(gameObject); }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PART))
            {
                SharedController_Movement temp_movement = GetComponent<SharedController_Movement>();
                if (temp_movement != null)
                {
                    temp_movement.ApplyOil();
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(PART))
            {
                SharedController_Movement temp_movement = GetComponent<SharedController_Movement>();
                if (temp_movement != null)
                {
                    temp_movement.RemoveOil();
                }
            }
        }

        private void MeshCuttingResize()
        {
            // Do a ground check from each Transform to check which parts of the Oil pool are not in contact with the ground
            foreach(Transform trans in m_groundCheckers)
            {
                Physics.Raycast(trans.localPosition, Vector3.down, out RaycastHit temp_hit, GC_LENGTH);

            }
        }
    }
}
