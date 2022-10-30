using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik and Aaron Duffey

namespace DuolBots
{ 
    public class BattleStateMusicManager : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField, Required] private BattleStateManager m_stateMan = null;
        [SerializeField, Required]
        private WwiseEventName m_battleMusicEventName = null;

        private WwiseMusicManager m_musicMan = null;

        private BattleStateChangeHandler m_battleHandler = null;
        private BattleStateChangeHandler m_gameOverHandler = null;


        private void Start()
        {
            m_musicMan = WwiseMusicManager.instance;
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_stateMan,
                nameof(m_stateMan), this);
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_musicMan, this);
            #endregion Asserts

            m_battleHandler = new BattleStateChangeHandler(m_stateMan,
                BeginBattleHandler, EndBattleHandler, eBattleState.Battle);
            m_gameOverHandler = new BattleStateChangeHandler(m_stateMan,
                BeginGameOverHandler, EndGameOverHandler, eBattleState.GameOver);
        }
        private void OnDestroy()
        {
            m_battleHandler.ToggleActive(false);
            m_gameOverHandler.ToggleActive(false);
        }


        private void BeginBattleHandler()
        {
            CustomDebug.LogForComponent(nameof(BeginBattleHandler),
                this, IS_DEBUGGING);
            m_musicMan.PlayMusic(m_battleMusicEventName);
        }
        private void EndBattleHandler() { }
        // Music will be stopped by other music playing

        private void BeginGameOverHandler()
        {

        }
        private void EndGameOverHandler() { }
        // Music will be stopped by other music playing
    }
}
