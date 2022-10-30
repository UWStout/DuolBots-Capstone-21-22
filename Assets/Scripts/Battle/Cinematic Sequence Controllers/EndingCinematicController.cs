using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Mirror;
using NaughtyAttributes;

using Cinemachine.Extensions;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    public class EndingCinematicController : NetworkBehaviour,
        ICinematicSequenceController
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField] [Required]
        private GameOverMonitor m_gameOverMonitor = null;
        [SerializeField] [Required]
        private CineBrainBlendMonitor m_blendMon = null;
        [SerializeField] [Required] private GameObject m_explosion = null;
        [SerializeField] [Min(0.0f)] private float m_explosionKillBotTime = 2.0f;
        [SerializeField] [Min(0.0f)] private float m_explosionRuntime = 5.0f;
        private CatchupEvent m_onFinished = new CatchupEvent();


        public IEventPrimer onFinished => m_onFinished;


        // Domestic Initialization
        private void Awake()
        {
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_gameOverMonitor,
                nameof(m_gameOverMonitor), this);
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


        [Server]
        public void StartCinematic()
        {
            // Wait until the blend ends
            m_blendMon.onEndBlendEvent += StartEDCinematicAfterBlend;
        }


        [Server]
        private void StartEDCinematicAfterBlend()
        {
            m_blendMon.onEndBlendEvent -= StartEDCinematicAfterBlend;
            StartEDCinematic();
        }
        [Server]
        private void StartEDCinematic()
        {
            GameOverData temp_goData = m_gameOverMonitor.gameOverData;
            #region Asserts
            CustomDebug.AssertIsTrueForComponent(temp_goData != null,
                $"GameOverData to be specified, but it was null.", this);
            #endregion Asserts

            // Handle what to do differently depending on the game over cause.
            switch (temp_goData.cause)
            {
                // Go to the game over screen right away? (Custom msg?)
                case eGameOverCause.Default:
                    InvokeOnFinished();
                    break;
                // Play explosion anim on the lower health bot
                case eGameOverCause.Health:
                    HandleHealthED(temp_goData);
                    break;
                // Display Time Up! and play explosion anim on the lower health bot?
                case eGameOverCause.Time:
                    HandleTimeED(temp_goData);
                    break;
                // Go to the game over screen right away? (Custom msg?)
                case eGameOverCause.Disconnect:
                    InvokeOnFinished();
                    break;
                default:
                    CustomDebug.UnhandledEnum(temp_goData.cause, this);
                    break;
            }
        }
        [Server]
        private void HandleHealthED(GameOverData gameOverData)
        {
            // Find the bot(s) belonging to the teams who have lost
            RobotHelpersSingleton temp_botHelpers = RobotHelpersSingleton.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(temp_botHelpers,
                this);
            #endregion Asserts
            // Begin by getting all the bots who have won
            IReadOnlyList<byte> temp_winningTeamIndicies = gameOverData.winningTeamIndices;
            #region Asserts
            CustomDebug.AssertIsTrueForComponent(temp_winningTeamIndicies.Count > 0,
                $"at least one one team needs to have won.", this);
            #endregion Asserts
            IReadOnlyList<GameObject> temp_losingBots = temp_botHelpers.FindLosingBots(
                temp_winningTeamIndicies);

            // Spawn the explosion for each losing bot
            foreach (GameObject temp_curLoseBot in temp_losingBots)
            {
                // Spawn the explosion on top of that bot
                Transform temp_botTrans = temp_curLoseBot.transform;
                GameObject temp_spawnedExplosion = Instantiate(m_explosion,
                    temp_botTrans.position, temp_botTrans.rotation);
                NetworkServer.Spawn(temp_spawnedExplosion);
                // Destroy the bot after a number of seconds (hidden by explosion)
                // Maybe don't do this and instead like, have all its parts fall off
                // or something.
                //DestroyObjAfterSeconds(temp_botHP.gameObject,
                //    m_explosionKillBotTime);
            }
            // Finish after the explosion is completed playing.
            Invoke(nameof(InvokeOnFinished), m_explosionRuntime);
        }
        [Server]
        private void HandleTimeED(GameOverData gameOverData)
        {
            // TODO Add a delay so that we can see text displaying
            // "Time's Up!" for a bit before starting the explosion.
            // Just do the same thing for as health, where we show the
            // bot with lower health dying.
            HandleHealthED(gameOverData);
        }


        [Server]
        private void DestroyObjAfterSeconds(GameObject obj, float lifeTime)
        {
            StartCoroutine(DestroyObjAfterSecondsCoroutine(obj, lifeTime));
        }
        [Server]
        private IEnumerator DestroyObjAfterSecondsCoroutine(GameObject obj,
            float lifeTime)
        {
            yield return new WaitForSeconds(lifeTime);
            NetworkServer.Destroy(obj);
        }
        [Server]
        private void InvokeOnFinished()
        {
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
