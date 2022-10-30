using System;
using System.Collections;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    public class LoadingScreenController : SingletonMonoBehaviourPersistant<LoadingScreenController>
    {
        [SerializeField] private Animator m_animator = null;
        [SerializeField] [AnimatorParam(nameof(m_animator))]
        private string m_startTriggerParameter = "Start";
        [SerializeField] [AnimatorParam(nameof(m_animator))]
        private string m_endTriggerParameter = "End";

        public event Action<float> onProgressChanged;


        public void ShowLoadingScreen(AsyncOperation asyncLoadingOp)
        {
            StartCoroutine(LoadingCoroutine(asyncLoadingOp));
        }


        private IEnumerator LoadingCoroutine(AsyncOperation asyncLoadingOp)
        {
            m_animator.SetTrigger(m_startTriggerParameter);
            while (!asyncLoadingOp.isDone)
            {
                // Divide by 0.9f because from 0-0.9 is the async loading.
                // The 0.9-1 is the deleting of the last scene.
                onProgressChanged?.Invoke(asyncLoadingOp.progress / 0.9f);
                yield return null;
            }

            m_animator.SetTrigger(m_endTriggerParameter);
        }
    }
}
