using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using NaughtyAttributes;
using Mirror;
// Original Authors - Aaron Duffey and Wyatt Senalik


namespace DuolBots.Mirror
{
    /// <summary>
    /// Applies splatter effects on the canvas of the bot whose parts when the
    /// PartImpactCollider collides with a part.
    /// </summary>
    [RequireComponent(typeof(PartImpactCollider))]
    public class PaintSplatterImpactHandler : NetworkBehaviour, IImpactHandler
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        static private PaintBombSplatterController s_splatterController = null;

        private static PaintBombSplatterController splatterController
        {
            get
            {
                if (s_splatterController == null)
                {
                    #region Logs
                    CustomDebug.Log($"{nameof(PaintSplatterImpactHandler)} " +
                        $"SplatterController is null, finding my splatter " +
                        $"controller", IS_DEBUGGING);
                    #endregion Logs
                    byte temp_teamIndex = BattlePlayerNetworkObject.
                        myPlayerInstance.teamIndex.teamIndex;
                    GameObject m_botRoot = RobotHelpersSingleton.instance.
                        FindBotRoot(temp_teamIndex);
                    #region Asserts
                    Assert.IsNotNull(m_botRoot,
                        $"{typeof(PaintSplatterImpactHandler)} could not find a " +
                        $"bot root but requires one to set the bot " +
                        $"{typeof(PaintBombSplatterController)}");
                    #endregion Asserts
                    s_splatterController = m_botRoot.
                        GetComponentInChildren<PaintBombSplatterController>();
                    #region Asserts
                    CustomDebug.AssertComponentInChildrenOnOtherIsNotNull(
                        s_splatterController, m_botRoot,
                        nameof(PaintSplatterImpactHandler));
                    #endregion Asserts
                }
                return s_splatterController;
            }
        }

        [SerializeField] private int m_priority = 0;
        public int priority => m_priority;

        public void HandleImpact(Collider collider, bool didHitEnemy,
            byte enemyTeamIndex)
        {
            // If we didn't impact an enemy, don't try to apply splatter effect.
           // if (!didHitEnemy) { return; }
            #region Logs
            CustomDebug.LogForComponent($"Trying to apply paint splatter " +
                $"to {collider.name}",
                this, IS_DEBUGGING);
            #endregion Logs

            // If host was hit
            if (enemyTeamIndex == BattlePlayerNetworkObject.myPlayerInstance.
                teamIndex.teamIndex)
            {
                splatterController.ApplySplatters();
            }
            // Check which client we hit
            else
            {
                ApplySplattersToEnemyClientRpc(enemyTeamIndex);
            }
        }

       [ClientRpc]
       private void ApplySplattersToEnemyClientRpc(byte enemyTeamIndex)
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(ApplySplattersToEnemyClientRpc),
                this, IS_DEBUGGING);
            #endregion Logs
            // Don't apply twice for host
            if (isServer) { return; }

            byte temp_teamIndex = BattlePlayerNetworkObject.myPlayerInstance.
                teamIndex.teamIndex;
            // This team was not hit team, do not apply splatter effects.
            if (enemyTeamIndex != temp_teamIndex) { return; }
            splatterController.ApplySplatters();
        }
    }
}
