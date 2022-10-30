using UnityEngine;

using DuolBots.Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    public enum eBattleState { Waiting, OpeningCinematic, Battle,
        EndingCinematic, GameOver, End }

    /// <summary>
    /// Holds the current state the battle is in and invokes an event on
    /// state change.
    ///
    /// Also determines when to advance states.
    ///
    /// States and when they advance is outlined below:
    ///
    /// 0. (Default) Waiting
    ///     Continues until both players have joined the game.
    /// 1. Opening Cinematic
    ///     Continues only as long as the cinematic does.
    /// 2. Battle
    ///     Continues until the <see cref="GameOverMonitor"/> determines that the
    ///     game/battle should be over.
    /// 3. Ending Cinematic
    ///     Continues only as long as the cinematic does.
    /// 4. Game Over
    ///     Continues until the player makes a selection on the game over screen
    ///     which will bring them out of this state or out of the scene.
    /// 5. End
    ///     This state exists only to be transitioned to once the player has made a
    ///     selection during the Game Over state so that appropriate things can be
    ///     cleaned up.
    /// </summary>
    public class BattleStateManager : NetworkStateManager,
        IStateManager<eBattleState>
    {
        private const bool IS_DEBUGGING = false;

        public static BattleStateManager instance => s_instance;
        private static BattleStateManager s_instance = null;

        [SerializeField] private bool m_isNetworked = true;

        private CatchupEvent<eBattleState> m_onInitialStateSet
            = new CatchupEvent<eBattleState>();
        private CatchupEvent<eBattleState, eBattleState> m_onStateChange
            = new CatchupEvent<eBattleState, eBattleState>();
        #region IStateManager<eBattleState>
        public eBattleState curState => (eBattleState)curStateInternal;
        public IEventPrimer<eBattleState> onInitialStateSet => m_onInitialStateSet;
        public IEventPrimer<eBattleState, eBattleState> onStateChange => m_onStateChange;
        #endregion IStateManager<eBattleState>


        // Domestic Initialization
        protected override void Awake()
        {
            base.Awake();

            if (s_instance == null)
            {
                s_instance = this;
            }
            else
            {
                Debug.LogError($"Multiple {GetType().Name} exist in the scene. " +
                    $"Destroying the newer one");
                Destroy(gameObject);
            }
        }
        public override void OnStartServer()
        {
            base.OnStartServer();

            Initialize();
        }
        public override void OnStopServer()
        {
            base.OnStopServer();

            CleanUp();
        }
        private void Start()
        {
            if (m_isNetworked) { return; }
            // Only do in a local scene, if there is no server
            Initialize();
        }
        private void OnDestroy()
        {
            if (m_isNetworked) { return; }
            // Only do in a local scene, if there is no server
            CleanUp();
        }


        public void SetState(eBattleState newState) => SetState((byte)newState);


        private void Initialize()
        {
            CatchupEventResetter temp_eventResetter = CatchupEventResetter.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(temp_eventResetter,
               this);
            #endregion Asserts

            onInitialStateSetInternal.ToggleSubscription(
                PassAlongInitialStateSetCall, true);
            onStateChangeInternal.ToggleSubscription(PassAlongStateChangeCall,
                true);

            temp_eventResetter.AddCatchupEventForReset(
                resetOnInitialStateSetInternal);
            temp_eventResetter.AddCatchupEventForReset(resetOnStateChangeInternal);
            temp_eventResetter.AddCatchupEventForReset(m_onInitialStateSet);
            temp_eventResetter.AddCatchupEventForReset(m_onStateChange);
        }
        private void CleanUp()
        {
            onInitialStateSetInternal.ToggleSubscription(
                PassAlongInitialStateSetCall, false);
            onStateChangeInternal.ToggleSubscription(PassAlongStateChangeCall,
                false);
        }
        private void PassAlongInitialStateSetCall(byte newState)
        {
            CustomDebug.Log(nameof(PassAlongInitialStateSetCall), IS_DEBUGGING);
            m_onInitialStateSet.Invoke((eBattleState)newState);
        }
        private void PassAlongStateChangeCall(byte oldState, byte newState)
        {
            m_onStateChange.Invoke((eBattleState)oldState,
                (eBattleState)newState);
        }
    }
}
