using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


using Mirror;
using NaughtyAttributes;

using DuolBots.Mirror;
// Original Authors - Wyatt Senalik and Ben Lussman

namespace DuolBots
{
    public class HasFlippedMonitorIcons : NetworkBehaviour
    {
        private const string GAMEPAD_CONTROL_SCHEME = "Gamepad";
        private const string KEYBOARD_MOUSE_CONTROL_SCHEME = "Keyboard and Mouse";

        // Used for setting up prompts
        [SerializeField, Required] private Sprite keyboardUp, keyboardDown,
            gamepadUp, gamepadDown;
        [SerializeField, Tag] private string m_playerInpTag = "PlayerInput";
        [SerializeField, Required] private Image m_p1Up = null, m_p1Down = null,
            m_p2Up = null, m_p2Down = null;

        private PlayerInput[] m_controllers = new PlayerInput[2];
        private List<Image> m_activeIcons = new List<Image>();
        private bool m_isOnOrOff = false;


        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            InitializePrompts();
        }


        public void TogglePromptsActive(bool onOrOff, byte teamIndex)
        {
            TogglePromptsActiveClientRpc(onOrOff, teamIndex);
        }


        [ClientRpc]
        private void TogglePromptsActiveClientRpc(bool onOrOff, byte teamIndex)
        {
            // If not this client
            byte temp_myTeamIndex = BattlePlayerNetworkObject.myPlayerInstance.
                teamIndex.teamIndex;
            if (temp_myTeamIndex != teamIndex) { return; }

            if (m_isOnOrOff == onOrOff) { return; }
            m_isOnOrOff = onOrOff;

            if (onOrOff) { SetUpPrompts(); }
            else { DeactivePrompts(); }
        }
        private void InitializePrompts()
        {
            GameObject[] temp_playerInpObjs =
                GameObject.FindGameObjectsWithTag(m_playerInpTag);
            m_controllers = new PlayerInput[temp_playerInpObjs.Length];
            for (int i = 0; i < temp_playerInpObjs.Length; ++i)
            {
                GameObject temp_inpObj = temp_playerInpObjs[i];
                m_controllers[i] = temp_inpObj.GetComponent<PlayerInput>();
                #region Asserts
                CustomDebug.AssertComponentOnOtherIsNotNull(m_controllers[i],
                    temp_inpObj, this);
                #endregion Asserts
            }
            SetPromptIconsToMatchController();
        }
        private void SetPromptIconsToMatchController()
        {
            #region Asserts
            CustomDebug.AssertIndexIsInRange(0, m_controllers, this);
            CustomDebug.AssertIndexIsInRange(1, m_controllers, this);
            #endregion Asserts
            string p1ControlScheme = m_controllers[0].currentControlScheme;
            string p2ControlScheme = m_controllers[1].currentControlScheme;

            switch (p1ControlScheme)
            {
                case GAMEPAD_CONTROL_SCHEME:
                    m_p1Up.sprite = gamepadUp;
                    m_p1Down.sprite = gamepadDown;
                    break;
                case KEYBOARD_MOUSE_CONTROL_SCHEME:
                    m_p1Up.sprite = keyboardUp;
                    m_p1Down.sprite = keyboardDown;
                    break;
                default:
                    Debug.LogError($"Unknown control scheme found for " +
                        $"player 1: {p1ControlScheme}");
                    break;
            }

            switch (p2ControlScheme)
            {
                case GAMEPAD_CONTROL_SCHEME:
                    m_p2Up.sprite = gamepadUp;
                    m_p2Down.sprite = gamepadDown;
                    break;
                case KEYBOARD_MOUSE_CONTROL_SCHEME:
                    m_p2Up.sprite = keyboardUp;
                    m_p2Down.sprite = keyboardDown;
                    break;
                default:
                    Debug.LogError($"Unknown control scheme found for " +
                        $"player 2: {p2ControlScheme}");
                    break;
            }
        }
        /// <summary>
        /// Sets up the icon prompts onscreen
        /// </summary>
        private void SetUpPrompts()
        {
            m_activeIcons.Add(m_p1Up);
            m_activeIcons.Add(m_p1Down);
            m_activeIcons.Add(m_p2Up);
            m_activeIcons.Add(m_p2Down);

            m_p1Up.gameObject.SetActive(true);
            m_p1Down.gameObject.SetActive(true);
            m_p2Up.gameObject.SetActive(true);
            m_p2Down.gameObject.SetActive(true);

            StartCoroutine(IconAnimationCoroutine());
        }
        private IEnumerator IconAnimationCoroutine()
        {
            bool upIsBig = true;
            while (m_activeIcons.Count > 0)
            {
                if (upIsBig)
                {
                    for (int x = 0; x < m_activeIcons.Count; x++)
                    {
                        if (x % 2 == 0)
                        {
                            m_activeIcons[x].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                        }
                        else
                        {
                            m_activeIcons[x].transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                        }
                    }
                    upIsBig = false;
                    yield return new WaitForSeconds(0.4f);
                }
                else
                {
                    for (int x = 0; x < m_activeIcons.Count; x++)
                    {
                        if (x % 2 == 0)
                        {
                            m_activeIcons[x].transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                        }
                        else
                        {
                            m_activeIcons[x].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                        }
                    }
                    upIsBig = true;
                    yield return new WaitForSeconds(0.4f);
                }
            }
        }
        private void DeactivePrompts()
        {
            m_activeIcons.Clear();
            m_p1Up.gameObject.SetActive(false);
            m_p1Down.gameObject.SetActive(false);
            m_p2Up.gameObject.SetActive(false);
            m_p2Down.gameObject.SetActive(false);
        }
    }
}
