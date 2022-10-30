using UnityEngine;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// When the required amount of teams join, invokes an event
    /// on both server and client for on all teams joined.
    /// </summary>
    public class TeamConnectionManager : NetworkBehaviour
    {
        private const bool IS_DEBUGGING = false;

        public static TeamConnectionManager instance => s_instance;
        private static TeamConnectionManager s_instance = null;

        public int requiredTeamAmount => m_requiredTeamAmount;
        [SerializeField] private int m_requiredTeamAmount = 2;
        [SerializeField] private int m_inEditorRequiredTeams = 1;

        private readonly SyncList<byte> m_connectedTeams = new SyncList<byte>();
        
        public IEventPrimer onAllTeamsJoined => m_onAllTeamsJoined;
        private CatchupEvent m_onAllTeamsJoined = new CatchupEvent();


        // Domestic Initialization (Server and Client)
        private void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this;
            }
            else
            {
                Debug.LogError($"Multiple {GetType().Name} exist in the scene");
            }

            m_connectedTeams.Callback += OnConnectedTeamsUpdated;

#if UNITY_EDITOR
            m_requiredTeamAmount = m_inEditorRequiredTeams;
#endif
        }
        // Foreign Initialization (client and server)
        private void Start()
        {
            CatchupEventResetter temp_eventResetter = CatchupEventResetter.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(temp_eventResetter,
                this);
            #endregion Asserts
            temp_eventResetter.AddCatchupEventForReset(m_onAllTeamsJoined);
        }
        public override void OnStartClient()
        {
            base.OnStartClient();

            // Check if the all the teams have joined on start
            HandleIfAllTeamsJoined();
        }
        private void OnDestroy()
        {
            m_connectedTeams.Callback -= OnConnectedTeamsUpdated;
        }


        /// <summary>
        /// Registers a team with the given team index as connected.
        /// </summary>
        [Server]
        public void ConnectTeam(byte teamIndex)
        {
            m_connectedTeams.Add(teamIndex);
        }
        [Server]
        public void DisconnectTeam(byte teamIndex)
        {
            m_connectedTeams.Remove(teamIndex);
        }


        private void OnConnectedTeamsUpdated(SyncList<byte>.Operation op,
            int itemIndex, byte oldItem, byte newItem)
        {
            HandleIfAllTeamsJoined();
        }
        private void HandleIfAllTeamsJoined()
        {
            if (m_connectedTeams.Count >= m_requiredTeamAmount)
            {
                CustomDebug.Log($"Invoking {nameof(m_onAllTeamsJoined)}",
                    IS_DEBUGGING);
                m_onAllTeamsJoined.Invoke();
            }
        }
    }
}
