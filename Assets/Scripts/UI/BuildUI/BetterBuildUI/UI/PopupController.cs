using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik and Eslis Vang

namespace DuolBots
{
    public class PopupController : MonoBehaviour
    {
        [SerializeField] [Required] private GameObject m_menuObj = null;
        [SerializeField] private string m_popupInputMapName = "";
        [SerializeField] private MultiplayerEventSystem m_eventSystem = null;
        [SerializeField] private GameObject m_firstSelected = null;
        [SerializeField] [Required] private InputMapStack m_currentInputMapStack = null;
        [SerializeField] [Required] private Input_UIEvents m_uiEvents = null;

        [SerializeField] private UnityEvent m_onSubmit = new UnityEvent();
        [SerializeField] private UnityEvent m_onCancel = new UnityEvent();
        [SerializeField] private UnityEvent m_onNavigation = new UnityEvent();

        private bool m_isSubbed = false;

        public event Action<string> onButtonPressed;

        public void Activate()
        {
            m_menuObj.SetActive(true);
            m_currentInputMapStack.SwitchInputMap(m_popupInputMapName);

            ToggleSubscription(true);

            // If none specified, its possibly okay if just using solely
            // on submit, on cancel.
            if (m_eventSystem == null) { return; }
            // Set first to be selected as currently selected
            m_eventSystem.SetSelectedGameObject(m_firstSelected);
        }
        public void Deactive()
        {
            m_menuObj.SetActive(false);
            m_currentInputMapStack.PopInputMap(m_popupInputMapName);

            ToggleSubscription(false);
        }
        public void OnButtonPressed(string buttonIdentifier)
        {
            onButtonPressed?.Invoke(buttonIdentifier);
        }

        private void ToggleSubscription(bool subOrUnsub)
        {
            // Same state, no need to update
            if (m_isSubbed == subOrUnsub) { return; }

            if (m_isSubbed)
            {
                m_uiEvents.onSubmit -= OnSubmit;
                m_uiEvents.onCancel -= OnCancel;
                m_uiEvents.onNavigate -= OnNavigate;
            }
            else
            {
                m_uiEvents.onSubmit += OnSubmit;
                m_uiEvents.onCancel += OnCancel;
                m_uiEvents.onNavigate += OnNavigate;
            }

            m_isSubbed = !m_isSubbed;
        }
        private void OnSubmit(InputValue value)
        {
            m_onSubmit.Invoke();
        }
        private void OnCancel(InputValue value)
        {
            m_onCancel.Invoke();
        }
        private void OnNavigate(InputValue value)
        {
            m_onNavigation.Invoke();
        }
    }
}
