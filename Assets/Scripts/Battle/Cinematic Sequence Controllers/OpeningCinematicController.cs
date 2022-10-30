using UnityEngine;

using Mirror;

using DuolBots.Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Drive the opening cinematic.
    /// As of now, it just spawns the bots and says its done.
    /// </summary>
    [RequireComponent(typeof(NetworkBotInstantiator))]
    public class OpeningCinematicController : NetworkBehaviour,
        ICinematicSequenceController
    {
        private const bool IS_DEBUGGING = false;

        private NetworkBotInstantiator m_botInstantiator = null;

        public IEventPrimer onFinished => m_onFinished;
        private CatchupEvent m_onFinished = new CatchupEvent();


        // Domestic Initialization
        private void Awake()
        {
            m_botInstantiator = GetComponent<NetworkBotInstantiator>();

            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_botInstantiator, this);
            #endregion Asserts
        }
        // Foreign Initialization
        private void Start()
        {
            CatchupEventResetter temp_eventResetter = CatchupEventResetter.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(temp_eventResetter,
                this);
            #endregion Asserts
            temp_eventResetter.AddCatchupEventForReset(m_onFinished);
        }


        public void StartCinematic()
        {
            // For now - Just spawn the bots and finish
            // TODO - Make its an actual cinematic

            // Only the server is allowed to spawn bots.
            if (!isServer) { return; }

            m_botInstantiator.SpawnBots();

            m_onFinished.Invoke();
            InvokeOnFinishedClientRpc();
        }


        [ClientRpc]
        private void InvokeOnFinishedClientRpc()
        {
            CustomDebug.Log(nameof(InvokeOnFinishedClientRpc), IS_DEBUGGING);
            m_onFinished.Invoke();
        }
    }
}
