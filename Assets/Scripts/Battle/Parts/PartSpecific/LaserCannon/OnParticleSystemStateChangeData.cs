using System;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Holds data to be sent over the network for when the
    /// laser's particle system needs to be updated.
    /// </summary>
    [Serializable]
    public class OnParticleSystemStateChangeData
    {
        public bool newState { private set; get; }
        public float curCharge { private set; get; }


        public OnParticleSystemStateChangeData()
        {
            newState = false;
            curCharge = 0;
        }
        public OnParticleSystemStateChangeData(bool state, float charge)
        {
            newState = state;
            curCharge = charge;
        }
    }
}
