using UnityEngine;
using UnityEngine.Assertions;
// Original Author - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Grabs the weights for all the parts and gives the total weight to the movement parts.
    /// </summary>
    public class BotWeightCalculator : MonoBehaviour
    {
        public enum eCalculateWeightTiming { Start, Manual }

        [SerializeField] private eCalculateWeightTiming m_calculateWeightTiming
            = eCalculateWeightTiming.Start;

        private void Start()
        {
            if (m_calculateWeightTiming != eCalculateWeightTiming.Start)
            {
                return;
            }

            int temp_totalWeight = CalculateTotalWeight();
            SetWeightToMovementPart(temp_totalWeight);
        }


        /// <summary>
        /// Calculates the sum of all the individual part weights.
        ///
        /// Pre Conditions - All parts must have PartWeight attached to them. A chassis and a
        /// movement part at the very least. A chassis and movement part should never have a
        /// combined weight of 0. No part should ever have negative weight.
        /// Post Conditions - Returns the sum of all individual part weights.
        /// </summary>
        public int CalculateTotalWeight()
        {
            PartWeight[] temp_partWeightList = GetComponentsInChildren<PartWeight>();
            Assert.IsTrue(temp_partWeightList.Length >= 2, $"At least 2 part weights should" +
                $" exists. One for chassis and one for movement. Yet only {temp_partWeightList.Length}" +
                $" were found for {name}'s {nameof(BotWeightCalculator)}");

            int temp_totalWeight = 0;
            foreach (PartWeight temp_singlePartWeight in temp_partWeightList)
            {
                temp_totalWeight += temp_singlePartWeight.weight;
                Assert.IsTrue(temp_singlePartWeight.weight >= 0, $"A part should not have negative weight," +
                    $" yet {temp_partWeightList}'s weight is {temp_singlePartWeight.weight}");
            }
            Assert.AreNotEqual(temp_totalWeight, 0, $"Weight calculated for bot was 0" +
                $" for {name}'s {nameof(BotWeightCalculator)}");
            
            return temp_totalWeight;
        }
        /// <summary>
        /// Sets the movement part's weight to be the given weight.
        /// </summary>
        /// <param name="totalWeight"></param>
        public void SetWeightToMovementPart(int totalWeight)
        {
            IMovementWeight temp_movementWeight = GetComponentInChildren<IMovementWeight>();
            Assert.IsNotNull(temp_movementWeight, $"{name}'s {nameof(BotWeightCalculator)} could not" +
                $" find {nameof(IMovementWeight)} in its children");
            temp_movementWeight.SetWeight(totalWeight);
        }
    }
}
