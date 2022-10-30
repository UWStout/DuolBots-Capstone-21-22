using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik, Aaron Duffey, and Zachary Gross

/// <summary>
/// Customly wrapped defined calls for Unity's Debug class.
/// </summary>
public static class CustomDebug
{
    /// <summary>
    /// Debug.Log that does not print if isDebugging is true.
    /// </summary>
    public static void Log(object message, bool isDebugging)
    {
        if (!isDebugging) { return; }
        if (!Debug.isDebugBuild) { return; }
        Debug.Log(message);
    }

    /// <summary>
    /// Debug.LogWarning wrapped in #if UNITY_EDITOR
    /// </summary>
    /// <param name="message"></param>
    public static void LogWarning(object message)
    {
        if (!Debug.isDebugBuild) { return; }
        Debug.LogWarning(message);
    }

    public static void LogForComponent(object message, Component requester,
        bool isDebugging)
    {
        Log($"{requester.name}'s {requester.GetType().Name} {message}",
            isDebugging);
    }
    public static void LogForObject(object message, object requester,
       bool isDebugging)
    {
        Log($"{requester.GetType().Name}'s {message}",
            isDebugging);
    }
    #region LogForContainerGameObjects
    /// <summary>
    /// Logs the input message after displaying the contents of a GameObject List.
    /// <code>For example, this function will print out:
    /// [NAME OF THE LIST] contains [x, y, z, ...].
    /// [YOUR MESSAGE HERE].</code>
    /// </summary>
    /// <param name="gameObjects">List of GameObjects to Log.</param>
    /// <param name="isDebugging">Whether this function runs (only runs when debugging is on).</param>
    public static void LogForContainerGameObjects(object message, List<GameObject> gameObjects, bool isDebugging)
    {
        string temp_objNames = "";
        foreach(GameObject go in gameObjects)
        {
            temp_objNames += go + ", ";
        }
        Log($"{gameObjects} contains {temp_objNames}. \n {message}", isDebugging);
    }
    /// <summary>
    /// Logs the input message after displaying the contents of a GameObject IReadOnlyList.
    /// <code>For example, this function will print out:
    /// [NAME OF THE LIST] contains [x, y, z, ...].
    /// [YOUR MESSAGE HERE].</code>
    /// </summary>
    /// <param name="gameObjects">IReadOnlyList of GameObjects to Log.</param>
    /// <param name="isDebugging">Whether this function runs (only runs when debugging is on).</param>
    public static void LogForContainerGameObjects(object message, IReadOnlyList<GameObject> gameObjects, bool isDebugging)
    {
        string temp_objNames = "";
        foreach (GameObject go in gameObjects)
        {
            temp_objNames += go + ", ";
        }
        Log($"{gameObjects} contains {temp_objNames}. \n {message}", isDebugging);
    }
    /// <summary>
    /// Logs the input message after displaying the contents of a GameObject array.
    /// <code>For example, this function will print out:
    /// [NAME OF THE ARRAY contains [x, y, z, ...].
    /// [YOUR MESSAGE HERE].</code>
    /// </summary>
    /// <param name="message">The message displayed in the log after the contents of the array.</param>
    /// <param name="gameObjects">List of GameObjects to Log.</param>
    /// <param name="isDebugging">Whether this function runs (only runs when debugging is on).</param>
    public static void LogForContainerGameObjects(object message, GameObject[] gameObjects, bool isDebugging)
    {
        string temp_objNames = "";
        foreach (GameObject go in gameObjects)
        {
            temp_objNames += go + ", ";
        }
        Log($"{gameObjects} contains {temp_objNames}. \n {message}", isDebugging);
    }
    #endregion LogForContainerGameObjects
    #region LogForContainerObjects
    /// <summary>
    /// Logs the input message after displaying the contents of an object List.
    /// <code>For example, this function will print out:
    /// [NAME OF THE LIST] contains [x, y, z, ...].
    /// [YOUR MESSAGE HERE].</code>
    /// </summary>
    /// <param name="objects">List of objects to Log.</param>
    /// <param name="isDebugging">Whether this function runs (only runs when debugging is on).</param>
    public static void LogForContainerObjects(object message, List<object> objects, bool isDebugging)
    {
        string temp_objNames = "";
        foreach (object obj in objects)
        {
            temp_objNames += obj + ", ";
        }
        Log($"{objects} contains {temp_objNames}. \n {message}", isDebugging);
    }
    /// <summary>
    /// Logs the input message after displaying the contents of an object array.
    /// <code>For example, this function will print out:
    /// [NAME OF THE ARRAY] contains [x, y, z, ...].
    /// [YOUR MESSAGE HERE].</code>
    /// </summary>
    /// <param name="objects">Array of objects to Log.</param>
    /// <param name="isDebugging">Whether this function runs (only runs when debugging is on).</param>
    public static void LogForContainerObjects(object message, object[] objects, bool isDebugging)
    {
        string temp_objNames = "";
        foreach (object obj in objects)
        {
            temp_objNames += obj + ", ";
        }
        Log($"{objects} contains {temp_objNames}. \n {message}", isDebugging);
    }
    #endregion LogForContainerObjects
    #region LogForContainerElements
    /// <summary>
    /// Logs the input message after displaying the contents of a T array.
    /// <code>For example, this function will print out:
    /// [NAME OF THE ARRAY] contains [x, y, z, ...].
    /// [YOUR MESSAGE HERE].</code>
    /// </summary>
    /// <param name="objects">Array of T to Log.</param>
    /// <param name="isDebugging">Whether this function runs (only runs when debugging is on).</param>
    public static void LogForContainerElements<T>(object message, T[] objects, bool isDebugging)
    {
        string temp_objNames = "";
        foreach (T obj in objects)
        {
            temp_objNames += obj + ", ";
        }
        Log($"{objects} contains {temp_objNames}. \n {message}", isDebugging);
    }
    /// <summary>
    /// Logs the input message after displaying the contents of a T List.
    /// <code>For example, this function will print out:
    /// [NAME OF THE LIST] contains [x, y, z, ...].
    /// [YOUR MESSAGE HERE].</code>
    /// </summary>
    /// <param name="objects">T List to Log.</param>
    /// <param name="isDebugging">Whether this function runs (only runs when debugging is on).</param>
    public static void LogForContainerElements<T>(object message, List<T> objects, bool isDebugging)
    {
        string temp_objNames = "";
        foreach (T obj in objects)
        {
            temp_objNames += obj + ", ";
        }
        Log($"{objects} contains {temp_objNames}. \n {message}", isDebugging);
    }
    /// <summary>
    /// Logs the input message after displaying the contents of a T IReadOnlyList.
    /// <code>For example, this function will print out:
    /// [NAME OF THE LIST] contains [x, y, z, ...].
    /// [YOUR MESSAGE HERE].</code>
    /// </summary>
    /// <param name="objects">T IReadOnlyList to Log.</param>
    /// <param name="isDebugging">Whether this function runs (only runs when debugging is on).</param>
    public static void LogForContainerElements<T>(object message, IReadOnlyList<T> objects, bool isDebugging)
    {
        string temp_objNames = "";
        foreach (T obj in objects)
        {
            temp_objNames += obj + ", ";
        }
        Log($"{objects} contains {temp_objNames}. \n {message}", isDebugging);
    }
    #endregion LogForContainerElements

