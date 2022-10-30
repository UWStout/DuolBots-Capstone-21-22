using UnityEngine;
using UnityEngine.Events;
// Original Authors - Wyatt Senalik and Aaron Duffey

public class InvokeUnityEventOnUnityMessage : MonoBehaviour
{
    [SerializeField] private UnityEvent m_eventToInvoke = new UnityEvent();
    [SerializeField]
    private eUnityMessage m_messageToInvokeDuring = eUnityMessage.Start;


    private void Start()
    {
        if (m_messageToInvokeDuring != eUnityMessage.Start) { return; }
        m_eventToInvoke.Invoke();
    }
    private void OnDestroy()
    {
        if (m_messageToInvokeDuring != eUnityMessage.OnDestroy) { return; }
        m_eventToInvoke.Invoke();
    }
}
