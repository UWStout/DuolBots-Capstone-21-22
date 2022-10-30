using UnityEngine;
using UnityEngine.Assertions;

using Mirror;
// Original Authors- Aaron Duffey, Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Networked implementation of spawning a
    /// projectile after fully reaching a charge time.
    /// </summary>
    [RequireComponent(typeof(Shared_ChargeSpawnProjectileFireController))]
    public class Network_ChargeSpawnProjectileFireController : MonoBehaviour
    {
        private Shared_ChargeSpawnProjectileFireController
            m_sharedFullChargeController = null;


        private void Awake()
        {
            m_sharedFullChargeController =
                GetComponent<Shared_ChargeSpawnProjectileFireController>();
            Assert.IsNotNull(m_sharedFullChargeController,
                $"{nameof(Shared_ChargeSpawnProjectileFireController)} " +
                $"was not found on {this.name} but is required.");
        }
        private void OnEnable()
        {
            m_sharedFullChargeController.onProjectileSpawned += SpawnProjectileAcrossNetwork;
        }
        private void OnDisable()
        {
            if (m_sharedFullChargeController == null) { return; }
            m_sharedFullChargeController.onProjectileSpawned -= SpawnProjectileAcrossNetwork;
        }


        [Server]
        private void SpawnProjectileAcrossNetwork(GameObject spawnedProjectile)
        {
            NetworkServer.Spawn(spawnedProjectile);
        }
    }
}
