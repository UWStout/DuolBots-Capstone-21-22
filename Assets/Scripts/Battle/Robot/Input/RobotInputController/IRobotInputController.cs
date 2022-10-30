using System;
using System.Collections.Generic;
// Original Authors - Wyatt Senalik and Ben Lussman

namespace DuolBots
{
    /// <summary>
    /// Interface for a RobotInputController/NetworkRobotInputController
    /// so that either may be used.
    /// </summary>
    public interface IRobotInputController : IMonoBehaviour
    {
        /// <summary>Called by OnPartInput. In network variant, this should be
        /// called only on the client who has authority.</summary>
        event Action<IReadOnlyList<CustomInputBinding>,
            CustomInputData> onPartInput;

        /// <summary>
        /// Accepts input from the player to pass along to the bot.
        /// </summary>
        /// <param name="playerIndex">Which player inputted.</param>
        /// <param name="inputType">What type of input.</param>
        void OnPlayerInput(byte playerIndex, eInputType inputType,
            byte slotIndex, CustomInputData inputValue);
    }
}
