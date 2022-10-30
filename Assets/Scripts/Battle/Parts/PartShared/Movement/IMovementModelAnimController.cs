// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Interface for animation specific kinds of movement parts.
    /// </summary>
    public interface IMovementModelAnimController
    {
        /// <summary>
        /// Updates the animation baed on the given movement values.
        /// </summary>
        /// <param name="moveValueArr">Values for each movement model.</param>
        void UpdateMoveValues(params float[] moveValueArr);
    }
}
