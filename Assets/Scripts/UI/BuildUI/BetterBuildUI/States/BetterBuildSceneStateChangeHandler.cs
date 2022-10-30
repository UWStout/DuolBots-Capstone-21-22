using System;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// <see cref="StateChangeHandler{TEnum}"/> for
    /// <see cref="eBetterBuildSceneState"/>.
    /// </summary>
    public class BetterBuildSceneStateChangeHandler :
        StateChangeHandler<eBetterBuildSceneState>
    {
        public BetterBuildSceneStateChangeHandler(
            IStateManager<eBetterBuildSceneState> stateMan,
            Action activateAction = null, Action deactivateAction = null,
            params eBetterBuildSceneState[] activateStates) :
            base(stateMan, activateAction, deactivateAction, activateStates)
        {
        }

        public static BetterBuildSceneStateChangeHandler CreateNew(
            Action activateAction = null, Action deactivateAction = null,
            params eBetterBuildSceneState[] activateStates)
        {
            BetterBuildSceneStateManager temp_stateMan
                = BetterBuildSceneStateManager.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(temp_stateMan,
                $"{typeof(BetterBuildSceneStateChangeHandler)}");
            #endregion Asserts

            return new BetterBuildSceneStateChangeHandler(temp_stateMan,
                activateAction, deactivateAction, activateStates);
        }
    }
}
