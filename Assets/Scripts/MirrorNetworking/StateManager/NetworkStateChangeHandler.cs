using System;
using System.Collections.Generic;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Network version of <see cref="StateChangeHandler{TEnum}"/>.
    /// </summary>
    public class NetworkStateChangeHandler
    {
        private const bool IS_DEBUGGING = false;

        private NetworkStateManager m_stateMan = null;
        private List<byte> m_activateStates = null;
        private Action m_activateAction = null;
        private Action m_deactivateAction = null;

        private bool m_isActive = false;


        /// <summary>
        /// Constructs a <see cref="NetworkStateChangeHandler"/>.
        /// </summary>
        /// <param name="stateMan">The state manager to respond to the state
        /// changing.</param>
        /// <param name="activateAction">The action to invoke when an activate state
        /// is changed to from an inactivate state.</param>
        /// <param name="deactivateAction">The action to invoke when an inactivate
        /// state is change to from an activate state.</param>
        /// <param name="activateStates">States that are marked as the activate
        /// ones. AKA the states that you want to be active for.</param>
        public NetworkStateChangeHandler(NetworkStateManager stateMan,
            Action activateAction = null, Action deactivateAction = null,
            params byte[] activateStates)
        {
            m_stateMan = stateMan;
            m_activateStates = new List<byte>(activateStates);
            m_activateAction = activateAction;
            m_deactivateAction = deactivateAction;

            ToggleActive(true);
        }


        /// <summary>
        /// Activate the <see cref="NetworkStateChangeHandler"/> to
        /// subscribe to the <see cref="NetworkStateManager"/>
        /// state changes.
        ///
        /// Starts enabled by default.
        /// </summary>
        /// <param name="cond">Enable (true) or disable (false).</param>
        public void ToggleActive(bool cond)
        {
            #region Logs
            CustomDebug.Log($"{GetType().Name}'s {nameof(ToggleActive)}",
                IS_DEBUGGING);
            #endregion Logs

            if (m_isActive == cond) { return; }

            // If this is called on destroy, its possible that the state
            // manager is null, in which case its fine.
            if (m_stateMan == null) { return; }

            m_stateMan.onInitialStateSetInternal.
                ToggleSubscription(HandleOnInitialStateSet, cond);
            m_stateMan.onStateChangeInternal.
                ToggleSubscription(HandleOnStateChange, cond);

            m_isActive = cond;
        }


        /// <summary>
        /// Calls the activate action if the initial state is one of the activate
        /// states.
        /// </summary>
        private void HandleOnInitialStateSet(byte initState)
        {

            // If the initial state is one of the activate states,
            // invoke the activate action
            if (m_activateStates.Contains(initState))
            {
                #region Logs
                CustomDebug.Log($"{nameof(HandleOnInitialStateSet)} with param " +
                    $"{initState} is <color=green>an active</color> state.",
                    IS_DEBUGGING);
                #endregion Logs

                m_activateAction?.Invoke();
            }
            else
            {
                #region Logs
                CustomDebug.Log($"{nameof(HandleOnInitialStateSet)} with param " +
                    $"{initState} is <color=red>not an active</color> state",
                    IS_DEBUGGING);
                #endregion Logs
            }
        }
        /// <summary>
        /// Handles moving from an inactive state to an active one
        /// or moving from an activate state to an inactive one.
        /// </summary>
        private void HandleOnStateChange(byte prevState, byte newState)
        {
            // Old state was an activate one, so if the new state is not also
            // an activate state, then we need to invoke the deactivate action
            if (m_activateStates.Contains(prevState))
            {
                // New state is not an activate state, so deactivate
                if (!m_activateStates.Contains(newState))
                {
                    #region Logs
                    CustomDebug.Log($"{nameof(HandleOnStateChange)} with params " +
                        $"{nameof(prevState)}={prevState} and " +
                        $"{nameof(newState)}={newState} caused a " +
                        $"<color=red>deactivation</color>.", IS_DEBUGGING);
                    #endregion Logs

                    m_deactivateAction?.Invoke();
                }
                else
                {
                    #region Logs
                    CustomDebug.Log($"{nameof(HandleOnStateChange)} with params " +
                        $"{nameof(prevState)}={prevState} and " +
                        $"{nameof(newState)}={newState} caused " +
                        $"<color=yellow>nothing</color>. They are both " +
                        $"<color>=green>active</color> states.",
                        IS_DEBUGGING);
                    #endregion Logs
                }
            }
            // Old state was not an activate one, so if the new state is, then
            // we need to invoke the activate action
            else
            {
                // New state is an activate state, so activate
                if (m_activateStates.Contains(newState))
                {
                    #region Logs
                    CustomDebug.Log($"{nameof(HandleOnStateChange)} with params " +
                        $"{nameof(prevState)}={prevState} and " +
                        $"{nameof(newState)}={newState} caused a " +
                        $"<color=green>activation</color>.", IS_DEBUGGING);
                    #endregion Logs

                    m_activateAction?.Invoke();
                }
                else
                {
                    #region Logs
                    CustomDebug.Log($"{nameof(HandleOnStateChange)} with params " +
                        $"{nameof(prevState)}={prevState} and " +
                        $"{nameof(newState)}={newState} caused " +
                        $"<color=yellow>nothing</color>. They are both " +
                        $"<color=red>inactive</color> states.",
                        IS_DEBUGGING);
                    #endregion Logs
                }
            }
        }
    }
}
