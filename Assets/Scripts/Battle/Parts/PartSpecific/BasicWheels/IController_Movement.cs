using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Zach Gross and Wyatt Senalik

namespace DuolBots
{
    // Allows inheriting classes to set the target power for each side (left or right) of the movement component
    public interface IController_Movement
    {
        void SetLeftTarget(float newTarget);

        void SetRightTarget(float newTarget);
    }
}
