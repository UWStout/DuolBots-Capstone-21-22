// Original Author - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Enumerator for controller-centric input options.
    /// </summary>
    public enum eInputType
    {
        buttonEast,         // PS4 - Circle
        buttonNorth,        // PS4 - Triangle
        buttonSouth,        // PS4 - X
        buttonWest,         // PS4 - Square
        dPad,               // Vector2 of dPad's x and y
        dPad_Down,
        dPad_Up,
        dPad_Left,
        dPad_Right,
        dPad_X,             // Left and right on dpad
        dPad_Y,             // Up and down on dpad
        leftShoulder,
        rightShoulder,
        leftTrigger,
        rightTrigger,
        leftStick,
        leftStick_Down,
        leftStick_Up,
        leftStick_Right,
        leftStick_Left,
        leftStick_X,
        leftStick_Y,
        leftStickPress,
        rightStick,
        rightStick_Down,
        rightStick_Up,
        rightStick_Right,
        rightStick_Left,
        rightStick_X,
        rightStick_Y,
        rightStickPress,
        select,             // PS4 - DNE
        start,              // PS4 - DNE
        triggerAxis,        // Left trigger = -1, Right trigger = 1
        shoulderAxis,       // Left shoulder = -1, Right shoulder = 1
        buttons,            // Vector2 of buttons X and Y
        buttons_X,          // West (-1) and East (1) button axis
        buttons_Y           // South (-1) and North (1) button axis
    }
}
