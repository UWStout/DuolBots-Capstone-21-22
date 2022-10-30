// Original Author - Wyatt Senalik and Zach Gross

namespace DuolBots
{
    // Allows inheriting classes to make calculations based on the total bot weight (and/or store the total bot weight)
    public interface IMovementWeight
    {
        void SetWeight(int totalBotWeight);
    }
}
