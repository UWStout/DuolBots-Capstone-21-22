using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ConfirmPanel : MonoBehaviour
{
    [SerializeField] [ReadOnly] PartSelectorManager m_partSelector = null;
    [SerializeField] private int m_buttonIndex = 1;
    [SerializeField] private Button[] m_buttons = null;
    [SerializeField] [ReadOnly] private ColorBlock m_buttonColorBlock;

    private void Awake()
    {
        m_partSelector = PartSelectorManager.instance;
        Assert.IsNotNull(m_partSelector, $"{this.name}: PartSelectorManager singleton is null.");

    }

    private void OnEnable()
    {
        HighlightButton();
    }

    public void Move(InputValue value)
    {
        float temp_xAxis = value.Get<Vector2>().x;
        if (temp_xAxis == 0) { return; }
        DehighlightButton();
        if (temp_xAxis > 0)
        {
            m_buttonIndex = (m_buttonIndex + 1) % 2;
        }
        else if (temp_xAxis < 0)
        {
            m_buttonIndex -= 1;
            if (m_buttonIndex == -1)
            {
                m_buttonIndex = 1;
            }
        }
        HighlightButton();
    }

    public int Select()
    {
        int temp_buttonIndex = m_buttonIndex;
        this.gameObject.SetActive(false);
        DehighlightButton();
        m_buttonIndex = 1;
        return temp_buttonIndex;
    }

    public void Cancel()
    {
        this.gameObject.SetActive(false);
        DehighlightButton();
        m_buttonIndex = 1;
    }

    private void HighlightButton()
    {
        m_buttons[m_buttonIndex].GetComponent<Outline>().enabled = true;
        m_buttonColorBlock = m_buttons[m_buttonIndex].colors;
        m_buttons[m_buttonIndex].colors = new ColorBlock
        {
            normalColor = m_buttonColorBlock.highlightedColor,
            highlightedColor = m_buttonColorBlock.highlightedColor,
            colorMultiplier = 1f,
            fadeDuration = .1f
        };
    }

    private void DehighlightButton()
    {
        m_buttons[m_buttonIndex].GetComponent<Outline>().enabled = false;
        m_buttons[m_buttonIndex].colors = new ColorBlock
        {
            normalColor = m_buttonColorBlock.normalColor,
            highlightedColor = m_buttonColorBlock.highlightedColor,
            colorMultiplier = 1f,
            fadeDuration = .1f
        };
    }
}
