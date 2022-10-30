// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Interface for a cinematic sequence controller.
    /// </summary>
    public interface ICinematicSequenceController
    {
        public IEventPrimer onFinished { get; }
        public void StartCinematic();
    }
}
