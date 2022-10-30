// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Interface for controlling the Turret2Axis part.
    /// Should have 2 implementations, one for network and one for local.
    /// </summary>
    public interface IController_Turret2Axis
    {
        /// <summary>
        /// Changes the current rotation input by the given amount.
        /// This change will be applied on Update.
        /// </summary>
        /// <param name="rotInpChangeAmount">Amount to change the rotation input by.</param>
        void ChangeRotationInput(float rotInpChangeAmount);
        /// <summary>
        /// Changes the current raise input by the given amount.
        /// This change will be applied on Update.
        /// </summary>
        /// <param name="raiseInpChangeAmount">Amount to change the raise input by.</param>
        void ChangeRaiseInput(float raiseInpChangeAmount);
    }
}
