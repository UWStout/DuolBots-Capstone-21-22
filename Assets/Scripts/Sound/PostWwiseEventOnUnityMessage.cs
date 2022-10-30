using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Posts a Wwise event on Start.
    /// </summary>
    public class PostWwiseEventOnUnityMessage : MonoBehaviour
    {
        [SerializeField, Required]
        private WwiseEventName m_wwiseEventNameToInvoke = null;
        [SerializeField] private eUnityMessage m_invokeMessage
            = eUnityMessage.Start;
        [Tooltip("How much time the object spawned to hold the event lives.")]
        [SerializeField,Min(0.0f), ShowIf(nameof(IsMessageOnDestroy))]
        private float m_liveTime = 10.0f;
        private bool m_isQuitting = false;


        private void Start()
        {
            if (m_invokeMessage != eUnityMessage.Start) { return; }
            AkSoundEngine.PostEvent(m_wwiseEventNameToInvoke.wwiseEventName,
                gameObject);
        }
        private void OnApplicationQuit()
        {
            m_isQuitting = true;
        }
        private void OnDestroy()
        {
            if (m_isQuitting) { return; }

            if (m_invokeMessage != eUnityMessage.OnDestroy) { return; }
            GameObject temp_holderObj = new GameObject($"{name}'s " +
                $"{GetType().Name} OnDestroy event holder");
            temp_holderObj.transform.position = transform.position;
            temp_holderObj.transform.rotation = transform.rotation;
            temp_holderObj.transform.localScale = transform.lossyScale;
            AkSoundEngine.PostEvent(m_wwiseEventNameToInvoke.wwiseEventName,
                temp_holderObj);
            Destroy(temp_holderObj, m_liveTime);
        }


        private bool IsMessageOnDestroy() =>
            m_invokeMessage == eUnityMessage.OnDestroy;
    }
}
