using System;
using System.Collections;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    public class WwiseSoundTest : MonoBehaviour
    {
        private const bool IS_DEBBUGING = true;

        [SerializeField] private WwiseEventWithDelayAfter[] m_wwiseEvents =
            new WwiseEventWithDelayAfter[0];


        private IEnumerator Start()
        {

            for (int i = 0; i < m_wwiseEvents.Length; ++i)
            {
                WwiseEventWithDelayAfter temp_curEv = m_wwiseEvents[i];
                AkSoundEngine.PostEvent(temp_curEv.eventName.wwiseEventName,
                    gameObject);
                CustomDebug.LogForComponent($"Playing {temp_curEv.eventName} " +
                    $"({temp_curEv.eventName.wwiseEventName})", this, IS_DEBBUGING);
                yield return new WaitForSeconds(temp_curEv.waitTime);
            }
        }
    }

    [Serializable]
    public class WwiseEventWithDelayAfter
    {
        [SerializeField] [Required] private WwiseEventName m_eventName = null;
        [SerializeField] [Min(0.0f)] private float m_waitTime = 2.0f;

        public WwiseEventName eventName => m_eventName;
        public float waitTime => m_waitTime;
    }
}
