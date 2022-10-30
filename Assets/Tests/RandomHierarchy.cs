using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots.Tests
{
    /// <summary>
    /// Generating random hierarchies of gameobjects.
    /// </summary>
    public static class RandomHierarchy
    {
        /// <summary>
        /// Creates a random hierarchy and returns the root of it.
        /// </summary>
        public static Transform GenerateRandomHierarchy(int minAmountGens = 1,
            int maxAmountGens = 6, int minAmountChildren = 1,
            int maxAmountChildren = 11)
        {
            return GenerateRandomHierarchy(out _, minAmountGens, maxAmountGens,
                minAmountChildren, maxAmountChildren);
        }
        /// <summary>
        /// Creates a random hierarchy and returns the root of it.
        /// </summary>
        /// <param name="amountGenerations">Amount of generations the
        /// hierarchy has.</param>
        public static Transform GenerateRandomHierarchy(out int amountGenerations,
            int minAmountGens = 1, int maxAmountGens = 6, int minAmountChildren = 1,
            int maxAmountChildren = 11)
        {
            Transform temp_root = new GameObject("root").transform;

            amountGenerations = Random.Range(minAmountGens, maxAmountGens);
            List<Transform> temp_lastGen = new List<Transform> { temp_root };
            for (int i = 0; i < amountGenerations; ++i)
            {
                List<Transform> temp_curGen = new List<Transform>();
                for (int k = 0; k < temp_lastGen.Count; ++k)
                {
                    int temp_amountChildren = Random.Range(minAmountChildren,
                        maxAmountChildren);
                    Transform temp_curParent = temp_lastGen[k];

                    for (int n = 0; n < temp_amountChildren; ++n)
                    {
                        Transform temp_child = new GameObject($"Gen {i}. " +
                            $"Child {n} of Parent {k}").transform;

                        temp_curGen.Add(temp_child);
                        temp_child.parent = temp_curParent;
                    }
                }
                temp_lastGen = temp_curGen;
            }

            return temp_root;
        }
    }
}
