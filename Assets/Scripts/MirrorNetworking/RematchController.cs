using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    public class RematchController : NetworkBehaviour
    {
        public static RematchController instance => s_instance;
        private static RematchController s_instance = null;

        [SerializeField] [Min(0.0f)] private float m_endBufferTime = 1.5f;

        private BattleStateManager m_battleStateMan = null;
        private TeamConnectionManager m_teamConMan = null;

        private readonly SyncList<byte> m_teamsThatWantRematch
            = new SyncList<byte>();

        public int amountTeamsThatWantRematch => m_teamsThatWantRematch.Count;


        // Domestic Initialization
        private void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this;
            }
            else
            {
                Debug.LogError($"Multiple {GetType().Name} in the scene. " +
                    $"Destroying the other");
                Destroy(gameObject);
            }
        }
        // Foreign Initialization
        private void Start()
        {
            m_teamConMan = TeamConnectionManager.instance;
            m_battleStateMan = BattleStateManager.instance;

            #region Asserts
            CustomDebug.AssertIsTrueForComponent(m_teamConMan != null,
                $"{nameof(TeamConnectionManager)} to be a singleton in the scene",
                this);
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_battleStateMan,
                this);
            #endregion Asserts
        }


        [Server]
        public void RequestRematchServer(byte teamIndex)
        {
            if (m_teamsThatWantRematch.Contains(teamIndex)) { return; }

            m_teamsThatWantRematch.Add(teamIndex);
            // All teams want a rematch, start it.
            if (m_teamsThatWantRematch.Count >= m_teamConMan.requiredTeamAmount)
            {
                BeginRematch();
            }
        }
        [Server]
        public void CancelRematchRequestServer(byte teamIndex)
        {
            m_teamsThatWantRematch.Remove(teamIndex);
        }
        public void ToggleSubscriptionToTeamsThatWantRematchUpdate(
            SyncList<byte>.SyncListChanged callback,
            bool cond)
        {
            // Subscribe
            if (cond)
            {
                m_teamsThatWantRematch.Callback += callback;
            }
            // Unsubscribe
            else
            {
                m_teamsThatWantRematch.Callback -= callback;
            }
        }


        [Server]
        private void BeginRematch()
        {
            StartCoroutine(BeginRematchCoroutine());
        }
        [Server]
        private IEnumerator BeginRematchCoroutine()
        {
            // Set the state manager to end briefly, to reset things.
            m_battleStateMan.SetState(eBattleState.End);

            // Wait one frame for any listens to handle themselves.
            yield return new WaitForEndOfFrame();
            yield return null;
            // Also wait the time specified
            yield return new WaitForSeconds(m_endBufferTime);

            // Now move the state back to the opening cinematic
            m_battleStateMan.SetState(eBattleState.OpeningCinematic);
        }
    }
}
