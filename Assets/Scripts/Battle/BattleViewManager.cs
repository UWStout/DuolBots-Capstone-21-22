using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    public class BattleViewManager : MonoBehaviour
    {
        [SerializeField] [Required] private BattleStateManager m_stateMan = null;
        [SerializeField] [Required] private BattleCameraSystem m_camSys = null;
        [SerializeField] [Required] private BattleUIManager m_uiMan = null;

        [SerializeField] [Required] private Image m_blackFadeImg = null;
        [SerializeField] [Min(0.0f)] private float m_fadeTime = 1.5f;
        [SerializeField] [Min(0.0f)] private float m_keptFadeTime = 0.25f;

        private BattleStateChangeHandler m_waitingHandler = null;
        private BattleStateChangeHandler m_opHandler = null;
        private BattleStateChangeHandler m_battleHandler = null;
        private BattleStateChangeHandler m_edHandler = null;
        private BattleStateChangeHandler m_gameOverHandler = null;
        private BattleStateChangeHandler m_endHandler = null;


        // Domestic Initialization
        private void Awake()
        {
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_stateMan,
                nameof(m_stateMan), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_camSys,
                nameof(m_camSys), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_uiMan,
                nameof(m_uiMan), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_blackFadeImg,
                nameof(m_blackFadeImg), this);
            #endregion Asserts

            // Create all the state handlers
            // Waiting
            m_waitingHandler = new BattleStateChangeHandler(m_stateMan,
                ActivateWaitingView, DeactivateWaitingView, eBattleState.Waiting);
            // Opening Cinematic
            m_opHandler = new BattleStateChangeHandler(m_stateMan,
               ActivateOPView, DeactivateOPView, eBattleState.OpeningCinematic);
            // Battle
            m_battleHandler = new BattleStateChangeHandler(m_stateMan,
               ActivateBattleView, DeactivateBattleView, eBattleState.Battle);
            // Ending Cinematic
            m_edHandler = new BattleStateChangeHandler(m_stateMan,
               ActivateEDView, DeactivateEDView, eBattleState.EndingCinematic);
            // Game Over
            m_gameOverHandler = new BattleStateChangeHandler(m_stateMan,
               ActivateGameOverView, DeactivateGameOverView, eBattleState.GameOver);
            // End
            m_endHandler = new BattleStateChangeHandler(m_stateMan,
                ActivateEndView, DeactivateEndView, eBattleState.End);
        }
        private void OnDestroy()
        {
            // Toggle all the state handlers inactive
            m_waitingHandler.ToggleActive(false);
            m_opHandler.ToggleActive(false);
            m_battleHandler.ToggleActive(false);
            m_edHandler.ToggleActive(false);
            m_gameOverHandler.ToggleActive(false);
            m_endHandler.ToggleActive(false);
        }


        #region Waiting
        private void ActivateWaitingView()
        {
            m_camSys.ChangeToWaitingCamera();
            m_uiMan.ChangeToWaitingUI();
        }
        private void DeactivateWaitingView() { } // Empty
        #endregion Waiting

        #region Opening Cinematic
        private void ActivateOPView()
        {
            // TODO
        }
        private void DeactivateOPView() { } // Empty
        #endregion Opening Cinematic

        #region Battle
        private void ActivateBattleView()
        {
            m_camSys.ChangeToActiveBattleCameras();
            m_uiMan.ChangeToInGameUI();
        }
        private void DeactivateBattleView() { } // Empty
        #endregion Battle

        #region Ending Cinematic
        private void ActivateEDView()
        {
            m_camSys.ChangeToEDCinematicCamera();
            m_uiMan.ChangeToEDCinematicUI();
        }
        private void DeactivateEDView() { } // Empty
        #endregion Ending Cinematic

        #region GameOver
        private void ActivateGameOverView()
        {
            // TODO
        }
        private void DeactivateGameOverView() { }
        #endregion GameOver

        #region End
        private void ActivateEndView()
        {
            StartFadeInOut();
            m_camSys.ChangeToEndCinematicCamera();
        }
        private void DeactivateEndView() { }
        #endregion End


        private void StartFadeInOut()
        {
            StopCoroutine(nameof(FadeInOutCoroutine));
            StartCoroutine(FadeInOutCoroutine());
        }
        private IEnumerator FadeInOutCoroutine()
        {
            Color temp_origFadeCol = m_blackFadeImg.color;

            // Fade to entirely opaque (a=1)
            float t = 0;
            float temp_halfFadeTime = m_fadeTime * 0.5f;
            float temp_halfFadeTimeInverse = 1 / temp_halfFadeTime;
            while (t < temp_halfFadeTime)
            {
                float temp_curAlpha = t * temp_halfFadeTimeInverse;
                temp_origFadeCol.a = temp_curAlpha;
                m_blackFadeImg.color = temp_origFadeCol;

                t += Time.deltaTime;
                yield return null;
            }
            t = temp_halfFadeTime;
            temp_origFadeCol.a = 1;
            m_blackFadeImg.color = temp_origFadeCol;

            yield return new WaitForSeconds(m_keptFadeTime);

            // Fade to entirely transparent (a=0)
            while (t > 0)
            {
                float temp_curAlpha = t * temp_halfFadeTimeInverse;
                temp_origFadeCol.a = temp_curAlpha;
                m_blackFadeImg.color = temp_origFadeCol;

                t -= Time.deltaTime;
                yield return null;
            }
            temp_origFadeCol.a = 0;
            m_blackFadeImg.color = temp_origFadeCol;
        }
        private void SetFadeAlpha(float alpha)
        {
            alpha = Mathf.Clamp01(alpha);

            Color temp_curCol = m_blackFadeImg.color;
            temp_curCol.a = alpha;
            m_blackFadeImg.color = temp_curCol;
        }
    }
}
