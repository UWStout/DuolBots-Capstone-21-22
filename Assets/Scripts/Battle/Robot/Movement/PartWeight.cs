using UnityEngine;
using UnityEngine.Assertions;
// Original Author - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Holds how much weight the part has.
    /// </summary>
    [RequireComponent(typeof(PartSOReference))]
    public class PartWeight : MonoBehaviour
    {
        private PartSOReference m_partSORef = null;

        public int weight => m_weight;
        private int m_weight = 0;


        // Domestic Initialization
        private void Awake()
        {
            m_partSORef = GetComponent<PartSOReference>();
            Assert.IsNotNull(m_partSORef, $"{name}'s {nameof(PartWeight)} requires" +
                $" {nameof(PartSOReference)}, but none was attached");

            m_weight = m_partSORef.partScriptableObject.weight;
        }
    }
}
