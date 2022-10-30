using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots.Tests
{
    public class FindChildrenWithTagTests
    {
        // Must be an actual tag
        private const string TEST_TAG = "Player";


        // A Test behaves as an ordinary method
        [Test]
        public void FixedHierarchyTest()
        {
            // Gen 0
            Transform temp_root = new GameObject("Root").transform;
            // Gen 1
            Transform temp_child0 = new GameObject("Child 0").transform;
            temp_child0.SetParent(temp_root);
            Transform temp_child1 = new GameObject("Child 1").transform;
            temp_child1.SetParent(temp_root);
            Transform temp_child2 = new GameObject("Child 2").transform;
            temp_child2.SetParent(temp_root);
            Transform temp_child3 = new GameObject("Child 3").transform;
            temp_child3.SetParent(temp_root);
            // Gen 2
            Transform temp_gen2Child0 = new GameObject("Child 0-0").transform;
            temp_gen2Child0.SetParent(temp_child0);
            Transform temp_gen2Child1 = new GameObject("Child 0-1").transform;
            temp_gen2Child1.SetParent(temp_child0);
            Transform temp_gen2Child2 = new GameObject("Child 1-0").transform;
            temp_gen2Child2.SetParent(temp_child1);
            Transform temp_gen2Child3 = new GameObject("Child 2-0").transform;
            temp_gen2Child3.SetParent(temp_child2);
            Transform temp_gen2Child4 = new GameObject("Child 3-0").transform;
            temp_gen2Child4.SetParent(temp_child3);
            Transform temp_gen2Child5 = new GameObject("Child 3-1").transform;
            temp_gen2Child5.SetParent(temp_child3);
            // Gen 3
            Transform temp_gen3Child0 = new GameObject("Child 0-0-0").transform;
            temp_gen3Child0.SetParent(temp_gen2Child0);
            Transform temp_gen3Child1 = new GameObject("Child 0-1-0").transform;
            temp_gen3Child1.SetParent(temp_gen2Child1);
            Transform temp_gen3Child2 = new GameObject("Child 1-0-0").transform;
            temp_gen3Child2.SetParent(temp_gen2Child2);
            Transform temp_gen3Child3 = new GameObject("Child 2-0-0").transform;
            temp_gen3Child3.SetParent(temp_gen2Child3);
            Transform temp_gen3Child4 = new GameObject("Child 3-0-0").transform;
            temp_gen3Child4.SetParent(temp_gen2Child4);
            Transform temp_gen3Child5 = new GameObject("Child 3-1-0").transform;
            temp_gen3Child5.SetParent(temp_gen2Child5);
            Transform temp_gen3Child6 = new GameObject("Child 3-1-1").transform;
            temp_gen3Child6.SetParent(temp_gen2Child5);

            // Determine objects that will be tagged
            List<GameObject> temp_taggedObjects = new List<GameObject>()
            {
                // Gen 1
                temp_child2.gameObject,
                // Gen 2
                temp_gen2Child2.gameObject, temp_gen2Child4.gameObject,
                // Gen 3
                temp_gen3Child2.gameObject, temp_gen3Child4.gameObject,
                temp_gen3Child6.gameObject
            };
            // Set tags
            foreach (GameObject temp_singleObj in temp_taggedObjects)
            {
                temp_singleObj.gameObject.tag = TEST_TAG;
            }

            // Do the search
            GameObject[] temp_foundObjects = temp_root.gameObject.
                FindChildrenWithTag(TEST_TAG);
            // Use the Assert class to test conditions.
            Assert.AreEqual(6, temp_foundObjects.Length);
            foreach (GameObject temp_singleObj in temp_foundObjects)
            {
                Assert.IsTrue(temp_taggedObjects.Contains(temp_singleObj));
                // Remove to check against finding the same object multiple times.
                temp_taggedObjects.Remove(temp_singleObj);
            }
        }
        // A Test behaves as an ordinary method
        [Test]
        public void RootTaggedTest()
        {
            Transform temp_root = RandomHierarchy.GenerateRandomHierarchy();
            temp_root.gameObject.tag = TEST_TAG;

            // Do the search
            GameObject[] temp_foundObjects = temp_root.gameObject.
                FindChildrenWithTag(TEST_TAG);

            // Use the Assert class to test conditions.
            Assert.AreEqual(1, temp_foundObjects.Length);
            Assert.IsTrue(temp_foundObjects[0] == temp_root.gameObject);
        }
    }
}
