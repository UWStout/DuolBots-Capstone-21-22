using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
// Original Author - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// PartInput for a turret with 2 axes (rotation and raising).
    /// RangedWeapon.
    /// </summary>
    [RequireComponent(typeof(PartSOReference))]
    public class Input_Turret2Axis : MonoBehaviour, IPartInput
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        // Unique string id for the turret.
        public string uniqueID => m_turretSOReference.partScriptableObject.partID;
        private PartSOReference m_turretSOReference = null;

        private IController_Turret2Axis m_turretController = null;
        private IWeaponFireController m_weaponFireController = null;

        // Current input values
        private float m_lastRotInpVal = 0.0f;
        private float m_curRotInpVal = 0.0f;
        private float m_lastRaiseInpVal = 0.0f;
        private float m_curRaiseInpVal = 0.0f;


        // Domestic Initialization
        private void Awake()
        {
            m_turretController = GetComponent<IController_Turret2Axis>();
            Assert.IsNotNull(m_turretController, $"{typeof(Input_Turret2Axis).Name} requires" +
                $"{typeof(IController_Turret2Axis).Name} attached, but none was found" +
                $" on GameObject ({name}).");
            m_turretSOReference = GetComponent<PartSOReference>();
            Assert.IsNotNull(m_turretSOReference, $"{typeof(Input_Turret2Axis).Name} requires" +
                $"{typeof(PartSOReference).Name} attached, but none was found" +
                $" on GameObject ({name}).");
            m_weaponFireController = GetComponent<IWeaponFireController>();
            Assert.IsNotNull(m_turretController, $"{typeof(Input_Turret2Axis).Name} requires" +
                 $"{typeof(IWeaponFireController).Name} attached, but none was found" +
                 $" on GameObject ({name}).");
        }


        /// <summary>
        /// Implementation from IPartInput.
        /// Called when input is received that pertains to this part.
        /// When an input pertains to a specific part is specified in RobotInputController.
        ///
        /// Pre Conditions - This was called by RobotInputController. actionIndex is in range [0, 2].
        /// Post Conditions - Handles input for one of the part actions.
        /// </summary>
        /// <param name="actionIndex">Which Turret2Axis action to take. Must be in range [0, 2].</param>
        /// <param name="value">Input Value.</param>
        public void DoPartAction(byte actionIndex, CustomInputData value)
        {
            switch (actionIndex)
            {
                case 0:
                    RotateTurretInput(value);
                    break;
                case 1:
                    RaiseBarrelInput(value);
                    break;
                case 2:
                    FireTurretInput(value);
                    break;
                case 3:
                    AlternateFireTurretInput(value);
                    break;
                default:
                    DebugInvalidActionIndex(actionIndex);
                    break;
            }
        }


        #region ActionInputs
        /// <summary>
        /// Handles input received from DoPartAction for an input that rotates the turret's barrel.
        ///
        /// Pre Conditions - value must be for an axis (float).
        /// Post Conditions - Stores the axis value in the current rotate input.
        /// </summary>
        /// <param name="value">InputValue. Must be for an axis.</param>
        private void RotateTurretInput(CustomInputData value)
        {
            // Expecting an axis input (float)
            m_lastRotInpVal = m_curRotInpVal;
            m_curRotInpVal = value.Get<float>();

            RotateTurret();
        }
        /// <summary>
        /// Handles input received from DoPartAction for an input that raises the turret's barrel.
        /// 
        /// Pre Conditions - value must be for an axis (float).
        /// Post Conditions - Stores the axis value in the current raise input.
        /// </summary>
        /// <param name="value">InputValue. Must be for an axis.</param>
        private void RaiseBarrelInput(CustomInputData value)
        {
            // Expecting an axis input (float)
            m_lastRaiseInpVal = m_curRaiseInpVal;
            m_curRaiseInpVal = value.Get<float>();

            RaiseBarrel();
        }
        /// <summary>
        /// Handles input received from DoPartAction for an input that fires the turret.
        /// 
        /// Pre Conditions - value must be for a button press.
        /// Post Conditions - On a button press, the turret is fired.
        /// </summary>
        /// <param name="value">InputValue. Must be for a button press.</param>
        private void FireTurretInput(CustomInputData value)
        {
            // Expecting a button input
            FireTurret(value.isPressed, value.inputType);
        }
        /// <summary>
        /// Handles input received from DoPartAction for an input that executes alternate firing for the turret.
        /// 
        /// Pre Conditions - value must be for a button press.
        /// Post Conditions - On a button press, the turret is fired.
        /// </summary>
        /// <param name="value">InputValue. Must be for a button press.</param>
        private void AlternateFireTurretInput(CustomInputData value)
        {
            // Expecting a button input
            AlternateFireTurret(value.isPressed, value.inputType);
        }
        #endregion PartActions


        /// <summary>
        /// Rotates the turret by the current input amount.
        ///
        /// Pre Conditions - Assumes m_rotateTrans has been serialized.
        /// Post Conditions - Rotates the turret based on the stored input data.
        /// </summary>
        private void RotateTurret()
        {
            m_turretController.ChangeRotationInput(m_curRotInpVal - m_lastRotInpVal);
        }
        /// <summary>
        /// Raises the barrel of the turret by the current input amount.
        ///
        /// Pre Conditions - Assumes m_raiseTrans has been serialized.
        /// Post Conditions - Rotates the base of the barrel on the stored input data.
        /// </summary>
        private void RaiseBarrel()
        {
            m_turretController.ChangeRaiseInput(m_curRaiseInpVal - m_lastRaiseInpVal);
        }
        /// <summary>
        /// Fires a projectile from the turret.
        ///
        /// Pre Conditions - Assumes m_projectilePrefab and m_projectileSpawnPos have been specified.
        /// Post Conditions - Spawns a projectile that should self propel itself forward.
        /// </summary>
        /// <param name="isPressed">Whether or not the button is pressed.</param>
        private void FireTurret(bool isPressed, eInputType type)
        {
            CustomDebug.Log($"Firing turret", IS_DEBUGGING);
            m_weaponFireController.Fire(isPressed, type);
        }
        /// <summary>
        /// Executes the secondry fire mode for the turret.
        ///
        /// Pre Conditions - Assumes m_projectilePrefab and m_projectileSpawnPos have been specified.
        /// Post Conditions - Spawns a projectile that should self propel itself forward.
        /// </summary>
        /// <param name="isPressed">Whether or not the button is pressed.</param>
        private void AlternateFireTurret(bool isPressed, eInputType type)
        {
            CustomDebug.Log($"Alternate Fire", IS_DEBUGGING);
            m_weaponFireController.AlternateFire(isPressed, type);
        }

        #region Debugging
        /// <summary>
        /// Prints out an error after receiving an invalid action index.
        /// </summary>
        /// <param name="actionIndex">The invalid action index.</param>
        private void DebugInvalidActionIndex(byte actionIndex)
        {
            // Meant to be Debug.LogError and not CustomDebug.
            // We want to see this error always, not just when debugging.
            Debug.LogError($"{name} has recieved an invalid action index of {actionIndex}");
        }
        #endregion Debugging
    }
}
