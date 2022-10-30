using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using NaughtyAttributes;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Script that controls the projectile spawned by the Oilslick.
    /// </summary>
    public class OilSlick_Projectile : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;
        [SerializeField] private GameObject m_projectile = null;

        private void Awake()
        {
            Assert.IsNotNull($"{this.name} could not find a {typeof(GameObject)} projectile prefab but requires one.");
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Create an Oilslick pool on contact with the ground.
            if (collision.collider.CompareTag("Ground"))
            {
                // Instantiate the Oilslick pool GameObject slightly below the contact point with the rotation of the surface it hit.
                GameObject temp_projectile = Instantiate(m_projectile, new Vector3(transform.position.x,
                    transform.position.y - 0.5f*(GetComponentInChildren<Collider>().bounds.size.y),
                    transform.position.z), collision.transform.rotation);
                CustomDebug.Log($"{collision.transform.name}, Rotation: {collision.transform.localRotation.eulerAngles}", IS_DEBUGGING);
                temp_projectile.transform.parent = null;
                Destroy(gameObject);
            }
        }
    }
}
