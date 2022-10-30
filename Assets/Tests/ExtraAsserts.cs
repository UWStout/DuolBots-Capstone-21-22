using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
// Original Authors - Wyatt Senalik

/// <summary>
/// Extra functions for Asserts.
/// </summary>
public static class ExtraAsserts
{
    /// <summary>
    /// Asserts that the Vector3s are close enough in value based on the tolerance.
    /// x, y, and z are compared separately.
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="actual"></param>
    /// <param name="tolerance"></param>
    public static void AreClose(Vector3 expected, Vector3 actual,
        float tolerance = 0.1f)
    {
        Assert.That(actual.x, Is.EqualTo(expected.x).Within(tolerance));
        Assert.That(actual.y, Is.EqualTo(expected.y).Within(tolerance));
        Assert.That(actual.z, Is.EqualTo(expected.z).Within(tolerance));
    }
    /// <summary>
    /// Asserts that the Quaternion are close enough in value based on the
    /// tolerance. x, y, z, and w are compared separately.
    /// </summary>
    /// <param name="expected"></param>
    /// <param name="actual"></param>
    /// <param name="tolerance"></param>
    public static void AreClose(Quaternion expected, Quaternion actual,
        float tolerance = 0.1f)
    {
        Assert.That(actual.x, Is.EqualTo(expected.x).Within(tolerance));
        Assert.That(actual.y, Is.EqualTo(expected.y).Within(tolerance));
        Assert.That(actual.z, Is.EqualTo(expected.z).Within(tolerance));
        Assert.That(actual.w, Is.EqualTo(expected.w).Within(tolerance));
    }
}
