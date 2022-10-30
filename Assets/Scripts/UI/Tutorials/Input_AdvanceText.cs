using System;
using UnityEngine;
using UnityEngine.InputSystem;
// Original Authors - Wyatt Senalik and Ben Lussman

namespace DuolBots
{
    /// <summary>
    /// Input for advancing through text boxes.
    /// </summary>
    public class Input_AdvanceText : MonoBehaviour
    {
        public event Action onAdvanceTextPrompt;
        public event Action onBackTextPrompt;


        #region PlayerInputMessages
        private void OnAdvanceTextPrompt(InputValue value)
        {
            if (!value.isPressed) { return; }
            onAdvanceTextPrompt?.Invoke();
        }
        private void OnBackTextPrompt(InputValue value)
        {
            if (!value.isPressed) { return; }
            onBackTextPrompt?.Invoke();
        }
        #endregion PlayerInputMessages
    }
}
