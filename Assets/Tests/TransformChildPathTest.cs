using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots.Tests
{
    /// <summary>
    /// Testing the TransformChildPath class to be sure that the int
    /// array translates correctly to the Transform we desired in the
    /// hierarchy.
    /// </summary>
    public class TransformChildPathTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void DefaultConstructorTest()
        {
            TransformChildPath temp_childPath = new TransformChildPath();

            // Use the Assert class to test conditions.
            Assert.IsNotNull(temp_childPath.path, $"Failed " +
                $"{nameof(DefaultConstructorTest)}. Path was null.");
            Assert.AreEqual(0, temp_childPath.path.Count, $"Failed " +
                $"{nameof(DefaultConstructorTest)}. Count was not 0.");
        }
        // A Test behaves as an ordinary method
        [Test]
        public void FullConstructorTest()
        {
            const int SIZE = 5;

            int[] temp_testPath = new int[SIZE] { 5, 4, 3, 2, 1 };
            TransformChildPath temp_childPath = new
                TransformChildPath(temp_testPath);

            // Use the Assert class to test conditions.
            Assert.IsNotNull(temp_childPath.path, $"Failed " +
                $"{nameof(FullConstructorTest)}. Path was null.");
            Assert.AreEqual(SIZE, temp_childPath.path.Count, $"Failed " +
                $"{nameof(FullConstructorTest)}. Count was not {SIZE}.");
            // Compare each element
            for (int i = 0; i < SIZE; ++i)
            {
                Assert.AreEqual(temp_testPath[i], temp_childPath.path[i],
                    $"Failed {nameof(FullConstructorTest)}. Element {i} not match.");
            }
        }
        // A Test behaves as an ordinary method
        [Test]
        public void TraverseEmptyTest()
        {
            TransformChildPath temp_childPath = new TransformChildPath();
            // Generate a random hierachy to traverse.
            Transform temp_randomRoot = RandomHierarchy.GenerateRandomHierarchy();
            Transform temp_value = temp_childPath.Traverse(temp_randomRoot);

            // Use the Assert class to test conditions.
            // Traversing the default (empty) path should return the root of the path.
            Assert.AreEqual(temp_value, temp_randomRoot,
                $"Failed {nameof(TraverseEmptyTest)} The default " +
                $"{nameof(TransformChildPath)}'s " +
                $"{nameof(TransformChildPath.Traverse)} did not return itself.");
        }
        // A Test behaves as an ordinary method
        [Test]
        public void TraverseSetPathTest()
        {
            // root -> 4th child -> 0th child -> 2nd child -> 1st child
            int[] temp_pathData = { 4, 0, 2, 1 };
            TransformChildPath temp_childPath = new TransformChildPath(temp_pathData);

            // Create a hierachy that the path can use (5 gens of 5 children).
            Transform temp_root = new GameObject("").transform;
            List<Transform> temp_curGen = new List<Transform>() { temp_root };
            List<Transform> temp_nextGen = new List<Transform>();
            Transform temp_targetChild = null;
            for (int i = 0; i < 5; ++i)
            {
                foreach (Transform temp_curParent in temp_curGen)
                {
                    for (int k = 0; k < 5; ++k)
                    {
                        Transform temp_newChild = new GameObject(
                            $"{temp_curParent.name} {k}").transform;
                        temp_newChild.SetParent(temp_curParent);
                        temp_nextGen.Add(temp_newChild);

                        // Based on the above naming, we can check if
                        // this child is the one we want.
                        // root -> 4th child -> 0th child -> 2nd child -> 1st child
                        if (temp_newChild.name == " 4 0 2 1")
                        {
                            temp_targetChild = temp_newChild;
                        }
                    }
                }
                temp_curGen.Clear();
                temp_curGen.AddRange(temp_nextGen);
                temp_nextGen.Clear();
            }

            // Traverse the hierarchy
            Transform temp_value = temp_childPath.Traverse(temp_root);

            // Use the Assert class to test conditions.
            Assert.IsNotNull(temp_targetChild, $"Test logic is flawed");
            // Test the traversed value off the value found by the naming convention
            // and the value found based on the Traverse function.
            Assert.AreEqual(temp_value, temp_targetChild,
                $"Failed {nameof(TraverseSetPathTest)} The set " +
                $"{nameof(TransformChildPath)}'s " +
                $"{nameof(TransformChildPath.Traverse)} did not return the " +
                $"expected value.");
        }
        // A Test behaves as an ordinary method
        [Test]
        public void TraverseRandomTest()
        {
            // Generate a random hierachy to traverse.
            Transform temp_root = RandomHierarchy.GenerateRandomHierarchy(out int
                    temp_amountGenerations);

            // Generate a random path based on the hierarchy
            int temp_pathDepth = Random.Range(1, temp_amountGenerations + 1);
            int[] temp_path = new int[temp_pathDepth];
            Transform temp_curParent = temp_root;
            for (int i = 0; i < temp_path.Length; ++i)
            {
                int temp_childIndex = Random.Range(0, temp_curParent.childCount);
                temp_path[i] = temp_childIndex;

                temp_curParent = temp_curParent.GetChild(temp_path[i]);
            }
            // The last temp_curParent is the end of the path.

            TransformChildPath temp_childPath = new TransformChildPath(temp_path);
            Transform temp_child = temp_childPath.Traverse(temp_root);
            // The traversed path should return the end of the path which
            // is the last temp_curParent.
            Assert.AreEqual(temp_child, temp_curParent, $"Failed " +
                $"{nameof(TraverseRandomTest)} {nameof(TransformChildPath)}'s " +
                $"{nameof(TransformChildPath.Traverse)} should have returned " +
                $"{temp_curParent.name} as the outcome, but {temp_child.name}" +
                $" was returned instead for the path {temp_childPath}.");
        }
    }
}
