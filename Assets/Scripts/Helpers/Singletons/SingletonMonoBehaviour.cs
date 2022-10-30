using UnityEngine;
// Original Authors - Eslis Vang and Wyatt Senalik

/// <summary>
/// Base singleton class for a MonoBehaviour in the scene.
/// Is destroyed on scene transition.
/// Will destroy any additional instances of itself that appear.
/// </summary>
public class SingletonMonoBehaviour<T> : MonoBehaviour where T :
    SingletonMonoBehaviour<T>
{
    /// <summary>
    /// Reference to the current instance of this class.
    /// </summary>
    public static T instance => m_instance;
    private static T m_instance = null;


    // Domestic initialization
    protected virtual void Awake()
    {
        // Set the instance to this unless it already exists.
        if (m_instance != null)
        {
            Debug.Log($"Multiple {GetType().Name} exist in the scene. " +
                $"Destroying the other");
            Destroy(gameObject);
            return;
        }

        m_instance = this as T;
    }
}
