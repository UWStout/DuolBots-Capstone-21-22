using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Assertions;

public class PopupButtonIndicator : MonoBehaviour
{
    [SerializeField] private Image m_image = null;
    [SerializeField] private Sprite[] m_buttonIcons = null;
    [SerializeField] private DetectPlayerDevice m_playerDevice = null;
    [SerializeField] [BoxGroup("Outline")] private Image m_outlineGlow = null;
    [SerializeField] [BoxGroup("Outline")] private Color32 m_outlineGlowColor;
    [SerializeField] [BoxGroup("Outline")] private float m_glowRate = 1.0f;
    [SerializeField] [BoxGroup("Outline")] [ReadOnly] private bool m_reverseGlowAlpha = true;
    [SerializeField] [BoxGroup("Outline")] [ReadOnly] private float m_glowAlphaValue = 255.0f;

    #region UnityMessages
    private void Awake()
    {
        m_image = this.GetComponent<Image>();

        Assert.IsNotNull(m_playerDevice, $"{this.name}: DetectPlayerDevice script is missing or null.");
        if (m_playerDevice == null) { return; }

        if (m_playerDevice.controlScheme == "Gamepad")
        {
            m_image.sprite = m_outlineGlow.sprite = m_buttonIcons[0];
        }
        else if (m_playerDevice.controlScheme == "Keyboard and Mouse")
        {
            m_image.sprite = m_outlineGlow.sprite = m_buttonIcons[1];
        }

        m_outlineGlowColor = m_outlineGlow.color;
    }
    private void Update()
    {

        OutlineGlow();
    } 
    #endregion

    #region UnityFunctions
    private void OutlineGlow()
    {
        if (m_glowAlphaValue >= 255.0f)
        {
            m_reverseGlowAlpha = true;
            m_glowAlphaValue = 254.5f;
        }
        else if (m_glowAlphaValue <= 0.0f)
        {
            m_reverseGlowAlpha = false;
            m_glowAlphaValue = 0.5f;
        }
        if (m_reverseGlowAlpha)
        {
            m_glowAlphaValue -= Mathf.Lerp(0.0f, 255.0f, Time.deltaTime * m_glowRate);
        }
        else
        {
            m_glowAlphaValue += Mathf.Lerp(0.0f, 255.0f, Time.deltaTime * m_glowRate);
        }
        m_outlineGlow.color = new Color32(m_outlineGlowColor.r, m_outlineGlowColor.g, m_outlineGlowColor.b, (byte)m_glowAlphaValue);
    } 
    #endregion
}
