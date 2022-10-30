using UnityEngine;
using UnityEngine.Assertions;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Calculates bot weight on the server.
    /// </summary>
    [RequireComponent(typeof(BotWeightCalculator))]
    public class NetworkBotWeightCalculator : NetworkBehaviour
    {
        private BotWeightCalculator m_botWeightCalc = null;


        public override void OnStartServer()
        {
            base.OnStartServer();

            m_botWeightCalc = GetComponent<BotWeightCalculator>();
            Assert.IsNotNull(m_botWeightCalc, $"{GetType().Name} requires " +
                $"{nameof(BotWeightCalculator)} but none was found");

            int temp_totalWeight = m_botWeightCalc.CalculateTotalWeight();
            Assert.IsTrue(temp_totalWeight > 0, $"Bot's weight was calculated to " +
                $"be 0 or less.");
            m_botWeightCalc.SetWeightToMovementPart(temp_totalWeight);
        }
    }
}
