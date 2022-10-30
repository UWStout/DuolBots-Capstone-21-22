using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
// Original Authors - Aaron Duffey, Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Interface that handles firing a weapon.
    /// </summary>
    public interface IWeaponFireController
    {
        /// <summary>
        /// Called by Turret2Axis, abstraction of fire. For example, for charging projectiles vs laser projectiles.
        /// Pre Condition - Should be called by Turret2Axis and the input value given is for a button (responds to isPressed).
        /// Post Condition - Unkown (decided by implentator). 
        /// </summary>
        /// <param name="value">Whether the button was pressed. </param>
        /// 
        void Fire(bool value, eInputType type);
        /// <summary>
        /// Called by Turret2Axis, a secondry abstraction of fire. For example, for charging projectiles vs laser projectiles.
        /// Pre Condition - Should be called by Turret2Axis and the input value given is for a button (responds to isPressed).
        /// Post Condition - Unkown (decided by implentator). 
        /// </summary>
        /// <param name="value">Whether the button was pressed. </param>
        void AlternateFire(bool value, eInputType type);
    }
}
