using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    public class LoadOnlineScene : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField]
        private List<UnityEvent> m_eventList = new List<UnityEvent>();

        private SceneLoader m_sceneLoader = null;
        private bool m_waitingForAsyncLoading = false;


        // Called 0th
        private void Awake()
        {
            m_sceneLoader = SceneLoader.instance;
            Assert.IsNotNull(m_sceneLoader, $"No {nameof(SceneLoader)} found");
        }
        private void OnDisable()
        {
            CustomDebug.Log($"{name}'s {GetType().Name} is being disabled",
                IS_DEBUGGING);

            if (!m_waitingForAsyncLoading)
            {
                m_sceneLoader.EndLoadingScreen();
            }
        }
        private void Update()
        {
            CustomDebug.Log("Waiting for async", IS_DEBUGGING);
            // Hack to fix waiting for async loading on client.
            // Server starts loading scene instantly, but client doesnt
            if (!m_waitingForAsyncLoading) { return; }

            CustomDebug.Log("No longer waiting for async", IS_DEBUGGING);
            AsyncOperation temp_loadingAsyncOp = NetworkManager.loadingSceneAsync;
            if (temp_loadingAsyncOp == null) { return; }

            CustomDebug.Log("ShowingLoadingScreen", IS_DEBUGGING);
            m_sceneLoader.ShowLoadingScreen(temp_loadingAsyncOp);
            m_waitingForAsyncLoading = false;
        }



        public void InvokeEvents(int index)
        {
            Assert.IsFalse(index < 0 || index > m_eventList.Count, $"Invalid " +
                $"index ({index}) given to {name}'s {GetType().Name}'s " +
                $"{nameof(InvokeEvents)}");

            m_sceneLoader.StartLoadingScreen();
            StartCoroutine(InvokeEventsAfterDelayCoroutine(index,
                m_sceneLoader.transitionTime));
        }


        private IEnumerator InvokeEventsAfterDelayCoroutine(int index, float delay)
        {
            Assert.IsFalse(index < 0 || index > m_eventList.Count, $"Invalid " +
                $"index ({index}) given to {name}'s {GetType().Name}'s " +
                $"{nameof(InvokeEventsAfterDelayCoroutine)}");

            yield return new WaitForSeconds(delay);

            UnityEvent temp_events = m_eventList[index];
            temp_events.Invoke();
            m_waitingForAsyncLoading = true;
        }
    }
}
