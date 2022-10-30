using UnityEngine;

using NaughtyAttributes;
// Original Author - Aaron Duffey
// Tewaked by Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Script that holds specifications and behavioral variables for
    /// DrillFireControllers
    /// </summary>
    [DisallowMultipleComponent]
    public class Specifications_DrillFireController : MonoBehaviour
    {
        [SerializeField] private DrillProjectile m_drillProjectile = null;
        public DrillProjectile drillProjectile => m_drillProjectile;
        [SerializeField] private Transform m_turretTransform = null;
        public Transform turretTransform => m_turretTransform;
        // How long the drill must wait before jabbing again.
        [SerializeField] private float m_jabDelay = 1.0f;
        public float jabDelay => m_jabDelay;
        // How long the drill remains in the jabbing position
        [SerializeField] private float m_jabDuration = 1.0f;
        public float jabDuration => m_jabDuration;
        // Time increment before the drill is allowed to deal damage again.
        [SerializeField] private float m_dmgResetTime = 0.1f;

        // Curve for how to jab
        [SerializeField] private BetterCurve m_jabCurve = null;
        // Curve for how to move back after jabbing
        [SerializeField] private BetterCurve m_resetCurve = null;

        public float dmgResetTime => m_dmgResetTime;

        public BetterCurve jabCurve => m_jabCurve;
        public BetterCurve resetCurve => m_resetCurve;
    }
}
