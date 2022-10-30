using UnityEngine;
using UnityEngine.Assertions;
// Original Author - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Supposed to be attached to every part.
    /// Just holds a reference to its own scriptable object.
    /// </summary>
    public class PartSOReference : MonoBehaviour
    {
        public PartScriptableObject partScriptableObject => m_partScriptableObject;
        [SerializeField] private PartScriptableObject m_partScriptableObject = null;


        // Domestic Initialization
        private void Awake()
        {
            Assert.IsNotNull(m_partScriptableObject, $"No {nameof(PartScriptableObject)} was specified" +
                $" for {name}'s {nameof(PartSOReference)}");
        }
    }
}
