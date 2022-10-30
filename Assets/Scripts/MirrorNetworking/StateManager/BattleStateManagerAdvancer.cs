using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    [RequireComponent(typeof(BattleStateManager))]
    public class BattleStateManagerAdvancer : NetworkBehaviour
    {
        private bool IS_DEBUGGING = true;

        [SerializeField] [Required]
        private TeamConnectionManager m_teamConMan = null;
        [SerializeField] [Required]
        private OpeningCinematicController m_opCineCont = null;
        [SerializeField] [Required]
        private EndingCinematicController m_edCineCont = null;
        [SerializeField] [Required] private GameOverMonitor m_gameOverMon = null;

        private BattleStateChangeHandler m_waitingHandler = null;
        private BattleStateChangeHandler m_opHandler = null;
        private BattleStateChangeHandler m_battleHandler = null;
        private BattleStateChangeHandler m_edHandler = null;
        private BattleStateChangeHandler m_gameOverHandler = null;
        private BattleStateChangeHandler m_endHandler = null;

        private BattleStateManager m_stateMan = null;


        // Domestic Initialization
        private void Awake()
        {
            m_stateMan = GetComponent<BattleStateManager>();

            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_teamConMan,
                nameof(m_teamConMan), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_opCineCont,
                nameof(m_opCineCont), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_edCineCont,
                nameof(m_edCineCont), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_gameOverMon,
                nameof(m_gameOverMon), this);
            CustomDebug.AssertComponentIsNotNull(m_stateMan, this);
            #endregion Asserts
        }
        public override void OnStartServer()
        {
            base.OnStartServer();

            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_teamConMan,
                nameof(m_teamConMan), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_opCineCont,
                nameof(m_opCineCont), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_gameOverMon,
                nameof(m_gameOverMon), this);
            #endregion Asserts

            // Waiting
            m_waitingHandler = new BattleStateChangeHandler(m_stateMan,
                HandleWaitingBegin, HandleWaitingEnd, eBattleState.Waiting);
            // Opening Cinematic
            m_opHandler = new BattleStateChangeHandler(m_stateMan,
                HandleOpBegin, HandleOpEnd, eBattleState.OpeningCinematic);
            // Battle
            m_battleHandler = new BattleStateChangeHandler(m_stateMan,
                HandleBattleBegin, HandleBattleEnd, eBattleState.Battle);
            // Ending Cinematic
            m_edHandler = new BattleStateChangeHandler(m_stateMan,
                HandleEdBegin, HandleEdEnd, eBattleState.EndingCinematic);
            // Game Over
            m_gameOverHandler = new BattleStateChangeHandler(m_stateMan,
                HandleGameOverBegin, HandleGameOverEnd, eBattleState.GameOver);
            // End
            m_endHandler = new BattleStateChangeHandler(m_stateMan,
                HandleEndBegin, HandleEndEnd, eBattleState.End);
        }
        public override void OnStopServer()
        {
            base.OnStopServer();

            m_waitingHandler.ToggleActive(false);
            m_opHandler.ToggleActive(false);
            m_battleHandler.ToggleActive(false);
            m_edHandler.ToggleActive(false);
            m_gameOverHandler.ToggleActive(false);
            m_endHandler.ToggleActive(false);
        }


        #region Waiting
        private void HandleWaitingBegin()
        {
            #region Logs
            CustomDebug.Log($"{nameof(eBattleState.Waiting)} State " +
                $"<color=green>Begun</color>", IS_DEBUGGING);
            #endregion Logs
            // Advance from waiting once both teams have joined
            m_teamConMan.onAllTeamsJoined.ToggleSubscription(
                m_stateMan.AdvanceState, true);

        }
        private void HandleWaitingEnd()
        {
            #region Logs
            CustomDebug.Log($"{nameof(eBattleState.Waiting)} State " +
                $"<color=red>Ended</color>", IS_DEBUGGING);
            #endregion Logs
            // Unsub from waiting subs
            m_teamConMan.onAllTeamsJoined.ToggleSubscription(
                m_stateMan.AdvanceState, false);
        }
        #endregion Waiting
        #region Opening Cinematic
        private void HandleOpBegin()
        {
            #region Logs
            CustomDebug.Log($"{nameof(eBattleState.OpeningCinematic)} State " +
                $"<color=green>Begun</color>", IS_DEBUGGING);
            #endregion Logs
            // Sub to the op cinematic's onFinished event.
            m_opCineCont.onFinished.ToggleSubscription(m_stateMan.AdvanceState,
                true);
            m_opCineCont.StartCinematic();
        }
        private void HandleOpEnd()
        {
            #region Logs
            CustomDebug.Log($"{nameof(eBattleState.OpeningCinematic)} State " +
                $"<color=red>Ended</color>", IS_DEBUGGING);
            #endregion Logs
            // Unsub from the op cinematic's onFinished event.
            m_opCineCont.onFinished.ToggleSubscription(m_stateMan.AdvanceState,
                false);
        }
        #endregion Opening Cinematic
        #region Battle
        private void HandleBattleBegin()
        {
            #region Logs
            CustomDebug.Log($"{nameof(eBattleState.Battle)} State " +
                $"<color=green>Begun</color>", IS_DEBUGGING);
            #endregion Logs
            m_gameOverMon.onGameOverNoParam.ToggleSubscription(
                m_stateMan.AdvanceState, true);
        }
        private void HandleBattleEnd()
        {
            #region Logs
            CustomDebug.Log($"{nameof(eBattleState.Battle)} State " +
                $"<color=red>Ended</color>", IS_DEBUGGING);
            #endregion Logs
            m_gameOverMon.onGameOverNoParam.ToggleSubscription(
                m_stateMan.AdvanceState, false);
        }
        #endregion Battle
        #region Ending Cinematic
        private void HandleEdBegin()
        {
            #region Logs
            CustomDebug.Log($"{nameof(eBattleState.EndingCinematic)} State " +
                $"<color=green>Begun</color>", IS_DEBUGGING);
            #endregion Logs
            m_edCineCont.onFinished.ToggleSubscription(m_stateMan.AdvanceState,
                true);
            m_edCineCont.StartCinematic();
        }
        private void HandleEdEnd()
        {
            #region Logs
            CustomDebug.Log($"{nameof(eBattleState.EndingCinematic)} State " +
                $"<color=red>Ended</color>", IS_DEBUGGING);
            #endregion Logs
            m_edCineCont.onFinished.ToggleSubscription(m_stateMan.AdvanceState,
                false);
        }
        #endregion Ending Cinematic
        #region Game Over
        private void HandleGameOverBegin()
        {
            #region Logs
            CustomDebug.Log($"{nameof(eBattleState.GameOver)} State " +
                $"<color=green>Begun</color>", IS_DEBUGGING);
            #endregion Logs
            // TODO - Listen to something that lets us know the player
            // has made a selection in the game over screen
            // Maybe? Idk, the GameOverScreen just sets the state anyway,
            // so we don't REALLY need to do this.
        }
        private void HandleGameOverEnd()
        {
            #region Logs
            CustomDebug.Log($"{nameof(eBattleState.GameOver)} State " +
                $"<color=red>Ended</color>", IS_DEBUGGING);
            #endregion Logs
            // TODO - Unsub from thing mentioned above
        }
        #endregion Game Over
        #region End
        private void HandleEndBegin()
        {
            #region Logs
            CustomDebug.Log($"{nameof(eBattleState.End)} State " +
                $"<color=green>Begun</color>", IS_DEBUGGING);
            #endregion Logs
        }
        private void HandleEndEnd()
        {
            #region Logs
            CustomDebug.Log($"{nameof(eBattleState.End)} State " +
                $"<color=red>Ended</color>", IS_DEBUGGING);
            #endregion Logs
        }
        #endregion End


        /// <summary>
        /// Advances the state after a specified time.
        /// </summary>
        private void AdvanceStateAfterTime(float waitTime)
        {
            StartCoroutine(AdvanceStateAfterTimeCoroutine(waitTime));
        }
        private IEnumerator AdvanceStateAfterTimeCoroutine(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            m_stateMan.AdvanceState();
        }
    }
}
