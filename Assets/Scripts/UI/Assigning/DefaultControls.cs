using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DuolBots;

public class DefaultControls 
{
    private List<ListValue> analogOrder;
    private List<ListValue> Vec1Order;
    private List<ListValue> Vec2Order;

    private List<eInputType> buttonList = new List<eInputType>();

    public DefaultControls()
    {
        analogOrder = new List<ListValue>
            {
                new ListValue(eInputType.buttonEast),
                new ListValue(eInputType.buttonWest),
                new ListValue(eInputType.buttonNorth),
                new ListValue(eInputType.buttonSouth),
                new ListValue(eInputType.dPad_Up),
                new ListValue(eInputType.dPad_Down),
                new ListValue(eInputType.dPad_Left),
                new ListValue(eInputType.dPad_Right),
                new ListValue(eInputType.rightTrigger),
                new ListValue(eInputType.leftTrigger),
                new ListValue(eInputType.rightShoulder),
                new ListValue(eInputType.leftShoulder),
                new ListValue(eInputType.leftStickPress),
                new ListValue(eInputType.rightStickPress)
            };
        Vec1Order = new List<ListValue>
            {
                new ListValue(eInputType.leftStick_Y),
                new ListValue(eInputType.shoulderAxis),
                new ListValue(eInputType.triggerAxis),
                new ListValue(eInputType.dPad_X),
                new ListValue(eInputType.dPad_Y),
                new ListValue(eInputType.buttons_Y),
                new ListValue(eInputType.buttons_X)
            };
        Vec2Order = new List<ListValue>
            {
                new ListValue(eInputType.dPad),
                new ListValue(eInputType.buttons)
            };
    }

    /// <summary>
    /// A function to assign each part in the scene a default control
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public eInputType UseInput(eActionType list)
    {
        // For each of the analog, vector 1, or vector 2 buttons in the list, if the list contains that button then go to the next
        // otherwise add that button to the list and assign it
        eInputType temp = new eInputType();
        switch (list)
        {
            case eActionType.Analog:
                foreach (ListValue lv in analogOrder)
                {
                    if (!buttonList.Contains(lv.InputType))
                    {
                        buttonList.Add(lv.InputType);
                        temp = lv.InputType;
                        ListValue newValue = new ListValue(temp);
                        break;
                    }
                }
                break;
            case eActionType.Vector1:
                foreach (ListValue lv in Vec1Order)
                {
                    if (!buttonList.Contains(lv.InputType))
                    {
                        buttonList.Add(lv.InputType);
                        temp = lv.InputType;
                        ListValue newValue = new ListValue(temp);
                        break;
                    }
                }
                break;
            case eActionType.Vector2:
                foreach (ListValue lv in Vec2Order)
                {
                    if (!buttonList.Contains(lv.InputType))
                    {
                        buttonList.Add(lv.InputType);
                        temp = lv.InputType;
                        ListValue newValue = new ListValue(temp);
                        break;
                    }
                }
                break;
        }
        //Debug.Log(temp.ToString());
        /*
        string result = "";
        foreach (var listMember in buttonList)
        {
            result += listMember.ToString() + "\n";
        }
        Debug.Log(result + "=========================");
        */
        return temp;
    }


    public struct ListValue
    {
        public eInputType InputType;

        public ListValue(eInputType _InputType)
        {
            InputType = _InputType;
        }
    }
}
