using UnityEngine;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Grouping of the dolly target cycler for the camera of each
    /// stage of the build UI.
    /// </summary>
    [RequireComponent(typeof(PlayerIndex))]
    public class PlayerCameras_BuildUI : MonoBehaviour
    {
        [SerializeField] private DollyTargetCycler m_chassisMoveCycler = null;
        [SerializeField] private DollyTargetCycler m_partCycler = null;

        private PlayerIndex m_playerIndex = null;

        public DollyTargetCycler chassisMoveCycler => m_chassisMoveCycler;
        public DollyTargetCycler partCycler => m_partCycler;
        public byte playerIndex => m_playerIndex.playerIndex;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            m_playerIndex = GetComponent<PlayerIndex>();
            CustomDebug.AssertComponentIsNotNull(m_playerIndex, this);
        }
    }
}
