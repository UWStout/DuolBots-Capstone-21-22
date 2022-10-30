using UnityEngine;

using Mirror;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Used for hits that don't deal damage. Otherwise, hit sound is handled
    /// by the <see cref="Network_HitSound"/> which plays the sound after a
    /// robot takes damage.
    /// </summary>
    public class Network_PartImpactColliderHitSound : NetworkBehaviour
    {
        [SerializeField, Required] private PartImpactCollider m_partImpCol = null;
        [SerializeField, Required] private WwiseEventName m_hitEventName = null;

        private byte m_myTeamIndex = byte.MaxValue;


        // Domestic Initialization
        private void Awake()
        {
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_partImpCol,
                nameof(m_partImpCol), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_hitEventName,
                nameof(m_hitEventName), this);
            #endregion Asserts
        }
        public override void OnStartServer()
        {
            base.OnStartServer();

            m_partImpCol.onHitEnemy += OnHitEnemy;
        }
        public override void OnStopServer()
        {
            base.OnStopServer();

            m_partImpCol.onHitEnemy -= OnHitEnemy;
        }
        // Foreign Initialization
        private void Start()
        {
            // Find my team index
            m_myTeamIndex = BattlePlayerNetworkObject.myPlayerInstance.
                teamIndex.teamIndex;
        }


        [Server]
        private void OnHitEnemy(byte attackingTeamIndex, byte hitEnemyTeamIndex)
        {
            // If the attacking team is the host
            if (m_myTeamIndex == attackingTeamIndex)
            {
                PlayHitSound();
            }
            // A client is the attacking team
            else
            {
                OnHitEnemyClientRpc(attackingTeamIndex);
            }
        }
        [ClientRpc]
        private void OnHitEnemyClientRpc(byte attackingTeamIndex)
        {
            // Don't do on host twice
            if (isServer) { return; }
            // This client isn't the client that hit an enemy
            if (m_myTeamIndex != attackingTeamIndex) { return; }

            PlayHitSound();
        }
        private void PlayHitSound()
        {
            AkSoundEngine.PostEvent(m_hitEventName.wwiseEventName, gameObject);
        }
    }
}
