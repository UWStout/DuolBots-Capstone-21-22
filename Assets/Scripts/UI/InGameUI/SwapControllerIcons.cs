using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

using NaughtyAttributes;

namespace DuolBots
{
    public class SwapControllerIcons : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;
        private static readonly Vector2 GAMEPAD_ICON_SIZE
            = new Vector2(120.0f, 120.0f);
        private static readonly Vector2 KEYBOARD_ICON_SIZE
            = new Vector2(80.0f, 80.0f);

        [SerializeField ,Required] private GameObject m_player1Con, m_player2Con;
        [SerializeField, Required] private Image m_player1ArrowLeft, m_player1ArrowRight, m_player2ArrowLeft, m_player2ArrowRight;
        [SerializeField, ShowAssetPreview] private Sprite m_keyLeftArrow, m_keyRightArrow, m_padLeftArrow, m_padRightArrow;
        [SerializeField, ShowAssetPreview] private Sprite m_keyButtonNorth, m_keyButtonSouth, m_keyButtonEast, m_xboxButtonNorth, m_xboxButtonSouth, m_xboxButtonEast;
        [SerializeField, ShowAssetPreview] private Sprite m_psButtonNorth, m_psButtonSouth, m_psButtonEast;
        [SerializeField, Tag] private string m_playerInpTag = "PlayerInput";
        private PlayerInput[] m_controllers = new PlayerInput[0];

        IEnumerator Start()
        {
            yield return new WaitForSeconds(0.1f);
            GameObject[] temp_inpObjs = GameObject.FindGameObjectsWithTag(
                m_playerInpTag);
            m_controllers = new PlayerInput[temp_inpObjs.Length];
            for (int i = 0; i < temp_inpObjs.Length; ++i)
            {
                GameObject temp_curInpObj = temp_inpObjs[i];
                m_controllers[i] = temp_curInpObj.GetComponent<PlayerInput>();
                PlayerIndex temp_index = temp_curInpObj.GetComponent<PlayerIndex>();
                #region Asserts
                CustomDebug.AssertComponentOnOtherIsNotNull(m_controllers[i],
                    temp_curInpObj, this);
                CustomDebug.AssertComponentOnOtherIsNotNull(temp_index,
                    temp_curInpObj, this);
                #endregion Asserts
                SwapIcons(temp_index.playerIndex);
            }
        }

        public void SwapIcons(byte playerIndex)
        {
            PlayerInput temp_playerInp = m_controllers[playerIndex];

            string temp_controlScheme = temp_playerInp.currentControlScheme;

            CustomDebug.LogForComponent($"{nameof(SwapIcons)} for player " +
                $"{playerIndex}. Their control scheme is {temp_controlScheme}",
                this, IS_DEBUGGING);

            GameObject currentControlDisplay;
            Image arrowLeft, arrowRight;

            switch (playerIndex)
            {
                case 0:
                    currentControlDisplay = m_player1Con;
                    arrowLeft = m_player1ArrowLeft;
                    arrowRight = m_player1ArrowRight;
                    break;
                case 1:
                    currentControlDisplay = m_player2Con;
                    arrowLeft = m_player2ArrowLeft;
                    arrowRight = m_player2ArrowRight;
                    break;
                default:
                    Debug.LogError($"Could not handle player index " +
                        $"{playerIndex}", this);
                    currentControlDisplay = m_player1Con;
                    arrowLeft = m_player1ArrowLeft;
                    arrowRight = m_player1ArrowRight;
                    break;
            }

            Image[] temp_IconImages = currentControlDisplay.GetComponentsInChildren<Image>(true);

            if (temp_controlScheme == "Gamepad")
            {
                if (CheckIfDevicesAreXbox(temp_playerInp))
                {
                    #region Logs
                    CustomDebug.LogForComponent($"Player " +
                        $"{temp_playerInp.playerIndex} is using an Xbox Controller",
                        this, IS_DEBUGGING);
                    #endregion Logs
                    foreach (Image image in temp_IconImages)
                    {
                        if (image.name == "ButtonNorthIcon")
                            image.sprite = m_xboxButtonNorth;
                        else if (image.name == "ButtonSouthIcon")
                            image.sprite = m_xboxButtonSouth;
                        else if (image.name == "ButtonEastIcon")
                            image.sprite = m_xboxButtonEast;
                    }
                }
                else
                {
                    #region Logs
                    CustomDebug.LogForComponent($"Player " +
                        $"{temp_playerInp.playerIndex} is using an Playstation " +
                        $"Controller", this, IS_DEBUGGING);
                    #endregion Logs
                    foreach (Image image in temp_IconImages)
                    {
                        if (image.name == "ButtonNorthIcon")
                            image.sprite = m_psButtonNorth;
                        else if (image.name == "ButtonSouthIcon")
                            image.sprite = m_psButtonSouth;
                        else if (image.name == "ButtonEastIcon")
                            image.sprite = m_psButtonEast;
                    }
                }


                arrowLeft.sprite = m_padLeftArrow;
                arrowRight.sprite = m_padRightArrow;
                arrowLeft.rectTransform.sizeDelta = GAMEPAD_ICON_SIZE;
                arrowRight.rectTransform.sizeDelta = GAMEPAD_ICON_SIZE;
            }
            else if (temp_controlScheme.Contains("Keyboard"))
            {
                #region Logs
                CustomDebug.LogForComponent($"Player " +
                    $"{temp_playerInp.playerIndex} is using a Keyboard " +
                    $"Controller", this, IS_DEBUGGING);
                #endregion Logs
                arrowLeft.sprite = m_keyLeftArrow;
                arrowRight.sprite = m_keyRightArrow;
                arrowLeft.rectTransform.sizeDelta = KEYBOARD_ICON_SIZE;
                arrowRight.rectTransform.sizeDelta = KEYBOARD_ICON_SIZE;

                foreach (Image image in temp_IconImages)
                {
                    if (image.name == "ButtonNorthIcon")
                        image.sprite = m_keyButtonNorth;
                    else if (image.name == "ButtonSouthIcon")
                        image.sprite = m_keyButtonSouth;
                    else if (image.name == "ButtonEastIcon")
                        image.sprite = m_keyButtonEast;
                }

            }
        }

        private bool CheckIfDevicesAreXbox(PlayerInput playerInp)
        {
            ReadOnlyArray<InputDevice> temp_devices = playerInp.devices;
            foreach (InputDevice temp_singDevice in temp_devices)
            {
                if (temp_singDevice.name.Contains("XInput"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
