using System;
using UnityEngine;
// Original Author - Wyatt Senalik

/// <summary>
/// ScriptableObject that holds the Sprites for a controller.
/// </summary>
[CreateAssetMenu(fileName = "New ControllerImageScheme",
    menuName = "ScriptableObjects/ControllerImageScheme")]
public class ControllerImageScheme : ScriptableObject
{
    [SerializeField] private Buttons m_buttons = null;
    [SerializeField] private DPad m_dPad = null;
    [SerializeField] private Sprite m_leftStick = null;
    [SerializeField] private Sprite m_rightStick = null;
    [SerializeField] private Sprite m_leftShoulder = null;
    [SerializeField] private Sprite m_rightShoulder = null;
    [SerializeField] private Sprite m_leftTrigger = null;
    [SerializeField] private Sprite m_rightTrigger = null;
    [SerializeField] private Sprite m_select = null;
    [SerializeField] private Sprite m_start = null;

    public Buttons buttons => m_buttons;
    public DPad dPad => m_dPad;
    public Sprite leftStick => m_leftStick;
    public Sprite rightStick => m_rightStick;
    public Sprite leftShoulder => m_leftShoulder;
    public Sprite rightShoulder => m_rightShoulder;
    public Sprite leftTrigger => m_leftTrigger;
    public Sprite rightTrigger => m_rightTrigger;
    public Sprite select => m_select;
    public Sprite start => m_start;


    [Serializable]
    public class Buttons
    {
        [SerializeField] private Sprite m_buttonEast = null;
        [SerializeField] private Sprite m_buttonWest = null;
        [SerializeField] private Sprite m_buttonSouth = null;
        [SerializeField] private Sprite m_buttonNorth = null;

        public Sprite buttonEast => m_buttonEast;
        public Sprite buttonWest => m_buttonWest;
        public Sprite buttonSouth => m_buttonSouth;
        public Sprite buttonNorth => m_buttonNorth;
    }
    [Serializable]
    public class DPad
    {
        [SerializeField] private Sprite m_right = null;
        [SerializeField] private Sprite m_left = null;
        [SerializeField] private Sprite m_down = null;
        [SerializeField] private Sprite m_up = null;

        public Sprite right => m_right;
        public Sprite left => m_left;
        public Sprite down => m_down;
        public Sprite up => m_up;
    }
}
