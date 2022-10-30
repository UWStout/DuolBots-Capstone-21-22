using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.VFX;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Script that handles visual effects on a spawned projectile.
    /// (VFX that applies while the projectile is alive like the HornProjectile's trail).
    /// </summary>
    public class VFXOnSpawnedProjectileHandler : MonoBehaviour
    {
        [SerializeField] private List<VisualEffect> m_vfx = null;
        
        private void Awake()
        {
            if (m_vfx == null)
            {
                m_vfx.AddRange(GetComponentsInChildren<VisualEffect>());
                Assert.IsNotNull(m_vfx, $"{nameof(m_vfx)} was not specificed on {name}'s {GetType().Name}");
            }
            
            if(m_vfx != null && m_vfx.Count > 0)
            {
                foreach (VisualEffect vfx in m_vfx)
                {
                    vfx.Play();
                }
            }
        }
    }
}
