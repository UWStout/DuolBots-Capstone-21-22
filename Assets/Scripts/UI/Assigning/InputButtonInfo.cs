using DuolBots;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Icons;

// Original Authors - Cole Woulf and Ben Lussman
public class InputButtonInfo : MonoBehaviour
{
    [SerializeField] private eSpriteRenderingTypes m_renderSpace = eSpriteRenderingTypes.Canvas;
    [SerializeField] private Button m_button;
    [SerializeField] private byte m_isPlayerOne;
    private eInputType m_input;
    private eActionType m_action;
    private byte m_partIndex;
    private string m_partId;

    private GenerateIcons m_genIcons;
    private List<GameObject> m_icons;
    private float m_spacing;
    private ControlUIScriptableObjectImplement m_control;
    //[SerializeField] Image m_image;

    private void Awake()
    {
        m_control = FindObjectOfType<ControlUIScriptableObjectImplement>();
        m_genIcons = GameObject.Find("Part Icon Generation").GetComponentInChildren<GenerateIcons>();
        m_icons = new List<GameObject>();
    }

    /// <summary>
    /// sets which player is player one or player two
    /// </summary>
    /// <param name="isPlayerOne"></param>
    public void SetisPlayerOne(byte isPlayerOne)
    {
        m_isPlayerOne = isPlayerOne;
    }

    /// <summary>
    /// Gets if the player is player one or player two
    /// </summary>
    /// <returns></returns>
    public byte GetisPlayerOne()
    {
        return m_isPlayerOne;
    }

    /// <summary>
    /// Gets the input type assigned on the part/button
    /// </summary>
    /// <returns></returns>
    public eInputType Getinput()
    {
        return m_input;
    }

    /// <summary>
    /// Sets the input wanted on the part/button
    /// </summary>
    /// <param name="input"></param>
    public void Setinput(eInputType input)
    {
        m_input = input;
    }

    public eInputType GetInput()
    {
        return m_input;
    }

    /// <summary>
    /// Gets which action the part uses (analog, vector 1, vector 2)
    /// </summary>
    /// <returns></returns>
    public eActionType Getaction()
    {
        return m_action;
    }

    /// <summary>
    /// Set the action for the part (analog, vector 1, vector 2)
    /// </summary>
    /// <param name="action"></param>
    public void Setaction(eActionType action)
    {
        m_action = action;
    }

    public string GetPartID()
    {
        return m_partId;
    }

    public void SetPartID(string partID)
    {
        m_partId = partID;
    }

    public byte GetPartIndex()
    {
        return m_partIndex;
    }

    public void SetPartIndex(byte index)
    {
        m_partIndex = index;
    }

}
