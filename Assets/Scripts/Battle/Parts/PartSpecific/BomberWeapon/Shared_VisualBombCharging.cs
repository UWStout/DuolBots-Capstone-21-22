using System;
using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Displays an partially transparent preview of the
    /// BomberWeapon's projectile that will be fired.
    /// </summary>
    [RequireComponent(typeof(Specifications_ChargeSpawnProjectileFireController))]
    [RequireComponent(typeof(Shared_ChargeSpawnProjectileFireController))]
    public class Shared_VisualBombCharging : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField] [Required]
        private GameObject m_projectilePreviewInstance = null;
        [SerializeField] [Min(0.001f)]
        private float m_sizeScalingMultiplier = 1.0f;

        private Specifications_ChargeSpawnProjectileFireController
            m_specifications = null;
        public Shared_ChargeSpawnProjectileFireController sharedController
            => m_sharedController;
        private Shared_ChargeSpawnProjectileFireController
            m_sharedController= null;

        public GameObject projectilePreviewInstance
            => m_projectilePreviewInstance;
        public Vector3 projectilePreviewStartingScale
            => m_projectilePreviewStartingScale;
        private Vector3 m_projectilePreviewStartingScale = Vector3.one;


        // Domestic Initialization
        private void Awake()
        {
            m_specifications = GetComponent<
                Specifications_ChargeSpawnProjectileFireController>();
            Assert.IsNotNull(m_specifications, $"{name}'s {GetType().Name} " +
                $"requires " +
                $"{typeof(Specifications_ChargeSpawnProjectileFireController)} " +
                $"but none was found");
            m_sharedController = GetComponent<
                Shared_ChargeSpawnProjectileFireController>();
            Assert.IsNotNull(m_sharedController, $"{name}'s " +
                $"{GetType().Name} requires " +
                $"{typeof(Shared_ChargeSpawnProjectileFireController)} but " +
                $"none was found");
        }
        private void Start()
        {
            m_projectilePreviewStartingScale =
                m_projectilePreviewInstance.transform.localScale;
        }


        public void UpdatePreviewObjectScale(float curCharge)
        {
            projectilePreviewInstance.transform.localScale =
                m_projectilePreviewStartingScale * curCharge
                    * m_sizeScalingMultiplier;
        }
        public void SetPreviewObjectActive(bool cond)
        {
            projectilePreviewInstance.SetActive(cond);
        }
    }
}
