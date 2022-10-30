using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuolBots {
    public class BreakControls
    {
        public List<Vec1ToAnalog> m_Controls = new List<Vec1ToAnalog>(
            new[] {
            new Vec1ToAnalog(eInputType.leftStick_Y,eInputType.leftStick_Up,eInputType.leftStick_Down),
            new Vec1ToAnalog(eInputType.leftStick_X, eInputType.leftStick_Right, eInputType.leftStick_Left),
            new Vec1ToAnalog(eInputType.rightStick_Y,eInputType.rightStick_Up,eInputType.rightStick_Down),
            new Vec1ToAnalog(eInputType.rightStick_X, eInputType.rightStick_Right, eInputType.rightStick_Left),
            new Vec1ToAnalog(eInputType.dPad_Y,eInputType.dPad_Up,eInputType.dPad_Down),
            new Vec1ToAnalog(eInputType.dPad_X,eInputType.dPad_Right,eInputType.dPad_Left),
            new Vec1ToAnalog(eInputType.triggerAxis,eInputType.rightTrigger,eInputType.leftTrigger),
            new Vec1ToAnalog(eInputType.shoulderAxis,eInputType.rightShoulder,eInputType.leftShoulder),
            new Vec1ToAnalog(eInputType.buttons_Y,eInputType.buttonNorth,eInputType.buttonSouth),
            new Vec1ToAnalog(eInputType.buttons_X,eInputType.buttonEast,eInputType.buttonWest)
            }
        );
        public eInputType FindAxis(eInputType value)
        {
            return m_Controls.Find(X => X.Positive == value || X.Negative == value).Vec1;
        }

        public eInputType FindPositive(eInputType value)
        {
            return m_Controls.Find(X => X.Vec1 == value || X.Negative == value).Positive;
        }
        public eInputType FindNegative(eInputType value)
        {
            return m_Controls.Find(X => X.Vec1 == value || X.Positive == value).Negative;
        }


        public struct Vec1ToAnalog
        {
            public Vec1ToAnalog(eInputType axis, eInputType p, eInputType n)
            {
                Vec1 = axis;
                Positive = p;
                Negative = n;
            }

            public override string ToString()
            {
                return $"Vec1: {Vec1}, Pos: {Positive}, Neg: {Negative}";
            }
            [SerializeField]
            public eInputType Vec1;
            [SerializeField]
            public eInputType Positive; // Up & Right
            [SerializeField]
            public eInputType Negative; // Down & Left
        }
    }
}
