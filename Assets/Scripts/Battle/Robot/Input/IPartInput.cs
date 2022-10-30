using UnityEngine.InputSystem;
// Original Authors - Wyatt Senalik and Zachary Gross

namespace DuolBots
{
    /// <summary>
    /// Interface for parts that want input (movement, most weapons, most utility).
    /// </summary>
    public interface IPartInput : IMonoBehaviour
    {
        /// <summary>
        /// Unique ID to differentiate the part inputs from each other.
        /// </summary>
        public string uniqueID { get; }

        /// <summary>
        /// Called when input for this part is received.
        ///
        /// Pre Conditions - The actionIndex must be within range of the specific
        /// part's available actions.
        /// Post Conditions - The part takes the action for the index specified.
        /// </summary>
        /// <param name="actionIndex">Index of the action to take with this input data.</param>
        /// <param name="value">Input data wrapper class.</param>
        public void DoPartAction(byte actionIndex, CustomInputData value);
    }
}
