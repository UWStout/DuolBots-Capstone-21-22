using System;
using UnityEngine;
using UnityEngine.InputSystem;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Input events to be called from messages from a PlayerInput.
    /// </summary>
    public class Input_UIEvents : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        public event Action<InputValue> onNavigate;
        public event Action<InputValue> onSubmit;
        public event Action<InputValue> onCancel;


        private void OnNavigate(InputValue value)
        {
            CustomDebug.Log(nameof(OnNavigate), IS_DEBUGGING);
            onNavigate?.Invoke(value);
        }
        private void OnSubmit(InputValue value)
        {
            CustomDebug.Log(nameof(OnSubmit), IS_DEBUGGING);
            onSubmit?.Invoke(value);
        }
        private void OnCancel(InputValue value)
        {
            CustomDebug.Log(nameof(OnCancel), IS_DEBUGGING);
            onCancel?.Invoke(value);
        }
    }
}
