using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Holds a reference to the string for a wwise event so that we don't have to
    /// rely on our spelling in every script, just in the serialize fields on these
    /// ScriptableObjects.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/WwiseEvent",
        fileName = "new WwiseEvent")]
    public class WwiseEventName : ScriptableObject
    {
        [SerializeField] private string m_wwiseEventName = "UNINIT";

        public string wwiseEventName => m_wwiseEventName;
    }
}
