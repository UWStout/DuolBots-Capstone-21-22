using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik and Eslis Vang

namespace DuolBots
{
    /// <summary>
    /// Starts the first selector for the input controller to be the chassis
    /// and movement.
    /// </summary>
    [RequireComponent(typeof(Input_ISelection_TargetCycler))]
    public class ActiveSelectorController : MonoBehaviour
    {
        [SerializeField] [Required]
        private ChassisMoveSelection_ISelection_DollyTargetCycler m_chassisSelector = null;

        private Input_ISelection_TargetCycler m_selectorInput = null;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            m_selectorInput = GetComponent<Input_ISelection_TargetCycler>();
            Assert.IsNotNull(m_selectorInput, $"{name}'s {GetType().Name} " +
                $"requires {nameof(Input_ISelection_TargetCycler)} but none " +
                $"was found.");
        }
        // Called 1st
        // Foreign Initialization
        private void Start()
        {
            m_selectorInput.SetSelector(m_chassisSelector);
        }
    }
}
