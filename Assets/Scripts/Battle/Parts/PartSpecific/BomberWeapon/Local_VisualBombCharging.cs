using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    [RequireComponent(typeof(Shared_VisualBombCharging))]
    [RequireComponent(typeof(Shared_ChargeSpawnProjectileFireController))]
    public class Local_VisualBombCharging : MonoBehaviour
    {
        private Shared_VisualBombCharging m_sharedVisualBomb = null;


        private void Awake()
        {
            m_sharedVisualBomb = GetComponent<Shared_VisualBombCharging>();
            Assert.IsNotNull(m_sharedVisualBomb, $"{name}'s {GetType().Name} " +
                $"requires a {nameof(Shared_VisualBombCharging)} but none was " +
                $"found");
        }
        private void Update()
        {
            if (m_sharedVisualBomb.sharedController.isCharging)
            {
                m_sharedVisualBomb.projectilePreviewInstance.SetActive(true);
                m_sharedVisualBomb.UpdatePreviewObjectScale(
                    m_sharedVisualBomb.sharedController.curCharge);
            }
            else
            {
                m_sharedVisualBomb.projectilePreviewInstance.SetActive(false);
            }
        }
    }
}
