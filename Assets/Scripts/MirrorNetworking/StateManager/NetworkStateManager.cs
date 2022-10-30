using System;
using UnityEngine;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Networked version of a <see cref="BaseStateManager{TEnum, TSelf}"./>
    /// </summary>
    /// <typeparam name="TEnum">Enum of the states. MUST have default int conversion
    /// values.</typeparam>
    /// <typeparam name="TSelf">Must be the type extending
    /// <see cref="NetworkStateManager"/>.</typeparam>
    public abstract class NetworkStateManager : NetworkBehaviour
    {
        private readonly SyncVar<byte> m_curState = new SyncVar<byte>(0);

        /// <summary>
        /// Parameter: Initial State.
        /// </summary>
        private CatchupEvent<byte> m_onInitialStateSetInternal
            = new CatchupEvent<byte>();
        /// <summary>
        /// Parameters: Previous State and New State.
        /// </summary>
        private CatchupEvent<byte, byte> m_onStateChangeInternal
            = new CatchupEvent<byte, byte>();

        protected ICatchupEventReset resetOnInitialStateSetInternal
            => m_onInitialStateSetInternal;
        protected ICatchupEventReset resetOnStateChangeInternal
            => m_onStateChangeInternal;

        public byte curStateInternal => m_curState.Value;
        public IEventPrimer<byte> onInitialStateSetInternal
            => m_onInitialStateSetInternal;
        public IEventPrimer<byte, byte> onStateChangeInternal
            => m_onStateChangeInternal;


        // Domestic Initialization
        protected virtual void Awake()
        {
            m_onInitialStateSetInternal.Invoke(m_curState.Value);
        }


        /// <summary>
        /// Advances to the next state.
        /// See <see cref="TEnum"/> for the order.
        ///
        /// Can only be called by the Server.
        /// </summary>
        [Server]
        public void AdvanceState()
        {
            SetState((byte)(m_curState.Value + 1));
        }
        /// <summary>
        /// Sets the currently active state to the one specified.
        ///
        /// Can only be called by the Server.
        /// </summary>
        /// <param name="newState">State to set.</param>
        [Server]
        public void SetState(byte newState)
        {
            byte temp_oldState = m_curState.Value;
            m_curState.Value = newState;
            CurStateChangedClientRpc(temp_oldState, newState);
        }


        /// <summary>
        /// Called when the current state is updated.
        /// </summary>
        [ClientRpc]
        private void CurStateChangedClientRpc(byte oldState, byte newState)
        {
            m_onStateChangeInternal.Invoke(oldState, newState);
        }
    }
}
