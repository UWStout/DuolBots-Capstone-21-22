using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using GameAnalyticsSDK;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    public class SceneLoader : SingletonMonoBehaviourPersistant<SceneLoader>
    {
        private const bool IS_DEBUGGING = false;

        public float transitionTime => m_transitionTime;
        [SerializeField] [Min(0.0f)] private float m_transitionTime = 1.0f;
        [SerializeField] [Required] private Animator m_animator = null;
        [SerializeField] [AnimatorParam(nameof(m_animator))]
        private string m_startTriggerParameter = "Start";
        [SerializeField] [AnimatorParam(nameof(m_animator))]
        private string m_endTriggerParameter = "End";

        public event Action<float> onProgressChanged;

        private bool m_hasLoadScreenStarted = false;
        private bool m_gameAnalyticsInitialized = false;

        public bool isLoading => m_hasLoadScreenStarted;


        public void Start()
        {
            if (!m_gameAnalyticsInitialized)
            {
                GameAnalytics.Initialize();
                m_gameAnalyticsInitialized = true;
            }
        }
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneByNameCoroutine(sceneName));
        }
        public void LoadScene(int sceneIndex)
        {
            StartCoroutine(LoadSceneByIndexCoroutine(sceneIndex));
        }
        public void ShowLoadingScreen(AsyncOperation asyncLoadingOp)
        {
            StartCoroutine(LoadingCoroutine(asyncLoadingOp));
        }
        public void StartLoadingScreen()
        {
            // Only advance to start, if it hasn't been started yet
            if (!m_hasLoadScreenStarted)
            {
                m_animator.SetTrigger(m_startTriggerParameter);
                m_hasLoadScreenStarted = true;
            }
        }
        public void EndLoadingScreen()
        {
            // Only end if we've started
            if (m_hasLoadScreenStarted)
            {
                m_animator.SetTrigger(m_endTriggerParameter);
                m_hasLoadScreenStarted = false;
            }
        }


        private IEnumerator LoadSceneByNameCoroutine(string sceneName)
        {
            StartLoadingScreen();
            yield return new WaitForSeconds(m_transitionTime);
            AsyncOperation temp_asyncOp = SceneManager.LoadSceneAsync(sceneName);
            StartCoroutine(LoadingCoroutine(temp_asyncOp));
        }
        private IEnumerator LoadSceneByIndexCoroutine(int sceneIndex)
        {
            StartLoadingScreen();
            yield return new WaitForSeconds(m_transitionTime);
            AsyncOperation temp_asyncOp = SceneManager.LoadSceneAsync(sceneIndex);
            StartCoroutine(LoadingCoroutine(temp_asyncOp));
        }
        private IEnumerator LoadingCoroutine(AsyncOperation asyncLoadingOp)
        {
            CustomDebug.LogForComponent($"Starting loading screen", this, IS_DEBUGGING);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, SceneManager.GetActiveScene().name);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "LoadingTransition");
            StartLoadingScreen();

            while (asyncLoadingOp != null && !asyncLoadingOp.isDone)
            {
                CustomDebug.LogForComponent($"Loading progress: " +
                    $"{asyncLoadingOp.progress}", this, IS_DEBUGGING);
                // Divide by 0.9f because from 0-0.9 is the async loading.
                // The 0.9-1 is the deleting of the last scene.
                onProgressChanged?.Invoke(asyncLoadingOp.progress / 0.9f);
                yield return null;
            }

            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "LoadingTransition");
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, SceneManager.GetActiveScene().name);
            EndLoadingScreen();
            CustomDebug.LogForComponent($"Ending loading screen", this, IS_DEBUGGING);
        }
    }
}