    public static void UnhandledEnum<T>(T enumVal, string querier) where T : Enum
    {
        if (!Debug.isDebugBuild) { return; }
        Debug.LogError($"Unhandled enum of type {enumVal.GetType()} with value " +
            $"{enumVal} in {querier}");
    }
    public static void UnhandledEnum<T>(T enumVal, Component querier) where T : Enum
    {
        if (!Debug.isDebugBuild) { return; }
        Debug.LogError($"Unhandled enum of type {enumVal.GetType()} with value " +
            $"{enumVal} in {querier.name}'s {querier.GetType().Name}");
    }

    #region AssertComponentIsNotNull
    public static void AssertComponentIsNotNull<T>(T varToCheck,
        Component queryComp, GameObject getCompTarget) where T : Component
    {
        AssertComponentIsNotNull(varToCheck, typeof(T), $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}", getCompTarget.name);
    }
    public static void AssertComponentIsNotNull<T>(T varToCheck,
        Component queryComp) where T : Component
    {
        AssertComponentIsNotNull(varToCheck, $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}");
    }
    public static void AssertComponentIsNotNull<T>(T varToCheck,
        Type queryType) where T : Component
    {
        AssertComponentIsNotNull(varToCheck, queryType.Name);
    }
    public static void AssertComponentIsNotNull(object varToCheck,
        Type typeOfVar, Type queryType)
    {
        AssertComponentIsNotNull(varToCheck, typeOfVar, queryType.Name,
            queryType.Name);
    }
    public static void AssertComponentIsNotNull<T>(T varToCheck,
       string querierName) where T : Component
    {
        AssertComponentIsNotNull(varToCheck, typeof(T), querierName, querierName);
    }
    public static void AssertComponentIsNotNull(object varToCheck,
        Type typeOfVar, string querierName, string getCompTargetName)
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.IsNotNull(varToCheck, $"{querierName} expected to have " +
            $"{typeOfVar.Name} attached to {getCompTargetName} but " +
            $"none was found.");
    }
    #endregion AssertComponentIsNotNull

    #region AssertIComponentIsNotNull
    public static void AssertIComponentIsNotNull<T>(T varToCheck,
        Component queryComp) where T : IComponent
    {
        AssertIComponentIsNotNull(varToCheck, $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}");
    }
    public static void AssertIComponentIsNotNull<T>(T varToCheck,
       string querierName) where T : IComponent
    {
        AssertComponentIsNotNull(varToCheck, typeof(T), querierName, querierName);
    }
    #endregion AssertIComponentIsNotNull

    #region AssertComponentOnOtherIsNotNull
    public static void AssertComponentOnOtherIsNotNull<T>(T varToCheck,
        GameObject other, Component queryComp) where T : Component
    {
        AssertComponentIsNotNull(varToCheck, typeof(T), $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}", other.name);
    }
    #endregion AssertComponentOnOtherIsNotNull

    #region AssertIComponentOnOtherIsNotNull
    public static void AssertIComponentOnOtherIsNotNull<T>(T varToCheck,
        GameObject other, Component queryComp) where T : IComponent
    {
        AssertComponentIsNotNull(varToCheck, typeof(T), $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}", other.name);
    }
    #endregion AssertIComponentOnOtherIsNotNull

    #region AssertComponentInChildrenIsNotNull
    public static void AssertComponentInChildrenIsNotNull<T>(T varToCheck,
    Component queryComp, GameObject getCompTarget) where T : Component
    {
        AssertComponentInChildrenIsNotNull(varToCheck, typeof(T), $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}", getCompTarget.name);
    }
    public static void AssertComponentInChildrenIsNotNull<T>(T varToCheck,
        Component queryComp) where T : Component
    {
        AssertComponentInChildrenIsNotNull(varToCheck, $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}");
    }
    public static void AssertComponentInChildrenIsNotNull<T>(T varToCheck,
        Type queryType) where T : Component
    {
        AssertComponentInChildrenIsNotNull(varToCheck, queryType.Name);
    }
    public static void AssertComponentInChildrenIsNotNull(object varToCheck,
        Type typeOfVar, Type queryType)
    {
        AssertComponentInChildrenIsNotNull(varToCheck, typeOfVar, queryType.Name,
            queryType.Name);
    }
    public static void AssertComponentInChildrenIsNotNull<T>(T varToCheck,
       string querierName) where T : Component
    {
        AssertComponentInChildrenIsNotNull(varToCheck, typeof(T), querierName, querierName);
    }
    public static void AssertComponentInChildrenIsNotNull(object varToCheck,
        Type typeOfVar, string querierName, string getCompTargetName)
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.IsNotNull(varToCheck, $"{querierName} expected to have " +
            $"{typeOfVar.Name} attached to {getCompTargetName} or its children but " +
            $"none was found.");
    }
    #endregion AssertComponentInChildrenIsNotNull

    #region AssertComponentInChildrenOnOtherIsNotNull
    public static void AssertComponentInChildrenOnOtherIsNotNull<T>(T varToCheck,
        GameObject other, Component queryComp) where T : Component
    {
        AssertComponentInChildrenIsNotNull(varToCheck, typeof(T), $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}", other.name);
    }
    public static void AssertComponentInChildrenOnOtherIsNotNull<T>(T varToCheck,
    GameObject other, string queryName) where T : Component
    {
        AssertComponentInChildrenIsNotNull(varToCheck, typeof(T), queryName, other.name);
    }
    #endregion AssertComponentInChildrenOnOtherIsNotNull

    #region AssertIComponentInChildrenOnOtherIsNotNull
    public static void AssertIComponentInChildrenOnOtherIsNotNull<T>(T varToCheck,
        GameObject other, Component queryComp) where T : IComponent
    {
        AssertComponentInChildrenIsNotNull(varToCheck, typeof(T), $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}", other.name);
    }
    #endregion AssertIComponentInChildrenOnOtherIsNotNull

    #region AssertComponentInParentIsNotNull
    public static void AssertComponentInParentIsNotNull<T>(T varToCheck,
    Component queryComp, GameObject getCompTarget) where T : Component
    {
        AssertComponentInParentIsNotNull(varToCheck, typeof(T), $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}", getCompTarget.name);
    }
    public static void AssertComponentInParentIsNotNull<T>(T varToCheck,
        Component queryComp) where T : Component
    {
        AssertComponentInParentIsNotNull(varToCheck, $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}");
    }
    public static void AssertComponentInParentIsNotNull<T>(T varToCheck,
        Type queryType) where T : Component
    {
        AssertComponentInParentIsNotNull(varToCheck, queryType.Name);
    }
    public static void AssertComponentInParentIsNotNull(object varToCheck,
        Type typeOfVar, Type queryType)
    {
        AssertComponentInParentIsNotNull(varToCheck, typeOfVar, queryType.Name,
            queryType.Name);
    }
    public static void AssertComponentInParentIsNotNull<T>(T varToCheck,
       string querierName) where T : Component
    {
        AssertComponentInParentIsNotNull(varToCheck, typeof(T), querierName, querierName);
    }
    public static void AssertComponentInParentIsNotNull(object varToCheck,
        Type typeOfVar, string querierName, string getCompTargetName)
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.IsNotNull(varToCheck, $"{querierName} expected to have " +
            $"{typeOfVar.Name} attached to {getCompTargetName} or its parents but " +
            $"none was found.");
    }
    #endregion AssertComponentInParentIsNotNull

    #region AssertIComponentInParentIsNotNull
    public static void AssertIComponentInParentIsNotNull<T>(T varToCheck,
        Component queryComp) where T : IComponent
    {
        AssertComponentInParentIsNotNull(varToCheck,typeof(T), $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}", $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}");
    }
    public static void AssertIComponentInParentOnOtherIsNotNull<T>(T varToCheck,
        GameObject other, Component queryComp) where T : IComponent
    {
        AssertComponentInParentIsNotNull(varToCheck, typeof(T), $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name}", $"{other.name}");
    }
    #endregion AssertIComponentInParentIsNotNull

    #region AssertSingletonMonoBehaviourIsNotNull
    public static void AssertSingletonMonoBehaviourIsNotNull<T>(T varToCheck,
        Component queryComp) where T : MonoBehaviour
    {
        AssertSingletonMonoBehaviourIsNotNull(varToCheck,
            $"{queryComp.name}'s {queryComp.GetType().Name}");
    }
    public static void AssertSingletonMonoBehaviourIsNotNull<T>(T varToCheck,
        string querierName) where T : MonoBehaviour
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.IsNotNull(varToCheck, $"{querierName} expected the " +
            $"singleton {typeof(T).Name} " +
            $"to exist in the scene, but none was found");
    }
    #endregion AssertSingletonMonoBehaviourIsNotNull

    public static void AssertDynamicSingletonMonoBehaviourPersistantIsNotNull<T>
        (T varToCheck, Component queryComp)
        where T : DynamicSingletonMonoBehaviourPersistant<T>
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.IsNotNull(varToCheck, $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name} expected the " +
            $"{nameof(DynamicSingletonMonoBehaviourPersistant<T>)} " +
            $"{typeof(T).Name} to exist (or be created) in the scene, but none " +
            $"was found");
    }
    public static void AssertListsAreSameSize<T, G>(IReadOnlyCollection<T> listOne,
        IReadOnlyCollection<G> listTwo, string listOneName, string listTwoName,
        Component queryComp)
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.AreEqual(listOne.Count, listTwo.Count, $"{listOneName} and " +
            $"{listTwoName} must be the same exact length but are of different " +
            $"lengths {listOne.Count} and {listTwo.Count} respectively for " +
            $"{queryComp.name}'s {queryComp.GetType().Name}.");
    }

    public static void AssertListIsSize<T>(IReadOnlyCollection<T> listOne,
        string listOneName, float expectedSize, Component queryComp)
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.AreEqual(listOne.Count, expectedSize, $"{listOneName} is expected " +
            $"to have {expectedSize} elements but instead had {listOne.Count} " +
            $"for {queryComp.name}'s {queryComp.GetType().Name}.");
    }

    #region AssertIndexIsInRange
    public static void AssertIndexIsInRange<T, G>(int index,
        IReadOnlyCollection<T> collection, G querierComp) where G : Component
    {
        AssertIndexIsInRange(index, collection,
            $"{querierComp.name}'s {querierComp.GetType().Name}");
    }
    public static void AssertIndexIsInRange<T>(int index,
        IReadOnlyCollection<T> collection, string querierName)
    {
        AssertIndexIsInRange(index, 0, collection.Count, querierName);
    }
    /// <param name="lowerBound">Inclusive</param>
    /// <param name="upperBound">Non-inclusive</param>
    public static void AssertIndexIsInRange<G>(int index,
       int lowerBound, int upperBound, G querierComp) where G : Component
    {
        AssertIndexIsInRange(index, lowerBound, upperBound,
            $"{querierComp.name}'s {querierComp.GetType().Name}");
    }
    /// <param name="lowerBound">Inclusive</param>
    /// <param name="upperBound">Non-inclusive</param>
    public static void AssertIndexIsInRange(int index,
       int lowerBound, int upperBound, string querierName)
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.IsTrue(index >= lowerBound && index < upperBound, $"index {index} " +
            $"out of bounds for {querierName}. Expected to be in range " +
            $"[0, {upperBound - 1}]");
    }
    #endregion AssertIndexIsInRange

    public static void AssertSerializeFieldIsNotNull<T>(T varToCheck,
        string nameOfVar, Component queryComp) where T : UnityEngine.Object
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.IsNotNull(varToCheck, $"{queryComp.name}'s " +
            $"{queryComp.GetType().Name} expected to have {nameOfVar}" +
           $" ({typeof(T).Name}) serialized but none was specified");
    }

    public static void AssertIsTrueForComponent(bool condition, string expectation,
        Component queryComp)
    {
        if (!Debug.isDebugBuild) { return; }
        Assert.IsTrue(condition, $"{queryComp.name}'s {queryComp.GetType().Name} " +
            $"expected {expectation}");
    }
}
