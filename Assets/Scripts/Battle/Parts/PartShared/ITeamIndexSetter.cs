// Original - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Interface for setting the team index of an object.
    /// </summary>
    public interface ITeamIndexSetter : IMonoBehaviour
    {
        public byte teamIndex { set; }
    }
}
