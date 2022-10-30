using System;
using UnityEngine;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Changes the UI based on the <see cref="BetterBuildSceneStateManager"/>.
    /// </summary>
    public class BuildUIUIManager : MonoBehaviour
    {
        private BetterBuildSceneStateManager m_stateMan = null;

        private BetterBuildSceneStateChangeHandler m_chassisMoveHandler = null;
        private BetterBuildSceneStateChangeHandler m_partHandler = null;

        private BuildUIPlayerUIManager[] m_playerUIMans
            = new BuildUIPlayerUIManager[0];


        // Domestic Initialization
        private void Start()
        {
            m_stateMan = BetterBuildSceneStateManager.instance;
            m_playerUIMans = FindObjectsOfType<BuildUIPlayerUIManager>();
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_stateMan, this);
            CustomDebug.AssertIsTrueForComponent(m_playerUIMans.Length >= 2,
                $"at least 2 {nameof(BuildUIPlayerUIManager)}s to be in the scene " +
                $"but only {m_playerUIMans.Length} were found.", this);
            #endregion Asserts

            m_chassisMoveHandler = new BetterBuildSceneStateChangeHandler(
                m_stateMan, BeginChassisMovementHandler,
                EndChassisMovementHandler, eBetterBuildSceneState.Chassis,
                eBetterBuildSceneState.Movement);
            m_partHandler = new BetterBuildSceneStateChangeHandler(
                m_stateMan, BeginPartHandler, EndPartHandler,
                eBetterBuildSceneState.Part);
        }
        private void OnDestroy()
        {
            m_chassisMoveHandler.ToggleActive(false);
            m_partHandler.ToggleActive(false);
        }


        #region Chassis/Movement
        private void BeginChassisMovementHandler()
        {
            foreach (BuildUIPlayerUIManager temp_singPlayerUIMan in m_playerUIMans)
            {
                temp_singPlayerUIMan.ChangeToChassisMoveUI();
            }
        }
        private void EndChassisMovementHandler() { } // Nothing
        #endregion Chassis/Movement

        #region Part
        private void BeginPartHandler()
        {
            foreach (BuildUIPlayerUIManager temp_singPlayerUIMan in m_playerUIMans)
            {
                temp_singPlayerUIMan.ChangeToPartUI();
            }
        }
        private void EndPartHandler() { } // Nothing
        #endregion Part
    }
}
