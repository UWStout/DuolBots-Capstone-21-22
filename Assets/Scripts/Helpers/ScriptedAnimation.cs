using System;
using UnityEngine;
// Original Authors - Eslis Vang and Wyatt Senalik

/// <summary>
/// Base class to extend when wanting to make an
/// programmable animation.
/// </summary>
public abstract class ScriptedAnimation : MonoBehaviour
{
    /// <summary>
    /// Starts the animation.
    /// </summary>
    public abstract void Play();

    public event Action onEnd;

    protected void InvokeOnEnd()
    {
        onEnd?.Invoke();
    }
}
