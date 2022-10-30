using System.Collections;
using UnityEngine;

using NaughtyAttributes;

using DuolBots.Mirror;
// Original Authors - ? (Ben?)
// Tweaked by Wyatt

namespace DuolBots.Tutorial
{
    public class ReturnFromTutorials : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField, Required] private GameObject m_completePopup = null;
        [SerializeField, Min(0.0f)] private float m_timeToWait = 5.0f;
        private bool m_isCoroutActive = false;


        public void ReturnFromTutorial()
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(ReturnFromTutorial),
                this, IS_DEBUGGING);
            #endregion Logs
            if (m_isCoroutActive) { return; }

            StartCoroutine(ReturnAfterDisplayingPopup());
        }


        private IEnumerator ReturnAfterDisplayingPopup()
        {
            m_isCoroutActive = true;

            m_completePopup.SetActive(true);

            yield return new WaitForSeconds(m_timeToWait);

            m_completePopup.SetActive(false);

            // Fake the player hitting main menu
            GameOverScreen temp_goScreen = FindObjectOfType<GameOverScreen>(true);
            CustomDebug.AssertIsTrueForComponent(temp_goScreen != null,
                $"Did not find {nameof(GameOverScreen)} in the scene", this);
            temp_goScreen.OnMainMenuButton();

            m_isCoroutActive = false;
        }
    }
}
