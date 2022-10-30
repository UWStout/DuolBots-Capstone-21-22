using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DuolBots.Tests
{
    public class SetRecursiveLayerTests
    {
        [Test]
        public void RecursiveLayerTesting()
        {
            Transform layerRoot = RandomHierarchy.GenerateRandomHierarchy();

            // Test with set layer
            GameObjectExtensions.SetLayerRecursively(layerRoot.gameObject, 5);

            foreach (Transform t in layerRoot)
            {
                Assert.AreEqual(t.gameObject.layer, 5);
            }

            // Test with random acceptable layer
            int randomLayer = Random.Range(1, 31);

            GameObjectExtensions.SetLayerRecursively(layerRoot.gameObject, randomLayer);

            foreach (Transform t in layerRoot)
            {
                Assert.AreEqual(t.gameObject.layer, randomLayer);
            }

            // Test with the same change made twice

            GameObjectExtensions.SetLayerRecursively(layerRoot.gameObject, 3);
            GameObjectExtensions.SetLayerRecursively(layerRoot.gameObject, 3);

            foreach (Transform t in layerRoot)
            {
                Assert.AreEqual(t.gameObject.layer, 3);
            }
        }
    }
}
