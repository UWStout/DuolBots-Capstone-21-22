using UnityEngine;
using UnityEngine.VFX;

using NaughtyAttributes;
// Original Author - Aaron Duffey
// Tweaked for sharing code with networking by Wyatt Senalik.

namespace DuolBots
{
    /// <summary>
    /// Handles VFX played during charging for Weapons that have a charge.
    /// </summary>
    [RequireComponent(typeof(Shared_OnChargeHandler))]
    public class Local_VFXOnChargeHandler : MonoBehaviour
    {
        [SerializeField] [Required] private VisualEffect m_vfx = null;

        private Shared_OnChargeHandler m_sharedHandler = null;


        // Domestic Initialization
        private void Awake()
        {
            m_sharedHandler = GetComponent<Shared_OnChargeHandler>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_sharedHandler, this);
            #endregion Asserts

            StopCharging();
        }
        // Subscribe
        private void OnEnable()
        {
            m_sharedHandler.ToggleSubscription(true, StartCharging, StopCharging);
        }
        // Unsubscribe
        private void OnDisable()
        {
            m_sharedHandler.ToggleSubscription(false, StartCharging, StopCharging);
        }


        private void StartCharging()
        {
            if (m_vfx != null)
            {
                m_vfx.Play();
            }
        }
        private void StopCharging()
        {
            if (m_vfx != null)
            {
                m_vfx.Stop();
            }
        }
    }
}
