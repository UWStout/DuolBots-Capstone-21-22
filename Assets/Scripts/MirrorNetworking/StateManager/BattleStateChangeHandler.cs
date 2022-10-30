using System;

using DuolBots.Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// StateChangeHandler for the battle state.
    /// </summary>
    public class BattleStateChangeHandler : NetworkStateChangeHandler
    {
        private const bool IS_DEBUGGING = false;

        public BattleStateChangeHandler(NetworkStateManager stateMan,
            Action activateAction, Action deactivateAction,
            params eBattleState[] activateStates) :
            base(stateMan, activateAction, deactivateAction,
                ConvertBattleStatesToBytes(activateStates))
        { }


        /// <summary>
        /// DONT USE? BROKEN?
        /// </summary>
        /// <param name="activateAction"></param>
        /// <param name="deactivateAction"></param>
        /// <param name="activateStates"></param>
        /// <returns></returns>
        [Obsolete]
        public static BattleStateChangeHandler CreateNew(Action activateAction = null,
            Action deactivateAction = null, params eBattleState[] activateStates)
        {
            BattleStateManager temp_stateMan = BattleStateManager.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(temp_stateMan,
                $"{nameof(BattleStateChangeHandler)}");
            #endregion Asserts
            return new BattleStateChangeHandler(temp_stateMan, activateAction,
                deactivateAction, activateStates);
        }

        private static byte[] ConvertBattleStatesToBytes(
            eBattleState[] battleStates)
        {
            #region Logs
            string temp_statesStr = "";
            string temp_bytesStr = "";
            #endregion Logs
            byte[] temp_byteArr = new byte[battleStates.Length];
            for (int i = 0; i < battleStates.Length; ++i)
            {
                temp_byteArr[i] = (byte)(battleStates[i]);
                #region Logs
                temp_statesStr += $"{(battleStates[i])}, ";
                temp_bytesStr += $"{temp_byteArr[i]}, ";
                #endregion Logs
            }
            #region Logs
            if (battleStates.Length > 0)
            {
                temp_statesStr = temp_statesStr.Substring(0, temp_statesStr.Length - 2);
                temp_bytesStr = temp_bytesStr.Substring(0, temp_bytesStr.Length - 2);
                CustomDebug.Log($"Converted [{temp_statesStr}] into [{temp_bytesStr}]",
                    IS_DEBUGGING);
            }
            #endregion Logs
            return temp_byteArr;
        }
    }
}
