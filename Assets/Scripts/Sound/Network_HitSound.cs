using UnityEngine;

using Mirror;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Listens to the <see cref="Shared_RobotHealth"/> and when it takes damage,
    /// plays the hit sound on the client/host that dealt the damage.
    /// </summary>
    [RequireComponent(typeof(Shared_RobotHealth))]
    public class Network_HitSound : NetworkBehaviour
    {
        [SerializeField, Required] private WwiseEventName m_hitEventName = null;
        [SerializeField, Min(0.0f)]
        private float m_minTimeBetweenNextHitSound = 0.5f;

        private Shared_RobotHealth m_robotHealth = null;
        private byte m_myTeamIndex = byte.MaxValue;
        private float m_lastHitTime = float.MinValue;


        // Domestic Initialization (Server and Client)
        private void Awake()
        {
            m_robotHealth = GetComponent<Shared_RobotHealth>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_robotHealth, this);
            #endregion Asserts
        }
        public override void OnStartServer()
        {
            base.OnStartServer();

            m_robotHealth.onTookDamageFromTeam += OnTookDamageFromTeam;
        }
        public override void OnStopServer()
        {
            base.OnStopServer();

            // If on destroy and other is destroyed first
            if (m_robotHealth != null)
            {
                m_robotHealth.onTookDamageFromTeam -= OnTookDamageFromTeam;
            }
        }
        // Foreign Initialization (Server and Client)
        private void Start()
        {
            if (!isServerOnly)
            {
                // Find my team index
                m_myTeamIndex = BattlePlayerNetworkObject.myPlayerInstance.
                    teamIndex.teamIndex;
            }
        }


        /// <summary>
        /// Handles when a robot takes damage and which team
        /// to play the hit sound for.
        /// </summary>
        /// <param name="attackingTeamIndex">Index of the team that is
        /// attacked and dealt the damage (the team who we will play the hit confirm
        /// sound for.</param>
        [Server]
        private void OnTookDamageFromTeam(float damageTaken,
            byte attackingTeamIndex)
        {
            // Host is the team that attacked
            if (m_myTeamIndex == attackingTeamIndex)
            {
                PlayHitSound();
                return;
            }
            // Host isn't team that attacked, find client who is
            OnTookDamageFromTeamClientRpc(attackingTeamIndex);
        }
        /// <summary>
        /// Looks in the clients for the client who is the attacking team.
        /// </summary>
        /// <param name="attackingTeamIndex">Index of the team that is
        /// attacked and dealt the damage (the team who we will play the hit confirm
        /// sound for.</param>
        [ClientRpc]
        private void OnTookDamageFromTeamClientRpc(byte attackingTeamIndex)
        {
            // Host already checked.
            if (isServer) { return; }
            // I am not the team that attacked
            if (m_myTeamIndex != attackingTeamIndex) { return; }
            // I am the team that attacked, so play the sound
            PlayHitSound();
        }
        private void PlayHitSound()
        {
            // Don't play the sound if it was played very recently.
            if (Time.time < m_lastHitTime + m_minTimeBetweenNextHitSound)
            { return; }
            m_lastHitTime = Time.time;

            AkSoundEngine.PostEvent(m_hitEventName.wwiseEventName, gameObject);
        }
    }
}
