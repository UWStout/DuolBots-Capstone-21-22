using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
// Original Authors - Wyatt Senalik and Eslis Vang

namespace DuolBots
{
    /// <summary>
    /// Input controller to confirm selection for a
    /// <see cref="ISelection_DollyTargetCycler"/>.
    /// </summary>
    public class Input_ISelection_TargetCycler : MonoBehaviour
    {
        private ISelection_DollyTargetCycler m_activeSelector = null;


        public void SetSelector(ISelection_DollyTargetCycler selector)
        {
            m_activeSelector = selector;
        }


        // InputSystem Messages
        private void OnConfirmSelection(InputValue value)
        {
            Assert.IsNotNull(m_activeSelector, $"Input was collected before " +
                $"{nameof(m_activeSelector)} was set in {name}'s {GetType().Name}");

            if (value.isPressed)
            {
                m_activeSelector.ConfirmSelection();
            }
        }
    }
}
